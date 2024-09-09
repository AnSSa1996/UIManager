using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(UIToggleSwitch))]
public class UIToggleSwitchEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        UIToggleSwitch targetToggleSwitch = (UIToggleSwitch)target;

        targetToggleSwitch.EnabledColor = EditorGUILayout.ColorField("EnabledColor:", targetToggleSwitch.EnabledColor);
        targetToggleSwitch.DisabledColor = EditorGUILayout.ColorField("DisabledColor:", targetToggleSwitch.DisabledColor);
        
        targetToggleSwitch.EnabledPosX = EditorGUILayout.FloatField("EnabledPosX", targetToggleSwitch.EnabledPosX);
        targetToggleSwitch.DisabledPosX = EditorGUILayout.FloatField("DisabledPosX", targetToggleSwitch.DisabledPosX);

        targetToggleSwitch.Handle = (RectTransform)EditorGUILayout.ObjectField("Handle", targetToggleSwitch.Handle, typeof(RectTransform), true);
        targetToggleSwitch.BackgroundImage = (Image)EditorGUILayout.ObjectField("Background", targetToggleSwitch.BackgroundImage, typeof(Image), true);
        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
        base.OnInspectorGUI();
    }
}
