using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class Item : ScriptableObject
{
    public List<ItemTag> tags;
    public string itemName;
    public Sprite icon;
}
