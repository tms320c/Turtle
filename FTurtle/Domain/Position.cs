namespace FTurtle.Domain
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
    }
}
