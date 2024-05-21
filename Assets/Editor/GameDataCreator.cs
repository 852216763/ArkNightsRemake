using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameDataCreator
{
    [MenuItem("Assets/Create/ArkNight/ItemMeta")]
    private static void CreateItemMetaDB()
    {
        ItemMetaDB instance = ScriptableObject.CreateInstance<ItemMetaDB>();

        string itemDir = "Assets/Arts/UI/Item/";

        Sprite[] sprites = GetAssets<Sprite>(itemDir);
        List<ItemMeta> itemMetaList = new List<ItemMeta>();
        int index = 0;
        foreach (Sprite sprite in sprites)
        {
            ItemMeta meta = new ItemMeta();
            meta.ItemID = index;
            meta.Icon = sprite;

            itemMetaList.Add(meta);
            index++;
        }

        instance.itemMetas = itemMetaList.ToArray();

        AssetDatabase.CreateAsset(instance, "Assets/GameData/item_meta_db.asset");
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Assets/Create/ArkNight/CharMeta")]
    private static void CreateCharMetaDB()
    {
        CharMetaDB instance = ScriptableObject.CreateInstance<CharMetaDB>();

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

        instance.charMetas = charMetaList.ToArray();

        AssetDatabase.CreateAsset(instance, "Assets/GameData/char_meta_db.asset");
        AssetDatabase.SaveAssets();
    }

    // 获取指定路径下的所有该类型资源
    static T[] GetAssets<T>(string path) where T : UnityEngine.Object
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
}
