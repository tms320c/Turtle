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
        bool Move();
        bool RotateLeft();
        bool RotateRight();
        (int, int) Position();
        (int, int) Direction();
    }
}
