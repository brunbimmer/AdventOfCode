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
    

    [AdventOfCode(Year = 2015, Day = 21)]
    public class Year2015Day21 : IAdventOfCode
    {
        public class Competitor
        {
            public string Name;
            public int HitPoints;
            public int Damage;
            public int Armour;
        }

        // all items from the shop are represented as a single class
        // this is so I can .Sum() out of laziness
        public class Item
        {
            public string Name;
            public int Cost;
            public int Damage;
            public int Armour;
            public string Group;
        }

        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2015Day21()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            SW.Restart();
            //Build BasePath and retrieve input. 
 
            var boss = new Competitor { HitPoints = 109, Damage = 8, Armour = 2, Name = "Boss" };
            var player = new Competitor { HitPoints = 100, Damage = 0, Armour = 0, Name = "Player" };

            // list out the items with options for no armour and rings but not no weapon
            List<Item> items = new List<Item>
            {
                new Item { Name = "Dagger", Cost = 8, Damage = 4, Armour = 0, Group = "Weapons" },
                new Item { Name = "Shortsword", Cost = 10, Damage = 5, Armour = 0, Group = "Weapons" },
                new Item { Name = "Warhammer", Cost = 25, Damage = 6, Armour = 0, Group = "Weapons" },
                new Item { Name = "Longsword", Cost = 40, Damage = 7, Armour = 0, Group = "Weapons" },
                new Item { Name = "Greataxe", Cost = 74, Damage = 8, Armour = 0, Group = "Weapons" },
                new Item { Name = "Leather", Cost = 13, Damage = 0, Armour = 1, Group = "Armour" },
                new Item { Name = "Chainmail", Cost = 31, Damage = 0, Armour = 2, Group = "Armour" },
                new Item { Name = "Splintmail", Cost = 53, Damage = 0, Armour = 3, Group = "Armour" },
                new Item { Name = "Bandedmail", Cost = 75, Damage = 0, Armour = 4, Group = "Armour" },
                new Item { Name = "Platemail", Cost = 102, Damage = 0, Armour = 5, Group = "Armour" },
                new Item { Name = "No Armour", Cost = 0, Damage = 0, Armour = 0, Group = "Armour" },
                new Item { Name = "Damage +1", Cost = 25, Damage = 1, Armour = 0, Group = "Rings" },
                new Item { Name = "Damage +2", Cost = 50, Damage = 2, Armour = 0, Group = "Rings" },
                new Item { Name = "Damage +3", Cost = 100, Damage = 3, Armour = 0, Group = "Rings" },
                new Item { Name = "Defence +1", Cost = 20, Damage = 0, Armour = 1, Group = "Rings" },
                new Item { Name = "Defence +2", Cost = 40, Damage = 0, Armour = 2, Group = "Rings" },
                new Item { Name = "Defence +3", Cost = 80, Damage = 0, Armour = 3, Group = "Rings" },
                // either or both rings can be 0
                new Item { Name = "No Ring A", Cost = 0, Damage = 0, Armour = 0, Group = "Rings" },
                new Item { Name = "No Ring B", Cost = 0, Damage = 0, Armour = 0, Group = "Rings" }
            };

            // get all possible combinations
            var combinations =
                items.Where(i => i.Group == "Weapons").SelectMany(w =>
                items.Where(i => i.Group == "Armour").SelectMany(a =>
                items.Where(i => i.Group == "Rings").SelectMany(r1 =>
                items.Where(i => i.Group == "Rings" && i != r1).Select(r2 =>
                new List<Item> { a, w, r1, r2 }))));

            // minimum spend for player to win
            var min = combinations.Min(i => Try("Player", player, boss, i) ?? Int32.MaxValue);
            Console.WriteLine(min);

            // maximum spend for boss to win
            var max = combinations.Max(i => Try("Boss", player, boss, i) ?? Int32.MinValue);
            Console.WriteLine(max);

            SW.Stop();
            Console.WriteLine("Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

        
        }   
        
        // simulate a fight until there's a winner
        // pure brute force
        private Competitor Fight(Competitor first, Competitor second)
        {
            var hpFirst = first.HitPoints;
            var hpSecond = second.HitPoints;

            var damageFirst = Math.Max(first.Damage - second.Armour, 1);
            var damageSecond = Math.Max(second.Damage - first.Armour, 1);

            while (true)
            {
                hpSecond -= damageFirst;

                if (hpSecond <= 0) return first;

                hpFirst -= damageSecond;
            
                if (hpFirst <= 0) return second;
            }            
        }

        // see who wins with this set of items
        // if the winner is the desired winner how much did it cost?
        private int? Try(string winnerName, Competitor player, Competitor boss, List<Item> items)
        {
            player.Damage = items.Sum(i => i.Damage);
            player.Armour = items.Sum(i => i.Armour);

            if (Fight(player, boss).Name == winnerName)
            {
                return items.Sum(i => i.Cost);
            }
            return null;
        }
    }
}
