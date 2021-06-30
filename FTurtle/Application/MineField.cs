using System.Collections.Generic;
using FTurtle.Domain;

namespace FTurtle.Application
{
    /// <summary>
    /// Mines holder. Just a decorator to actual data storage which implements ISet<Position>.
    /// </summary>
    public sealed class MineField : IMineField
    {
        private readonly ISet<Position> _mines;

        public MineField() : this(null)
        {}

        public MineField(ISet<Position> mines)
        {
            _mines = mines ?? new HashSet<Position>();
        }

        /// <summary>
        /// Adds a mine to the storage
        /// </summary>
        /// <param name="position">Mine coordinates</param>
        public void SetMine(Position position)
        {
            _mines.Add(position); // don't care about the possible duplicates
        }

        /// <summary>
        /// Check if there is a mine at the position
        /// </summary>
        /// <param name="position">Coordinates to check</param>
        /// <returns>True if the mine has been found</returns>
        public bool HasMine(Position position)
        {
            return _mines.Contains(position);
        }
    }
}
