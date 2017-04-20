using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using UnityEngine;

public class NetworkHandler : MonoBehaviour
{

   private ConnectionFactory _connectionfactory;
   private IConnection _connection;
   private IModel _channel;
   private readonly Queue<BasicDeliverEventArgs> _messageQueue = new Queue<BasicDeliverEventArgs>();
   private readonly Dictionary<string, List<Action<RabbitMqMessage>>> _subscribers = new Dictionary<string, List<Action<RabbitMqMessage>>>();

   private readonly object _lock = new object();

	// Use this for initialization
	void Start ()
	{
	   _connectionfactory = new ConnectionFactory() {HostName = "localhost"};
	   _connection = _connectionfactory.CreateConnection();
	   _channel = _connection.CreateModel();
      Subscribe();
	}

   private void Subscribe()
   {
      string exchangeName = "MIX_game";
      string routingKey = "*.unity";
      _channel.ExchangeDeclare(exchangeName, "topic");
      string queueName = _channel.QueueDeclare().QueueName;
      _channel.QueueBind(queueName, exchangeName, routingKey);
      var consumer = new EventingBasicConsumer(_channel);
      consumer.Received += (sender, args) =>
      {
         lock (_lock)
         {
            _messageQueue.Enqueue(args);
         }
      };
   }

   public void Register(string id, Action<RabbitMqMessage> callback)
   {
      if (_subscribers.ContainsKey(id))
      {
         _subscribers.Add(id, new List<Action<RabbitMqMessage>>());
      }
      _subscribers[id].Add(callback);
   }

	// Update is called once per frame
	void Update () {
	   lock (_lock)
	   {
	      foreach (BasicDeliverEventArgs messageArgs in _messageQueue)
	      {
	         string data = Encoding.UTF8.GetString(messageArgs.Body);
	         RabbitMqMessage message = JsonUtility.FromJson<RabbitMqMessage>(data);
	         if (_subscribers.ContainsKey(message.Key))
	         {
	            foreach (Action<RabbitMqMessage> callback in _subscribers[message.Key])
	            {
	               callback?.Invoke(message);
	            }
	         }
	      }
	   }
	}
}

[Serializable]
public class RabbitMqMessage
{
   public string Key;
   public string Message;
   private object _cache;

   public T GetObject<T>()
   {
      if (_cache is T)
      {
         return (T) _cache;
      }
      T instance = JsonUtility.FromJson<T>(Message);
      _cache = instance;
      return instance;
   }
}
