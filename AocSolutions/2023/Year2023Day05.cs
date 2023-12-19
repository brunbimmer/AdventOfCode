using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;
using Microsoft.Extensions.Primitives;
using MoreLinq;
using MoreLinq.Extensions;

namespace AdventOfCode
{

    [AdventOfCode(Year = 2023, Day = 5)]
    public class Year2023Day05: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2023Day05()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        private long[] seeds;

        private List<long[]> seedToSoil = new List<long[]>();
        private List<long[]> soilToFert = new List<long[]>();
        private List<long[]> fertToWater = new List<long[]>();
        private List<long[]> waterToLight = new List<long[]>();
        private List<long[]> lightToTemp = new List<long[]>();
        private List<long[]> tempToHumidity = new List<long[]>();
        private List<long[]> humidityToLocation = new List<long[]>();

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 
            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            ParseInput(input);
           
            _SW.Start();

            long lowestLocationNumber = FindLowestLocationForSeedMap(seeds);

            Console.WriteLine($"  Part 1: Lowest Location: {lowestLocationNumber}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");


            _SW.Stop();


            _SW.Reset();
            _SW.Start();

            //long lowestPart2 = FindLowestLocationForSeedMapPart2(seeds);

            _SW.Stop();

            //Console.WriteLine($"  Part 2: Lowest Location: {lowestPart2}");
            Console.WriteLine($"   Execution Time: {StopwatchUtil.getInstance().GetTimestamp(_SW)}");
        }



        private void ParseInput(string[] lines)
        {
            bool _seedToSoil = false;
            bool _soilToFert = false;
            bool _fertToWater = false;
            bool _waterToLight = false;
            bool _lightToTemp = false;
            bool _tempToHumidity = false;
            bool _humidityToLocation = false;

            foreach (string line in lines)
            {
                if (line.StartsWith("seeds:"))
                {
                    seeds = Array.ConvertAll(line.Split("seeds:")[1].Trim().Split(' '), long.Parse);
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    _seedToSoil = false;
                    _soilToFert = false;
                    _fertToWater = false;
                    _waterToLight = false;
                    _lightToTemp = false;
                    _tempToHumidity = false;
                    _humidityToLocation = false;
                }
                else if (line.StartsWith("seed-to-soil") || _seedToSoil)
                {
                    if (_seedToSoil == false)
                    {
                        _seedToSoil = true;
                        continue;
                    }
                    else
                    {
                        seedToSoil.Add(Array.ConvertAll(line.Split(' '), long.Parse));
                    }
                }
                else if (line.StartsWith("soil-to-fertilizer") || _soilToFert)
                {
                    if (_soilToFert == false)
                    {
                        _soilToFert = true;
                        continue;
                    }
                    else
                    {
                        soilToFert.Add(Array.ConvertAll(line.Split(' '), long.Parse));
                    }
                }
                else if (line.StartsWith("fertilizer-to-water") || _fertToWater)
                {
                    if (_fertToWater == false)
                    {
                        _fertToWater = true;
                        continue;
                    }
                    else
                    {
                        fertToWater.Add(Array.ConvertAll(line.Split(' '), long.Parse));
                    }
                }
                else if (line.StartsWith("water-to-light") || _waterToLight)
                {
                    if (_waterToLight == false)
                    {
                        _waterToLight = true;
                        continue;
                    }
                    else
                    {
                        waterToLight.Add(Array.ConvertAll(line.Split(' '), long.Parse));
                    }
                }
                else if (line.StartsWith("light-to-temperature") || _lightToTemp)
                {
                    if (_lightToTemp == false)
                    {
                        _lightToTemp = true;
                        continue;
                    }
                    else
                    {
                        lightToTemp.Add(Array.ConvertAll(line.Split(' '), long.Parse));
                    }
                }
                else if (line.StartsWith("temperature-to-humidity") || _tempToHumidity)
                {
                    if (_tempToHumidity == false)
                    {
                        _tempToHumidity = true;
                        continue;
                    }
                    else
                    {
                        tempToHumidity.Add(Array.ConvertAll(line.Split(' '), long.Parse));
                    }
                }
                else if (line.StartsWith("humidity-to-location") || _humidityToLocation)
                {
                    if (_humidityToLocation == false)
                    {
                        _humidityToLocation = true;
                        continue;
                    }
                    else
                    {
                        humidityToLocation.Add(Array.ConvertAll(line.Split(' '), long.Parse));
                    }
                }
            }
        }

        private long FindLowestLocationForSeedMap(long[] seedMap)
        {
            long? lowestLocation = null;

            foreach (long seed in seedMap)
            {
                long location = FindLocation(seed);

                if (lowestLocation == null)
                    lowestLocation = location;
                else if (location < lowestLocation)
                    lowestLocation = location;
            }

            return lowestLocation.Value;
        }

        private long FindLowestLocationForSeedMapPart2(long[] seedMap)
        {
            return 0;
            //long? lowestLocation = null;

            //long? lowBoundary = null; 
            //long? upperBoundary  = null;

            //for(int i = 0; i < seedMap.Length; i = i + 2)
            //{
            //    long _seedValue = seedMap[i];
            //    long _seedValueMax = seedMap[i] + seedMap[i + 1];

            //    long locationLow = FindLocation(_seedValue);
            //    long locationHigh = FindLocation(_seedValueMax);


            //    while (_seedValue <= _seedValueMax)
            //    {
            //        if ((lowestSeedProcessed == null) && (highestSeedProcessed == null)
            //            || (_seedValue < lowestSeedProcessed) || _seedValue > highestSeedProcessed)
            //        {
            //            long soil = FindNextValue(_seedValue, seedToSoil);
            //            long fert = FindNextValue(soil, soilToFert);
            //            long water = FindNextValue(fert, fertToWater);
            //            long light = FindNextValue(water, waterToLight);
            //            long temp = FindNextValue(light, lightToTemp);
            //            long humidity = FindNextValue(temp, tempToHumidity);
            //            long location = FindNextValue(humidity, humidityToLocation);

            //            if (lowestSeedProcessed == null)
            //                lowestSeedProcessed = seedMap[i];
            //            else if (_seedValue < lowestSeedProcessed)
            //                lowestSeedProcessed = _seedValue;

            //            if (highestSeedProcessed == null)
            //                highestSeedProcessed = seedMap[i];
            //            else if (_seedValue > highestSeedProcessed)
            //                highestSeedProcessed = _seedValue;

            //            if (lowestLocation == null)
            //                lowestLocation = location;
            //            else if (location < lowestLocation)
            //                lowestLocation = location;
            //        }

            //        _seedValue += 1;
            //    }
            //}

            //return lowestLocation.Value;
        }

        private long FindLocation(long seed)
        {
            long soil = FindNextValue(seed, seedToSoil);
            long fert = FindNextValue(soil, soilToFert);
            long water = FindNextValue(fert, fertToWater);
            long light = FindNextValue(water, waterToLight);
            long temp = FindNextValue(light, lightToTemp);
            long humidity = FindNextValue(temp, tempToHumidity);
            long location = FindNextValue(humidity, humidityToLocation);

            return location;
        }


        private long FindNextValue(long seed, List<long[]> map)
        {
            var range = map.FirstOrDefault(x => x[1] <= seed && seed <= (x[1] + x[2]));

            if (range == null)
                return seed;
            else
            {
                long difference = range[1] - range[0];

                return (seed - difference);
            }
        }
    }
}
