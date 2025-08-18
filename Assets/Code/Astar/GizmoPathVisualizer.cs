using Code.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Astar
{
    public class GizmoPathVisualizer : MonoBehaviour, IPathVisualizer
    {
        List<Vector3> _points;

        public void DrawPath(List<HexTileData> path)
        {
            _points = new List<Vector3>(path.Count);
            foreach (var t in path)
                _points.Add(t.worldPos + Vector3.up * 0.05f);
        }

        private void OnDrawGizmos()
        {
            if (_points == null || _points.Count == 0) return;
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _points.Count; i++)
            {
                Gizmos.DrawSphere(_points[i], 0.07f);
                if (i < _points.Count - 1) Gizmos.DrawLine(_points[i], _points[i + 1]);
            }
        }
    }
}