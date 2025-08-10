using System.Collections.Generic;
using UnityEngine;

namespace Code.Grid
{
    public interface IPathVisualizer
    {
        void DrawPath(List<HexTile> path);
    }
}