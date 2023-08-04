using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainEngine : MonoBehaviour
{
    public Tilemap tilemap;
    public Resource defaultResource;
    public Camera mainCamera;
    public int chunkSize = 16;
    public int chunkPadding = 2;

    public KeyCode debugKey = KeyCode.F1;
    public KeyCode CycleLayerKey = KeyCode.V;

    private Vector3Int currentCameraChunk;
    private Vector3Int lastCameraChunk;
    private Dictionary<string, Vector3> lastViewportCorners;
    private Dictionary<string, Vector3> currentViewportCorners;

    private Dictionary<Vector3Int, Chunk> loadedChunks = new Dictionary<Vector3Int, Chunk>();

    public LayerName currentLayer = LayerName.Terrain;

    private bool isDebugging = false;

    private void Start()
    {
        UpdateCameraInfo();
        lastCameraChunk = currentCameraChunk;
        lastViewportCorners = currentViewportCorners;
        UpdateChunks();
    }

    private void Update()
    {
        UpdateCameraInfo();

        if (currentCameraChunk != lastCameraChunk || HasViewportMovedChunks())
        {
            UpdateChunks();
        }

        if (Input.GetKeyDown(debugKey))
        {
            isDebugging = !isDebugging;
        }
        if (Input.GetKeyDown(CycleLayerKey))
        {
            CycleLayer();
        }
    }

    private void Log(string message)
    {
        if (isDebugging)
            Debug.Log(message);
    }

    public void CycleLayer()
    {
        currentLayer = (LayerName)(
            ((int)currentLayer + 1) % LayerName.GetNames(typeof(LayerName)).Length
        );
        loadedChunks.Clear();
        UpdateChunks();
    }

    private bool HasViewportMovedChunks()
    {
        foreach (var corner in currentViewportCorners)
        {
            Vector3Int currentChunk = GetChunkForPoint(corner.Value);
            Vector3Int lastChunk = GetChunkForPoint(lastViewportCorners[corner.Key]);
            if (currentChunk != lastChunk)
            {
                return true;
            }
        }
        return false;
    }

    // Camera stuff
    public Dictionary<string, Vector3> GetViewportCorners()
    {
        Dictionary<string, Vector3> corners = new Dictionary<string, Vector3>();
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        float leftEdge = mainCamera.transform.position.x - (camWidth / 2f);
        float rightEdge = mainCamera.transform.position.x + (camWidth / 2f);
        float bottomEdge = mainCamera.transform.position.y - (camHeight / 2f);
        float topEdge = mainCamera.transform.position.y + (camHeight / 2f);

        corners["TopLeft"] = new Vector3(leftEdge, topEdge, mainCamera.transform.position.z);
        corners["TopRight"] = new Vector3(rightEdge, topEdge, mainCamera.transform.position.z);
        corners["BottomLeft"] = new Vector3(leftEdge, bottomEdge, mainCamera.transform.position.z);
        corners["BottomRight"] = new Vector3(
            rightEdge,
            bottomEdge,
            mainCamera.transform.position.z
        );

        return corners;
    }

    public List<Vector3Int> GetChunksIntersecting()
    {
        Dictionary<string, Vector3> corners = currentViewportCorners;

        // Get the chunk for each corner of the viewport
        Vector3Int topLeftChunk = GetChunkForPoint(corners["TopLeft"]);
        Vector3Int bottomRightChunk = GetChunkForPoint(corners["BottomRight"]);

        List<Vector3Int> chunks = new List<Vector3Int>();

        // Generate all chunks in between, with padding
        for (int x = topLeftChunk.x - chunkPadding; x <= bottomRightChunk.x + chunkPadding; x++)
        {
            for (int y = bottomRightChunk.y - chunkPadding; y <= topLeftChunk.y + chunkPadding; y++)
            {
                chunks.Add(new Vector3Int(x, y, 0));
            }
        }

        return chunks;
    }

    private Vector3Int GetChunkForPoint(Vector3 point)
    {
        return new Vector3Int(
            Mathf.FloorToInt(point.x / chunkSize),
            Mathf.FloorToInt(point.y / chunkSize),
            0
        );
    }

    private Vector3Int GetOriginForChunk(Vector3Int chunkPosition)
    {
        return new Vector3Int(chunkPosition.x * chunkSize, chunkPosition.y * chunkSize, 0);
    }

    private void UpdateCameraInfo()
    {
        currentCameraChunk = new Vector3Int(
            Mathf.FloorToInt(mainCamera.transform.position.x / chunkSize),
            Mathf.FloorToInt(mainCamera.transform.position.y / chunkSize),
            0
        );
        currentViewportCorners = GetViewportCorners();
    }

    private List<Vector3Int> GetChunksToLoad(HashSet<Vector3Int> intersectingChunks)
    {
        List<Vector3Int> chunksToLoad = new List<Vector3Int>();

        foreach (Vector3Int chunkPosition in intersectingChunks)
        {
            if (!loadedChunks.ContainsKey(chunkPosition))
            {
                chunksToLoad.Add(chunkPosition);
            }
        }

        return chunksToLoad;
    }

    private List<Vector3Int> GetChunksToUnload(HashSet<Vector3Int> intersectingChunks)
    {
        List<Vector3Int> chunksToUnload = new List<Vector3Int>();

        foreach (Vector3Int chunkPosition in loadedChunks.Keys)
        {
            if (!intersectingChunks.Contains(chunkPosition))
            {
                chunksToUnload.Add(chunkPosition);
            }
        }

        return chunksToUnload;
    }

    private void UpdateChunks()
    {
        HashSet<Vector3Int> intersectingChunksHashSet = new HashSet<Vector3Int>(
            GetChunksIntersecting()
        );
        List<Vector3Int> intersectingChunksList = new List<Vector3Int>(intersectingChunksHashSet);

        Log($"Intersecting Chunks:\n{string.Join(",", intersectingChunksList)}");
        List<Vector3Int> newChunks = GetChunksToLoad(intersectingChunksHashSet);
        Log($"New Chunks:\n{string.Join(",", newChunks)}");
        List<Vector3Int> chunksToUnload = GetChunksToUnload(intersectingChunksHashSet);
        Log($"Chunks to unload:\n{string.Join(",", chunksToUnload)}");
        LoadChunks(newChunks);
        UnloadChunks(chunksToUnload);

        lastCameraChunk = currentCameraChunk;
        lastViewportCorners = currentViewportCorners;
    }

    private Chunk GenerateChunkData(Vector3Int chunkPosition)
    {
        // get the noise set for the chunk
        FastNoiseSIMDUnity noiseGenerator = ResourceDatabase.Instance.GetGenerator(currentLayer);
        List<float> noiseSet = noiseGenerator.GetNoiseSet(
            chunkPosition.x * chunkSize,
            chunkPosition.y * chunkSize,
            0,
            chunkSize,
            chunkSize,
            1
        );

        List<Resource> resourceList = ResourceDatabase.Instance.NoiseToResources(
            noiseSet,
            currentLayer
        );

        // Convert the list of resources to a 2D array
        Resource[,] resourceArray = new Resource[chunkSize, chunkSize];
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                resourceArray[i, j] = resourceList[i * chunkSize + j];
            }
        }

        return new Chunk(chunkSize, chunkPosition, resourceArray);
    }

    private void LoadChunks(List<Vector3Int> chunksToLoad)
    {
        foreach (Vector3Int chunkPosition in chunksToLoad)
        {
            LoadChunk(chunkPosition);
        }
    }

    private void UnloadChunks(List<Vector3Int> chunksToUnload)
    {
        foreach (Vector3Int chunkPosition in chunksToUnload)
        {
            UnloadChunk(chunkPosition);
        }
    }

    private void LoadChunk(Vector3Int chunkPosition)
    {
        if (!loadedChunks.TryGetValue(chunkPosition, out Chunk loadedChunk))
        {
            Chunk chunk = GenerateChunkData(chunkPosition);
            loadedChunks[chunkPosition] = chunk;
            loadedChunk = chunk;
        }

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
        if (loadedChunks.TryGetValue(chunkPosition, out Chunk loadedChunk))
        {
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
