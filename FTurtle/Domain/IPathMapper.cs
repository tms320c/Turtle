using System.Collections.Generic;

namespace FTurtle.Domain
{
    public interface IPathMapper
    {
        IEnumerable<IArrow> MapRelative(string path, IPathTokenizer tokenizer);
        IEnumerable<Position> MapAbsolute(IEnumerable<IArrow> moves, Position initialPosition);
    }
}
