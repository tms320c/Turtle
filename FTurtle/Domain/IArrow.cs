namespace FTurtle.Domain
{
    public interface IArrow
    {
        IArrow RotateRight();
        IArrow RotateLeft();
        (int, int) Head { get; set; }
        int X { get; }
        int Y { get; }
    }
}
