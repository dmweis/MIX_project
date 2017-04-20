using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraTracker.Camera
{
   public class TrackableMarker
   {
      public string Name { get; }
      public float X { get; }
      public float Y { get; }

      public TrackableMarker(string name, float x, float y)
      {
         Name = name;
         X = x;
         Y = y;
      }
   }
}
