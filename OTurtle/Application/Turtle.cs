using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OTurtle.Domain;
using TurtleWorld.Core;

namespace OTurtle.Application
{
    public class Turtle : IMovable
    {
        public Turtle((int, int) start, Direction direction)
        {
            Position = start;
            Direction = direction;
        }

        public bool Move(int dx, int dy)
        {
            throw new NotImplementedException();
        }

        public bool RotateLeft()
        {
            throw new NotImplementedException();
        }

        public bool RotateRight()
        {
            throw new NotImplementedException();
        }

        public (int, int) Position { get; }
        public Direction Direction { get; }
    }
}
