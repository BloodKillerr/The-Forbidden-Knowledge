using UnityEngine;
using UnityEngine.Events;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private Stat armor;
    [SerializeField] private Stat damage;

    private int currentHealth;
    [SerializeField] private Stat maxHealth;

    public Stat Armor { get => armor; set => armor = value; }
    public Stat Damage { get => damage; set => damage = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public Stat MaxHealth { get => maxHealth; set => maxHealth = value; }

    public UnityEvent<int, int> HealthChanged = new UnityEvent<int, int>();

    private void Awake()
    {
        currentHealth = maxHealth.GetValue();
        HealthChanged?.Invoke(currentHealth, maxHealth.GetValue());
    }

    public void TakeDamage(int damage)
    {
        int net = Mathf.Clamp(damage - armor.GetValue(), 0, int.MaxValue);

        currentHealth = Mathf.Max(currentHealth - net, 0);

        HealthChanged?.Invoke(currentHealth, maxHealth.GetValue());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpgradeMaxHealth(int upgrade)
    {
        maxHealth.Upgrade(upgrade);
        currentHealth = Mathf.Clamp(currentHealth + upgrade, 0, maxHealth.GetValue());
        HealthChanged?.Invoke(currentHealth, maxHealth.GetValue());
    }

    public virtual void Die()
    {
        //Die
    }
}
