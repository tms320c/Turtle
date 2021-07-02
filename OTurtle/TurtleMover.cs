using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Application;
using OTurtle.Domain;
using TurtleWorld.Core;
using TurtleWorld.Structure;

namespace OTurtle
{
    class TurtleMover
    {
        private readonly IMovable _turtle;
        private readonly IBoard _board;
        private readonly IPathTokenizer _tokenizer;

        public TurtleMover(IMovable turtle, IBoard board, IPathTokenizer tokenizer = null)
        {
            _turtle = turtle;
            _board = board;
            _tokenizer = tokenizer ?? new Tokenizer();
        }

        public void Move(int moveNum, string path, Action<string> reporter = null, bool verbose = false)
        {
            var result = "Still in danger";
            
            // Check initial position
            var position = _turtle.Position();

            var recon = CheckPosition(position);
            if (recon.Length > 0)
            {
                reporter?.Invoke(result);
                return;
            }

            if (verbose)
            {
                reporter?.Invoke($"Movement {moveNum}: at ({position.X}, {position.Y}).");
            }

            // Move the turtle
            foreach (var command in _tokenizer.Parse(path))
            {
                switch (command)
                {
                    case Command.Left:
                        _turtle.RotateLeft();
                        break;
                    case Command.Right:
                        _turtle.RotateRight();
                        break;
                    case Command.Move:
                        position = _turtle.Move();
                        if (verbose)
                        {
                            reporter?.Invoke($"Movement {moveNum}: at ({position.X}, {position.Y}).");
                        }
                        recon = CheckPosition(position);
                        if (recon.Length > 0)
                        {
                            _turtle.Reset();
                            reporter?.Invoke(recon);
                            return;
                        }
                        break;
                    default:
                        continue;
                }
            }

            _turtle.Reset();
            reporter?.Invoke(result);
        }

        private string CheckPosition(Position position)
        {
            if (_board.HasMine(position))
            {
                return "Mine Hit";
            }
            if (position == _board.Target)
            {
                return "Success";
            }

            return String.Empty;
        }
    }
}
