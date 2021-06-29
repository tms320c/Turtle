using System;
using System.Collections.Generic;
using FTurtle.Domain;

namespace FTurtle.Application
{
    public class PathTokenizerDefault : IPathTokenizer
    {
        public IEnumerable<Command> Parse(string path, Func<char, Command> validator = null)
        {
            // rotations at the tail are not interesting - they do not contribute to movement
            char[] tokensToTrim = {(char)Command.Left, (char)Command.Right};

            var tokens = path.Trim().TrimEnd(tokensToTrim).ToCharArray();
            for (int i = 0; i < tokens.Length; ++i)
            {
                var command = validator?.Invoke(tokens[i]) ?? DefaultValidator(tokens[i]);
                if (command != Command.Skip)
                {
                    yield return command;
                }
            }
        }

        private Command DefaultValidator(char c) {
            try
            {
                return (Command) c;
            }
            catch (Exception e)
            {
            }

            return Command.Skip;
        }
    }
}
