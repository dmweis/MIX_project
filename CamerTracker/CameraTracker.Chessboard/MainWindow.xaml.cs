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

            Observable.FromEventPattern<MarkerChangeEventArgs>(tracker, "MarkerChanged")
                .SkipWhile(evt => evt.EventArgs.Y <= 0 || evt.EventArgs.Y >= 4)
                .SkipWhile(evt => evt.EventArgs.MarkerId <= 2)
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
                                Name = GetNextMonsterName(),
                                Health = 100
                            };
                        }

                        _activeMarkers[id] = monster;
                        AddToGrid(monster, row, column);
                        Debug.WriteLine($"Id: {id} moved to Column: {column} Row: {row}");
                    });
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
