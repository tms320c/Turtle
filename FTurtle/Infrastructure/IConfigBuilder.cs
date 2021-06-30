using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTurtle.Infrastructure
{
    public interface IConfigBuilder
    {
        void Build(string rawLine);
        IConfiguration Get();
    }
}
