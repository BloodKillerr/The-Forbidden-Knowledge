using UnityEngine;

[CreateAssetMenu(
    fileName = "ArmorUpgradeAbility",
    menuName = "Abilities/Passive/ArmorUpgradeAbility"
)]
public class ArmorUpgradeAbility : PassiveAbility
{
    public int ArmorBoost = 10;

    public override void OnEquip()
    {
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();
        stats.Armor.AddModifier(ArmorBoost);
    }

    public override void OnUnequip()
    {
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();
        stats.Armor.RemoveModifier(ArmorBoost);
    }
}
