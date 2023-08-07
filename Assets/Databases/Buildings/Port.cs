using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class Port
{
    [SerializeField]
    private PortType _portType;

    [SerializeField]
    private PortFlow _portFlow;

    [SerializeField]
    private float _maxTransferRate;

    [SerializeField]
    private float _transferRate;

    [SerializeField]
    private bool _isBlocked;

    [SerializeField]
    private bool _canTransfer;

    [SerializeField]
    private Item _currentItem;

    [SerializeField]
    private Port _connectedPort;

    [SerializeField]
    private bool _connectionValid;

    [SerializeField]
    private bool _connectionChecked;

    [SerializeField]
    private bool _connectionUpdated;

    public Port(PortConfig config)
    {
        PortType = config.Type;
        PortFlow = config.Flow;
        MaxTransferRate = config.MaxTransferRate;
        TransferRate = config.TransferRate;
        IsBlocked = false;
        CanTransfer = false;
        CurrentItem = null;
        ConnectedPort = null;
        ConnectionValid = false;
        ConnectionChecked = false;
        ConnectionUpdated = false;
    }

    public PortType PortType
    {
        get => _portType;
        set => _portType = value;
    }
    public PortFlow PortFlow
    {
        get => _portFlow;
        set => _portFlow = value;
    }
    public float MaxTransferRate
    {
        get => _maxTransferRate;
        set => _maxTransferRate = value;
    }

    public float TransferRate
    {
        get => _transferRate;
        set => _transferRate = value;
    }

    public bool IsBlocked
    {
        get => _isBlocked;
        set => _isBlocked = value;
    }

    public bool CanTransfer
    {
        get => _canTransfer;
        set => _canTransfer = value;
    }

    public Item CurrentItem
    {
        get => _currentItem;
        set => _currentItem = value;
    }

    public Port ConnectedPort
    {
        get => _connectedPort;
        set => _connectedPort = value;
    }

    public bool ConnectionValid
    {
        get => _connectionValid;
        set => _connectionValid = value;
    }

    public bool ConnectionChecked
    {
        get => _connectionChecked;
        set => _connectionChecked = value;
    }

    public bool ConnectionUpdated
    {
        get => _connectionUpdated;
        set => _connectionUpdated = value;
    }

    public bool ValidateConnection()
    {
        if (
            ConnectedPort != null
            && (ConnectionChecked == false || ConnectedPort.ConnectionChecked == false)
        )
        {
            ConnectionChecked = true;
            ConnectionUpdated = false;
            ConnectedPort.ConnectionChecked = true;
            ConnectedPort.ConnectionUpdated = false;
            if (this.IsCompatableWith(ConnectedPort))
            {
                ConnectionValid = true;
                ConnectedPort.ConnectionValid = true;
                return true;
            }
            else
            {
                ConnectionValid = false;
                ConnectedPort.ConnectionValid = false;
                return false;
            }
        }
        return false;
    }

    public bool UpdateConnection()
    {
        if (ConnectionUpdated == true)
        {
            return true;
        }

        if (ValidateConnection())
        {
            UpdateItem();
        }
        ConnectionUpdated = true;
        return true;
    }

    public void UpdateItem()
    {
        if (ConnectedPort == null)
            return;

        if ((PortFlow == PortFlow.Out || PortFlow == PortFlow.InOut) && CurrentItem != null)
        {
            if (ConnectedPort.ReceiveItem(CurrentItem))
            {
                CanTransfer = true;
            }
        }

        if (PortFlow == PortFlow.In || PortFlow == PortFlow.InOut)
        {
            CurrentItem = ConnectedPort.SendItem();
        }
    }

    public bool ReceiveItem(Item item)
    {
        if ((PortFlow == PortFlow.In || PortFlow == PortFlow.InOut) && !IsBlocked)
        {
            CurrentItem = item;

            return true;
        }

        return false;
    }

    public Item SendItem()
    {
        if (
            (PortFlow == PortFlow.Out || PortFlow == PortFlow.InOut)
            && CurrentItem != null
            && !IsBlocked
        )
        {
            return CurrentItem;
        }
        return null;
    }

    public void UpdateConnectedPort()
    {
        // If this port has a valid connection, update the connected port.
        if (ConnectedPort != null && ConnectionValid)
        {
            ConnectedPort.UpdateConnection();
        }
    }
}
