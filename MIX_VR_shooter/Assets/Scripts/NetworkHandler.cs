using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UnityEngine;
using System.Collections.Concurrent;

public class NetworkHandler : MonoBehaviour
{
    private ConnectionFactory _connectionfactory;
    private IConnection _connection;
    private IModel _channel;
    private readonly ConcurrentQueue<BasicDeliverEventArgs> _messageQueue = new ConcurrentQueue<BasicDeliverEventArgs>();
    private readonly Dictionary<string, List<Action<string>>> _subscribers = new Dictionary<string, List<Action<string>>>();

    // Use this for initialization
    void Start()
    {
        _connectionfactory = new ConnectionFactory() { HostName = "localhost" };
        _connection = _connectionfactory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Register(string exchangeName, Action<string> callback)
    {
        if (!_subscribers.ContainsKey(exchangeName))
        {
            _channel.ExchangeDeclare(exchangeName, "fanout");
            _channel.QueueBind(_channel.QueueDeclare().QueueName, exchangeName, "");
            new EventingBasicConsumer(_channel).Received += (sender, args) => _messageQueue.Enqueue(args);
            _subscribers[exchangeName] = new List<Action<string>>();
        }
        _subscribers[exchangeName].Add(callback);
    }

   public void SendMessage(object message, string exchange)
   {
      string json = JsonUtility.ToJson(message);
      byte[] data = Encoding.UTF8.GetBytes(json);
      _channel.BasicPublish(exchange, string.Empty, null, data);
   }

    // Update is called once per frame
    void Update()
    {

        while (_messageQueue.Count > 0)
        {
            BasicDeliverEventArgs messageArgs;
            bool isDequeueSuccessful = _messageQueue.TryDequeue(out messageArgs);
            if (isDequeueSuccessful)
            {
                string exchangeName = messageArgs.Exchange;
                string jsonMessage = Encoding.UTF8.GetString(messageArgs.Body);
                if (_subscribers.ContainsKey(exchangeName))
                {
                    foreach (Action<string> callback in _subscribers[exchangeName])
                    {
                        if (callback != null)
                        {
                            callback.Invoke(jsonMessage);
                        }
                    }
                }
            }
        }
    }
}