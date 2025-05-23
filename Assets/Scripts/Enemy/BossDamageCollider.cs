using UnityEngine;

public class BossDamageCollider : DamageCollider
{
    public float CurrentDamageMultiplier { get; set; } = 1f;

    protected override void OnTriggerEnter(Collider other)
    {
        if (!damageCollider.enabled)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            EnemyStats ownStats = GetComponentInParent<EnemyStats>();

            if (stats != null && ownStats != null)
            {
                int baseDamage = ownStats.Damage.GetValue();
                int finalDamage = Mathf.RoundToInt(baseDamage * CurrentDamageMultiplier);
                stats.TakeDamage(finalDamage);
            }
        }
    }
}
