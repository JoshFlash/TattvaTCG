using System;
using TweenKey;
using TweenKey.Interpolation;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : Slider
{
    [SerializeField] private float barFillSpeed = 2f;
    
    private Transform currentCameraTransform = default;
    private Tween<float> healthTween = default;

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

    public void SetMaxHealth(int maxHealth)
    {
        maxValue = maxHealth;
    }

    public void SetCurrentHealth(int currentHealth)
    {
        TweenToHealth(currentHealth);
    }

    private void TweenToHealth(float currentHealth)
    {
        var tween = TweenCreator.Create((v) => value = v, value, currentHealth, 1 / barFillSpeed,
            LerpFunctions.Float, OffsetFunctions.Float, Easing.Cubic.Out);
        
        if (healthTween is null || healthTween.isExpired)
        {
            healthTween = tween;
            TweenRunner.RunTween(healthTween);
        }
        else
        {
            healthTween.Supersede(tween);
        }
    }
}