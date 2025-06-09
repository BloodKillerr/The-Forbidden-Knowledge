using UnityEngine;
using UnityEngine.Events;

public class SpellManager : MonoBehaviour
{
    private Spell spell1 = null;
    private Spell spell2 = null;

    private float nextUseTime1 = 0f;
    private float nextUseTime2 = 0f;

    public UnityEvent<int, Spell> OnSpellUsed = new UnityEvent<int, Spell>();
    public UnityEvent<int, Spell> OnSpellEquipped = new UnityEvent<int, Spell>();
    public UnityEvent<int> OnSpellUnequipped = new UnityEvent<int>();

    public static SpellManager Instance { get; private set; }
    public Spell Spell1 { get => spell1; set => spell1 = value; }
    public Spell Spell2 { get => spell2; set => spell2 = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void EquipSpell(Spell spell)
    {
        int slot;
        if (spell1 == null)
        {
            spell1 = spell;
            slot = 1;
        }
        else if (spell2 == null)
        {
            spell2 = spell;
            slot = 2;
        }
        else
        {
            spell1.Equipped = false;
            spell1 = spell;
            slot = 1;
        }

        spell.Equipped = true;
        spell.slot = slot;
        OnSpellEquipped.Invoke(slot, spell);
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
        UIManager.Instance.UpdateEquipmentSlotHolder(spell, true);
    }

    public void UnEquipSpell(Spell spell)
    {
        if (spell1 != null && spell1.Name == spell.Name)
        {
            spell1.Equipped = false;
            spell1 = null;
            OnSpellUnequipped.Invoke(1);
        }
        else if (spell2 != null && spell2.Name == spell.Name)
        {
            spell2.Equipped = false;
            spell2 = null;
            OnSpellUnequipped.Invoke(2);
        }
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
    }

    public void UnEquipAll()
    {
        if (spell1 != null)
        {
            spell1.Equipped = false;
            spell1 = null;
        }
        if (spell2 != null)
        {
            spell2.Equipped = false;
            spell2 = null;
        }
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
    }

    public void UseSpell1()
    {
        if (spell1 == null)
        {
            return;
        }

        if (Time.time < nextUseTime1)
        {
            return;
        }

        spell1.UseEffects();


        nextUseTime1 = Time.time + spell1.Cooldown;
        OnSpellUsed.Invoke(1, spell1);
    }

    public void UseSpell2()
    {
        if (spell2 == null)
        {
            return;
        }

        if (Time.time < nextUseTime2)
        {
            return;
        }

        spell2.UseEffects();


        nextUseTime2 = Time.time + spell2.Cooldown;
        OnSpellUsed.Invoke(2, spell2);
    }

    public void ShortenCooldown(float amount)
    {
        if (spell1 != null)
        {
            nextUseTime1 = Mathf.Max(Time.time, nextUseTime1 - amount);
        }

        if (spell2 != null)
        {
            nextUseTime2 = Mathf.Max(Time.time, nextUseTime2 - amount);
        }
    }

    public void ResetCooldowns()
    {
        nextUseTime1 = 0f;
        nextUseTime2 = 0f;
    }

    public float GetRemainingCooldown(int slot)
    {
        if (slot == 1 && spell1 != null)
            return Mathf.Max(0f, nextUseTime1 - Time.time);
        if (slot == 2 && spell2 != null)
            return Mathf.Max(0f, nextUseTime2 - Time.time);
        return 0f;
    }

    public void SetRemainingCooldown(int slot, float remaining)
    {
        if (slot == 1 && spell1 != null)
        {
            nextUseTime1 = Time.time + remaining;
        }
        else if (slot == 2 && spell2 != null)
        {
            nextUseTime2 = Time.time + remaining;
        }
    }
}
