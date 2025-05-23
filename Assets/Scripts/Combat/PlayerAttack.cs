using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerAnimatorHandler animatorHandler;

    private bool isAttacking = false;

    private int comboIndex = 0;
    private float lastAttackTime;
    private bool canAttack = true;
    private bool inputBuffered = false;

    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool CanAttack { get => canAttack; set => canAttack = value; }
    public bool InputBuffered { get => inputBuffered; set => inputBuffered = value; }

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
            if(!canAttack)
            {
                if(comboIndex < primaryWeapon.AttackAnimations.Length - 1)
                {
                    inputBuffered = true;
                }
                
                return;
            }

            FireComboStep(primaryWeapon);
        }
    }

    public void FireComboStep(Weapon primaryWeapon)
    {
        if (Time.time - lastAttackTime <= primaryWeapon.ComboResetTime && comboIndex < primaryWeapon.AttackAnimations.Length - 1)
        {
            comboIndex++;
        }
        else
        {
            comboIndex = 0;
        }
        animatorHandler.Animator.CrossFade(primaryWeapon.AttackAnimations[comboIndex], .2f);
        lastAttackTime = Time.time;
        canAttack = false;
        inputBuffered = false;
    }

    public void FireComboStep()
    {
        Weapon primaryWeapon = Player.Instance.GetComponent<WeaponMeshController>().PrimaryWeapon;
        if (Time.time - lastAttackTime <= primaryWeapon.ComboResetTime && comboIndex < primaryWeapon.AttackAnimations.Length - 1)
        {
            comboIndex++;
        }
        else
        {
            comboIndex = 0;
        }
        animatorHandler.Animator.CrossFade(primaryWeapon.AttackAnimations[comboIndex], .2f);
        lastAttackTime = Time.time;
        canAttack = false;
        inputBuffered = false;
    }

    public void OnAttackAnimationComplete()
    {
        canAttack = true;
        comboIndex = 0;
        inputBuffered = false;
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
