using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Reactive;
using System.Reactive.Linq;
using CameraTracker.Camera;
using System.Collections.Generic;
using System.Diagnostics;

namespace CameraTracker.Chessboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<int, Rectangle> _activeMarkers = new Dictionary<int, Rectangle>();
        private TrackingService _tracker;

        public MainWindow()
        {
            InitializeComponent();

            _tracker = new TrackingService(5, 5);

            IObservable<EventPattern<MarkerChangeEventArgs>> markerChange = Observable.FromEventPattern<MarkerChangeEventArgs>(_tracker, "MarkerChanged");
            IObservable<EventPattern<MarketDisappearedEventArgs>> markerRemove = Observable.FromEventPattern<MarketDisappearedEventArgs>(_tracker, "MarkerDisappeared");

            markerChange.Subscribe(evt =>
            {
                int id = evt.EventArgs.MarkerId;
                int row = evt.EventArgs.Y;
                int column = evt.EventArgs.X;

                Dispatcher.Invoke(() =>
                {
                    var rectangle = new Rectangle { Stroke = Brushes.Red, StrokeThickness = 200 };

                    if (_activeMarkers.ContainsKey(id))
                    {
                        MainGrid.Children.Remove(_activeMarkers[id]);
                    }

                    _activeMarkers[id] = rectangle;
                    MainGrid.Children.Add(rectangle);
                    Grid.SetColumn(rectangle, column);
                    Grid.SetRow(rectangle, row);
                    Debug.WriteLine($"Column: {column} Row: {row}");
                });
                // Do RabbitMQ stuff
            });

            markerRemove.Subscribe(evt =>
            {
                int id = evt.EventArgs.MarkerId;

                if (_activeMarkers.ContainsKey(id))
                {
                    Dispatcher.Invoke(() =>
                    {
                        MainGrid.Children.Remove(_activeMarkers[id]);
                    });
                }
                // Do RabbitMQ stuff
            });

        }
    }
}
