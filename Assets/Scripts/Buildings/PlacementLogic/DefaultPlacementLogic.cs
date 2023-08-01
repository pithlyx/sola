using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class DefaultPlacementLogic : IPlacementLogic
{
    public void DisplayBuildingAtCursor(BuildingHandler handler, bool force = false)
    {
        Vector3Int currentCursorPosition = handler.GetCursorPosition();
        Tilemap overlay = handler.buildingManager.overlayLayer;
        BuildingDatabase buildingDatabase = handler.buildingManager.buildingDatabase;
        Building selectedBuilding = handler.selectedBuildingObject;
        if (selectedBuilding == null)
        {
            return;
        }
        else
        {
            if (force || overlay.GetTile(currentCursorPosition) == null)
            {
                overlay.ClearAllTiles();
                overlay.SetTile(currentCursorPosition, selectedBuilding.GetTile());
            }
        }
    }

    public void PlaceBuildingAtCursor(BuildingHandler handler)
    {
        Vector3Int currentCursorPosition = handler.GetCursorPosition();
        Tilemap overlay = handler.buildingManager.overlayLayer;
        Tilemap buildingLayer = handler.buildingManager.buildingLayer;
        Building selectedBuilding = handler.selectedBuildingObject;
    }

    public void RemoveBuildingAtCursor(BuildingHandler handler)
    {
        Vector3Int currentCursorPosition = handler.GetCursorPosition();
        Tilemap overlay = handler.buildingManager.overlayLayer;
        Tilemap buildingLayer = handler.buildingManager.buildingLayer;
        Building selectedBuilding = handler.selectedBuildingObject;
    }
}
