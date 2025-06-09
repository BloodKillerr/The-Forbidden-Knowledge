using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string Name = "New Item";
    public Sprite Icon = null;
    public int Amount = 1;
    public ItemType Type = ItemType.ITEM;

    public virtual void Use()
    {

    }

    public virtual bool UpdateUIState()
    {
        return false;
    }

    public virtual List<(string label, string value)> GetTooltipData()
    {
        List<(string label, string value)> data = new List<(string, string)>
        {
            ("Amount", Amount.ToString())
        };
        return data;
    }
}

public enum ItemType
{
    ITEM,
    EQUIPMENT,
    CONSUMABLE,
    SPELL,
    RESOURCE
}
