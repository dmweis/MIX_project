using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aruco.Net;
using OpenCV.Net;

namespace CamerTracker.Console
{
   class Program
   {
      private static NamedWindow _window;
      static void Main(string[] args)
      {
         _window = new NamedWindow("Single camera window", WindowFlags.KeepRatio);
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
                     detectedMarker.Draw(image, Scalar.Rgb(1,0,0));
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
