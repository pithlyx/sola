using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "Database/BuildingDatabase", order = 1)]
public class BuildingDatabase : ScriptableObject
{
    public enum BuildingGroup
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
    public struct BuildingLevelData
    {
        public float operationRate; // The operation rate at this level
        public List<CraftableItem> craftableItems; // The items that can be crafted at this level
        public List<MaterialCost> buildCost; // The cost to build or upgrade to this level

        [Header("Fuel")]
        public bool requiresFuel; // Whether or not the building requires fuel to operate
        public CraftableItem fuelRequired; // The fuel required to operate the building
        public float maxStock; // The maximum amount of "fuel" the building can hold
        public int fuelStock; // The amount of "fuel" the building has
        public float operationCost; // The fuel cost of a single operation
    }

    [System.Serializable]
    public struct BuildingData
    {
        public string name; // The name of the building
        public BuildingGroup group; // The group the building belongs to
        public Building buildingPrefab;
        public TileBase StaticTile; // The static tile for the building
        public TileBase AnimTile; // The animated tile for the building
        public Sprite Icon; // The icon for the building
        public List<BuildingLevelData> levels; // The data for each level of the building

        [Header("Rotation")]
        public bool canRotate; // Whether or not the building can be rotated
        public int defaultRotationIndex; // The default rotation of the building

        [Header("Input/Output")]
        public List<InOut> inOuts; // Input and output configuration for the building
    }

    [System.Serializable]
    public struct BuildingGroupHotkey
    {
        public BuildingGroup group;
        public KeyCode hotkey;
    }

    public List<BuildingData> allBuildings;
    public List<BuildingGroupHotkey> groupHotkeys; // The hotkeys for each building group

    public BuildingData GetBuildingDataByName(string name)
    {
        foreach (BuildingData buildingData in allBuildings)
        {
            if (buildingData.name == name)
            {
                return buildingData;
            }
        }

        Debug.LogError("No building data found for the given name: " + name);
        return default(BuildingData); // Return the default value for BuildingData
    }

    public List<BuildingData> GetBuildingsByGroup(BuildingGroup group)
    {
        return allBuildings.FindAll(buildingData => buildingData.group == group);
    }

    public BuildingData GetBuildingDataByPrefab(Building buildingPrefab)
    {
        foreach (BuildingData buildingData in allBuildings)
        {
            if (buildingData.buildingPrefab == buildingPrefab)
            {
                return buildingData;
            }
        }

        Debug.LogError("No building data found for the given prefab: " + buildingPrefab.name);
        return default(BuildingData);
    }

    public KeyCode GetHotkeyByGroup(BuildingGroup group)
    {
        foreach (BuildingGroupHotkey groupHotkey in groupHotkeys)
        {
            if (groupHotkey.group == group)
            {
                return groupHotkey.hotkey;
            }
        }

        Debug.LogError("No hotkey found for the given group: " + group);
        return default(KeyCode);
    }

    public List<BuildingData> GetAllBuildings()
    {
        return allBuildings;
    }
}
