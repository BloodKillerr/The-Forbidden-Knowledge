using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private Item item;

    [SerializeField] private Image icon;

    [SerializeField] private Image equippedFrame;

    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            UIManager.Instance.Tooltip.Show(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.Tooltip.Hide();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (item == null || rectTransform == null || parentCanvas == null)
        {
            return;
        }

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 topCenter = (corners[1] + corners[2]) * 0.5f;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(parentCanvas.worldCamera, topCenter);
        UIManager.Instance.Tooltip.Show(item, screenPos);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UIManager.Instance.Tooltip.Hide();
    }
}
