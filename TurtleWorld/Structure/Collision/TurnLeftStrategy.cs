using TurtleWorld.Core;

namespace TurtleWorld.Structure.Collision
{
    public class TurnLeftStrategy : IBoundaryAvoidanceStrategy
    {
        /// <summary>
        /// This strategy makes a left turn: the turtle turns to move along the boundary.
        /// It (along the boundary) may be false assumption, because if the next command is 'L', or 'R' then
        /// The turtle makes two turns in row. So, the next step may no be "along the boundary".
        /// </summary>
        /// <param name="position">Position to fix</param>
        /// <param name="board">Board instance</param>
        /// <returns>Fixed position</returns>
        public Position Fix(Position position, IBoard board)
        {
            
            var h = board.Height;
            var w = board.Width;

            var collisionDetected = position.X >= h || position.X < 0 || position.Y >= w || position.Y < 0;

            return new Position
            {
                X = ClipValue(position.X, 0, h - 1),
                Y = ClipValue(position.Y, 0, w - 1),
                Heading = collisionDetected ? Heading.West : Heading.Void // left turn is required if collision. Otherwise, stay on the current course.
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
