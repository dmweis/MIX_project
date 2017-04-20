namespace CameraTracker.Camera
{
   public class MarketDisappearedEventArgs
   {
      public int MarkerId { get; set; }

      public MarketDisappearedEventArgs(int markerId)
      {
         MarkerId = markerId;
      }
   }
}
