using System;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    private Animator animator;

    private bool isDead = false;

    private UnityEvent interactEvent = new UnityEvent();

    public UnityEvent InteractEvent { get => interactEvent; set => interactEvent = value; }

    public static Player Instance { get; private set; }
    public bool IsDead { get => isDead; set => isDead = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
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
