using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GigglyLib.ProcGen
{
    public class Room
    {
        public Rectangle Region;
        public List<int> Links;
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
            CarveHallways(rooms, map);

            Console.WriteLine($"RoomGenerator finished with {rooms.Count} rooms generated");
            return rooms;
        }

        private void CarveHallways(List<Room> rooms, bool[,] map)
        {
            int[,] costGraph = BuildCostGraph(map, rooms);
            for (int i = 0; i < rooms.Count; i++)
            {
                var room = rooms[i];
                foreach (var idx in room.Links)
                {
                    rooms[idx].Links.Remove(i);
                    CarveHallway(room, rooms[idx], map, costGraph);
                }
            }
        }

        private void CarveHallway(Room room1, Room room2, bool[,] map, int[,] costGraph)
        {
            AStar aStar = new AStar();
            int goalX = room2.Region.X + (room2.Region.Width / 2);
            int goalY = room2.Region.Y + (room2.Region.Height / 2);
            int startX = room1.Region.X + (room1.Region.Width / 2);
            int startY = room1.Region.Y + (room1.Region.Height / 2);
            var path = aStar.GetPath(startX, startY, goalX, goalY, costGraph);
            foreach (var (x, y) in path)
            {
                CarveSquare(x, y, 2, 2, map);
                costGraph[x, y] = 0;
            }
        }

        private void CarveSquare(int cX, int cY, int width, int height, bool[,] map)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[cX + x, cY + y] = false;
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
                        IsTileAroundRoom(rooms, x, y, map) ? 4 :
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