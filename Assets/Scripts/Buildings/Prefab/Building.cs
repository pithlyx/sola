using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct Process
{
    public string input;
    public string output;
    public float rate;
    public int stock;
}


public class Building : MonoBehaviour
{
    public Vector3Int Position; // Vector3Int is used by the Tilemap system for positions
    public bool Active = false;
    public KeyCode Hotkey; // KeyCode is used by the Input system for key presses
    public TileBase Sprite; // TileBase for the sprite
    public TileBase Anim; // TileBase for the animated sprite
    public Process Process;

    public int rotationIndex = 0; // Stores current rotation index
    public Matrix4x4 rotationMatrix = Matrix4x4.identity; // Stores current rotation matrix


    public void ChangeState()
    {
        Active = !Active;
    }

    public TileBase GetState()
    {
        TileBase activeTile = Active ? Anim : Sprite;
        return activeTile;
    }

    public void Rotate(int? rotationIndex = null)
    {
        // If a specific rotation index is specified, use it. Otherwise, increment the current rotation index
        this.rotationIndex = rotationIndex ?? (this.rotationIndex + 1) % 4;

        // Calculate the rotation angle in degrees
        float rotationAngle = -90 * this.rotationIndex;

        // Create a rotation matrix for the calculated angle
        this.rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotationAngle));
    }
}