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
}

public enum ItemType
{
    ITEM,
    EQUIPMENT,
    CONSUMABLE,
    SPELL,
    RESOURCE
}
