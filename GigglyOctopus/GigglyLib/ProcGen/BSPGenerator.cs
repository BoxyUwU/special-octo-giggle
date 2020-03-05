using System;
using System.Collections.Generic;

namespace GigglyLib.ProcGen
{
    public class BSPSplit
    {
        public int X1;
        public int Y1;
        public int X2;
        public int Y2;
        public bool[,] Reg1;
        public bool[,] Reg2;
    }

    public class BSPGenerator
    {
        public void Generate(bool[,] tiles)
        {
            bool[,] sanitizedTiles = GetLargestRegion(tiles);
            BSPSplit split = SplitRegion(sanitizedTiles, false);
        }
    
        public BSPSplit SplitRegion(bool[,] region, bool verticalSplit)
        {
            bool[,] clonedRegion = (bool[,])region.Clone();

            BSPSplit output = null;

            int best = int.MaxValue;

            int splitIndexMax = verticalSplit == true ? clonedRegion.GetLength(0) : clonedRegion.GetLength(1);
            int splitLengthMax = verticalSplit == true ? clonedRegion.GetLength(1) : clonedRegion.GetLength(0);

            for (int spltIndex = 0; spltIndex < splitIndexMax; spltIndex++)
            {
                BSPSplit thisSplit = null;
                bool startedSplit = false;
                for (int splitLength = 0; splitLength < splitLengthMax; splitLength++)
                {
                    int x = verticalSplit == true ? spltIndex : splitLength;
                    int y = verticalSplit == true ? splitLength : spltIndex;

                    if (clonedRegion[x, y] && !startedSplit)
                    {
                        thisSplit = new BSPSplit();
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

                if (GetRegionCount(clonedRegion) == 2)
                {
                    var (first, firstRegion, second, secondRegion) = GetTileCount(clonedRegion);
                    if (first != int.MinValue && second != int.MinValue)
                    {
                        var diff = first - second;
                        if (diff < best)
                        {
                            best = diff;
                            output = thisSplit;
                            thisSplit.Reg1 = firstRegion;
                            thisSplit.Reg2 = secondRegion;
                        }
                    }
                }
                clonedRegion = (bool[,])region.Clone();
            }
            return output;
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

        private (int first, bool[,] firstRegion, int second, bool[,] secondRegion) GetTileCount(bool[,] regions)
        {
            (int first, bool[,] firstRegion, int second, bool[,] secondRegion) output = (int.MinValue, null, int.MinValue, null);

            for (int x = 0; x < regions.GetLength(0); x++)
            {
                for (int y = 0; y < regions.GetLength(1); y++) 
                {
                    if (regions[x, y])
                    {
                        (int tiles, bool[,] filledRegion) = FloodFill(regions, x, y);
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
