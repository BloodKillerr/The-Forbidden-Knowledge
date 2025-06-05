using UnityEngine;

[CreateAssetMenu(
    fileName = "SpeedUpgradeAbility",
    menuName = "Abilities/Passive/SpeedUpgradeAbility"
)]
public class SpeedUpgradeAbility : PassiveAbility
{
    public int SpeedBoost = 1;

    public override void OnEquip()
    {
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();
        stats.MovementSpeed.AddModifier(SpeedBoost);
    }

    public override void OnUnequip()
    {
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();
        stats.MovementSpeed.RemoveModifier(SpeedBoost);
    }
}
