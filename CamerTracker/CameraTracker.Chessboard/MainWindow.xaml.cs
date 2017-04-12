using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CameraTracker.Chessboard
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private Rectangle _dot = new Rectangle();
      private CameraTracker.Camera.TrackingService _tracker;
      private Timer timer;

      public MainWindow()
      {
         InitializeComponent();
         _tracker = new Camera.TrackingService();
         timer = new Timer();
         timer.AutoReset = true;
         timer.Interval = 50;
         timer.Elapsed += TimerOnElapsed;
         timer.Start();
         _dot.Stroke = Brushes.Red;
         _dot.StrokeThickness = 200;
         MainGrid.Children.Add(_dot);
      }

      private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
      {
         if (_tracker.CurrentlyDetectedMarkers.ContainsKey(69) && _tracker.CurrentlyDetectedMarkers.ContainsKey(0) && _tracker.CurrentlyDetectedMarkers.ContainsKey(1) && _tracker.CurrentlyDetectedMarkers.ContainsKey(2))
         {
            var marker = _tracker.CurrentlyDetectedMarkers[69];
            var zero = _tracker.CurrentlyDetectedMarkers[0];
            var one = _tracker.CurrentlyDetectedMarkers[1];
            var two = _tracker.CurrentlyDetectedMarkers[2];
            Dispatcher.Invoke(() =>
            {
               int x =(int) Map(marker.X, zero.X, one.X, 0, 4);
               int y = (int) Map(marker.Y, zero.Y, two.Y, 0, 4);
               //System.Diagnostics.Debug.WriteLine($"X: {x} Y: {y}");
               Grid.SetColumn(_dot, x);
               Grid.SetRow(_dot, y);
            });
         }
         else
         {
            Dispatcher.Invoke(() =>
            {
               Grid.SetColumn(_dot, 2);
               Grid.SetRow(_dot, 2);
            });
         }
      }

      private static float Map(float value, float inMin, float inMax, float outMin, float outMax)
      {
         return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
      }
   }
}
