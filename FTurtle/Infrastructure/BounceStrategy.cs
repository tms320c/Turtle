using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurtleWorld.Core;

namespace FTurtle.Infrastructure
{
    public class BounceStrategy : IBoundaryAvoidanceStrategy
    {
        /// <summary>
        /// This strategy makes a bounce: the turtle bounces from a boundary (moves one step back) but keeps the heading
        /// </summary>
        /// <param name="position">Position to fix</param>
        /// <param name="board">Board instance</param>
        /// <returns>Fixed position</returns>
        public Position Fix(Position position, IBoard board)
        {
            return new Position
            {
                X = BounceValue(position.X, 0, board.Height - 1),
                Y = BounceValue(position.Y, 0, board.Width - 1)
                // Heading does not matter
            };
        }

        private int BounceValue(int value, int min, int max)
        {
            var result = value;
            if (result > max)
            {
                result -= 2; // -1 moves to the position at the boundary, and another -1 makes the bounce
            } else if (result < min)
            {
                result = min + 1; // extra +1 for the bounce
            }

            // these extra shifts may move position outside of the board in corner cases (e.g. on 1xN board)
            if (result > max)
            {
                result = max;
            }
            else if (result < min)
            {
                result = min;
            }

            return result;
        }
    }
}
