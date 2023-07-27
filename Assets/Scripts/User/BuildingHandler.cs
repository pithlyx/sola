using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingHandler : MonoBehaviour
{
    // set the maximum undo steps
    public int maxUndoSteps = 100;

    // reference to the tilemaps
    public Tilemap buildings;
    public Tilemap overlay;

    // keep track of the selected tile
    public TileBase selectedTile;

    // keep track of last position
    private Vector3Int lastPosition;
    private Vector3Int newPosition;

    // keep track of the rotation
    private Matrix4x4 rotationMatrix = Matrix4x4.identity;

    // keep track of the placed tiles for undo
    private LinkedList<List<Vector3Int>> undoHistory = new LinkedList<List<Vector3Int>>();
    private List<Vector3Int> currentPlacedTiles = new List<Vector3Int>();

    void Update()
    {
        if (selectedTile != null)
        {
            Vector3Int newPosition = overlay.WorldToCell(
                Camera.main.ScreenToWorldPoint(Input.mousePosition)
            );
            // if the mouse has moved update the position of the new tile and remove the last position tile
            if (newPosition != lastPosition)
            {
                overlay.SetTile(newPosition, selectedTile);
                overlay.SetTransformMatrix(newPosition, rotationMatrix);
                overlay.SetTile(lastPosition, null);
                lastPosition = newPosition;
            }
            // if the user presses r, rotate the tile
            if (Input.GetKeyDown(KeyCode.R))
            {
                rotationMatrix *= Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90));
                overlay.SetTransformMatrix(newPosition, rotationMatrix);
            }
            // if the user right clicks, deselect the current tile
            if (Input.GetMouseButtonUp(1))
            {
                overlay.SetTile(newPosition, null);
                selectedTile = null;
            }
            if (Input.GetMouseButtonDown(0))
            {
                // Start a new operation
                currentPlacedTiles = new List<Vector3Int>();
            }
            if (Input.GetMouseButton(0))
            {
                // Place the tile from the overlay onto the building tilemap
                Vector3Int position = overlay.WorldToCell(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition)
                );
                buildings.SetTile(position, selectedTile);
                buildings.SetTransformMatrix(position, rotationMatrix);

                // Add the position to the current operation
                currentPlacedTiles.Add(position);
            }
            if (Input.GetMouseButtonUp(0))
            {
                // End the current operation and add it to the undo history
                undoHistory.AddLast(currentPlacedTiles);

                // If the undo history is too large, remove the oldest operation
                while (undoHistory.Count > maxUndoSteps)
                {
                    undoHistory.RemoveFirst();
                }
            }
        }
        else
        {
            // if the user presses right click and there is a tile in that position, remove it
            if (Input.GetMouseButton(1))
            {
                Vector3Int position = buildings.WorldToCell(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition)
                );
                buildings.SetTile(position, null);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Select a tile from the building tilemap
            Vector3Int selectedPosition = buildings.WorldToCell(
                Camera.main.ScreenToWorldPoint(Input.mousePosition)
            );
            selectedTile = buildings.GetTile(selectedPosition);

            // Fetch the transform matrix of the selected tile
            if (selectedTile != null)
            {
                rotationMatrix = buildings.GetTransformMatrix(selectedPosition);
            }

            // add tile to overlay tilemap
            overlay.SetTile(
                overlay.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)),
                selectedTile
            );
            overlay.SetTransformMatrix(newPosition, rotationMatrix);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Undo the last operation
            if (undoHistory.Count > 0)
            {
                List<Vector3Int> lastOperation = undoHistory.Last.Value;
                undoHistory.RemoveLast();
                foreach (Vector3Int pos in lastOperation)
                {
                    buildings.SetTile(pos, null);
                }
            }
        }
    }
}
