using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 用户持有的物品数据
/// </summary>
[Serializable]
public class ItemData
{
    [SerializeField]
    int itemID;
    [SerializeField]
    int count;

    // 物品元数据不保存,即时获取
    ItemMeta meta;

    /// <summary>
    /// 物品ID
    /// </summary>
    public int ItemID { get => itemID; set => itemID = value; }

    /// <summary>
    /// 物品数量
    /// </summary>
    public int Count { get => count; set => count = value; }

    /// <summary>
    /// 格式化的物品数量
    /// </summary>
    public string FormattedCount
    {
        get
        {
            string format;
            if(count > 10000)
            {
                format = $"{count / 10000f:.0}万";
            }
            else
            {
                format = count.ToString();
            }

            return format;
        }
    }

    /// <summary>
    /// 物品元数据
    /// </summary>
    public ItemMeta Meta
    {
        get
        {
            if (meta == null)
            {
                ItemMetaDB db = FrameworkEntry.Resource.LoadAsset<ItemMetaDB>(Constant.DataAsset_Item_Meta_DB);
                ItemMeta m = db.itemMetas[ItemID];
                if (m.ItemID == itemID)
                {
                    meta = m;
                }
                else
                {
                    foreach (ItemMeta item in db.itemMetas)
                    {
                        if (item.ItemID == itemID)
                        {
                            meta = item;
                            break;
                        }
                    }
                }
                FrameworkEntry.Resource.ReleaseAsset<ItemMetaDB>(db);
            }

            return meta;
        }
    }

}
