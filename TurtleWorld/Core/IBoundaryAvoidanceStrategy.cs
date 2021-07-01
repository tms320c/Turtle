using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleWorld.Core
{
    public interface IBoundaryAvoidanceStrategy
    {
        /// <summary>
        /// Takes current position and return new position which is inside board.
        /// </summary>
        /// <param name="position">Position to fix</param>
        /// <param name="board">Board instance</param>
        /// <returns></returns>
        Position Fix(Position position, IBoard board);
    }
}
