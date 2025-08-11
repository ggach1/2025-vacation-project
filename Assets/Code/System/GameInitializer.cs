using Code.Astar;
using Code.Entities.Units;
using UnityEngine;

namespace Code.System
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] UnitMover unitMover;
        [SerializeField] GizmoPathVisualizer visualizer;

        private void Start()
        {
            var pathFinder = new AstarPathFinder();
            unitMover.Initialize(pathFinder, visualizer);

            // 예시: 특정 타일로 이동
            unitMover.MoveTo(new Vector3Int(5, -5, 0));
        }
    }
}