using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurtleWorld.Core;

namespace OTurtle.Domain
{
    public interface IMovable
    {
        bool Move(int dx, int dy);
        bool RotateLeft();
        bool RotateRight();
        (int, int) Position { get; }
        Direction Direction { get; }
    }
}
