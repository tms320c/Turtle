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
        Position Move();
        void RotateLeft();
        void RotateRight();
        Position Position();
        (int, int) Direction();
        void Reset();
    }
}
