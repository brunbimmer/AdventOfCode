using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 25)]
    public class Year2020Day25 : IAdventOfCode
    {
        private const long Modulus = 20_201_227;
        private const long SubjectNumber = 7;

        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day25()
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
            long[] publicKeys = FileIOHelper.getInstance().ReadDataAsLines(file)
                .Where(line => !string.IsNullOrWhiteSpace(line)).Select(long.Parse).ToArray();
            if (publicKeys.Length != 2 || publicKeys.Any(key => key <= 0 || key >= Modulus))
                throw new FormatException($"Exactly two public keys between 1 and {Modulus - 1} are required.");

            _SW.Restart();
            int cardLoopSize = FindLoopSize(publicKeys[0]);
            long encryptionKey = (long)BigInteger.ModPow(publicKeys[1], cardLoopSize, Modulus);
            _SW.Stop();

            output.WriteDiagnostic($"Card loop size: {cardLoopSize}");
            output.WritePart(1, encryptionKey, trackTime ? _SW.Elapsed : null, "Encryption key");
        }

        private int FindLoopSize(long publicKey)
        {
            long value = 1;
            for (int loopSize = 0; loopSize < Modulus; loopSize++)
            {
                if (value == publicKey)
                    return loopSize;
                value = value * SubjectNumber % Modulus;
            }

            throw new InvalidOperationException("The public key cannot be produced by transforming subject number 7.");
        }
    }
}
