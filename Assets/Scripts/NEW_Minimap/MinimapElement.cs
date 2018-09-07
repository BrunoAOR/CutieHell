﻿using UnityEngine;

public class MinimapElement : MonoBehaviour
{
    public Sprite sprite;
    public Color color = Color.white;
    [Tooltip("The size in pixels of the sprite shown in the minimap.")]
    public int size = 20;
    [Tooltip("Elements with a higher priority number are drawn on top.")]
    public int priority = 0;

    private bool hasStarted = false;

    #region MonoBehaviour Methods
    private void Start()
    {
        hasStarted = true;
        MinimapController.instance.AddMinimapElement(this);
    }

    private void OnEnable()
    {
        if (hasStarted)
            MinimapController.instance.AddMinimapElement(this);
    }

    private void OnDisable()
    {
        MinimapController.instance.RemoveMinimapElement(this);
    }
    #endregion
}
