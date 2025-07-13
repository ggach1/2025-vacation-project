using UnityEngine;

namespace Code.System
{
    public class PlayerHandler : MonoBehaviour
    {
        [field:SerializeField] public InputSO InputSO { get; private set; }

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
            // 유닛 관련 로직 구현
        }

        private void RightClicked()
        {
            // 정보 UI 관련 구현
            // 우클릭한 애의 정보를 받아와서
            // UI에 어떤 애의 정보를 띄울지 결정
        }
    }
}

