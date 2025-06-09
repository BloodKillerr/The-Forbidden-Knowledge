using UnityEngine;

[CreateAssetMenu(
    fileName = "DamageUpgradeAbility",
    menuName = "Abilities/Passive/DamageUpgradeAbility"
)]
public class DamageUpgradeAbility : PassiveAbility
{
    public int DamageBoost = 10;

    public override void OnEquip()
    {
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();
        stats.Damage.AddModifier(DamageBoost);
    }

    public override void OnUnequip()
    {
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();
        stats.Damage.RemoveModifier(DamageBoost);
    }
}
