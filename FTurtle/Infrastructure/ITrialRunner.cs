using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTurtle.Infrastructure
{
    public interface ITrialRunner
    {
        Task Run(Action<string> reporter, bool verbose);
    }
}
