using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

public class BuildingHandler : MonoBehaviour
{
    private BuildingManager buildingManager; // Reference to the BuildingManager
    private IPlacementLogic currentPlacementLogic; // Reference to the current placement logic
    public Vector2 CursorPosition; // Public variable to store the cursor position
    public BuildingDatabase.BuildingData selectedBuilding; // Public variable to store the currently selected building
    public Building selectedBuildingObject; // Public variable to store the currently selected building GameObject

    private void Start()
    {
        buildingManager = GetComponent<BuildingManager>(); // Get the BuildingManager component
        if (buildingManager == null) // Check if BuildingManager was found
            Debug.LogError("No BuildingManager found on the same game object");
        currentPlacementLogic = new DefaultPlacementLogic(); // Set the default placement logic
    }

    private void Update()
    {
        // Update the cursor position
        CursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Convert the cursor position to a Vector3Int
        Vector3Int cursorPos = Vector3Int.FloorToInt(CursorPosition);

        // Display the currently selected building at the cursor position
        currentPlacementLogic.DisplayBuildingAtCursor(
            this,
            cursorPos,
            buildingManager.buildingDatabase,
            buildingManager.overlayLayer
        );

        // Listen for each building group's hotkey
        foreach (
            BuildingDatabase.BuildingGroupHotkey groupHotkey in buildingManager
                .buildingDatabase
                .groupHotkeys
        )
        {
            if (Input.GetKeyDown(groupHotkey.hotkey))
            {
                // If the hotkey is pressed, select the next building in the group
                SelectBuilding(groupHotkey.group);
            }
        }
    }

    public void SelectBuilding(BuildingDatabase.BuildingGroup group)
    {
        List<BuildingDatabase.BuildingData> buildingsInGroup =
            buildingManager.buildingDatabase.GetBuildingsByGroup(group); // Get the buildings in the group

        // Check if there are any buildings in the group
        if (buildingsInGroup.Count == 0)
        {
            Debug.Log("No buildings in the selected group");
            return;
        }

        int currentIndex = buildingsInGroup.IndexOf(selectedBuilding); // Find the index of the selected building
        int nextIndex = (currentIndex + 1) % buildingsInGroup.Count; // Calculate the index of the next building

        if (buildingsInGroup[nextIndex].name == selectedBuilding.name) // Check if the next building is the same
        {
            Debug.Log("No New Building");
            return;
        }

        if (selectedBuildingObject != null) // Check if there is a selected building GameObject
            Destroy(selectedBuildingObject.gameObject); // Destroy the old selected building GameObject

        selectedBuilding = buildingsInGroup[nextIndex]; // Select the next building
        selectedBuildingObject = buildingManager.CreateBuildingGameObject(
            selectedBuilding,
            nextIndex
        ); // Create a new GameObject for the selected building
        Debug.Log("Selected building: " + selectedBuilding.name); // Log the selected building
    }

    // public void SetTileWithRotation(
    //     Vector3Int position,
    //     TileBase tileBase,
    //     Matrix4x4 rotationMatrix
    // )
    // {
    //     // Create a new tilebase from the tileBase
    //     TileBase tile = Instantiate(tileBase) as Tile;

    //     if (tile == null)
    //     {
    //         Debug.LogError("Failed to create a new tile from the provided tileBase");
    //         return;
    //     }

    //     // Apply the rotation to the tile's transform
    //     tile.transform = rotationMatrix;

    //     // Set the tile at the position on the overlay layer
    //     buildingManager.overlayLayer.SetTile(position, tile);
    // }
}
