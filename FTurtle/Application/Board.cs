using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;

namespace FTurtle.Application
{
    public class Board : IBoard
    {
        private readonly IMineField _mineField;

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

        public bool HasMine(Position position)
        {
            if (position.X < 0 || position.X > Height - 1 || position.Y < 0 || position.Y > Width - 1)
            {
                throw new ArgumentException($"Position ({position.X}, {position.Y}) is outside of the board");
            }
            return _mineField.HasMine(position);
        }

        public bool HasMine(int x, int y)
        {
            return HasMine(new Position {X = x, Y = y});
        }
    }
}
