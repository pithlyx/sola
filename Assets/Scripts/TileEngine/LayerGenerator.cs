using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[System.Serializable]
public struct NoiseGenerator
{
    public FastNoiseSIMDUnity noiseObject; // The FastNoiseSIMDUnity of the noise generator
    public LayerName layer; // The layer this noise generator is associated with

    public NoiseGenerator(FastNoiseSIMDUnity noiseObject, LayerName layer)
    {
        this.noiseObject = noiseObject;
        this.layer = layer;
    }
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
    public int renderDistance = 2; // The distance in chunks to generate around the players viewport

    [Header("Layer Information")]
    public LayerName currentlayer; // The name of the layer to generate
    public Vector3Int currentChunkPosition; // The current chunk position
    public Vector3Int lastChunkPosition; // The previous chunk position
    private Dictionary<Vector3Int, Chunk> loadedChunks = new Dictionary<Vector3Int, Chunk>(); // Dictionary of loaded chunks

    private void Start()
    {
        // Initialize the terrain when the game starts
        InitializeTerrain();
    }

    private void Update()
    {
        // Update the terrain as needed when the game updates
        UpdateTerrain();
    }

    private Chunk GenerateChunk(Vector3Int chunkPosition)
    {
        // This method should generate a chunk at the given position
        // It might involve generating noise for each tile in the chunk and converting that noise to resource indices using the ResourceDatabase
    }

    private List<Vector3Int> Get CunksInRange(Vector3Int chunkPosition, int range)
    {
        // This method should return a list of chunk positions within the given range of the given chunk position
    }
}
