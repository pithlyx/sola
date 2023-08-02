using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class ConveyorPlacementLogic : IPlacementLogic
{
    public void TrackBuildingToCursor(
        BuildingHandler handler,
        BuildingManager manager,
        Building ghostBuilding
    )
    {
        // Remove the building's tile from previous position
        manager.ResetTile(manager.overlayLayer, handler.lastCursorPosition);
        // Set the Overlay tilemap to display the selected building's sprite
        manager.PlaceTile(manager.overlayLayer, handler.cursorPosition, ghostBuilding.GetTile());
        // Set the tiles rotation to the current rotation index
        manager.RotateTile(manager.overlayLayer, handler.cursorPosition, handler.rotationIndex);
    }

    public void PlaceBuilding(
        BuildingHandler handler,
        BuildingManager manager,
        Building ghostBuilding
    )
    {
        // Check if a building is already in that location
        if (!manager.CheckPosition(handler.cursorPosition))
        {
            return;
        }
        // Remove the building's tile from the overlay layer
        manager.ResetTile(manager.overlayLayer, handler.cursorPosition);
        // Place the building's tile on the building layer
        manager.PlaceTile(manager.buildingLayer, handler.cursorPosition, ghostBuilding.GetTile());
        // Set the tiles rotation to the current rotation index
        manager.RotateTile(manager.buildingLayer, handler.cursorPosition, handler.rotationIndex);
        int buildingIndex = manager.GetNewBuildingIndex(ghostBuilding.buildingData.name);
        // Create a copy of the ghost building
        Building newBuilding = ghostBuilding.Clone(ghostBuilding.buildingData.name + buildingIndex);
        // Add the building to the buildings dictionary
        manager.buildings.Add(handler.cursorPosition, newBuilding);
    }

    public void RemoveBuilding(BuildingManager manager, Vector3Int cursorPosition)
    {
        if (manager.buildings.ContainsKey(cursorPosition))
        {
            // Get the building from the buildings dictionary
            Building buildingToRemove = manager.buildings[cursorPosition];
            // Remove the building's tile from the tilemap
            manager.ResetTile(manager.buildingLayer, cursorPosition);

            // Remove the building from the buildings dictionary
            manager.buildings.Remove(cursorPosition);

            // Destroy the building's game object
            GameObject.Destroy(buildingToRemove.gameObject);
        }
    }
}
