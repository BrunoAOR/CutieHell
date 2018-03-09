﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    [Header("Speed Variabes")]
    public float maxSpeed = 10;
    public float acceleration = 50;

    private Rigidbody rb;
    private Vector3 speedDirection;
    private GameObject[] traps;
    private const int maxEvilLevel = 20;
    public PlayerStates state, nextState;

    public enum PlayerStates { STILL, MOVE, WOLF, FOG, TURRET}

	void Start () 
    {
        rb = this.GetComponent<Rigidbody>();
        state = nextState = PlayerStates.MOVE;
        ResetTrapList();
    }

    private void Update() 
    {
        UseTrap();
    }

    private void FixedUpdate() 
    {
        switch (state) 
        {
            case PlayerStates.STILL:
                break;
            case PlayerStates.MOVE:
                MovePlayer();
                break;
            case PlayerStates.WOLF:
                break;
            case PlayerStates.FOG:
                break;
            case PlayerStates.TURRET:
                break;
            default:
                break;
        }
        state = nextState;
    }

    public void StopTrapUse() 
    {

    }

    private void MovePlayer() 
    {
        speedDirection = Vector3.zero;

        if (InputManager.instance.GetLeftStickUp()) {
            speedDirection += new Vector3(0.0f, 0.0f, 0.5f);
        }
        if (InputManager.instance.GetLeftStickDown()) {
            speedDirection += new Vector3(0.0f, 0.0f, -0.5f);
        }
        if (InputManager.instance.GetLeftStickLeft()) {
            speedDirection += new Vector3(-0.5f, 0.0f, 0.0f);
        }
        if (InputManager.instance.GetLeftStickRight()) {
            speedDirection += new Vector3(0.5f, 0.0f, 0.0f);
        }

        if (speedDirection.magnitude > 0.0f) {
            rb.drag = 0.0f;
        } else {
            rb.drag = 10.0f;
        }

        rb.AddRelativeForce(speedDirection * acceleration, ForceMode.Acceleration);

        if (rb.velocity.magnitude > maxSpeed) {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    public void UseTrap() 
    {
        for(int i = 0; i < traps.Length; i++) 
        {
            if(Vector3.Distance(this.transform.position, traps[i].transform.position) < 3.0f) {
                if (InputManager.instance.GetXButtonDown()) 
                {
                    //Trap trapScript = traps[i].GetComponent<Trap>();
                    //if (trapScript.CanUse()) 
                    //{
                    //    this.transform.position = traps[i].transform.position;
                    //    trapScript.Activate(this);
                    //    nextState = PlayerStates.TURRET;
                    //}
                    this.transform.position = traps[i].transform.position;
                    //trapScript.Activate(this);
                    nextState = PlayerStates.TURRET;
                }
            }
        }
    }

    public void ResetTrapList() 
    {
        traps = null;
        traps = GameObject.FindGameObjectsWithTag("Traps");
    }

    public int GetMaxEvilLevel() 
    {
        return maxEvilLevel;
    }
}
