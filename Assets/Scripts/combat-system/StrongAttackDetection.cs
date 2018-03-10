﻿using UnityEngine;

public class StrongAttackDetection : MonoBehaviour
{
    #region Fields

    private const float actionTimeRange = 1f;
    private const float strongAttackCadency = 5f;
    private const int evilCost = -10;
    private const int damage = 10;

    private float actionTime = 0f;
    private float cadencyTime = strongAttackCadency;
    private bool activateAttack = false;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour Methods

    private void Update()
    {
        StrongAttackActivation();
    }

    private void OnTriggerStay(Collider other)
    {
        if (activateAttack)
        {
            if (other.gameObject.layer == 8)
            {
                other.GetComponent<AIEnemy_temp>().TakeDamage(damage, AttackType.STRONG);
            }
        }
    }

	#endregion
	
	#region Public Methods
	
	#endregion
	
	#region Private Methods

    private void StrongAttackActivation()
    {
        if (InputManager.instance.GetL2ButtonDown() && !activateAttack && GameManager.instance.GetPlayerOne().GetEvilLevel() >= Mathf.Abs(evilCost) && cadencyTime >= strongAttackCadency)
        {
            if (actionTime < actionTimeRange)
            {
                activateAttack = true;
                GameManager.instance.GetPlayerOne().SetEvilLevel(evilCost);
                GetComponent<MeshCollider>().enabled = true;
                GetComponent<Renderer>().enabled = true;
            }
        }

        if (activateAttack)
        {
            actionTime += Time.deltaTime;
        }

        if (actionTime >= actionTimeRange)
        {
            activateAttack = false;
            actionTime = 0f;
            cadencyTime = 0f;
            GetComponent<MeshCollider>().enabled = false;
            GetComponent<Renderer>().enabled = false;
        }

        cadencyTime += Time.deltaTime;
    }

	#endregion
}