using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private Ability ability;

    [SerializeField] private Image icon;

    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void AddAbility(Ability _ability)
    {
        ability = _ability;
        icon.sprite = _ability.Icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ability != null)
        {
            UIManager.Instance.AbilityTooltip.Show(ability);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.AbilityTooltip.Hide();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (ability == null || rectTransform == null || parentCanvas == null)
        {
            return;
        }

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 topCenter = (corners[1] + corners[2]) * 0.5f;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(parentCanvas.worldCamera, topCenter);
        UIManager.Instance.AbilityTooltip.Show(ability, screenPos);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UIManager.Instance.AbilityTooltip.Hide();
    }
}
