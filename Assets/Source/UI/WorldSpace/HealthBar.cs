using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : Slider
{
    private Transform currentCameraTransform = default;

    public void SetCamera(Camera cam = null)
    {
        if (Application.isPlaying)
        {
            cam ??= Camera.main;
            currentCameraTransform = cam.transform;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        transition = Transition.None;
        navigation = new Navigation { mode = Navigation.Mode.None };
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetCamera();
    }

    private void LateUpdate()
    {
        if (Application.isPlaying)
        {
            LookTowardCurrentCamera();
        }
    }

    private void LookTowardCurrentCamera()
    {
        transform.LookAt(transform.position - currentCameraTransform.forward);
    }

    public void SetMaxHealth(float maxHealth)
    {
        maxValue = maxHealth;
    }

    public void SetCurrentHealth(float currentHealth)
    {
        value = currentHealth;
    }
}