using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CamerTracker.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnection _connection;
            IModel _channel;

            //var factory = new ConnectionFactory { HostName = "localhost" };
            var factory = new ConnectionFactory() { Uri = @"amqp://ruzmquwb:uSALbfFaxJIyw_wvT_MEWx8o-SBHaeK0@lark.rmq.cloudamqp.com/ruzmquwb" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("health_update", "fanout");
            _channel.ExchangeDeclare("location_update", "fanout");
            _channel.ExchangeDeclare("marker_update", "fanout");

            Console.WriteLine("Welcome to Testomatic");
            bool keepRunning = true;
            while(keepRunning)
            {
                Console.WriteLine("Choose desired action");
                Console.WriteLine("1) Hurt Character");
                Console.WriteLine("2) Move Character");
                Console.WriteLine("3) Move Character for game");
                Console.WriteLine("0) End");
                int choice = int.Parse(Console.ReadLine());

                switch(choice)
                {
                    case 1:
                        Console.WriteLine("Enter id: ");
                        int id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter new health: ");
                        int newHealth = int.Parse(Console.ReadLine());
                        _channel.BasicPublish("health_update", "", null, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { Id = id, NewHealth = newHealth })));
                        break;
                    case 2:
                        Console.WriteLine("Enter id: ");
                        id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter new row: ");
                        int newRowLocation = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter new column: ");
                        int newColumnLocation = int.Parse(Console.ReadLine());
                        _channel.BasicPublish("location_update", "", null, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { Id = id, NewRowLocation = newRowLocation, NewColumnLocation = newColumnLocation })));
                        break;
                    case 3:
                        Console.WriteLine("Enter id: ");
                        id = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter new row: ");
                        newRowLocation = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter new column: ");
                        newColumnLocation = int.Parse(Console.ReadLine());
                        _channel.BasicPublish("marker_update", "", null, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { Type = "update", Id = id, Row = newRowLocation, Column = newColumnLocation })));
                        break;
                    case 0:
                    default:
                        keepRunning = false;
                        break;
                }
            }
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
