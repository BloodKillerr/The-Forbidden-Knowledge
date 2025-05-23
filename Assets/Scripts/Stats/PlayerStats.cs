using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : CharacterStats
{
    [SerializeField] private Stat movementSpeed;

    private int currentDodgeCharges;
    [SerializeField] private Stat maxDodgeCharges;

    public UnityEvent<int, int> DodgeChargesChanged = new UnityEvent<int, int>();

    public Stat MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    public int CurrentDodgeCharges { get => currentDodgeCharges; set => currentDodgeCharges = value; }
    public Stat MaxDodgeCharges { get => maxDodgeCharges; set => maxDodgeCharges = value; }

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
}
