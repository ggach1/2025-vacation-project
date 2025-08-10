using System.Collections.Generic;
using UnityEngine;

namespace Code.Grid
{
    public class HexGridManager : MonoBehaviour
    {
        public static HexGridManager Instance { get; private set; }

        public Dictionary<Vector3Int, HexTile> tiles = new();
        public float Radius { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else Instance = this;
        }

        public void RegisterTile(HexTile tile)
        {
            tiles[tile.CubeCoord] = tile;
        }

        public HexTile GetTileAt(Vector3Int coord)
        {
            tiles.TryGetValue(coord, out var tile);
            return tile;
        }

        public void SetRadius(float radius)
        {
            Radius = radius;
        }

        public Vector3Int WorldToCube(Vector3 worldPos)
        {
            float q = (Mathf.Sqrt(3f) / 3f * worldPos.x - 1f / 3f * worldPos.z) / Radius;
            float r = (2f / 3f * worldPos.z) / Radius;
            return CubeRound(q, r);
        }

        private Vector3Int CubeRound(float q, float r)
        {
            float x = q;
            float z = r;
            float y = -x - z;

            int rx = Mathf.RoundToInt(x);
            int ry = Mathf.RoundToInt(y);
            int rz = Mathf.RoundToInt(z);

            float xDiff = Mathf.Abs(rx - x);
            float yDiff = Mathf.Abs(ry - y);
            float zDiff = Mathf.Abs(rz - z);

            if (xDiff > yDiff && xDiff > zDiff)
                rx = -ry - rz;
            else if (yDiff > zDiff)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new Vector3Int(rx, ry, rz);
        }
    }
}

