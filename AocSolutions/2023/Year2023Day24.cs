using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2023, Day = 24)]
    public class Year2023Day24: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day24()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            var hailstones = ParseHailstones(lines);

            _SW.Start();

            long part1 = SolvePart1(hailstones);

            _SW.Stop();

            Console.WriteLine($"  Part 1: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            long part2 = SolvePart2(hailstones);

            _SW.Stop();

            Console.WriteLine($"  Part 2: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private struct Hailstone
        {
            public double Px, Py, Pz;
            public double Vx, Vy, Vz;
        }

        private List<Hailstone> ParseHailstones(string[] lines)
        {
            var hailstones = new List<Hailstone>();
            
            foreach (var line in lines)
            {
                // Parse: "px, py, pz @ vx, vy, vz"
                var parts = line.Split('@');
                var posStr = parts[0].Trim().Split(',').Select(s => s.Trim()).ToArray();
                var velStr = parts[1].Trim().Split(',').Select(s => s.Trim()).ToArray();
                
                hailstones.Add(new Hailstone
                {
                    Px = double.Parse(posStr[0]),
                    Py = double.Parse(posStr[1]),
                    Pz = double.Parse(posStr[2]),
                    Vx = double.Parse(velStr[0]),
                    Vy = double.Parse(velStr[1]),
                    Vz = double.Parse(velStr[2])
                });
            }
            
            return hailstones;
        }

        private long SolvePart1(List<Hailstone> hailstones)
        {
            long min = 200000000000000;
            long max = 400000000000000;
            int count = 0;
            
            for (int i = 0; i < hailstones.Count; i++)
            {
                for (int j = i + 1; j < hailstones.Count; j++)
                {
                    if (CheckIntersection(hailstones[i], hailstones[j], min, max))
                        count++;
                }
            }
            
            return count;
        }

        private bool CheckIntersection(Hailstone a, Hailstone b, long minPos, long maxPos)
        {
            // Line A: (x, y) = (Px, Py) + t * (Vx, Vy)
            // Line B: (x, y) = (Px, Py) + s * (Vx, Vy)
            
            double denom = a.Vx * b.Vy - a.Vy * b.Vx;
            
            // Parallel lines
            if (Math.Abs(denom) < 1e-9)
                return false;
            
            // Solve for t and s
            double t = ((b.Px - a.Px) * b.Vy - (b.Py - a.Py) * b.Vx) / denom;
            double s = ((b.Px - a.Px) * a.Vy - (b.Py - a.Py) * a.Vx) / denom;
            
            // Check if intersection is in the future for both hailstones
            if (t < 0 || s < 0)
                return false;
            
            // Calculate intersection point
            double x = a.Px + t * a.Vx;
            double y = a.Py + t * a.Vy;
            
            // Check if within test area
            return x >= minPos && x <= maxPos && y >= minPos && y <= maxPos;
        }

        private long SolvePart2(List<Hailstone> hailstones)
        {
            // Use linear algebra approach with cross products
            // View everything relative to hailstone 0
            var h0 = hailstones[0];
            var h1 = hailstones[1];
            var h2 = hailstones[2];
            
            // Relative positions and velocities (from stone 0's perspective)
            var p1 = new Vector3(h1.Px - h0.Px, h1.Py - h0.Py, h1.Pz - h0.Pz);
            var v1 = new Vector3(h1.Vx - h0.Vx, h1.Vy - h0.Vy, h1.Vz - h0.Vz);
            
            var p2 = new Vector3(h2.Px - h0.Px, h2.Py - h0.Py, h2.Pz - h0.Pz);
            var v2 = new Vector3(h2.Vx - h0.Vx, h2.Vy - h0.Vy, h2.Vz - h0.Vz);
            
            // Calculate times using cross product and dot product
            var p1xp2 = Vector3.Cross(p1, p2);
            var v1xp2 = Vector3.Cross(v1, p2);
            var p1xv2 = Vector3.Cross(p1, v2);
            
            double denom1 = Vector3.Dot(v1xp2, v2);
            double denom2 = Vector3.Dot(p1xv2, v1);
            
            if (Math.Abs(denom1) < 1e-9 || Math.Abs(denom2) < 1e-9)
                return 0;
            
            double t1 = -Vector3.Dot(p1xp2, v2) / denom1;
            double t2 = -Vector3.Dot(p1xp2, v1) / denom2;
            
            if (t1 < -1e-6 || t2 < -1e-6)
                return 0;
            
            // Calculate collision points in absolute coordinates
            var c1 = new Vector3(
                h1.Px + t1 * h1.Vx,
                h1.Py + t1 * h1.Vy,
                h1.Pz + t1 * h1.Vz
            );
            
            var c2 = new Vector3(
                h2.Px + t2 * h2.Vx,
                h2.Py + t2 * h2.Vy,
                h2.Pz + t2 * h2.Vz
            );
            
            // Calculate rock velocity and position
            double timeDiff = t2 - t1;
            if (Math.Abs(timeDiff) < 1e-9)
                return 0;
            
            var rockVel = (c2 - c1) * (1.0 / timeDiff);
            var rockPos = c1 - rockVel * t1;
            
            // Verify with other hailstones (just a sanity check, not strict)
            for (int i = 3; i < Math.Min(hailstones.Count, 5); i++)
            {
                var h = hailstones[i];
                var hPos = new Vector3(h.Px, h.Py, h.Pz);
                var hVel = new Vector3(h.Vx, h.Vy, h.Vz);
                
                // rockPos + t * rockVel = hPos + t * hVel
                // rockPos - hPos = t * (hVel - rockVel)
                var diff = rockPos - hPos;
                var velDiff = hVel - rockVel;
                
                // Try to find collision time
                double t = -1;
                if (Math.Abs(velDiff.X) > 1e-6)
                    t = diff.X / velDiff.X;
                else if (Math.Abs(velDiff.Y) > 1e-6)
                    t = diff.Y / velDiff.Y;
                else if (Math.Abs(velDiff.Z) > 1e-6)
                    t = diff.Z / velDiff.Z;
                
                if (t < -1e-3)
                    return 0;
            }
            
            return (long)Math.Round(rockPos.X + rockPos.Y + rockPos.Z);
        }
        
        private struct Vector3
        {
            public double X, Y, Z;
            
            public Vector3(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }
            
            public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            public static Vector3 operator *(Vector3 a, double s) => new Vector3(a.X * s, a.Y * s, a.Z * s);
            public static Vector3 operator *(double s, Vector3 a) => new Vector3(a.X * s, a.Y * s, a.Z * s);
            public static Vector3 operator /(Vector3 a, double s) => new Vector3(a.X / s, a.Y / s, a.Z / s);
            
            public static Vector3 Cross(Vector3 a, Vector3 b)
            {
                return new Vector3(
                    a.Y * b.Z - a.Z * b.Y,
                    a.Z * b.X - a.X * b.Z,
                    a.X * b.Y - a.Y * b.X
                );
            }
            
            public static double Dot(Vector3 a, Vector3 b)
            {
                return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            }
            
            public static explicit operator Vector3(double t) => new Vector3(t, t, t);
        }

        private long? TryVelocity(List<Hailstone> hailstones, long vx, long vy, long vz)
        {
            // Transform all hailstones to rock's reference frame
            // In rock's frame, rock is stationary at origin
            // Hailstone i moves with velocity (Vxi - vx, Vyi - vy, Vzi - vz)
            
            // Find intersection of first two hailstones in this frame
            var h0 = hailstones[0];
            var h1 = hailstones[1];
            
            double dvx0 = h0.Vx - vx;
            double dvy0 = h0.Vy - vy;
            double dvz0 = h0.Vz - vz;
            
            double dvx1 = h1.Vx - vx;
            double dvy1 = h1.Vy - vy;
            double dvz1 = h1.Vz - vz;
            
            // Both must have non-zero velocity in rock frame
            if (Math.Abs(dvx0) < 1e-9 || Math.Abs(dvx1) < 1e-9)
                return null;
            
            // Find time t when x positions match
            // h0.Px + t * dvx0 = h1.Px + t * dvx1
            // t * (dvx0 - dvx1) = h1.Px - h0.Px
            double t = (h1.Px - h0.Px) / (dvx0 - dvx1);
            
            if (t < 0)
                return null;
            
            // Calculate position at this time
            double px = h0.Px + t * dvx0;
            double py = h0.Py + t * dvy0;
            double pz = h0.Pz + t * dvz0;
            
            // Verify all other hailstones intersect at this point
            for (int i = 2; i < hailstones.Count; i++)
            {
                var h = hailstones[i];
                double dvxi = h.Vx - vx;
                double dvyi = h.Vy - vy;
                double dvzi = h.Vz - vz;
                
                if (Math.Abs(dvxi) < 1e-9)
                    return null;
                
                double ti = (px - h.Px) / dvxi;
                
                if (ti < 0)
                    return null;
                
                double yi = h.Py + ti * dvyi;
                double zi = h.Pz + ti * dvzi;
                
                if (Math.Abs(yi - py) > 1e-6 || Math.Abs(zi - pz) > 1e-6)
                    return null;
            }
            
            return (long)(px + py + pz);
        }
    }
}
