using TurtleWorld.Core;

namespace TurtleWorld.Structure.Collision
{
    public class ClipStrategy : IBoundaryAvoidanceStrategy
    {
        /// <summary>
        /// Simple clipping strategy.
        /// The turtle stays by the boundary until a rotation command (or end of the path)
        /// </summary>
        /// <param name="position">Position to fix</param>
        /// <param name="board">Board instance</param>
        /// <returns>Fixed position</returns>
        public Position Fix(Position position, IBoard board)
        {
            return new Position
            {
                X = ClipValue(position.X, 0, board.Height - 1),
                Y = ClipValue(position.Y, 0, board.Width - 1),
                // Heading does not matter for this strategy
            };
        }

        private int ClipValue(int value, int min, int max)
        {
            int result = value;
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
