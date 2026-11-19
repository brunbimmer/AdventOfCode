using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 13)]
    public class Year2020Day13 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day13()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);
            long earliestDeparture = long.Parse(lines[0]);
            var buses = lines[1].Split(',')
                .Select((id, offset) => (Id: id, Offset: offset))
                .Where(bus => bus.Id != "x")
                .Select(bus => (Id: long.Parse(bus.Id), Offset: bus.Offset))
                .ToArray();

            _SW.Restart();
            long result = Part1(earliestDeparture, buses);
            _SW.Stop();

            output.WritePart(1, result, trackTime ? _SW.Elapsed : null, "Earliest bus ID multiplied by waiting time");


            _SW.Restart();
            BigInteger timestamp = Part2(buses);
            _SW.Stop();

            output.WritePart(2, timestamp, trackTime ? _SW.Elapsed : null, "Earliest timestamp matching all bus offsets");

        }

        private long Part1(long earliestDeparture, (long Id, int Offset)[] buses)
        {
            if (buses.Length == 0)
                throw new InvalidOperationException("No active buses were found in the schedule.");

            long bestBusId = 0;
            long shortestWait = long.MaxValue;
            foreach (var bus in buses)
            {
                long busId = bus.Id;
                long wait = (busId - earliestDeparture % busId) % busId;
                if (wait < shortestWait)
                {
                    bestBusId = busId;
                    shortestWait = wait;
                }
            }

            return checked(bestBusId * shortestWait);
        }

        private BigInteger Part2((long Id, int Offset)[] buses)
        {
            BigInteger timestamp = 0;
            BigInteger period = 1;

            foreach (var bus in buses)
            {
                if (bus.Id <= 0)
                    throw new InvalidOperationException("Bus IDs must be positive.");

                long gcd = Numerics.Gcd((long)(period % bus.Id), bus.Id);
                BigInteger difference = -bus.Offset - timestamp;
                if (difference % gcd != 0)
                    throw new InvalidOperationException("The bus offsets have no matching timestamp.");

                // Solve period * increments = -offset - timestamp (mod bus ID).
                BigInteger modulus = bus.Id / gcd;
                BigInteger inverse = ModularInverse(period / gcd, modulus);
                BigInteger increments = (difference / gcd * inverse) % modulus;
                if (increments < 0)
                    increments += modulus;

                timestamp += period * increments;
                period *= modulus;
            }

            return timestamp;
        }

        private BigInteger ModularInverse(BigInteger value, BigInteger modulus)
        {
            BigInteger remainder = value % modulus;
            BigInteger nextRemainder = modulus;
            BigInteger coefficient = 1;
            BigInteger nextCoefficient = 0;

            while (nextRemainder != 0)
            {
                BigInteger quotient = remainder / nextRemainder;
                (remainder, nextRemainder) = (nextRemainder, remainder - quotient * nextRemainder);
                (coefficient, nextCoefficient) = (nextCoefficient, coefficient - quotient * nextCoefficient);
            }

            return (coefficient % modulus + modulus) % modulus;
        }
    }
}
