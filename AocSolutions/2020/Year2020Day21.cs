using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 21)]
    public class Year2020Day21 : IAdventOfCode
    {
        private sealed record Food(string[] Ingredients, string[] Allergens);

        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day21()
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
            Food[] foods = ParseFoods(FileIOHelper.getInstance().ReadDataAsLines(file));

            _SW.Restart();
            Dictionary<string, HashSet<string>> candidates = BuildAllergenCandidates(foods);
            HashSet<string> potentiallyAllergenicIngredients = candidates.Values.SelectMany(ingredients => ingredients)
                .ToHashSet(StringComparer.Ordinal);
            int safeOccurrences = foods.Sum(food => food.Ingredients.Count(ingredient => !potentiallyAllergenicIngredients.Contains(ingredient)));
            _SW.Stop();

            output.WritePart(1, safeOccurrences, trackTime ? _SW.Elapsed : null, "Occurrences of ingredients that cannot contain allergens");

            _SW.Restart();
            string dangerousIngredients = BuildDangerousIngredientList(candidates);
            _SW.Stop();

            output.WritePart(2, dangerousIngredients, trackTime ? _SW.Elapsed : null, "Canonical dangerous ingredient list");
        }

        private Food[] ParseFoods(string[] lines)
        {
            Food[] foods = lines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line =>
            {
                string[] parts = line.Trim().Split(" (contains ", 2, StringSplitOptions.None);
                if (parts.Length != 2 || !parts[1].EndsWith(")", StringComparison.Ordinal))
                    throw new FormatException("Each food must list ingredients followed by '(contains allergen, ...)'.");

                string[] ingredients = parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string[] allergens = parts[1][..^1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (ingredients.Length == 0 || allergens.Length == 0)
                    throw new FormatException("Each food must contain at least one ingredient and one listed allergen.");

                return new Food(ingredients, allergens);
            }).ToArray();

            if (foods.Length == 0)
                throw new FormatException("At least one food is required.");

            return foods;
        }

        private Dictionary<string, HashSet<string>> BuildAllergenCandidates(Food[] foods)
        {
            var candidates = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            foreach (Food food in foods)
            {
                foreach (string allergen in food.Allergens)
                {
                    if (candidates.TryGetValue(allergen, out HashSet<string> ingredients))
                        ingredients.IntersectWith(food.Ingredients);
                    else
                        candidates.Add(allergen, new HashSet<string>(food.Ingredients, StringComparer.Ordinal));
                }
            }

            return candidates;
        }

        private string BuildDangerousIngredientList(Dictionary<string, HashSet<string>> candidates)
        {
            var remaining = candidates.ToDictionary(entry => entry.Key,
                entry => new HashSet<string>(entry.Value, StringComparer.Ordinal), StringComparer.Ordinal);
            var assignments = new Dictionary<string, string>(StringComparer.Ordinal);

            while (remaining.Count > 0)
            {
                var resolved = remaining.FirstOrDefault(entry => entry.Value.Count == 1);
                if (resolved.Key == null)
                    throw new InvalidOperationException("Allergen assignments are inconsistent or cannot be uniquely determined.");

                string ingredient = resolved.Value.Single();
                assignments.Add(resolved.Key, ingredient);
                remaining.Remove(resolved.Key);
                foreach (HashSet<string> ingredients in remaining.Values)
                    ingredients.Remove(ingredient);
            }

            return string.Join(",", assignments.OrderBy(entry => entry.Key, StringComparer.Ordinal).Select(entry => entry.Value));
        }
    }
}
