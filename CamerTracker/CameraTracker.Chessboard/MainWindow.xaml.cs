using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Reactive.Linq;
using CameraTracker.Camera;
using System.Collections.Generic;
using System.Diagnostics;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace CameraTracker.Chessboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<int, CharacterCard> _activeMarkers = new Dictionary<int, CharacterCard>();

        private List<string> _monsterNames = new List<string>() { "Kyle", "Kenny", "Cartman", "Stan" };
        private int _iterator = 0;

        public MainWindow(TrackingService tracker, IModel channel)
        {
            InitializeComponent();

            string locationQueue = channel.QueueDeclare().QueueName;
            string healthQueue = channel.QueueDeclare().QueueName;

            channel.ExchangeDeclare("marker_update", "fanout");
            channel.ExchangeDeclare("location_update", "fanout");
            channel.ExchangeDeclare("health_update", "fanout");

            channel.QueueBind(locationQueue, "location_update", "");
            channel.QueueBind(healthQueue, "health_update", "");

            var locationConsumer = new EventingBasicConsumer(channel);
            var healthConsumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(locationQueue, true, locationConsumer);
            channel.BasicConsume(healthQueue, true, healthConsumer);

            Observable.FromEventPattern<BasicDeliverEventArgs>(healthConsumer, "Received")
                .Subscribe(evt =>
                {
                    string body = Encoding.UTF8.GetString(evt.EventArgs.Body);
                    var healthUpdate = JsonConvert.DeserializeObject<HealthUpdate>(body);

                    if (healthUpdate != null)
                    {
                        if (_activeMarkers.ContainsKey(healthUpdate.Id))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                _activeMarkers[healthUpdate.Id].Health = healthUpdate.NewHealth;
                            });
                        }

                    }
                });

            Observable.FromEventPattern<BasicDeliverEventArgs>(locationConsumer, "Received")
                .Subscribe(evt =>
                {
                    string body = Encoding.UTF8.GetString(evt.EventArgs.Body);
                    var locationUpdate = JsonConvert.DeserializeObject<LocationUpdate>(body);

                    if (locationUpdate != null)
                    {
                        if (_activeMarkers.ContainsKey(locationUpdate.Id))
                        {
                            var character = _activeMarkers[locationUpdate.Id];
                            Dispatcher.Invoke(() =>
                            {
                                MainGrid.Children.Remove(character);
                                AddToGrid(character, locationUpdate.NewRowLocation, locationUpdate.NewColumnLocation);
                            });
                        }

                    }
                });

            Observable.FromEventPattern<MarkerChangeEventArgs>(tracker, "MarkerChanged")
                .Where(evt => evt.EventArgs.Y > 0)
                .Where(evt => evt.EventArgs.Y != 4 || evt.EventArgs.X > 0)
                .Where(evt => evt.EventArgs.MarkerId > 2)
                .Where(evt =>
                {
                    int row = evt.EventArgs.Y;
                    int column = evt.EventArgs.X;
                    int id = evt.EventArgs.MarkerId;
                    if(!_activeMarkers.ContainsKey(id))
                    {
                        return true;
                    }
                    CharacterCard element = _activeMarkers[id];
                    bool sameAsBefore = false;
                    Dispatcher.InvokeAsync(() =>
                    {
                        bool rowIsSame = Grid.GetRow(element) == row;
                        bool columnIsSame = Grid.GetColumn(element) == column;
                        sameAsBefore = rowIsSame && columnIsSame;
                    }).Wait();
                    return !sameAsBefore;
                })
                .Subscribe(evt =>
                {
                    int id = evt.EventArgs.MarkerId;
                    int row = evt.EventArgs.Y;
                    int column = evt.EventArgs.X;

                    Dispatcher.Invoke(() =>
                    {
                        CharacterCard monster;
                        if (_activeMarkers.ContainsKey(id))
                        {
                            monster = _activeMarkers[id];
                            MainGrid.Children.Remove(monster);
                        }
                        else
                        {
                            monster = new CharacterCard()
                            {
                                Color = Brushes.Red,
                                CharacterName = GetNextMonsterName(),
                                Health = 100
                            };
                        }

                        _activeMarkers[id] = monster;
                        AddToGrid(monster, row, column);
                        Debug.WriteLine($"Id: {id} moved to Column: {column} Row: {row}");
                    });

                    var marker = new
                    {
                        Type = "update",
                        Id = evt.EventArgs.MarkerId,
                        Column = evt.EventArgs.X,
                        Row = evt.EventArgs.Y
                    };

                    var message = JsonConvert.SerializeObject(marker);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("marker_update", "", null, body);
                });

            Observable.FromEventPattern<MarketDisappearedEventArgs>(tracker, "MarkerDisappeared")
                .Subscribe(evt =>
                {
                    int id = evt.EventArgs.MarkerId;

                    if (_activeMarkers.ContainsKey(id))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MainGrid.Children.Remove(_activeMarkers[id]);
                        });
                    }
                    Debug.WriteLine($"Id: {id} removed");
                });

            CharacterCard player = new CharacterCard()
            {
                CharacterName = "Player",
                Health = 100,
                Color = Brushes.Green
            };

            _activeMarkers[-1] = player;

            AddToGrid(player, 2, 2);
        }

        private string GetNextMonsterName()
        {
            return _monsterNames[_iterator++ % _monsterNames.Count];
        }

        private void AddToGrid(UIElement element, int row, int column)
        {
            MainGrid.Children.Add(element);
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
        }
    }
}
