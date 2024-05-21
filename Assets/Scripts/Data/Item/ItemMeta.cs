using System;
using UnityEngine;

[Serializable]
public class ItemMeta
{
    [Header("物品ID"), SerializeField]
    private int itemID;

    [Header("名称"), SerializeField]
    private string name;

    [Header("稀有度"), Range(1, 6), SerializeField]
    private int rarity;

    [Header("图标"), SerializeField]
    private Sprite icon;

    [Header("物品类型"), SerializeField]
    private ItemEnum itemType;

    [Header("物品用途"), SerializeField]
    private string purpose;

    [Header("物品描述"), SerializeField]
    private string description;

    [Header("获取方式"), SerializeField]
    private string obtainWay;

    [Header("排序顺序"), SerializeField]
    private int sortOrder;


    /// <summary>
    /// 物品ID
    /// </summary>
    public int ItemID { get => itemID; set => itemID = value; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get => name; set => name = value; }

    /// <summary>
    /// 稀有度
    /// </summary>
    public int Rarity { get => rarity; set => rarity = value; }

    /// <summary>
    /// 图标
    /// </summary>
    public Sprite Icon { get => icon; set => icon = value; }

    /// <summary>
    /// 物品类型
    /// </summary>
    public ItemEnum ItemType { get => itemType; set => itemType = value; }

    /// <summary>
    /// 物品用途
    /// </summary>
    public string Purpose { get => purpose; set => purpose = value; }


    /// <summary>
    /// 物品描述
    /// </summary>
    public string Description { get => description; set => description = value; }

    /// <summary>
    /// 获取方式
    /// </summary>
    public string ObtainWay { get => obtainWay; set => obtainWay = value; }

    /// <summary>
    /// 排序顺序
    /// </summary>
    public int SortOrder { get => sortOrder; set => sortOrder = value; }
}
