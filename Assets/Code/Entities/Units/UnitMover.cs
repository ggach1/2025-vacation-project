using Code.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Entities.Units
{
    public class UnitMover : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 2f;
        IPathFinder pathFinder;
        IPathVisualizer pathVisualizer;

        public void Initialize(IPathFinder finder, IPathVisualizer visualizer)
        {
            pathFinder = finder;
            pathVisualizer = visualizer;
        }

        public void MoveTo(Vector3Int targetCoord)
        {
            var startCoord = HexGridManager.Instance.WorldToCube(transform.position);
            var path = pathFinder.FindPath(startCoord, targetCoord);
            if (path != null)
            {
                pathVisualizer?.DrawPath(path);
                StopAllCoroutines();
                StartCoroutine(FollowPath(path));
            }
        }

        private IEnumerator FollowPath(List<HexTile> path)
        {
            foreach (var tile in path)
            {
                Vector3 targetPos = tile.transform.position;
                while (Vector3.Distance(transform.position, targetPos) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
        }
    }
}

