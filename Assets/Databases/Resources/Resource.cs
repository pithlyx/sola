using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[System.Serializable]
public class ResourceThreshold
{
    public int Layer;
    public float Threshold;
}

[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/Resource")]
public class Resource : ScriptableObject
{
    public string resourceName;
    public Sprite icon;
    public TileBase tile;
    public string description;
    public List<ResourceThreshold> thresholds;
}
