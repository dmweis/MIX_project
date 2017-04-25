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
        //_connectionfactory = new ConnectionFactory() { HostName = "localhost" };
        _connectionfactory = new ConnectionFactory() { Uri = @"amqp://ruzmquwb:uSALbfFaxJIyw_wvT_MEWx8o-SBHaeK0@lark.rmq.cloudamqp.com/ruzmquwb" };
        _connection = _connectionfactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare("health_update", "fanout");
        _channel.ExchangeDeclare("location_update", "fanout");
    }

    public void Register(string exchangeName, Action<string> callback)
    {
        if (!_subscribers.ContainsKey(exchangeName))
        {
            print("Registering");
            _channel.ExchangeDeclare(exchangeName, "fanout");
            string queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, exchangeName, "");
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, args) =>
            {
                print("Recieved message " + Encoding.ASCII.GetString( args.Body));
                _messageQueue.Enqueue(args);
            };
            _channel.BasicConsume(queueName, false, consumer);
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
            print("Dequeuing message");
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