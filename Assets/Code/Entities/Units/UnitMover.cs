using Code.Astar;
using Code.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Entities.Units
{
    public class UnitMover : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 2f;

        IPathFinder _pathFinder;
        IPathVisualizer _pathVisualizer;

        public void Initialize(IPathFinder finder, IPathVisualizer visualizer)
        {
            _pathFinder = finder;
            _pathVisualizer = visualizer;
        }

        public void MoveTo(Vector3Int targetCoord)
        {
            var startCoord = HexGridManager.Instance.WorldToCube(transform.position);
            var path = _pathFinder.FindPath(startCoord, targetCoord);
            if (path == null || path.Count == 0) return;

            _pathVisualizer?.DrawPath(path);
            StopAllCoroutines();
            StartCoroutine(Follow(path));
        }

        private IEnumerator Follow(List<HexTileData> path)
        {
            foreach (var tile in path)
            {
                // 높이는 유닛 현재 y 유지
                Vector3 targetPos = new Vector3(tile.worldPos.x, transform.position.y, tile.worldPos.z);
                while ((transform.position - targetPos).sqrMagnitude > 0.0004f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
        }
    }
}

