﻿using UnityEngine;
using UnityEngine.UI;

public class MaxCombo : Combo {

    #region Attributes
    private float maxComboTime;
    [SerializeField]
    private float maxComboLimitTime;
    private int maxComboRecord;
    [SerializeField]
    private Image maxComboUITimer;
    [SerializeField]
    private Text maxComboUICounter;
    #endregion

    #region MonoBehaviour methods
    public void Update()
    {
        if(comboEnabled)
            ReviewConditions();
            UpdateUIMaxComboTimer();
    }
    #endregion

    #region Public methods
    public override void ReviewConditions()
    {
        if (comboEnabled)
        {
            maxComboTime += Time.deltaTime;

            if (maxComboTime >= maxComboLimitTime)
            {
                comboEnabled = false;
                ResetCount();
            }
        }
    }

    public override void IncreaseCurrentCount(int addToCount)
    {
        if(currentCount == 0)
            maxComboUITimer.gameObject.SetActive(true);

        currentCount += addToCount;
        UpdateUIMaxComboCounter();
        maxComboTime = 0f;
    }

    public override void ResetCount()
    {
        maxComboTime = 0f;

        if (currentCount > maxComboRecord)
            maxComboRecord = currentCount;

        currentCount = 0;

        maxComboUITimer.gameObject.SetActive(false);
        maxComboUITimer.fillAmount = 1;
    }
    #endregion

    #region Protected methods
    protected override void GrantReward()
    {
        Debug.Log("TODO: GrantReward() from: " + comboName + "has to be called at the end of the game");
        StatsManager.instance.IncreaseGlobalPoints(reward * maxComboRecord);
    }
    #endregion

    #region Private methods
    private void UpdateUIMaxComboTimer()
    {
        maxComboUITimer.fillAmount = 1 - (maxComboTime / maxComboLimitTime);
    }

    private void UpdateUIMaxComboCounter()
    {
        maxComboUICounter.text = currentCount.ToString();
    }
    #endregion


}