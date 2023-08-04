using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Resource defaultResource;
    public Camera mainCamera;
    public int chunkSize = 16;
    public int generationRadius = 2;

    private Vector3Int currentCameraChunk;
    private Vector3Int lastCameraChunk;
    private Dictionary<Vector3Int, Chunk> loadedChunks = new Dictionary<Vector3Int, Chunk>();

    private void Start()
    {
        UpdateCameraChunk();
        LoadChunksInRadius();
    }

    private void Update()
    {
        UpdateCameraChunk();

        if (currentCameraChunk != lastCameraChunk)
        {
            LoadChunksInRadius();
            UnloadChunksOutsideRadius();
            lastCameraChunk = currentCameraChunk;
        }
    }

    private void UpdateCameraChunk()
    {
        currentCameraChunk = new Vector3Int(
            Mathf.FloorToInt(mainCamera.transform.position.x / chunkSize),
            Mathf.FloorToInt(mainCamera.transform.position.y / chunkSize),
            0
        );
    }

    private Chunk GenerateChunkData(Vector3Int chunkPosition)
    {
        Resource[,] resources = new Resource[chunkSize, chunkSize];

        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                resources[x, y] = defaultResource; // Replace this with your resource generation logic
            }
        }

        return new Chunk(chunkSize, chunkPosition, resources);
    }

    private void LoadChunksInRadius()
    {
        for (int yOffset = -generationRadius; yOffset <= generationRadius; yOffset++)
        {
            for (int xOffset = -generationRadius; xOffset <= generationRadius; xOffset++)
            {
                Vector3Int chunkPosition = currentCameraChunk + new Vector3Int(xOffset, yOffset, 0);
                if (!loadedChunks.ContainsKey(chunkPosition))
                {
                    LoadChunk(chunkPosition);
                }
            }
        }
    }

    private void UnloadChunksOutsideRadius()
    {
        List<Vector3Int> chunksToUnload = new List<Vector3Int>();

        foreach (Vector3Int chunkPosition in loadedChunks.Keys)
        {
            if (
                Mathf.Abs(chunkPosition.x - currentCameraChunk.x) > generationRadius
                || Mathf.Abs(chunkPosition.y - currentCameraChunk.y) > generationRadius
            )
            {
                chunksToUnload.Add(chunkPosition);
            }
        }

        foreach (Vector3Int chunkPosition in chunksToUnload)
        {
            UnloadChunk(chunkPosition);
        }
    }

    private void LoadChunk(Vector3Int chunkPosition)
    {
        if (!loadedChunks.ContainsKey(chunkPosition))
        {
            Chunk chunk = GenerateChunkData(chunkPosition);
            loadedChunks[chunkPosition] = chunk;
        }

        Chunk loadedChunk = loadedChunks[chunkPosition];

        for (int localY = 0; localY < chunkSize; localY++)
        {
            for (int localX = 0; localX < chunkSize; localX++)
            {
                Vector3Int tilePosition = loadedChunk.GetTilePosition(localX, localY);
                Resource resource = loadedChunk.GetResourceAt(localX, localY);
                tilemap.SetTile(tilePosition, resource.tile);
            }
        }
    }

    private void UnloadChunk(Vector3Int chunkPosition)
    {
        if (loadedChunks.ContainsKey(chunkPosition))
        {
            Chunk loadedChunk = loadedChunks[chunkPosition];

            for (int localY = 0; localY < chunkSize; localY++)
            {
                for (int localX = 0; localX < chunkSize; localX++)
                {
                    Vector3Int tilePosition = loadedChunk.GetTilePosition(localX, localY);
                    tilemap.SetTile(tilePosition, null);
                }
            }

            loadedChunks.Remove(chunkPosition);
        }
    }
}
