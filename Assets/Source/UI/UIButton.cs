using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum ButtonCallbackTarget { Component, GameService, Static }

public class UIButton : Button
{
	public bool DisableButtonOnClick = false;
	
	public ButtonCallbackTarget CallbackTarget = ButtonCallbackTarget.GameService;
	public GameObject CallbackTargetObject = default;
	
	public string CallbackMethodName = string.Empty;

	protected override void OnEnable()
	{
		base.OnEnable();
		onClick.AddListener(InvokeCallbackFromMethodName);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		onClick.RemoveListener(InvokeCallbackFromMethodName);
	}
	
	private void InvokeCallbackFromMethodName()
	{
		if (!string.IsNullOrEmpty(CallbackMethodName))
		{
			string[] splitMethodName = CallbackMethodName.Split('.');
			if (splitMethodName.Length < 2)
			{
				Log.Info("UIButton callbacks must include the name of the target service or class and a method name\n" +
				         "(e.g. PhotonGameService.Connect)", this.GetType().ToString(), LogType.Error);
				return;
			}

			if (splitMethodName.Length > 2)
			{
				// todo - add support for subclasses/namespaces
				Log.Info("UIButton callbacks to subclasses or namespaces is currently unsupported. Specify only a class and method\n" +
				         "(e.g. PhotonGameService.Connect)", this.GetType().ToString(), LogType.Error);
				return;
			}

			switch (CallbackTarget)
			{
				case ButtonCallbackTarget.Component:
					var component = CallbackTargetObject?.GetComponent(splitMethodName[0]);
					InvokeCallbackOnInstance(splitMethodName[1], component);
					break;
				case ButtonCallbackTarget.GameService:
					var service = GameServices.GetService(splitMethodName[0]);
					InvokeCallbackOnInstance(splitMethodName[1], service);
					break;
				case ButtonCallbackTarget.Static:
					Type.GetType(splitMethodName[0])?.GetMethod(splitMethodName[1])?.Invoke(null, null);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		if (DisableButtonOnClick)
		{
			interactable = false;
		}
	}

	private void InvokeCallbackOnInstance(string methodName, object instance)
	{
		var method = instance?.GetType().GetMethod(methodName);
		method?.Invoke(instance, null);
	}
}