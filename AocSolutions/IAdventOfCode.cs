using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Utilities;


namespace AdventOfCode
{
    public interface IAdventOfCode
    {

        public const CompassDirection N = CompassDirection.N;
        public const CompassDirection S = CompassDirection.S;
        public const CompassDirection E = CompassDirection.E;
        public const CompassDirection W = CompassDirection.W;
        public const CompassDirection NE = CompassDirection.NE;
        public const CompassDirection NW = CompassDirection.NW;
        public const CompassDirection SE = CompassDirection.SE;
        public const CompassDirection SW = CompassDirection.SW;
        

        /// <summary>
        /// General Solution that will perform the daily coding problem from Advent of Code
        /// </summary>
        /// <param name="path">Path to test file</param>
        /// <returns>Total time to complete execution of solution</returns>
        public void GetSolution(string path, bool trackTime = false);       
    }
}
