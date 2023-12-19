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
    [AdventOfCode(Year = 2021, Day = 16)]
    public class Year2021Day16 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        class Packet
        {
            public int PacketVersion;
            public int PacketType;
            public long LiteralValue;
            public List<Packet> SubPackets = new();
            public long VersionSum => PacketVersion + SubPackets.Sum(x => x.VersionSum);
            public long Value => (PacketType) switch
            {
                0 => SubPackets.Sum(a => a.Value),
                1 => SubPackets.Aggregate(1L, (accumulatedTotal, packet) => accumulatedTotal * packet.Value),
                2 => SubPackets.Min(a => a.Value),
                3 => SubPackets.Max(a => a.Value),
                4 => LiteralValue,
                5 => SubPackets[0].Value > SubPackets[1].Value ? 1 : 0,
                6 => SubPackets[0].Value < SubPackets[1].Value ? 1 : 0,
                7 => SubPackets[0].Value == SubPackets[1].Value ? 1 : 0,
                _ => throw new NotImplementedException(),
            };
        }


        public Year2021Day16()
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

            string transmission = FileIOHelper.getInstance().ReadDataAsString(file).Trim();

            _SW.Start();                       

            var OuterPacket = ParseStream(transmission);  
            
            _SW.Stop();

            Console.WriteLine("Part 1: Sum of all the Version Numbers: {0}", OuterPacket.VersionSum);
            Console.WriteLine("Part 1: Final Value: {0}", OuterPacket.Value);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }      
        
        private Packet ParseStream(string stream)
        {
            string binaryStream = "";

            try
            {

                binaryStream = String.Join(String.Empty,
                                                stream.Select(
                                                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                                           ));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            int increment = 0;
            return ParsePacket(binaryStream, 0, ref increment);
        }

        private Packet ParsePacket(string bStream, int startPoint, ref int incrementBy)
        {
            int posLength = 0;

            Packet packet = new Packet();
            packet.PacketVersion = Convert.ToInt32(bStream.Substring(startPoint, 3), 2);
            posLength += 3;
            packet.PacketType = Convert.ToInt32(bStream.Substring(startPoint + posLength, 3), 2);
            posLength += 3;

            if (packet.PacketType == 4)
            {
                //Add literal values to sequence and convert
                StringBuilder literalValue = new();
                while (bStream[startPoint + posLength] == '1')
                {
                    posLength += 1;
                    literalValue.Append(bStream.AsSpan(startPoint + posLength, 4));
                    posLength += 4;
                }

                posLength += 1;
                literalValue.Append(bStream.AsSpan(startPoint + posLength, 4));
                posLength += 4;
                packet.LiteralValue = Convert.ToInt64(literalValue.ToString(), 2);
            }
            else
            {
                char lengthTypeID = bStream[startPoint + posLength];
                posLength += 1;

                int subPosLength = 0;               //internal position tracker when recursing into sub routines
                if (lengthTypeID == '0')
                {
                    //process sub-packets based on length
                    int bitLengthSubPackets = Convert.ToInt32(bStream.Substring(startPoint + posLength, 15), 2);
                    posLength += 15;
                    while (subPosLength < bitLengthSubPackets)
                    {
                        packet.SubPackets.Add(ParsePacket(bStream, startPoint + posLength + subPosLength, ref subPosLength));
                    }
                }
                else
                {
                    //process sub-packets based on number of subpackets
                    int numberSubPackets = Convert.ToInt32(bStream.Substring(startPoint + posLength, 11), 2);
                    posLength += 11;
                    
                    for (int i = 0; i < numberSubPackets; i++)
                    {
                        packet.SubPackets.Add(ParsePacket(bStream, startPoint + posLength + subPosLength, ref subPosLength));
                    }
                }
                posLength += subPosLength;
            }
            incrementBy += posLength;

            return packet; //packet length if we are iterating through packets
        }
    }
}
