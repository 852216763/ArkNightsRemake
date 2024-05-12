using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 用户持有的角色数据
/// </summary>
[Serializable]
public class CharData
{
    [SerializeField]
    int charID;
    [SerializeField]
    CharEliteLevel elite;
    [SerializeField]
    int currentLevel = 1;
    [SerializeField]
    int currentExp;

    // 角色元数据不保存,即时获取
    CharMeta meta;

    // TODO 待添加角色运行时数据 技能等级之类的

    /// <summary>
    /// 角色ID
    /// </summary>
    public int CharID { get => charID; set => charID = value; }

    /// <summary>
    /// 角色精英化阶段
    /// </summary>
    public CharEliteLevel Elite { get => elite; set => elite = value; }

    /// <summary>
    /// 角色当前等级
    /// </summary>
    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }

    /// <summary>
    /// 角色当前经验值
    /// </summary>
    public int CurrentExp { get => currentExp; set => currentExp = value; }

    /// <summary>
    /// 角色元数据
    /// </summary>
    public CharMeta Meta
    {
        get
        {
            if (meta == null)
            {
                // TODO 添加角色数据库资源路径
                CharMetaDB db = Addressables.LoadAssetAsync<CharMetaDB>("Assets/GameData/char_meta_db.asset").WaitForCompletion();
                for (int i = 0; i < db.charIDs.Length; i++)
                {
                    if (db.charIDs[i] == charID)
                    {
                        meta = db.charMetas[i];
                        break;
                    }
                }
            }
            return meta;
        }
    }

    /// <summary>
    /// 当前等级升级最大所需经验
    /// </summary>
    public int MaxLevelUpExp
    {
        get
        {
            return CharMeta.GetLevelUpMaxExp(elite, currentLevel);
        }
    }

    /// <summary>
    /// 当前等级的角色属性
    /// </summary>
    public CharAttribute CurrentAttribute
    {
        get
        {
            int lvIndex = -1;
            for (int i = 0; i < 2; i++)
            {
                if (i < ((int)Elite))
                {
                    lvIndex += meta.GetMaxLevel()[i];
                }
            }
            lvIndex += currentLevel;
            return meta.Attributes[lvIndex];
        }
    }
}
