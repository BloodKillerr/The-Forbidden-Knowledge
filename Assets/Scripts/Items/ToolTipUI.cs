using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipUI : MonoBehaviour
{
    [SerializeField] private Image tooltipIcon;
    [SerializeField] private TMP_Text tooltipName;
    [SerializeField] private Transform tooltipContent;
    [SerializeField] private GameObject tooltipLinePrefab;

    [SerializeField] private Vector2 mouseOffset = new Vector2(10, -10);

    [SerializeField] private CanvasGroup canvasGroup;

    private Canvas parentCanvas;
    private RectTransform bgRect;
    private Item currentItem;
    private Vector3? forcedPosition;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        bgRect = GetComponent<RectTransform>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (canvasGroup.alpha > 0f && !forcedPosition.HasValue)
        {
            FollowCursor();
            ClampToScreen();
        }
    }

    public void Show(Item item)
    {
        currentItem = item;
        forcedPosition = null;

        Populate(item);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;

        FollowCursor();
        ClampToScreen();
    }

    public void Show(Item item, Vector3 screenPos)
    {
        currentItem = item;
        forcedPosition = screenPos;

        Populate(item);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            screenPos,
            parentCanvas.worldCamera,
            out Vector2 localPoint
        );
        bgRect.anchoredPosition = localPoint;
        ClampToScreen();

        forcedPosition = null;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        forcedPosition = null;
    }

    private void Populate(Item item)
    {
        tooltipIcon.sprite = item.Icon;
        tooltipName.text = item.Name;

        foreach (Transform child in tooltipContent)
            Destroy(child.gameObject);

        foreach (var (label, value) in item.GetTooltipData())
        {
            var line = Instantiate(tooltipLinePrefab, tooltipContent);
            line.transform.Find("Label").GetComponent<TMP_Text>().text = $"{label}:";
            line.transform.Find("Value").GetComponent<TMP_Text>().text = value;
        }
    }

    private void FollowCursor()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            (Vector2)Input.mousePosition + mouseOffset,
            parentCanvas.worldCamera,
            out Vector2 localPoint
        );
        bgRect.anchoredPosition = localPoint;
    }

    private void ClampToScreen()
    {
        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Vector2.zero,
            parentCanvas.worldCamera,
            out Vector2 bottomLeft
        );
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            new Vector2(Screen.width, Screen.height),
            parentCanvas.worldCamera,
            out Vector2 topRight
        );

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
