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

    public float MoveAmount { get => moveAmount; set => moveAmount = value; }
    public float Vertical { get => vertical; set => vertical = value; }
    public float Horizontal { get => horizontal; set => horizontal = value; }

    private void Awake()
    {
        
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
            Player.Instance.GetComponent<PlayerMovement>().HandleDodge();
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
}
