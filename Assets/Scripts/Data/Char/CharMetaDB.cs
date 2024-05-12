using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "char_meta_db", menuName = "ArkNight/CharMeta")]
public class CharMetaDB : ScriptableObject
{
    [SerializeField]
    public int[] charIDs;
    [SerializeField]
    public CharMeta[] charMetas;

#if UNITY_EDITOR
    // 初始化完之后注释掉Awake,不然重启项目或内存清理后会再次调用!
#if false
    void Awake()
    {
        Debug.Log("请注释掉Awake!!!");
        // TODO 自动化配置角色数据
        string[] charDirs = Directory.GetDirectories("Assets/Arts/Char");
        List<string> subfolders = new List<string>();
        foreach (string directory in charDirs)
        {
            string subfolderPath = directory.Replace(Path.DirectorySeparatorChar, '/');
            subfolders.Add(subfolderPath + '/');
        }

        List<int> charIDList = new List<int>();
        List<CharMeta> charMetaList = new List<CharMeta>();
        foreach (string charFolderPath in subfolders)
        {
            string[] split = charFolderPath.Split('/');
            string charFolderName = split[^2];
            split = charFolderName.Split('_');
            int id = int.Parse(split[0]);
            string englishName = split.Length >= 2 ? split[1] : "";

            CharMeta meta = new CharMeta();
            meta.CharID = id;
            meta.EnglishName = englishName;

            meta.CharSprites = GetAssets<Sprite>(charFolderPath + "sprites");
            meta.CharSkeleton = GetAssets<SkeletonDataAsset>(charFolderPath + "spine");
            meta.Portraits = GetAssets<Sprite>(charFolderPath + "portraits");
            meta.Avatars = GetAssets<Sprite>(charFolderPath + "avatars");
            meta.BattleSkeleton = GetAssets<SkeletonDataAsset>(charFolderPath + "battleSpine");

            charIDList.Add(id);
            charMetaList.Add(meta);
        }

        charIDs = charIDList.ToArray();
        charMetas = charMetaList.ToArray();
    }
#endif

    //private void OnEnable()
    //{
    //    CreateDefaultAttribute();
    //    EditorUtility.SetDirty(this);
    //}

    // 获取指定路径下的所有该类型资源
    T[] GetAssets<T>(string path) where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new string[] { path });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T sprite = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (sprite != null)
            {
                assets.Add(sprite);
            }
        }
        return assets.ToArray();
    }

    void CreateDefaultAttribute()
    {
        Debug.Log("自动创建默认数据");
        foreach (CharMeta item in charMetas)
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
#endif
}
