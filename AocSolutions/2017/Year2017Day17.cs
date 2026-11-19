using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2017, Day = 17)]
    public class Year2017Day17 : IAdventOfCode
    {
        private readonly int _Year;
        private readonly int _Day;
        private readonly string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2017Day17()
        {
            var attribute = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));
            _Year = attribute.Year;
            _Day = attribute.Day;
            _OverrideFile = attribute.OverrideTestFile;
            _SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            int step = int.Parse(FileIOHelper.getInstance().ReadDataAsString(file).Trim());
            if (step < 0) throw new FormatException("The spinlock step size must be non-negative.");
            _SW.Restart();
            int first = ValueAfterLast(step, 2017);
            _SW.Stop();
            output.WritePart(1, first, trackTime ? _SW.Elapsed : null, "Value after the final spinlock insertion");
            _SW.Restart();
            int second = ValueAfterZero(step, 50_000_000);
            _SW.Stop();
            output.WritePart(2, second, trackTime ? _SW.Elapsed : null, "Value after zero following fifty million insertions");
        }

        private int ValueAfterLast(int step, int insertions)
        {
            var buffer = new List<int> { 0 };
            int position = 0;
            for (int value = 1; value <= insertions; value++)
            {
                position = (int)((position + (long)step) % buffer.Count) + 1;
                buffer.Insert(position, value);
            }
            return buffer[(position + 1) % buffer.Count];
        }

        private int ValueAfterZero(int step, int insertions)
        {
            int position = 0;
            int afterZero = 0;
            for (int value = 1; value <= insertions; value++)
            {
                position = (int)((position + (long)step) % value) + 1;
                if (position == 1) afterZero = value;
            }
            return afterZero;
        }
    }
}
