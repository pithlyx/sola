using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DebugNoise : MonoBehaviour
{
    public FastNoiseSIMDUnity noiseGenerator;
    public TextMeshProUGUI debugText;

    void Update()
    {
        // Convert mouse position to world coordinates
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Generate noise for the mouse position
        int x = Mathf.RoundToInt(mousePosition.x);
        int y = Mathf.RoundToInt(mousePosition.y);
        List<float> noiseSet = noiseGenerator.GetNoiseSet(x, y, 0, 1, 1, 1); // Assuming that your noise function takes (xStart, yStart, zStart, xSize, ySize, zSize)

        // Get the index of the resource for the current noise value
        int index0 = ResourceDatabase.Instance.NoiseToIndex(LayerName.Terrain, noiseSet[0]); // Assuming that the layer name is "Resource"
        Resource resource0 = ResourceDatabase.Instance.IndexToResource(index0);
        // get the index of the resource for the current noise value using the second method
        List<int> index1 = ResourceDatabase.Instance.NoiseToIndices(noiseSet, LayerName.Terrain); // Assuming that the layer name is "Resource"
        List<Resource> resource1 = ResourceDatabase.Instance.IndicesToResources(index1);
        // Update the debug text
        debugText.text =
            $"Noise: {noiseSet[0]}\nResource\n  Method 1: {index0} | {(resource0 != null ? resource0.resourceName : "N/A")}\n  Method 2: {index1[0]} | {(resource1[0] != null ? resource1[0].resourceName : "N/A")}"; // Get the first (and only) value from the noise set
    }
}
