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
    [AdventOfCode(Year = 2016, Day = 12)]
    public class Year2016Day12 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        private Dictionary<string, int> registers = new Dictionary<string, int>()
        {
            {"a", 0 },
            {"b", 0 },
            {"c", 0 },
            {"d", 0 }
        };


        public Year2016Day12()
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

            List<string> instructions = FileIOHelper.getInstance().ReadDataAsLines(file).ToList<string>();

            _SW.Start();                       

            registers = BunnyAssemblyParser.ParseInstructions(registers, instructions, output.WriteDiagnostic);
            
            _SW.Stop();
            
            output.WritePart(1, registers["a"], trackTime ? _SW.Elapsed : null, "(Value on Register A)");

            _SW.Restart();

            //Reinitialize with Part 2 instructions.
            registers["a"] = 0;
            registers["b"] = 0;
            registers["c"] = 1;
            registers["d"] = 0;

            registers = BunnyAssemblyParser.ParseInstructions(registers, instructions, output.WriteDiagnostic);
            
            _SW.Stop();

             output.WritePart(2, registers["a"], trackTime ? _SW.Elapsed : null, "(Value on Register A after initializing Register C to 1)");


        }          
    }
}
