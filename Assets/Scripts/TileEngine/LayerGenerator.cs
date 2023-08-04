using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[System.Serializable]
public struct NoiseGenerator
{
    public FastNoiseSIMDUnity generator; // The FastNoiseSIMDUnity of the noise generator
    public LayerName layer; // The layer this noise generator is associated with
}

public class LayerGenerator : MonoBehaviour
{
    [Header("Layer Generator References")]
    public Tilemap tilemap; // Reference to the tilemap
    public Camera mainCamera; // The main camera (used for getting the "position" of the user)
    public ResourceDatabase resourceDatabase; // ResourceDatabase to use for generating layers

    public List<NoiseGenerator> noiseGenerators; // List of noise generators used for each layers noise

    [Header("Generator Settings")]
    public int chunkSize = 16; // The size of the chunks
    public TileBase defaultTile; // The tile to use if no matching threshold is found
    public int renderPadding = 2; // The distance in chunks to generate around the players viewport

    [Header("Layer Information")]
    public LayerName currentlayer; // The name of the layer to generate
    public Vector3Int currentChunkPosition; // The current chunk position
    public Vector3Int lastChunkPosition; // The previous chunk position
    private Dictionary<Vector3Int, Chunk> loadedChunks = new Dictionary<Vector3Int, Chunk>(); // Dictionary of loaded chunks

    private void Start()
    {
        // Initialize the terrain when the game starts
        InitializeTerrain(Vector3Int.zero);
    }

    private void Update()
    {
        // Update the terrain as needed when the game updates
        // UpdateTerrain();
    }

    private void InitializeTerrain(Vector3Int chunkPosition)
    {
        // Initialize the current chunk position and camera transform
        currentChunkPosition = chunkPosition;
        lastChunkPosition = chunkPosition;
        mainCamera.transform.position = new Vector3(
            chunkPosition.x * chunkSize,
            chunkPosition.y * chunkSize,
            mainCamera.transform.position.z
        );
        // Get a list of chunks to generate
        List<Vector3Int> chunkOrigins = GetChunksToGenerate();
        Debug.Log($"Generating {chunkOrigins.Count} chunks");
        Debug.Log($"Chunk origins: {string.Join(", ", chunkOrigins)}");
        foreach (Vector3Int origin in chunkOrigins)
        {
            // Generate the chunk at the given origin
            Chunk newChunk = GenerateChunk(origin, currentlayer);
            // Add the chunk to the dictionary of loaded chunks
            loadedChunks[origin] = newChunk;
            DisplayChunk(newChunk);
        }
    }

    public List<Vector3Int> GetChunksToGenerate()
    {
        // Initialize list of chunks to generate
        List<Vector3Int> chunksToGenerate = new List<Vector3Int>();

        // Determine the camera's viewport in world coordinates
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(
            new Vector3(0, 0, mainCamera.nearClipPlane)
        );
        Vector3 topRight = mainCamera.ViewportToWorldPoint(
            new Vector3(1, 1, mainCamera.nearClipPlane)
        );

        // Get the current camera position in terms of chunks
        Vector3Int cameraChunk = new Vector3Int(
            Mathf.FloorToInt(bottomLeft.x / chunkSize),
            Mathf.FloorToInt(bottomLeft.y / chunkSize),
            0 // Z coordinate is always 0
        );

        // Determine the range of chunks to generate around the camera
        Vector3Int minChunk = cameraChunk - Vector3Int.one * renderPadding;
        Vector3Int maxChunk = cameraChunk + Vector3Int.one * renderPadding;

        // Generate all chunks within the renderPadding distance from the camera's chunk
        for (int y = minChunk.y; y <= maxChunk.y; y++)
        {
            for (int x = minChunk.x; x <= maxChunk.x; x++)
            {
                chunksToGenerate.Add(new Vector3Int(x * chunkSize, y * chunkSize, 0));
            }
        }

        // Return the list of chunks to generate
        return chunksToGenerate;
    }

    private Chunk GenerateChunk(Vector3Int chunkPosition, LayerName layer)
    {
        // Get the noise generator associated with the given layer
        NoiseGenerator? noiseGenerator = noiseGenerators.FirstOrDefault(ng => ng.layer == layer);

        // If there is no noise generator for the given layer, output a debug message and return null
        if (noiseGenerator == null)
        {
            Debug.LogError($"No noise generator found for layer {layer}");
            return null;
        }
        // Generate the noise for the chunk
        List<float> noiseSet = resourceDatabase.GetNoise(
            noiseGenerator.Value.generator,
            chunkPosition,
            chunkSize
        );

        // Convert the noise to resource indices
        List<int> resourceIndices = resourceDatabase.NoiseToIndices(noiseSet, layer);

        // Create a new Chunk at the given position with the given resource indices
        Chunk newChunk = new Chunk(chunkSize, chunkPosition, resourceIndices);
        DisplayChunk(newChunk);
        return newChunk;
    }

    public void DisplayChunk(Chunk chunk)
    {
        // Get the resources for the tile indices
        List<Resource> resources = resourceDatabase.IndicesToResources(chunk.resourceIndices);

        for (int localY = 0; localY < chunkSize; localY++)
        {
            for (int localX = 0; localX < chunkSize; localX++)
            {
                // Calculate the index within the chunk
                int index = localY * chunkSize + localX;

                // Get the resource at the current tile
                Resource resource = resources[index];

                // If the resource is null, skip this tile
                if (resource == null)
                    continue;

                // Get the tile for the current resource
                TileBase tile = resource.tile;

                // Get the position of the current tile in world coordinates
                Vector3Int tilePosition = new Vector3Int(
                    chunk.origin.x + localX,
                    chunk.origin.y + localY,
                    chunk.origin.z
                );

                // Set the tile at the current tile position
                tilemap.SetTile(tilePosition, tile);
            }
        }
    }
}
