using System;
using UnityEngine;

// 角色属性数据
[Serializable]
public class CharAttribute
{
    [SerializeField]
    float maxHealth;
    [SerializeField]
    float attack;
    [SerializeField]
    float defence;
    [SerializeField]
    float magicResistance;



    /// <summary>
    /// 生命上限
    /// </summary>
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    /// <summary>
    /// 攻击
    /// </summary>
    public float Attack { get => attack; set => attack = value; }

    /// <summary>
    /// 防御力
    /// </summary>
    public float Defence { get => defence; set => defence = value; }

    /// <summary>
    /// 法术抗性
    /// </summary>
    public float MagicResistance { get => magicResistance; set => magicResistance = value; }


    public static CharAttribute operator +(CharAttribute a, CharAttribute b)
    {
        return new CharAttribute
        {
            MaxHealth = a.MaxHealth + b.MaxHealth,
            Attack = a.Attack + b.Attack,
            Defence = a.Defence + b.Defence,
            MagicResistance = a.MagicResistance + b.MagicResistance,
        };
    }

    public static CharAttribute operator -(CharAttribute a, CharAttribute b)
    {
        return new CharAttribute
        {
            MaxHealth = a.MaxHealth - b.MaxHealth,
            Attack = a.Attack - b.Attack,
            Defence = a.Defence - b.Defence,
            MagicResistance = a.MagicResistance - b.MagicResistance,
        };
    }

    public static CharAttribute operator *(CharAttribute a, int amount)
    {
        return new CharAttribute
        {
            MaxHealth = a.MaxHealth * amount,
            Attack = a.Attack * amount,
            Defence = a.Defence * amount,
            MagicResistance = a.MagicResistance * amount,
        };
    }

    public static CharAttribute operator /(CharAttribute a, int amount)
    {
        if (amount == 0)
        {
            throw new Exception("干员数据不能除以0!");
        }
        return new CharAttribute
        {
            MaxHealth = a.MaxHealth / amount,
            Attack = a.Attack / amount,
            Defence = a.Defence / amount,
            MagicResistance = a.MagicResistance / amount,
        };
    }
}
