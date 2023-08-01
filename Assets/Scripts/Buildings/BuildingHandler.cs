using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

public class BuildingHandler : MonoBehaviour
{
    public BuildingManager buildingManager;
    private IPlacementLogic currentPlacementLogic;
    public Vector3Int CursorPosition;
    public BuildingDatabase.BuildingData selectedBuilding;
    public Building selectedBuildingObject;
    public KeyCode rotateKey = KeyCode.R;
    private BuildingDatabase.BuildingGroup lastGroup = BuildingDatabase.BuildingGroup.None;
    private Dictionary<BuildingDatabase.BuildingGroup, int> lastSelectedBuildingIndex =
        new Dictionary<BuildingDatabase.BuildingGroup, int>(); // keeps track of the last selected building index for each group

    private void Start()
    {
        buildingManager = GetComponent<BuildingManager>();
        if (buildingManager == null)
            Debug.LogError("No BuildingManager found on the same game object");
        currentPlacementLogic = new DefaultPlacementLogic();
    }

    private void Update()
    {
        HandleSelect();
        if (selectedBuildingObject == null)
        {
            HandleRemove();
        }
        else
        {
            HandleDisplay();
            HandlePlacement();
            HandleDeselect();
            HandleRotate();
        }
    }

    public void HandleSelect()
    {
        foreach (
            BuildingDatabase.BuildingGroupHotkey groupHotkey in buildingManager
                .buildingDatabase
                .groupHotkeys
        )
        {
            if (Input.GetKeyDown(groupHotkey.hotkey))
            {
                List<BuildingDatabase.BuildingData> buildingsInGroup =
                    buildingManager.buildingDatabase.GetBuildingsByGroup(groupHotkey.group);

                if (buildingsInGroup.Count == 0)
                {
                    Debug.Log("No buildings in the selected group");
                    return;
                }

                int nextIndex = 0;
                if (lastSelectedBuildingIndex.ContainsKey(groupHotkey.group))
                {
                    nextIndex =
                        (lastSelectedBuildingIndex[groupHotkey.group] + 1) % buildingsInGroup.Count;
                }

                lastSelectedBuildingIndex[groupHotkey.group] = nextIndex;
                SelectBuilding(buildingsInGroup[nextIndex]);
            }
        }
    }

    public void SelectBuilding(BuildingDatabase.BuildingData buildingData)
    {
        if (selectedBuildingObject != null)
            Destroy(selectedBuildingObject.gameObject);

        selectedBuilding = buildingData;
        // selectedBuildingObject = buildingManager.CreateBuildingGameObject(selectedBuilding);
        currentPlacementLogic.DisplayBuildingAtCursor(this, true);
        Debug.Log("Selected building: " + selectedBuilding.name);
    }

    public Vector3Int GetCursorPosition()
    {
        // Convert the screen position of the mouse to world position
        Vector3 currentCursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Floor the position
        Vector3Int flooredPosition = new Vector3Int(
            Mathf.FloorToInt(currentCursorPosition.x),
            Mathf.FloorToInt(currentCursorPosition.y),
            0
        );

        return flooredPosition;
    }

    public void HandleDisplay()
    {
        currentPlacementLogic.DisplayBuildingAtCursor(this);
    }

    public void HandlePlacement()
    {
        if (Input.GetMouseButton(0))
        {
            currentPlacementLogic.PlaceBuildingAtCursor(this);
            // Call SelectBuilding again with the last selected building to create a new building object for placement
            SelectBuilding(selectedBuilding);
        }
    }

    public void HandleDeselect()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonUp(1))
        {
            selectedBuildingObject = null;
            selectedBuilding = default;
            lastGroup = BuildingDatabase.BuildingGroup.None;
            currentPlacementLogic.DisplayBuildingAtCursor(this, true);
        }
    }

    public void HandleRotate()
    {
        if (Input.GetKeyDown(rotateKey) && !selectedBuildingObject.BuildingSettings.RotationLocked)
        {
            selectedBuildingObject.Rotate();
            currentPlacementLogic.DisplayBuildingAtCursor(this, true);
        }
    }

    public void HandleRemove()
    {
        if (Input.GetMouseButton(1))
            currentPlacementLogic.RemoveBuildingAtCursor(this);
    }
}
