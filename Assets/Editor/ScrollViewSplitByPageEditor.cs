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
    SerializedProperty toggleParent;

    protected override void OnEnable()
    {
        base.OnEnable();
        autoScroll = serializedObject.FindProperty("autoScroll");
        flipInterval = serializedObject.FindProperty("flipInterval");
        flipDuration = serializedObject.FindProperty("flipDuration");
        toggleParent = serializedObject.FindProperty("toggleParent");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(autoScroll);
        EditorGUILayout.PropertyField(flipInterval);
        EditorGUILayout.PropertyField(flipDuration);
        EditorGUILayout.PropertyField(toggleParent);
        serializedObject.ApplyModifiedProperties();
    }
}
