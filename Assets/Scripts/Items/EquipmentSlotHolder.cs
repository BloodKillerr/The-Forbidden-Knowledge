using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotHolder : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void AddItem(Item item)
    {
        icon.enabled = true;
        icon.sprite = item.Icon;
    }

    public void RemoveItem()
    {
        icon.enabled = false;
        icon.sprite = null;
    }
}
