using System;
using System.Collections.Generic;
using System.Globalization;

namespace AdventOfCode
{
    public interface IPuzzleOutput
    {
        void WritePart(int part, object answer, TimeSpan? elapsed = null, string description = null);
        void WriteDiagnostic(string message);
    }

    public sealed record PuzzlePart(string Answer, TimeSpan? Elapsed, string Description);

    public sealed class ConsolePuzzleOutput : IPuzzleOutput
    {
        private readonly bool trackTime;
        private readonly bool verbose;

        public ConsolePuzzleOutput(bool trackTime = false, bool verbose = false)
        {
            this.trackTime = trackTime;
            this.verbose = verbose;
        }

        public void WritePart(int part, object answer, TimeSpan? elapsed = null, string description = null)
        {
            string value = Convert.ToString(answer ?? throw new ArgumentNullException(nameof(answer)), CultureInfo.InvariantCulture);
            string label = string.IsNullOrEmpty(description) ? $"Part {part}" : $"Part {part} - {description}";
            Console.WriteLine(value.Contains('\n') || value.Contains('\r') ? $"{label}\n{value}" : $"{label}: {value}");
            if (trackTime && elapsed.HasValue)
                Console.WriteLine($"  Execution time {elapsed.Value.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)} ms");
        }

        public void WriteDiagnostic(string message)
        {
            if (verbose)
                Console.WriteLine(message);
        }
    }

    public sealed class CollectingPuzzleOutput : IPuzzleOutput
    {
        private readonly Dictionary<int, PuzzlePart> parts = new();
        private readonly List<string> diagnostics = new();
        private readonly bool verbose;

        public CollectingPuzzleOutput(bool verbose = false)
        {
            this.verbose = verbose;
        }

        public IReadOnlyDictionary<int, PuzzlePart> Parts => parts;
        public IReadOnlyList<string> Diagnostics => diagnostics;

        public void WritePart(int part, object answer, TimeSpan? elapsed = null, string description = null)
        {
            if (part != 1 && part != 2)
                throw new ArgumentOutOfRangeException(nameof(part), "Puzzle parts must be 1 or 2.");
            string value = Convert.ToString(answer ?? throw new ArgumentNullException(nameof(answer)), CultureInfo.InvariantCulture);
            parts.Add(part, new PuzzlePart(value, elapsed, description));
        }

        public void WriteDiagnostic(string message)
        {
            if (verbose)
                diagnostics.Add(message);
        }
    }
}
