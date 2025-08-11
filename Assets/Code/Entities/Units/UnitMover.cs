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
            Debug.Log($"UnitMover initialized in {gameObject.name} with finder={finder?.GetType().Name}, visualizer={(visualizer == null ? "null" : visualizer.GetType().Name)}");
        }

        private void Start()
        {
            var startCoord = HexGridManager.Instance.WorldToCube(transform.position);
            var startTile = HexGridManager.Instance.GetTileAt(startCoord);

            Debug.Log($"startCoord : {(startCoord == null ? "NULL" : startCoord)}");
            Debug.Log($"startTile : {(startTile == null ? "NULL" : startTile)}");

            if (startTile != null)
            {
                transform.position = startTile.transform.position;
                Debug.Log($"¿Ø¥÷ Ω√¿€ ¡¬«• Ω∫≥¿: {startCoord}");
            }
            else
            {
                Debug.LogWarning($"¿Ø¥÷ Ω√¿€ ¡¬«•({startCoord})ø° «ÿ¥Á«œ¥¬ ≈∏¿œ¿Ã æ¯¿Ω!");
            }
        }

        public void MoveTo(Vector3Int targetCoord)
        {
            var startCoord = HexGridManager.Instance.WorldToCube(transform.position);
            var path = _pathFinder.FindPath(startCoord, targetCoord); // Null
            Debug.Log($"MoveTo: start={startCoord}, target={targetCoord}, path={(path == null ? "NULL" : path.Count.ToString())}");

            if (path != null)
            {
                _pathVisualizer?.DrawPath(path);
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

