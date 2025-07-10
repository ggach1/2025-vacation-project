using UnityEngine;

namespace Code.Grid
{
    /// <summary>
    /// ������ Ÿ�� �޽��� �����ϴ� Ŭ����
    /// </summary>
    public static class HexMeshGenerator
    {
        /// <summary>
        /// ������ �߽��� �������� �� ������(r) �޽��� ������
        /// </summary>
        public static Mesh GenerateHexMesh(float radius)
        {
            Mesh mesh = new Mesh();

            // �߽��� + 6���� ������
            Vector3[] vertices = new Vector3[7];
            int[] triangles = new int[18];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < 6; i++)
            {
                float angleDeg = 60 * i - 30; // flat-top ���� ���� ����: -30��
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

