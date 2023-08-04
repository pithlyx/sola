using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;

public class Chunk
{
    public Vector3Int origin; // The origin of the chunk in world coordinates
    public int chunkSize; // The size of the chunk
    public Resource[,] resources; // 2D array of the resources in each tile

    public Chunk(int size, Vector3Int chunkOrigin, Resource[,] resourceArray)
    {
        this.chunkSize = size; // Set the size of the chunk
        this.origin = chunkOrigin; // Set the origin of the chunk in world coordinates
        this.resources = resourceArray; // Set the resources of the chunk
    }

    public Vector3Int GetTilePosition(int localX, int localY)
    {
        // Convert the local position within the chunk to a global position
        return new Vector3Int(localX + origin.x, localY + origin.y, origin.z);
    }

    public Resource GetResourceAt(int x, int y)
    {
        // Return the resource at the given local coordinates within the chunk
        return resources[x, y];
    }
}
