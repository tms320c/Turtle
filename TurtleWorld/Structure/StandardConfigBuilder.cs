﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TurtleWorld.Core;
using TurtleWorld.Entities;
using TurtleWorld.Structure.Exceptions;

namespace TurtleWorld.Structure
{
    /// <summary>
    /// Builds simple (standard) configuration
    /// </summary>
    internal class StandardConfigBuilder : IConfigBuilder
    {
        private const int SizeLineNum = 0;       // The first line should define the board size.
        private const int MinesLineNum = 1;      // The second line should contain a list of mines (i.e. list of co-ordinates separated by a space).
        private const int TargetLineNum = 2;     // The third line of the file should contain the exit point.
        private const int StartLineNum = 3;      // The fourth line of the file should contain the starting position of the turtle.
        private const int MovesFirstLineNum = 4; // The fifth line to the end of the file should contain a series of moves. 

        private int _currentLine = 0; // state machine

        private IConfiguration _configuration;

        private struct Partials
        {
            public Position BoardDimensions;
            public Position Target;
            public Position Start;
            public IList<Position> Mines;
            public IList<string> Moves;
        }

        private readonly Func<int, int, Position, IMineField, IBoard> _boardMaker;
        private readonly Func<IMineField> _mineFieldMaker;

        private Partials _partials;

        // Accept digits, spaces, comma, and valid commands in either case. At least 1 per line.
        private readonly Regex _generalValidator = new Regex(@"[\d\s,RLMNSEW]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _movesValidator = new Regex(@"[\s,RLM]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _minesValidator = new Regex(@"(\d,\d\s)+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        private readonly Regex _pairValidator = new Regex(@"\d\s\d", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Regex _startValidator = new Regex(@"\d\s\d\s[NESW]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Build with standard board and mines
        /// </summary>
        public StandardConfigBuilder() : this(null, null) { }

        /// <summary>
        /// Build board and mines using custom object makers
        /// </summary>
        /// <param name="boardMaker">Board producer delegate</param>
        /// <param name="mineFieldMaker">Mine field producer delegate</param>
        public StandardConfigBuilder(Func<int, int, Position, IMineField, IBoard> boardMaker, Func<IMineField> mineFieldMaker)
        {
            _boardMaker = boardMaker;
            _mineFieldMaker = mineFieldMaker;

            _partials = new Partials
            {
                Mines = new List<Position>(),
                Moves = new List<string>()
            };
        }

        /// <summary>
        /// Incrementally builds the configuration.
        /// ConfigurationFactory gives this method as delegate to the configuration file reader
        /// The builder do not throw exceptions because the exception(s) may be thrown
        /// at configuration request if the builder has not build complete and consistent configuration.
        /// The builder tries its best.
        /// </summary>
        /// <param name="rawLine">A line from the configuration file</param>
        public void Build(string rawLine)
        {
            var line = Sanitize(rawLine);
            if (line.Length == 0)
            {
                return;
            }

            // The parsers expect sanitized line and non-successful completion means
            // that the data has been seriously broken.
            // Thus, the state machine stays at current state in hope that the correct line will be read eventually.
            var success = _currentLine switch
            {
                SizeLineNum => ParseSize(line),
                MinesLineNum => ParseMines(line),
                TargetLineNum => ParseTarget(line),
                StartLineNum => ParseStart(line),
                _ => ParseMoves(line),
            };

            if (success)
            {
                ++_currentLine; // A kind of retry if not success
            }

            return;
        }

        /// <summary>
        /// Returns the configuration if ready.
        /// </summary>
        /// <returns>Configuration instance</returns>
        public IConfiguration Get()
        {
            if (!IsReady())
            {
                throw new ConfigurationNotReadyException("The configuration is incomplete");
            }

            if (_configuration != null)
            {
                return _configuration;
            }

            string message = ValidateConfiguration();
            if (message.Length > 0)
            {
                throw new InconsistentConfigurationException(message);
            }

            var mines = _mineFieldMaker?.Invoke() ?? new MineField();
            var board = _boardMaker?.Invoke(_partials.BoardDimensions.Y, _partials.BoardDimensions.X, _partials.Target, mines) 
                        ?? new Board(_partials.BoardDimensions.Y, _partials.BoardDimensions.X, _partials.Target, mines);

            foreach (var mine in _partials.Mines)
            {
                board.AddMine(mine);
            }

            _configuration = new StandardConfiguration(board, _partials.Start, _partials.Target, _partials.Moves);

            Reset();

            return _configuration;
        }

        private void Reset()
        {
            _currentLine = 0;
            _partials.Mines.Clear();
            _partials.Moves.Clear();
            _partials.Start = new Position();
            _partials.Target = new Position();
            _partials.BoardDimensions = new Position();
        }

        /// <summary>
        /// Convert record N M to Height Width of the board. N -> height, M -> width.
        /// </summary>
        /// <param name="line">string to parse</param>
        /// <returns>true if converted</returns>
        private bool ParseSize(string line)
        {
            if (!_pairValidator.IsMatch(line))
            {
                return false;
            }

            var parts = line.Split(" ");
            if (parts.Length < 2)
            {
                return false;
            }

            var values = ParseNumberPair(parts);
            if (!values.Item3)
            {
                return false;
            }

            _partials.BoardDimensions = new Position
            {
                X = values.Item2, // board height (X-axis span)
                Y = values.Item1  // board width (Y-axis span)
            };

            return true;
        }

        /// <summary>
        /// Convert record N,M to (x,y) coordinates of mine. N -> x, M -> y.
        /// </summary>
        /// <param name="line">string to parse</param>
        /// <returns>true if converted</returns>
        private bool ParseMines(string line)
        {
            if (!_minesValidator.IsMatch(line))
            {
                return false;
            }

            var pairs = line.Split();
            if (pairs.Length == 0)
            {
                return false;
            }

            foreach (var pair in pairs)
            {
                var parts = pair.Split(',');
                if (parts.Length == 0)
                {
                    continue; // well, don't be so pedantic, don't throw the line away...
                }

                var values = ParseNumberPair(parts);
                if (!values.Item3)
                {
                    continue; // well...
                }
                _partials.Mines.Add(new Position
                {
                    X = values.Item1,
                    Y = values.Item2
                });
            }

            return _partials.Mines.Count > 0; // By spec, the mines are mandatory (config file elements are specified by its line number)
        }

        /// <summary>
        /// Convert record N M to (x,y) coordinates of exit point. N -> x, M -> y.
        /// </summary>
        /// <param name="line">string to parse</param>
        /// <returns>true if converted</returns>
        private bool ParseTarget(string line)
        {
            if (!_pairValidator.IsMatch(line))
            {
                return false;
            }

            var parts = line.Split(" ");
            if (parts.Length < 2)
            {
                return false;
            }

            var values = ParseNumberPair(parts);
            if (!values.Item3)
            {
                return false;
            }

            _partials.Target = new Position
            {
                X = values.Item1,
                Y = values.Item2
            };

            return true;
        }

        /// <summary>
        /// Convert record N M W to (x,y) starting point coordinates and initial heading. N -> x, M -> y, W -> heading
        /// </summary>
        /// <param name="line">string to parse</param>
        /// <returns>true if converted</returns>
        private bool ParseStart(string line)
        {
            if (!_startValidator.IsMatch(line))
            {
                return false;
            }

            var parts = line.Split(" ");
            if (parts.Length < 3)
            {
                return false;
            }

            var values = ParseNumberPair(parts[0..2]);
            if (!values.Item3)
            {
                return false;
            }

            var heading = parts[2] switch
            {
                "N" => Heading.North,
                "S" => Heading.South,
                "E" => Heading.East,
                "W" => Heading.West,
                _ => Heading.Void
            };
            if (heading == Heading.Void)
            {
                return false;
            }

            _partials.Start = new Position
            {
                X = values.Item1,
                Y = values.Item2,
                Heading = heading
            };

            return true;
        }

        /// <summary>
        /// Convert record C1 C2 C3 to movements string C1C2C3...
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool ParseMoves(string line)
        {
            if (!_movesValidator.IsMatch(line))
            {
                return false;
            }

            line = Regex.Replace(line, @"\s+", ""); // remove spaces
            _partials.Moves.Add(line);
            // I do not count moves because:
            // a) the spec can be read that the moves are not mandatory at all
            // b) _partials.Moves is IEnumerable, so may be lazy collection and Count() may take a lot of time and CPU
            return true;
        }

        /// <summary>
        /// General two strings to to integers converter
        /// </summary>
        /// <param name="pair">string array to parse</param>
        /// <returns>tuple (number1, number2, true if success)</returns>
        private (int, int, bool) ParseNumberPair(string[] pair)
        {
            if (pair.Length < 2)
            {
                return (default, default, false);
            }

            if (!int.TryParse(pair[0], out var a))
            {
                return (default, default, false);
            }

            if (!int.TryParse(pair[1], out var b))
            {
                return (default, default, false);
            }

            return (a, b, true);
        }

        /// <summary>
        /// Sanitize input string. See the tests for the possible inputs.
        /// </summary>
        /// <param name="rawLine"></param>
        /// <returns>A string that conforms to the spec, or empty string.</returns>
        private string Sanitize(string rawLine)
        {
            var line = rawLine.Trim().TrimEnd(',').TrimStart(',');
            if (line.Length == 0 || !_generalValidator.IsMatch(line))
            {
                return "";
            }

            line = Regex.Replace(line, @"\s+", " ").ToUpper(); // multiple spaces to a single one and string to uppercase
            line = Regex.Replace(line, @"\s*,\s*", ","); // remove spaces around commas
            line = Regex.Replace(line, @",+", ","); // multiple commas to a single one

            return line;
        }

        /// <summary>
        /// Simple build completion indicator. The builder has to reach to the first movements line at least.
        /// </summary>
        /// <returns>true if ready</returns>
        private bool IsReady()
        {
            return _currentLine > MovesFirstLineNum;
        }

        /// <summary>
        /// Configuration consistency validator. E.g. verifies that the board has reasonable (positive) dimensions.
        /// </summary>
        /// <returns>Empty string if the configuration is valid, error messages if not.</returns>
        private string ValidateConfiguration()
        {
            var builder = new StringBuilder();

            var height = _partials.BoardDimensions.X;
            var width = _partials.BoardDimensions.Y;

            if (height <= 0)
            {
                builder.Append($"Invalid board height {height} ");
                return builder.ToString();
            }
            if (width <= 0)
            {
                builder.Append($"Invalid board width {width} ");
                return builder.ToString();
            }

            var startX = _partials.Start.X;
            var startY = _partials.Start.Y;

            if (!IsInside(startX, startY))
            {
                builder.Append($"Starting position ( {startX}, {startY}) is outside of the board {width}x{height} ");
            }

            var targetX = _partials.Target.X;
            var targetY = _partials.Target.Y;

            if (!IsInside(targetX, targetY))
            {
                builder.Append($"Starting position ( {targetX}, {targetY}) is outside of the board {width}x{height} ");
            }

            if (_partials.Moves.Count == 0)
            {
                builder.Append("Empty moves description ");
            }

            return builder.ToString();
        }

        private bool IsInside(int x, int y)
        {
            return x >= 0 && x < _partials.BoardDimensions.X && y >= 0 && y < _partials.BoardDimensions.Y;
        }
    }
}
