using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

[System.Serializable]
public class MaterialCost
{
    public Item material;
    public int quantity;
}

[System.Serializable]
public enum itemType
{
    Resource,
    Craftable
}

[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class Item : ScriptableObject
{
    public itemType type;
    public string itemName;
    public Sprite icon;
    public List<MaterialCost> materialCosts;
    public float operationCost; // How many operations it takes to craft this item
}
