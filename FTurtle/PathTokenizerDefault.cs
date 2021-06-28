using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;

namespace FTurtle
{
    public class PathTokenizerDefault : IPathTokenizer
    {
        public IEnumerable<char> Parse(string path)
        {
            // rotations at the tail are not interesting - they do not contribute to movement
            char[] tokensToTrim = {'L', 'R'};

            return path.Trim().TrimEnd(tokensToTrim).ToCharArray();
        }
    }
}
