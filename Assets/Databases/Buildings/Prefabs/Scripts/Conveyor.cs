// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Conveyor : Building
// {
//     public float rate; // Rate of resource transport per second
//     private Resource lastProcessedResource; // Track the last processed resource

//     public override void ProcessInput(Resource inputResource)
//     {
//         // If the input resource hasn't changed, don't need to do anything
//         if (inputResource == lastProcessedResource)
//             return;

//         // Output resource is the same as the input resource
//         Resource outputResource = inputResource;

//         // Iterate over the inOuts list
//         for (int i = 0; i < inOuts.Count; i++)
//         {
//             // If this inOut is an output, set its resource to the input resource
//             if (inOuts[i].flow == InOut.Flow.Output)
//             {
//                 InOut output = inOuts[i];
//                 output.itemType = InOut.ItemType.Resource; // Assuming that the Conveyor only deals with Resources
//                 output.resource = outputResource;
//                 inOuts[i] = output; // Update the inOut in the list
//             }
//         }

//         // Update lastProcessedResource to the current inputResource
//         lastProcessedResource = inputResource;
//     }

//     public override Resource GenerateOutput()
//     {
//         // Conveyors don't generate output by themselves, they just pass resources along
//         return null;
//     }
// }
