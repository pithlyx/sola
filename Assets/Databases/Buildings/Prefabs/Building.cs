using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public abstract class Building : MonoBehaviour
{
    [Header("Base")]
    public BuildingInfo buildingInfo; // Stores the default building information
    public BuildingData buildingData; // Stores the data for the building from the database

    [Header("Ports")]
    [SerializeField]
    private PortCollection _portCollection; // Stores the port information for the building
    public PortCollection Ports
    {
        get { return _portCollection; }
        set { _portCollection = value; }
    }

    public LayerName operationLayer;

    // Define the overridable method to set the output items
    public virtual void SetOutputItems() { }

    public virtual Building Clone(string name = null)
    {
        // Create a new building with the name for the game object
        Building newBuilding = Instantiate(this, this.transform.position, this.transform.rotation);
        // name the game object
        newBuilding.gameObject.name = name ?? this.gameObject.name;

        newBuilding.Ports = new PortCollection(buildingData.portDirections);
        // Return the new building
        return newBuilding;
    }

    public Vector3Int Location
    {
        get { return Vector3Int.FloorToInt(transform.position); }
        set { transform.position = value; }
    }

    public int RotationIndex
    {
        get { return buildingInfo.RotationIndex; }
        set
        {
            buildingInfo.RotationIndex = value % 4;
            transform.rotation = Quaternion.Euler(0, 0, -90 * buildingInfo.RotationIndex);
        }
    }
    public bool IsActive
    {
        get { return buildingInfo.Active; }
        set { buildingInfo.Active = value; }
    }

    public BuildingGroup GetBuildingGroup()
    {
        return buildingData.group;
    }

    public TileBase GetTile()
    {
        return IsActive ? buildingData.AnimTile : buildingData.StaticTile;
    }

    public void UpdatePorts()
    {
        // Update the ports
        Ports.UpdateAllPorts();
    }

    // Method to get ports based on PortFlow
    public List<Port> GetPortsByFlow(PortFlow portFlow)
    {
        List<Port> ports = new List<Port>();

        foreach (Port port in Ports.AllPorts)
        {
            if (port.PortFlow == portFlow || port.PortFlow == PortFlow.InOut)
            {
                ports.Add(port);
            }
        }

        return ports;
    }
}
