using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class LayerGenerator : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the tilemap
    public Camera mainCamera; // The main camera
    public Vector3Int currentChunkPosition; // The current chunk position
    public Vector3Int lastChunkPosition; // The previous chunk position
    public ResourceDatabase resourceDatabase; // ResourceDatabase
    public List<GameObject> NoiseGenerators; // List of noise generators used for each layers noise
    public int layerIndex; // The index of the displayed layer

    [Header("Layer Settings")]
    public int chunkSize; // The size of the chunks
    public TileBase defaultTile; // The tile to use if no matching threshold is found
    private Dictionary<Vector3Int, Chunk> loadedChunks = new Dictionary<Vector3Int, Chunk>();

    private void Awake()
    {
        // Get the main camera
        mainCamera = Camera.main;
        // Get the parent tilemap
        tilemap = GetComponent<Tilemap>();
    }

    public void Update()
    {
        // Get the chunk position of the camera
        currentChunkPosition = Vector3Int.FloorToInt(mainCamera.transform.position / chunkSize);
        if (currentChunkPosition == lastChunkPosition)
        {
            return;
        }
        //generate chunks that intersect the cameras viewport and arent in the loaded chunks dictionary
        for (int y = currentChunkPosition.y - 1; y <= currentChunkPosition.y + 1; y++)
        {
            for (int x = currentChunkPosition.x - 1; x <= currentChunkPosition.x + 1; x++)
            {
                Vector3Int chunkPosition = new Vector3Int(x, y, 0);
                if (!loadedChunks.ContainsKey(chunkPosition))
                {
                    loadedChunks[chunkPosition] = GenerateChunk(chunkPosition);
                }
            }
        }

        // Get the chunk position of the camera
        lastChunkPosition = currentChunkPosition;
    }

    public Chunk GenerateChunk(Vector3Int chunkPosition)
    {
        // get the noise for the current chunk
        float[] noise = GetNoiseForRegion(
            NoiseGenerators[layerIndex],
            chunkPosition * chunkSize,
            chunkSize,
            0
        );
        Debug.Log(noise);
        int[] resourceIndices = resourceDatabase.MapResourcesToNoise(noise, layerIndex);
        Debug.Log(resourceIndices);
        List<Resource> resources = resourceDatabase.GetResourcesFromIndexes(resourceIndices);
        DisplayChunk(resources, currentChunkPosition * chunkSize);
        return new Chunk(); // Need to return a Chunk object here
    }

    public float[] GetNoiseForRegion(
        GameObject noiseGenerator,
        Vector3Int start,
        int size,
        int Depth = 0
    )
    {
        FastNoiseSIMDUnity noiseComponent = noiseGenerator.GetComponent<FastNoiseSIMDUnity>();

        // Save the current settings of the noise component
        noiseComponent.SaveSettings();

        // Use the FastNoiseSIMD object of the noise component to fill a noise set
        float[] noiseSet = noiseComponent.fastNoiseSIMD.GetEmptyNoiseSet(size, size, Depth);
        noiseComponent.fastNoiseSIMD.FillNoiseSet(
            noiseSet,
            start.x,
            start.y,
            0,
            start.x + size,
            start.y + size,
            Depth
        );

        return noiseSet;
    }

    public void DisplayChunk(List<Resource> resources, Vector3 chunkPosition)
    {
        // Convert the chunk position to a Vector3Int, which is used by the tilemap
        Vector3Int chunkPositionInt = Vector3Int.FloorToInt(chunkPosition);

        // Loop over all tiles in the chunk
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                // Calculate the index of the resource for this tile
                int index = y * chunkSize + x;

                // Check if the index is within the range of the resources list
                if (index >= 0 && index < resources.Count)
                {
                    // Get the position of the tile in the tilemap
                    Vector3Int tilePosition = chunkPositionInt + new Vector3Int(x, y, 0);

                    // Set the tile in the tilemap
                    tilemap.SetTile(tilePosition, resources[index].tile);
                }
            }
        }
    }
}
