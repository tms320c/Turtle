namespace FTurtle.Domain
{
    public struct Position // it is ValueType for the sake of immutability
    {
        public int X { get; init; }
        public int Y { get; init; }
        public (int, int) Heading { get; init; }
    }
}
