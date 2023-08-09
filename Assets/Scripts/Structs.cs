using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using System;

// ----------> LayerGenerator.cs <----------
[System.Serializable]
public enum LayerName // LayerGenerator.cs
{
    // LayerNames for the LayerGenerator
    Terrain,
    Ore
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

// ----------> Chunk.cs <----------
[System.Serializable]
public struct TileData // Chunk.cs
{
    // Struct for storing tile data
    public TileBase tileBase;
    public Matrix4x4 transformMatrix;
}

// ----------> Items <----------
[System.Serializable]
public enum ItemTag // Item.cs
{
    None,
    Crafted,
    Resource,
    Fluid,
    Power,
    Fuel,
    Building
}

[System.Serializable]
public struct BuildingInventory
{
    public int maxCapacity; // The maximum number of distinct items the inventory can hold
    public int currentCapacity; // The current number of distinct items in the inventory

    [ShowInInspector]
    public Dictionary<Item, int> items; // The items and their quantities

    // Initializes the inventory with a given capacity
    public BuildingInventory(int capacity)
    {
        maxCapacity = capacity;
        currentCapacity = 0;
        items = new Dictionary<Item, int>();
    }

    // Adds an item to the inventory, if there's space
    public bool AddItem(Item item, int quantity)
    {
        if (currentCapacity < maxCapacity || items.ContainsKey(item))
        {
            if (items.ContainsKey(item))
            {
                items[item] += quantity;
            }
            else
            {
                items[item] = quantity;
                currentCapacity++;
            }
            return true;
        }
        return false; // Inventory is full
    }

    // Removes a specified quantity of an item from the inventory
    public bool RemoveItem(Item item, int quantity)
    {
        if (items.ContainsKey(item) && items[item] >= quantity)
        {
            items[item] -= quantity;
            if (items[item] <= 0)
            {
                items.Remove(item);
                currentCapacity--;
            }
            return true;
        }
        return false; // Not enough of the item in inventory
    }

    // Checks if the inventory contains a specific item and quantity
    public bool Contains(Item item, int quantity)
    {
        return items.ContainsKey(item) && items[item] >= quantity;
    }
}

// ----------> Buildings <----------
[System.Serializable]
public enum BuildingGroup // BuildingDatabase.cs
{
    None,
    Extractor,
    Conveyor,
    BasicCrafting,
    AdvancedCrafting,
    Power,
    Storage,
    Research,
    Other
}

[System.Serializable]
public struct BuildingData // BuildingDatabase.cs
{
    // Database struct for storing default building data
    [Header("Building Info")]
    public string name;
    public BuildingGroup group;
    public Building buildingPrefab;

    [Header("Building Visuals")]
    public TileBase StaticTile;
    public TileBase AnimTile;
    public Sprite Icon;
    public int DefaultRotation;

    [Header("Building Recipes")]
    public float operationRate;
    public List<CraftingRecipe> recipes;

    [Header("Ports")]
    public PortDirections portDirections;
}

[System.Serializable]
public struct BuildingInfo // Building.cs
{
    public int RotationIndex; // Stores the relative rotation index
    public int RealRotationIndex; // Stores the real rotation index
    public int Level; // The current level of the building
    public bool Active; // Whether the building is active or not
}

// ----------> Ports <----------
[System.Serializable]
public enum Direction
{
    North,
    East,
    South,
    West
};

[System.Serializable]
public enum PortFlow
{
    In,
    Out,
    InOut,
    None
}

[System.Serializable]
public enum PortType
{
    Item,
    Fluid,
    Power,
    Fuel
}

[System.Serializable]
public struct PortConfig
{
    public PortFlow Flow;
    public PortType Type;
    public float MaxTransferRate;
    public float TransferRate;
    public Item Item;
}

[System.Serializable]
public struct PortDirections
{
    public PortConfig North;
    public PortConfig East;
    public PortConfig South;
    public PortConfig West;
}

[System.Serializable]
public static class PortExtensions
{
    public static PortFlow OppositeFlow(this PortFlow flow)
    {
        switch (flow)
        {
            case PortFlow.In:
                return PortFlow.Out;
            case PortFlow.Out:
                return PortFlow.In;
            case PortFlow.InOut:
                return PortFlow.InOut;
            case PortFlow.None:
                return PortFlow.None;
            default:
                throw new ArgumentOutOfRangeException(nameof(flow), flow, null);
        }
    }

    public static bool IsCompatableWith(this Port thisPort, Port otherPort)
    {
        // Catch if either port flow is null (this means our inout check will not have to check if the other port is inout)
        if (thisPort.PortFlow == PortFlow.None || otherPort.PortFlow == PortFlow.None)
        {
            return false;
        }
        // Catch if the port types don't match
        if (thisPort.PortType != otherPort.PortType)
        {
            return false;
        }
        // Return false if both ports are InOut
        if (thisPort.PortFlow == PortFlow.InOut && otherPort.PortFlow == PortFlow.InOut)
        {
            return false;
        }
        // Return if either port is InOut (InOut ports are always compatable)
        if (thisPort.PortFlow == PortFlow.InOut || otherPort.PortFlow == PortFlow.InOut)
        {
            return true;
        }
        // Return if the ports are opposite (In and Out)
        if (thisPort.PortFlow == otherPort.PortFlow.OppositeFlow())
        {
            return true;
        }

        // Return false if none of the above are true
        return false;
    }
}

[System.Serializable]
public static class DirectionExtensions
{
    public static Direction Opposite(this Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return Direction.South;
            case Direction.South:
                return Direction.North;
            case Direction.East:
                return Direction.West;
            case Direction.West:
                return Direction.East;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Vector3Int Vector(this Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return new Vector3Int(0, 1, 0);
            case Direction.South:
                return new Vector3Int(0, -1, 0);
            case Direction.East:
                return new Vector3Int(1, 0, 0);
            case Direction.West:
                return new Vector3Int(-1, 0, 0);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Direction ToDirection(this Vector3Int vector)
    {
        if (vector.x > 0)
            return Direction.East;
        else if (vector.x < 0)
            return Direction.West;
        else if (vector.y > 0)
            return Direction.North;
        else if (vector.y < 0)
            return Direction.South;
        else
            throw new ArgumentException(
                "The provided vector does not correspond to a valid direction."
            );
    }
}

[System.Serializable]
public static class BuildingExtensions
{
    public static Direction? IsAdjacentTo(this Building buildingA, Building buildingB)
    {
        Vector3Int delta = buildingB.Location - buildingA.Location;

        // Check if it's a unit vector in one of the cardinal directions
        if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1 && delta.z == 0)
        {
            return delta.ToDirection();
        }

        return null; // Not adjacent
    }
}
