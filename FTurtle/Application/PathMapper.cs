using System;
using System.Collections.Generic;
using FTurtle.Domain;
using TurtleWorld.Core;

namespace FTurtle.Application
{
    /// <summary>
    /// Converts (maps) sequence of the movement commands to a sequence of coordinates on the board.
    /// </summary>
    public sealed class PathMapper : IPathMapper
    {
        private readonly IBoard _board;

        /// <summary>
        /// The board is required because we "map trajectory onto the board"
        /// </summary>
        /// <param name="board">Game board</param>
        public PathMapper(IBoard board)
        {
            _board = board ?? throw new ArgumentNullException(nameof(board), "Board is mandatory");
        }

        /// <summary>
        /// Converts commands to the turtle trajectory.
        /// </summary>
        /// <param name="path">Sequence of the movement commands</param>
        /// <param name="initialPosition">coordinates and heading of the turtle's starting position</param>
        /// <param name="constraintHandler">Verifies and fixes coordinates to handle collisions with the board boundaries. See the interface comments for details.</param>
        /// <returns>Collection of positions. initialPosition is the first element of the collection.</returns>
        public IEnumerable<Position> Map(IEnumerable<Command> path, Position initialPosition, Func<Position, IBoard, Position> constraintHandler)
        {
            var position = initialPosition;
            yield return position;

            var pathMapper = new RelativeMapper(); // just a helper class defined below. Not a real dependency.

            // All moves are relative to Heading.North and should be transformed (rotated) to a new
            // coordinate system where initialPosition.Heading is equal to Heading.North
            var rotation = GetRotationCorrection(initialPosition.Heading);

            foreach (var move in pathMapper.Map(path, Heading.North))
            {
                // Transform coordinates increment to the board frame
                var realMove = rotation switch
                {
                    "R" => move.RotateRight(),
                    "2R" => move.RotateRight().RotateRight(), // add Mirror methods and save one mem alloc, maybe?
                    "L" => move.RotateLeft(),
                    _ => move
                };
                var nextPosition = new Position
                {
                    X = position.X + realMove.X,
                    Y = position.Y + realMove.Y,
                    Heading = realMove.Head // not really necessary here. Just a bookkeeping. It may be returned by constraintHandler
                };

                // we avoid mutations, but position is generator state, not the class instance state.
                position = constraintHandler?.Invoke(nextPosition, _board) ?? nextPosition;

                rotation = constraintHandler == null ? rotation : MayBeAdjustRotation(rotation, position);

                yield return position;
            }
        }

        /// <summary>
        /// Collision handler may request a rotation. The convention is:
        ///  Request is transmitted using position.Heading field and encoded as:
        ///   Heading.North or Heading.Void - keep the current rotation
        ///   Heading.East - add one rotation to right
        ///   Heading.South - add two rotations to right (turn around)
        ///   Heading.West - add one rotation to left
        /// </summary>
        /// <param name="currentRotation">current coordinates rotation</param>
        /// <param name="position">Position received from the collision handler</param>
        /// <returns></returns>
        private string MayBeAdjustRotation(string currentRotation, Position position)
        {
            if (position.Heading == Heading.Void || position.Heading == Heading.North) // correction is not required
            {
                return currentRotation;
            }

            return currentRotation switch
            {
                "0" => GetRotationCorrection(position.Heading), // the currentRotation rotation was identity (heading to North)
                "R" => position.Heading == Heading.East
                    ? "2R" 
                    : (position.Heading == Heading.South ? "L" : "0"),
                "2R" => position.Heading == Heading.East
                    ? "L"
                    : (position.Heading == Heading.South ? "0" : "R"),
                "L" => position.Heading == Heading.East
                    ? "0"
                    : (position.Heading == Heading.South ? "R" : "2R"),
                _ => currentRotation // whatever. to make compiler happy
            };
        }

        /// <summary>
        /// Reference frame transformation from "fixed frame" (where initial heading is North) to the board frame with arbitrary initial heading.
        /// </summary>
        /// <param name="initHead">The heading of the turtle's starting position</param>
        /// <returns>Required rotation tag</returns>
        private string GetRotationCorrection((int, int) initHead)
        {
            // All rotations are from Heading.North!
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

    /// <summary>
    /// Transforms the sequence of the movement commands to a sequence of position increments.
    /// </summary>
    internal sealed class RelativeMapper
    {
        /// <summary>
        /// Iterates on the commands and converts them to a trajectory in relative coordinates increments
        /// </summary>
        /// <param name="path">Sequence of the movement commands</param>
        /// <param name="originHeading">Initial heading. Does not relate to the initial position of the turtle!</param>
        /// <returns>Collection of coordinates increments</returns>
        public IEnumerable<Arrow> Map(IEnumerable<Command> path, (int, int) originHeading)
        {
            var currentMove = Arrow.Create(originHeading);

            foreach (var command in path)
            {
                // Rotation commands does not contribute to the coordinates increment but define its value.
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

    /// <summary>
    /// Representation of the coordinates increments. Can be interpreted as a vector (so, arrow).
    /// </summary>
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

        /// <summary>
        /// Clockwise rotation
        /// </summary>
        /// <returns>New vector</returns>
        public Arrow RotateRight() => Arrow.Create(this.Head switch
        {
            (-1, 0) => Heading.East, // N to E
            (0, 1) => Heading.South, // E to S
            (1, 0) => Heading.West, // S to W
            (0, -1) => Heading.North, // W to N
            (_, _) => this.Head
        });

        /// <summary>
        /// Counterclockwise rotation
        /// </summary>
        /// <returns>New vector</returns>
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
