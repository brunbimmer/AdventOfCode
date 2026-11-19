using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 20)]
    public class Year2017Day20 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day20()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        private sealed record Particle(long[] Position, long[] Velocity, long[] Acceleration);

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            Particle[] particles = ParseParticles(FileIOHelper.getInstance().ReadDataAsLines(file));
            if (particles.Length == 0) throw new FormatException("At least one particle is required.");
            _SW.Restart();
            int closest = ClosestParticle(particles);
            _SW.Stop();
            output.WritePart(1, closest, trackTime ? _SW.Elapsed : null, "Particle closest to the origin in the long term");
            _SW.Restart();
            int survivors = Survivors(particles);
            _SW.Stop();
            output.WritePart(2, survivors, trackTime ? _SW.Elapsed : null, "Particles remaining after all collisions");
        }

        private Particle[] ParseParticles(string[] lines)
        {
            return lines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line =>
            {
                long[] values = Regex.Matches(line, @"-?\d+").Select(match => long.Parse(match.Value)).ToArray();
                if (values.Length != 9) throw new FormatException("Each particle needs position, velocity, and acceleration vectors.");
                return new Particle(values[..3], values[3..6], values[6..]);
            }).ToArray();
        }

        private int ClosestParticle(Particle[] particles)
        {
            return particles.Select((particle, index) =>
            {
                BigInteger quadratic = 0;
                BigInteger linear = 0;
                BigInteger constant = 0;
                for (int axis = 0; axis < 3; axis++)
                {
                    BigInteger a = particle.Acceleration[axis];
                    BigInteger b = a + 2 * (BigInteger)particle.Velocity[axis];
                    BigInteger c = 2 * (BigInteger)particle.Position[axis];
                    int sign = a != 0 ? a.Sign : b != 0 ? b.Sign : c.Sign;
                    quadratic += sign * a;
                    linear += sign * b;
                    constant += sign * c;
                }
                return (Index: index, Quadratic: quadratic, Linear: linear, Constant: constant);
            }).OrderBy(item => item.Quadratic).ThenBy(item => item.Linear).ThenBy(item => item.Constant)
                .ThenBy(item => item.Index).First().Index;
        }

        private int Survivors(Particle[] particles)
        {
            var events = new SortedDictionary<BigInteger, List<(int First, int Second)>>();
            for (int first = 0; first < particles.Length; first++)
            {
                for (int second = first + 1; second < particles.Length; second++)
                {
                    foreach (BigInteger time in CollisionTimes(particles[first], particles[second]))
                    {
                        if (!events.TryGetValue(time, out var pairs))
                            events.Add(time, pairs = new List<(int, int)>());
                        pairs.Add((first, second));
                    }
                }
            }
            var alive = Enumerable.Range(0, particles.Length).ToHashSet();
            foreach (var collision in events.Values)
            {
                var removed = new HashSet<int>();
                foreach (var pair in collision)
                {
                    if (alive.Contains(pair.First) && alive.Contains(pair.Second))
                    {
                        removed.Add(pair.First);
                        removed.Add(pair.Second);
                    }
                }
                alive.ExceptWith(removed);
            }
            return alive.Count;
        }

        private HashSet<BigInteger> CollisionTimes(Particle first, Particle second)
        {
            HashSet<BigInteger> times = null;
            for (int axis = 0; axis < 3; axis++)
            {
                BigInteger a = (BigInteger)first.Acceleration[axis] - second.Acceleration[axis];
                BigInteger b = a + 2 * ((BigInteger)first.Velocity[axis] - second.Velocity[axis]);
                BigInteger c = 2 * ((BigInteger)first.Position[axis] - second.Position[axis]);
                HashSet<BigInteger> axisTimes = SolveTimes(a, b, c);
                if (axisTimes == null) continue;
                if (times == null) times = axisTimes;
                else times.IntersectWith(axisTimes);
                if (times.Count == 0) return times;
            }
            return times ?? new HashSet<BigInteger> { BigInteger.Zero };
        }

        private HashSet<BigInteger> SolveTimes(BigInteger a, BigInteger b, BigInteger c)
        {
            if (a == 0 && b == 0)
                return c == 0 ? null : new HashSet<BigInteger>();
            var times = new HashSet<BigInteger>();
            void AddRoot(BigInteger numerator, BigInteger denominator)
            {
                if (numerator % denominator == 0 && numerator / denominator >= 0)
                    times.Add(numerator / denominator);
            }
            if (a == 0)
            {
                AddRoot(-c, b);
                return times;
            }
            BigInteger discriminant = b * b - 4 * a * c;
            if (discriminant < 0) return times;
            BigInteger root = SquareRoot(discriminant);
            if (root * root != discriminant) return times;
            AddRoot(-b + root, 2 * a);
            AddRoot(-b - root, 2 * a);
            return times;
        }

        private BigInteger SquareRoot(BigInteger value)
        {
            if (value == 0) return 0;
            BigInteger estimate = BigInteger.One << (int)((value.GetBitLength() + 1) / 2);
            while (true)
            {
                BigInteger next = (estimate + value / estimate) / 2;
                if (next >= estimate) return estimate;
                estimate = next;
            }
        }
    }
}
