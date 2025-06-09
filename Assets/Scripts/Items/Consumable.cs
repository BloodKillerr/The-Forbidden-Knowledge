using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    public bool Equipped = false;
    public int slot = 0;

    public float Cooldown = 5f;

    public List<Effect> effects = new List<Effect>();

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

    public void UseEffects()
    {
        foreach (Effect effect in effects)
        {
            effect.UseEffect();
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
            ("Cooldown", Cooldown.ToString()),
            ("Amount", Amount.ToString())
        };
        
        return data;
    }
}
