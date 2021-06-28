using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;

namespace FTurtle
{
    public class PathMapper : IPathMapper
    {
        public PathMapper() {}

        public IList<Arrow> MapRelative(string path, IPathTokenizer tokenizer)
        {
            var movements = new List<Arrow>();
            
            var currentMove = Arrow.Create(Arrow.North);

            foreach (var symbol in tokenizer.Parse(path))
            {
                currentMove = symbol switch
                {
                    'R' => currentMove.RotateRight(),
                    'L' => currentMove.RotateLeft(),
                    _ => currentMove
                };
                
                if (symbol == 'M')
                {
                    movements.Add(currentMove);
                }
            }

            return movements;
        }
    }
}
