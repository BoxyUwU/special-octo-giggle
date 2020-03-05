using System;
using System.Collections.Generic;

namespace GigglyLib.ProcGen
{
    public class BSPSplit
    {
        public bool IsLeaf = false;
        public int X1;
        public int Y1;
        public int X2;
        public int Y2;
        public BSPSplit Child1;
        public BSPSplit Child2;
        public bool[,] Region;
        public bool verticalSplit;
    }

    public class BSPGenerator
    {
        Random _rand;
        public BSPGenerator(int seed)
        {
            _rand = new Random(seed);
        }

        public (BSPSplit root, List<BSPSplit> leafs) Generate(bool[,] tiles, int splitThreshold)
        {
            bool[,] sanitizedTiles = GetLargestRegion(tiles);

            var root = new BSPSplit { Region = sanitizedTiles };
            var leafs = new List<BSPSplit>();
            var openSet = new List<BSPSplit> { root };
            var closedSet = new List<BSPSplit>();
            int totalSplits = 0;
            int closedSetIndex = 0;
            while (openSet.Count > 0)
            {
                SplitRegion(openSet[0]);
                closedSet.Add(openSet[0]);
                openSet.RemoveAt(0);
                totalSplits++;
                if (openSet.Count == 0)
                    for (int i = closedSetIndex; i < closedSet.Count; i++)
                    {
                        if (GetTileCount(closedSet[i].Child1.Region) >= splitThreshold)
                            openSet.Add(closedSet[i].Child1);
                        else
                        {
                            closedSet[i].Child1.IsLeaf = true;
                            leafs.Add(closedSet[i].Child1);
                        }

                        if (GetTileCount(closedSet[i].Child2.Region) >= splitThreshold)
                            openSet.Add(closedSet[i].Child2);
                        else
                        {
                            closedSet[i].Child2.IsLeaf = true;
                            leafs.Add(closedSet[i].Child2);
                        }

                        closedSetIndex++;
                    }
            }
            Console.WriteLine($"BSP finished with {totalSplits} total splits, {leafs.Count} leafs");
            return (root, leafs);
        }
    
        public void SplitRegion(BSPSplit split)
        {
            bool verticalSplit = split.verticalSplit;
            bool[,] region = split.Region;
            bool[,] clonedRegion = (bool[,])region.Clone();

            BSPSplit output = null;

            int best = int.MaxValue;

            int splitIndexMax = verticalSplit == true ? clonedRegion.GetLength(0) : clonedRegion.GetLength(1);
            int splitLengthMax = verticalSplit == true ? clonedRegion.GetLength(1) : clonedRegion.GetLength(0);

            for (int spltIndex = 0; spltIndex < splitIndexMax; spltIndex++)
            {
                BSPSplit thisSplit = null;
                bool startedSplit = false;

                // Travel in the split direction until reaching an air tile then stop
                // Replaces wall tiles with air tiles along the way
                for (int splitLength = 0; splitLength < splitLengthMax; splitLength++)
                {
                    int x = verticalSplit == true ? spltIndex : splitLength;
                    int y = verticalSplit == true ? splitLength : spltIndex;

                    if (clonedRegion[x, y] && !startedSplit)
                    {
                        thisSplit = new BSPSplit()
                        {
                            X1 = x,
                            Y1 = y,
                        };
                        startedSplit = true;
                        thisSplit.X1 = x;
                        thisSplit.Y1 = y;
                    }

                    if ((clonedRegion[x, y] == false || splitLength == splitLengthMax-1) && startedSplit)
                    {
                        thisSplit.X2 = x;
                        thisSplit.Y2 = y;
                        break;
                    }
                    clonedRegion[x, y] = false;
                }

                // Compare split quality to last split
                var (first, firstRegion, second, secondRegion, total) = GetTwoTileCount(clonedRegion);
                if (total == 2)
                {
                    if (first != int.MinValue && second != int.MinValue)
                    {
                        var diff = first - second;
                        if (diff < best)
                        {
                            best = diff;
                            output = thisSplit;
                            thisSplit.Child1 = new BSPSplit { Region = firstRegion, verticalSplit = !verticalSplit };
                            thisSplit.Child2 = new BSPSplit { Region = secondRegion, verticalSplit = !verticalSplit };
                        }
                    }
                }
                clonedRegion = (bool[,])region.Clone();
            }
            split.Child1 = output.Child1;
            split.Child2 = output.Child2;
            split.X1 = output.X1;
            split.X2 = output.X2;
            split.Y1 = output.Y1;
            split.Y2 = output.Y2;
        }

        private int GetRegionCount(bool[,] map)
        {
            int count = 0;
            bool[,] clonedMap = (bool[,])map.Clone();
            for (int x = 0; x < clonedMap.GetLength(0); x++)
            {
                for (int y = 0; y < clonedMap.GetLength(1); y++)
                {
                    if (clonedMap[x, y])
                    {
                        count++;
                        FloodFill(clonedMap, x, y);
                    }
                }
            }
            return count;
        }

        private int GetTileCount(bool[,] region)
        {
            for (int x = 0; x < region.GetLength(0); x++)
            {
                for (int y = 0; y < region.GetLength(1); y++)
                {
                    if (region[x, y])
                    {
                        return FloodFill((bool[,])region.Clone(), x, y).count;
                    }
                }
            }
            return 0;
        }

        private (int first, bool[,] firstRegion, int second, bool[,] secondRegion, int total) GetTwoTileCount(bool[,] regions)
        {
            (int first, bool[,] firstRegion, int second, bool[,] secondRegion, int total) output = (int.MinValue, null, int.MinValue, null, 0);

            int height = regions.GetLength(1);
            for (int x = 0; x < regions.GetLength(0); x++)
            {
                for (int y = 0; y < height; y++) 
                {
                    if (regions[x, y])
                    {
                        (int tiles, bool[,] filledRegion) = FloodFill(regions, x, y);
                        output.total++;
                        if (tiles > output.first)
                        {
                            output.second = output.first;
                            output.secondRegion = output.firstRegion;
                            output.first = tiles;
                            output.firstRegion = filledRegion;
                        }
                        else if (tiles > output.second)
                        {
                            output.second = tiles;
                            output.secondRegion = filledRegion;
                        }
                    }
                } 
            }

            return output;
        }

        private (int count, bool[,] region) FloodFill(bool[,] region, int x, int y)
        {
            bool[,] filledRegion = new bool[region.GetLength(0), region.GetLength(1)];
            int tileCount = 0;
            List<(int x, int y)> openSet = new List<(int x, int y)>{
                (x, y),
            };

            while (openSet.Count > 0)
            {
                (x, y) = openSet[0];
                if (region[x, y])
                {
                    region[x, y] = false;
                    filledRegion[x, y] = true;
                    tileCount++;
                    openSet.RemoveAt(0);

                    if (x + 1 < region.GetLength(0))
                        openSet.Add((x + 1, y));
                    if (x - 1 >= 0)
                        openSet.Add((x - 1, y));
                    if (y + 1 < region.GetLength(1))
                        openSet.Add((x, y + 1));
                    if (y - 1 >= 0)
                        openSet.Add((x, y - 1));
                }
                else
                {
                    openSet.RemoveAt(0);
                }
            }

            return (tileCount, filledRegion);
        }

        private bool[,] GetLargestRegion(bool[,] map)
        {
            var copyMap = (bool[,])map.Clone();
            (int count, bool[,] region) highest = (-1, null);
            for (int x = 0; x < copyMap.GetLength(0); x++)
                for (int y = 0; y < copyMap.GetLength(1); y++)
                {
                    if (copyMap[x, y])
                    {
                        var res = FloodFill(copyMap, x, y);
                        if (res.count > highest.count)
                        {
                            highest.count = res.count;
                            highest.region = res.region;
                        }
                    }

                }
            return highest.region;
        }
    }
}
