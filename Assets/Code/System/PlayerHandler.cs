using Code.Entities.Units;
using Code.Grid;
using UnityEngine;

namespace Code.System
{
    public class PlayerHandler : MonoBehaviour, ISystem
    {
        [field: SerializeField] public InputSO InputSO { get; private set; }

        [SerializeField] LayerMask unitLayer;
        [SerializeField] LayerMask fieldLayer;

        GameObject _selectedUnit;
        UnitController _selectedUnitController;

        private void OnEnable()
        {
            InputSO.LeftClickPressed += LeftClicked;
            InputSO.RightClickPressed += RightClicked;
        }
        private void OnDisable()
        {
            InputSO.LeftClickPressed -= LeftClicked;
            InputSO.RightClickPressed -= RightClicked;
        }

        private void LeftClicked()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 1) 유닛 선택
            if (Physics.Raycast(ray, out var hit, 100f, unitLayer))
            {
                _selectedUnit = hit.collider.gameObject;
                _selectedUnitController = _selectedUnit.GetComponent<UnitController>();
                Debug.Log($"Unit Selected : {_selectedUnit.name}");
                return;
            }

            // 2) 선택된 유닛이 있고, 필드를 클릭 → 좌표 구해 이동
            if (_selectedUnitController != null && Physics.Raycast(ray, out hit, 100f, fieldLayer))
            {
                var coord = HexGridManager.Instance.WorldToCube(hit.point);
                _selectedUnitController.MoveToTileCoord(coord);
            }
        }

        private void RightClicked()
        {
            // 추후 UI/정보 표시 로직
        }
    }
}

