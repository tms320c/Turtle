namespace TurtleWorld.Core
{
    /// <summary>
    /// This is fundamental entity by itself. There is no reason to implement higher abstraction.
    /// It does not have an interface on purpose.
    /// </summary>
    public struct Position // it is ValueType for the sake of immutability
    {
        public int X { get; init; }
        public int Y { get; init; }
        public (int, int) Heading { get; init; }

        // Heading is not important in comparision
        public static bool operator ==(Position p1, Position p2) => p1.X == p2.X && p1.Y == p2.Y;
        public static bool operator !=(Position p1, Position p2) => !(p1 == p2);
    }

}
