using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdventOfCode
{
    internal sealed class ExpectedAnswers
    {
        private sealed record AnswerFile(
            [property: JsonPropertyName("$schema")] string Schema,
            List<Entry> Puzzles);

        private sealed record Entry(int Year, int Day, string Part1, string Part2);

        private readonly Dictionary<(int Year, int Day), Entry> answers = new();

        public static ExpectedAnswers Load()
        {
            string[] locations =
            {
                Path.GetFullPath("answers.json"),
                Path.GetFullPath(Path.Combine("AocSolutions", "answers.json")),
                Path.Combine(AppContext.BaseDirectory, "answers.json")
            };
            string path = locations.FirstOrDefault(File.Exists);
            var result = new ExpectedAnswers();
            if (path == null)
                return result;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
                };
                AnswerFile file = JsonSerializer.Deserialize<AnswerFile>(File.ReadAllText(path), options);
                if (file?.Puzzles == null)
                    throw new FormatException("A 'puzzles' array is required.");

                foreach (Entry entry in file.Puzzles)
                {
                    if (entry == null || entry.Year < 2015 || entry.Day < 1 || entry.Day > 25)
                        throw new FormatException("Each puzzle must have a year of at least 2015 and a day between 1 and 25.");
                    if (!result.answers.TryAdd((entry.Year, entry.Day), entry))
                        throw new FormatException($"Duplicate answers for {entry.Year} day {entry.Day:00}.");
                }
            }
            catch (Exception ex) when (ex is JsonException || ex is FormatException)
            {
                throw new FormatException($"Invalid answers file '{path}': {ex.Message}", ex);
            }

            return result;
        }

        public string GetAnswer(int year, int day, int part)
        {
            if (!answers.TryGetValue((year, day), out Entry entry))
                return null;
            return part == 1 ? entry.Part1 : entry.Part2;
        }

        public static string Reconcile(string actual, string expected)
        {
            if (expected == null)
                return "Unknown";
            if (actual == null)
                return "Missing";
            return Normalize(actual).Equals(Normalize(expected), StringComparison.Ordinal) ? "Match" : "Mismatch";
        }

        private static string Normalize(string answer)
        {
            return answer.Replace("\r\n", "\n").Replace('\r', '\n').TrimEnd('\n');
        }
    }
}
