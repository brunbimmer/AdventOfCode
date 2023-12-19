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
    [AdventOfCode(Year = 2021, Day = 23)]
    public class Year2021Day23 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

         //The key here is that the hallway and amphipods being represented in a flat structure.
        //                      #############
        //                      #...........#
        //                      ###B#C#B#D###
        //                        #A#D#C#A#
        //                        #########
        //
        // Would end up as "...........BCBDADCA" in a char[]
        // Initial depth is according ot this map.
        private  char[] worldOfAmphipods;

        //hallway size
        private  int hallwaySize;

        private record struct BorrowState(char[] World, int Energy) { }

        //Cost per Amphipod (ordered char array index).
        //Index 0 is "A", Index 1 is "B", and so on.
        private  int[] cost = new[] { 1, 10, 100, 1000 };

        //Possible directions for a given Amphipod
        private  int[] directions = new[] { -1, 1 };

        public Year2021Day23()
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

            string input = FileIOHelper.getInstance().ReadDataAsString(file);
            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            _SW.Start();                       


            int cost = CalculateEnergyCost(input);      //input in 
            
            _SW.Stop();

            Console.WriteLine("Part 1: Energy Cost to organize the amphipods (depth 2): {0}", cost);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));

            _SW.Restart();

            //add the two new lines

            StringBuilder newInput = new StringBuilder();

            newInput.AppendLine(lines[0]);
            newInput.AppendLine(lines[1]);
            newInput.AppendLine(lines[2]);
            newInput.AppendLine("  #D#C#B#A#");
            newInput.AppendLine("  #D#B#A#C#");
            newInput.AppendLine(lines[3]);
            newInput.AppendLine(lines[4]);
           
            cost = CalculateEnergyCost(newInput.ToString());            

            _SW.Stop();

            Console.WriteLine("Part 2: Energy Cost to organize the amphipods (depth 4): {0}", cost);

            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(_SW));


        }       

        private  int CalculateEnergyCost(string input)
        {           
            //ordered naturally using Where clause.
            worldOfAmphipods = input.Where(c => c == '.' || (c >= 'A' && c <= 'D')).ToArray();  
            hallwaySize = worldOfAmphipods.Where(c => c == '.').Count();

            return Organize(new BorrowState(worldOfAmphipods, 0));
        }

        private  int Organize(BorrowState initialState)
        {
            // Dijkstra on the graph
            var depth = (initialState.World.Length - hallwaySize) / 4;
            var borrowStateQueue = new PriorityQueue<BorrowState, int>();
            borrowStateQueue.Enqueue(initialState, 0);
            var visited = new HashSet<string>();
      
            while (borrowStateQueue.Count > 0)
            {
                var node = borrowStateQueue.Dequeue();
                var worldString = new String(node.World);

                //if we visited this specific state, then continue skip and continue
                //to next world.
                if (visited.Contains(worldString))  
                {
                    continue;
                }

                if (IsSolved(node, depth))
                {
                    borrowStateQueue.Clear();       //clear the remaining items from queue.
                    return node.Energy;
                }

                //Add this state to the listed of visited worlds.
                visited.Add(worldString);   //we visited this state

                //Get a list of possible next borrow States and their costs. Place then in the queue.
                //Priority queue sorts itself naturally based on the energy value guaranting that we
                //are always working with the lowest energy states first in the queue.
                borrowStateQueue.EnqueueRange(GetAllPossibleMovements(depth, node).Select(n => (n, n.Energy)));
            }

            throw new Exception("Impossible to organize");
        }

        // Returns true if the hallway is empty and each room is organized with the
        // correct amphipod.
        private  bool IsSolved(BorrowState state, int depth)
        {
            for (int i = 0; i < hallwaySize; i++)
            {
                if (state.World[i] != '.')
                {
                    return false;
                }
            }

            for (int r = 0; r < 4; r++)
            {
                if (!IsRoomOrganized(state.World, depth, r))
                {
                    return false;
                }
            }

            return true;
        }

        //This method generates a list of possible movements and returns a list of possible
        //states and their cost associated to their states.
        private  List<BorrowState> GetAllPossibleMovements(int depth, BorrowState state)
        {
            var borrowStates = new List<BorrowState>();

            // Let's try to move any amphipod in any position in the hallway into their
            // target room.
            for (int i = 0; i < hallwaySize; i++)
            {                
                if (state.World[i] == '.')
                {
                    continue;
                }

                //found an amphipod. 
                var picked = state.World[i];

                //Get the integer index for the character. 
                var pickedIndex = picked - 'A';

                // Is the room empty or ready?
                // The pickedIndex represents the room in which the amphipod belongs.
                // How convenient. :-)
                bool canMoveToRoom = IsRoomOrganized(state.World, depth, pickedIndex);
                if (!canMoveToRoom)
                {
                    //room isn't ready for occuption.
                    continue;
                }

                //we get to this point if we can move into a room. Get the
                //hallway position of the amphipod's intended home.
                var targetPosition = 2 + 2 * pickedIndex;

                //determine what direction to move.
                var direction = targetPosition > i ? 1 : -1; 

                // Is the path empty between amphipod position and entrnace.
                for (int j = direction; Math.Abs(j) <= Math.Abs(targetPosition - i); j += direction)
                {
                    if (state.World[i + j] != '.')
                    {
                        //path is blocked.
                        canMoveToRoom = false;
                        break;
                    }
                }

                if (!canMoveToRoom)
                {
                    continue;
                }

                //hey, we can get to the room. Create a new borrow state.
                var newWorld = new char[state.World.Length];

                //copy the original borrow state.
                state.World.CopyTo(newWorld, 0);

                //clear the hallway position.
                newWorld[i] = '.';
                //push the ampipod into the room.
                PushRoom(newWorld, depth, pickedIndex, picked);

                //recalibrate the total energy cost to move amphipod into the room. 
                //current state cost + (cost to move in hallway + cost to move into room an specific depth) * step cost of amphipod.
                var newEnergy = state.Energy + (Math.Abs(targetPosition - i) + (depth - RoomCount(state.World, depth, pickedIndex))) * cost[pickedIndex];

                //add to list of neighbours.
                borrowStates.Add(new BorrowState(newWorld, newEnergy));
            }

            // If more then one amphipod was moved into a room. Then we return.
            if (borrowStates.Count > 0) return borrowStates;

            // Option 2: Remove one amphipod from a room. 
            for (int r = 0; r < 4; r++)
            {
                //room is empty or contains the correct amphipods. Skip to next
                //room.
                if (IsRoomOrganized(state.World, depth, r)) continue;
                

                //Take a peek at the amphipod in the room.
                var picked = PeekRoom(state.World, depth, r);
                var pickedIndex = picked - 'A';

                // Move the Amphipod
                // Possible targets are empty spaces on hallway, not in front of any room, with no blockers
                
                //calculate energy to leave the room from.
                var energy = state.Energy + (depth - RoomCount(state.World, depth, r) + 1) * (cost[pickedIndex]);

                var roomPosition = 2 + 2 * r;

                //Create a new borrowState for every position in each direction. In each direction until
                //we can't move anymore (either blocked or hallway edge).
                foreach (var direction in directions)
                {
                    var distance = direction;
                    while (roomPosition + distance >= 0 && roomPosition + distance < hallwaySize && state.World[roomPosition + distance] == '.')
                    {
                        if (roomPosition + distance == 2 || roomPosition + distance == 4 || roomPosition + distance == 6 || roomPosition + distance == 8)
                        {
                            // In front of a room, skip
                            distance += direction;
                            continue;
                        }

                        var newWorld = new char[state.World.Length];
                        state.World.CopyTo(newWorld, 0);

                        newWorld[roomPosition + distance] = picked;
                        PopRoom(newWorld, depth, r);

                        borrowStates.Add(new BorrowState(newWorld, energy + Math.Abs(distance) * cost[pickedIndex]));

                        distance += direction;
                    }
                }
            }

            return borrowStates;
        }

        //An organized room can either be an empty zoom (i.e., empty space represented by dots),
        //or an appropriate Amphipod in the room
        private  bool IsRoomOrganized(char[] world, int depth, int room)
        {
            for (int i = depth - 1; i >= 0; i--)
            {
                var c = world[hallwaySize + i * 4 + room];
                if (c == '.')
                {
                    return true;
                }
                if (c != 'A' + room)
                {
                    return false;
                }
            }
            return true;
        }

        //Returns the number of Amphipods in a given room.
        private  int RoomCount(char[] world, int depth, int room)
        {
            int count = 0;
            for (int i = depth - 1; i >= 0; i--)
            {
                var c = world[hallwaySize + i * 4 + room];
                if (c == '.')
                {
                    return count;
                }
                count++;
            }
            return count;
        }

        //Push character into a room.
        private  void PushRoom(char[] world, int depth, int room, char amphipod)
        {
            for (int i = depth - 1; i >= 0; i--)
            {
                var index = hallwaySize + i * 4 + room;
                if (world[index] == '.')
                {
                    world[index] = amphipod;
                    return;
                }
            }
            throw new Exception("Cannot push into full room");
        }

        //Pop character into a room.
        private  void PopRoom(char[] world, int depth, int room)
        {
            for (int i = 0; i < depth; i++)
            {
                var index = hallwaySize + i * 4 + room;
                if (world[index] != '.')
                {
                    world[index] = '.';
                    return;
                }
            }
            throw new Exception("Cannot pop from empty room");
        }

        //Take a peek at character in a specific room
        private  char PeekRoom(char[] world, int depth, int room)
        {
            for (int i = 0; i < depth; i++)
            {
                var index = hallwaySize + i * 4 + room;
                if (world[index] != '.')
                {
                    return world[index];
                }
            }
            throw new Exception("Cannot peek at empty room");
        }       
    }
}
