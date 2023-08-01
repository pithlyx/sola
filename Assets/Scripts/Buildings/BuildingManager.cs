using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public ItemDatabase itemDatabase;
    public BuildingDatabase buildingDatabase;
    public Tilemap buildingLayer;
    public Tilemap overlayLayer;
    public ChunkGenerator chunkGenerator;
    public Dictionary<BuildingDatabase.BuildingGroup, GameObject> buildingGroupGameObjects =
        new Dictionary<BuildingDatabase.BuildingGroup, GameObject>(); // Dictionary to store group GameObjects
    private Dictionary<Vector3Int, Building> buildings = new Dictionary<Vector3Int, Building>(); // Dictionary to store all the buildings

    public Building NewBuilding(BuildingDatabase.BuildingData buildingData)
    {
        if (!buildingGroupGameObjects.ContainsKey(buildingData.group)) // Check if group GameObject already exists
        {
            GameObject newGroupGameObject = new GameObject(buildingData.group.ToString()); // Create a new GameObject
            newGroupGameObject.transform.parent = this.transform; // Set this GameObject as parent
            buildingGroupGameObjects.Add(buildingData.group, newGroupGameObject); // Add new GameObject to the dictionary
        }

        GameObject groupGameObject = buildingGroupGameObjects[buildingData.group]; // Get the group GameObject
        Building newBuilding = buildingData.buildingPrefab.CreateBuilding(); // Instantiate a new Building from prefab
        newBuilding.buildingData = buildingData; // Set the buildingData for the new Building
        GameObject newBuildingGameObject = newBuilding.gameObject; // Get the GameObject of the new Building
        newBuildingGameObject.transform.parent = groupGameObject.transform; // Set the group GameObject as parent
        newBuildingGameObject.name = buildingData.name; // Set the name of the GameObject

        return newBuilding; // Return the new Building
    }

    // public method that will add a building to the dictionary
    public Building AddBuilding(Building building)
    {
        Vector3Int buildingLocation = building.GetBuildingLocation(); // Get the location of the building
        // if there is already a building at the location return null
        if (buildings.ContainsKey(buildingLocation))
        {
            return null;
        }
        buildings.Add(buildingLocation, building); // Add the building to the dictionary
        return building; // Return the building
    }

    // public method that will get a building from the dictionary
    public Building GetBuilding(Vector3Int buildingLocation)
    {
        Building building = buildings[buildingLocation]; // Get the building from the dictionary
        // if the building doesn't exist return null
        if (building == null)
        {
            return null;
        }
        return building; // Return the building
    }
}
