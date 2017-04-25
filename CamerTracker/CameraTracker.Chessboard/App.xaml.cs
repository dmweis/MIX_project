using System;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using CameraTracker.Camera;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace CameraTracker.Chessboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IConnection _connection;
        private IModel _channel;
        private TrackingService _tracker;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // For now we're just using the server locally, so we can hard-code this hostname
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _tracker = new TrackingService(5, 5);

            Observable
                .FromEventPattern<MarketDisappearedEventArgs>(_tracker, "MarkerDisappeared")
                .Subscribe(evt =>
                {
                    var marker = new
                    {
                        Type = "remove",
                        Id = evt.EventArgs.MarkerId
                    };

                    var message = JsonConvert.SerializeObject(marker);
                    var body = Encoding.UTF8.GetBytes(message);

                    _channel.BasicPublish("marker_update", "", null, body);
                });

            MainWindow mainWindow = new MainWindow(_tracker, _channel);
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _tracker.Dispose();
            _channel.Dispose();
            _connection.Dispose();

            base.OnExit(e);
        }
    }

}
