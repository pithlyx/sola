using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Resource", menuName = "Items/New Resource")]
public class Resource : Item
{
    public string resourceName;
    public bool canHarvest;
    public TileBase tile;
    public int minHarvestLevel;
}
