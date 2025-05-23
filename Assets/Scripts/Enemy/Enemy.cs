using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    protected EnemyStats enemyStats;

    protected EnemyLoot enemyLoot;

    protected NavMeshAgent agent;

    protected EnemyAnimationHandler animationHandler;

    public LayerMask PlayerLayer;

    [SerializeField] protected float timeBetweenAttacks;
    protected bool attacked;

    [SerializeField] protected float attackRange;
    protected bool inAttackRange;

    [SerializeField] protected float rotationSpeed = 25f;

    [SerializeField] private DamageCollider damageCollider;

    protected Coroutine freezeRoutine;
    protected bool isFrozen = false;
    protected float originalRotationSpeed;

    public RoomController RoomController;

    public static UnityEvent<Enemy> OnEnemyKilled = new UnityEvent<Enemy>();

    public DamageCollider DamageCollider { get => damageCollider; set => damageCollider = value; }

    protected virtual void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        enemyLoot = GetComponent<EnemyLoot>();
        agent = GetComponent<NavMeshAgent>();
        animationHandler = GetComponentInChildren<EnemyAnimationHandler>();
        agent.updateRotation = false;
        damageCollider = GetComponentInChildren<DamageCollider>();
        originalRotationSpeed = rotationSpeed;
    }

    protected virtual void Update()
    {
        if (isFrozen)
        {
            return;
        }

        if (enemyStats.IsInvincible)
        {
            agent.isStopped = true;
            return;
        }
        agent.isStopped = false;

        inAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerLayer);

        if (inAttackRange)
        {
            Attack();
        }
        else if(!attacked)
        {
            Chase();
        }
    }

    private void Chase()
    {
        if (enemyStats.CurrentHealth <= 0)
        {
            agent.ResetPath();
            return;
        }

        if(!attacked)
        {
            agent.SetDestination(Player.Instance.gameObject.transform.position);
        }
        

        if(animationHandler.Animator != null && !attacked)
        {
            animationHandler.Animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        }

        FaceMovementDirection();
    }

    private void Attack()
    {
        if (enemyStats.CurrentHealth <= 0)
        {
            agent.ResetPath();
            return;
        }
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        animationHandler.Animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);

        Vector3 lookPos = Player.Instance.transform.position;
        lookPos.y = transform.position.y;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(lookPos - transform.position),
            rotationSpeed * Time.deltaTime
        );

        if (!attacked)
        {
            AttackAction();

            attacked = true;
            Invoke(nameof(ResetAtack), timeBetweenAttacks);
        }
    }

    private void ResetAtack()
    {
        attacked = false;
        agent.isStopped = false;
    }

    private void FaceMovementDirection()
    {
        Vector3 vel = agent.velocity;
        vel.y = 0;
        if (vel.sqrMagnitude > 0.01f)
        {
            Quaternion desired = Quaternion.LookRotation(vel);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desired,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public virtual void AttackAction()
    {
        animationHandler.Animator.CrossFade("Attack", 0.2f);
    }

    public void GetLoot()
    {
        enemyLoot.GetLoot();
    }

    private void OnDestroy()
    {
        OnEnemyKilled?.Invoke(this);
    }

    public void Freeze(float duration)
    {
        if (freezeRoutine != null)
        {
            StopCoroutine(freezeRoutine);
        }            

        freezeRoutine = StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        agent.isStopped = true;
        animationHandler.Animator.speed = 0f;
        rotationSpeed = 0f;

        yield return new WaitForSeconds(duration);

        isFrozen = false;
        agent.isStopped = false;
        animationHandler.Animator.speed = 1f;
        rotationSpeed = originalRotationSpeed;

        freezeRoutine = null;
    }

    public virtual void Die()
    {
        //Actions on death
    }
}
