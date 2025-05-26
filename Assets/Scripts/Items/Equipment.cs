using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot EqSlot;

    public int ArmorModifier;
    public int DamageModifier;
    public int MovementSpeedModifier;

    public bool Equipped = false;

    public override void Use()
    {
        if(Player.Instance.GetComponent<PlayerAttack>().IsAttacking)
        {
            return;
        }

        base.Use();
        if(Equipped)
        {
            EquipmentManager.Instance.UnEquip((int)EqSlot);
            UIManager.Instance.UpdateEquipmentSlotHolder(this, false);
        }
        else
        {
            EquipmentManager.Instance.Equip(this);
            UIManager.Instance.UpdateEquipmentSlotHolder(this, true);
        }
    }

    public override bool UpdateUIState()
    {
        return Equipped;
    }

    public override List<(string label, string value)> GetTooltipData()
    {
        List<(string label, string value)> data = new List<(string, string)>
        {
            ("Description", Description),
            ("Armor", ArmorModifier.ToString()),
            ("Damage", DamageModifier.ToString()),
            ("Speed", MovementSpeedModifier.ToString()),
            ("Amount", Amount.ToString())
        };
        return data;
    }
}

public enum EquipmentSlot
{
    HEAD,
    CHEST,
    LEGS,
    ARMS,
    PRIMARY,
    SECONDARY
}
