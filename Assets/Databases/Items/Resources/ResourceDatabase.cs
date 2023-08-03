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
    public Dictionary<LayerName, List<float>> layerThresholds =
        new Dictionary<LayerName, List<float>>();

    // The instance of the ResourceDatabase
    public static ResourceDatabase Instance { get; set; }

    public void PopulateData()
    {
        // Clear existing data
        layers.Clear();
        layerThresholds.Clear();

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

                // Store the keys of the dictionary in a separate list
                layerThresholds[layerConfig.layerName] = layers[
                    layerConfig.layerName
                ].Keys.ToList();
            }
        }
    }

    public List<float> GetNoise(FastNoiseSIMDUnity noiseGenerator, Vector3Int origin, int chunkSize)
    {
        // Scale the input coordinates
        int xStart = (int)(origin.x);
        int yStart = (int)(origin.y);
        int zStart = (int)(origin.z);
        int xSize = (int)(chunkSize);
        int ySize = (int)(chunkSize);

        // Generate the noise for the chunk
        List<float> noiseSet = noiseGenerator.GetNoiseSet(xStart, yStart, zStart, xSize, ySize, 1);
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
        var thresholdKeys = layerThresholds[layerName];
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
    public List<int> NoiseToIndices(List<float> noiseValues, LayerName layerName)
    {
        // Check if the layers dictionary contains the layerName
        if (!layers.ContainsKey(layerName))
        {
            // Return a list of -1s to indicate that the layerName was not found
            return Enumerable.Repeat(-1, noiseValues.Count).ToList();
        }

        // Get the dictionary of thresholds and indices for the layer
        var thresholds = layers[layerName];

        // Get the list of threshold keys
        var thresholdKeys = layerThresholds[layerName];

        // Iterate over the list of noise values
        List<int> indices = new List<int>();
        foreach (float noise in noiseValues)
        {
            // Perform a binary search for the noise value
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
                // Add the index associated with the threshold to the list of indices
                indices.Add(thresholds[thresholdKeys[index]]);
            }
            else
            {
                // Add -1 to the list to indicate that no suitable threshold was found
                indices.Add(-1);
            }
        }

        // Return the list of indices
        return indices;
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
