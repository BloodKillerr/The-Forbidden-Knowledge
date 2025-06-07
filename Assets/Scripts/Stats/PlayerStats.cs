using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : CharacterStats
{
    [SerializeField] private Stat movementSpeed;

    private int currentDodgeCharges;
    [SerializeField] private Stat maxDodgeCharges;

    public Stat MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    public int CurrentDodgeCharges { get => currentDodgeCharges; set => currentDodgeCharges = value; }
    public Stat MaxDodgeCharges { get => maxDodgeCharges; set => maxDodgeCharges = value; }

    public UnityEvent<int, int> DodgeChargesChanged = new UnityEvent<int, int>();
    public UnityEvent<float> ApplyInvincibilityEvent = new UnityEvent<float>();

    private void Start()
    {
        EquipmentManager.Instance.EquipmentChanged.AddListener(OnEquipmentChanged);
        currentDodgeCharges = maxDodgeCharges.GetValue();
        DodgeChargesChanged?.Invoke(currentDodgeCharges, maxDodgeCharges.GetValue());
    }

    private void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if(newItem != null)
        {
            Armor.AddModifier(newItem.ArmorModifier);
            Damage.AddModifier(newItem.DamageModifier);
            movementSpeed.AddModifier(newItem.MovementSpeedModifier);
        }

        if(oldItem != null)
        {
            Armor.RemoveModifier(oldItem.ArmorModifier);
            Damage.RemoveModifier(oldItem.DamageModifier);
            movementSpeed.RemoveModifier(oldItem.MovementSpeedModifier);
        }
    }

    public bool UseDodgeCharge()
    {
        if (currentDodgeCharges > 0)
        {
            currentDodgeCharges--;
            DodgeChargesChanged?.Invoke(currentDodgeCharges, maxDodgeCharges.GetValue());
            return true;
        }
        return false;
    }

    public void UpgradeMaxDodgeCharges(int upgrade)
    {
        if(maxDodgeCharges.Upgrade(upgrade))
        {
            currentDodgeCharges = Mathf.Clamp(currentDodgeCharges + upgrade, 0, maxDodgeCharges.GetValue());
            DodgeChargesChanged?.Invoke(currentDodgeCharges, maxDodgeCharges.GetValue());
        }  
    }

    public void ApplyEffectToMovementSpeed(int amount, float duration)
    {
        movementSpeed.AddModifier(amount);
        StartCoroutine(RemoveEffectFromStat(movementSpeed, amount, duration));
    }

    public void ApplyEffectToDamage(int amount, float duration)
    {
        Damage.AddModifier(amount);
        StartCoroutine(RemoveEffectFromStat(Damage, amount, duration));
    }

    public void ApplyInvincibility(float duration)
    {
        IsInvincible = true;
        ApplyInvincibilityEvent.Invoke(duration);
        StartCoroutine(RemoveInvincibility(duration));
    }

    private IEnumerator RemoveEffectFromStat(Stat stat, int amount, float duration)
    {
        yield return new WaitForSeconds(duration);
        stat.RemoveModifier(amount);
    }

    private IEnumerator RemoveInvincibility(float duration)
    {
        yield return new WaitForSeconds(duration);
        IsInvincible = false;
    }

    public override void Die()
    {
        base.Die();
        GetComponentInChildren<Animator>().Play("PlayerDeath");
        Player.Instance.IsDead = true;
    }

    public StatsData GetStatsData()
    {
        return new StatsData
        {
            currentHealth = CurrentHealth,
            baseMaxHealth = MaxHealth.GetBaseValue(),
            isInvincible = IsInvincible,
            baseArmor = Armor.GetBaseValue(),
            baseDamage = Damage.GetBaseValue(),
            baseMovementSpeed = movementSpeed.GetBaseValue(),
            currentDodgeCharges = CurrentDodgeCharges,
            baseMaxDodgeCharges = MaxDodgeCharges.GetBaseValue()
        };
    }

    public void ApplyStatsData(StatsData d)
    {
        EquipmentManager.Instance.EquipmentChanged.RemoveListener(OnEquipmentChanged);

        MaxHealth = new Stat
        {
            BaseValue = d.baseMaxHealth,
            ValueUpgradeCap = MaxHealth.ValueUpgradeCap
        };
        CurrentHealth = d.currentHealth;
        IsInvincible = d.isInvincible;
        Armor = new Stat
        {
            BaseValue = d.baseArmor,
            ValueUpgradeCap = Armor.ValueUpgradeCap
        };
        Damage = new Stat
        {
            BaseValue = d.baseDamage,
            ValueUpgradeCap = Damage.ValueUpgradeCap
        };

        movementSpeed = new Stat
        {
            BaseValue = d.baseMovementSpeed,
            ValueUpgradeCap = movementSpeed.ValueUpgradeCap
        };
        maxDodgeCharges = new Stat
        {
            BaseValue = d.baseMaxDodgeCharges,
            ValueUpgradeCap = maxDodgeCharges.ValueUpgradeCap
        };
        CurrentDodgeCharges = d.currentDodgeCharges;

        EquipmentManager.Instance.EquipmentChanged.AddListener(OnEquipmentChanged);

        HealthChanged?.Invoke(CurrentHealth, MaxHealth.GetValue());
        DodgeChargesChanged?.Invoke(CurrentDodgeCharges, MaxDodgeCharges.GetValue());
        ApplyInvincibilityEvent?.Invoke(0f);
        UIManager.Instance.BuildStatUI(this);
        GetComponent<PlayerMovement>().RestoreDodgeCharges();
    }
}
