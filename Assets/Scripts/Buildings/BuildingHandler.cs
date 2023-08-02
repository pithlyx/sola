using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class BuildingHandler : MonoBehaviour
{
    public KeyCode rotateKey = KeyCode.R; // The key to rotate the building
    private BuildingManager buildingManager;
    private BuildingDatabase.BuildingData? selectedBuilding; // Made this nullable
    private Dictionary<BuildingDatabase.BuildingGroup, int> lastSelectedBuildingIndex =
        new Dictionary<BuildingDatabase.BuildingGroup, int>();
    public IPlacementLogic currentPlacementLogic;
    private Dictionary<BuildingDatabase.BuildingGroup, IPlacementLogic> placementLogicByGroup;
    public Building ghostBuilding; // The "ghost" building that follows the cursor
    public Vector3Int cursorPosition; // The current cursor position
    public Vector3Int lastCursorPosition; // Store the last cursor position
    public int rotationIndex = 0; // Store the current rotation index
    public int lastRotationIndex = 0; // Store the last rotation index
    private bool updated = false;

    private void Start()
    {
        buildingManager = GetComponentInParent<BuildingManager>();
        if (buildingManager == null)
        {
            Debug.LogError("No BuildingManager found in parent game object");
        }

        // Initialize the placement logic for each group
        placementLogicByGroup = new Dictionary<BuildingDatabase.BuildingGroup, IPlacementLogic>
        {
            { BuildingDatabase.BuildingGroup.None, new DefaultPlacementLogic() },
            { BuildingDatabase.BuildingGroup.Extractor, new ExtractorPlacementLogic() },
            { BuildingDatabase.BuildingGroup.Conveyor, new ConveyorPlacementLogic() }
            // Add other BuildingGroup and IPlacementLogic pairs here...
        };

        // Set the initial placement logic
        currentPlacementLogic = placementLogicByGroup[BuildingDatabase.BuildingGroup.None];
    }

    private void Update()
    {
        cursorPosition = GetCursorPosition();
        if (cursorPosition != lastCursorPosition)
        {
            updated = false;
        }
        DatabaseSelection();
        if (selectedBuilding.HasValue)
        {
            if (ghostBuilding == null)
            {
                CreateGhostBuilding();
            }
            HandleRotation();
            HandleDeselection();
            HandlePlacement();
        }
        else
        {
            HandleRemove();
        }

        if (!updated && ghostBuilding != null)
        {
            updated = true;
            ghostBuilding.transform.position = cursorPosition;
            ghostBuilding.RotationIndex = rotationIndex;
            currentPlacementLogic.TrackBuildingToCursor(this, buildingManager, ghostBuilding);
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
            ghostBuilding.RotationIndex = rotationIndex;
            ghostBuilding.transform.SetParent(this.transform);
        }
    }

    public void HandleRotation()
    {
        if (Input.GetKeyDown(rotateKey))
        {
            rotationIndex = (rotationIndex + 1) % 4;
            ghostBuilding.RotationIndex = rotationIndex;
            updated = false;
        }
    }

    public void HandleDeselection()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonUp(1))
        {
            DeselectBuilding();
            buildingManager.ResetTile(buildingManager.overlayLayer, cursorPosition);
            updated = false;
        }
    }

    public void HandlePlacement()
    {
        if (Input.GetMouseButton(0))
        {
            currentPlacementLogic.PlaceBuilding(this, buildingManager, ghostBuilding);
            updated = false;
        }
    }

    public void HandleRemove()
    {
        if (Input.GetMouseButton(1) && ghostBuilding == null)
        {
            currentPlacementLogic.RemoveBuilding(buildingManager, GetCursorPosition());
            updated = false;
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
        foreach (
            BuildingDatabase.BuildingGroupHotkey groupHotkey in buildingManager
                .buildingDatabase
                .groupHotkeys
        )
        {
            if (Input.GetKeyDown(groupHotkey.hotkey))
            {
                updated = false;
                List<BuildingDatabase.BuildingData> buildingsInGroup =
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

    public void ChangePlacementStrategy(BuildingDatabase.BuildingData buildingData)
    {
        BuildingDatabase.BuildingGroup group = buildingData.group;
        if (placementLogicByGroup.ContainsKey(group))
        {
            currentPlacementLogic = placementLogicByGroup[group];
        }
        else
        {
            Debug.LogWarning(
                $"No placement strategy defined for group {group}. Using default strategy."
            );
            currentPlacementLogic = placementLogicByGroup[BuildingDatabase.BuildingGroup.None];
        }
    }

    // Other methods and functionality...
}
