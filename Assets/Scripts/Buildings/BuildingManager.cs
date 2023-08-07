using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using System;

public class BuildingManager : MonoBehaviour
{
    public ItemDatabase itemDatabase;
    public BuildingDatabase buildingDatabase;
    public Tilemap buildingLayer;
    public Tilemap overlayLayer;
    public TerrainEngine terrainEngine;
    public Dictionary<BuildingGroup, GameObject> buildingGroupGameObjects;
    public Dictionary<Vector3Int, Building> buildings;
    public Dictionary<string, int> buildingTypeIndexes;
    public LayerName currentLayer = LayerName.Terrain;

    public int resourceLayerIndex = 1;

    private Dictionary<BuildingGroup, Dictionary<string, GameObject>> buildingObjectTree;

    // -------------------> Init Methods <-------------------
    private void Awake()
    {
        buildingGroupGameObjects = new Dictionary<BuildingGroup, GameObject>();
        buildings = new Dictionary<Vector3Int, Building>();
    }

    private void Start()
    {
        List<BuildingGroup> buildingGroups = Enum.GetValues(typeof(BuildingGroup))
            .Cast<BuildingGroup>()
            .ToList();

        buildingObjectTree = new Dictionary<BuildingGroup, Dictionary<string, GameObject>>();
        foreach (BuildingGroup group in buildingGroups)
        {
            GameObject groupParent = transform.Find(group.ToString())?.gameObject;

            if (groupParent == null)
            {
                groupParent = new GameObject(group.ToString());
                groupParent.transform.parent = transform;
            }

            // Create sub-groups for each building type within the current group
            var buildingsInCurrentGroup = buildingDatabase.GetBuildingsByGroup(group);
            var typeDict = new Dictionary<string, GameObject>();
            foreach (var buildingData in buildingsInCurrentGroup)
            {
                GameObject buildingTypeParent = groupParent.transform
                    .Find(buildingData.name)
                    ?.gameObject;

                if (!buildingTypeParent)
                {
                    buildingTypeParent = new GameObject(buildingData.name);
                    buildingTypeParent.transform.parent = groupParent.transform;
                }

                typeDict[buildingData.name] = buildingTypeParent;
            }

            // Cleanup: Remove any children not in the database
            foreach (Transform child in groupParent.transform)
            {
                if (!typeDict.ContainsKey(child.name))
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            buildingObjectTree[group] = typeDict;
        }
    }

    public BuildingManager(
        ItemDatabase itemDatabase,
        BuildingDatabase buildingDatabase,
        Tilemap buildingLayer,
        Tilemap overlayLayer,
        TerrainEngine terrainEngine
    )
    {
        this.itemDatabase = itemDatabase;
        this.buildingDatabase = buildingDatabase;
        this.buildingLayer = buildingLayer;
        this.overlayLayer = overlayLayer;
        this.terrainEngine = terrainEngine;
    }

    // -------------------> Tilemap Methods <-------------------
    public void PlaceTile(Tilemap tileMap, Vector3Int position, TileBase tileBase)
    {
        Vector3Int gridPosition = position;
        tileMap.SetTile(gridPosition, tileBase);
    }

    public void RotateTile(Tilemap tileMap, Vector3Int position, int rotationIndex)
    {
        Vector3Int gridPosition = position;
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90 * rotationIndex));
        tileMap.SetTransformMatrix(gridPosition, rotationMatrix);
    }

    public void ResetTile(Tilemap tileMap, Vector3Int position)
    {
        Vector3Int gridPosition = position;
        tileMap.SetTile(gridPosition, null);
        tileMap.SetTransformMatrix(gridPosition, Matrix4x4.identity);
    }

    // -------------------> Validation Methods <-------------------
    public bool CheckPosition(Vector3Int position)
    {
        if (buildings.ContainsKey(position))
        {
            return false;
        }
        return true;
    }

    // -------------------> Building Object Methods <-------------------
    public int GetNewBuildingIndex(BuildingData buildingData)
    {
        BuildingGroup group = buildingData.group;
        string buildingName = buildingData.name;

        if (
            buildingObjectTree.ContainsKey(group)
            && buildingObjectTree[group].ContainsKey(buildingName)
        )
        {
            // Get the number of child objects (existing buildings of this type)
            int currentCount = buildingObjectTree[group][buildingName].transform.childCount;

            // Increment by one to get the new index
            return currentCount + 1;
        }
        else
        {
            Debug.LogWarning(
                $"No parent object found for building: {buildingName} in group: {group}"
            );
            return -1; // Indicate an error
        }
    }

    public GameObject GetParentObjectForBuilding(BuildingData buildingData)
    {
        BuildingGroup group = buildingData.group;
        string buildingName = buildingData.name;
        if (
            buildingObjectTree.ContainsKey(group)
            && buildingObjectTree[group].ContainsKey(buildingName)
        )
        {
            return buildingObjectTree[group][buildingName];
        }
        else
        {
            Debug.LogWarning(
                $"No parent object found for building: {buildingName} in group: {group}"
            );
            return null;
        }
    }

    // -------------------> Building Fetching Methods <-------------------
    public List<Building> GetAllBuildings()
    {
        return buildings.Values.ToList();
    }

    public Building GetBuilding(Vector3Int position, Direction? direction = null)
    {
        // If a direction is provided, adjust the position.
        if (direction.HasValue)
        {
            Vector3Int offset = direction.Value.Vector(); // Use your Direction extension method.
            position += offset;
        }

        // Attempt to get the building at the adjusted position.
        buildings.TryGetValue(position, out Building building);

        // The TryGetValue method will return null for 'building' if there is no building at the position.
        return building;
    }

    public Dictionary<Direction, Building> GetAdjacentBuildings(Building building)
    {
        // Get the building's location.
        Vector3Int buildingPosition = building.Location;

        Dictionary<Direction, Building> adjacentBuildings = new Dictionary<Direction, Building>();

        // Iterate over each value in the Direction enum
        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            Building adjacentBuilding = GetBuilding(buildingPosition, direction);
            if (adjacentBuilding != null)
            {
                adjacentBuildings[direction] = adjacentBuilding;
            }
        }

        return adjacentBuildings;
    }

    // -------------------> Building Connection Methods <-------------------
    public bool ConnectBuildings(Building buildingA, Building buildingB)
    {
        // Calculate the direction from building A to building B
        Direction directionToB = (buildingB.Location - buildingA.Location).ToDirection();

        // Get the ports from each building in the appropriate direction
        Port portA = buildingA.Ports.GetPort(directionToB);
        Port portB = buildingB.Ports.GetPort(directionToB.Opposite());

        // Check if the ports are compatible for connection
        if (portA.IsCompatableWith(portB) == false)
        {
            return false;
        }

        // Connect the ports
        ConnectPorts(portA, portB);

        return true;
    }

    private void ConnectPorts(Port portA, Port portB)
    {
        portA.ConnectedPort = portB;
        portA.ConnectionEstablished = true;
        portB.ConnectedPort = portA;
        portB.ConnectionEstablished = true;
    }

    private void DisconnectPorts(Port portA, Port portB)
    {
        portA.ConnectedPort = null;
        portA.ConnectionEstablished = false;
        portB.ConnectedPort = null;
        portB.ConnectionEstablished = false;
    }

    // -------------------> Building Handler Methods <-------------------
    public virtual Building PlaceNewBuilding(BuildingHandler handler, Building ghostBuilding)
    {
        if (!CheckPosition(handler.cursorPosition))
        {
            return null;
        }

        PlaceBuildingTile(handler, ghostBuilding);
        int buildingIndex = DetermineBuildingIndex(ghostBuilding);
        Building newBuilding = ghostBuilding.Clone(ghostBuilding.buildingData.name + buildingIndex);
        buildings.Add(handler.cursorPosition, newBuilding);
        ParentBuilding(newBuilding, ghostBuilding.buildingData);

        Dictionary<Direction, Building> adjacentBuildings = GetAdjacentBuildings(newBuilding);
        foreach (Building adjacentBuilding in adjacentBuildings.Values)
        {
            bool connected = ConnectBuildings(newBuilding, adjacentBuilding);
            if (connected)
            {
                Debug.Log($"Connected {newBuilding.name} to {adjacentBuilding.name}");
            }
        }
        return newBuilding;
    }

    private void PlaceBuildingTile(BuildingHandler handler, Building ghostBuilding)
    {
        ResetTile(overlayLayer, handler.cursorPosition);
        PlaceTile(buildingLayer, handler.cursorPosition, ghostBuilding.GetTile());
        RotateTile(buildingLayer, handler.cursorPosition, handler.rotationIndex);
    }

    private int DetermineBuildingIndex(Building ghostBuilding)
    {
        return GetNewBuildingIndex(ghostBuilding.buildingData);
    }

    private void ParentBuilding(Building newBuilding, BuildingData buildingData)
    {
        BuildingGroup group = buildingData.group;
        string buildingName = buildingData.name;

        if (
            buildingObjectTree.ContainsKey(group)
            && buildingObjectTree[group].ContainsKey(buildingName)
        )
        {
            newBuilding.transform.parent = buildingObjectTree[group][buildingName].transform;
        }
        else
        {
            Debug.LogWarning(
                $"No parent object found for building: {buildingName} in group: {group}"
            );
        }
    }
}
