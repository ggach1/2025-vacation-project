using Code.Astar;
using Code.Grid;
using UnityEngine;

namespace Code.Entities.Units
{
    [RequireComponent(typeof(UnitMover))]
    public class UnitController : MonoBehaviour
    {
        [SerializeField] GizmoPathVisualizer visualizer;

        UnitMover _mover;
        IPathFinder _pathFinder;

        private void Awake()
        {
            _mover = GetComponent<UnitMover>();
            _pathFinder = new AstarPathFinder();
            _mover.Initialize(_pathFinder, visualizer);
        }

        // 외부에서 호출: 적 Transform을 넘기면 컨트롤러가 좌표로 변환해서 이동을 지시한다
        public void MoveToEnemyTransform(Transform enemyTransform)
        {
            if (enemyTransform == null) return;
            Vector3Int goalCoord = HexGridManager.Instance.WorldToCube(enemyTransform.position);
            _mover.MoveTo(goalCoord);
        }

        public void MoveToTileCoord(Vector3Int tileCoord)
        {
            _mover.MoveTo(tileCoord);
        }
    }
}