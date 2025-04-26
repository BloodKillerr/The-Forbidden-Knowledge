using System;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    private PlayerInputHandler inputHander;
    private Animator animator;

    private UnityEvent interactEvent = new UnityEvent();

    public UnityEvent InteractEvent { get => interactEvent; set => interactEvent = value; }

    public static Player Instance { get; private set; }
    
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
        inputHander = GetComponent<PlayerInputHandler>();
        animator = GetComponentInChildren<Animator>();
    }

    public void SubscribeToInteraction(UnityAction callback)
    {
        interactEvent.AddListener(callback);
    }

    public void UnsubscribeFromInteraction(UnityAction callback)
    {
        interactEvent.RemoveListener(callback);
    }
}
