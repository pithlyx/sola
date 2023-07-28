using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public struct BuildingInfo
    {
        public Building PrefabType;
        public GameObject Instance;
        public int Id;
    }

    public Dictionary<Vector3Int, BuildingInfo> buildings = new Dictionary<Vector3Int, BuildingInfo>();

    public void AddBuilding(Vector3Int position, Building prefabType, GameObject instance, int id)
    {
        var buildingInfo = new BuildingInfo
        {
            PrefabType = prefabType,
            Instance = instance,
            Id = id
        };

        buildings[position] = buildingInfo;
    }

    public BuildingInfo GetBuildingInfo(Vector3Int position)
    {
        if (buildings.TryGetValue(position, out BuildingInfo buildingInfo))
        {
            return buildingInfo;
        }

        return default;
    }

    public void RemoveBuilding(Vector3Int position)
    {
        buildings.Remove(position);
    }
}
