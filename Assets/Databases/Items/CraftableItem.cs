using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Material
{
    public Resource resource;
    public CraftableItem craftableItem;
}

[System.Serializable]
public class MaterialCost
{
    public Material material;
    public int quantity;
}

[CreateAssetMenu(fileName = "New CraftableItem", menuName = "Items/CraftableItem")]
public class CraftableItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public List<MaterialCost> materialCosts;
    public float processCost; // How many operations it takes to craft this item

    // Add any other properties or methods you need for your craftable items.
}
