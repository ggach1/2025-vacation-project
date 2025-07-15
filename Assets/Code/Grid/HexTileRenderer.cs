using UnityEngine;

namespace Code.Grid
{
    /// <summary>
    /// 실제 Unity GameObj에 육각형 Mesh를 작용하는 컴포넌트
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class HexTileRenderer : MonoBehaviour, IRenderer
    {
        public void Initialize(float radius)
        {
            LineRenderer renderer = GetComponent<LineRenderer>();

            Vector3[] positions = new Vector3[7];

            // 6개의 꼭짓점
            for (int i = 0; i < 6; i++)
            {
                float angleDeg = 60 * i - 30; // flat-top 육각형 기준 시작 각도: -30도
                float angleRad = Mathf.Deg2Rad * angleDeg;

                float x = Mathf.Cos(angleRad) * radius;
                float z = Mathf.Sin(angleRad) * radius;

                positions[i] = new Vector3(x, 0, z);
            }

            positions[6] = positions[0]; // 루프를 닫기 위한 첫 번째 점 반복

            renderer.positionCount = positions.Length;
            renderer.useWorldSpace = false; // 로컬 좌표계 사용
            renderer.loop = true; // 선을 닫음
            renderer.SetPositions(positions);

            renderer.widthMultiplier = 0.05f;

            renderer.material = new Material(Shader.Find("Sprites/Default"))
            {
                color = Color.white
            };
        }
    }
}

