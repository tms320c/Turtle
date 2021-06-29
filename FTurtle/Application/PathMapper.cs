using System.Collections.Generic;
using FTurtle.Domain;

namespace FTurtle.Application
{
    public sealed class PathMapper : IPathMapper
    {
        private readonly (int, int) _relHeading = Heading.North;

        public PathMapper() { }

        public IEnumerable<Position> Map(IEnumerable<Command> path, Position initialPosition)
        {
            var position = initialPosition;
            yield return position;

            var pathMapper = new RelativeMapper();

            // All moves are relative to Heading.North and should be transformed (rotated) to a new
            // coordinate system where initialPosition.Heading is equal to Heading.North
            var rotation = GetRotationCorrection(initialPosition.Heading);

            foreach (var move in pathMapper.Map(path, Heading.North))
            {
                var realMove = rotation switch
                {
                    "R" => move.RotateRight(),
                    "2R" => move.RotateRight().RotateRight(), // add Mirror methods and save one mem alloc, maybe?
                    "L" => move.RotateLeft(),
                    _ => move
                };
                position = new Position // we avoid mutations, but this is generator state, not the class instance state.
                {
                    X = position.X + realMove.X,
                    Y = position.Y + realMove.Y,
                    Heading = realMove.Head
                };
                yield return position;
            }
        }

        private string GetRotationCorrection((int, int) initHead)
        {
            if (initHead == Heading.East)
            {
                return "R";
            }
            else if (initHead == Heading.South)
            {
                return "2R";
            }
            else if (initHead == Heading.West)
            {
                return "L";
            }

            return "0";
        }
    }

    internal sealed class RelativeMapper
    {
        public IEnumerable<Arrow> Map(IEnumerable<Command> path, (int, int) originHeading)
        {
            var currentMove = Arrow.Create(originHeading);

            foreach (var command in path)
            {
                currentMove = command switch
                {
                    Command.Right => currentMove.RotateRight(),
                    Command.Left => currentMove.RotateLeft(),
                    _ => currentMove
                };

                if (command == Command.Move)
                {
                    yield return currentMove;
                }
            }
        }
    }

    internal sealed class Arrow
    {
        public static Arrow Create((int, int) head)
        {
            return Arrow.Create(head.Item1, head.Item2);
        }

        public static Arrow Create(int x, int y)
        {
            return new Arrow(x, y);
        }

        public Arrow RotateRight() => Arrow.Create(this.Head switch
        {
            (-1, 0) => Heading.East, // N to E
            (0, 1) => Heading.South, // E to S
            (1, 0) => Heading.West, // S to W
            (0, -1) => Heading.North, // W to N
            (_, _) => this.Head
        });

        public Arrow RotateLeft() => Arrow.Create(this.Head switch
        {
            (-1, 0) => Heading.West, // N to W
            (0, -1) => Heading.South, // W to S
            (1, 0) => Heading.East, // S to E
            (0, 1) => Heading.North, // E to N
            (_, _) => this.Head
        });

        private Arrow(int x, int y)
        {
            Head = (x, y);
        }

        private Arrow((int, int) head)
        {
            Head = head;
        }

        public (int, int) Head { get; set; }
        public int X => Head.Item1;
        public int Y => Head.Item2;
    }

}
