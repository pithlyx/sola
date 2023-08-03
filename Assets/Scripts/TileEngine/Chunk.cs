using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;

public class Chunk
{
    public Vector3Int origin; // The origin of the chunk
    public int chunkSize; // The size of the chunk
    public List<int> resourceIndices; // Indices of the resources in each tile

    public Chunk(int size, Vector3Int chunkOrigin, List<int> indices)
    {
        this.chunkSize = size; // Set the size of the chunk
        this.origin = chunkOrigin; // Set the origin of the chunk
        this.resourceIndices = indices; // Set the tiles of the chunk
    }
}
