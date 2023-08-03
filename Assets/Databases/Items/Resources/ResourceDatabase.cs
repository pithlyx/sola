using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct Threshold
{
    public int mapLayer;
    public float threshold;
}

[System.Serializable]
public struct ResourceConfig
{
    public Resource resource;
    public List<Threshold> thresholds;
    public int index; // Add an index field
}

[System.Serializable]
public struct ThresholdResourcePair
{
    public float Threshold;
    public Resource Resource;

    public ThresholdResourcePair(float threshold, Resource resource)
    {
        Threshold = threshold;
        Resource = resource;
    }
}

[CreateAssetMenu(fileName = "New Resource Database", menuName = "Database/Resource Database")]
public class ResourceDatabase : ScriptableObject
{
    public List<ResourceConfig> allResources;

    // Add a mapping from Resource to index
    public Dictionary<Resource, int> resourceToIndex = new Dictionary<Resource, int>();

    // Pre-sorted thresholds
    private Dictionary<int, List<ThresholdResourcePair>> sortedThresholds =
        new Dictionary<int, List<ThresholdResourcePair>>();

    private void OnEnable()
    {
        // Populate the resourceToIndex dictionary
        for (int i = 0; i < allResources.Count; i++)
        {
            resourceToIndex[allResources[i].resource] = i;
        }

        // Sort thresholds on enable
        foreach (var resource in allResources)
        {
            foreach (var threshold in resource.thresholds)
            {
                if (!sortedThresholds.ContainsKey(threshold.mapLayer))
                {
                    sortedThresholds[threshold.mapLayer] = new List<ThresholdResourcePair>();
                }

                sortedThresholds[threshold.mapLayer].Add(
                    new ThresholdResourcePair(threshold.threshold, resource.resource)
                );
            }
        }

        // Sort each list of thresholds
        foreach (var pairList in sortedThresholds.Values)
        {
            pairList.Sort((pair1, pair2) => pair1.Threshold.CompareTo(pair2.Threshold));
        }
    }

    public int[] MapResourcesToNoise(float[] noise, int layer)
    {
        // Check if there are thresholds for the given layer
        if (!sortedThresholds.ContainsKey(layer))
        {
            Debug.LogError("No thresholds found for layer " + layer);
            return null;
        }

        // Get the thresholds for the layer
        var thresholds = sortedThresholds[layer];

        // Create a new array for the resource indexes
        int[] resourceIndexes = new int[noise.Length];

        // Loop over all noise values
        for (int i = 0; i < noise.Length; i++)
        {
            // Initialize with a default value
            resourceIndexes[i] = -1;

            // Loop over all thresholds
            for (int j = 0; j < thresholds.Count; j++)
            {
                // If the noise value is less than the threshold, assign the resource index
                if (noise[i] < thresholds[j].Threshold)
                {
                    Debug.Log("Assigning (" + i + "," + j + ") to index " + i);
                    resourceIndexes[i] = resourceToIndex[thresholds[j].Resource];
                    break;
                }
            }
        }

        return resourceIndexes;
    }

    public List<Resource> GetResourcesFromIndexes(int[] indexes)
    {
        // Create a new list for the resources
        List<Resource> resources = new List<Resource>(indexes.Length);

        // Loop over all indexes
        for (int i = 0; i < indexes.Length; i++)
        {
            // Check if the index is valid
            if (indexes[i] >= 0 && indexes[i] < allResources.Count)
            {
                // Get the resource from the database and add it to the list
                resources.Add(allResources[indexes[i]].resource);
            }
        }

        return resources;
    }
}
