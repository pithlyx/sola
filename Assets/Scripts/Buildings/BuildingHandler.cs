using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class BuildingHandler : MonoBehaviour
{
    public KeyCode rotateKey = KeyCode.R; // The key to rotate the building
    private BuildingManager buildingManager;
    private BuildingData? selectedBuilding; // Made this nullable
    private Dictionary<BuildingGroup, int> lastSelectedBuildingIndex =
        new Dictionary<BuildingGroup, int>();
    public IPlacementLogic currentPlacementLogic;
    private Dictionary<BuildingGroup, IPlacementLogic> placementLogicByGroup;
    public Building ghostBuilding; // The "ghost" building that follows the cursor
    public Vector3Int cursorPosition; // The current cursor position
    public Vector3Int lastCursorPosition; // Store the last cursor position
    public int rotationIndex = 0; // Store the current rotation index
    public int lastRotationIndex = 0; // Store the last rotation index
    public int relativeCursorRotation = 0; // Store the relative cursor rotation

    private void Start()
    {
        buildingManager = GetComponentInParent<BuildingManager>();
        if (buildingManager == null)
        {
            Debug.LogError("No BuildingManager found in parent game object");
        }

        // Initialize the placement logic for each group
        placementLogicByGroup = new Dictionary<BuildingGroup, IPlacementLogic>
        {
            { BuildingGroup.None, new DefaultPlacementLogic() },
            { BuildingGroup.Extractor, new ExtractorPlacementLogic() },
            { BuildingGroup.Conveyor, new ConveyorPlacementLogic() }
            // Add other BuildingGroup and IPlacementLogic pairs here...
        };

        // Set the initial placement logic
        currentPlacementLogic = placementLogicByGroup[BuildingGroup.None];
    }

    private void Update()
    {
        cursorPosition = GetCursorPosition();
        if (cursorPosition != lastCursorPosition)
        {
            UpdateGhostObject();
        }

        DatabaseSelection();
        if (selectedBuilding.HasValue)
        {
            if (ghostBuilding == null)
            {
                CreateGhostBuilding();
            }
            UpdateGhostObject();
            HandleRotation();
            HandleDeselection();
            HandlePlacement();
        }
        else
        {
            HandleRemove();
        }

        lastCursorPosition = cursorPosition; // Update the last cursor position
        lastRotationIndex = rotationIndex; // Update the last rotation index
    }

    public void CreateGhostBuilding()
    {
        if (ghostBuilding == null)
        {
            ghostBuilding = buildingManager.buildingDatabase.CreateBuildingObject(
                selectedBuilding.Value,
                "Ghost " + selectedBuilding.Value.name
            );
            ghostBuilding.transform.position = GetCursorPosition();
            ghostBuilding.RotationIndex = rotationIndex;

            // Set the parent of the ghost building using the helper function
            GameObject parentObject = buildingManager.GetParentObjectForBuilding(
                selectedBuilding.Value
            );
            if (parentObject != null)
            {
                ghostBuilding.transform.SetParent(parentObject.transform);
            }
        }
    }

    public void UpdateGhostObject()
    {
        if (ghostBuilding != null)
        {
            ghostBuilding.transform.position = GetCursorPosition();
            ghostBuilding.RotationIndex = rotationIndex;
            currentPlacementLogic.TrackBuildingToCursor(this, buildingManager, ghostBuilding);
        }
    }

    public void HandleRotation(int newIndex = -1)
    {
        if (
            Input.GetKeyDown(rotateKey)
            || (newIndex != -1 && newIndex != rotationIndex && newIndex < 3)
        )
        {
            rotationIndex = (newIndex == -1) ? (rotationIndex + 1) % 4 : newIndex;

            ghostBuilding.RotationIndex = rotationIndex;
            UpdateGhostObject();
        }
    }

    public void HandleDeselection()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonUp(1))
        {
            DeselectBuilding();
            buildingManager.overlayLayer.ClearAllTiles();
        }
    }

    public void HandlePlacement()
    {
        if (Input.GetMouseButton(0))
        {
            Building newBuilding = currentPlacementLogic.PlaceBuilding(
                this,
                buildingManager,
                ghostBuilding
            );
            if (newBuilding != null)
            {
                if (newBuilding.buildingData.group == BuildingGroup.Conveyor)
                {
                    newBuilding.SetOutputItems();
                }
                UpdateGhostObject();
            }
        }
    }

    public void MakeParent(Building building)
    {
        building.transform.SetParent(this.transform);
    }

    public void HandleRemove()
    {
        if (Input.GetMouseButton(1) && ghostBuilding == null)
        {
            currentPlacementLogic.RemoveBuilding(buildingManager, GetCursorPosition());
            UpdateGhostObject();
        }
    }

    public Vector3Int GetCursorPosition()
    {
        // Get the world coordinates of the cursor and return the Vector3Int rounded down
        return Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void DeselectBuilding()
    {
        if (ghostBuilding != null)
        {
            // Destroy the "ghost" building
            Destroy(ghostBuilding.gameObject);
            ghostBuilding = null;
        }

        selectedBuilding = null; // Assign null to selectedBuilding
    }

    public void DatabaseSelection()
    {
        foreach (BuildingGroupHotkey groupHotkey in buildingManager.buildingDatabase.groupHotkeys)
        {
            if (Input.GetKeyDown(groupHotkey.hotkey))
            {
                UpdateGhostObject();
                List<BuildingData> buildingsInGroup =
                    buildingManager.buildingDatabase.GetBuildingsByGroup(groupHotkey.group);
                if (buildingsInGroup.Count > 0)
                {
                    int nextIndex = 0;
                    if (
                        selectedBuilding.HasValue
                        && selectedBuilding.Value.group == groupHotkey.group
                        && lastSelectedBuildingIndex.ContainsKey(groupHotkey.group)
                    )
                    {
                        int currentIndex = lastSelectedBuildingIndex[groupHotkey.group];
                        nextIndex = (currentIndex + 1) % buildingsInGroup.Count;

                        if (nextIndex == currentIndex && buildingsInGroup.Count > 1)
                        {
                            Debug.Log("No building to select");
                            return;
                        }

                        if (selectedBuilding.Value.name == buildingsInGroup[nextIndex].name)
                        {
                            Debug.Log("Building already selected");
                            return;
                        }
                    }

                    DeselectBuilding(); // Delete the old "ghost" building

                    lastSelectedBuildingIndex[groupHotkey.group] = nextIndex;
                    selectedBuilding = buildingsInGroup[nextIndex];

                    ChangePlacementStrategy(selectedBuilding.Value); // Change the placement strategy
                }
                else
                {
                    Debug.Log("No buildings in the selected group");
                }
            }
        }
    }

    public void ChangePlacementStrategy(BuildingData buildingData)
    {
        BuildingGroup group = buildingData.group;
        if (placementLogicByGroup.ContainsKey(group))
        {
            currentPlacementLogic = placementLogicByGroup[group];
        }
        else
        {
            Debug.LogWarning(
                $"No placement strategy defined for group {group}. Using default strategy."
            );
            currentPlacementLogic = placementLogicByGroup[BuildingGroup.None];
        }
    }

    // Other methods and functionality...
}
