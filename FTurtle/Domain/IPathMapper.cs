using System.Collections.Generic;

namespace FTurtle.Domain
{
    public interface IPathMapper
    {
        IList<Arrow> MapRelative(string path, IPathTokenizer tokenizer);
    }
}
