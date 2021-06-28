using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;

namespace FTurtle
{
    public sealed class PathMapper : IPathMapper
    {
        private readonly (int, int) _relHeading = Heading.North;

        public PathMapper() { }

        public IList<IArrow> MapRelative(string path, IPathTokenizer tokenizer)
        {
            var movements = new List<IArrow>();

            var currentMove = Arrow.Create(_relHeading);

            foreach (var symbol in tokenizer.Parse(path))
            {
                currentMove = symbol switch
                {
                    'R' => currentMove.RotateRight(),
                    'L' => currentMove.RotateLeft(),
                    _ => currentMove
                };

                if (symbol == 'M')
                {
                    movements.Add(currentMove);
                }
            }

            return movements;
        }

        public IEnumerable<Position> MapAbsolute(IEnumerable<IArrow> moves, Position initialPosition)
        {
            var position = initialPosition;
            yield return position;

            // All moves are relative to Heading.North and should be transformed (rotated) to a new
            // coordinate system where initialPosition.Heading is equal to Heading.North
            var rotation = GetRotationCorrection(initialPosition.Heading.Head);

            foreach (var move in moves)
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
                    Heading = realMove
                };
                yield return position;
            }
        }

        private string GetRotationCorrection((int, int) initHead)
        {
            var rotation = "0";
            if (initHead == Heading.East)
            {
                rotation = "R";
            }
            else if (initHead == Heading.South)
            {
                rotation = "2R";
            }
            else if (initHead == Heading.West)
            {
                rotation = "L";
            }

            return rotation;
        }
    }
}
