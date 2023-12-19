using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public record Coordinate2D(int X, int Y)
    {
        
        public int ManhattenDistance(Coordinate2D other) => (int)(Math.Abs(X - other.X) + Math.Abs(Y - other.Y));
        public int Magnitude() => Math.Abs(X) + Math.Abs(Y);
        public Coordinate2D Vector(Coordinate2D other) => new(other.X - X, other.Y - Y);


        public (int, int) Difference(Coordinate2D other)
        {
            int xDiff = this.X - other.X;
            int yDiff = this.Y - other.Y;

            return (xDiff, yDiff);
        }

        public List<Coordinate2D> Neighbours(bool diagonals, bool self)
        {
            var tmp = new List<Coordinate2D>();

            if (diagonals) tmp.Add(new Coordinate2D(X - 1, Y - 1));     //top left
            tmp.Add(new Coordinate2D(X, Y - 1));                        //top
            if (diagonals) tmp.Add(new Coordinate2D(X + 1, Y - 1));     //top right
            tmp.Add(new Coordinate2D(X - 1, Y));                        //left
            if (self) tmp.Add(new Coordinate2D(X, Y));                  //center
            tmp.Add(new Coordinate2D(X + 1, Y));                        //right
            if (diagonals) tmp.Add(new Coordinate2D(X - 1, Y + 1));     //bottom left
            tmp.Add(new Coordinate2D(X, Y + 1));                        //bottom 
            if (diagonals) tmp.Add(new Coordinate2D(X + 1, Y + 1));     //bottom right            
            return tmp;
        }

        public static Coordinate2D operator +(Coordinate2D a) => a;
        public static Coordinate2D operator +(Coordinate2D a, Coordinate2D b) => new(a.X + b.X, a.Y + b.Y);
        public static Coordinate2D operator -(Coordinate2D a) => new(-a.X, -a.Y);
        public static Coordinate2D operator -(Coordinate2D a, Coordinate2D b) => a + (-b);
        public static Coordinate2D operator *(int scale, Coordinate2D a) => new(scale * a.X, scale * a.Y);

        public static implicit operator Coordinate2D((int x, int y) a) => new(a.x, a.y);

        public static implicit operator (int x, int Y)(Coordinate2D a) => (a.X, a.Y);
    }

    public record Coordinate2DLong(long X, long Y)
    {
        
        public long ManhattenDistance(Coordinate2DLong other) => (long)(Math.Abs(X - other.X) + Math.Abs(Y - other.Y));
        public long Magnitude() => Math.Abs(X) + Math.Abs(Y);
        public Coordinate2DLong Vector(Coordinate2DLong other) => new(other.X - X, other.Y - Y);

        public (long, long) Difference(Coordinate2DLong other)
        {
            long xDiff = this.X - other.X;
            long yDiff = this.Y - other.Y;

            return (xDiff, yDiff);
        }

        public List<Coordinate2DLong> Neighbours(bool diagonals, bool self)
        {
            var tmp = new List<Coordinate2DLong>();

            if (diagonals) tmp.Add(new Coordinate2DLong(X - 1, Y - 1));     //top left
            tmp.Add(new Coordinate2DLong(X, Y - 1));                        //top
            if (diagonals) tmp.Add(new Coordinate2DLong(X + 1, Y - 1));     //top right
            tmp.Add(new Coordinate2DLong(X - 1, Y));                        //left
            if (self) tmp.Add(new Coordinate2DLong(X, Y));                  //center
            tmp.Add(new Coordinate2DLong(X + 1, Y));                        //right
            if (diagonals) tmp.Add(new Coordinate2DLong(X - 1, Y + 1));     //bottom left
            tmp.Add(new Coordinate2DLong(X, Y + 1));                        //bottom 
            if (diagonals) tmp.Add(new Coordinate2DLong(X + 1, Y + 1));     //bottom right            
            return tmp;
        }

        public static Coordinate2DLong operator +(Coordinate2DLong a) => a;
        public static Coordinate2DLong operator +(Coordinate2DLong a, Coordinate2DLong b) => new(a.X + b.X, a.Y + b.Y);
        public static Coordinate2DLong operator -(Coordinate2DLong a) => new(-a.X, -a.Y);
        public static Coordinate2DLong operator -(Coordinate2DLong a, Coordinate2DLong b) => a + (-b);
        public static Coordinate2DLong operator *(int scale, Coordinate2DLong a) => new(scale * a.X, scale * a.Y);

        public static implicit operator Coordinate2DLong((long x, long y) a) => new(a.x, a.y);

        public static implicit operator (long x, long Y)(Coordinate2DLong a) => (a.X, a.Y);
    }

    public record Coordinate3D(int X, int Y, int Z)
    {
        public int ManhattenDistance(Coordinate3D other) => (int)(Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z));
        public int Magnitude() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        public Coordinate3D Vector(Coordinate3D other) => new(other.X - X, other.Y - Y, other.Z - Z);
        public Coordinate3D Translate(Coordinate3D translation) => new(X + translation.X, Y + translation.Y, Z + translation.Z);
    }

}
