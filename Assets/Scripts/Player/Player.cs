using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerInputHandler inputHander;
    private Animator animator;

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

    private void Update()
    {

    }
}
