using System.Collections.Generic;
using UnityEngine;

namespace Code.Grid
{
    public class HexGridManager : MonoBehaviour
    {
        public static HexGridManager Instance { get; private set; }

        public Dictionary<Vector3Int, HexTile> tiles = new();

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
    }
}

