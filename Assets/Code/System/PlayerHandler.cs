using Code.Entities.Units;
using Code.Grid;
using UnityEngine;

namespace Code.System
{
    public class PlayerHandler : MonoBehaviour, ISystem
    {
        [field: SerializeField] public InputSO InputSO { get; private set; }

        [SerializeField] LayerMask unitLayer;
        [SerializeField] LayerMask enemyLayer;
        [SerializeField] LayerMask tileLayer;

        UnitController _selectedUnitController;

        private void OnEnable()
        {
            if (InputSO == null)
            {
                Debug.LogError("Input SO 없다");
                return;
            }

            InputSO.LeftClickPressed += LeftClicked;
            InputSO.RightClickPressed += RightClicked;
        }

        private void OnDisable()
        {
            if (InputSO == null) return;
            InputSO.LeftClickPressed -= LeftClicked;
            InputSO.RightClickPressed -= RightClicked;
        }

        private void LeftClicked()
        {
            if (Camera.main == null)
            {
                Debug.LogError("카메라 없다");
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 1) 유닛 클릭 -> 선택 
            if (Physics.Raycast(ray, out RaycastHit hitUnit, 100f, unitLayer))
            {
                var ctrl = hitUnit.collider.GetComponentInParent<UnitController>();
                if (ctrl != null)
                {
                    _selectedUnitController = ctrl;
                    Debug.Log($"유닛 선택: {ctrl.gameObject.name}");
                }
                else
                {
                    Debug.LogWarning("부모에게서 UnitController를 찾을 수 없음");
                }
                return;
            }

            // 2) 선택된 유닛이 있고 타일 클릭했으면 A* 이동 명령 전달
            if (_selectedUnitController != null && Physics.Raycast(ray, out RaycastHit hitTile, 100f, tileLayer))
            {
                var tile = hitTile.collider.GetComponent<HexTile>() ?? hitTile.collider.GetComponentInParent<HexTile>();
                if (tile != null)
                {
                    _selectedUnitController.MoveToTileCoord(tile.CubeCoord);
                    Debug.Log($"유닛 이동 {tile.name} ({tile.CubeCoord})");
                }
                else
                {
                    Debug.LogWarning("HexTile Compo를 찾을 수 없다");
                }
            }
        }

        private void RightClicked()
        {
            if (_selectedUnitController == null) return;

            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitEnemy, 100f, enemyLayer))
            {
                var enemyTf = hitEnemy.collider.transform;
                _selectedUnitController.MoveToEnemyTransform(enemyTf);
                Debug.Log($"Command: Move selected unit to enemy {enemyTf.name}");
            }
        }
    }
}

