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
public struct BuildingSettings
{
    public bool RotationLocked; // Whether the building can be rotated or not
    public int RotationIndex; // Stores the current rotation index
    public int level; // The current level of the building
    public bool Active; // Whether the building is active or not
}

[System.Serializable]
public enum Flow
{
    Input,
    Output
}

[System.Serializable]
public struct Ports
{
    public PortConfig NorthConfig;
    public PortConfig EastConfig;
    public PortConfig SouthConfig;
    public PortConfig WestConfig;
}

public enum ConnectionType
{
    item,
    fluid,
    Power
}

[System.Serializable]
public struct PortConfig
{
    public bool canConnect;
    public ConnectionType connectionType;
    public Flow flowType;
    public float maxFlowRate;
    public float flowRate;
    public Building connectedBuilding;
}
