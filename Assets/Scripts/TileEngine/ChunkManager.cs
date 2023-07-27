using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance;

    public Tilemap[] Tilemaps; // Now an array
    public ChunkGenerator ChunkGenerator;

    public int currentLayer = 0;

    private Dictionary<Vector2Int, Dictionary<int, Chunk>> loadedChunks =
        new Dictionary<Vector2Int, Dictionary<int, Chunk>>();

    // Property to access loadedChunks
    public Dictionary<Vector2Int, Dictionary<int, Chunk>> LoadedChunks
    {
        get { return loadedChunks; }
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            // Hide the current layer
            Tilemaps[currentLayer].gameObject.SetActive(false);

            // Increment currentLayer, wrapping around to 0 if necessary
            currentLayer = (currentLayer + 1) % Tilemaps.Length;

            // Show the next layer
            Tilemaps[currentLayer].gameObject.SetActive(true);
        }
    }

    public void LoadChunk(Vector2Int chunkPosition)
    {
        if (!loadedChunks.ContainsKey(chunkPosition))
        {
            loadedChunks[chunkPosition] = new Dictionary<int, Chunk>();
        }

        for (int i = 0; i < Tilemaps.Length; i++)
        {
            if (loadedChunks[chunkPosition].ContainsKey(i))
                continue;

            Chunk chunk = ChunkGenerator.GenerateChunk(chunkPosition.x, chunkPosition.y, i); // Pass layer index
            loadedChunks[chunkPosition][i] = chunk;

            chunk.DisplayOnTilemap(Tilemaps, chunkPosition * 16); // Use corresponding tilemap
        }
    }

    public void UnloadChunk(Vector2Int chunkPosition)
    {
        if (!loadedChunks.ContainsKey(chunkPosition))
            return;

        // Optionally clear the tiles from the Tilemap
        for (int i = 0; i < Tilemaps.Length; i++)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    Tilemaps[i].SetTile(
                        new Vector3Int(chunkPosition.x * 16 + x, chunkPosition.y * 16 + y, 0),
                        null
                    );
                }
            }
        }

        loadedChunks.Remove(chunkPosition);
    }
}
