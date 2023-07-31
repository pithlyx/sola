using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class DefaultPlacementLogic : IPlacementLogic
{
    public void DisplayBuildingAtCursor(
        BuildingHandler handler,
        Vector3Int cursorPos,
        BuildingDatabase buildingDatabase,
        Tilemap overlayLayer
    )
    {
        // Get the selected building's tile
        TileBase buildingTile = handler.selectedBuilding.StaticTile;

        if (buildingTile == null)
        {
            Debug.LogError("The selected building does not have a static tile");
            return;
        }

        // Get the selected building's rotation matrix
        Matrix4x4 rotationMatrix = handler.selectedBuildingObject.GetBuildingRotationMatrix();

        // // Set the tile at the cursor position with the correct rotation
        // handler.SetTileWithRotation(cursorPos, buildingTile, rotationMatrix);
    }

    public void RotateBuilding(BuildingHandler handler, int rotationIndex) { }
}
