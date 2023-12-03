using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 4)]
    public class Year2016Day04 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2016Day04()
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

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] roomList = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();


            int sum = ParseRoomList(roomList);


            SW.Stop();

            Console.WriteLine("Valid room Sector ID Sum: {0}, Execution Time: {1}", sum, StopwatchUtil.getInstance().GetTimestamp(SW));

       }

        int ParseRoomList(string[] roomList)
        {
            string roomName, sectorId, checksum = "";
            int sum = 0;
            foreach (string room in roomList)
            {
                (roomName, sectorId, checksum) = GetRoomDetails(room);

                if (CheckValidRoom(roomName.Replace("-", ""), checksum))
                {
                    sum += int.Parse(sectorId);
                    var decipheredRoomName = DecypherRoomName(roomName.Replace("-", " "), int.Parse(sectorId));

                    if (decipheredRoomName.Contains("northpole"))
                        Console.WriteLine($"The room we are looking for is \"{decipheredRoomName}\" it is located in Sector ID {sectorId}.");
                }
                    

            }

            return sum;
        }

        // Get the name, the sector id and the checksum of the room.
        private (string, string, string) GetRoomDetails(string room)
        {
            string[] details = new string[3];
            var pattern = @"^(.+)-([0-9]+)\[([a-z]+)\]$";
            MatchCollection matches = Regex.Matches(room, pattern);

            return (matches[0].Groups[1].Value, matches[0].Groups[2].Value, matches[0].Groups[3].Value);
        }

        private bool CheckValidRoom(string roomName, string checksum)
        {
            var rulesChecksum = string.Join("", roomName.GroupBy(c => c).OrderByDescending(c => c.Count()).ThenBy(c => c.Key).Take(5).Select(c => c.Key).ToList());
            return rulesChecksum == checksum;
        }

        private string DecypherRoomName(string name, int shift)
        {
            var alphabet = "abcdefghijklmnopqrstuvwxyz";
            var builder = new StringBuilder(name);
            for (int i = 0; i < name.Length; i++)
            {
                if (builder[i] == ' ')
                    continue;
                builder[i] = alphabet[(alphabet.IndexOf(builder[i]) + shift) % 26];
            }

            return builder.ToString();
        }

    }
}
