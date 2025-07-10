using Code.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Astar
{
    public class AstarPathFinder
    {
        static readonly Vector3Int[] Directions = new[]
        {
            new Vector3Int(1, -1, 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(0, -1, 1)
        };

        public List<HexTile> FindPath(Vector3Int start, Vector3Int goal)
        {
            var open = new PriorityQueue<HexTile>();
            var cameFrom = new Dictionary<HexTile, HexTile>();
            var gScore = new Dictionary<HexTile, int>();

            var startTile = HexGridManager.Instance.GetTileAt(start);
            var goalTile = HexGridManager.Instance.GetTileAt(goal);

            gScore[startTile] = 0;
            open.Enqueue(startTile, Heuristic(start, goal));

            while (open.Count > 0)
            {
                var current = open.Dequeue();

                if (current == goalTile)
                    return ReconstructPath(cameFrom, current);

                foreach (var dir in Directions)
                {
                    var neighborCoord = current.CubeCoord + dir;
                    var neighbor = HexGridManager.Instance.GetTileAt(neighborCoord);
                    if (neighbor == null || !neighbor.IsWalkable) continue;

                    int tentaiveG = gScore[current] + 1;
                    if (!gScore.ContainsKey(neighbor) || tentaiveG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentaiveG;
                        if (!open.Contains(neighbor))
                        {
                            int fScore = tentaiveG + Heuristic(neighbor.CubeCoord, goal);
                            open.Enqueue(neighbor, fScore);
                        }
                    }
                }
            }

            return null;
        }

        private int Heuristic(Vector3Int a, Vector3Int b)
        {
            return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
        }

        private List<HexTile> ReconstructPath(Dictionary<HexTile, HexTile> cameFrom, HexTile current)
        {
            var path = new List<HexTile> { current};
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, current);
            }
            return path;
        }
    }
}

