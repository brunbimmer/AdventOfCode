using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using CommonAlgorithms;
using Microsoft.VisualBasic;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 13)]
    public class Year2015Day13Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        
        private Dictionary<Tuple<string, string>, int> locations = new Dictionary<Tuple<string, string>, int>();
        private List<string> allTowns = new List<string>();

        struct Guest
        {
            public string person;
            public string nextTo;
            public int gain;
            public int lose;
        }

        public Year2015Day13Problem()
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

             

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);         
            _SW.Start();
          
            List<Guest> guests = ParseSeatingChart(instructions);

            List<string> persons = guests.Select(s => s.person).GroupBy(s => s).Select(s => s.Key).ToList();
            int totalChange = GetSittingHappiness(guests, persons);

            _SW.Stop();

            Console.WriteLine("  Part 1: Total Change of Happiness for optimal seating: {0}, Execution Time: {1}", totalChange, StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();
                       
            AddMe(guests, persons);
            persons.Add("Me");
            int newTotalChange = GetSittingHappiness(guests, persons);

            _SW.Stop();
            
            Console.WriteLine("  Part 2: Total Change of Happiness for optimal seating with me: {0}, Execution Time: {1}", newTotalChange, StopwatchUtil.getInstance().GetTimestamp(_SW));
            


        }

       
        List<Guest> ParseSeatingChart(string[] instructions)
        {
            List<Guest> guests = new List<Guest>();

            foreach (string line in instructions)
            {
                var data = line.Split(' ');
                Guest guest = new();

                guest.person = data[0];
                guest.nextTo = data[10].Replace(".", "");
                
                if (data[2] == "gain")
                    guest.gain = Convert.ToInt32(data[3]);
                else
                    guest.lose = Convert.ToInt32(data[3]);
                
                guests.Add(guest);
            }

            return guests;
        }

        private void AddMe(List<Guest> guests, List<String> persons)
        {
            foreach (string person in persons)
            {
                Guest guest = new Guest();
                guest.person = person;
                guest.nextTo = "Me";
                guests.Add(guest);
                guest = new Guest();
                guest.person = "Me";
                guest.nextTo = person;
                guests.Add(guest);
            }
        }
        
        
        private int GetSittingHappiness(List<Guest> guests, List<string> persons)
        {
            var sittingsPerms = BuildPermutations(persons);
            var sittingsValues = new Dictionary<string, int>();

            foreach (List<string> perm in sittingsPerms)
            {
                int happiness = 0;
                Guest sitting;
                perm.Add(perm[0]);
                for (int i = 0; i < perm.Count - 1; i++)
                {
                    if (guests.Any(s => s.person == perm[i] && s.nextTo == perm[i + 1]))
                    {
                        sitting = guests.First(s => s.person == perm[i] && s.nextTo == perm[i + 1]);
                        happiness += sitting.gain - sitting.lose;
                    }
                    if (guests.Any(s => s.person == perm[i + 1] && s.nextTo == perm[i]))
                    {
                        sitting = guests.First(s => s.person == perm[i + 1] && s.nextTo == perm[i]);
                        happiness += sitting.gain - sitting.lose;
                    }
                }
                if (happiness > 0)
                    sittingsValues.Add(String.Join("->", perm), happiness);
            }
            var happiestSitting = sittingsValues.OrderByDescending(v => v.Value).First();
         
            return happiestSitting.Value;
        }

        public List<List<string>> BuildPermutations(List<string> items)
        {
            if (items.Count > 1)
            {
                return items.SelectMany(item => BuildPermutations(items.Where(i => !i.Equals(item)).ToList()),
                                       (item, permutation) => new [] { item }.Concat(permutation).ToList()).ToList();
            }

            return new List<List<string>> { items };
        }
    }
}
