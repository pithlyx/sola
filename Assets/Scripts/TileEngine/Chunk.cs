using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Chunk
{
    public Vector3Int chunkOrigin; // The origin of the chunk
    public int chunkSize; // The size of the chunk
    public int[,] tiles; // Indices of the resources in each tile

    public Chunk(int size, Vector3Int origin)
    {
        this.chunkSize = size; // Set the size of the chunk
        this.chunkOrigin = origin; // Set the origin of the chunk
        tiles = new int[chunkSize, chunkSize]; // Initialize the tiles array
    }
}
