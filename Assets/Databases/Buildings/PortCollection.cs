using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;

[System.Serializable]
public class PortCollection
{
    [ShowInInspector]
    private Dictionary<Direction, Port> _ports;
    public IEnumerable<Port> AllPorts => _ports.Values;

    public PortCollection(PortDirections portDirections)
    {
        _ports = new Dictionary<Direction, Port>
        {
            { Direction.North, new Port(portDirections.North) },
            { Direction.East, new Port(portDirections.East) },
            { Direction.South, new Port(portDirections.South) },
            { Direction.West, new Port(portDirections.West) }
        };
    }

    public Port GetPort(Direction direction)
    {
        return _ports[direction];
    }

    public void BlockAllPorts()
    {
        foreach (var port in _ports.Values)
        {
            port.IsBlocked = true;
        }
    }

    public void UnblockAllPorts()
    {
        foreach (var port in _ports.Values)
        {
            port.IsBlocked = false;
        }
    }

    public void UpdateAllPorts()
    {
        foreach (var port in _ports.Values)
        {
            if (port.ConnectionUpdated == false)
            {
                port.UpdateConnection();
            }
        }
    }
}
