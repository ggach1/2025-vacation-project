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
            // ���� ���� ���� ����
        }

        private void RightClicked()
        {
            // ���� UI ���� ����
            // ��Ŭ���� ���� ������ �޾ƿͼ�
            // UI�� � ���� ������ ����� ����
        }
    }
}

