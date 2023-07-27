using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Layer
{
    public Threshold[] Thresholds;
    public GameObject NoiseObjectContainer; // GameObject that contains FastNoiseSIMDUnity components
}

[System.Serializable]
public class Threshold
{
    public TileBase[] TileBases;
    public Vector2 MinMaxThreshold;
}

public class ChunkGenerator : MonoBehaviour
{
    public Layer[] Layers; // Define your layers here

    public TileBase GetTileBaseFromThreshold(float noiseValue, Threshold[] thresholds)
    {
        foreach (var threshold in thresholds)
        {
            if (
                noiseValue >= threshold.MinMaxThreshold.x
                && noiseValue <= threshold.MinMaxThreshold.y
            )
            {
                // Pick a random TileBase from this threshold's array of TileBases
                int index = Random.Range(0, threshold.TileBases.Length);
                return threshold.TileBases[index];
            }
        }

        return null; // Return null if no matching threshold is found
    }

    public Chunk GenerateChunk(int chunkX, int chunkY, int layerIndex)
    {
        Chunk chunk = new Chunk();
        // Assign layerIndex to chunk.Layer
        chunk.Layer = layerIndex;
        // Get the thresholds and noise object container for this layer
        Threshold[] thresholds = Layers[layerIndex].Thresholds;
        GameObject noiseObjectContainer = Layers[layerIndex].NoiseObjectContainer;

        // Get all FastNoiseSIMDUnity components from the GameObject
        FastNoiseSIMDUnity[] noiseObjects =
            noiseObjectContainer.GetComponents<FastNoiseSIMDUnity>();

        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                // Calculate world position of this tile
                int worldX = chunkX * 16 + x;
                int worldY = chunkY * 16 + y;

                // Get Perlin noise value at this position from each noiseObject and average them
                float totalNoise = 0f;
                foreach (var noiseObject in noiseObjects)
                {
                    float[] noiseSet = noiseObject.fastNoiseSIMD.GetNoiseSet(
                        worldX,
                        worldY,
                        0,
                        1,
                        1,
                        1
                    );
                    totalNoise += noiseSet[0];
                }
                float averageNoise = totalNoise / noiseObjects.Length;

                // Get a TileBase based on the average noise value
                TileBase tileBase = GetTileBaseFromThreshold(averageNoise, thresholds);

                // Create a new tile with this TileBase
                Tile tile = new Tile();
                tile.TileBase = tileBase;

                // Add the tile to the chunk
                chunk.Tiles[x, y] = tile;
            }
        }

        return chunk;
    }
}
