using System;
using System.Collections.Generic;
using FTurtle.Domain;
using TurtleWorld.Core;

namespace FTurtle.Application
{
    /// <summary>
    /// Simple implementation of the path description tokenizer.
    /// </summary>
    public class PathTokenizer : IPathTokenizer
    {
        public IEnumerable<Command> Parse(string path, Func<char, Command> converter = null)
        {
            // rotations at the tail are not interesting - they do not contribute to the movement
            char[] tokensToTrim = {(char)Command.Left, (char)Command.Right};

            var tokens = path.Trim().ToUpper().TrimEnd(tokensToTrim).ToCharArray();
            for (int i = 0; i < tokens.Length; ++i)
            {
                var command = converter?.Invoke(tokens[i]) ?? DefaultConverter(tokens[i]);
                if (command != Command.Nop)
                {
                    yield return command;
                }
            }
        }

        /// <summary>
        /// Simple char to Command converter. Replaces unknown chars to Command.Nop, which is to be ignored by all functions downstream. Or may be not.
        /// </summary>
        /// <param name="c">Character to convert</param>
        /// <returns>Command</returns>
        private Command DefaultConverter(char c) {
            try
            {
                return (Command) c;
            }
            catch (Exception e)
            {
                // Just to be on the safe side
            }

            return Command.Nop;
        }
    }
}
