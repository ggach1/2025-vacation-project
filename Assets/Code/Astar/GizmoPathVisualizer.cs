using Code.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Astar
{
    public class GizmoPathVisualizer : MonoBehaviour, IPathVisualizer
    {
        List<HexTile> _paths;

        public void DrawPath(List<HexTile> path)
        {
            _paths = path;
        }

        private void OnDrawGizmos()
        {
            if (_paths == null || _paths.Count == 0)
                return;

            Gizmos.color = Color.yellow;

            for (int i = 0; i < _paths.Count; i++)
            {
                Gizmos.DrawSphere(_paths[i].transform.position + Vector3.up * 0.05f, 0.1f);

                if (i < _paths.Count - 1)
                {
                    Gizmos.DrawLine(_paths[i].transform.position + Vector3.up * 0.05f,
                                    _paths[i + 1].transform.position + Vector3.up * 0.05f);
                }
            }
        }
    }
}