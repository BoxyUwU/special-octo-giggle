using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GigglyLib.ProcGen
{
    public class Room
    {
        public Rectangle Region;
        public List<int> Links;

        public Room() { }
        public Room(Room room)
        {
            Region = room.Region;
            Links = new List<int>(room.Links);
        }
    }

    public class RoomGenerator
    {
        Random _rand;

        public RoomGenerator(int seed)
        {
            _rand = new Random(seed);
        }

        public List<Room> Generate(bool[,] map)
        {
            List<Room> rooms = GenerateRooms(map);
            BuildLinks(rooms);
            GenerateHallways(rooms, map);
            Console.WriteLine($"RoomGenerator finished with {rooms.Count} rooms generated");
            return rooms;
        }

        private void GenerateHallways(List<Room> rooms, bool[,] map)
        {
            int[,] costGraph = BuildCostGraph(map, rooms);

            List<(int start, int end)> possibilities = new List<(int start, int end)>();
            for (int i = 0; i < rooms.Count; i++)
                for (int j = 0; j < rooms.Count; j++)
                    if (i != j)
                    {
                        possibilities.Add((i, j));
                    }

            List<Room> newRooms;
            List<Room> path;
            bool res = false;
            do
            {
                newRooms = new List<Room>();
                for (int i = 0; i < rooms.Count; i++)
                    newRooms.Add(new Room(rooms[i]));

                int possibility = _rand.Next(0, possibilities.Count);
                var (start, end) = possibilities[possibility];

                path = new List<Room> { newRooms[start] };
                res = RecursiveGetPath(newRooms[end], path, newRooms, map, costGraph);
                possibilities.RemoveAt(possibility);
            } while (possibilities.Count > 0 && res == false);

            List<Room> leafs = new List<Room>();
            for (int i = 0; i < newRooms.Count; i++)
                if (!path.Contains(newRooms[i]))
                    leafs.Add(newRooms[i]);
            ConnectLeafs(leafs, newRooms, map, costGraph);
        }

        private void ConnectLeafs(List<Room> leafs, List<Room> rooms, bool[,] map, int[,] costGraph)
        {
            for (int i = 0; i < leafs.Count; i++)
            {
                Room leaf = leafs[i];
                bool worked = false;
                List<int> possibleConnections = new List<int>(leaf.Links);
                do
                {
                    int connection = _rand.Next(0, possibleConnections.Count);
                    worked = CarveHallway(leaf, rooms[possibleConnections[connection]], map, costGraph, false);
                    possibleConnections.RemoveAt(connection);
                } while (possibleConnections.Count > 0 && !worked);
            }
        }

        private bool RecursiveGetPath(Room end, List<Room> path, List<Room> roomSet, bool[,] map, int[,] costGraph)
        {
            List<int> options = new List<int>(path[path.Count-1].Links);
            for (int i = 0; i < options.Count;)
            {
                if (path.Contains(roomSet[options[i]]))
                    options.RemoveAt(i);
                else
                    i++;
            }

            while (options.Count > 0)
            {
                int picked = _rand.Next(0, options.Count);
                path.Add(roomSet[options[picked]]);
                options.RemoveAt(picked);

                // If we reached goal
                if (path[path.Count-1].Region.X == end.Region.X && path[path.Count-1].Region.Y == end.Region.Y)
                {
                    // any constraints we want
                    if (path.Count == roomSet.Count - 3)
                    {
                        if (CarveHallways(path, map, costGraph))
                            return true;
                    }
                }

                bool res = RecursiveGetPath(end, path, roomSet, map, costGraph);
                if (res)
                    return true;
                path.RemoveAt(path.Count-1);
            }
            return false;
        }

        private bool CarveHallways(List<Room> path, bool[,] map, int[,] costGraph)
        {
            bool[,] mapCopy = (bool[,])map.Clone();
            int[,] costCopy = (int[,])costGraph.Clone();
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (CarveHallway(path[i], path[i + 1], mapCopy, costCopy) == false)
                    return false;
            }

            UpdateOriginalsFromCopy(map, mapCopy);
            UpdateOriginalsFromCopy(costGraph, costCopy);
            return true;
        }

        private bool CarveHallway(Room room1, Room room2, bool[,] map, int[,] costGraph, bool forceCarve = false)
        {
            AStar aStar = new AStar();
            int goalX = room2.Region.X + (room2.Region.Width / 2);
            int goalY = room2.Region.Y + (room2.Region.Height / 2);
            int startX = room1.Region.X + (room1.Region.Width / 2);
            int startY = room1.Region.Y + (room1.Region.Height / 2);
            var path = aStar.GetPath(startX, startY, goalX, goalY, costGraph);

            foreach (var (x, y) in path)
                if (OverlapsWithEmpty(x, y, 2, 2, map, room1.Region, room2.Region) && !forceCarve)
                    return false;
            foreach (var (x, y) in path)
                CarveSquare(x, y, 2, 2, map, costGraph);
            return true;
        }

        private void UpdateOriginalsFromCopy<T>(T[,] original, T[,] copy)
        {
            for (int i = 0; i < original.GetLength(0); i++)
            {
                for (int j = 0; j < original.GetLength(1); j++)
                {
                    original[i, j] = copy[i, j];
                }
            }
        }

        private bool OverlapsWithEmpty(int cX, int cY, int width, int height, bool[,] map, Rectangle room1, Rectangle room2)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!map[cX + x, cY + y] && !room1.Contains(cX + x, cY + y) && !room2.Contains(cX + x, cY + y))
                        return true;
                }
            }
            return false;
        }

        private void CarveSquare(int cX, int cY, int width, int height, bool[,] map, int[,] costGraph)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[cX + x, cY + y] = false;
                    costGraph[x, y] = int.MaxValue;
                }
            }
        }

        private int[,] BuildCostGraph(bool[,] map, List<Room> rooms)
        {
            int[,] costGraph = new int[map.GetLength(0), map.GetLength(1)];
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    costGraph[x, y] =
                        IsTileInRoom(rooms, x, y) ? 0 :
                        IsTileAroundRoom(rooms, x, y, map) ? 1 :
                        map[x, y] ? 1 :
                        int.MaxValue;
                }
            }
            return costGraph;
        }

        private bool IsTileAroundRoom(List<Room> rooms, int x, int y, bool[,] map)
        {
            foreach (var room in rooms)
            {
                if (map[x, y])
                {
                    Rectangle expReg = room.Region;
                    expReg.X--;
                    expReg.Y--;
                    expReg.Width += 2;
                    expReg.Height += 2;
                    if (expReg.Contains(x, y))
                        return true;
                }
            }
            return false;
        }

        private bool IsTileInRoom(List<Room> rooms, int x, int y)
        {
            foreach (var room in rooms)
                if (room.Region.Contains(x, y))
                    return true;
            return false;
        }

        private void BuildLinks(List<Room> rooms)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Rectangle region = rooms[i].Region;

                if (rooms[i].Links == null)
                    rooms[i].Links = new List<int>();
                // TL Corner
                rooms[i].Links.Add(RaycastToRoom(rooms, region.X - 1, region.Y, -1, 0));
                rooms[i].Links.Add(RaycastToRoom(rooms, region.X, region.Y - 1, 0, -1));
                // TR Corner
                rooms[i].Links.Add(RaycastToRoom(rooms, region.X + region.Width, region.Y, 1, 0));
                rooms[i].Links.Add(RaycastToRoom(rooms, region.X + region.Width - 1, region.Y - 1, 0, -1));
                // BL Corner
                rooms[i].Links.Add(RaycastToRoom(rooms, region.X - 1, region.Y + region.Height - 1, -1, 0));
                rooms[i].Links.Add(RaycastToRoom(rooms, region.X, region.Y + region.Height, 0, 1));
                // BR Corner
                rooms[i].Links.Add(RaycastToRoom(rooms, region.X + region.Width, region.Y + region.Height - 1, 1, 0));
                rooms[i].Links.Add(RaycastToRoom(rooms, region.X + region.Width - 1, region.Y + region.Height, 0, 1));

                rooms[i].Links = CleanLinks(rooms[i].Links, i);

                for (int j = 0; j < rooms[i].Links.Count; j++)
                {
                    int linkedRoom = rooms[i].Links[j];
                    if (rooms[linkedRoom].Links == null)
                        rooms[linkedRoom].Links = new List<int>();
                    rooms[linkedRoom].Links.Add(i);
                }
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                rooms[i].Links = CleanLinks(rooms[i].Links, i);
            }
        }

        private List<int> CleanLinks(List<int> links, int roomId)
        {
            List<int> output = new List<int>();

            for (int i = 0; i < links.Count; i++) 
            {
                if (roomId == links[i])
                    Console.WriteLine("\nSelf referential link ^^;;;\n");
                if (!output.Contains(links[i]) && links[i] != -1)
                    output.Add(links[i]);
            }

            return output;
        }

        private int RaycastToRoom(List<Room> rooms, int sX, int sY, int dX, int dY)
        {
            int eX = sX + (dX * 1000);
            int eY = sY + (dY * 1000);
            int output = -1;
            int bestDist = -1;

            bool finishedOuter = true;
            bool finishedInner = true;

            for (int x = sX; finishedOuter; x += dX)
            {
                for (int y = sY; finishedInner; y += dY)
                {
                    int distance = Math.Abs(x - sX) + Math.Abs(y - sY);
                    if (distance < bestDist || bestDist == -1)
                    {
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            Rectangle region = rooms[i].Region;
                            if (region.Contains(x, y))
                            {
                                bestDist = distance;
                                output = i;
                            }
                        }
                    }
                    finishedInner = y != eY;
                }
                finishedInner = true;
                finishedOuter = x != eX;
            }
            return output;
        }

        private List<Room> GenerateRooms(bool[,] map) 
        {
            List<Room> rooms = new List<Room>();
            var points = GetPoints(map);
            while (points.Count > 0)
            {
                if (points.Count == 0)
                    break;
                var spawnPoint = points[_rand.Next(0, points.Count)];
                int width = _rand.Next(7, 13);
                int height = _rand.Next(7, 13);
                if (CreateRoom(spawnPoint.x, spawnPoint.y, width, height, map, points, out Room room))
                    rooms.Add(room);
            }
            return rooms;
        }

        private bool CreateRoom(int sX, int sY, int width, int height, bool[,] map, List<(int x, int y)> points, out Room room)
        {
            room = new Room();
            for (int x = (sX - width) - 3; x <= sX + width + 3; x++)
                for (int y = (sY - height) - 3; y <= sY + height + 3; y++)
                    if (map[x < 0 ? 0 : x > map.GetLength(0) ? map.GetLength(0)-1 : x, y < 0 ? 0 : y >= map.GetLength(1) ? map.GetLength(1) - 1 : y] == false)
                    {
                        RemoveAt(sX, sY, points);
                        return false;
                    }
            room.Region = new Rectangle(sX - width, sY - height, (width*2)+1, (height*2)+1);
            for (int x = sX - width; x <= sX + width; x++)
                for (int y = sY - height; y <= sY + height; y++)
                    map[x, y] = false;
            return true;
        }

        private void RemoveAt(int x, int y, List<(int x, int y)> points)
        {
            for (int i = 0; i < points.Count; i++)
                if (points[i].x == x && points[i].y == y)
                {
                    points.RemoveAt(i);
                    i--;
                }
        }

        private List<(int x, int y)> GetPoints(bool[,] map)
        {
            float width = map.GetLength(0);
            float height = map.GetLength(1);

            var output = new List<(int x, int y)>();
            for (int i = 0; i < 500; i++)
            {
                double a = _rand.NextDouble();
                double b = _rand.NextDouble();
                if (b < a)
                {
                    double swap = a;
                    a = b;
                    b = swap;
                }
                var result = (b * Math.Cos(2 * Math.PI * a / b), b * Math.Sin(2 * Math.PI * a / b));
                int x = (int)((result.Item1 * (width / 2f)) + (width / 2f));
                int y = (int)((result.Item2 * (height / 2f)) + (height / 2f));

                if (GetEmptyInRange(x, y, 2, map) == 0)
                    output.Add((x, y));
            }
            return output;
        }

        private int GetEmptyInRange(int x, int y, int range, bool[,] map)
        {
            int startX = x - range;
            int startY = y - range;
            int endX = x + range;
            int endY = y + range;

            int notWalls = 0;

            // Loop from top left corner of scrop to bottomo right corner
            for (int cX = startX; cX <= endX; cX++)
                for (int cY = startY; cY <= endY; cY++)
                    if (!IsWall(cX, cY, map)) { notWalls++; }
            return notWalls;
        }

        private bool IsWall(int x, int y, bool[,] map)
        {
            if (x < 0 || x >= map.GetLength(0))
                return false;
            if (y < 0 || y >= map.GetLength(1))
                return false;
            return map[x, y];
        }
    }
}
