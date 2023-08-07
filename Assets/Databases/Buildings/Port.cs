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
    private bool _isTransfering;

    [SerializeField]
    private Item _currentItem;

    [SerializeField]
    private Port _connectedPort;

    [SerializeField]
    private bool _connectionEstablished;

    public Port(PortConfig config)
    {
        PortType = config.Type;
        PortFlow = config.Flow;
        MaxTransferRate = config.MaxTransferRate;
        TransferRate = config.TransferRate;
        IsBlocked = false;
        IsTransfering = false;
        CurrentItem = null;
        ConnectedPort = null;
        ConnectionEstablished = false;
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

    public bool IsTransfering
    {
        get => _isTransfering;
        set => _isTransfering = value;
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

    public bool ConnectionEstablished
    {
        get => _connectionEstablished;
        set => _connectionEstablished = value;
    }

    public bool CanTransfer()
    {
        // This method checks if the port can currently transfer items.
        // It could return false if the port is blocked or if it's not transferring.
        return !IsBlocked && IsTransfering;
    }

    public void StartTransfer(Item item)
    {
        // This method starts a transfer of the specified item.
        // It sets the currentItem to the specified item and sets isTransfering to true.
        CurrentItem = item;
        IsTransfering = true;
    }
}
