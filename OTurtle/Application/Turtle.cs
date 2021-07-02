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
        private readonly (int, int) _startPosition;
        private readonly (int, int) _startDirection;

        public Turtle((int, int) start, (int, int) direction, IBoard board, Func<Position, IBoard, Position> collisionHandler = null)
        {
            _startPosition = start;
            _startDirection = direction;
            _position = start;
            _direction = direction;
            _board = board;
            _collisionHandler = collisionHandler ?? BoundaryAvoidanceFactory.Create(StrategyKind.Clip);
        }

        public void Reset()
        {
            _position = _startPosition;
            _direction = _startDirection;
        }

        public Position Move()
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

            return new Position
            {
                X = _position.Item1,
                Y = _position.Item2,
                Heading = _direction
            };
        }

        public void RotateLeft()
        {
            //_direction = _direction switch
            //{
            //    (-1, 0) => Heading.West, // N to W
            //    (0, -1) => Heading.South, // W to S
            //    (1, 0) => Heading.East, // S to E
            //    (0, 1) => Heading.North, // E to N
            //    (_, _) => _direction
            //};
            // Just for the code reuse. Forget the performance for a while.
            RotateRight();
            RotateRight();
            RotateRight();
        }

        public void RotateRight()
        {
            _direction = _direction switch
            {
                (-1, 0) => Heading.East, // N to E
                (0, 1) => Heading.South, // E to S
                (1, 0) => Heading.West, // S to W
                (0, -1) => Heading.North, // W to N
                (_, _) => _direction
            };
        }

        public Position Position()
        {
            return new Position
            {
                X = _position.Item1,
                Y = _position.Item2,
                Heading = _direction
            };
        }

        public (int, int) Direction()
        {
            return _direction;
        }
    }
}
