using UnityEngine;

namespace Code.Grid
{
    /// <summary>
    /// 테스트 용
    /// </summary>
    public class HexTileSpawner : MonoBehaviour
    {
        [SerializeField] Transform originField;

        [SerializeField] float radius = 0.5f;
        int _rows = 4;
        int _cols = 9;

        private void Start()
        {
            float xStep = radius * Mathf.Sqrt(3f); // 가로
            float zStep = radius * 1.5f; // 세로

            for (int x = 0; x < _rows; x++)
            {
                for (int y = 0; y < _cols; y++)
                {
                    // 홀수 행은 x 방형으로 반 칸 이동
                    float xOffset = (x % 2 == 0) ? 0f : xStep / 2f;

                    float xPos = y * xStep + xOffset;
                    float zPos = x * zStep;

                    Vector3 position = new Vector3(xPos, 0, zPos) + originField.position;

                    GameObject tile = new GameObject($"HexTile {x} {y}");
                    tile.transform.position = position;

                    var renderer = tile.AddComponent<HexTileRenderer>();
                    renderer.Initialize(radius);

                    Vector3Int cubeCoord = OffsetToCube(x, y);

                    var tiles = tile.AddComponent<HexTile>();
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

