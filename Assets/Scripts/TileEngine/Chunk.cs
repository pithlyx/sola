using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk
{
    public struct TileData
    {
        public TileBase tileBase;
        public Matrix4x4 transformMatrix;
    }

    public TileData[,] Tiles = new TileData[16, 16];
    public int Layer;

    public void DisplayOnTilemap(Tilemap[] tilemaps, Vector2Int position)
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                Vector3Int tilemapPosition = new Vector3Int(position.x + x, position.y + y, 0);
                tilemaps[Layer].SetTile(tilemapPosition, Tiles[x, y].tileBase);
                tilemaps[Layer].SetTransformMatrix(tilemapPosition, Tiles[x, y].transformMatrix);
            }
        }
    }

    public void SetTile(int x, int y, TileBase tileBase)
    {
        TileData tileData;
        tileData.tileBase = tileBase;
        tileData.transformMatrix = Matrix4x4.identity;

        Tiles[x, y] = tileData;
    }
}
