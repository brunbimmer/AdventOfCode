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
    [AdventOfCode(Year = 2021, Day = 19)]
    public class Year2021Day19 : IAdventOfCode
    {
        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch SW { get; set; }

        private   Dictionary<int, HashSet<Coordinate3D>> _scannerData;
        private   HashSet<Coordinate3D> _beaconMap;
        private   Dictionary<int, Coordinate3D> _scannerPositions;

        public Year2021Day19()
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
            _scannerData = new();

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);

            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);

            SW.Start();                       

            InitializeDataSets(lines);
            List<Coordinate3D> beacons = new List<Coordinate3D>();

            int numberOfBeacons = Part1();

            SW.Stop();

            Console.WriteLine("Part 1: Number of beacons: {0}", numberOfBeacons);
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));

            SW.Restart();

            int distance = Part2();           
            
            SW.Stop();

            Console.WriteLine("Part 2: Maximum distance between any two scanners: {0}", distance);
            
            Console.WriteLine("   Execution Time: {0}", StopwatchUtil.getInstance().GetTimestamp(SW));


        }       

        private   void InitializeDataSets(string[] lines)
        {
            HashSet<Coordinate3D> scannerReadings = null;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.Contains("scanner"))
                {
                    scannerReadings = new HashSet<Coordinate3D>();
                    
                    //extract scanner ID from this line.
                    int scannerId = int.Parse(Regex.Matches(line, @"(\d+)")[0].ToString()); 
                    _scannerData.Add(scannerId, scannerReadings);
                }
                else
                {
                    string[] coordinates = line.Split(',');
                    Coordinate3D point = new Coordinate3D(int.Parse(coordinates[0]), int.Parse(coordinates[1]), int.Parse(coordinates[2]));
                    scannerReadings.Add(point);
                }
            }

            _beaconMap = new HashSet<Coordinate3D>(_scannerData[0]);          //Scanner Position 0 and list of already known beacons. This is the minimum number of beacons.
            _scannerPositions = new() { [0] = new Coordinate3D(0, 0, 0) };    //Scanner position 0 is already known and is the central Coordinate3D at 0,0,0. 
        }

        public   int Part1()
        {
            //kick thinks off by mapping the Beacon Space.
            MapSpace();                 
            return _beaconMap.Count;
        }

        public   int Part2()
        {
            HashSet<(int, int)> tested = new();
            int maxDistance = 0;

            foreach ((int scannerFromId, Coordinate3D scannerFrom) in _scannerPositions)
            {
                foreach ((int scannerToId, Coordinate3D scannerTo) in _scannerPositions)
                {
                    if (scannerFromId == scannerToId)
                    {
                        continue;
                    }

                    var key1 = (scannerFromId, scannerToId);
                    var key2 = (scannerToId, scannerFromId);

                    if (tested.Contains(key1) || tested.Contains(key2))
                    {
                        continue;
                    }

                    int distance = scannerFrom.ManhattenDistance(scannerTo);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }

                    tested.Add(key1);
                }
            }

            return maxDistance;
        }

        private   void MapSpace()
        {
            Dictionary<Coordinate3D, Coordinate3D> beaconVectorMap = ReadVectors(_beaconMap);

            Queue<int> scannersToCheck = new();
            for (int i = 1; i < _scannerData.Count; i++)
            {
                scannersToCheck.Enqueue(i);
            }

            while (scannersToCheck.Count > 0)
            {
                int scanner = scannersToCheck.Dequeue();
                var readings = _scannerData[scanner];

                Func<Coordinate3D, Coordinate3D> scannerRotation = null;
                Coordinate3D newScannerCoordinate3Ds = null;

                foreach (var rotation in GetRotations())
                {
                    if (TestRotation(beaconVectorMap, readings, rotation, out newScannerCoordinate3Ds))
                    {
                        scannerRotation = rotation;
                        break;
                    }
                }

                if (scannerRotation != null)
                {
                    var rotated = RotateScannerReadings(readings, scannerRotation);
                    var translatedScannerList = TranslateScannerReadings(rotated, newScannerCoordinate3Ds);

                    foreach (Coordinate3D beacon in translatedScannerList)
                    {
                        _beaconMap.Add(beacon);     //this call will not add duplicates.
                    }

                    beaconVectorMap = ReadVectors(_beaconMap);

                    _scannerPositions.Add(scanner, newScannerCoordinate3Ds);
                }
                else
                {
                    scannersToCheck.Enqueue(scanner);
                }
            }
        }

        private   Dictionary<Coordinate3D, Coordinate3D> ReadVectors(HashSet<Coordinate3D> beaconMap)
        {
            Dictionary<Coordinate3D, Coordinate3D> vectors = new();
            foreach (var p1 in beaconMap)
            {
                foreach (var p2 in beaconMap)
                {
                    if (p1 == p2) continue;
                    Coordinate3D vector = p2.Vector(p1);
                    if (!vectors.ContainsKey(vector))
                    {
                        vectors.Add(vector, p2);
                    }
                }
            }
            return vectors;
        }

        /// <summary>
        /// Test a rotation
        /// </summary>
        /// <param name="masterVectors">Master set of Vectors based on the overall Beacon Map</param>
        /// <param name="beacons">Beacons positions from the other scanner </param>
        /// <param name="rotation">Rotation Metric</param>
        /// <param name="translation">Translated Coordinate3D</param>
        /// <returns></returns>
        private   bool TestRotation(Dictionary<Coordinate3D, Coordinate3D> masterVectors, HashSet<Coordinate3D> beacons, Func<Coordinate3D, Coordinate3D> rotation, out Coordinate3D newScannerCoordinate3Ds)
        {
            int matchCount = 0;
            foreach (var p1 in beacons)
            {
                Coordinate3D p1Rotated = rotation(p1);
                foreach (var p2 in beacons)
                {
                    if (p1 == p2) continue;

                    Coordinate3D p2Rotated = rotation(p2);
                    Coordinate3D vector = p1Rotated.Vector(p2Rotated);

                    if (masterVectors.ContainsKey(vector) && ++matchCount == 12)
                    {
                        newScannerCoordinate3Ds = p1Rotated.Vector(masterVectors[vector]);
                        return true;
                    }
                }
            }

            newScannerCoordinate3Ds = null;
            return false;
        }

        private   HashSet<Coordinate3D> RotateScannerReadings(HashSet<Coordinate3D> beacons, Func<Coordinate3D, Coordinate3D> scannerRotation)
        {
            HashSet<Coordinate3D> rotatedBeacons = new();
            foreach (Coordinate3D beacon in beacons)
            {
                rotatedBeacons.Add(scannerRotation(beacon));
            }
            return rotatedBeacons;
        }

        private   HashSet<Coordinate3D> TranslateScannerReadings(HashSet<Coordinate3D> beacons, Coordinate3D translation)
        {
            HashSet<Coordinate3D> translatedBeacons = new();
            foreach (Coordinate3D beacon in beacons)
            {
                translatedBeacons.Add(beacon.Translate(translation));
            }
            return translatedBeacons;
        }


        private   IEnumerable<Func<Coordinate3D, Coordinate3D>> GetRotations()
        {
            //original rotation for reference            
            //                      rec X,  rec.Y,  rec.Z

            yield return rec => new(rec.X, -rec.Z,  rec.Y);
            yield return rec => new(rec.X, -rec.Y, -rec.Z);
            yield return rec => new(rec.X,  rec.Z, -rec.Y);

            yield return rec => new(-rec.Y, rec.X,  rec.Z);
            yield return rec => new( rec.Z, rec.X,  rec.Y);
            yield return rec => new( rec.Y, rec.X, -rec.Z);
            yield return rec => new(-rec.Z, rec.X, -rec.Y);

            yield return rec => new(-rec.X, -rec.Y,  rec.Z);
            yield return rec => new(-rec.X, -rec.Z, -rec.Y);
            yield return rec => new(-rec.X,  rec.Y, -rec.Z);
            yield return rec => new(-rec.X,  rec.Z,  rec.Y);

            yield return rec => new( rec.Y, -rec.X,  rec.Z);
            yield return rec => new( rec.Z, -rec.X, -rec.Y);
            yield return rec => new(-rec.Y, -rec.X, -rec.Z);
            yield return rec => new(-rec.Z, -rec.X,  rec.Y);

            yield return rec => new(-rec.Z,  rec.Y, rec.X);
            yield return rec => new( rec.Y,  rec.Z, rec.X);
            yield return rec => new( rec.Z, -rec.Y, rec.X);
            yield return rec => new(-rec.Y, -rec.Z, rec.X);

            yield return rec => new(-rec.Z, -rec.Y, -rec.X);
            yield return rec => new(-rec.Y,  rec.Z, -rec.X);
            yield return rec => new( rec.Z,  rec.Y, -rec.X);
            yield return rec => new( rec.Y, -rec.Z, -rec.X);
        }

    }
}
