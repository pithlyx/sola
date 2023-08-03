using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public abstract class Building : MonoBehaviour
{
    [Header("Base")]
    public BuildingSettings BuildingSettings; // Stores the default building metadata
    public BuildingDatabase.BuildingData buildingData; // Stores the data for the building from the database

    // Method to create a copy of the building
    public virtual Building Clone(string name = null)
    {
        // Create a new building with the name for the game object
        Building newBuilding = Instantiate(this, this.transform.position, this.transform.rotation);
        // name the game object
        newBuilding.gameObject.name = name ?? this.gameObject.name;
        // Return the new building
        return newBuilding;
    }

    public Vector3Int GetBuildingLocation()
    {
        return Vector3Int.FloorToInt(transform.position);
    }

    public void SetBuildingLocation(Vector3Int location)
    {
        transform.position = location;
    }

    public int RotationIndex
    {
        get { return BuildingSettings.RotationIndex; }
        set
        {
            BuildingSettings.RotationIndex = value % 4;
            transform.rotation = Quaternion.Euler(0, 0, -90 * BuildingSettings.RotationIndex);
        }
    }
    public bool IsActive
    {
        get { return BuildingSettings.Active; }
        set { BuildingSettings.Active = value; }
    }

    public BuildingDatabase.BuildingGroup GetBuildingGroup()
    {
        return buildingData.group;
    }

    public TileBase GetTile()
    {
        return IsActive ? buildingData.AnimTile : buildingData.StaticTile;
    }

    public virtual void OnPlaced(Item item = null) { }
}
