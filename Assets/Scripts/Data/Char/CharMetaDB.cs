using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class CharMetaDB : ScriptableObject
{
    [SerializeField]
    public CharMeta[] charMetas;

    //private void OnEnable()
    //{
    //    CreateDefaultAttribute(this);
    //}

    static void CreateDefaultAttribute(CharMetaDB instance)
    {
        Debug.Log("自动创建默认干员数据");
        foreach (CharMeta item in instance.charMetas)
        {
            if (item.Rarity == 0)
            {
                continue;
            }

            List<CharAttribute> attr = new List<CharAttribute>();
            int levelCount = 0;
            for (int i = 0; i <= (int)item.GetMaxElite(); i++)
            {
                levelCount += item.GetMaxLevel()[i];
            }
            for (int i = 0; i < levelCount; i++)
            {
                CharAttribute ca = new CharAttribute();
                ca.Attack = 250 + 5 * i;
                ca.Defence = 200 + 3 * i;
                ca.MagicResistance = 5;
                ca.MaxHealth = 1000 + 7 * i;
                attr.Add(ca);
            }

            item.Attributes = attr.ToArray();
        }
    }
}
