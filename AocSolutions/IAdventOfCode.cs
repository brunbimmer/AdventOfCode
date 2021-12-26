using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AdventOfCode
{
    public interface IAdventOfCode
    {
        public Stopwatch SW {get; set;}

        /// <summary>
        /// General Solution that will perform the daily coding problem from Advent of Code
        /// </summary>
        /// <param name="path">Path to test file</param>
        /// <returns>Total time to complete execution of solution</returns>
        public void GetSolution(string path, bool trackTime = false);       
    }
}
