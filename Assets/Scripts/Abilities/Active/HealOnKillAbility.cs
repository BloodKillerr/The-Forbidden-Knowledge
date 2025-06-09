using UnityEngine;

[CreateAssetMenu(
    fileName = "HealOnKillAbility",
    menuName = "Abilities/Active/HealOnKillAbility"
)]
public class HealOnKillAbility : ActiveAbility
{
    [Range(0f, 1f)]
    public float healPercent = 0.2f;

    public override void OnEquip()
    {
        if (AbilityManager.Instance != null)
        {
            AbilityManager.Instance.OnEnemyKilledInt.AddListener(OnEnemyDeathMethod);
        }
    }

    public override void OnUnequip()
    {
        if (AbilityManager.Instance != null)
        {
            AbilityManager.Instance.OnEnemyKilledInt.RemoveListener(OnEnemyDeathMethod);
        }
    }

    private void OnEnemyDeathMethod(int enemyMaxHealth)
    {
        int healAmount = (int)(enemyMaxHealth * healPercent);
        PlayerStats stats = Player.Instance.GetComponent<PlayerStats>();

        if(AbilityManager.Instance.HasAbility(typeof(DamageUpgradeAbility)))
        {
            stats.Heal((int)(healAmount*1.1f));
        }
        else
        {
            stats.Heal(healAmount);
        }
        
    }
}
