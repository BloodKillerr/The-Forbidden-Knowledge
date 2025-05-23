using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ConsumableManager : MonoBehaviour
{
    private Consumable consumable1 = null;
    private Consumable consumable2 = null;

    private float nextUseTime1 = 0f;
    private float nextUseTime2 = 0f;

    public static ConsumableManager Instance { get; private set; }

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
        if (consumable1 == null)
        {
            consumable1 = consumable;
            consumable.slot = 1;
        }
        else if (consumable2 == null)
        {
            consumable2 = consumable;
            consumable.slot = 2;
        }
        else
        {
            consumable1.Equipped = false;
            consumable1 = consumable;
            consumable.slot = 1;
        }

        consumable.Equipped = true;
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
    }

    public void UnEquipConsumable(Consumable consumable)
    {
        if (consumable1 != null && consumable1.Name == consumable.Name)
        {
            consumable1.Equipped = false;
            consumable1 = null;
        }
        else if(consumable2 != null && consumable2.Name == consumable.Name)
        {
            consumable2.Equipped = false;
            consumable2 = null;
        }
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
    }

    public void UnEquipAll()
    {
        if (consumable1 != null)
        {
            consumable1.Equipped = false;
            consumable1 = null;
        }
        if (consumable2 != null)
        {
            consumable2.Equipped = false;
            consumable2 = null;
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
}
