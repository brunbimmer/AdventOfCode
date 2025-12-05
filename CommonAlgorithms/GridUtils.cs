using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum Axis
    {
        X,
        Y,
        Z,
        W
    }

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

        public List<Coordinate2D> Neighbours(bool diagonals = false, bool self = false)
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

        public Coordinate2D Move(Utilities.Direction dir, int dist = 1)
        {

            #pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            return dir switch
            {
                Utilities.Direction.N => new Coordinate2D(this.X - dist, this.Y),
                Utilities.Direction.S => new Coordinate2D(this.X + dist, this.Y),
                Utilities.Direction.E => new Coordinate2D(this.X, this.Y + dist),
                Utilities.Direction.W => new Coordinate2D(this.X, this.Y - dist),
            };
            #pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        }

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

        public List<Coordinate2DLong> Neighbours(bool diagonals = false, bool self = false)
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
        public int ManhattanDistance(Coordinate3D other) => (int)(Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z));
        public int Magnitude() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        public Coordinate3D Vector(Coordinate3D other) => new(other.X - X, other.Y - Y, other.Z - Z);
        public Coordinate3D Translate(Coordinate3D translation) => new(X + translation.X, Y + translation.Y, Z + translation.Z);

        public static implicit operator Coordinate3D((int x, int y, int z) a) => new(a.x, a.y, a.z);

        public static implicit operator (int x, int y, int z)(Coordinate3D a) => (a.X, a.Y, a.Z);
        public static Coordinate3D operator +(Coordinate3D a) => a;
        public static Coordinate3D operator +(Coordinate3D a, Coordinate3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Coordinate3D operator -(Coordinate3D a) => new(-a.X, -a.Y, -a.Z);
        public static Coordinate3D operator -(Coordinate3D a, Coordinate3D b) => a + (-b);
    }

}
