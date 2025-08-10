using System.Collections.Generic;
using UnityEngine;

namespace Code.Grid
{
    public interface IPathFinder
    {
        List<HexTile> FindPath(Vector3Int start, Vector3Int goal);
    }
}