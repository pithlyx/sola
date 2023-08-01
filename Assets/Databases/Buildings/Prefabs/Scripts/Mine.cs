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

    public override void OnPlaced(CraftableItem item)
    {
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
                    item = item
                };

                this.inOuts[i] = modifiedInOut;
            }
        }
    }
}
