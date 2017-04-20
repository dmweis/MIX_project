namespace CameraTracker.Camera
{
   public class MarkerChangeEventArgs
   {
      public int MarkerId { get; }
      public int X { get; set; }
      public int Y { get; set; }

      public MarkerChangeEventArgs(int markerId, int x, int y)
      {
         MarkerId = markerId;
         X = x;
         Y = y;
      }
   }
}
