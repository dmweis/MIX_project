namespace CameraTracker.Chessboard
{
    public class GameCharacter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int ColumnLocation { get; set; }
        public int RowLocation { get; set; }

        public GameCharacter(int id, string name, int startingColumnLocation, int startingRowLocation, int startingHealth = 100)
        {
            Id = id;
            Name = name;
            Health = startingHealth;
            ColumnLocation = startingColumnLocation;
            RowLocation = startingRowLocation;
        }
    }
}
