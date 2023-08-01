using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public abstract class Building : MonoBehaviour
{
    [Header("Base")]
    public BuildingSettings BuildingSettings; // Stores the default building settings
    public BuildingDatabase.BuildingData buildingData;

    [Header("Input/Output")]
    public List<InOut> inOuts; // Input and output configuration for the building

    // Create a new InOut configuration
    public void CreateInOut(InOut newInOut)
    {
        inOuts.Add(newInOut);
    }

    // Read an existing InOut configuration
    public InOut ReadInOut(int index)
    {
        if (index < 0 || index >= inOuts.Count)
        {
            Debug.LogError("Invalid index for reading InOut");
            return default; // Returns the default value for the InOut struct
        }

        return inOuts[index];
    }

    // Update an existing InOut configuration
    public void UpdateInOut(int index, InOut updatedInOut)
    {
        if (index < 0 || index >= inOuts.Count)
        {
            Debug.LogError("Invalid index for updating InOut");
            return;
        }

        inOuts[index] = updatedInOut;
    }

    // Delete an existing InOut configuration
    public void DeleteInOut(int index)
    {
        if (index < 0 || index >= inOuts.Count)
        {
            Debug.LogError("Invalid index for deleting InOut");
            return;
        }

        inOuts.RemoveAt(index);
    }

    public virtual Building CreateBuilding()
    {
        // Instantiate a new instance of this prefab
        GameObject newInstance = Instantiate(gameObject);

        // Get the Building component from the new instance
        Building newBuilding = newInstance.GetComponent<Building>();

        // Copy the InOut configurations from the BuildingData to the new Building
        newBuilding.inOuts = new List<InOut>(buildingData.inOuts);

        // Return the new building instance
        return newBuilding;
    }

    // Method to connect an adjacent building
    public virtual CraftableItem ConnectBuilding(Building adjacentBuilding)
    {
        // Calculate relative position of the adjacent building
        Vector3Int relativePos =
            adjacentBuilding.BuildingSettings.Location - this.BuildingSettings.Location;

        // Check if the adjacent building is indeed adjacent
        if (Mathf.Abs(relativePos.x) + Mathf.Abs(relativePos.y) != 1)
        {
            Debug.LogError("Building is not adjacent");
            return null;
        }

        // Determine the direction of the adjacent building
        Direction adjacentDirection;
        if (relativePos == Vector3Int.up)
            adjacentDirection = Direction.North;
        else if (relativePos == Vector3Int.right)
            adjacentDirection = Direction.East;
        else if (relativePos == Vector3Int.down)
            adjacentDirection = Direction.South;
        else /* if (relativePos == Vector3Int.left) */
            adjacentDirection = Direction.West;

        // Get the opposite direction
        Direction oppositeDirection = (Direction)(((int)adjacentDirection + 2) % 4);

        // Find InOut of the current building that matches the direction of the adjacent building
        int currentInOutIndex = this.inOuts.FindIndex(
            inOut => inOut.direction == adjacentDirection
        );

        // Find InOut of the adjacent building that matches the opposite direction
        int adjacentInOutIndex = adjacentBuilding.inOuts.FindIndex(
            inOut => inOut.direction == oppositeDirection
        );

        // Check if matching InOuts were found and if they have opposite flows
        if (
            currentInOutIndex >= 0
            && adjacentInOutIndex >= 0
            && this.inOuts[currentInOutIndex].flow
                != adjacentBuilding.inOuts[adjacentInOutIndex].flow
        )
        {
            if (this.inOuts[currentInOutIndex].flow == Flow.Output)
            {
                // Set the item of the adjacent building to the output of the current building
                InOut adjacentInOut = adjacentBuilding.inOuts[adjacentInOutIndex];
                adjacentInOut.item = this.inOuts[currentInOutIndex].item;
                adjacentBuilding.inOuts[adjacentInOutIndex] = adjacentInOut;
            }
            else
            {
                // Set the input of the current building to the output of the adjacent building
                InOut currentInOut = this.inOuts[currentInOutIndex];
                currentInOut.item = adjacentBuilding.inOuts[adjacentInOutIndex].item;
                this.inOuts[currentInOutIndex] = currentInOut;
            }

            return this.inOuts[currentInOutIndex].item;
        }

        return null;
    }

    public void SetBuildingActive()
    {
        BuildingSettings.Active = true;
    }

    public void SetBuildingInactive()
    {
        BuildingSettings.Active = false;
    }

    public Vector3Int GetBuildingLocation()
    {
        return Vector3Int.FloorToInt(transform.position);
    }

    public bool IsBuildingActive()
    {
        return BuildingSettings.Active;
    }

    public int Rotate(int RotationIndex = 1, bool relative = true)
    {
        if (relative)
        {
            return (
                BuildingSettings.RotationIndex =
                    (BuildingSettings.RotationIndex + RotationIndex) % 4
            );
        }
        else
        {
            return (BuildingSettings.RotationIndex = RotationIndex % 4);
        }
    }

    public int GetRotationIndex()
    {
        return BuildingSettings.RotationIndex;
    }

    public Matrix4x4 GetRotationMatrix()
    {
        // return a matrix that rotates the building by the rotation index (0=0, 1=-90, 2=-180, 3=-270)
        return Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90 * BuildingSettings.RotationIndex));
    }

    public BuildingDatabase.BuildingGroup GetBuildingGroup()
    {
        return buildingData.group;
    }

    public TileBase GetTile()
    {
        return BuildingSettings.Active ? buildingData.AnimTile : buildingData.StaticTile;
    }

    public virtual void OnPlaced(CraftableItem item = null) { }
}
