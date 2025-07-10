using UnityEngine;

namespace Code.Grid
{
    /// <summary>
    /// 육각형 타일 메쉬를 생성하는 클래스
    /// </summary>
    public static class HexMeshGenerator
    {
        /// <summary>
        /// 육각형 중심을 기준으로 한 반지름(r) 메쉬를 생성함
        /// </summary>
        public static Mesh GenerateHexMesh(float radius)
        {
            Mesh mesh = new Mesh();

            // 중심점 + 6개의 꼭짓점
            Vector3[] vertices = new Vector3[7];
            int[] triangles = new int[18];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < 6; i++)
            {
                float angleDeg = 60 * i - 30; // flat-top 기준 시작 각도: -30도
                float angleRad = Mathf.Deg2Rad * angleDeg;

                float x = Mathf.Cos(angleRad) * radius;
                float z = Mathf.Sin(angleRad) * radius;

                vertices[i + 1] = new Vector3(x, 0, z);
            }

            for (int i = 0; i < 6; i++)
            {
                int baseIndex = i * 3;
                triangles[baseIndex] = 0;
                triangles[baseIndex + 1] = i == 5 ? 1 : i + 2;
                triangles[baseIndex + 2] = i + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}

