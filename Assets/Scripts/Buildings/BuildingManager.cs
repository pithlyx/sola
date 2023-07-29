using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class BuildingManager : MonoBehaviour
{
    public struct BuildingInfo
    {
        public Building PrefabType;
        public GameObject Instance;
        public int Id;
    }

    public Dictionary<Vector3Int, BuildingInfo> buildings =
        new Dictionary<Vector3Int, BuildingInfo>();

    // Reference to BuildingHandler
    public BuildingHandler buildingHandler;

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

    private void Update()
    {
        UpdateBuildingStates();
    }

    private void UpdateBuildingStates()
    {
        foreach (KeyValuePair<Vector3Int, BuildingInfo> entry in buildings)
        {
            Building building = entry.Value.Instance.GetComponent<Building>();
            if (building != null)
            {
                TileBase tile = building.GetState();
                buildingHandler.buildingsTilemap.SetTile(entry.Key, tile);
                buildingHandler.buildingsTilemap.SetTransformMatrix(
                    entry.Key,
                    building.rotationMatrix
                );
            }
        }
    }
}
