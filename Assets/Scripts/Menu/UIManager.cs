using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private EventSystem eventSystem;

    public static UIManager Instance { get; private set; }

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
        eventSystem = EventSystem.current;
    }

    public void ChangeSelectedElement(GameObject toSelect)
    {
        eventSystem.SetSelectedGameObject(toSelect);
    }
}
