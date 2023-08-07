using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct BuildingGroupHotkey
{
    public BuildingGroup group;
    public KeyCode hotkey;
}

[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "Database/BuildingDatabase", order = 1)]
public class BuildingDatabase : ScriptableObject
{
    public List<BuildingData> allBuildings;
    public List<BuildingGroupHotkey> groupHotkeys; // The hotkeys for each building group

    public BuildingData GetBuildingDataByName(string name)
    {
        var result = allBuildings.FirstOrDefault(buildingData => buildingData.name == name);
        if (result.name == null)
        {
            Debug.LogError("No building data found for the given name: " + name);
            return default(BuildingData); // Return the default value for BuildingData
        }
        return result;
    }

    public BuildingData GetBuildingDataByPrefab(Building buildingPrefab)
    {
        var result = allBuildings.FirstOrDefault(
            buildingData => buildingData.buildingPrefab == buildingPrefab
        );
        if (result.buildingPrefab == null)
        {
            Debug.LogError("No building data found for the given prefab: " + buildingPrefab.name);
            return default(BuildingData);
        }
        return result;
    }

    public List<BuildingData> GetBuildingsByGroup(BuildingGroup group)
    {
        return allBuildings.FindAll(buildingData => buildingData.group == group);
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

    public virtual Building CreateBuildingObject(BuildingData data, string name)
    {
        // Instantiate a new instance of this prefab
        Building newBuilding = Instantiate(data.buildingPrefab);

        // Set the name of the GameObject
        newBuilding.gameObject.name = name;

        // Set the buildingData of the new building
        newBuilding.buildingData = data;

        // Return the new building instance
        return newBuilding;
    }
}
