using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HealthBar), true)]
[CanEditMultipleObjects]
public class HealthBarEditor : UnityEditor.UI.SliderEditor
{
    private SerializedProperty direction;
    private SerializedProperty fillRect;
    private SerializedProperty minValue;
    private SerializedProperty maxValue;
    private SerializedProperty wholeNumbers;
    private SerializedProperty value;
    private SerializedProperty onValueChanged;
    private SerializedProperty barFillSpeed;

    private string[] excludedProperties = new[] { "" };

    protected override void OnEnable()
    {
        base.OnEnable();
		
        minValue = serializedObject.FindProperty("m_MinValue");
        maxValue = serializedObject.FindProperty("m_MaxValue");
        wholeNumbers = serializedObject.FindProperty("m_WholeNumbers");
        value = serializedObject.FindProperty("m_Value");
        onValueChanged = serializedObject.FindProperty("m_OnValueChanged");
        fillRect = serializedObject.FindProperty("m_FillRect");
        barFillSpeed = serializedObject.FindProperty("barFillSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(fillRect);
        EditorGUILayout.PropertyField(barFillSpeed);
        
        if (fillRect.objectReferenceValue != null)
        {
            EditorGUI.BeginChangeCheck();
            float newMin = EditorGUILayout.FloatField("Min Value", minValue.floatValue);
            if (EditorGUI.EndChangeCheck() && wholeNumbers.boolValue ? Mathf.Round(newMin) < maxValue.floatValue : newMin < maxValue.floatValue)
            {
                minValue.floatValue = newMin;
            }

            EditorGUI.BeginChangeCheck();
            float newMax = EditorGUILayout.FloatField("Max Value", maxValue.floatValue);
            if (EditorGUI.EndChangeCheck() && wholeNumbers.boolValue ? Mathf.Round(newMax) > minValue.floatValue : newMax > minValue.floatValue)
            {
                maxValue.floatValue = newMax;
            }
            
            EditorGUILayout.PropertyField(wholeNumbers);

            bool areMinMaxEqual = (minValue.floatValue == maxValue.floatValue);

            if (areMinMaxEqual)
                EditorGUILayout.HelpBox("Min Value and Max Value cannot be equal.", MessageType.Warning);

            EditorGUI.BeginDisabledGroup(areMinMaxEqual);
            EditorGUILayout.Slider(value, minValue.floatValue, maxValue.floatValue);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(onValueChanged);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}