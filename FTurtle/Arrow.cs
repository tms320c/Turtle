using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FTurtle
{
    public class Arrow
    {
        public static readonly (int, int) North = (-1, 0);
        public static readonly (int, int) West = (0, -1);
        public static readonly (int, int) South = (1, 0);
        public static readonly (int, int) East = (0, 1);

        public static Arrow Create((int, int) head)
        {
            return Arrow.Create(head.Item1, head.Item2);
        }

        public static Arrow Create(int x, int y)
        {
            return new Arrow(x, y);
        }

        public Arrow RotateRight() => Arrow.Create(this.Head switch
        {
            (-1, 0) => East, // N to E
            (0, 1) => South, // E to S
            (1, 0) => West, // S to W
            (0, -1) => North, // W to N
            (_, _) => this.Head
        });

        public Arrow RotateLeft() => Arrow.Create(this.Head switch
        {
            (-1, 0) => West, // N to W
            (0, -1) => South, // W to S
            (1, 0) => East, // S to E
            (0, 1) => North, // E to N
            (_, _) => this.Head
        });

        private Arrow(int x, int y)
        {
            Head = (x, y);
        }

        private Arrow((int, int) head)
        {
            Head = head;
        }

        public (int, int) Head { get; set; }
        public int X => Head.Item1;
        public int Y => Head.Item2;
    }
}
