using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public BuildingDatabase buildingDatabase;
    public Tilemap buildingLayer;
    public Tilemap overlayLayer;
    public ChunkGenerator chunkGenerator;
    public Dictionary<BuildingDatabase.BuildingGroup, GameObject> buildingGroupGameObjects =
        new Dictionary<BuildingDatabase.BuildingGroup, GameObject>(); // Dictionary to store group GameObjects
    private List<Building> buildings = new List<Building>(); // List to store all the buildings

    public Building CreateBuildingGameObject(
        BuildingDatabase.BuildingData buildingData,
        int buildingIndex
    )
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
        newBuildingGameObject.name = buildingData.name + "_" + buildingIndex; // Set the name of the GameObject
        buildings.Add(newBuilding); // Add the new Building to the buildings list
        return newBuilding; // Return the new Building
    }
}
