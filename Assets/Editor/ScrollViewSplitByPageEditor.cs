using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(ScrollViewSplitByPage))]
public class ScrollViewSplitByPageEditor : ScrollRectEditor
{

    SerializedProperty autoScroll;
    SerializedProperty flipInterval;
    SerializedProperty flipDuration;
    SerializedProperty sensitive;
    SerializedProperty toggleParent;

    protected override void OnEnable()
    {
        base.OnEnable();
        autoScroll = serializedObject.FindProperty("autoScroll");
        flipInterval = serializedObject.FindProperty("flipInterval");
        flipDuration = serializedObject.FindProperty("flipDuration");
        sensitive = serializedObject.FindProperty("sensitive");
        toggleParent = serializedObject.FindProperty("toggleParent");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(autoScroll);
        EditorGUILayout.PropertyField(flipInterval);
        EditorGUILayout.PropertyField(flipDuration);
        EditorGUILayout.PropertyField(sensitive);
        EditorGUILayout.PropertyField(toggleParent);
        serializedObject.ApplyModifiedProperties();
    }
}
