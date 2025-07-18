﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [Serializable]
    public struct Attack
    {
        public string animationName;
        public float damageMultiplier;
        public float attackCooldown;

        [Range(0f, 1f)]
        public float probability;
    }

    [Serializable]
    public class PhaseSettings
    {
        public string entryAnimationName;

        public List<Attack> attacks;

        public float attackRange;

        public float moveSpeed;
    }

    [SerializeField] private List<PhaseSettings> phases = new List<PhaseSettings>();

    private int totalPhases => phases.Count;

    public GameObject PortalPrefab { get => portalPrefab; set => portalPrefab = value; }

    private int currentPhase = 1;
    private List<Attack> currentAttacks;

    private PhaseSettings currentPhaseSettings;
    private BossDamageCollider bossDamageCollider;

    private bool isTransitioning = false;

    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private int nextScene;
    [SerializeField] private bool isFinal = false;

    [SerializeField] private List<string> notes = new List<string>();

    protected override void Awake()
    {
        base.Awake();
        bossDamageCollider = GetComponentInChildren<BossDamageCollider>();

        if (phases.Count > 0)
        {
            ApplyPhaseSettings(0);
        }
    }

    protected override void Update()
    {
        if (isTransitioning)
        {
            return;
        } 

        base.Update();
        UpdatePhase();
    }

    private void UpdatePhase()
    {
        float healthPercent = (float)EnemyStats.CurrentHealth / EnemyStats.MaxHealth.GetValue();
        int newPhase = Mathf.Clamp(
            Mathf.CeilToInt((1 - healthPercent) * totalPhases),
            1,
            totalPhases
        );

        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            StartCoroutine(PhaseTransitionCoroutine(currentPhase - 1));
        }
    }

    private IEnumerator PhaseTransitionCoroutine(int phaseIndex)
    {
        isTransitioning = true;

        agent.isStopped = true;
        animationHandler.Animator.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);

        string entryAnim = phases[phaseIndex].entryAnimationName;
        if (!string.IsNullOrEmpty(entryAnim))
        {
            animationHandler.Animator.CrossFade(entryAnim, 0.2f);

            yield return null;
            AnimatorStateInfo info;
            do
            {
                info = animationHandler.Animator.GetCurrentAnimatorStateInfo(0);
                yield return null;
            }
            while (info.IsName(entryAnim) && info.normalizedTime < 1f);
        }

        ApplyPhaseSettings(phaseIndex);

        if (!attacked)
        {
            agent.isStopped = false;
        }
        isTransitioning = false;
    }

    private void ApplyPhaseSettings(int phaseIndex)
    {
        currentPhaseSettings = phases[phaseIndex];

        agent.speed = currentPhaseSettings.moveSpeed;
        agent.stoppingDistance = currentPhaseSettings.attackRange;
        attackRange = currentPhaseSettings.attackRange;
    }

    public override void AttackAction()
    {
        if (EnemyStats.CurrentHealth <= 0 ||
        currentPhaseSettings == null ||
        currentPhaseSettings.attacks.Count == 0)
        {
            return;
        }

        float totalProb = 0f;
        foreach (Attack attack in currentPhaseSettings.attacks)
        {
            totalProb += attack.probability;
        }
            
        bool useEqual = totalProb <= 0f;
        if (useEqual)
        {
            totalProb = currentPhaseSettings.attacks.Count;
        }

        float roll = UnityEngine.Random.value * totalProb;

        Attack chosen = currentPhaseSettings.attacks[0];
        foreach (Attack attack in currentPhaseSettings.attacks)
        {
            float chunk = useEqual ? 1f : attack.probability;
            if (roll < chunk)
            {
                chosen = attack;
                break;
            }
            roll -= chunk;
        }

        timeBetweenAttacks = chosen.attackCooldown;
        bossDamageCollider.CurrentDamageMultiplier = chosen.damageMultiplier;
        animationHandler.Animator.CrossFade(chosen.animationName, 0.2f);
    }

    public override void Die()
    {
        base.Die();

        if (RoomController is BossRoomController)
        {
            BossRoomController controller = RoomController as BossRoomController;
            BossPortal portal = Instantiate(portalPrefab, controller.PortalSpawnPoint.transform.position,
                controller.PortalSpawnPoint.transform.rotation, RoomController.gameObject.transform).GetComponent<BossPortal>();

            portal.SceneIndex = nextScene;
            portal.isFinal = isFinal;
        }

        UnlockNote();
    }

    private void UnlockNote()
    {
        bool unlocked = false;
        do
        {
            int random = UnityEngine.Random.Range(0, notes.Count);

            if(NotesManager.Instance.UnlockNote(notes[random]))
            {
                unlocked = true;
            }
        } while (!unlocked);
    }
}
