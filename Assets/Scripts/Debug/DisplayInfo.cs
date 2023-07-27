using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEditor;

public class DisplayInfo : MonoBehaviour
{
    public TextMeshProUGUI infoText; // Drag your TextMeshProUGUI component here in Unity Editor
    public FastNoiseSIMDUnity noiseObject; // Drag your FastNoiseSIMDUnity component here in Unity Editor

    void Update()
    {
        // Convert mouse position to world position
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Convert world position to grid coordinates
        Vector3Int mouseGridPos = new Vector3Int(
            Mathf.RoundToInt(mouseWorldPos.x),
            Mathf.RoundToInt(mouseWorldPos.y),
            0
        );

        // Get noise value at mouse position
        float[] mouseNoiseSet = noiseObject.fastNoiseSIMD.GetNoiseSet(
            mouseGridPos.x,
            mouseGridPos.y,
            0,
            1,
            1,
            1
        );

        // Convert camera position to grid coordinates
        Vector3Int cameraGridPos = new Vector3Int(
            Mathf.RoundToInt(Camera.main.transform.position.x),
            Mathf.RoundToInt(Camera.main.transform.position.y),
            0
        );

        // Get noise value at camera position
        float[] cameraNoiseSet = noiseObject.fastNoiseSIMD.GetNoiseSet(
            cameraGridPos.x,
            cameraGridPos.y,
            0,
            1,
            1,
            1
        );

        // Set the text to display the grid coordinates and the noise value
        infoText.text =
            @$"Mouse:
    Global - X:{mouseGridPos.x}|Y:{mouseGridPos.y}
    Chunk - X:{Mathf.FloorToInt(mouseGridPos.x / 16f)}|Y:{Mathf.FloorToInt(mouseGridPos.y / 16f)}
    Noise - {mouseNoiseSet[0]}
Camera:
    Global - X:{cameraGridPos.x}|Y:{cameraGridPos.y}
    Chunk - X:{Mathf.FloorToInt(cameraGridPos.x / 16f)}|Y:{Mathf.FloorToInt(cameraGridPos.y / 16f)}
    Noise - {cameraNoiseSet[0]}";
    }
}
