using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    private EnemyStats enemyStats;

    private Animator animator;

    private Enemy enemy;

    public Animator Animator { get => animator; set => animator = value; }

    private void Start()
    {
        enemyStats = GetComponentInParent<EnemyStats>();
        animator = GetComponent<Animator>();
        enemy = GetComponentInParent<Enemy>();
    }

    public void Spawn()
    {
        enemyStats.IsInvincible = true;
    }

    public void AfterSpawn()
    {
        enemyStats.IsInvincible = false;
    }

    public void Die()
    {
        enemy.GetLoot();
        enemy.Die();
        Enemy.OnEnemyKilled?.Invoke(enemy);
        Destroy(transform.parent.gameObject);
    }

    public void EnableDamageCollider()
    {
        enemy.DamageCollider.EnableDamageCollider();
    }

    public void DisableDamageCollider()
    {
        enemy.DamageCollider.DisableDamageCollider();
    }
}
