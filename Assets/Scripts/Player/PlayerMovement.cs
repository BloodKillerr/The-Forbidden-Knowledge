using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Transform cameraTransform;
    private PlayerInputHandler inputHandler;
    private PlayerAnimatorHandler animatorHandler;
    private Rigidbody rb;

    private Vector3 moveDirection;
    private float deltaTime;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private float dodgeForce = 10f;
    [SerializeField] private float dodgeDuration = 0.5f;

    private bool isDodging;

    [SerializeField] private float dodgeRechargeInterval = 3f;
    private Coroutine rechargeCoroutine;

    public Rigidbody Rb { get => rb; set => rb = value; }
    public bool IsDodging { get => isDodging; set => isDodging = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        animatorHandler = GetComponentInChildren<PlayerAnimatorHandler>();
        cameraTransform = Camera.main.transform;

        animatorHandler.Init();
    }

    private void Update()
    {
        deltaTime = Time.deltaTime;
        inputHandler.TickInput();

        animatorHandler.UpdateAnimatorValues(inputHandler.MoveAmount, 0f);
    }

    private void FixedUpdate()
    {
        if (!isDodging && !Player.Instance.GetComponent<PlayerAttack>().IsAttacking && !Player.Instance.IsDead)
        {
            HandleMovement();
            HandleRotation();
        }
    }

    private void HandleMovement()
    {
        movementSpeed = GetComponent<PlayerStats>().MovementSpeed.GetValue();
        moveDirection = cameraTransform.forward * inputHandler.Vertical + cameraTransform.right * inputHandler.Horizontal;
        moveDirection.y = 0f;

        if (moveDirection.sqrMagnitude > 0f)
        {
            moveDirection.Normalize();
        }

        Vector3 desiredVelocity = moveDirection * movementSpeed;
        Vector3 currentVel = rb.linearVelocity;
        Vector3 velocityChange = desiredVelocity - new Vector3(currentVel.x, 0f, currentVel.z);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void HandleRotation()
    {
        if (!animatorHandler.CanRotate)
        {
            return;
        }

        Vector3 targetDir = cameraTransform.forward * inputHandler.Vertical + cameraTransform.right * inputHandler.Horizontal;
        targetDir.y = 0f;

        if (targetDir.sqrMagnitude == 0f)
        {
            targetDir = transform.forward;
        } 
        else
        {
            targetDir.Normalize();
        }   

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        Quaternion smoothed = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * deltaTime);

        rb.MoveRotation(smoothed);
    }
    public void HandleDodge()
    {
        if (isDodging || Player.Instance.GetComponent<PlayerAttack>().IsAttacking)
        {
            return;
        }

        PlayerStats stats = GetComponent<PlayerStats>();

        StartCoroutine(PerformDodge(stats));
    }

    private IEnumerator PerformDodge(PlayerStats stats)
    {
        if(stats.UseDodgeCharge())
        {
            isDodging = true;

            animatorHandler.Animator.SetTrigger("Dodge");

            Vector3 dodgeDir = moveDirection.sqrMagnitude > 0f ? moveDirection : transform.forward;
            dodgeDir.Normalize();

            rb.AddForce(dodgeDir * dodgeForce, ForceMode.VelocityChange);

            if (rechargeCoroutine == null)
            {
                rechargeCoroutine = StartCoroutine(RechargeDodge(stats));
            }

            yield return new WaitForSeconds(dodgeDuration);

            isDodging = false;
        }
    }

    private IEnumerator RechargeDodge(PlayerStats stats)
    {
        while (stats.CurrentDodgeCharges < stats.MaxDodgeCharges.GetValue())
        {
            yield return new WaitForSeconds(dodgeRechargeInterval);
            stats.CurrentDodgeCharges++;
            stats.DodgeChargesChanged?.Invoke(stats.CurrentDodgeCharges, stats.MaxDodgeCharges.GetValue());
        }

        rechargeCoroutine = null;
    }

    public void RestoreDodgeCharges()
    {
        if (rechargeCoroutine == null)
        {
            PlayerStats stats = GetComponent<PlayerStats>();
            rechargeCoroutine = StartCoroutine(RechargeDodge(stats));
        }
    }
}
