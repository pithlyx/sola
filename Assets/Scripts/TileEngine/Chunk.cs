using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk
{
    public Tile[,] Tiles = new Tile[16, 16];
    public int Layer;

    public void DisplayOnTilemap(Tilemap[] tilemaps, Vector2Int position)
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                tilemaps[Layer].SetTile(
                    new Vector3Int(position.x + x, position.y + y, 0),
                    Tiles[x, y]?.TileBase
                );
            }
        }
    }
}
