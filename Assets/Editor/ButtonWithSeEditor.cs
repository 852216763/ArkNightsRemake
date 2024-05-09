using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(ButtonWithSe))]
public class ButtonWithSeEditor : ButtonEditor
{
    int SEIndex;
    string[] SE_List;

    protected override void OnEnable()
    {
        base.OnEnable();

        // 截取SE音效资源路径的最后一部分,作为下拉列表选项
        SE_List = new string[Constant.SoundAsset_UISFX_ButtonSE.Length];
        for (int i = 0; i < SE_List.Length; i++)
        {
            string[] splits = Constant.SoundAsset_UISFX_ButtonSE[i].Split('/');
            SE_List[i] = splits[splits.Length - 1];
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        var se = serializedObject.FindProperty("SEAssetPath");
        for (int i = 0; i < SE_List.Length; i++)
        {
            if (se.stringValue == Constant.SoundAsset_UISFX_ButtonSE[i])
            {
                SEIndex = i;
                break;
            }
        }
        SEIndex = EditorGUILayout.Popup("点击音效", SEIndex, SE_List);
        se.stringValue = Constant.SoundAsset_UISFX_ButtonSE[SEIndex];

        serializedObject.ApplyModifiedProperties();
    }
}
