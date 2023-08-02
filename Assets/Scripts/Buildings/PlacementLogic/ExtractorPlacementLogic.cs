using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class ExtractorPlacementLogic : IPlacementLogic
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
        // Use a Switch statement to determine the resource layer index based on building name
        int resourceLayerIndex = 0;
        switch (newBuilding.buildingData.name)
        {
            case "Pump":
                resourceLayerIndex = 0;
                break;
            case "Mine":
                resourceLayerIndex = 1;
                break;
            default:
                resourceLayerIndex = 1;
                break;
        }
        // Create a Vector3Int for the resource location with the z value set to the resource layer index
        Vector3Int resourceLocation = new Vector3Int(
            handler.cursorPosition.x,
            handler.cursorPosition.y,
            resourceLayerIndex
        );
        // Get the resource for the extractor
        Resource resource = manager.chunkGenerator.GetResourceForPoint(resourceLocation);
        // Convert the resource to an item
        CraftableItem item = manager.itemDatabase.GetItemByResource(resource);

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
