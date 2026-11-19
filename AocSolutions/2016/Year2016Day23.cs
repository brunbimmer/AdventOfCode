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
using ConsoleTables;
using Microsoft.Extensions.Primitives;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2016, Day = 23)]
    public class Year2016Day23 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        private Dictionary<string, int> registers = new Dictionary<string, int>()
        {
            {"a", 7 },
            {"b", 0 },
            {"c", 0 },
            {"d", 0 }
        };

        public Year2016Day23()
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

            string[] instructions = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();                       

            registers = BunnyAssemblyParser.ParseInstructions(registers, instructions.ToList(), output.WriteDiagnostic);

            
            _SW.Stop();

            output.WritePart(1, registers["a"], trackTime ? _SW.Elapsed : null, "Value of Register A");

            //Reinitialize with Part 2 instructions.
            registers["a"] = 12;
            registers["b"] = 0;
            registers["c"] = 0;
            registers["d"] = 0;

            _SW.Restart();

           registers = BunnyAssemblyParser.ParseInstructions(registers, instructions.ToList(), output.WriteDiagnostic);
            
            _SW.Stop();

            output.WritePart(2, registers["a"], trackTime ? _SW.Elapsed : null, "Value of Register A");


        }   

      
    }
}
