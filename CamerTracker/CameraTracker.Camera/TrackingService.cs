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
   public class TrackingService
   {
      private NamedWindow _window;
      private Task _readerTask;

      public Dictionary<int, CustomMarker> CurrentlyDetectedMarkers { get; } = new Dictionary<int, CustomMarker>();

      public TrackingService()
      {
         _window = new NamedWindow("Camera preview");
         _readerTask = Task.Factory.StartNew(CameraLoop);
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
               while (true)
               {
                  IplImage image = capture.QueryFrame();
                  var detectedMarkers = detector.Detect(image, cameraMatrix, distortion, markerSize);
                  foreach (Marker detectedMarker in detectedMarkers)
                  {
                     var center = detectedMarker.Center;
                     var customMarker = new CustomMarker(detectedMarker.Id.ToString(), center.X, center.Y);
                     if (CurrentlyDetectedMarkers.ContainsKey(detectedMarker.Id))
                     {
                        CurrentlyDetectedMarkers[detectedMarker.Id] = customMarker;
                     }
                     else
                     {
                        CurrentlyDetectedMarkers.Add(detectedMarker.Id, customMarker);
                     }
                     detectedMarker.Draw(image, Scalar.Rgb(1, 0, 0));
                  }
                  IplImage imageCopy = image.Clone();
                  _window.ShowImage(imageCopy);
                  CV.WaitKey(1);
               }
            }
         }
      }
   }
}
