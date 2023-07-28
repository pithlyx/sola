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

    public void SetTile(int x, int y, TileBase tileBase, bool random, float noise)
    {
        TileData tileData;
        tileData.tileBase = tileBase;

        // If the "random" rotation should be applied, set the transform matrix
        if (random)
        {
            // Scale and round the noise value to get a rotation index from 0 to 3
            int rotationIndex = Mathf.RoundToInt(noise * 3);

            float rotationDegrees = rotationIndex * -90f;
            tileData.transformMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotationDegrees));
        }
        else
        {
            tileData.transformMatrix = Matrix4x4.identity;
        }

        Tiles[x, y] = tileData;
    }
}
