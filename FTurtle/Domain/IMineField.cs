namespace FTurtle.Domain
{
    public interface IMineField
    {
        void SetMine(Position position);
        bool HasMine(Position position);
        // It is too dangerous to disarm a mine. Thus, there is no RemoveMine method.
    }
}