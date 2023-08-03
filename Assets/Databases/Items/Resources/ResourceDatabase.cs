using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct LayerConfig
{
    public LayerName layerName;
    public List<float> minThresholds;
}

[System.Serializable]
public struct ResourceConfig
{
    public string resourceName;
    public Resource resource;
    public bool canHarvest;
    public List<LayerConfig> layerConfigs;
}

[CreateAssetMenu(fileName = "New Layer Database", menuName = "Database/Layer Database")]
public class ResourceDatabase : ScriptableObject
{
    public List<ResourceConfig> resourceConfigs;
    public List<Resource> resources = new List<Resource>();
    public Dictionary<LayerName, SortedDictionary<float, int>> layers =
        new Dictionary<LayerName, SortedDictionary<float, int>>();

    // The instance of the ResourceDatabase
    public static ResourceDatabase Instance { get; set; }

    public void PopulateData()
    {
        // Clear existing data
        layers.Clear();

        // Iterate over all resource configs
        for (int i = 0; i < resourceConfigs.Count; i++)
        {
            // Add the resource to the list of resources
            resources.Add(resourceConfigs[i].resource);

            // Iterate over all layer configs of the current resource config
            foreach (var layerConfig in resourceConfigs[i].layerConfigs)
            {
                // If the layer doesn't exist in the dictionary yet, add it
                if (!layers.ContainsKey(layerConfig.layerName))
                {
                    layers[layerConfig.layerName] = new SortedDictionary<float, int>();
                }

                // Iterate over all minThresholds of the current layer config
                foreach (var minThreshold in layerConfig.minThresholds)
                {
                    // Add the minThreshold and the index of the current resource config to the dictionary
                    layers[layerConfig.layerName][minThreshold] = i;
                }
            }
        }
    }

    public List<float> GetNoise(FastNoiseSIMD noiseGenerator, Vector3Int origin, int chunkSize)
    {
        // Generate the noise for the chunk
        List<float> noiseSet = noiseGenerator
            .GetNoiseSet(origin.x, origin.y, origin.z, chunkSize, chunkSize, 1)
            .ToList();
        return noiseSet;
    }

    public int NoiseToIndex(LayerName layerName, float noise)
    {
        // Check if the layers dictionary contains the layerName
        if (!layers.ContainsKey(layerName))
        {
            // Return -1 to indicate that the layerName was not found
            return -1;
        }

        // Get the dictionary of thresholds and indices for the layer
        var thresholds = layers[layerName];

        // Perform a binary search for the noise value
        var thresholdKeys = thresholds.Keys.ToList();
        int index = thresholdKeys.BinarySearch(noise);

        // If the exact noise value is not found, BinarySearch returns a negative number
        // that is the bitwise complement of the next larger element
        if (index < 0)
        {
            index = ~index - 1;
        }

        // Check if a suitable threshold was found
        if (index >= 0)
        {
            // Return the index associated with the threshold
            return thresholds[thresholdKeys[index]];
        }

        // Return -1 to indicate that no suitable threshold was found
        return -1;
    }

    // When calling the NoiseToIndices method, returns a list of resource indices (or -1 if a layer name is not found).
    public List<int> NoiseToIndices(LayerName layerName, List<float> noises)
    {
        // Check if the layers dictionary contains the layerName
        if (!layers.ContainsKey(layerName))
        {
            // Return a list of -1s to indicate that the layerName was not found
            return Enumerable.Repeat(-1, noises.Count).ToList();
        }

        // Get the dictionary of thresholds and indices for the layer
        var thresholds = layers[layerName];

        // Create a list of tuples, each containing a noise value and its original index
        var noiseTuples = noises.Select((n, i) => (noise: n, index: i)).ToList();

        // Sort the list of tuples by noise value in ascending order
        var sortedNoiseTuples = noiseTuples.OrderBy(t => t.noise).ToList();

        // Get the thresholds in ascending order
        var thresholdKeys = thresholds.Keys.ToList();

        // Initialize the list of indices
        var indices = new int[noises.Count];

        // Initialize the current threshold index and noise tuple index
        int thresholdIndex = 0,
            noiseTupleIndex = 0;

        // Iterate over the sorted noise tuples and thresholds
        while (noiseTupleIndex < sortedNoiseTuples.Count)
        {
            // If the current threshold is less than or equal to the current noise value, or if we've reached the last threshold,
            // add the index associated with the current threshold to the list of indices at the original index of the noise value,
            // and advance to the next noise tuple
            if (
                thresholdIndex == thresholdKeys.Count - 1
                || thresholdKeys[thresholdIndex] <= sortedNoiseTuples[noiseTupleIndex].noise
            )
            {
                indices[sortedNoiseTuples[noiseTupleIndex].index] = thresholds[
                    thresholdKeys[thresholdIndex]
                ];
                noiseTupleIndex++;
            }
            else
            {
                // Otherwise, advance to the next threshold
                thresholdIndex++;
            }
        }

        // Return the list of indices
        return indices.ToList();
    }

    public Resource IndexToResource(int index)
    {
        // Check if the index is within the bounds of the resources list
        if (index < 0 || index >= resources.Count)
        {
            // Return null to indicate that the index is out of bounds
            return null;
        }

        // Return the Resource at the given index
        return resources[index];
    }

    public List<Resource> IndicesToResources(List<int> indices)
    {
        // Initialize the list of Resources
        var resources = new List<Resource>();

        // Iterate over the list of indices
        foreach (var index in indices)
        {
            // Check if the index is within the bounds of the resources list
            if (index >= 0 && index < this.resources.Count)
            {
                // Add the Resource at the given index to the list of Resources
                resources.Add(this.resources[index]);
            }
            else
            {
                // Add null to the list to indicate that the index is out of bounds
                resources.Add(null);
            }
        }

        // Return the list of Resources
        return resources;
    }
}
