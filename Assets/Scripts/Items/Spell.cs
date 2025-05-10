using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Inventory/Spell")]
public class Spell : Item
{
    public bool Equipped = false;
    public int slot = 0;

    public override void Use()
    {
        base.Use();
        if (Equipped)
        {
            SpellManager.Instance.UnEquipSpell(this);
            UIManager.Instance.UpdateEquipmentSlotHolder(this, false);
        }
        else
        {
            SpellManager.Instance.EquipSpell(this);
            UIManager.Instance.UpdateEquipmentSlotHolder(this, true);
        }
    }

    public override bool UpdateUIState()
    {
        return Equipped;
    }
}
