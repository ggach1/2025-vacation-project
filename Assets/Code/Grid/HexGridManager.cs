using System.Collections.Generic;
using UnityEngine;

namespace Code.Grid
{
    public class HexGridManager : MonoBehaviour
    {
        public static HexGridManager Instance { get; private set; }

        public Dictionary<Vector3Int, HexTile> tiles = new();

        float _radius;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else Instance = this;
        }

        public void SetRadius(float radius)
        {
            _radius = radius;
        }

        public void RegisterTile(HexTile tile)
        {
            tiles[tile.CubeCoord] = tile;
        }

        public HexTile GetTileAt(Vector3Int coord)
        {
            tiles.TryGetValue(coord, out var tile);
            Debug.Log($"tile : {(tile == null ? "NULL" : tile)}");
            return tile;
        }

        /// <summary>
        /// 월드 좌표를 큐브 좌표로 변환
        /// </summary>
        public Vector3Int WorldToCube(Vector3 worldPos)
        {
            if (_radius <= 0)
            {
                Debug.LogError("HexGridManager: Radius not set! Call SetRadius before using WorldToCube.");
                return Vector3Int.zero;
            }

            Vector3 gridOrigin = HexGridManager.Instance.transform.position;
            Vector3 pos = worldPos - gridOrigin;

            // Axial 좌표를 계산하고
            float q = (Mathf.Sqrt(3f) / 3f * worldPos.x - 1f / 3f * worldPos.z);
            float r = (2f / 3f * worldPos.z);

            // Axial 좌표를 큐브 좌표로 변환해준다
            float x = q;
            float z = r;
            float y = -x - z;

            // 가장 가까운 큐브 좌표를 반올림 해줘서
            float rx = Mathf.RoundToInt(x);
            float ry = Mathf.RoundToInt(y);
            float rz = Mathf.RoundToInt(z);

            // 반올림 오차를 보정해준다
            float xDiff = Mathf.Abs(rx - x);
            float yDiff = Mathf.Abs(ry - y);
            float zDiff = Mathf.Abs(rz - z);

            if (xDiff > yDiff && xDiff > zDiff)
                rx = -ry - rz;
            else if (yDiff > zDiff)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new Vector3Int((int)rx, (int)ry, (int)rz);
        }

        //private Vector3Int AxialToCube(Vector2 axial)
        //{
        //    int x = Mathf.RoundToInt(axial.x);
        //    int z = Mathf.RoundToInt(axial.y);
        //    int y = -x - z;
        //    return new Vector3Int(x, y, z);
        //}

        //private Vector3Int CubeRound(Vector3 cube)
        //{
        //    int rx = Mathf.RoundToInt(cube.x);
        //    int ry = Mathf.RoundToInt(cube.y);
        //    int rz = Mathf.RoundToInt(cube.z);

        //    float xDiff = Mathf.Abs(rx - cube.x);
        //    float yDiff = Mathf.Abs(ry - cube.y);
        //    float zDiff = Mathf.Abs(rz - cube.z);

        //    if (xDiff > yDiff && xDiff > zDiff)
        //        rx = -ry - rz;
        //    else if (yDiff > zDiff)
        //        ry = -rx - rz;
        //    else
        //        rz = -rx - ry;

        //    return new Vector3Int(rx, ry, rz);
        //}
    }
}

