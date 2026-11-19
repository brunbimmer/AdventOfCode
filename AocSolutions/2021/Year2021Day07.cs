using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2021, Day = 7)]
    public class Year2021Day7 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2021Day7()
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

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string input = FileIOHelper.getInstance().ReadDataAsString(file);

            int[] crabPositions = Array.ConvertAll(input.Split(','), s => int.Parse(s));

            int median = GetMedian(crabPositions);
            int average = GetAverage(crabPositions);
            int centerOfMass = GetCenterOfMass(crabPositions);
            double fuelCost = 0;

            _SW.Start();            

            fuelCost = GetFuelCost(median, crabPositions, true); 
            
            _SW.Stop();

            output.WritePart(1, fuelCost, trackTime ? _SW.Elapsed : null, "Fuel Cost");
            output.WriteDiagnostic(string.Format("Fuel Cost - Part 1 (Median: {0}): {1}", centerOfMass, fuelCost));
            output.WriteDiagnostic(string.Format("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW)));

            _SW.Restart();

            fuelCost = GetFuelCost(average, crabPositions, false);

            output.WritePart(2, fuelCost, description: "Fuel Cost using average position");
            output.WriteDiagnostic(string.Format("Fuel Cost - Part 2 (Average: {0}): {1}", average, fuelCost));

            fuelCost = GetFuelCost(centerOfMass, crabPositions, false);

            output.WriteDiagnostic(string.Format("Fuel Cost - Part 2 (Center of Mass: {0}): {1}", centerOfMass, fuelCost));
            
            _SW.Stop();

            output.WriteDiagnostic(string.Format("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW)));
            output.WriteDiagnostic("");
            output.WriteDiagnostic("I am showing two possible fuel cost answers to the Part 2. One using average and another using center of mass.");
            output.WriteDiagnostic("One of these options will result in the lower fuel cost. I am displaying both for comparision purposes.");


        }        

        private   int GetMedian(int[] crabs)
        {
            int numberCount = crabs.Count();
            int halfIndex = crabs.Count()/2;
            var sortedNumbers = crabs.OrderBy(n=>n);
            double median;
            if ((numberCount % 2) == 0)
            {
                median = ((sortedNumbers.ElementAt(halfIndex) +
                           sortedNumbers.ElementAt(halfIndex - 1))/ 2);
            } else {
                median = sortedNumbers.ElementAt(halfIndex);
            }
            return Convert.ToInt32(median);
        }

        private   int GetAverage(int[] crabs)
        {
            return Convert.ToInt32(Math.Round(crabs.Average()));
        }

        private   int GetCenterOfMass(int[] crabs)
        {
            var massPoints = crabs.Distinct().ToDictionary(x => x, x => crabs.Count(y => y == x));

            int centerOfMass = massPoints.Sum(x => x.Key * x.Value) / massPoints.Sum(x => x.Value);

            return centerOfMass;
        }

        private   double GetFuelCost(int position, int[] crabs, bool linearFuelConsumption)
        {
            double fuelCost = 0;
            for (int i = 0; i < crabs.Length; i++)
            {
                if (linearFuelConsumption)
                    fuelCost = fuelCost + Math.Abs(crabs[i] - position);
                else
                    fuelCost = fuelCost + Convert.ToDouble(Enumerable.Range(1, Math.Abs(crabs[i] - position)).Sum());
            }

            return fuelCost;
        }
    }
}
