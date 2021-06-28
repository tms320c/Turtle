using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;

namespace FTurtle
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
