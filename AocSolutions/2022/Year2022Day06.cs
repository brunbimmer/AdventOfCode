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
    [AdventOfCode(Year = 2022, Day = 6)]
    public class Year2022Day06 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2022Day06()
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

            String input = FileIOHelper.getInstance().ReadDataAsString(file);

            _SW.Restart();

            int startOfPacket = FindStart(input, 4);            

            _SW.Stop();            
            output.WritePart(1, startOfPacket, trackTime ? _SW.Elapsed : null, "Start of Packet Location");
            
            _SW.Restart();
            
            int startOfPacket2 = ScanAsSpan(input, 4);                                    
            _SW.Stop();
            output.WriteDiagnostic(string.Format("Part 1: Start of Packet Location {0}, Execution Time: {1} - Scan", startOfPacket2, StopwatchUtil.getInstance().GetTimestamp(_SW)));

            _SW.Restart();

            int startOfMessage = FindStart(input, 14);
            _SW.Stop();

            output.WritePart(2, startOfMessage, trackTime ? _SW.Elapsed : null, "Start of Message Location");
            _SW.Restart();

            int startOfMessage2 = ScanAsSpan(input, 14);                        
            output.WriteDiagnostic(string.Format("Part 2: Start of Message Location {0}, Execution Time: {1} - Scan", startOfMessage2, StopwatchUtil.getInstance().GetTimestamp(_SW)));
            
            _SW.Stop();
        }       

        private int FindStart(string input, int length)
        {
            int markerPosition = 0;

            for (int i = 0; i < input.Length; i++)
            {
                string sequence = input.Substring(i, length);

                if (sequence.Distinct().ToArray().Length == length)
                {
                    markerPosition = i + length;
                    break;
                }
                    
                    
            }
            return markerPosition;
        }


        private int ScanAsSpan(string stream, int n)
        {
            var content = stream.ToCharArray().AsSpan();
            int i;
            for(i=n; i< content.Length; i++)
            {
                if(content.Slice(i-n,n).ToArray().Distinct().Count() == n)
                    break;
            }
            return i;
        }
    }
}
