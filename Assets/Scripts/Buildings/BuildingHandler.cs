using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BuildingHandler : MonoBehaviour
{
    public Tilemap overlayTilemap;
    public Tilemap buildingsTilemap;

    [SerializeField]
    private List<BuildingGroup> buildingGroups;

    [SerializeField]
    private KeyCode RotateHotkey = KeyCode.R;

    [SerializeField]
    private KeyCode PickerHotkey = KeyCode.Q;

    private Building selectedBuilding;
    private Vector3Int lastMousePosition;
    private Vector3Int lastPlacementPosition = new Vector3Int(
        int.MinValue,
        int.MinValue,
        int.MinValue
    );
    private BuildingManager buildingManager;
    private HistoryManager historyManager;
    private BatchCommand currentBatchCommand;
    private int relativeRotationIndex = 0;
    private Dictionary<string, int> buildingCounts = new Dictionary<string, int>();

    private void Awake()
    {
        buildingManager = GetComponent<BuildingManager>();
        historyManager = GetComponent<HistoryManager>();
    }

    private void Update()
    {
        HandleHotKeys();
        TrackOverlay();
        HandleBuildingPlacement();
        HandleBuildingRemoval();
        HandleBatchCommandEnd();
        HandleUndoAction();
    }

    private void HandleHotKeys()
    {
        SelectBuildingGroup();
        RotateBuilding();
        PickBuilding();
    }

    private void HandleBuildingPlacement()
    {
        StartBatchCommandIfApplicable();
        PlaceBuilding();
    }

    private void HandleBuildingRemoval()
    {
        StartBatchCommandIfApplicable();
        RemoveBuilding();
    }

    private void SelectBuildingGroup()
    {
        foreach (BuildingGroup buildingGroup in buildingGroups)
        {
            if (Input.GetKeyDown(buildingGroup.Hotkey))
            {
                if (selectedBuilding != null && buildingGroup.Buildings.Contains(selectedBuilding))
                {
                    buildingGroup.NextBuilding();
                }
                selectedBuilding = buildingGroup.GetCurrentBuilding();
                selectedBuilding.Rotate(relativeRotationIndex);
            }
        }
    }

    private void RotateBuilding()
    {
        if (selectedBuilding != null && Input.GetKeyDown(RotateHotkey))
        {
            relativeRotationIndex = (relativeRotationIndex + 1) % 4;
            selectedBuilding.Rotate(relativeRotationIndex);
            overlayTilemap.SetTransformMatrix(lastMousePosition, selectedBuilding.rotationMatrix);
        }
    }

    private void TrackOverlay()
    {
        if (selectedBuilding == null)
            return;

        Vector3Int mousePosition = GetMousePosition();
        if (mousePosition != lastMousePosition)
        {
            overlayTilemap.SetTile(lastMousePosition, null);
            overlayTilemap.SetTransformMatrix(lastMousePosition, Matrix4x4.identity);
        }

        TileBase activeTile = selectedBuilding.GetState();
        overlayTilemap.SetTile(mousePosition, activeTile);
        overlayTilemap.SetTransformMatrix(mousePosition, selectedBuilding.rotationMatrix);
        lastMousePosition = mousePosition;

        if (Input.GetMouseButtonDown(1))
        {
            ClearSelected();
        }
    }

    private void ClearSelected()
    {
        if (selectedBuilding == null)
            return;

        Vector3Int mousePosition = GetMousePosition();
        overlayTilemap.SetTile(mousePosition, null);
        selectedBuilding = null;
        relativeRotationIndex = 0;
    }

    private void StartBatchCommandIfApplicable()
    {
        if (
            currentBatchCommand == null
            && (
                (selectedBuilding != null && Input.GetMouseButtonDown(0))
                || (selectedBuilding == null && Input.GetMouseButtonDown(1))
            )
        )
        {
            currentBatchCommand = new BatchCommand();
        }
    }

    private void PlaceBuilding()
    {
        if (selectedBuilding == null || !Input.GetMouseButton(0))
            return;

        Vector3Int mousePosition = GetMousePosition();
        if (
            mousePosition == lastPlacementPosition
            || buildingManager.GetBuildingInfo(mousePosition).Instance != null
        )
            return;

        lastPlacementPosition = mousePosition;

        TileBase activeTile = selectedBuilding.GetState();
        buildingsTilemap.SetTile(mousePosition, activeTile);
        buildingsTilemap.SetTransformMatrix(mousePosition, selectedBuilding.rotationMatrix);

        Transform containerTransform = transform.Find($"{selectedBuilding.name}s");
        if (containerTransform == null)
        {
            GameObject newContainer = new GameObject($"{selectedBuilding.name}s");
            newContainer.transform.parent = transform;
            containerTransform = newContainer.transform;
        }

        int buildingId = buildingCounts.TryGetValue(selectedBuilding.name, out int count)
            ? count
            : 0;
        buildingCounts[selectedBuilding.name] = buildingId + 1;

        Vector3 placementPosition = buildingsTilemap.GetCellCenterWorld(mousePosition);
        Building newBuilding = Instantiate(
            selectedBuilding,
            placementPosition,
            Quaternion.Euler(0, 0, -90 * relativeRotationIndex),
            containerTransform
        );
        newBuilding.name = $"{selectedBuilding.name}{buildingId}";

        buildingManager.AddBuilding(
            mousePosition,
            selectedBuilding,
            newBuilding.gameObject,
            buildingId
        );

        if (currentBatchCommand != null)
        {
            currentBatchCommand.AddCommand(
                new AddBuildingCommand(
                    mousePosition,
                    selectedBuilding,
                    newBuilding.gameObject,
                    buildingManager,
                    buildingId,
                    buildingsTilemap
                )
            );
        }
    }

    private void RemoveBuilding()
    {
        if (selectedBuilding != null || !Input.GetMouseButton(1))
            return;

        Vector3Int mousePosition = GetMousePosition();
        BuildingManager.BuildingInfo buildingInfo = buildingManager.GetBuildingInfo(mousePosition);
        if (buildingInfo.Instance == null)
            return;

        Destroy(buildingInfo.Instance);
        buildingsTilemap.SetTile(mousePosition, null);
        buildingManager.RemoveBuilding(mousePosition);

        if (currentBatchCommand != null)
        {
            currentBatchCommand.AddCommand(
                new RemoveBuildingCommand(
                    mousePosition,
                    buildingInfo,
                    buildingsTilemap,
                    buildingManager
                )
            );
        }
    }

    private void HandleBatchCommandEnd()
    {
        if (
            (selectedBuilding != null && Input.GetMouseButtonUp(0))
            || (selectedBuilding == null && Input.GetMouseButtonUp(1))
        )
        {
            if (currentBatchCommand != null)
            {
                historyManager.AddAndExecute(currentBatchCommand);
                currentBatchCommand = null;
            }
        }
    }

    private void HandleUndoAction()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            historyManager.Undo();
        }
    }

    private void PickBuilding()
    {
        if (!Input.GetKeyDown(PickerHotkey))
            return;

        Vector3Int mousePosition = GetMousePosition();
        TileBase tileUnderMouse = buildingsTilemap.GetTile(mousePosition);
        if (tileUnderMouse == null)
            return;

        foreach (BuildingGroup buildingGroup in buildingGroups)
        {
            Building building = buildingGroup.GetBuildingByTileBase(tileUnderMouse);
            if (building == null)
                continue;

            // Get the rotation index from the building that's already placed on the map
            BuildingManager.BuildingInfo buildingInfo = buildingManager.GetBuildingInfo(
                mousePosition
            );
            Building placedBuilding = buildingInfo.Instance.GetComponent<Building>();
            int rotationIndex = placedBuilding.RotationIndex;

            // Set the selected building
            selectedBuilding = buildingGroup.SelectBuilding(building);
            selectedBuilding.Rotate(rotationIndex);
            relativeRotationIndex = rotationIndex;
            break;
        }
    }

    private Vector3Int GetMousePosition()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return overlayTilemap.WorldToCell(worldPosition);
    }
}
