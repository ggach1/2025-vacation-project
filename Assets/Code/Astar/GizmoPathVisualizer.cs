using Code.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Astar
{
    public class GizmoPathVisualizer : MonoBehaviour, IPathVisualizer
    {
        List<HexTile> currentPath;

        public void DrawPath(List<HexTile> path)
        {
            currentPath = path;
        }

        private void OnDrawGizmos()
        {
            if (currentPath == null || currentPath.Count == 0) return;

            Gizmos.color = Color.red;
            for (int i = 0; i < currentPath.Count; i++)
            {
                Gizmos.DrawSphere(currentPath[i].transform.position + Vector3.up * 0.1f, 0.05f);
                if (i < currentPath.Count - 1)
                    Gizmos.DrawLine(currentPath[i].transform.position, currentPath[i + 1].transform.position);
            }
        }
    }
}