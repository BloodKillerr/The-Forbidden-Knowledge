using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot EqSlot;

    public int ArmorModifier;
    public int DamageModifier;
    public int MovementSpeedModifier;

    public bool Equipped = false;

    public GameObject mesh;

    public Vector3 HolsterPositionOffset;
    public Vector3 HolsterRotationOffset;

    public Vector3 HandPositionOffset;
    public Vector3 HandRotationOffset;

    public override void Use()
    {
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
