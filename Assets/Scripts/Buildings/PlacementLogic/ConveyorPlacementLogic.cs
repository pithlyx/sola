using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class ConveyorPlacementLogic : DefaultPlacement
{
    Vector3Int initialPlacementDirection = Vector3Int.zero; // Direction of the initial placement

    public override Building PlaceBuilding(
        BuildingHandler handler,
        BuildingManager manager,
        Building ghostBuilding
    )
    {
        // Update Ghost Building Before Placement
        Vector3Int direction = handler.cursorPosition - handler.lastCursorPosition;

        // If this is the first conveyor, save the initial placement direction
        if (initialPlacementDirection == Vector3Int.zero)
        {
            initialPlacementDirection = direction;
        }

        // Set the rotation based on the cursor movement direction
        handler.rotationIndex = GetRotationIndex(direction, initialPlacementDirection);
        handler.HandleRotation(handler.rotationIndex);

        // Use the base class's PlaceBuilding method place the updated ghost building
        var newBuilding = base.PlaceBuilding(handler, manager, ghostBuilding);

        return newBuilding;
    }

    private int GetRotationIndex(Vector3Int direction, Vector3Int initialDirection)
    {
        if (direction == initialDirection)
        {
            // Moving in the same direction as the initial placement
            return 0;
        }
        else if (direction == -initialDirection)
        {
            // Moving in the opposite direction of the initial placement
            return 2;
        }
        else if (direction == new Vector3Int(-initialDirection.y, initialDirection.x, 0))
        {
            // Moving to the right relative to the initial placement
            return 1;
        }
        else
        {
            // Moving to the left relative to the initial placement
            return 3;
        }
    }
}
