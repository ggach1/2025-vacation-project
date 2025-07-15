using UnityEngine;

namespace Code.Grid
{
    /// <summary>
    /// ���� Unity GameObj�� ������ Mesh�� �ۿ��ϴ� ������Ʈ
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class HexTileRenderer : MonoBehaviour, IRenderer
    {
        public void Initialize(float radius)
        {
            LineRenderer renderer = GetComponent<LineRenderer>();

            Vector3[] positions = new Vector3[7];

            // 6���� ������
            for (int i = 0; i < 6; i++)
            {
                float angleDeg = 60 * i - 30; // flat-top ������ ���� ���� ����: -30��
                float angleRad = Mathf.Deg2Rad * angleDeg;

                float x = Mathf.Cos(angleRad) * radius;
                float z = Mathf.Sin(angleRad) * radius;

                positions[i] = new Vector3(x, 0, z);
            }

            positions[6] = positions[0]; // ������ �ݱ� ���� ù ��° �� �ݺ�

            renderer.positionCount = positions.Length;
            renderer.useWorldSpace = false; // ���� ��ǥ�� ���
            renderer.loop = true; // ���� ����
            renderer.SetPositions(positions);

            renderer.widthMultiplier = 0.05f;

            renderer.material = new Material(Shader.Find("Sprites/Default"))
            {
                color = Color.white
            };
        }
    }
}

