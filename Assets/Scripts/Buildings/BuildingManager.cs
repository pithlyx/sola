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
    public Dictionary<BuildingDatabase.BuildingGroup, GameObject> buildingGroupGameObjects;
    public Dictionary<Vector3Int, Building> buildings;
    public Dictionary<string, int> buildingTypeIndexes;
    public LayerName currentLayer = LayerName.Terrain;

    public int resourceLayerIndex = 1;

    private void Awake()
    {
        buildingGroupGameObjects = new Dictionary<BuildingDatabase.BuildingGroup, GameObject>();
        buildings = new Dictionary<Vector3Int, Building>();
        buildingTypeIndexes = new Dictionary<string, int>();
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

    public bool CheckPosition(Vector3Int position)
    {
        if (buildings.ContainsKey(position))
        {
            return false;
        }
        return true;
    }

    public List<Building> GetBuildings()
    {
        return buildings.Values.ToList();
    }

    public List<Building> GetBuildingsByGroup(BuildingDatabase.BuildingGroup groupName)
    {
        return buildings.Values
            .Where(building => building.buildingData.group == groupName)
            .ToList();
    }

    public List<Building> GetBuildingsByName(string buildingName)
    {
        return buildings.Values
            .Where(building => building.buildingData.name == buildingName)
            .ToList();
    }

    public int GetNewBuildingIndex(string buildingName)
    {
        // Check if we already have an index for this building type
        if (buildingTypeIndexes.ContainsKey(buildingName))
        {
            // Increment the index and return the new value
            buildingTypeIndexes[buildingName]++;
        }
        else
        {
            // This is the first building of this type, so initialize the index at 1
            buildingTypeIndexes[buildingName] = 1;
        }

        return buildingTypeIndexes[buildingName];
    }

    // Other methods and functionality...
}
