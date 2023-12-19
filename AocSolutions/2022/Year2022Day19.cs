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
    [AdventOfCode(Year = 2022, Day = 19)]
    public class Year2022Day19 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2022Day19()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            _SW = new Stopwatch();
        }

        internal class Blueprint
        {
            public int oreCostOre;
            public int clayCostOre;
            public int obsidianCostOre;
            public int obsidianCostClay;
            public int geodeCostOre;
            public int geodeCostObsidian;

            public Blueprint(int _oreCostOre, 
                             int _clayCostOre, 
                             int _obsidianCostOre, 
                             int _obsidianCostClay, 
                             int _geodeCostOre,
                             int _geodeCostObsidian)
            {
                oreCostOre = _oreCostOre;
                clayCostOre = _clayCostOre;
                obsidianCostOre = _obsidianCostOre;
                obsidianCostClay = _obsidianCostClay;
                geodeCostOre = _geodeCostOre;
                geodeCostObsidian = _geodeCostObsidian;
            }
        }

        [Flags]
        enum AllowedRobots 
        {
          Ore       = 0x01,
          Clay      = 0x02,
          Obsidian  = 0x04,
          Geode     = 0x08,

          None      = 0x00,
          All       = 0x0F,
        }

        List<Blueprint> blueprints = new List<Blueprint>();

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            ParseInput(FileIOHelper.getInstance().ReadDataAsLines(file));

            _SW.Start();                       

            int index = 1;
            int totalP1Score = 0;
            foreach (var bp in blueprints)
            {
                int best = Simulate(bp, 1, 0, 0, 0, 0, 0, 0, 0, AllowedRobots.All, 24);
                totalP1Score += index * best;
                index++;
            }

            
            _SW.Stop();

            Console.WriteLine("Part 1: Total Score {0}, Execution Time: {1}", totalP1Score, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            int totalP2Score = 1;

            //only use the first three blueprints
            foreach (var bp in blueprints.Take(3))
            {
                int best = Simulate(bp, 1, 0, 0, 0, 0, 0, 0, 0, AllowedRobots.All, 32);
                totalP2Score *= best;             
            }
                
            _SW.Stop();

            Console.WriteLine("Part 2: Three Blueprint Multiplier {0}, Execution Time: {1}", totalP2Score, StopwatchUtil.getInstance().GetTimestamp(_SW));


        }     
        

        //Parse the line as is and map them to a resource group defined above.
        private void ParseInput(string[] lines)
        {
            blueprints = lines.Select(line => Regex.Match(
                    line,
                    @"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian."))
                .Select(match => match.Groups.Values.Skip(1).Select(group => int.Parse(group.Value)).ToList())
                .Select(values => new Blueprint(values[1], values[2], values[3],values[4],values[5],values[6])).ToList();         
        }

        int Simulate(
              Blueprint bp, 
              int oreRobotCount, 
              int clayRobotCount, 
              int obsidianRobotCount, 
              int geodeRobotCount, 
              int oreCount, 
              int clayCount, 
              int obsidianCount, 
              int geodeCount, 
              AllowedRobots allowedRobots, 
              int timeRemaining)
            {
              int maxOreCost = Math.Max(bp.clayCostOre, Math.Max(bp.geodeCostOre, bp.obsidianCostOre));

              while (true)
              {
                // If we're only 1 from the end, there's no point in buying a robot because it won't finish in time to do anything.
                if (timeRemaining > 1)
                {
                  int bestResult = 0;

                  // We can buy a geode robot if we have its cost and we didn't skip buying it in the past.
                  if ((allowedRobots & AllowedRobots.Geode) != 0 
                    && obsidianCount >= bp.geodeCostObsidian 
                    && oreCount >= bp.geodeCostOre)
                  {
                    allowedRobots &= ~AllowedRobots.Geode;
                    bestResult = Simulate(
                      bp, 
                      oreRobotCount, 
                      clayRobotCount, 
                      obsidianRobotCount, 
                      geodeRobotCount + 1, 
                      oreCount - bp.geodeCostOre + oreRobotCount, 
                      clayCount + clayRobotCount, 
                      obsidianCount - bp.geodeCostObsidian + obsidianRobotCount, 
                      geodeCount + geodeRobotCount, 
                      AllowedRobots.All, 
                      timeRemaining - 1);
                  }
                  else // We can only build other robots if we couldn't build a geode bot
                  {
                    // With obsidian robot, we have the additional condition that if we're making enough obsidian every minute
                    //  to fulfill the geode robot's cost we never need to build one again.
                    if (obsidianRobotCount < bp.geodeCostObsidian 
                      && (allowedRobots & AllowedRobots.Obsidian) != 0 
                      && clayCount >= bp.obsidianCostClay 
                      && oreCount >= bp.obsidianCostOre)
                    {
                      allowedRobots &= ~AllowedRobots.Obsidian;
                      bestResult = Math.Max(bestResult, Simulate(
                        bp, 
                        oreRobotCount, 
                        clayRobotCount, 
                        obsidianRobotCount + 1, 
                        geodeRobotCount, 
                        oreCount - bp.obsidianCostOre + oreRobotCount, 
                        clayCount - bp.obsidianCostClay + clayRobotCount, 
                        obsidianCount + obsidianRobotCount, 
                        geodeCount + geodeRobotCount, 
                        AllowedRobots.All, 
                        timeRemaining - 1));
                    }

                    // Clay robot basically same as obsidian: if we aren't already producing enough and we can afford it and we didn't
                    //  skip it
                    if (clayRobotCount < bp.obsidianCostClay 
                      && (allowedRobots & AllowedRobots.Clay) != 0 
                      && oreCount >= bp.clayCostOre)
                    {
                      allowedRobots  &= ~AllowedRobots.Clay;
                      bestResult = Math.Max(bestResult, Simulate(
                        bp, 
                        oreRobotCount, 
                        clayRobotCount + 1, 
                        obsidianRobotCount, 
                        geodeRobotCount, 
                        oreCount - bp.clayCostOre + oreRobotCount, 
                        clayCount + clayRobotCount, 
                        obsidianCount + obsidianRobotCount, 
                        geodeCount + geodeRobotCount, 
                        AllowedRobots.All, 
                        timeRemaining - 1));
                    }

                    // Ore robot tests against the maximum ore cost of all robots (except itself because who cares?) to see if we should bother 
                    //  building one
                    if (oreRobotCount < maxOreCost 
                      && (allowedRobots & AllowedRobots.Ore) != 0 
                      && oreCount >= bp.oreCostOre)
                    {
                      allowedRobots  &= ~AllowedRobots.Ore;
                      bestResult = Math.Max(bestResult, Simulate(
                        bp, 
                        oreRobotCount + 1, 
                        clayRobotCount, 
                        obsidianRobotCount, 
                        geodeRobotCount, 
                        oreCount - bp.oreCostOre + oreRobotCount, 
                        clayCount + clayRobotCount, 
                        obsidianCount + obsidianRobotCount, 
                        geodeCount + geodeRobotCount, 
                        AllowedRobots.All, 
                        timeRemaining - 1));
                    }
                  }

                  // Test to see if we recursed into any other robots: if we did, their flags will be cleared and we can return.
                  if (allowedRobots != AllowedRobots.All)
                  {
                    // If we recursed into any robots (except geode), we also need to simulate "what if we chose to build nothing 
                    //  at all this round" in which case we disallow future building of any robots we had the option to build
                    //  since there's never a scenario in which we should wait to build a robot we can afford now, but do nothing 
                    //  else.
                    if ((allowedRobots & AllowedRobots.Geode) != 0)
                    {
                      bestResult = Math.Max(bestResult, Simulate(
                        bp, 
                        oreRobotCount, 
                        clayRobotCount, 
                        obsidianRobotCount, 
                        geodeRobotCount, 
                        oreCount + oreRobotCount, 
                        clayCount + clayRobotCount, 
                        obsidianCount + obsidianRobotCount, 
                        geodeCount + geodeRobotCount, 
                        allowedRobots, 
                        timeRemaining - 1));
                    }

                    // Regardless of whether we simulate the "do nothing" case or not, return our best result from the recursion.
                    return bestResult;
                  }
                }

                // We had no choice on that round so just increment our resource counts and go again (unless we're
                //  out of time)
                oreCount += oreRobotCount;
                clayCount += clayRobotCount;
                obsidianCount += obsidianRobotCount;
                geodeCount += geodeRobotCount;
                timeRemaining--;
                if (timeRemaining == 0)
                {
                  return geodeCount;
                }
              }
            }
    }
}
