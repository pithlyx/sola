using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ResourceDatabase", menuName = "Database/ResourceDatabase")]
public class ResourceDatabase : ScriptableObject
{
    public List<Resource> resources;

    private Dictionary<int, List<(float, TileBase)>> layerToThresholds;

    private void OnEnable()
    {
        // Create the dictionary mapping layers to sorted lists of thresholds
        layerToThresholds = new Dictionary<int, List<(float, TileBase)>>();

        foreach (var resource in resources)
        {
            foreach (var threshold in resource.thresholds)
            {
                if (!layerToThresholds.ContainsKey(threshold.Layer))
                {
                    layerToThresholds[threshold.Layer] = new List<(float, TileBase)>();
                }
                layerToThresholds[threshold.Layer].Add((threshold.Threshold, resource.tile));
            }
        }

        // Sort each list of thresholds by the threshold value
        foreach (var thresholds in layerToThresholds.Values)
        {
            thresholds.Sort();
        }
    }

    public TileBase GetTileForNoise(int layer, float noise)
    {
        if (!layerToThresholds.ContainsKey(layer))
            return null;

        var thresholds = layerToThresholds[layer];
        for (int i = thresholds.Count - 1; i >= 0; i--)
        {
            if (noise >= thresholds[i].Item1)
            {
                return thresholds[i].Item2;
            }
        }

        return null;
    }

    public Resource GetResourceForNoise(int layer, float noise)
    {
        if (!layerToThresholds.ContainsKey(layer))
            return null;

        var thresholds = layerToThresholds[layer];
        for (int i = thresholds.Count - 1; i >= 0; i--)
        {
            if (noise >= thresholds[i].Item1)
            {
                // Find the resource with the matching tile
                Resource resource = resources.FirstOrDefault(r => r.tile == thresholds[i].Item2);
                return resource;
            }
        }

        return null;
    }
}
