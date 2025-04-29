using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] protected Stat armor;
    [SerializeField] protected Stat damage;

    protected int currentHealth;
    [SerializeField] protected int maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        damage -= armor.GetValue();

        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Die
    }
}
