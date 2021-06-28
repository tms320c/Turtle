using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FTurtle.Domain;

namespace FTurtle
{
    public sealed class Arrow : IArrow
    {
        public static IArrow Create((int, int) head)
        {
            return Arrow.Create(head.Item1, head.Item2);
        }

        public static IArrow Create(int x, int y)
        {
            return new Arrow(x, y);
        }

        public IArrow RotateRight() => Arrow.Create(this.Head switch
        {
            (-1, 0) => Heading.East, // N to E
            (0, 1) => Heading.South, // E to S
            (1, 0) => Heading.West, // S to W
            (0, -1) => Heading.North, // W to N
            (_, _) => this.Head
        });

        public IArrow RotateLeft() => Arrow.Create(this.Head switch
        {
            (-1, 0) => Heading.West, // N to W
            (0, -1) => Heading.South, // W to S
            (1, 0) => Heading.East, // S to E
            (0, 1) => Heading.North, // E to N
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
