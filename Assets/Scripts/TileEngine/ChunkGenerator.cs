using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Layer
{
    public GameObject NoiseObjectContainer; // GameObject that contains FastNoiseSIMDUnity components
    public ResourceDatabase resourceDatabase; // ResourceDatabase for this layer
}

public class ChunkGenerator : MonoBehaviour
{
    public Layer[] Layers; // Define your layers here
    public TileBase defaultTile; // The tile to use if no matching threshold is found

    public Chunk GenerateChunk(int chunkX, int chunkY, int layerIndex)
    {
        Chunk chunk = new Chunk();
        // Assign layerIndex to chunk.Layer
        chunk.Layer = layerIndex;
        // Get the resource database and noise object container for this layer
        ResourceDatabase resourceDatabase = Layers[layerIndex].resourceDatabase;
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
                TileBase tileBase = resourceDatabase.GetTileForNoise(layerIndex, averageNoise);

                // If no matching tile was found, use the default tile
                if (tileBase == null)
                {
                    tileBase = defaultTile;
                }

                // Create a new tile with this TileBase
                chunk.SetTile(x, y, tileBase);
            }
        }

        return chunk;
    }

    public Resource GetResourceForPoint(int worldX, int worldY, int layerIndex)
    {
        // Get the resource database and noise object container for this layer
        ResourceDatabase resourceDatabase = Layers[layerIndex].resourceDatabase;
        GameObject noiseObjectContainer = Layers[layerIndex].NoiseObjectContainer;

        // Get all FastNoiseSIMDUnity components from the GameObject
        FastNoiseSIMDUnity[] noiseObjects =
            noiseObjectContainer.GetComponents<FastNoiseSIMDUnity>();

        // Get Perlin noise value at this position from each noiseObject and average them
        float totalNoise = 0f;
        foreach (var noiseObject in noiseObjects)
        {
            float[] noiseSet = noiseObject.fastNoiseSIMD.GetNoiseSet(worldX, worldY, 0, 1, 1, 1);
            totalNoise += noiseSet[0];
        }
        float averageNoise = totalNoise / noiseObjects.Length;

        // Get the resource based on the average noise value
        Resource resource = resourceDatabase.GetResourceForNoise(layerIndex, averageNoise);

        return resource;
    }
}
