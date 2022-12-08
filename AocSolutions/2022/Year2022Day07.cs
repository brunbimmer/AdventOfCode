using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventFileIO;
using Common;


namespace AdventOfCode
{
    [AdventOfCode(Year = 2022, Day = 7)]
    public class Year2022Day07 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        public Year2022Day07()
        {
            //Get Attributes
            AdventOfCodeAttribute ca = (AdventOfCodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute));

            _Year = ca.Year;
            _Day = ca.Day;
            _OverrideFile = ca.OverrideTestFile;

            SW = new Stopwatch();
        }

        class Folder
        {
            public string Path { get; set; }
            public int Size { get; set; }
        }

        public void GetSolution(string path, bool trackTime = false)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Launching Puzzle for Dec. {_Day}, {_Year}");
            Console.WriteLine("===========================================");

            //Build BasePath and retrieve input. 
 

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] input = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();                       

            List<Folder> folders = ParseFileSystem(input);

            int total = 0;

            foreach(var d in folders.Where(o => o.Size < 100000))
            {
                total += d.Size;
            }
            
            SW.Stop();

            Console.WriteLine("Part 1: {0}, Execution Time: {1}", total, StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            int available = 70000000 - folders.First(o => o.Path == "").Size;

            int smallest = folders.Where(o => o.Size >= (30000000 - available)).Min(o => o.Size);
            
            SW.Stop();

            Console.WriteLine("Part 2: {0}, Execution Time: {1}", smallest, StopwatchUtil.getInstance().GetTimestamp(SW));
        }       

        private List<Folder> ParseFileSystem(string[] input)
        {
            //create the first root folder designated by empty string
            Folder currentFolder = new Folder()
            {
                Path = ""
            };

            List<Folder> folders = new List<Folder>() { currentFolder };


            foreach (string line in input)
            {
                if(line.StartsWith("$"))
                {
                    //cmd
                    if(line.Contains("cd"))
                    {
                        string operationArgument = line.Split(' ').Last();

                        switch (operationArgument)
                        {
                            case "/":
                                currentFolder = folders.First(o => o.Path == "");
                                break;
                            case "..":
                                var split = currentFolder.Path.Split('/');
                                string parentPath = string.Join('/', split.Take(split.Count() - 1));                            
                                currentFolder = folders.First(o => o.Path == parentPath);
                                break;
                            default:
                                string newPath = currentFolder.Path + "/" +  operationArgument;
                                currentFolder = new Folder() { Path = newPath };
                                folders.Add(currentFolder);
                                break;
                        }
                    }
                }
                else if(line.StartsWith("dir"))
                {
                    // ignore, nothing to do with the listing of a directory name.
                }
                else
                {
                    //file
                    int size = int.Parse(line.Split(' ').First());

                    //add to the total size of the directory
                    foreach (var f in folders.Where(o => currentFolder.Path.StartsWith(o.Path)))
                    {
                        f.Size += size;
                    }
                }
            }

            return folders;           
        }
    }
}
