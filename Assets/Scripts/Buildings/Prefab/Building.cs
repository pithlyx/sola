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
    public bool Active = false; // Whether the building is active or not
    public TileBase Sprite; // TileBase for the sprite
    public TileBase Anim; // TileBase for the animated sprite
    public Process Process;

    public int defaultRotationIndex = 0; // Add this line to store the default rotation index
    public int RotationIndex; // Stores the current rotation index

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

    public void Rotate(int relativeRotationIndex)
    {
        // Calculate the rotation angle in degrees, taking into account the default and relative rotation
        float rotationAngle = -90 * this.defaultRotationIndex - 90 * relativeRotationIndex;

        // Create a rotation matrix for the calculated angle
        this.rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotationAngle));

        // Set the RotationIndex property
        this.RotationIndex = relativeRotationIndex;
    }
}
