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
    [AdventOfCode(Year = 2021, Day = 20)]
    public class Year2021Day20 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        private   string _algorithm;
        private   Dictionary<Coordinate2D, char> _image;
        private   bool _invert = false;


        public Year2021Day20()
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
            _image = new();

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] dataInput = FileIOHelper.getInstance().ReadDataAsLines(file);

            if (trackTime) SW.Start();                       

            InitializeDataSets(dataInput);                        
            int activeLights = EnhanceImage(2);

            
            if (trackTime) SW.Stop();

            Console.WriteLine("Part 1: Number of Active Lights after 2 passes: {0}", activeLights);
            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            if (trackTime) SW.Reset();
            if (trackTime) SW.Start();

            activeLights = EnhanceImage(50);
            
            if (trackTime) SW.Stop();

            Console.WriteLine("Part 2: Number of Active Lights after 50 passes: {0}", activeLights);

            if (trackTime) Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            Console.WriteLine("");
            Console.WriteLine("===========================================");
            Console.WriteLine("");
            Console.WriteLine("Please hit any key to continue");
            Console.ReadLine();
        }    
        
        private   void InitializeDataSets(string[] dataInput)
        {

            _algorithm = dataInput[0];
            if (_algorithm[0] == '#') _invert = true;

            var imageLines = dataInput.Skip(2).Select(x => x.ToArray()).ToArray();

            for (int y = 0; y < imageLines.Length; y++)
                for (int x = 0; x < imageLines[y].Length; x++)
                    _image.Add(new Coordinate2D(x, y), (imageLines[y][x] == '#' ? '1': '0'));

        }

        public   int EnhanceImage(int numPasses)
        {
            //copy over existing data to another image that will store the enhancedImage so we don't destory original
            Dictionary<Coordinate2D, char> enhancedImage = new Dictionary<Coordinate2D, char>(_image);

            foreach (int i in Enumerable.Range(1, numPasses))
            {
                enhancedImage = DoEnhance(enhancedImage, i);
            }
            return enhancedImage.Values.Count(x => x == '1');
        }

        private   Dictionary<Coordinate2D, char> DoEnhance(Dictionary<Coordinate2D, char> image, int step)
        {
            Dictionary<Coordinate2D, char> newImage = new Dictionary<Coordinate2D, char>();

            int minX = image.Min(a => a.Key.X);
            int minY = image.Min(a => a.Key.Y);
            int maxX = image.Max(a => a.Key.X);
            int maxY = image.Max(a => a.Key.Y);

            for (int x = minX - 2; x < maxX + 2; x++)
            {
                for (int y = minY - 2; y < maxY + 2; y++)
                {
                    StringBuilder binaryString = new StringBuilder();                    
                    List<Coordinate2D> neighbours = new Coordinate2D(x, y).Neighbours(true, true);

                    foreach (Coordinate2D neighbour in neighbours)
                    {
                        var tmp = (step % 2) == 1 || !_invert ? image.GetValueOrDefault(neighbour, '0') : image.GetValueOrDefault(neighbour, '1');
                        binaryString.Append(tmp);
                    }

                    int algorithIndex = Convert.ToInt32(binaryString.ToString(), 2);
                    newImage.Add(new Coordinate2D(x, y), _algorithm[algorithIndex] == '#' ? '1' : '0');
                }
            }

            return newImage;
        }       
    }
}
