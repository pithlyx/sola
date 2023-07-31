using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IPlacementLogic
{
    void DisplayBuildingAtCursor(
        BuildingHandler handler,
        Vector3Int cursorPos,
        BuildingDatabase buildingDatabase,
        Tilemap overlayLayer
    );
    void RotateBuilding(BuildingHandler buildingHandler, int rotationIndex);
    // Add other methods as needed
}
