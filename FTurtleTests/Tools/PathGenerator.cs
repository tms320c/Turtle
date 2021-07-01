using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleTests.Tools
{
    public static class PathGenerator
    {
        private static readonly string[] _validTokens = { "M", "R", "M", "L", "M", "M"}; // we need more movements!
        private static readonly Random _rnd = new Random(42); // we want the same sequence at each try

        public static string Create(int length)
        {
            if (length <= 1)
            {
                return _validTokens[_rnd.Next(_validTokens.Length)];
            }

            length = length > 32767 ? 32768 : length;

            var builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                builder.Append(_validTokens[_rnd.Next(_validTokens.Length)]);
            }

            return builder.ToString();
        }
    }
}
