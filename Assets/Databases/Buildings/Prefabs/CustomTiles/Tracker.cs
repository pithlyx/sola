using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTracker : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap component assigned in the Inspector
    public TileBase currentTileBase; // Variable to store the selected TileBase on the tilemap

    public int rotationIndex; // Public integer to store the rotation index (you can set this value in the Inspector)

    private Vector3Int previousTilePosition;

    void Update()
    {
        Vector3Int? newTilePosition = GetNewTilePositionUnderCursor();

        // Check if the cursor has moved to a new tile
        if (newTilePosition.HasValue)
        {
            // Get the TileBase at the new tile position
            currentTileBase = GetTile(newTilePosition.Value);

            // Log something to the console
            Debug.Log("Cursor moved to a new tile: " + newTilePosition);

            // Update the previousTilePosition to the current position for the next frame
            previousTilePosition = newTilePosition.Value;
        }

        // Check for the hotkey press and update the rotation index
        if (Input.GetKeyDown(KeyCode.R))
        {
            Rotate();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DebugRotationIndex();
        }
    }

    private Vector3Int? GetNewTilePositionUnderCursor()
    {
        Vector3Int currentTilePosition = tilemap.WorldToCell(
            Camera.main.ScreenToWorldPoint(Input.mousePosition)
        );

        // Check if the cursor has moved to a new tile
        if (currentTilePosition != previousTilePosition)
        {
            return currentTilePosition;
        }

        // Cursor hasn't moved to a new tile, return null
        return null;
    }

    private TileBase GetTile(Vector3Int position)
    {
        // Get the Tile at the specified position on the tilemap
        return tilemap.GetTile(position);
    }

    // Rotate the tiles based on the rotation index
    private void Rotate()
    {
        rotationIndex = (rotationIndex + 1) % 4; // Increment the rotation index and wrap around 0 to 3 (0, 1, 2, 3).
        Quaternion rotation = Quaternion.Euler(0, 0, rotationIndex * -90f);

        TileBase tile = tilemap.GetTile(previousTilePosition);
        if (tile != null)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            tilemap.SetTile(previousTilePosition, tile);
            tilemap.SetTransformMatrix(previousTilePosition, matrix);
        }
    }

    private void DebugRotationIndex()
    {
        // Get the tile at the current cursor position
        Vector3Int currentTilePosition = tilemap.WorldToCell(
            Camera.main.ScreenToWorldPoint(Input.mousePosition)
        );
        TileBase tile = tilemap.GetTile(currentTilePosition);

        if (tile != null)
        {
            // Get the tile's current rotation matrix
            Matrix4x4 matrix = tilemap.GetTransformMatrix(currentTilePosition);
            Quaternion rotation = matrix.rotation;

            // Convert the rotation to an angle in degrees (0 to 360)
            float rotationAngle = rotation.eulerAngles.z;
            if (rotationAngle < 0)
            {
                rotationAngle += 360f;
            }

            // Calculate the rotation index based on 90-degree offsets
            int rotationIndex = Mathf.RoundToInt(rotationAngle / 90f) % 4;

            // Output the rotation index to the console
            Debug.Log("Rotation Index of Hovered Tile: " + rotationIndex);
        }
    }
}
