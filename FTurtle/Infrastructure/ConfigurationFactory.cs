using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTurtle.Infrastructure
{
    /// <summary>
    /// Builds the configuration
    /// </summary>
    public static class ConfigurationFactory
    {
        private const int SizeLineNum = 0;       // The first line should define the board size.
        private const int MinesLineNum = 1;      // The second line should contain a list of mines (i.e. list of co-ordinates separated by a space).
        private const int TargetLineNum = 2;     // The third line of the file should contain the exit point.
        private const int StartLineNum = 3;      // The fourth line of the file should contain the starting position of the turtle.
        private const int MovesFirstLineNum = 4; // The fifth line to the end of the file should contain a series of moves. 

        /// <summary>
        /// Parses raw data and creates configuration instance.
        /// The raw data should be sanitized:
        ///  There are no blank lines,
        ///  There are no more than 1 space between characters
        ///  All characters are in upper case.
        /// </summary>
        /// <param name="configType">What to create</param>
        /// <param name="rawData">The sanitized content of the configuration file</param>
        /// <returns></returns>
        public static IConfiguration Create(string configType, IList<string> rawData)
        {
            if (configType != "standard")
            {
                throw new NotImplementedException($"'{configType}' is not supported yet.");
            }


            var rawSize = rawData[SizeLineNum];


            var rawMines = rawData[MinesLineNum];


            var rawTarget = rawData[TargetLineNum];


            var rawStart = rawData[StartLineNum];


            var movesStartIdx = MovesFirstLineNum;
            return null;
        }

        /// <summary>
        /// Split and converts strings like "17 42"
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private static (int, int) ParsePairOfNumbers(string raw)
        {
            var pair = raw.Split(" ");
            return (0, 0);
        }
    }
}
