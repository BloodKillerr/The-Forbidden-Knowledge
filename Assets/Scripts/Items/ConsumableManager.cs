using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ConsumableManager : MonoBehaviour
{
    private Consumable consumable1 = null;
    private Consumable consumable2 = null;

    private float nextUseTime1 = 0f;
    private float nextUseTime2 = 0f;

    public UnityEvent<int, Consumable> OnConsumableUsed = new UnityEvent<int, Consumable>();
    public UnityEvent<int, Consumable> OnConsumableEquipped = new UnityEvent<int, Consumable>();
    public UnityEvent<int> OnConsumableUnequipped = new UnityEvent<int>();

    public static ConsumableManager Instance { get; private set; }
    public Consumable Consumable1 { get => consumable1; set => consumable1 = value; }
    public Consumable Consumable2 { get => consumable2; set => consumable2 = value; }

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

    public void EquipConsumable(Consumable consumable)
    {
        int slot;
        if (consumable1 == null)
        {
            consumable1 = consumable;
            slot = 1;
        }
        else if (consumable2 == null)
        {
            consumable2 = consumable;
            slot = 2;
        }
        else
        {
            consumable1.Equipped = false;
            consumable1 = consumable;
            slot = 1;
        }

        consumable.Equipped = true;
        consumable.slot = slot;
        OnConsumableEquipped.Invoke(slot, consumable);
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
        UIManager.Instance.UpdateEquipmentSlotHolder(consumable, true);
    }

    public void UnEquipConsumable(Consumable consumable)
    {
        if (consumable1 != null && consumable1.Name == consumable.Name)
        {
            consumable1.Equipped = false;
            consumable1 = null;
            OnConsumableUnequipped.Invoke(1);
        }
        else if(consumable2 != null && consumable2.Name == consumable.Name)
        {
            consumable2.Equipped = false;
            consumable2 = null;
            OnConsumableUnequipped.Invoke(2);
        }
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
    }

    public void UnEquipAll()
    {
        if (consumable1 != null)
        {
            consumable1.Equipped = false;
            consumable1 = null;
            OnConsumableUnequipped.Invoke(1);
        }
        if (consumable2 != null)
        {
            consumable2.Equipped = false;
            consumable2 = null;
            OnConsumableUnequipped.Invoke(2);
        }
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
    }

    public void UseConsumable1()
    {
        if (consumable1 == null)
        {
            return;
        }

        if (Time.time < nextUseTime1)
        {
            return;
        }
        
        consumable1.UseEffects();

        
        nextUseTime1 = Time.time + consumable1.Cooldown;
        OnConsumableUsed.Invoke(1, consumable1);
    }

    public void UseConsumable2()
    {
        if (consumable2 == null)
        {
            return;
        }

        if (Time.time < nextUseTime2)
        {
            return;
        }

        consumable2.UseEffects();


        nextUseTime2 = Time.time + consumable2.Cooldown;
        OnConsumableUsed.Invoke(2, consumable2);
    }

    public void ShortenCooldown(float amount)
    {
        if (consumable1 != null)
        {
            nextUseTime1 = Mathf.Max(Time.time, nextUseTime1 - amount);
        }

        if (consumable2 != null)
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
        if (slot == 1 && consumable1 != null)
            return Mathf.Max(0f, nextUseTime1 - Time.time);
        if (slot == 2 && consumable2 != null)
            return Mathf.Max(0f, nextUseTime2 - Time.time);
        return 0f;
    }

    public void SetRemainingCooldown(int slot, float remaining)
    {
        if (slot == 1 && consumable1 != null)
        {
            nextUseTime1 = Time.time + remaining;
        }
        else if (slot == 2 && consumable2 != null)
        {
            nextUseTime2 = Time.time + remaining;
        }
    }
}
