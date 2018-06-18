﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InitGame : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private MenuButton[] buttons;

    [Header("Alternate screens")]
    [SerializeField]
    private Image helpScreen;
    [SerializeField]
    private Image creditsScreen;

    [Header("Faders")]
    [SerializeField]
    private ScreenFadeController blackFader;
    [SerializeField]
    private ScreenFadeController foregroundFader;

    private bool menuActive = false;
    private int index = 0;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        UnityEngine.Assertions.Assert.IsNotNull(blackFader, "ERROR: ScreenFadeController (blackFader) not assigned for InitGame in GameObject '" + gameObject.name + "'!");
        UnityEngine.Assertions.Assert.IsNotNull(foregroundFader, "ERROR: ScreenFadeController (foregroundFader) not assigned for InitGame in GameObject '" + gameObject.name + "'!");
    }

    private void Start()
    {
        helpScreen.enabled = false;
        creditsScreen.enabled =      false;
        blackFader.TurnOpaque();
        foregroundFader.TurnOpaque();
        blackFader.FadeToTransparent(OnFadedIn);
    }

    private void Update()
    {
        if (menuActive)
        {
            ProcessToggleBetweenButtons();
            ProcessButtonClick();
        }
        else
        {
            ProcessBackButton();
        }
    }

    #endregion

    #region Public Methods
    public void OnFadedIn()
    {
        foregroundFader.FadeToTransparent(OnMenuShown);
    }

    public void OnMenuShown()
    {
        menuActive = true;
    }
    #endregion

    #region Private Methods


    private void ProcessToggleBetweenButtons()
    {
        if (InputManager.instance.GetPadDownDown() || InputManager.instance.GetLeftStickDownDown())
        {
            buttons[index].UnselectButton();

            if (index == buttons.Length - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }

            buttons[index].SelectButton();
        }
        else if (InputManager.instance.GetPadUpDown() || InputManager.instance.GetLeftStickUpDown())
        {
            buttons[index].UnselectButton();

            if (index == 0)
            {
                index = buttons.Length - 1;
            }
            else
            {
                index--;
            }

            buttons[index].SelectButton();
        }
    }

    private void ProcessButtonClick()
    {
        if (InputManager.instance.GetXButtonDown())
        {
            switch (index)
            {
                case 0:
                    SceneManager.LoadScene("Game", LoadSceneMode.Single);
                    break;

                case 1:
                    menuActive = false;
                    helpScreen.enabled = true;
                    break;

                case 2:
                    menuActive = false;
                    creditsScreen.enabled = true;
                    break;

                case 3:
                    Application.Quit();
                    break;
            }
        }

    }

    private void ProcessBackButton()
    {
        if (InputManager.instance.GetOButtonDown())
        {
            menuActive = true;
            helpScreen.enabled = false;
            creditsScreen.enabled = false;
        }
    }
	#endregion
}