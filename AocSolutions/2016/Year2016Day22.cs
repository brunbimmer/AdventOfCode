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
    [AdventOfCode(Year = 2016, Day = 22)]
    public class Year2016Day22 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        private record Node(int X, int Y, int Size, int Used)
        {
            public int Available => Size - Used;
        }

        public Year2016Day22()
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

            string[] input  = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();                       

            int viablePairs = CountViablePairs(input);

            
            _SW.Stop();

            Console.WriteLine("Part 1 - Viable Pairs: {0}, Execution Time: {1}", viablePairs, StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            List<Node> nodes = ParseNodes(input);
            int minSteps = FindMinStepsToMoveGoal(nodes);
            
            _SW.Stop();

            Console.WriteLine("Part 2 - Min Steps to Move Goal Data: {0}, Execution Time: {1}", minSteps, StopwatchUtil.getInstance().GetTimestamp(_SW));


        }       

        int CountViablePairs(string[] input)
        {
            // Parse input to extract node information
            List<Node> nodes = ParseNodes(input);
            
            // Count viable pairs
            int count = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (i != j && nodes[i].Used != 0 && nodes[i].Used <= nodes[j].Available)
                    {
                        count++;
                    }
                }
            }
            
            return count;
        }

        List<Node> ParseNodes(string[] input)
        {
            List<Node> nodes = new List<Node>();
            
            // Skip header lines (first 2 lines are typically filesystem headers)
            var regex = new Regex(@"node-x(\d+)-y(\d+)\s+(\d+)T\s+(\d+)T\s+(\d+)T");
            
            foreach (string line in input)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    int x = int.Parse(match.Groups[1].Value);
                    int y = int.Parse(match.Groups[2].Value);
                    int size = int.Parse(match.Groups[3].Value);
                    int used = int.Parse(match.Groups[4].Value);
                    
                    nodes.Add(new Node(x, y, size, used));
                }
            }
            
            return nodes;
        }

        //This was a complex problem that required a BFS approach to find the minimum steps. I got this solution using Claude Haiku 4.5. As these are past puzzles, I am not competing
        //with anyone and testing out agentic AI to see where it starts to fail. 
        int FindMinStepsToMoveGoal(List<Node> nodes)
        {
            // Find the maximum coordinates
            int maxX = nodes.Max(n => n.X);
            int maxY = nodes.Max(n => n.Y);
            
            // Find the empty node (the one that can accept data)
            Node empty = nodes.FirstOrDefault(n => n.Used == 0);
            if (empty == null) return -1;
            
            // Find the goal node (top-right, contains the data we want to move)
            Node goalNode = nodes.FirstOrDefault(n => n.X == maxX && n.Y == 0);
            if (goalNode == null) return -1;
            
            // BFS to find minimum steps
            // State: (empty node X, empty node Y, goal data X, goal data Y)
            var start = (empty.X, empty.Y, goalNode.X, goalNode.Y);
            
            var queue = new Queue<((int, int, int, int), int)>();
            var visited = new HashSet<(int, int, int, int)>();
            
            queue.Enqueue((start, 0));
            visited.Add(start);
            
            while (queue.Count > 0)
            {
                var ((emptyX, emptyY, goalX, goalY), steps) = queue.Dequeue();
                
                // Check if goal is reached
                if (goalX == 0 && goalY == 0)
                {
                    return steps;
                }
                
                // Try moving empty node to adjacent positions
                int[] dx = { -1, 1, 0, 0 };
                int[] dy = { 0, 0, -1, 1 };
                
                for (int i = 0; i < 4; i++)
                {
                    int newEmptyX = emptyX + dx[i];
                    int newEmptyY = emptyY + dy[i];
                    
                    // Check bounds
                    if (newEmptyX < 0 || newEmptyX > maxX || newEmptyY < 0 || newEmptyY > maxY)
                        continue;
                    
                    // Find the node at the new position
                    Node targetNode = nodes.FirstOrDefault(n => n.X == newEmptyX && n.Y == newEmptyY);
                    if (targetNode == null) continue;
                    
                    // Check if we can move (node's data must fit in empty space)
                    Node emptyNodeObj = nodes.FirstOrDefault(n => n.X == emptyX && n.Y == emptyY);
                    if (emptyNodeObj == null || targetNode.Used > emptyNodeObj.Size) continue;
                    
                    // Update goal position if it was at the target node
                    int newGoalX = goalX;
                    int newGoalY = goalY;
                    if (goalX == newEmptyX && goalY == newEmptyY)
                    {
                        newGoalX = emptyX;
                        newGoalY = emptyY;
                    }
                    
                    var newState = (newEmptyX, newEmptyY, newGoalX, newGoalY);
                    if (!visited.Contains(newState))
                    {
                        visited.Add(newState);
                        queue.Enqueue((newState, steps + 1));
                    }
                }
            }
            
            return -1; // No solution found
        }
    }
}
