using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class AutoScrollDynamic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scrollSpeed = 10f;
    private bool mouseOver = false;

    private List<Selectable> m_Selectables = new List<Selectable>();
    private ScrollRect m_ScrollRect;

    private Vector2 m_NextScrollPosition = Vector2.up;

    void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }

    void Start()
    {
        RefreshSelectables();
        ScrollToSelected(true);
    }

    void Update()
    {
        if (!mouseOver)
        {
            InputScroll();
            m_ScrollRect.normalizedPosition = Vector2.Lerp(
                m_ScrollRect.normalizedPosition,
                m_NextScrollPosition,
                scrollSpeed * Time.unscaledDeltaTime
            );
        }
        else
        {
            m_NextScrollPosition = m_ScrollRect.normalizedPosition;
        }
    }

    void InputScroll()
    {
        RefreshSelectables();
        ScrollToSelected(false);
    }

    void RefreshSelectables()
    {
        m_Selectables.Clear();
        m_ScrollRect.content.GetComponentsInChildren(true, m_Selectables);
    }

    void ScrollToSelected(bool quickScroll)
    {
        GameObject selectedObject = null;
        if (EventSystem.current != null)
        {
            selectedObject = EventSystem.current.currentSelectedGameObject;
        }
        if (selectedObject == null) return;

        Selectable selectedElement = selectedObject.GetComponent<Selectable>();
        if (selectedElement == null) return;

        int selectedIndex = m_Selectables.IndexOf(selectedElement);
        int count = m_Selectables.Count;

        if (selectedIndex >= 0 && count > 1)
        {
            float normalizedY = 1 - (selectedIndex / (float)(count - 1));
            Vector2 targetPosition = new Vector2(0, normalizedY);

            if (quickScroll)
            {
                m_ScrollRect.normalizedPosition = targetPosition;
            }

            m_NextScrollPosition = targetPosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }
}
