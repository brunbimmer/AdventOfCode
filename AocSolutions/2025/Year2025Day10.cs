using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2025, Day = 10)]
    public class Year2025Day10 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2025Day10()
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
            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();

            var machines = ParseMachines(input);

            _SW.Stop();
            Console.WriteLine($"  Parsed {machines.Count} machines");
            Console.WriteLine("  Execution Time to Prepare Data: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long part1 = SolvePart1(machines);

            _SW.Stop();
            Console.WriteLine($"  Part 1 - Total minimum button presses: {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            long part2 = SolvePart2(machines);

            _SW.Stop();
            Console.WriteLine($"  Part 2 - Total minimum button presses for joltage configuration: {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
        }

        private class Machine
        {
            public string Target { get; set; }
            public List<List<int>> Buttons { get; set; }
            public List<int> JoltageRequirements { get; set; }
        }

        private List<Machine> ParseMachines(string[] lines)
        {
            var machines = new List<Machine>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Parse: [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
                var targetMatch = System.Text.RegularExpressions.Regex.Match(line, @"\[(.*?)\]");
                if (!targetMatch.Success)
                    continue;

                var target = targetMatch.Groups[1].Value;
                
                var buttonMatches = System.Text.RegularExpressions.Regex.Matches(line, @"\((.*?)\)");
                var buttons = new List<List<int>>();
                
                foreach (System.Text.RegularExpressions.Match match in buttonMatches)
                {
                    var indices = match.Groups[1].Value.Split(',')
                        .Select(s => int.Parse(s.Trim()))
                        .ToList();
                    buttons.Add(indices);
                }

                // Parse joltage requirements from {3,5,4,7}
                var joltageMatch = System.Text.RegularExpressions.Regex.Match(line, @"\{(.*?)\}");
                var joltageRequirements = new List<int>();
                if (joltageMatch.Success)
                {
                    joltageRequirements = joltageMatch.Groups[1].Value.Split(',')
                        .Select(s => int.Parse(s.Trim()))
                        .ToList();
                }

                machines.Add(new Machine 
                { 
                    Target = target, 
                    Buttons = buttons,
                    JoltageRequirements = joltageRequirements
                });
            }

            return machines;
        }

        private long SolvePart1(List<Machine> machines)
        {
            long totalPresses = 0;

            foreach (var machine in machines)
            {
                int minPresses = FindMinimumPresses(machine);
                totalPresses += minPresses;
            }

            return totalPresses;
        }

        private int FindMinimumPresses(Machine machine)
        {
            // Convert target to array of bools (true = on, false = off)
            var targetState = machine.Target.Select(c => c == '#').ToArray();
            int numLights = targetState.Length;

            // Use BFS to find the minimum number of button presses
            var queue = new Queue<(int[] state, int presses)>();
            var visited = new HashSet<string>();

            var initialState = new int[numLights];
            string initialKey = string.Join("", initialState);
            queue.Enqueue((initialState, 0));
            visited.Add(initialKey);

            while (queue.Count > 0)
            {
                var (currentState, presses) = queue.Dequeue();

                // Check if current state matches target
                bool matches = true;
                for (int i = 0; i < numLights; i++)
                {
                    if ((currentState[i] % 2 == 1) != targetState[i])
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                    return presses;

                // Try pressing each button once
                for (int buttonIdx = 0; buttonIdx < machine.Buttons.Count; buttonIdx++)
                {
                    var nextState = (int[])currentState.Clone();
                    
                    // Toggle the lights for this button
                    foreach (var lightIdx in machine.Buttons[buttonIdx])
                    {
                        nextState[lightIdx]++;
                    }

                    string stateKey = string.Join(",", nextState.Select(s => s % 2));
                    if (!visited.Contains(stateKey))
                    {
                        visited.Add(stateKey);
                        queue.Enqueue((nextState, presses + 1));
                    }
                }
            }

            return -1; // No solution found
        }

        private long SolvePart2(List<Machine> machines)
        {
            long totalPresses = 0;

            foreach (var machine in machines)
            {
                long minPresses = FindMinimumPressesJoltage(machine);
                totalPresses += minPresses;
            }

            return totalPresses;
        }

        private long FindMinimumPressesJoltage(Machine machine)
        {
            int numButtons = machine.Buttons.Count;
            int numCounters = machine.JoltageRequirements.Count;

            if (numButtons == 0 || numCounters == 0)
                return 0;

            // Build coefficient matrix for real numbers
            double[,] matrix = new double[numCounters, numButtons + 1];
            
            for (int i = 0; i < numCounters; i++)
            {
                for (int j = 0; j < numButtons; j++)
                {
                    matrix[i, j] = machine.Buttons[j].Contains(i) ? 1.0 : 0.0;
                }
                matrix[i, numButtons] = machine.JoltageRequirements[i];
            }

            // Gaussian elimination to find pivots and free variables
            var (reducedMatrix, pivotCols) = GaussianEliminationReal(matrix, numCounters, numButtons);
            
            // Find free variables (non-pivot columns)
            var freeVarIndices = new List<int>();
            var pivotSet = new HashSet<int>(pivotCols);
            for (int j = 0; j < numButtons; j++)
            {
                if (!pivotSet.Contains(j))
                    freeVarIndices.Add(j);
            }

            // If no free variables, solve directly
            if (freeVarIndices.Count == 0)
            {
                return SolveDirectBackSubstitution(reducedMatrix, pivotCols, numButtons, machine.JoltageRequirements);
            }

            // With free variables, enumerate possible values
            return EnumerateFreeVariables(reducedMatrix, pivotCols, freeVarIndices, numButtons, machine.JoltageRequirements);
        }

        private (double[,], List<int>) GaussianEliminationReal(double[,] matrix, int rows, int cols)
        {
            var mat = (double[,])matrix.Clone();
            var pivots = new List<int>();
            
            int currentRow = 0;
            for (int col = 0; col < cols && currentRow < rows; col++)
            {
                // Find pivot
                int pivotRow = -1;
                double maxVal = 1e-10;
                for (int row = currentRow; row < rows; row++)
                {
                    if (Math.Abs(mat[row, col]) > maxVal)
                    {
                        maxVal = Math.Abs(mat[row, col]);
                        pivotRow = row;
                    }
                }

                if (pivotRow == -1)
                    continue; // No pivot in this column

                // Swap rows
                for (int j = 0; j <= cols; j++)
                {
                    double temp = mat[currentRow, j];
                    mat[currentRow, j] = mat[pivotRow, j];
                    mat[pivotRow, j] = temp;
                }

                // Normalize pivot row
                double pivot = mat[currentRow, col];
                for (int j = 0; j <= cols; j++)
                    mat[currentRow, j] /= pivot;

                // Eliminate column in other rows
                for (int row = 0; row < rows; row++)
                {
                    if (row != currentRow && Math.Abs(mat[row, col]) > 1e-10)
                    {
                        double factor = mat[row, col];
                        for (int j = 0; j <= cols; j++)
                            mat[row, j] -= factor * mat[currentRow, j];
                    }
                }

                pivots.Add(col);
                currentRow++;
            }

            return (mat, pivots);
        }

        private long SolveDirectBackSubstitution(double[,] matrix, List<int> pivots, int numButtons, List<int> targets)
        {
            var solution = new double[numButtons];
            
            // For each pivot, get the value from the augmented column
            for (int i = 0; i < pivots.Count; i++)
            {
                solution[pivots[i]] = matrix[i, numButtons];
            }

            // Check if valid (all non-negative integers)
            long total = 0;
            for (int i = 0; i < numButtons; i++)
            {
                double val = solution[i];
                if (Math.Abs(val - Math.Round(val)) > 1e-9 || val < -1e-9)
                    return 0;
                
                long intVal = (long)Math.Round(val);
                if (intVal < 0)
                    return 0;
                
                total += intVal;
            }

            return total;
        }

        private long EnumerateFreeVariables(double[,] matrix, List<int> pivots, List<int> freeVars, 
            int numButtons, List<int> targets)
        {
            long minTotal = long.MaxValue;
            int maxVal = targets.Count > 0 ? targets.Max() : 50;
            
            // Create pivot map for quick lookup
            var pivotMap = new Dictionary<int, int>();
            for (int i = 0; i < pivots.Count; i++)
                pivotMap[pivots[i]] = i;

            // Enumerate all combinations of free variable values
            EnumerateFreeRec(matrix, pivotMap, freeVars.ToArray(), 0, new int[freeVars.Count],
                0, maxVal, targets.Max() * 2, numButtons, ref minTotal);

            return minTotal == long.MaxValue ? 0 : minTotal;
        }

        private void EnumerateFreeRec(double[,] matrix, Dictionary<int, int> pivotMap, int[] freeVars,
            int freeIdx, int[] freeValues, long currentSum, int maxVal, int maxSum,
            int numButtons, ref long minTotal)
        {
            if (currentSum >= minTotal || currentSum > maxSum)
                return;

            if (freeIdx == freeVars.Length)
            {
                // Compute dependent variables
                var solution = new double[numButtons];
                
                // Set free variables
                for (int i = 0; i < freeVars.Length; i++)
                    solution[freeVars[i]] = freeValues[i];

                // Back-substitute for pivot variables
                bool valid = true;
                long total = currentSum;
                
                foreach (var pivotCol in pivotMap.Keys)
                {
                    int row = pivotMap[pivotCol];
                    double value = matrix[row, numButtons];
                    
                    // Subtract contributions from free variables
                    for (int i = 0; i < freeVars.Length; i++)
                    {
                        value -= matrix[row, freeVars[i]] * freeValues[i];
                    }

                    solution[pivotCol] = value;
                    
                    // Check if integer and non-negative
                    if (Math.Abs(value - Math.Round(value)) > 1e-9 || value < -1e-9)
                    {
                        valid = false;
                        break;
                    }

                    total += (long)Math.Round(value);
                }

                if (valid && total < minTotal)
                    minTotal = total;
                
                return;
            }

            // Try values for this free variable
            for (int val = 0; val <= maxVal; val++)
            {
                freeValues[freeIdx] = val;
                EnumerateFreeRec(matrix, pivotMap, freeVars, freeIdx + 1, freeValues,
                    currentSum + val, maxVal, maxSum, numButtons, ref minTotal);
            }
        }


    }
}
