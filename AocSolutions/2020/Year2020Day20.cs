using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventFileIO;
using Common;

namespace AdventOfCode
{
    [AdventOfCode(Year = 2020, Day = 20)]
    public class Year2020Day20 : IAdventOfCode
    {
        private sealed record Tile(long Id, string[] Pixels);
        private sealed record OrientedTile(long Id, string[] Pixels, string Top, string Bottom, string Left, string Right);

        private int _Year;
        private int _Day;
        private string _OverrideFile;

        public Stopwatch _SW { get; set; }

        public Year2020Day20()
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

            string file = FileIOHelper.getInstance().InitFileInput(_Year, _Day, _OverrideFile ?? path);
            string[] lines = FileIOHelper.getInstance().ReadDataAsLines(file);
            Tile[] tiles = ParseTiles(lines);

            _SW.Restart();
            long product = Part1(tiles);
            _SW.Stop();

            output.WritePart(1, product, trackTime ? _SW.Elapsed : null, "Product of the four corner tile IDs");


            _SW.Restart();
            int roughness = Part2(tiles);
            _SW.Stop();

            output.WritePart(2, roughness, trackTime ? _SW.Elapsed : null, "Water roughness after finding sea monsters");

        }

        private Tile[] ParseTiles(string[] lines)
        {
            List<Tile> tiles = new List<Tile>();
            List<string> pixels = new List<string>();
            long? id = null;

            foreach (string line in lines.Append(string.Empty))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (pixels.Count == 0)
                        continue;
                    if (!id.HasValue || pixels.Any(row => row.Length != pixels.Count))
                        throw new FormatException("Each tile must have an ID and a square pixel grid.");

                    tiles.Add(new Tile(id.Value, pixels.ToArray()));
                    pixels.Clear();
                    id = null;
                }
                else if (line.StartsWith("Tile ", StringComparison.Ordinal))
                {
                    id = long.Parse(line.AsSpan(5, line.Length - 6));
                }
                else
                {
                    pixels.Add(line);
                }
            }

            return tiles.ToArray();
        }

        private long Part1(Tile[] tiles)
        {
            var edgeOwners = BuildEdgeOwners(tiles);
            long[] corners = tiles
                .Where(tile => GetEdges(tile.Pixels).Count(edge => edgeOwners[NormalizeEdge(edge)].Count == 1) == 2)
                .Select(tile => tile.Id).ToArray();
            if (corners.Length != 4)
                throw new InvalidOperationException("Expected exactly four tiles with two unmatched borders.");

            return corners.Aggregate(1L, (product, id) => checked(product * id));
        }

        private Dictionary<string, HashSet<long>> BuildEdgeOwners(Tile[] tiles)
        {
            var edgeOwners = new Dictionary<string, HashSet<long>>();
            foreach (Tile tile in tiles)
            {
                foreach (string edge in GetEdges(tile.Pixels).Select(NormalizeEdge))
                {
                    if (!edgeOwners.TryGetValue(edge, out HashSet<long> owners))
                    {
                        owners = new HashSet<long>();
                        edgeOwners[edge] = owners;
                    }

                    owners.Add(tile.Id);
                }
            }

            return edgeOwners;
        }

        private int Part2(Tile[] tiles)
        {
            return FindWaterRoughness(AssembleImage(tiles));
        }

        private string[] AssembleImage(Tile[] tiles)
        {
            int side = (int)Math.Sqrt(tiles.Length);
            if (side == 0 || side * side != tiles.Length)
                throw new FormatException("The tile count must form a nonempty square.");

            int tileSize = tiles[0].Pixels.Length;
            if (tileSize < 3 || tiles.Any(tile => tile.Pixels.Length != tileSize))
                throw new FormatException("Tiles must have equal dimensions and an interior inside their borders.");

            var edgeOwners = BuildEdgeOwners(tiles);
            bool IsOuterEdge(string edge) => edgeOwners[NormalizeEdge(edge)].Count == 1;

            OrientedTile[] orientations = tiles.SelectMany(tile => GetOrientations(tile.Pixels).Select(pixels =>
            {
                string[] edges = GetEdges(pixels);
                return new OrientedTile(tile.Id, pixels, edges[0], edges[1], edges[2], edges[3]);
            })).ToArray();
            var byTop = orientations.GroupBy(tile => tile.Top).ToDictionary(group => group.Key, group => group.ToArray());
            var byLeft = orientations.GroupBy(tile => tile.Left).ToDictionary(group => group.Key, group => group.ToArray());
            OrientedTile[] placed = new OrientedTile[tiles.Length];
            HashSet<long> used = new HashSet<long>();

            bool Place(int index)
            {
                if (index == placed.Length)
                    return true;

                int row = index / side;
                int column = index % side;
                string top = row > 0 ? placed[index - side].Bottom : null;
                string left = column > 0 ? placed[index - 1].Right : null;
                OrientedTile[] candidates = orientations;
                if (top != null)
                    candidates = byTop.GetValueOrDefault(top, Array.Empty<OrientedTile>());
                else if (left != null)
                    candidates = byLeft.GetValueOrDefault(left, Array.Empty<OrientedTile>());

                foreach (OrientedTile tile in candidates)
                {
                    if (used.Contains(tile.Id) || (left != null && tile.Left != left)
                        || IsOuterEdge(tile.Top) != (row == 0)
                        || IsOuterEdge(tile.Bottom) != (row == side - 1)
                        || IsOuterEdge(tile.Left) != (column == 0)
                        || IsOuterEdge(tile.Right) != (column == side - 1))
                        continue;

                    placed[index] = tile;
                    used.Add(tile.Id);
                    if (Place(index + 1))
                        return true;
                    used.Remove(tile.Id);
                }

                return false;
            }

            if (!Place(0))
                throw new InvalidOperationException("The tiles cannot be assembled with matching borders.");

            int interiorSize = tileSize - 2;
            string[] image = new string[side * interiorSize];
            for (int tileRow = 0; tileRow < side; tileRow++)
            {
                for (int row = 0; row < interiorSize; row++)
                {
                    char[] pixels = new char[side * interiorSize];
                    for (int tileColumn = 0; tileColumn < side; tileColumn++)
                    {
                        placed[tileRow * side + tileColumn].Pixels[row + 1]
                            .AsSpan(1, interiorSize).CopyTo(pixels.AsSpan(tileColumn * interiorSize, interiorSize));
                    }

                    image[tileRow * interiorSize + row] = new string(pixels);
                }
            }

            return image;
        }

        private IEnumerable<string[]> GetOrientations(string[] pixels)
        {
            for (int flip = 0; flip < 2; flip++)
            {
                string[] current = flip == 0 ? pixels : pixels.Select(row => new string(row.Reverse().ToArray())).ToArray();
                for (int rotation = 0; rotation < 4; rotation++)
                {
                    yield return current;
                    current = RotateClockwise(current);
                }
            }
        }

        private string[] RotateClockwise(string[] pixels)
        {
            int height = pixels.Length;
            int width = height == 0 ? 0 : pixels[0].Length;
            string[] rotated = new string[width];
            for (int row = 0; row < width; row++)
            {
                char[] nextRow = new char[height];
                for (int column = 0; column < height; column++)
                    nextRow[column] = pixels[height - column - 1][row];
                rotated[row] = new string(nextRow);
            }

            return rotated;
        }

        private int FindWaterRoughness(string[] image)
        {
            string[] monster =
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };
            var monsterPixels = new List<(int X, int Y)>();
            for (int row = 0; row < monster.Length; row++)
            {
                for (int column = 0; column < monster[row].Length; column++)
                {
                    if (monster[row][column] == '#')
                        monsterPixels.Add((column, row));
                }
            }

            int totalHashes = image.Sum(row => row.Count(pixel => pixel == '#'));
            int mostMonsterPixels = 0;
            foreach (string[] orientation in GetOrientations(image))
            {
                var marked = new HashSet<(int X, int Y)>();
                for (int row = 0; row <= orientation.Length - monster.Length; row++)
                {
                    for (int column = 0; column <= orientation[row].Length - monster[0].Length; column++)
                    {
                        if (monsterPixels.All(pixel => orientation[row + pixel.Y][column + pixel.X] == '#'))
                        {
                            foreach (var pixel in monsterPixels)
                                marked.Add((column + pixel.X, row + pixel.Y));
                        }
                    }
                }

                mostMonsterPixels = Math.Max(mostMonsterPixels, marked.Count);
            }

            return totalHashes - mostMonsterPixels;
        }

        private string[] GetEdges(string[] pixels)
        {
            return new[]
            {
                pixels[0],
                pixels[^1],
                new string(pixels.Select(row => row[0]).ToArray()),
                new string(pixels.Select(row => row[^1]).ToArray())
            };
        }

        private string NormalizeEdge(string edge)
        {
            string reversed = new string(edge.Reverse().ToArray());
            return string.CompareOrdinal(edge, reversed) <= 0 ? edge : reversed;
        }
    }
}
