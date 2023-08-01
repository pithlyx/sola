using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IPlacementLogic
{
    void DisplayBuildingAtCursor(BuildingHandler handler, bool force = false);
    void PlaceBuildingAtCursor(BuildingHandler handler);
    void RemoveBuildingAtCursor(BuildingHandler handler);
}
