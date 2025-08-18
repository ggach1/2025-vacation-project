using System.Collections.Generic;
using UnityEngine;

namespace Code.Grid
{
    public interface IPathFinder
    {
        List<HexTileData> FindPath(Vector3Int start, Vector3Int goal);
    }
}