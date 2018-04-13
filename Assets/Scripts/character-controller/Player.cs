﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    #region Fields
    [Header("Movement Variabes")]
    public float maxSpeed = 10;
    public float acceleration = 50;
    [SerializeField]
    private Transform centerTeleportPoint;
    [SerializeField]
    private Transform statueTeleportPoint;
    public GameObject footSteps;

    [Header("Evilness")]
    [SerializeField]
    private int maxEvilLevel = 50;
    public int evilLevel;

    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Vector3 initialBulletSpawnPointPos;
    [HideInInspector]
    public float timeSinceLastTrapUse;
    [HideInInspector]
    public MeshRenderer meshRenderer;

    [Header("Attacks")]
    [SerializeField]
    public Transform bulletSpawnPoint;

    [Header("Actual Trap")]
    [HideInInspector]
    public Trap[] allTraps;
    public Trap nearbyTrap;
    public Trap currentTrap;
    [SerializeField]
    private int trapUseCooldown;
    public float trapMaxUseDistance;
    [HideInInspector]
    public bool shouldExitTrap = false;

    [Header("Player States")]
    [SerializeField]
    private State currentState;
    public PlayerStates state;
    [HideInInspector]
    public float timeSinceLastAttack;
    [HideInInspector]
    public AIEnemy currentBasicAttackTarget = null;

    #endregion

    public enum PlayerStates { STILL, MOVE, WOLF, FOG, TURRET}

    #region MonoBehaviour Methods
    private void Awake() 
    {
        state = PlayerStates.MOVE;
        initialBulletSpawnPointPos = new Vector3(0.8972f, 1.3626f, 0.1209f);
        meshRenderer = this.GetComponentInChildren<MeshRenderer>();
        rb = this.GetComponent<Rigidbody>();
        GameObject[] allTrapsGameObjects = GameObject.FindGameObjectsWithTag("Traps");
        allTraps = new Trap[allTrapsGameObjects.Length];
        for (int i = 0; i < allTrapsGameObjects.Length; ++i)
        {
            allTraps[i] = allTrapsGameObjects[i].GetComponent<Trap>();
        }
    }

    private void Start () 
    {
        footSteps.SetActive(false);
        evilLevel = maxEvilLevel;
        timeSinceLastTrapUse = trapUseCooldown;
        timeSinceLastAttack = 1000.0f;
    }

    private void Update() 
    {
        if (GameManager.instance.gameIsPaused)
        {
            return;
        }
        currentState.UpdateState(this);

        if (InputManager.instance.GetL1ButtonDown() && state == PlayerStates.MOVE)
            transform.position = statueTeleportPoint.position;

        if (InputManager.instance.GetR1ButtonDown() && state == PlayerStates.MOVE)
            transform.position = centerTeleportPoint.position;

    }
    #endregion

    #region Public Methods
    public virtual void TransitionToState(State targetState)
    {
        for (int i = 0; i < currentState.onExitActions.Length; ++i)
            currentState.onExitActions[i].Act(this);

        for (int i = 0; i < targetState.onEnterActions.Length; ++i)
            targetState.onEnterActions[i].Act(this);

        currentState = targetState;
    }

    public void StopTrapUse() 
    {
        shouldExitTrap = true;
    }

    public int GetMaxEvilLevel() 
    {
        return maxEvilLevel;
    }

    public int GetEvilLevel()
    {
        return evilLevel;
    }

    public void SetEvilLevel(int value)
    {
        evilLevel += value;

        if (evilLevel < 0)
        {
            evilLevel = 0;
        }
        else if (evilLevel > maxEvilLevel)
        {
            evilLevel = maxEvilLevel;
        }

        UIManager.instance.SetEvilBarValue(evilLevel);
    }
    #endregion
}