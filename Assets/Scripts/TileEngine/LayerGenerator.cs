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
    public int chunkPadding = 2; // The distance in chunks to generate around the players viewport

    [Header("Generator Info")]
    public Vector3Int currentCameraPosition; // The current camera position
    public Vector3Int lastCameraPosition; // The previous camera position
    public Vector3Int currentCameraChunk; // The current chunk position
    public Vector3Int lastCameraChunk; // The previous chunk position
    public LayerName currentlayer; // The name of the layer to generate

    private Dictionary<Vector3Int, Chunk> loadedChunks = new Dictionary<Vector3Int, Chunk>(); // Dictionary of loaded chunks

    // private void Start()
    // {
    //     UpdatePositionalValues();
    //     // Initialize the terrain when the game starts
    //     InitializeTerrain();
    // }

    // private void Update()
    // {
    //     UpdatePositionalValues();

    //     if (currentCameraChunk != lastCameraChunk)
    //     {
    //         // Get a list of chunks to generate
    //         List<Vector3Int> chunkOrigins = GetChunksToGenerate();

    //         // Create a list of origins from the loaded chunks that aren't in the new chunk origins
    //         List<Vector3Int> chunksToUnload = loadedChunks.Keys.Except(chunkOrigins).ToList();

    //         // Unload the chunks that are no longer in the view (plus padding)
    //         foreach (Vector3Int origin in chunksToUnload)
    //         {
    //             loadedChunks.Remove(origin);
    //         }

    //         // Create a list of origins from the new chunk origins that aren't in the loaded chunks
    //         List<Vector3Int> chunksToLoad = chunkOrigins.Except(loadedChunks.Keys).ToList();

    //         // Load all new chunks that aren't already loaded
    //         foreach (Vector3Int origin in chunksToLoad)
    //         {
    //             // Generate the chunk at the given origin
    //             Chunk newChunk = GenerateChunk(origin, currentlayer);

    //             // Add the chunk to the dictionary of loaded chunks
    //             loadedChunks[origin] = newChunk;

    //             // Display the chunk
    //             DisplayChunk(newChunk);
    //         }
    //     }

    //     lastCameraChunk = currentCameraChunk;
    // }

    // public void UpdatePositionalValues()
    // {
    //     currentCameraPosition = new Vector3Int(
    //         Mathf.FloorToInt(mainCamera.transform.position.x),
    //         Mathf.FloorToInt(mainCamera.transform.position.y),
    //         Mathf.FloorToInt(mainCamera.transform.position.z)
    //     );
    //     currentCameraChunk = RoundToChunkSize(currentCameraPosition);
    // }

    // private void InitializeTerrain()
    // {
    //     lastCameraPosition = currentCameraPosition;
    //     lastCameraChunk = currentCameraChunk;

    //     // Get a list of chunks to generate
    //     List<Vector3Int> chunkOrigins = GetChunksToGenerate();
    //     Debug.Log(
    //         "Chunk Origins:\nCount: "
    //             + chunkOrigins.Count
    //             + "\n"
    //             + string.Join("\n", chunkOrigins.Select(c => c.ToString()).ToArray())
    //     );
    //     foreach (Vector3Int origin in chunkOrigins)
    //     {
    //         // Generate the chunk at the given origin
    //         Chunk newChunk = GenerateChunk(origin, currentlayer);
    //         // Add the chunk to the dictionary of loaded chunks
    //         loadedChunks[origin] = newChunk;
    //         DisplayChunk(newChunk);
    //     }
    // }

    // public List<Vector3Int> GetChunksToGenerate()
    // {
    //     List<Vector3Int> chunkOrigins = new List<Vector3Int>();

    //     Vector3Int bottomLeftChunk = RoundToChunkSize(
    //         mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0))
    //     );
    //     Vector3Int topRightChunk = RoundToChunkSize(
    //         mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0))
    //     );

    //     // Add padding
    //     bottomLeftChunk = bottomLeftChunk - (Vector3Int.one * chunkPadding * chunkSize);
    //     topRightChunk = topRightChunk + (Vector3Int.one * chunkPadding * chunkSize);

    //     for (int y = bottomLeftChunk.y; y <= topRightChunk.y; y += chunkSize)
    //     {
    //         for (int x = bottomLeftChunk.x; x <= topRightChunk.x; x += chunkSize)
    //         {
    //             chunkOrigins.Add(new Vector3Int(x, y, 0));
    //         }
    //     }

    //     return chunkOrigins;
    // }

    // public Vector3Int RoundToChunkSize(Vector3 point)
    // {
    //     return new Vector3Int(
    //         Mathf.FloorToInt(point.x / chunkSize),
    //         Mathf.FloorToInt(point.y / chunkSize),
    //         Mathf.FloorToInt(point.z / chunkSize)
    //     );
    // }

    // private Chunk GenerateChunk(Vector3Int chunkPosition, LayerName layer)
    // {
    //     // Get the noise generator associated with the given layer
    //     NoiseGenerator? noiseGenerator = noiseGenerators.FirstOrDefault(ng => ng.layer == layer);

    //     // If there is no noise generator for the given layer, output a debug message and return null
    //     if (noiseGenerator == null)
    //     {
    //         Debug.LogError($"No noise generator found for layer {layer}");
    //         return null;
    //     }
    //     // Generate the noise for the chunk
    //     List<float> noiseSet = resourceDatabase.GetNoise(
    //         noiseGenerator.Value.generator,
    //         chunkPosition,
    //         chunkSize
    //     );

    //     // Convert the noise to resource indices
    //     List<int> resourceIndices = resourceDatabase.NoiseToIndices(noiseSet, layer);

    //     // Create a new Chunk at the given position with the given resource indices
    //     Chunk newChunk = new Chunk(chunkSize, chunkPosition, resourceIndices);
    //     return newChunk;
    // }

    // public void DisplayChunk(Chunk chunk)
    // {
    //     // Get the resources for the tile indices
    //     List<Resource> resources = resourceDatabase.IndicesToResources(chunk.resourceIndices);

    //     for (int localY = 0; localY < chunkSize; localY++)
    //     {
    //         for (int localX = 0; localX < chunkSize; localX++)
    //         {
    //             // Calculate the index within the chunk
    //             int index = localX * chunkSize + localY;

    //             // Get the resource at the current tile
    //             Resource resource = resources[index];

    //             // If the resource is null, skip this tile
    //             if (resource == null)
    //                 continue;

    //             // Get the tile for the current resource
    //             TileBase tile = resource.tile;

    //             // Get the position of the current tile in world coordinates
    //             Vector3Int tilePosition = chunk.GetTilePosition(localX, localY);

    //             // Set the tile at the current tile position
    //             tilemap.SetTile(tilePosition, tile);
    //         }
    //     }
    // }
}
