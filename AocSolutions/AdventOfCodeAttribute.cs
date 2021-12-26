using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AdventOfCodeAttribute: Attribute
    {
        public int Year { get; set; }

        public int Day {get; set; }

        public int Index
        {
            get
            {
                int index = (Year - 2015) * 25 + Day;
                return index;
            }
        }

        public string OverrideTestFile {get; set; }
    }

}
