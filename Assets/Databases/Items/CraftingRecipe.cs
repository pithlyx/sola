using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Recipes/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [System.Serializable]
    public struct RecipeIngredient
    {
        public Item item;
        public int amount;
    }

    public List<RecipeIngredient> ingredients; // Input items
    public RecipeIngredient output; // Output item
    public int operations; // Crafting cost (could be in terms of energy, time, currency etc.)

    // You can add more functionality such as checking if a set of items can be used to craft the output
    public bool CanCraft(Dictionary<Item, int> availableItems)
    {
        foreach (var ingredient in ingredients)
        {
            if (
                !availableItems.ContainsKey(ingredient.item)
                || availableItems[ingredient.item] < ingredient.amount
            )
            {
                return false;
            }
        }
        return true;
    }

    public Item CraftItem(Dictionary<Item, int> availableItems)
    {
        if (!CanCraft(availableItems))
        {
            Debug.LogError("Cannot craft item: " + output.item.itemName);
            return null;
        }

        foreach (var ingredient in ingredients)
        {
            availableItems[ingredient.item] -= ingredient.amount;
        }

        return output.item;
    }
}
