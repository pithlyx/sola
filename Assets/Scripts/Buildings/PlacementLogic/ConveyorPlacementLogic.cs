using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class ConveyorPlacementLogic : DefaultPlacement
{
    public override Building PlaceBuilding(
        BuildingHandler handler,
        BuildingManager manager,
        Building ghostBuilding
    )
    {
        Building newBuilding = base.PlaceBuilding(handler, manager, ghostBuilding);
        // Check adjacency
        if (manager.lastPlacedBuilding != null && newBuilding != null)
        {
            Direction? adjacencyDirection = manager.lastPlacedBuilding.IsAdjacentTo(newBuilding);

            if (adjacencyDirection.HasValue)
            {
                Debug.Log(
                    $"Adjacent: {adjacencyDirection.Value} | {(int)adjacencyDirection.Value}\nDifference: {manager.lastPlacedBuilding.RotationIndex - (int)adjacencyDirection.Value}"
                );
                // Set the rotation index of the last building to match the index of the adjacency direction
                manager.UpdateBuilding(manager.lastPlacedBuilding, (int)adjacencyDirection.Value);
            }
        }

        return newBuilding;
    }
}
