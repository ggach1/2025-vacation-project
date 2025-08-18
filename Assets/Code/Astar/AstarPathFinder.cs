using Code.Grid;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Code.Astar
{
    public class AstarPathFinder : IPathFinder
    {
        static readonly Vector3Int[] Directions =
        {
            new Vector3Int(1,-1,0),  new Vector3Int(1,0,-1),  new Vector3Int(0,1,-1),
            new Vector3Int(-1,1,0),  new Vector3Int(-1,0,1),  new Vector3Int(0,-1,1)
        };

        public List<HexTileData> FindPath(Vector3Int start, Vector3Int goal)
        {
            var grid = HexGridManager.Instance;

            var open = new PriorityQueue<HexTileData>();
            var cameFrom = new Dictionary<HexTileData, HexTileData>();
            var gScore = new Dictionary<HexTileData, int>();

            var startTile = grid.GetTileAt(start);
            var goalTile = grid.GetTileAt(goal);
            if (startTile == null || goalTile == null) return null;

            gScore[startTile] = 0;
            open.Enqueue(startTile, Heuristic(start, goal));

            while (open.Count > 0)
            {
                var current = open.Dequeue();
                if (current == goalTile)
                    return ReconstructPath(cameFrom, current);

                foreach (var dir in Directions)
                {
                    var nCoord = current.coord + dir;
                    var neighbor = grid.GetTileAt(nCoord);
                    if (neighbor == null || !neighbor.walkable) continue;

                    int tentative = gScore[current] + 1;
                    if (!gScore.ContainsKey(neighbor) || tentative < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentative;

                        int f = tentative + Heuristic(neighbor.coord, goal);
                        open.Enqueue(neighbor, f); // 이미 들어있어도 그냥 다시 Enqueue
                    }
                }
            }
            return null;
        }

        private int Heuristic(Vector3Int a, Vector3Int b) => Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));

        private List<HexTileData> ReconstructPath(Dictionary<HexTileData, HexTileData> cameFrom, HexTileData current)
        {
            var path = new List<HexTileData> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, current);
            }
            return path;
        }
    }
}

