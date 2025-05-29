using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimatorHandler : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private bool canRotate;

    private PlayerInputHandler inputHandler;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    public bool CanRotate { get => canRotate; set => canRotate = value; }
    public Animator Animator { get => animator; set => animator = value; }

    public void Init()
    {
        animator = GetComponent<Animator>();
        inputHandler = GetComponentInParent<PlayerInputHandler>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
    {
        float v = 0;

        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            v = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            v = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            v = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }

        float h = 0;

        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            h = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            h = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            h = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            h = -1;
        }
        else
        {
            h = 0;
        }

        animator.SetFloat("Vertical", v, 0.1f, Time.deltaTime);
        animator.SetFloat("Horizontal", h, 0.1f, Time.deltaTime);
    }

    public void StartRotation()
    {
        canRotate = true;
    }

    public void StopRotation()
    {
        canRotate = false;
    }

    public void StartAttack()
    {
        playerAttack.SetAttacking(true);
        Player.Instance.GetComponent<WeaponMeshController>().AttackWithPrimaryWeapon();
    }

    public void StopAttack()
    {
        playerAttack.SetAttacking(false);
        playerAttack.OnAttackAnimationComplete();
        Player.Instance.GetComponent<WeaponMeshController>().HolsterPrimaryWeapon();
    }
    public void EnableComboWindow()
    {
        playerAttack.CanAttack = true;

        if(playerAttack.InputBuffered)
        {
            playerAttack.FireComboStep();
        }
    }

    public void EnableIFrames()
    {
        Player.Instance.GetComponent<PlayerStats>().IsInvincible = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowInvincibilityFrame();
        }
    }

    public void DisableIFrames()
    {
        Player.Instance.GetComponent<PlayerStats>().IsInvincible = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideInvincibilityFrame();
        }
    }

    public void DisableComboWindow()
    {
        playerAttack.CanAttack = false;
    }

    public void EnablePrimaryDamageCollider()
    {
        Player.Instance.GetComponent<WeaponMeshController>().EnablePrimaryDamageCollider();
    }

    public void DisablePrimaryDamageCollider()
    {
        Player.Instance.GetComponent<WeaponMeshController>().DisablePrimaryDamageCollider();
    }

    public void Die()
    {
        //Debug only
        SceneManager.LoadScene(1);
    }
}
