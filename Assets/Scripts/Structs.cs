using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum LayerName // LayerGenerator.cs
{
    // LayerNames for the LayerGenerator
    Terrain,
    Ore,
}

[System.Serializable]
public enum CombineMethod // LayerGenerator.cs
{
    // Basic methods for combining noise maps
    Add,
    Subtract,
    Multiply,
    Divide,
    Max,
    Min,
    Average
}

public struct TileData // Chunk.cs
{
    // Struct for storing tile data
    public TileBase tileBase;
    public Matrix4x4 transformMatrix;
}
