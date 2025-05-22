using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    private Collider damageCollider;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            EnemyStats ownStats = gameObject.GetComponentInParent<EnemyStats>();

            if (stats != null)
            {
                stats.TakeDamage(ownStats.Damage.GetValue());
            }
        }

        if (other.CompareTag("Enemy"))
        {
            EnemyStats stats = other.GetComponent<EnemyStats>();
            PlayerStats ownStats = gameObject.GetComponentInParent<PlayerStats>();

            if (stats != null)
            {
                stats.TakeDamage(ownStats.Damage.GetValue());
            }
        }
    }
}
