using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<CraftableItem> items;

    // Method to get a CraftableItem by name
    public CraftableItem GetItemByName(string name)
    {
        return items.FirstOrDefault(item => item.itemName == name);
    }

    // Method to get a CraftableItem by associated resource
    public CraftableItem GetItemByResource(Resource resource)
    {
        return items.FirstOrDefault(
            item =>
                item.materialCosts.Any(
                    materialCost =>
                        materialCost.material.resource != null
                        && materialCost.material.resource.resourceName == resource.resourceName
                )
        );
    }
}
