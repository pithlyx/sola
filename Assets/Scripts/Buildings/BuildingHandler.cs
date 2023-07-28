using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;


public class BuildingHandler : MonoBehaviour
{
    [SerializeField]
    private Tilemap overlayTilemap;

    [SerializeField]
    private Tilemap buildingsTilemap;

    [SerializeField]
    private Building[] buildingPrefabs;

    [SerializeField]
    private KeyCode RotateHotkey = KeyCode.R;  // Assign the hotkey for rotation

    private Building selectedBuilding;
    private Vector3Int lastMousePosition;
    private Vector3Int lastPlacementPosition = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    private Dictionary<string, int> buildingCounts = new Dictionary<string, int>();
    private BuildingManager buildingManager;

    private HistoryManager historyManager;
    private BatchCommand currentBatchCommand;



    private void Awake()
    {
        buildingManager = GetComponent<BuildingManager>();
        historyManager = GetComponent<HistoryManager>();
    }

    private void Update()
    {
        SelectBuilding();
        RotateBuilding();
        TrackOverlay();
        StartBatchCommand();
        PlaceBuilding();
        RemoveBuilding();
        EndBatchCommand();
        UndoAction();
    }


    private void SelectBuilding()
    {
        foreach (Building building in buildingPrefabs)
        {
            if (Input.GetKeyDown(building.Hotkey))
            {
                selectedBuilding = building;
                selectedBuilding.rotationIndex = 0; // Reset the rotation index
                selectedBuilding.rotationMatrix = Matrix4x4.identity; // Reset the rotation matrix
            }
        }
    }

    private void TrackOverlay()
    {
        if (selectedBuilding != null)
        {
            Vector3Int mousePosition = GetMousePosition();

            // Clear the tile at the last mouse position
            if (mousePosition != lastMousePosition)
            {
                overlayTilemap.SetTile(lastMousePosition, null);
                overlayTilemap.SetTransformMatrix(lastMousePosition, Matrix4x4.identity); // Clear transform matrix
            }

            // Set the overlay tile at the current mouse position
            TileBase activeTile = selectedBuilding.GetState();
            overlayTilemap.SetTile(mousePosition, activeTile);
            overlayTilemap.SetTransformMatrix(mousePosition, selectedBuilding.rotationMatrix);

            lastMousePosition = mousePosition;
            if (Input.GetMouseButtonDown(1))
            {
                ClearSelected();
            }
        }
    }

    private void RotateBuilding()
    {
        if (selectedBuilding != null && Input.GetKeyDown(RotateHotkey))
        {
            selectedBuilding.Rotate();
            overlayTilemap.SetTransformMatrix(lastMousePosition, selectedBuilding.rotationMatrix); // Apply transform matrix
        }
    }

    private void ClearSelected()
    {
        if (selectedBuilding != null)
        {
            // Clear the overlay tilemap at the current mouse position
            Vector3Int mousePosition = GetMousePosition();
            overlayTilemap.SetTile(mousePosition, null);
            selectedBuilding = null; // Deselect the current building
        }
    }

    private void StartBatchCommand()
    {
        if ((selectedBuilding != null && Input.GetMouseButtonDown(0)) || (selectedBuilding == null && Input.GetMouseButtonDown(1)))
        {
            currentBatchCommand = new BatchCommand();
        }
    }

    private void PlaceBuilding()
    {
        if (selectedBuilding != null && Input.GetMouseButton(0))
        {
            Vector3Int mousePosition = GetMousePosition();
            if (mousePosition != lastPlacementPosition && buildingManager.GetBuildingInfo(mousePosition).Instance == null)
            {
                lastPlacementPosition = mousePosition;

                TileBase activeTile = selectedBuilding.GetState();
                buildingsTilemap.SetTile(mousePosition, activeTile);
                buildingsTilemap.SetTransformMatrix(mousePosition, selectedBuilding.rotationMatrix);


                Transform containerTransform = transform.Find(selectedBuilding.name + "s");
                if (containerTransform == null)
                {
                    GameObject newContainer = new GameObject(selectedBuilding.name + "s");
                    newContainer.transform.parent = transform;
                    containerTransform = newContainer.transform;
                }

                if (!buildingCounts.ContainsKey(selectedBuilding.name))
                {
                    buildingCounts[selectedBuilding.name] = 0;
                }
                int buildingId = buildingCounts[selectedBuilding.name]++;

                Building newBuilding = Instantiate(selectedBuilding, mousePosition, Quaternion.identity, containerTransform);
                newBuilding.name = $"{selectedBuilding.name}{buildingId}";

                buildingManager.AddBuilding(mousePosition, selectedBuilding, newBuilding.gameObject, buildingId);

                if (currentBatchCommand != null)
                {
                    currentBatchCommand.AddCommand(new AddBuildingCommand(mousePosition, selectedBuilding, newBuilding.gameObject, buildingManager, buildingId, buildingsTilemap));
                }
            }
        }
    }

    private void RemoveBuilding()
    {
        if (selectedBuilding == null && Input.GetMouseButton(1))
        {
            Vector3Int mousePosition = GetMousePosition();
            BuildingManager.BuildingInfo buildingInfo = buildingManager.GetBuildingInfo(mousePosition);
            if (buildingInfo.Instance != null)
            {
                Destroy(buildingInfo.Instance);
                buildingsTilemap.SetTile(mousePosition, null);
                buildingManager.RemoveBuilding(mousePosition);

                if (currentBatchCommand != null)
                {
                    currentBatchCommand.AddCommand(new RemoveBuildingCommand(mousePosition, buildingInfo, buildingsTilemap, buildingManager));
                }
            }
        }
    }

    private void EndBatchCommand()
    {
        if ((selectedBuilding != null && Input.GetMouseButtonUp(0)) || (selectedBuilding == null && Input.GetMouseButtonUp(1)))
        {
            if (currentBatchCommand != null)
            {
                historyManager.AddAndExecute(currentBatchCommand);
                currentBatchCommand = null;
            }
        }
    }

    private void UndoAction()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            historyManager.Undo();
        }
    }

    private Vector3Int GetMousePosition()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return overlayTilemap.WorldToCell(worldPosition);
    }
}

