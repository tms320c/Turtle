﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;
using TurtleWorld.Core;
using TurtleWorld.Structure;

namespace FTurtle.Infrastructure
{
    public class TrialRunner : ITrialRunner
    {
        private readonly IConfiguration _config;
        private readonly IPathMapper _mapper;
        private readonly IPathTokenizer _tokenizer;
        private readonly Func<Position, IBoard, Position> _constraint;

        public TrialRunner(IConfiguration config, IPathMapper pathMapper, IPathTokenizer tokenizer, Func<Position, IBoard, Position> constraintHandler)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config), "Configuration is mandatory");
            _mapper = pathMapper ?? throw new ArgumentNullException(nameof(pathMapper), "Path mapper is mandatory");
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer), "Tokenizer is mandatory");
            _constraint = constraintHandler ?? ((p, b) => new Position
            {
                // Simple clipping strategy.
                // The turtle stays by the boundary until a rotation command (or end of the path)
                X = p.X >= b.Height
                    ? b.Height - 1
                    : (p.X < 0
                        ? 0
                        : p.X),
                Y = p.Y >= b.Width
                    ? b.Width - 1
                    : (p.Y < 0
                        ? 0
                        : p.Y),
                // Heading does not matter
            });
        }

        public void Run(Action<string> reporter, bool verbose = false)
        {
            var traced = 0;
            var minesHit = 0;
            var targetsReached = 0;

            foreach (var move in _config.Moves)
            {
                var result = "Still in Danger";

                var positions = _mapper.Map(_tokenizer.Parse(move), _config.Start, _constraint);

                ++traced;

                foreach (var position in positions)
                {
                    if (_config.Board.HasMine(position))
                    {
                        ++minesHit;
                        result = "Mine Hit";
                        break;
                    }

                    if (position == _config.Board.Target)
                    {
                        ++targetsReached;
                        result = "Success";
                        break;
                    }
                }
                
                reporter?.Invoke($"Movement number {traced}: {result}");
            }

            reporter?.Invoke($"Completed {traced} movements. Mines hit: {minesHit}, exits reached: {targetsReached}");
        }
    }
}