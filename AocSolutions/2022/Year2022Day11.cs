using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 11)]
    public class Year2022Day11 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public class MonkeyBusiness
        {
            public long number;
            public Queue<long> items;
                        
            public int monkeyDestTrue;
            public int monkeyDestFalse;

            public int divisibleBy;

            public long Inspections;

            public long Operation(long _level, bool worry, long worryDivisor) 
            {
                long _newLevel = 0;
                switch (number)
                {
                    case 0: _newLevel =  (long) (_level * 7) ; break;
                    case 1: _newLevel =  (long) (_level * 11) ; break;
                    case 2: _newLevel =  (long) (_level + 8) ; break;
                    case 3: _newLevel =  (long) (_level + 7) ; break;
                    case 4: _newLevel =  (long) (_level + 5) ; break;
                    case 5: _newLevel =  (long) (_level + 4) ; break;
                    case 6: _newLevel =  (long) (_level * _level); break;
                    case 7: _newLevel =  (long) (_level + 3) ; break;
                    default: _newLevel =  0; break;
                }

                if (worry)
                    return (long)Math.Floor((double)_newLevel / worryDivisor);
                else
                    return (long)_newLevel % worryDivisor;
                
                                  
            }
           
            public bool Test (long level) { return (level % divisibleBy == 0);}
        }


        public Stopwatch SW { get; set; }

        public Year2022Day11()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        private List<MonkeyBusiness> GetInitialMonkeyState()
        {
            List<MonkeyBusiness> monkeys = new List<MonkeyBusiness>
            {
                new MonkeyBusiness {number = 0, Inspections = 0, monkeyDestTrue = 6, monkeyDestFalse = 7, divisibleBy = 19, items = new Queue<long>(new Int64[] {85, 77, 77}) } ,
                new MonkeyBusiness {number = 1, Inspections = 0, monkeyDestTrue = 3, monkeyDestFalse = 5, divisibleBy = 3, items = new Queue<long>(new Int64[] {80, 99}) } ,
                new MonkeyBusiness {number = 2, Inspections = 0, monkeyDestTrue = 0, monkeyDestFalse = 6, divisibleBy = 13, items = new Queue<long>(new Int64[] {74, 60, 74, 63, 86, 92, 80}) } ,
                new MonkeyBusiness {number = 3, Inspections = 0, monkeyDestTrue = 2, monkeyDestFalse = 4, divisibleBy = 7, items = new Queue<long>(new Int64[] {71, 58, 93, 65, 80, 68, 54, 71}) } ,
                new MonkeyBusiness {number = 4, Inspections = 0, monkeyDestTrue = 2, monkeyDestFalse = 0, divisibleBy = 5, items = new Queue<long>(new Int64[] {97, 56, 79, 65, 58}) } ,
                new MonkeyBusiness {number = 5, Inspections = 0, monkeyDestTrue = 4, monkeyDestFalse = 3, divisibleBy = 11, items = new Queue<long>(new Int64[] {77}) } ,
                new MonkeyBusiness {number = 6, Inspections = 0, monkeyDestTrue = 7, monkeyDestFalse = 1, divisibleBy = 17, items = new Queue<long>(new Int64[] {99, 90, 84, 50}) } ,
                new MonkeyBusiness {number = 7, Inspections = 0, monkeyDestTrue = 5, monkeyDestFalse = 1, divisibleBy = 2, items = new Queue<long>(new Int64[] {50, 66, 61, 92, 64, 78}) }
            };

            return monkeys;
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 
            // list out the items with options for no armour and rings but not no weapon
            List<MonkeyBusiness> monkeys = GetInitialMonkeyState();          
            SW.Start();                     
            long InspectionMetric = RunMonkeyBusiness(20, monkeys, true, 3);

            
            SW.Stop();

            Console.WriteLine("Part 1: Inspection Metric: {0}, Execution Time: {1}", InspectionMetric, StopwatchUtil.getInstance().GetTimestamp(SW));
            
            monkeys = GetInitialMonkeyState();
            SW.Restart();            

            long worryDivisor = monkeys.Aggregate(1, (current, monkey) => current * monkey.divisibleBy);

            InspectionMetric = RunMonkeyBusiness(10000, monkeys, false, worryDivisor);
            
            SW.Stop();

            Console.WriteLine("Part 2: Inspection Metric: {0}, Execution Time: {1}", InspectionMetric, StopwatchUtil.getInstance().GetTimestamp(SW));


        }     
        
        private long RunMonkeyBusiness(int rounds, List<MonkeyBusiness> monkeys, bool worry, long reduceWorry)
        {
            for(int i = 0; i < rounds ; i++)
            {
                foreach(MonkeyBusiness monkey in monkeys)
                {
                    while(monkey.items.Count > 0)
                    {
                        long itemValue = monkey.items.Dequeue();
                        long newItemValue = monkey.Operation(itemValue, worry, reduceWorry);

                        if (monkey.Test(newItemValue))
                            monkeys.Where(x => x.number == monkey.monkeyDestTrue).First().items.Enqueue(newItemValue);
                        else
                            monkeys.Where(x => x.number == monkey.monkeyDestFalse).First().items.Enqueue(newItemValue);

                        monkey.Inspections += 1;
                    }                                        
                }
            }

            List<MonkeyBusiness> ordered = monkeys.OrderByDescending(x => x.Inspections).ToList();

            return ordered[0].Inspections * ordered[1].Inspections;

        }

    }
}
