using System;
using System.Collections.Generic;
using System.Linq;
using TurtleWorld.Core;

namespace TurtleWorld.Structure
{
    public sealed class StandardConfiguration : IConfiguration
    {
        public StandardConfiguration(IBoard board, Position start, Position target, IEnumerable<string> moves)
        {
            Board = board ?? throw new ArgumentNullException(nameof(board), "Board is mandatory argument");
            if (moves == null)
            {
                throw new ArgumentNullException(nameof(moves), "Moves is mandatory argument");
            }

            Moves = new List<string>(moves.ToList()); // release the original collection
            Start = start;
            Target = target;
        }

        public IBoard Board { get; }
        public Position Start { get; }
        public Position Target { get; }
        public IEnumerable<string> Moves { get; }
    }
}
