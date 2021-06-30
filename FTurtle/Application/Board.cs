using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;

namespace FTurtle.Application
{
    /// <summary>
    /// The game board. Has dimensions and has some mines implanted.
    /// </summary>
    public class Board : IBoard
    {
        private readonly IMineField _mineField;

        /// <summary>
        /// The board coordinates frame is defined as the following:
        ///  X coordinate runs from 0 at North (top) boundary to height - 1 at South (bottom) boundary
        ///  Y coordinate runs from 0 at West (left) boundary to width - 1 at East (right) boundary
        /// Thus, the top left corner has coordinates (0, 0), and the bottom right corner is at (height - 1, width - 1)
        /// </summary>
        /// <param name="width">width of the board</param>
        /// <param name="height">height of the board</param>
        /// <param name="mines">Mines collection. The mines are just coordinates (Position objects)</param>
        public Board(int width, int height, IMineField mines)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width and height should be greater than zero");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Width and height should be greater than zero");
            }

            _mineField = mines ?? throw new ArgumentNullException(nameof(mines), "Mine field is mandatory");

            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// Validates the position and delegates the query to the mine holder.
        /// </summary>
        /// <param name="position">Coordinates to look for a mine</param>
        /// <returns>True if the mine has been detected</returns>
        public bool HasMine(Position position)
        {
            ValidatePosition(position);
            return _mineField.HasMine(position);
        }

        public bool HasMine(int x, int y)
        {
            return HasMine(new Position {X = x, Y = y});
        }

        /// <summary>
        /// Arm new mine and plant it at the position
        /// </summary>
        /// <param name="position">Coordinates of the mine</param>
        public void AddMine(Position position)
        {
            ValidatePosition(position);
            _mineField.SetMine(position);
        }

        public void AddMine(int x, int y)
        {
            AddMine(new Position { X = x, Y = y });
        }

        /// <summary>
        /// Guards the board and does not allow to reference positions outside of it's boundaries.
        /// </summary>
        /// <param name="position">Position to validate</param>
        private void ValidatePosition(Position position)
        {
            if (position.X < 0 || position.X > Height - 1 || position.Y < 0 || position.Y > Width - 1)
            {
                throw new ArgumentException($"Position ({position.X}, {position.Y}) is outside of the board");
            }
        }
    }
}
