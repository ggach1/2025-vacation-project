using UnityEngine;

namespace Code.Grid
{
    /// <summary>
    /// 테스트 용
    /// </summary>
    public class HexTileSpawner : MonoBehaviour
    {
        [SerializeField] Transform originField;

        [SerializeField] Vector2 fieldSize = new Vector2(8f, 6f);
        int _rows = 4;
        int _cols = 9;

        private void Start()
        {
            BoxCollider collider = originField.GetComponent<BoxCollider>();
            Vector3 fieldSize3D = Vector3.Scale(collider.size, originField.lossyScale);
            Vector2 fieldSize = new Vector2(fieldSize3D.x, fieldSize3D.z);

            float maxRadiusByWidth = (fieldSize.x / _cols) / Mathf.Sqrt(3f);
            float maxRadiusByHeight = (fieldSize.y / _rows) / 1.5f;

            float radius = Mathf.Min(maxRadiusByWidth, maxRadiusByHeight) * 0.95f;

            float xStep = radius * Mathf.Sqrt(3f);
            float zStep = radius * 1.5f;

            float totalWidth = (_cols - 1) * xStep + xStep;
            if (_rows > 1)
                totalWidth += xStep / 2f; // 오른쪽 오프셋을 고려해서 해주어야 밖으로 안 삐져나감

            float totalHeight = (_rows - 1) * zStep + zStep;

            Vector3 originGridOffset = originField.position - new Vector3(totalWidth, 0, totalHeight) * 0.5f;

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    float xOffset = (row % 2 == 0) ? 0f : xStep / 2f;

                    float xPos = col * xStep + xOffset;
                    float zPos = row * zStep;

                    Vector3 position = new Vector3(xPos, 0.01f, zPos) + originGridOffset;

                    GameObject tileObj = new GameObject($"HexTile {row} {col}");
                    tileObj.transform.position = position;
                    tileObj.transform.SetParent(originField);

                    var renderer = tileObj.AddComponent<HexTileRenderer>();
                    renderer.Initialize(radius);

                    Vector3Int cubeCoord = OffsetToCube(col, row);

                    var tiles = tileObj.AddComponent<HexTile>();
                    tiles.CubeCoord = cubeCoord;
                    tiles.IsWalkable = true;

                    HexGridManager.Instance.RegisterTile(tiles);
                }
            }
        }

        private Vector3Int OffsetToCube(int col, int row)
        {
            int x = col - (row - (row & 1)) / 2;
            int z = row;
            int y = -x - z;
            return new Vector3Int(x, y, z);
        }
    }
}

