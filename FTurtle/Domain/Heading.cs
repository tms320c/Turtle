namespace FTurtle.Domain
{
    public ref struct Heading
    {
        // The coordinate system:
        // X coordinate direction from North (top side) to South (bottom side)
        // Y coordinate direction from West (left side) to East (right side)
        // The headings are unit vectors in this coordinate system
        public static readonly (int, int) North = (-1, 0);
        public static readonly (int, int) West = (0, -1);
        public static readonly (int, int) South = (1, 0);
        public static readonly (int, int) East = (0, 1);
    }
}
