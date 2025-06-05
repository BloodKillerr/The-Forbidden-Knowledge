using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AbilityToolTipUI : MonoBehaviour
{
    [SerializeField] private Image tooltipIcon;
    [SerializeField] private TMP_Text tooltipName;
    [SerializeField] private TMP_Text tooltipDescription;

    [SerializeField] private Vector2 mouseOffset = new Vector2(10, -10);

    [SerializeField] private CanvasGroup canvasGroup;
    private Canvas parentCanvas;
    private RectTransform bgRect;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        bgRect = GetComponent<RectTransform>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (canvasGroup.alpha > 0f)
        {
            FollowCursor();
            ClampToScreen();
        }
    }

    public void Show(Ability ability)
    {
        if (ability == null)
        {
            return;
        }

        tooltipIcon.sprite = ability.Icon;
        tooltipName.text = ability.AbilityName;

        string typeString = ability.type.ToString();

        tooltipDescription.text = $"{ability.Description}\n\nType: {typeString}";

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;

        FollowCursor();
        ClampToScreen();
    }

    public void Show(Ability ability, Vector3 screenPos)
    {
        if (ability == null)
        {
            return;
        }

        tooltipIcon.sprite = ability.Icon;
        tooltipName.text = ability.AbilityName;
        string typeString = ability.type.ToString();
        tooltipDescription.text = $"{ability.Description}\n\nType: {typeString}";

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            screenPos,
            parentCanvas.worldCamera,
            out Vector2 localPoint);

        bgRect.anchoredPosition = localPoint;
        ClampToScreen();
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void FollowCursor()
    {
        Vector2 screenPosition = (Vector2)Input.mousePosition + mouseOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            screenPosition,
            parentCanvas.worldCamera,
            out Vector2 localPoint);

        bgRect.anchoredPosition = localPoint;
    }

    private void ClampToScreen()
    {
        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Vector2.zero,
            parentCanvas.worldCamera,
            out Vector2 bottomLeft);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            new Vector2(Screen.width, Screen.height),
            parentCanvas.worldCamera,
            out Vector2 topRight);

        Vector2 anchoredPosition = bgRect.anchoredPosition;
        Vector2 size = bgRect.sizeDelta;

        if (anchoredPosition.x + size.x > topRight.x)
            anchoredPosition.x = topRight.x - size.x;
        if (anchoredPosition.y - size.y < bottomLeft.y)
            anchoredPosition.y = bottomLeft.y + size.y;
        if (anchoredPosition.x < bottomLeft.x)
            anchoredPosition.x = bottomLeft.x;
        if (anchoredPosition.y > topRight.y)
            anchoredPosition.y = topRight.y;

        bgRect.anchoredPosition = anchoredPosition;
    }
}
