using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aruco.Net;
using OpenCV.Net;

namespace CameraTracker.Camera
{
   public class TrackingService : IDisposable
   {
      private NamedWindow _window;
      private Task _readerTask;

      private int _chessBoardWidth;
      private int _chessBoardHeight;
      private bool _KeppRunning = true;

      private int _detectionTimeout = 2000;

      private MarkerPosition _topLeft = new MarkerPosition();
      private MarkerPosition _topRight = new MarkerPosition();
      private MarkerPosition _bottomLeft = new MarkerPosition();

      private List<TrackableMarker> _detectedmarkers = new List<TrackableMarker>();

      public event EventHandler<MarkerChangeEventArgs> MarkerChanged;
      public event EventHandler<MarketDisappearedEventArgs> MarkerDisappeared;

      public TrackingService(int chessBoardWidth, int chessBoardHeight)
      {
         _chessBoardHeight = chessBoardHeight;
         _chessBoardWidth = chessBoardWidth;
         _window = new NamedWindow("Camera preview");
         _readerTask = Task.Run((Action)CameraLoop);
      }

      private void CameraLoop()
      {
         var parameters = new CameraParameters();
         Size size;
         var cameraMatrix = new Mat(3, 3, Depth.F32, 1);
         var distortion = new Mat(1, 4, Depth.F32, 1);
         parameters.CopyParameters(cameraMatrix, distortion, out size);
         using (var detector = new MarkerDetector())
         {
            detector.ThresholdMethod = ThresholdMethod.AdaptiveThreshold;
            detector.Param1 = 7.0;
            detector.Param2 = 7.0;
            detector.MinSize = 0.04f;
            detector.MaxSize = 0.5f;
            detector.CornerRefinement = CornerRefinementMethod.Lines;
            var markerSize = 10;
            using (var capture = Capture.CreateCameraCapture(0))
            {
               while (_KeppRunning)
               {
                  IplImage image = capture.QueryFrame();
                  var detectedMarkers = detector.Detect(image, cameraMatrix, distortion, markerSize);
                  foreach (Marker detectedMarker in detectedMarkers)
                  {
                     // for each marker
                     var center = detectedMarker.Center;
                     detectedMarker.Draw(image, Scalar.Rgb(1, 0, 0));
                     var trackableMarker = new TrackableMarker(detectedMarker.Id, center.X, center.Y);
                     if (trackableMarker.Id == 0)
                     {
                        _topLeft.X = trackableMarker.X;
                        _topLeft.Y = trackableMarker.Y;
                        continue;
                     }
                     if (trackableMarker.Id == 1)
                     {
                        _topRight.X = trackableMarker.X;
                        _topRight.Y = trackableMarker.Y;
                        continue;
                     }
                     if (trackableMarker.Id == 2)
                     {
                        _bottomLeft.X = trackableMarker.X;
                        _bottomLeft.Y = trackableMarker.Y;
                        continue;
                     }
                     trackableMarker.ChessX = (int)Map(trackableMarker.X, _topLeft.X, _topRight.X, 0, _chessBoardWidth);
                     trackableMarker.ChessY = (int)Map(trackableMarker.Y, _topLeft.Y, _bottomLeft.Y, 0, _chessBoardHeight);
                     var oldTrackable = _detectedmarkers.FirstOrDefault(marker => marker.Id == trackableMarker.Id);
                     if (oldTrackable.ChessX != trackableMarker.ChessX || oldTrackable.ChessY != trackableMarker.ChessY)
                     {
                        _detectedmarkers.Remove(oldTrackable);
                        _detectedmarkers.Add(trackableMarker);
                        MarkerChanged?.Invoke(this, new MarkerChangeEventArgs(trackableMarker.Id, trackableMarker.ChessX, trackableMarker.ChessY));
                     }
                  }
                  foreach (var marker in _detectedmarkers)
                  {
                     if (marker.LastDetected.AddMilliseconds(_detectionTimeout) < DateTime.Now)
                     {
                        MarkerDisappeared?.Invoke(this, new MarketDisappearedEventArgs(marker.Id));
                     }
                  }
                  IplImage imageCopy = image.Clone();
                  _window.ShowImage(imageCopy);
                  CV.WaitKey(1);
               }
            }
         }
      }

      public void Dispose()
      {
         _KeppRunning = false;
      }

      private static float Map(float value, float inMin, float inMax, float outMin, float outMax)
      {
         return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
      }
   }
}
