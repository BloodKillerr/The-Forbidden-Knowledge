using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    public bool Equipped = false;
    public int slot = 0;

    public override void Use()
    {
        base.Use();
        if (Equipped)
        {
            ConsumableManager.Instance.UnEquipConsumable(this);
            UIManager.Instance.UpdateEquipmentSlotHolder(this, false);
        }
        else
        {
            ConsumableManager.Instance.EquipConsumable(this);
            UIManager.Instance.UpdateEquipmentSlotHolder(this, true);
        }
    }

    public override bool UpdateUIState()
    {
        return Equipped;
    }
}
