using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float Speed = 10f;

    public int Damage = 25;

    public float lifeTime = 5f;

    private void Update()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject hit)
    {
        EnemyStats stats = hit.GetComponent<EnemyStats>();
        if (stats != null)
        {
            stats.TakeDamage(Damage);
        }
        Destroy(gameObject);
    }
}
