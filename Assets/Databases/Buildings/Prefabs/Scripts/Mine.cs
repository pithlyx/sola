using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Mine : Building
{
    public int layerIndex; // The layer index the mine operates on

    public ChunkGenerator chunkGenerator; // ChunkGenerator component

    // Reference to the ItemDatabase
    public ItemDatabase itemDatabase;

    public override Building CreateBuilding()
    {
        // Call the CreateBuilding method from the base Building class
        Mine newMine = base.CreateBuilding() as Mine;

        // Return the new mine instance
        return newMine;
    }

    public override void PlaceBuilding(int worldX, int worldY, params object[] optionalParams)
    {
        // Set the chunkGenerator for the new mine
        this.chunkGenerator = optionalParams[0] as ChunkGenerator;

        // Get the resource for the point
        Resource resource = chunkGenerator.GetResourceForPoint(worldX, worldY, layerIndex);

        // Get the CraftableItem that matches the resource
        CraftableItem craftableItem = itemDatabase.GetItemByResource(resource);

        // Modify existing InOuts
        for (int i = 0; i < this.inOuts.Count; i++)
        {
            // Only modify InOuts that have their flow set to Output
            if (this.inOuts[i].flow == Flow.Output)
            {
                InOut modifiedInOut = new InOut
                {
                    direction = this.inOuts[i].direction,
                    flow = this.inOuts[i].flow,
                    item = craftableItem
                };

                this.inOuts[i] = modifiedInOut;
            }
        }
    }
}
