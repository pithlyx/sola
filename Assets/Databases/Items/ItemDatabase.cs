using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems;
    public List<CraftingRecipe> allRecipes;

    // Method to get a Item by name
    public Item GetItemByName(string name)
    {
        return allItems.FirstOrDefault(item => item.itemName == name);
    }
}
