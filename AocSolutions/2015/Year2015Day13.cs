using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2015, Day = 13)]
    public class Year2015Day13Problem : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

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

        public void GetSolution(string path, bool trackTime = false, IPuzzleOutput output = null)
        {
            output ??= new ConsolePuzzleOutput(trackTime);

             

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);         
            _SW.Restart();
          
            List<Guest> guests = ParseSeatingChart(instructions);

            List<string> persons = guests.Select(s => s.person).GroupBy(s => s).Select(s => s.Key).ToList();
            int totalChange = GetSittingHappiness(guests, persons);

            _SW.Stop();

            output.WritePart(1, totalChange, trackTime ? _SW.Elapsed : null, "Total Change of Happiness for optimal seating");
            
            _SW.Restart();
                       
            AddMe(guests, persons);
            persons.Add("Me");
            int newTotalChange = GetSittingHappiness(guests, persons);

            _SW.Stop();
            
            output.WritePart(2, newTotalChange, trackTime ? _SW.Elapsed : null, "Total Change of Happiness for optimal seating with me");
            


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
            var changes = new Dictionary<(string Person, string Neighbor), int>();
            foreach (Guest guest in guests)
                changes.TryAdd((guest.person, guest.nextTo), guest.gain - guest.lose);

            int[,] happiness = new int[persons.Count, persons.Count];
            for (int first = 0; first < persons.Count; first++)
            {
                for (int second = first + 1; second < persons.Count; second++)
                {
                    changes.TryGetValue((persons[first], persons[second]), out int forward);
                    changes.TryGetValue((persons[second], persons[first]), out int reverse);
                    happiness[first, second] = forward + reverse;
                    happiness[second, first] = forward + reverse;
                }
            }

            return GetMaximumHappiness(happiness);
        }

        private int GetMaximumHappiness(int[,] happiness)
        {
            int remainingGuests = happiness.GetLength(0) - 1;
            if (remainingGuests <= 0)
                return 0;

            // Guest zero anchors the circle. Each mask identifies the other guests already seated.
            int stateCount = 1 << remainingGuests;
            var best = new int[stateCount, remainingGuests];
            for (int mask = 0; mask < stateCount; mask++)
                for (int last = 0; last < remainingGuests; last++)
                    best[mask, last] = int.MinValue;

            for (int guest = 0; guest < remainingGuests; guest++)
                best[1 << guest, guest] = happiness[0, guest + 1];

            for (int mask = 1; mask < stateCount; mask++)
            {
                for (int last = 0; last < remainingGuests; last++)
                {
                    if (best[mask, last] == int.MinValue)
                        continue;

                    for (int next = 0; next < remainingGuests; next++)
                    {
                        int nextBit = 1 << next;
                        if ((mask & nextBit) != 0)
                            continue;

                        int nextMask = mask | nextBit;
                        int score = best[mask, last] + happiness[last + 1, next + 1];
                        best[nextMask, next] = Math.Max(best[nextMask, next], score);
                    }
                }
            }

            int maximum = int.MinValue;
            for (int last = 0; last < remainingGuests; last++)
                maximum = Math.Max(maximum, best[stateCount - 1, last] + happiness[last + 1, 0]);
            return maximum;
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
