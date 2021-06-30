using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;

namespace FTurtle.Infrastructure
{
    public interface IConfiguration
    {
        IBoard Board { get; }
        Position Start { get; }
        Position Target { get; }
        IEnumerable<string> Moves { get; }
    }
}

// The first line should define the board size
// The second line should contain a list of mines (i.e. list of co-ordinates separated by a space)
// The third line of the file should contain the exit point.
// The fourth line of the file should contain the starting position of the turtle.
// The fifth line to the end of the file should contain a series of moves. 