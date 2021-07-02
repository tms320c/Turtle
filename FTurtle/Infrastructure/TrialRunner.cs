using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Application;
using FTurtle.Domain;
using TurtleWorld.Core;
using TurtleWorld.Structure;
using TurtleWorld.Structure.Collision;

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
            _constraint = constraintHandler ?? BoundaryAvoidanceFactory.Create(StrategyKind.Clip); ;
        }

        public async Task Run(Action<string> reporter, bool verbose = false)
        {
            var traced = 0;
            var minesHit = 0;
            var targetsReached = 0;

            foreach (var move in _config.Moves)
            {
                var result = "Still in Danger";

                ++traced;
                if (verbose)
                {
                    reporter?.Invoke($"Start of movement {traced} processing");
                }

                var positions = _mapper.Map(_tokenizer.Parse(move), _config.Start, _constraint);

                foreach (var position in positions)
                {
                    if (verbose)
                    {
                        reporter?.Invoke($"Movement {traced}: at ({position.X}, {position.Y}).");
                    }

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

                    if (verbose)
                    {
                        reporter?.Invoke($"Movement {traced}: so far so good.");
                    }
                }

                reporter?.Invoke($"Movement {traced}: {result}");
            }

            reporter?.Invoke($"Completed {traced} movements. Mines hit: {minesHit}, exits reached: {targetsReached}");
        }
    }
}
