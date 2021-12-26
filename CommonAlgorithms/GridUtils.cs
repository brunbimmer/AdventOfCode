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

        public List<Coordinate2D> NeighboursIncludingSelf(bool diagonals, bool self)
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
    }

}
