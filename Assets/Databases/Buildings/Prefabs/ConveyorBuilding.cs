using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBuilding : Building
{
    // Override the abstract method in the derived class
    public override void SetOutputItems()
    {
        // Get the input and output ports
        List<Port> inputPorts = GetPortsByFlow(PortFlow.In);
        List<Port> outputPorts = GetPortsByFlow(PortFlow.Out);

        foreach (Port inputPort in inputPorts)
        {
            foreach (Port outputPort in outputPorts)
            {
                // Ensure outputPort can receive the item
                if (outputPort.PortFlow == PortFlow.Out || outputPort.PortFlow == PortFlow.InOut)
                {
                    outputPort.CurrentItem = inputPort.CurrentItem;
                }
            }
        }
    }
}
