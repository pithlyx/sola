using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public interface IPlacementLogic
{
    void TrackBuildingToCursor(
        BuildingHandler handler,
        BuildingManager manager,
        Building selectedBuilding
    );
    void PlaceBuilding(BuildingHandler handler, BuildingManager manager, Building ghostBuilding);
    void RemoveBuilding(BuildingManager manager, Vector3Int cursorPosition);
}
