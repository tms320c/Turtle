using System.Collections.Generic;
using FTurtle.Domain;

namespace FTurtle.Application
{
    public sealed class MineField : IMineField
    {
        private readonly ISet<Position> _mines;

        public MineField() : this(null)
        {}

        public MineField(ISet<Position> mines)
        {
            _mines = mines ?? new HashSet<Position>();
        }

        public void SetMine(Position position)
        {
            _mines.Add(position); // don't care about the possible duplicates
        }

        public bool HasMine(Position position)
        {
            return _mines.Contains(position);
        }
    }
}
