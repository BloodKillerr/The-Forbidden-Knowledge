using UnityEngine;

[CreateAssetMenu(
    fileName = "DodgeUpgradeAbility",
    menuName = "Abilities/Passive/DodgeUpgradeAbility"
)]
public class DodgeUpgradeAbility : PassiveAbility
{
    public int ExtraDodgeCharges = 1;

    public override void OnEquip()
    {
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();
        stats.MaxDodgeCharges.AddModifier(ExtraDodgeCharges);

        stats.CurrentDodgeCharges = stats.MaxDodgeCharges.GetValue();
    }

    public override void OnUnequip()
    {
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();
        stats.MaxDodgeCharges.RemoveModifier(ExtraDodgeCharges);

        if (stats.CurrentDodgeCharges > stats.MaxDodgeCharges.GetValue())
        {
            stats.CurrentDodgeCharges = stats.MaxDodgeCharges.GetValue();
        }
    }
}
