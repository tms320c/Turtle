namespace TurtleWorld.Core
{
    public interface IBoard
    {
        public int Width { get; } // Y span
        public int Height { get; } // X span
        public Position Target { get; } // Exit position
        public bool IsInside(int x, int y);
        public bool IsInside(Position position);
        bool HasMine(Position position);
        bool HasMine(int x, int y);
        void AddMine(Position position);
        void AddMine(int x, int y);
    }
}
