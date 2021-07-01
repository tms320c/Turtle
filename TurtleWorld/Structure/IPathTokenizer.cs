using System;
using System.Collections.Generic;
using TurtleWorld.Core;

namespace TurtleWorld.Structure
{
    /// <summary>
    /// String to Command converters
    /// </summary>
    public interface IPathTokenizer
    {
        /// <summary>
        /// Maps string with the commands to the commands tokens
        /// </summary>
        /// <param name="path">String of the commands as defined in the trial configuration</param>
        /// <param name="converter">char to Command conversion strategy</param>
        /// <returns>Collection of the validated commands</returns>
        public IEnumerable<Command> Parse(string path, Func<char, Command> converter = null);
    }
}
