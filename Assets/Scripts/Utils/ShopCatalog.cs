using System;
using UnityEngine;

// every item of one kind (apple skins, tablecloths) in shop order; saves refer to items by id, so ids must never change
[CreateAssetMenu(fileName = "ShopCatalog", menuName = "Scriptable Objects/ShopCatalog")]
public class ShopCatalog : ScriptableObject
{
    [Serializable]
    public class Item
    {
        public string id;
        public string name; // shown in the shop
        public Sprite icon;
        public AnimatorOverrideController skin; // apple skins only; tablecloths use icon as the table sprite
        public int price;
    }

    public bool table; // equips into the tablecloth slot instead of the apple skin
    public Item[] items;

    // unknown or empty id falls back to the first (default) item
    public Item Find(string id) => Array.Find(items, i => i.id == id) ?? items[0];
}
