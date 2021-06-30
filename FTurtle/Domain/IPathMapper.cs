using System;
using System.Collections.Generic;
using FTurtle.Application;

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
        /// <param name="constraintHandler">Verifies and fixes coordinates to handle collisions with the board boundaries</param>
        /// <returns>Collection of positions. initialPosition should be the first element of the collection.</returns>
        IEnumerable<Position> Map(IEnumerable<Command> path, Position initialPosition, Func<Position, IBoard, Position> constraintHandler);
    }
}

// constraintHandler strategy receives current position and returns the fixed one. The strategy has Board object so it can get information
// about board (e.g. width and height).
// The strategy can signal to the mapper that the heading should be changed (i.e. imitates L or R commands) using the Position.Heading field in the returning position:
//   North or Void means "rotation is not required"
//   East - "rotate right"
//   South - "rotate right twice"
//   West - "rotate left"
