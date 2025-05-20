using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerAnimatorHandler animatorHandler;

    private bool isAttacking = false;

    private int comboIndex = 0;
    private float lastAttackTime;
    private bool canAttack = true;

    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool CanAttack { get => canAttack; set => canAttack = value; }

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
            if(canAttack)
            {
                if (Time.time - lastAttackTime <= primaryWeapon.ComboResetTime && comboIndex < primaryWeapon.AttackAnimations.Length - 1)
                {
                    comboIndex++;
                }
                else
                {
                    comboIndex = 0;
                }
                animatorHandler.Animator.Play(primaryWeapon.AttackAnimations[comboIndex]);
                lastAttackTime = Time.time;
                canAttack = false;
            }   
        }
    }

    public void OnAttackAnimationComplete()
    {
        canAttack = true;
        if (comboIndex >= Player.Instance.GetComponent<WeaponMeshController>().PrimaryWeapon.AttackAnimations.Length -1)
        {
            comboIndex = 0;
        }
    }

    public void HandleNormalSecondaryAttack()
    {
        Weapon secondaryWeapon = Player.Instance.GetComponent<WeaponMeshController>().SecondaryWeapon;

        if (secondaryWeapon != null && !Player.Instance.GetComponent<PlayerMovement>().IsDodging)
        {
            animatorHandler.Animator.Play(secondaryWeapon.AttackAnimations[0]);
        }
    }

    public void SetAttacking(bool state)
    {
        IsAttacking = state;
    }
}
