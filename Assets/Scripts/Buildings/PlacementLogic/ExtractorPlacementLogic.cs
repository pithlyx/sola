using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class ExtractorPlacementLogic : DefaultPlacement
{
    public override Building PlaceBuilding(
        BuildingHandler handler,
        BuildingManager manager,
        Building ghostBuilding
    )
    {
        // First, do the common placement logic
        var newBuilding = base.PlaceBuilding(handler, manager, ghostBuilding);

        if (newBuilding != null)
        {
            // Then, do the extractor-specific logic
            // get the generator for the operating layer
            FastNoiseSIMDUnity noiseGenerator = ResourceDatabase.Instance.GetGenerator(
                newBuilding.operationLayer
            );
            // get the noise value for the building's position
            List<float> noiseSet = noiseGenerator.GetNoiseSet(
                handler.cursorPosition.x,
                handler.cursorPosition.y,
                0,
                1,
                1,
                1
            );
            // get the resource for the noise from the database
            Resource resource = ResourceDatabase.Instance.NoiseToResource(
                noiseSet[0],
                newBuilding.operationLayer
            );
            Debug.Log("Resource: " + resource.resourceName);
            // set the building's output items to the resource
            foreach (Port port in newBuilding.Ports.AllPorts)
            {
                // Debug.Log("Iterating Port with Flow: " + port.PortFlow);
                if (port.PortFlow == PortFlow.Out)
                {
                    port.CurrentItem = resource;
                }
                Debug.Log("Port Item: " + port.CurrentItem.itemName);
            }
        }

        return newBuilding;
    }
}
