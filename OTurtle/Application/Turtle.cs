using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OTurtle.Domain;
using TurtleWorld.Core;
using TurtleWorld.Structure.Collision;

namespace OTurtle.Application
{
    public class Turtle : IMovable
    {
        private readonly Func<Position, IBoard, Position> _collisionHandler;
        private readonly IBoard _board;
        private (int, int) _position;
        private (int, int) _direction;

        public Turtle((int, int) start, (int, int) direction, IBoard board, Func<Position, IBoard, Position> collisionHandler = null)
        {
            _position = start;
            _direction = direction;
            _board = board;
            _collisionHandler = collisionHandler ?? BoundaryAvoidanceFactory.Create(StrategyKind.Clip);
        }

        public bool Move()
        {
            var newPosition = _collisionHandler(
                new Position
                {
                    X = _position.Item1 + _direction.Item1,
                    Y = _position.Item2 + _direction.Item2,
                },
                _board);

            _position.Item1 = newPosition.X;
            _position.Item2 = newPosition.Y;

            // Obey protocol of collision avoidance
            if (newPosition.Heading == Heading.West)
            {
                RotateLeft();
            }
            else if (newPosition.Heading == Heading.East)
            {
                RotateRight();
            }
            else if (newPosition.Heading == Heading.South)
            {
                RotateRight();
                RotateRight();
            }

            return true;
        }

        public bool RotateLeft()
        {
            _direction = _direction switch
            {
                (-1, 0) => Heading.West, // N to W
                (0, -1) => Heading.South, // W to S
                (1, 0) => Heading.East, // S to E
                (0, 1) => Heading.North, // E to N
                (_, _) => _direction
            };
            return true;
        }

        public bool RotateRight()
        {
            _direction = _direction switch
            {
                (-1, 0) => Heading.East, // N to E
                (0, 1) => Heading.South, // E to S
                (1, 0) => Heading.West, // S to W
                (0, -1) => Heading.North, // W to N
                (_, _) => _direction
            };
            return true;
        }

        public (int, int) Position()
        {
            return _position;
        }

        public (int, int) Direction()
        {
            return _direction;
        }
    }
}
