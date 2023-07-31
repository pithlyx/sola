// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// [System.Serializable]
// public class BuildingGroup
// {
//     public string Name;
//     public List<Building> Buildings;
//     public KeyCode Hotkey;

//     [System.NonSerialized]
//     public int currentIndex = 0;

//     public Building GetCurrentBuilding()
//     {
//         if (Buildings == null || Buildings.Count == 0)
//         {
//             return null;
//         }

//         return Buildings[currentIndex];
//     }

//     public void NextBuilding()
//     {
//         if (Buildings == null || Buildings.Count == 0)
//         {
//             currentIndex = 0;
//             return;
//         }

//         currentIndex = (currentIndex + 1) % Buildings.Count;
//     }

//     public Building GetBuildingByTileBase(TileBase tileBase)
//     {
//         foreach (Building building in Buildings)
//         {
//             if (
//                 building.baseSettings.Sprite == tileBase
//                 || building.baseSettings.Animated == tileBase
//             )
//             {
//                 return building;
//             }
//         }
//         return null;
//     }

//     public Building SelectBuilding(Building building)
//     {
//         int index = Buildings.IndexOf(building);
//         if (index >= 0)
//         {
//             currentIndex = index;
//         }
//         return GetCurrentBuilding();
//     }

//     public Building SelectBuildingWithRotation(Building building)
//     {
//         int index = Buildings.IndexOf(building);
//         if (index >= 0)
//         {
//             currentIndex = index;
//         }
//         Building selectedBuilding = GetCurrentBuilding();
//         selectedBuilding.baseSettings.RotationIndex = building.baseSettings.RotationIndex;
//         return selectedBuilding;
//     }
// }
