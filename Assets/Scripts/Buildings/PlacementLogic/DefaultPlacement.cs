using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public abstract class DefaultPlacement : IPlacementLogic
{
    public virtual void TrackBuildingToCursor(
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
        manager.RotateTile(
            manager.overlayLayer,
            handler.cursorPosition,
            ghostBuilding.RotationIndex
        );
    }

    public virtual Building PlaceBuilding(
        BuildingHandler handler,
        BuildingManager manager,
        Building ghostBuilding
    )
    {
        Building newBuilding = manager.PlaceNewBuilding(handler, ghostBuilding);

        return newBuilding;
    }

    public virtual void RemoveBuilding(BuildingManager manager, Vector3Int cursorPosition)
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
