using UnityEngine;

public class PlayerStats : CharacterStats
{
    private void Start()
    {
        EquipmentManager.Instance.EquipmentChanged.AddListener(OnEquipmentChanged);
    }

    private void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if(newItem != null)
        {
            armor.AddModifier(newItem.ArmorModifier);
            damage.AddModifier(newItem.DamageModifier);
        }

        if(oldItem != null)
        {
            armor.RemoveModifier(oldItem.ArmorModifier);
            damage.RemoveModifier(oldItem.DamageModifier);
        }
    }
}
