using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Resource", menuName = "Items/Resource")]
public class Resource : Item
{
    public bool canHarvest;
    public string resourceName;
    public TileBase tile;
}
