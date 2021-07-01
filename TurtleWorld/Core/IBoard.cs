namespace TurtleWorld.Core
{
    public interface IBoard
    {
        public int Width { get; } // Y span
        public int Height { get; } // X span
        bool HasMine(Position position);
        bool HasMine(int x, int y);
        void AddMine(Position position);
        void AddMine(int x, int y);
    }
}
