using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public enum Direction
{
    North,
    East,
    South,
    West
};

[System.Serializable]
public enum Flow
{
    Input,
    Output
};

// Structure for the in/out configuration
[System.Serializable]
public struct InOut
{
    public Direction direction;
    public Flow flow;
    public CraftableItem item;
}

[System.Serializable]
public struct BuildingSettings
{
    [Header("MetaData")]
    public bool RotationLocked; // Whether the building can be rotated or not
    public int DefaultRotationIndex; // Stores the default rotation index
    public int RotationIndex; // Stores the current rotation index
    public bool Active; // Whether the building is active or not
}
