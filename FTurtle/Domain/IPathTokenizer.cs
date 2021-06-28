using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTurtle.Domain
{
    public interface IPathTokenizer
    {
        public IEnumerable<char> Parse(string path);
    }
}
