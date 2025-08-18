using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Grid
{
    [Serializable]
    public class HexTileData
    {
        public Vector3Int coord;   // 큐브 좌표
        public Vector3 worldPos;   // 유닛이 서야 할 월드 위치
        public bool walkable;

        public HexTileData(Vector3Int coord, Vector3 worldPos, bool walkable = true)
        {
            this.coord = coord;
            this.worldPos = worldPos;
            this.walkable = walkable;
        }

        public override string ToString() => $"HexTileData({coord}, {worldPos}, walkable={walkable})";
    }

    public class HexGridManager : MonoBehaviour
    {
        public static HexGridManager Instance { get; private set; }

        public Dictionary<Vector3Int, HexTileData> tiles = new();

        [Header("Field & Tile Settings")]
        [SerializeField] Transform field;
        [SerializeField] float radius = 1.066f;
        [SerializeField] Transform tileParent;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            GenerateTilesFromField();
        }

        /// <summary>Field(Plane) 크기 기반으로 가상 타일 좌표 생성</summary>
        private void GenerateTilesFromField()
        {
            tiles.Clear();
            if (field == null) return;

            // Unity Plane 기본 크기 10,10
            float fieldWidth = field.localScale.x * 10f;
            float fieldHeight = field.localScale.z * 10f;
            Vector3 origin = field.position;

            float hexWidth = Mathf.Sqrt(3f) * radius;
            float hexHeight = 2f * radius;

            int cols = Mathf.CeilToInt(fieldWidth / hexWidth);
            int rows = Mathf.CeilToInt(fieldHeight / (hexHeight * 0.75f)); // 세로 1/4 겹침

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    float xOffset = (r % 2 == 0) ? 0f : hexWidth / 2f;
                    float x = origin.x + c * hexWidth + xOffset - fieldWidth / 2f;
                    float z = origin.z + r * (hexHeight * 0.75f) - fieldHeight / 2f;

                    Vector3 worldPos = new Vector3(x, origin.y, z);
                    Vector3Int cube = WorldToCube(worldPos);

                    if (!tiles.ContainsKey(cube))
                        tiles.Add(cube, new HexTileData(cube, worldPos, true));
                }
            }

            Debug.Log($"[HexGridManager] 타일 {tiles.Count}개 생성");
        }

        public HexTileData GetTileAt(Vector3Int coord)
        {
            tiles.TryGetValue(coord, out var tile);
            return tile;
        }

        /// <summary>월드 좌표 → 큐브 좌표</summary>
        public Vector3Int WorldToCube(Vector3 worldPos)
        {
            float q = (Mathf.Sqrt(3f) / 3f * worldPos.x - 1f / 3f * worldPos.z) / radius;
            float r = (2f / 3f * worldPos.z) / radius;

            float x = q, z = r, y = -x - z;

            int rx = Mathf.RoundToInt(x);
            int ry = Mathf.RoundToInt(y);
            int rz = Mathf.RoundToInt(z);

            float xDiff = Mathf.Abs(rx - x);
            float yDiff = Mathf.Abs(ry - y);
            float zDiff = Mathf.Abs(rz - z);

            if (xDiff > yDiff && xDiff > zDiff) rx = -ry - rz;
            else if (yDiff > zDiff) ry = -rx - rz;
            else rz = -rx - ry;

            return new Vector3Int(rx, ry, rz);
        }
    }
}

