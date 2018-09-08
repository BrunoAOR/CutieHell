﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttackCooldown : AIAttackLogic
{
    #region Fields
    public EnemyType enemyType;
    public float attackRange;
    public float attackDamage;
    [Tooltip("Time expressed in seconds.")]
    public float cooldownDuration;

    public Transform attackSpawnPoint;
    public EnemyRangeAttack attackPrefab;
    private IDamageable attackTarget = null;

    private float lastAttackTime = 0;
    private Animator animator;
    public ParticleSystem heartShotVFX;

    [SerializeField]
    private AudioSource bearAttackSource;
    [SerializeField]
    private AudioClip bearAttackClip;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        animator = GetComponent<Animator>();
        bearAttackSource.clip = bearAttackClip;
    }
    #endregion

    #region Public Methods
    public override void AttemptAttack(IDamageable attackTarget, Vector3 navigationTarget)
    {
        if (IsInAttackRange(navigationTarget))
        {
            if (Time.time - lastAttackTime > cooldownDuration)
            {
                animator.SetTrigger("Attack");
                this.transform.LookAt(attackTarget.transform.position);
                /* Actual attack will be launched by the Animation */
                this.attackTarget = attackTarget;
                lastAttackTime = Time.time;
            }
        }
    }

    public override bool IsInAttackRange(Vector3 navigationTarget)
    {
        return Vector3.Distance(this.transform.position, navigationTarget) < attackRange;
    }

    public void LaunchAttack()
    {
        if (attackTarget != null && attackTarget.IsTargetable() && IsPositionWithinScaledRange(attackTarget.transform.position, 2.0f))
        {
            Attack(attackTarget);
        }
        attackTarget = null;
    }
    #endregion

    #region Private Methods
    private void Attack(IDamageable target)
    {
        EnemyRangeAttack currentAttack = AttacksPool.instance.GetAttackObject(enemyType, attackSpawnPoint.position, attackSpawnPoint.rotation).GetComponent<EnemyRangeAttack>();
        ParticlesManager.instance.LaunchParticleSystem(heartShotVFX, attackSpawnPoint.position, attackSpawnPoint.rotation);
        currentAttack.Fire(target, attackDamage);
        SoundManager.instance.PlaySfxClip(bearAttackSource, bearAttackClip,true);
    }

    private bool IsPositionWithinScaledRange(Vector3 position, float rangeScale)
    {
        Vector3 thisToTarget = position - transform.position;
        return thisToTarget.sqrMagnitude < (attackRange * rangeScale) * (attackRange * rangeScale);
    }
    #endregion
}
