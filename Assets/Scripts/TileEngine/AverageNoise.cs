using UnityEngine;
using System.Linq;

public class AverageNoise : MonoBehaviour
{
    private FastNoiseSIMD[] noiseComponents;
    private int xSize = 100;
    private int ySize = 100;
    private int zSize = 100;

    void Start()
    {
        noiseComponents = GetComponents<FastNoiseSIMD>();
        if (noiseComponents.Length > 0)
        {
            float[] averageNoiseMap = new float[xSize * ySize * zSize];
            foreach (var noiseComponent in noiseComponents)
            {
                float[] noiseMap = noiseComponent.GetNoiseSet(0, 0, 0, xSize, ySize, zSize);
                for (int i = 0; i < noiseMap.Length; i++)
                {
                    averageNoiseMap[i] += noiseMap[i];
                }
            }

            for (int i = 0; i < averageNoiseMap.Length; i++)
            {
                averageNoiseMap[i] /= noiseComponents.Length;
            }
            // At this point, averageNoiseMap contains the average noise map
            // You can now use this to generate a texture or do other processing
        }
    }
}
