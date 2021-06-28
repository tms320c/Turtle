using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;

namespace FTurtle
{
    public struct Position // it is ValueType for the sake of immutability
    {
        public int X { get; init; }
        public int Y { get; init; }
        public IArrow Heading { get; init; }
    }
}
