using UnityEngine;
using System.Collections.Generic;

public class ChunkLoader : MonoBehaviour
{
    public Camera cameraComponent;
    public int additionalChunks = 1; // Add this line
    private Vector2Int currentChunkPosition;
    private HashSet<Vector2Int> loadedChunks = new HashSet<Vector2Int>();
    private float previousCameraSize;
    private float previousCameraAspect;

    void Start()
    {
        previousCameraSize = cameraComponent.orthographicSize;
        previousCameraAspect = cameraComponent.aspect;
        LoadVisibleChunks();
    }

    void Update()
    {
        Vector2Int newChunkPosition = new Vector2Int(
            Mathf.FloorToInt(cameraComponent.transform.position.x / 16),
            Mathf.FloorToInt(cameraComponent.transform.position.y / 16)
        );

        if (
            newChunkPosition != currentChunkPosition
            || cameraComponent.orthographicSize != previousCameraSize
            || cameraComponent.aspect != previousCameraAspect
        )
        {
            currentChunkPosition = newChunkPosition;
            previousCameraSize = cameraComponent.orthographicSize;
            previousCameraAspect = cameraComponent.aspect;
            LoadVisibleChunks();
        }
    }

    private void LoadVisibleChunks()
    {
        int visibleChunksX =
            Mathf.CeilToInt(
                cameraComponent.orthographicSize * cameraComponent.aspect / 16 // Use the assigned camera's properties
            ) + additionalChunks; // Add additionalChunks here
        int visibleChunksY =
            Mathf.CeilToInt(cameraComponent.orthographicSize / 16) + additionalChunks; // And here

        HashSet<Vector2Int> newChunks = new HashSet<Vector2Int>();

        for (int x = -visibleChunksX; x <= visibleChunksX; x++)
        {
            for (int y = -visibleChunksY; y <= visibleChunksY; y++)
            {
                Vector2Int chunkPosition = currentChunkPosition + new Vector2Int(x, y);
                newChunks.Add(chunkPosition);

                if (!loadedChunks.Contains(chunkPosition))
                {
                    ChunkManager.Instance.LoadChunk(chunkPosition);
                }
            }
        }

        foreach (Vector2Int chunkPosition in loadedChunks)
        {
            if (!newChunks.Contains(chunkPosition))
            {
                ChunkManager.Instance.UnloadChunk(chunkPosition);
            }
        }

        loadedChunks = newChunks;
    }
}
