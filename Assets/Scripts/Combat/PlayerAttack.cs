using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerAnimatorHandler animatorHandler;

    private bool isAttacking = false;

    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

    private void Start()
    {
        animatorHandler = GetComponentInChildren<PlayerAnimatorHandler>();

        animatorHandler.Init();
    }

    public void HandleNormalPrimaryAttack()
    {
        Weapon primaryWeapon = Player.Instance.GetComponent<WeaponMeshController>().PrimaryWeapon;

        if (primaryWeapon != null && !Player.Instance.GetComponent<PlayerMovement>().IsDodging)
        {
            animatorHandler.Animator.Play(primaryWeapon.NormalAttackAnimation);
        }
    }

    public void HandleNormalSecondaryAttack()
    {
        Weapon secondaryWeapon = Player.Instance.GetComponent<WeaponMeshController>().SecondaryWeapon;

        if (secondaryWeapon != null && !Player.Instance.GetComponent<PlayerMovement>().IsDodging)
        {
            animatorHandler.Animator.Play(secondaryWeapon.NormalAttackAnimation);
        }
    }

    public void SetAttacking(bool state)
    {
        IsAttacking = state;
    }
}
