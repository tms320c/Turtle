using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;

namespace FTurtle.Infrastructure
{
    public sealed class Configuration : IConfiguration
    {
        public Configuration(IBoard board, Position start, Position target, IEnumerable<string> moves)
        {
            Board = board ?? throw new ArgumentNullException(nameof(board), "Board is mandatory argument");
            Moves = moves ?? throw new ArgumentNullException(nameof(moves), "Moves is mandatory argument"); ;
            Start = start;
            Target = target;
        }

        public IBoard Board { get; }
        public Position Start { get; }
        public Position Target { get; }
        public IEnumerable<string> Moves { get; }
    }
}
