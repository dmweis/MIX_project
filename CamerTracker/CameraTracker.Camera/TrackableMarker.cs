using System;

namespace CameraTracker.Camera
{
   public class TrackableMarker
   {
      public int Id { get; }
      public float X { get; set; }
      public float Y { get; set; }
      public int ChessX { get; set; }
      public int ChessY { get; set; }
      public DateTime LastDetected { get; set; }

      public TrackableMarker(int id, float x, float y)
      {
         Id = id;
         X = x;
         Y = y;
         LastDetected = DateTime.Now;
      }
   }
}
