using System.Collections.Generic;

namespace FTurtle.Domain
{
    /// <summary>
    /// Defines the interface for the mappers (converters) from the sequence of the movement commands to a sequence of coordinates on the board.
    /// </summary>
    public interface IPathMapper
    {
        /// <summary>
        /// Converts commands to the turtle trajectory.
        /// </summary>
        /// <param name="path">Sequence of the movement commands</param>
        /// <param name="initialPosition">coordinates and heading of the turtle's starting position</param>
        /// <returns>Collection of positions. initialPosition should be the first element of the collection.</returns>
        IEnumerable<Position> Map(IEnumerable<Command> path, Position initialPosition);
    }
}
