using UnityEngine;
using UnityEngine.Events;

public class EquipmentManager : MonoBehaviour
{
    private Equipment[] currentEquipment;

    public UnityEvent<Equipment, Equipment> EquipmentChanged = new UnityEvent<Equipment, Equipment>();

    public static EquipmentManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
    }

    public void Equip(Equipment item)
    {
        int slotIndex = (int)item.EqSlot;

        Equipment oldItem = null;

        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            oldItem.Equipped = false;
        }

        currentEquipment[slotIndex] = item;
        item.Equipped = true;
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
        EquipmentChanged.Invoke(item, oldItem);
    }

    public void UnEquip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];
            oldItem.Equipped = false;
            currentEquipment[slotIndex] = null;
            UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
            EquipmentChanged.Invoke(null, oldItem);
        }
    }

    public void UnEquipAll()
    {
        for(int i = 0; i < currentEquipment.Length; i++)
        {
            UnEquip(i);
        }
        UIManager.Instance.UpdateInventoryUI(UIManager.Instance.CurrentInventoryTab);
    }
}
