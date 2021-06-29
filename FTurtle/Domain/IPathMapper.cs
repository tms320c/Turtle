using System.Collections.Generic;

namespace FTurtle.Domain
{
    public interface IPathMapper
    {
        IEnumerable<Position> Map(IEnumerable<Command> path, Position initialPosition);
    }
}
