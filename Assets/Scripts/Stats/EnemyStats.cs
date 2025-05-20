using UnityEngine;

public class EnemyStats : CharacterStats
{
    public override void Die()
    {
        base.Die();
        //GetComponentInChildren<Animator>().Play("Death");
        Destroy(gameObject);
    }
}
