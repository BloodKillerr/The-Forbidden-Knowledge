using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    private float moveAmount;
    private float mouseX;
    private float mouseY;

    private Vector2 movementInput;
    private Vector2 cameraInput;

    private PlayerInput playerInput;

    [SerializeField] private string lastUsedDevice = "Keyboard";

    public string LastUsedDevice { get => lastUsedDevice; set => lastUsedDevice = value; }

    public PlayerInput PlayerInput { get => playerInput; set => playerInput = value; }

    public float MoveAmount { get => moveAmount; set => moveAmount = value; }
    public float Vertical { get => vertical; set => vertical = value; }
    public float Horizontal { get => horizontal; set => horizontal = value; }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.controlsChangedEvent.AddListener(UpdateLastUsedDevice);
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
    }

    public void MovementEvent(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void CameraEvent(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
    }

    public void DodgeEvent(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if(!GameManager.Instance.IsGameStatePaused && !Player.Instance.IsDead)
            {
                Player.Instance.GetComponent<PlayerMovement>().HandleDodge();
            }
        }
    }

    public void InteractEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(!GameManager.Instance.IsGameStatePaused && !Player.Instance.IsDead)
            {
                Player.Instance.InteractEvent.Invoke();
            }
        }
    }

    public void InventoryEvent(InputAction.CallbackContext context)
    {
        if (context.performed && !Player.Instance.IsDead)
        {
            UIManager.Instance.ToogleMenu(MenuType.INVENTORY);
        }
    }

    public void PrimaryEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!GameManager.Instance.IsGameStatePaused && !Player.Instance.IsDead)
            {
                Player.Instance.GetComponent<PlayerAttack>().HandleNormalPrimaryAttack();
            }
        }
    }
            
    public void PauseResumeEvent(InputAction.CallbackContext context)
    {
        if (context.performed && !Player.Instance.IsDead && !PauseManager.Instance.RebindingUI.activeSelf)
        {
            UIManager.Instance.ToogleMenu(MenuType.PAUSE);
        }
    }

    public void ConsumableEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!GameManager.Instance.IsGameStatePaused && !Player.Instance.IsDead)
            {
                float direction = context.ReadValue<float>();

                if (direction < 0)
                {
                    ConsumableManager.Instance.UseConsumable1();
                }
                else if (direction > 0)
                {
                    ConsumableManager.Instance.UseConsumable2();
                }
            }
        }
    }

    public void SpellEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!GameManager.Instance.IsGameStatePaused && !Player.Instance.IsDead)
            {
                float direction = context.ReadValue<float>();

                if (direction < 0)
                {
                    SpellManager.Instance.UseSpell1();
                }
                else if (direction > 0)
                {
                    SpellManager.Instance.UseSpell2();
                }
            }
        }
    }

    public void TickInput()
    {
        MoveInput();
    }

    public void MoveInput()
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    public void UpdateLastUsedDevice(PlayerInput playerInput)
    {
        switch (playerInput.currentControlScheme)
        {
            case "KeyBoardMouse":
                lastUsedDevice = "Keyboard";
                break;
            case "XBoxController":
                lastUsedDevice = "XBox";
                break;
            case "PSController":
                lastUsedDevice = "PlayStation";
                break;
        }
    }
}
