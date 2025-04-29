using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Item item;

    [SerializeField] private Image icon;

    [SerializeField] private Image equippedFrame;

    public void AddItem(Item _item)
    {
        item = _item;
        icon.sprite = _item.Icon;
    }

    public void UseItem()
    {
        if(item != null)
        {
            UIManager.Instance.LastUsedItem = item;
            item.Use();
        }
    }

    public void UpdateEquippedUI()
    {
        equippedFrame.enabled = item.UpdateUIState();
    }
}
