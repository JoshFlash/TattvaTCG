using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(UIButton), true)]
[CanEditMultipleObjects]
public class UIButtonEditor : UnityEditor.UI.SelectableEditor
{
	private SerializedProperty disableButtonOnClickProperty;
	private SerializedProperty callbackMethodNameProperty;
	private SerializedProperty buttonCallbackTarget;
	private SerializedProperty buttonCallbackTargetObject;

	protected override void OnEnable()
	{
		base.OnEnable();
		
		disableButtonOnClickProperty = serializedObject.FindProperty("DisableButtonOnClick");
		callbackMethodNameProperty = serializedObject.FindProperty("CallbackMethodName");
		buttonCallbackTarget = serializedObject.FindProperty("CallbackTarget");
		buttonCallbackTargetObject = serializedObject.FindProperty("CallbackTargetObject");
	}
	
	public override void OnInspectorGUI()
	{
		var component = (UIButton) target;
		
		serializedObject.Update();
		
		EditorGUILayout.PropertyField(disableButtonOnClickProperty);

		EditorGUILayout.PropertyField(callbackMethodNameProperty);
		EditorGUILayout.PropertyField(buttonCallbackTarget);

		if (component.CallbackTarget == ButtonCallbackTarget.Component)
		{
			EditorGUILayout.PropertyField(buttonCallbackTargetObject);
		}

		serializedObject.ApplyModifiedProperties();
		
		EditorGUILayout.Space();	
		
		base.OnInspectorGUI();
	}
}