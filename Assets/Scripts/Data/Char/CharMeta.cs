using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharMeta
{
    [Header("角色ID"), SerializeField]
    private int charID;

    [Header("中文名"), SerializeField]
    private string chineseName;

    [Header("英文名"), SerializeField]
    private string englishName;

    [Header("稀有度"), Range(1, 6), SerializeField]
    private int rarity;

    [Header("职业"), SerializeField]
    private CharProfession profession;

    [Header("可部署地形"), SerializeField]
    private CharPosition charPosition;

    [Header("阵营"), SerializeField]
    private CharCamp camp;

    [Header("标签"), SerializeField]
    private CharTag[] tags;

    [Header("干员属性"), SerializeField]
    private CharAttribute[] attributes;

    [Header("再部署"), SerializeField]
    private float deployCD;

    [Header("费用"), SerializeField]
    private int cost;

    [Header("阻挡数"), SerializeField]
    private int block;

    [Header("攻击间隔(s)"), SerializeField]
    private float attackInterval;

    [Header("攻击范围"), SerializeField]
    private Vector2[] atkRange;

    [Header("天赋名"), SerializeField]
    private string[] talent;

    [Header("特性"), SerializeField]
    private string feature;

    [Header("所有立绘"), SerializeField]
    private Sprite[] charSprites;

    [Header("Spine立绘"), SerializeField]
    private SkeletonDataAsset[] charSkeleton;

    [Header("卡绘"), SerializeField]
    private Sprite[] portraits;

    [Header("头像"), SerializeField]
    private Sprite[] avatars;

    [Header("战斗Spine"), SerializeField]
    private SkeletonDataAsset[] battleSkeleton;

    /// <summary>
    /// 角色ID
    /// </summary>
    public int CharID { get => charID; set => charID = value; }

    /// <summary>
    /// 中文名
    /// </summary>
    public string ChineseName { get => chineseName; set => chineseName = value; }

    /// <summary>
    /// 英文名
    /// </summary>
    public string EnglishName { get => englishName; set => englishName = value; }

    /// <summary>
    /// 稀有度
    /// </summary>
    public int Rarity { get => rarity; set => rarity = value; }

    /// <summary>
    /// 职业
    /// </summary>
    public CharProfession Profession { get => profession; set => profession = value; }

    /// <summary>
    /// 可部署地形
    /// </summary>
    public CharPosition CharPosition { get => charPosition; set => charPosition = value; }

    /// <summary>
    /// 阵营
    /// </summary>
    public CharCamp Camp { get => camp; set => camp = value; }

    /// <summary>
    /// 标签
    /// </summary>
    public CharTag[] Tags { get => tags; set => tags = value; }

    /// <summary>
    /// 干员属性
    /// </summary>
    public CharAttribute[] Attributes { get => attributes; set => attributes = value; }

    /// <summary>
    /// 再部署时间
    /// </summary>
    public float DeployCD { get => deployCD; set => deployCD = value; }

    /// <summary>
    /// 费用
    /// </summary>
    public int Cost { get => cost; set => cost = value; }

    /// <summary>
    /// 阻挡数
    /// </summary>
    public int Block { get => block; set => block = value; }

    /// <summary>
    /// 攻击间隔
    /// </summary>
    public float AttackInterval { get => attackInterval; set => attackInterval = value; }

    /// <summary>
    /// 攻击范围
    /// 这里假设角色朝右, (0,0)就代表攻击范围包含当前格,(-1,0)就代表包含当前格左1格
    /// </summary>
    public Vector2[] AtkRange { get => atkRange; set => atkRange = value; }

    /// <summary>
    /// 天赋
    /// </summary>
    public string[] Talent { get => talent; set => talent = value; }

    /// <summary>
    /// 特性
    /// </summary>
    public string Feature { get => feature; set => feature = value; }

    /// <summary>
    /// 所有立绘
    /// </summary>
    public Sprite[] CharSprites { get => charSprites; set => charSprites = value; }

    /// <summary>
    /// 所有Spine立绘
    /// </summary>
    public SkeletonDataAsset[] CharSkeleton { get => charSkeleton; set => charSkeleton = value; }

    /// <summary>
    /// 卡绘
    /// </summary>
    public Sprite[] Portraits { get => portraits; set => portraits = value; }

    /// <summary>
    /// 头像
    /// </summary>
    public Sprite[] Avatars { get => avatars; set => avatars = value; }

    /// <summary>
    /// 战斗Spine
    /// </summary>
    public SkeletonDataAsset[] BattleSkeleton { get => battleSkeleton; set => battleSkeleton = value; }


    /// <summary>
    /// 获取最大精英化等级
    /// </summary>
    public CharEliteLevel GetMaxElite()
    {
        switch (rarity)
        {
            case 1:
            case 2:
                return CharEliteLevel.zero;
            case 3:
                return CharEliteLevel.one;
            case 4:
            case 5:
            case 6:
                return CharEliteLevel.two;
            default:
                return CharEliteLevel.zero;
        }
    }

    /// <summary>
    /// 获取各精英化阶段最高等级
    /// </summary>
    public int[] GetMaxLevel()
    {
        List<int> maxLevel = new List<int>();
        switch (rarity)
        {
            case 1:
            case 2:
                maxLevel.Add(30);
                break;
            case 3:
                maxLevel.Add(40);
                maxLevel.Add(55);
                break;
            case 4:
                maxLevel.Add(45);
                maxLevel.Add(60);
                maxLevel.Add(70);
                break;
            case 5:
                maxLevel.Add(50);
                maxLevel.Add(70);
                maxLevel.Add(80);
                break;
            case 6:
                maxLevel.Add(60);
                maxLevel.Add(80);
                maxLevel.Add(90);
                break;
        }
        return maxLevel.ToArray();
    }

    /// <summary>
    /// 获取当前等级升级所需最大经验
    /// TODO 升级经验应该另行配置
    /// 养成材料等应该在一起
    /// </summary>
    /// <param name="elite">精英化阶段</param>
    /// <param name="level">当前等级</param>
    /// <returns></returns>
    public static int GetLevelUpMaxExp(CharEliteLevel elite, int level)
    {
        switch (elite)
        {
            case CharEliteLevel.zero:
                return 100 + level * 20;
            case CharEliteLevel.one:
                return 120 + level * 60;
            case CharEliteLevel.two:
                return 120 + level * 120;
            default:
                return 0;
        }
    }

}
