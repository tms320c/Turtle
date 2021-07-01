using System;
using System.Collections.Generic;
using TurtleWorld.Core;
using TurtleWorld.Structure;

namespace FTurtle.Application
{
    /// <summary>
    /// Simple implementation of the path description tokenizer.
    /// </summary>
    public class Tokenizer : IPathTokenizer
    {
        public IEnumerable<Command> Parse(string path, Func<char, Command> converter = null)
        {
            var tokens = path.Trim().ToUpper().ToCharArray();
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
