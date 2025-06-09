using UnityEngine;
using UnityEngine.Events;

public class CharacterStats : MonoBehaviour
{
    public string CharacterName;

    [SerializeField] private Stat armor;
    [SerializeField] private Stat damage;

    [SerializeField] private int currentHealth;
    [SerializeField] private Stat maxHealth;

    private bool isInvincible = false;

    public Stat Armor { get => armor; set => armor = value; }
    public Stat Damage { get => damage; set => damage = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public Stat MaxHealth { get => maxHealth; set => maxHealth = value; }
    public bool IsInvincible { get => isInvincible; set => isInvincible = value; }

    public UnityEvent<int, int> HealthChanged = new UnityEvent<int, int>();

    private void Awake()
    {
        currentHealth = maxHealth.GetValue();
        HealthChanged?.Invoke(currentHealth, maxHealth.GetValue());
    }

    public void TakeDamage(int damage)
    {
        if(isInvincible)
        {
            return;
        }

        int net = (damage * 100) / (100 + armor.GetValue());
        net = Mathf.Clamp(net, 0, int.MaxValue);

        currentHealth = Mathf.Max(currentHealth - net, 0);

        HealthChanged?.Invoke(currentHealth, maxHealth.GetValue());

        CheckHealth();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth.GetValue());
        HealthChanged?.Invoke(currentHealth, maxHealth.GetValue());
    }

    public void UpgradeMaxHealth(int upgrade)
    {
        if(maxHealth.Upgrade(upgrade))
        {
            currentHealth = Mathf.Clamp(currentHealth + upgrade, 0, maxHealth.GetValue());
            HealthChanged?.Invoke(currentHealth, maxHealth.GetValue());
        }
    }

    public void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        //Die
    }
}
