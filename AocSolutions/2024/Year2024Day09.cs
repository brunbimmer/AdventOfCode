using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventFileIO;
using Common;
using LINQPad.Extensibility.DataContext;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2024, Day = 9)]
    public class Year2024Day09: IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2024Day09()
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

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);
            
            string diskMap = lines[0].Trim();

            _SW.Start();

            long part1 = SolvePart1(diskMap);

            _SW.Stop();

            Console.WriteLine($"  Part 1 (Individual block compaction): {part1}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));
            
            _SW.Restart();

            long part2 = SolvePart2(diskMap);

            _SW.Stop();

            Console.WriteLine($"  Part 2 (Whole file defragmentation): {part2}");
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

        }

        private long SolvePart1(string diskMap)
        {
            // Convert disk map to list of block IDs: -1 for free space, 0+ for file IDs
            var blocks = new List<int>();
            int fileId = 0;
            
            for (int i = 0; i < diskMap.Length; i++)
            {
                int count = diskMap[i] - '0';
                if (i % 2 == 0)
                {
                    // File block
                    for (int j = 0; j < count; j++)
                        blocks.Add(fileId);
                    fileId++;
                }
                else
                {
                    // Free space
                    for (int j = 0; j < count; j++)
                        blocks.Add(-1);
                }
            }

            // Compact by moving file blocks from the end to fill free spaces
            int leftPtr = 0;
            int rightPtr = blocks.Count - 1;

            while (leftPtr < rightPtr)
            {
                // Find the next free space on the left
                while (leftPtr < rightPtr && blocks[leftPtr] != -1)
                    leftPtr++;

                // Find the next file block on the right
                while (leftPtr < rightPtr && blocks[rightPtr] == -1)
                    rightPtr--;

                if (leftPtr < rightPtr)
                {
                    // Swap the file block to the free space
                    blocks[leftPtr] = blocks[rightPtr];
                    blocks[rightPtr] = -1;
                    leftPtr++;
                    rightPtr--;
                }
            }

            // Calculate checksum
            long checksum = 0;
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i] != -1)
                    checksum += (long)i * blocks[i];
            }

            return checksum;
        }

        private sealed record FileInfo(int Id, int Size, int Position);
        private sealed record FreeSpace(int Position, int Size);

        private long SolvePart2(string diskMap)
        {
            // Parse the disk map to get file info
            var files = new List<FileInfo>();
            var freeSpaces = new List<FreeSpace>();
            
            int fileId = 0;
            int position = 0;
            
            for (int i = 0; i < diskMap.Length; i++)
            {
                int count = diskMap[i] - '0';
                if (i % 2 == 0)
                {
                    // File block
                    files.Add(new FileInfo(fileId, count, position));
                    fileId++;
                    position += count;
                }
                else
                {
                    // Free space
                    if (count > 0)
                        freeSpaces.Add(new FreeSpace(position, count));
                    position += count;
                }
            }

            // Try to move files from right to left (highest ID first)
            for (int i = files.Count - 1; i >= 0; i--)
            {
                var file = files[i];
                
                // Find the leftmost free space that fits this file
                for (int j = 0; j < freeSpaces.Count; j++)
                {
                    var space = freeSpaces[j];
                    
                    // Only move if the free space is to the left of the file
                    if (space.Position >= file.Position)
                        break;
                    
                    if (space.Size >= file.Size)
                    {
                        // Move the file to this free space
                        files[i] = file with { Position = space.Position };
                        
                        // Update the free space
                        if (space.Size == file.Size)
                        {
                            freeSpaces.RemoveAt(j);
                        }
                        else
                        {
                            freeSpaces[j] = space with { Position = space.Position + file.Size, Size = space.Size - file.Size };
                        }
                        
                        break;
                    }
                }
            }

            // Calculate checksum
            long checksum = 0;
            foreach (var file in files)
            {
                for (int i = 0; i < file.Size; i++)
                {
                    checksum += (long)(file.Position + i) * file.Id;
                }
            }

            return checksum;
        }
    }
}
