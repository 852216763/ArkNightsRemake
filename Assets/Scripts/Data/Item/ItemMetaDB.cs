using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ItemMetaDB : ScriptableObject
{
    [SerializeField]
    public ItemMeta[] itemMetas;
}
