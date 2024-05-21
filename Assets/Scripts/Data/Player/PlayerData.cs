using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerData
{
    static PlayerData _instance;

    int _ap;
    int _userLevel;
    string _userName;
    int _userID;
    List<CharData> _ownCharDataList;
    List<int> _formationList;
    List<ItemData> _ownItemDataList;

    public static PlayerData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerData();
                _instance.Init();
            }
            return _instance;
        }
    }

    // 禁用new
    private PlayerData() { }

    /// <summary>
    /// 当前理智
    /// </summary>
    public int Ap
    {
        get
        {
            return _ap;
        }
        set
        {
            _ap = value;
            PlayerPrefs.SetInt("AP", _ap);
        }
    }

    /// <summary>
    /// 理智上限
    /// </summary>
    public int MaxAP
    {
        get
        {
            return 75 + _userLevel / 2;
        }
    }

    /// <summary>
    /// 用户等级
    /// </summary>
    public int UserLevel
    {
        get
        {
            return _userLevel;
        }
        set
        {
            _userLevel = value;
            PlayerPrefs.SetInt("UserLevel", _userLevel);
        }
    }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string UserName
    {
        get
        {
            return _userName;
        }
        set
        {
            _userName = value;
            PlayerPrefs.SetString("UserName", _userName);
        }
    }

    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserID
    {
        get
        {
            return _userID;
        }
        set
        {
            _userID = value;
            PlayerPrefs.SetInt("UserID", _userID);
        }
    }

    /// <summary>
    /// 龙门币
    /// </summary>
    public int Money
    {
        get
        {
            return OwnItemDataList[Constant.ItemID.ItemID_Money].Count;
        }
        set
        {
            OwnItemDataList[Constant.ItemID.ItemID_Money].Count = value;
            OwnItemDataList = OwnItemDataList;
        }
    }

    /// <summary>
    /// 合成玉
    /// </summary>
    public int Jade
    {
        get
        {
            return OwnItemDataList[Constant.ItemID.ItemID_Jade].Count;
        }
        set
        {
            OwnItemDataList[Constant.ItemID.ItemID_Jade].Count = value;
            OwnItemDataList = OwnItemDataList;
        }
    }

    /// <summary>
    /// 源石
    /// </summary>
    public int Diamond
    {
        get
        {
            return OwnItemDataList[Constant.ItemID.ItemID_Diamond].Count;
        }
        set
        {
            OwnItemDataList[Constant.ItemID.ItemID_Diamond].Count = value;
            OwnItemDataList = OwnItemDataList;
        }
    }

    /// <summary>
    /// 持有干员列表
    /// </summary>
    public List<CharData> OwnCharDataList
    {
        get
        {
            if (_ownCharDataList == null)
            {
                string ownCharDataJson = PlayerPrefs.GetString("OwnCharData");
                List<CharData> list = JsonUtilityExtend.FromJson<CharData>(ownCharDataJson);
                _ownCharDataList = list;
            }
            return _ownCharDataList;
        }
        set
        {
            _ownCharDataList = value;
            string ownCharDataJson = JsonUtilityExtend.ToJson<CharData>(_ownCharDataList);
            PlayerPrefs.SetString("OwnCharData", ownCharDataJson);
        }
    }

    /// <summary>
    /// 编队 存储干员ID
    /// </summary>
    public List<int> FormationList
    {
        get
        {
            if (_formationList == null)
            {
                string formationJson = PlayerPrefs.GetString("FormationList");
                List<int> list = JsonUtilityExtend.FromJson<int>(formationJson);
                _formationList = list;
            }
            return _formationList;
        }
        set
        {
            _formationList = value;
            string formationJson = JsonUtilityExtend.ToJson<int>(_formationList);
            PlayerPrefs.SetString("FormationList", formationJson);
            FrameworkEntry.Event.Fire(this, ReferencePool.Acquire<FormationListChangedEventData>());
        }
    }

    /// <summary>
    /// 持有物品列表
    /// </summary>
    public List<ItemData> OwnItemDataList
    {
        get
        {
            if (_ownItemDataList == null)
            {
                string ownCharDataJson = PlayerPrefs.GetString("OwnItemData");
                List<ItemData> list = JsonUtilityExtend.FromJson<ItemData>(ownCharDataJson);
                _ownItemDataList = list;
            }
            return _ownItemDataList;
        }
        set
        {
            _ownItemDataList = value;
            string ownCharDataJson = JsonUtilityExtend.ToJson<ItemData>(_ownItemDataList);
            PlayerPrefs.SetString("OwnItemData", ownCharDataJson);
        }
    }


    /// <summary>
    /// 初始化数据
    /// </summary>
    public void Init()
    {
        _ap = PlayerPrefs.GetInt("AP", 49);
        _userLevel = PlayerPrefs.GetInt("", 120);
        _userName = PlayerPrefs.GetString("UserName", "Doc");
        _userID = PlayerPrefs.GetInt("UserID", 114514);

        // 设置默认持有的角色数据
        if (OwnCharDataList.Count <= 0)
        {
            SetDefaultCharData();
        }
        // 设置默认持有的物品数据
        if (OwnItemDataList.Count <= 0)
        {
            SetDefaultItemData();
        }
    }

    #region 测试用

    void SetDefaultCharData()
    {
        List<CharData> charDataList = new List<CharData>();
        CharData d = new CharData();
        d.CharID = 1;
        d.CurrentExp = 500;
        d.CurrentLevel = 20;
        d.Elite = CharEliteLevel.one;
        charDataList.Add(d);

        d = new CharData();
        d.CharID = 2;
        d.CurrentExp = 200;
        d.CurrentLevel = 15;
        d.Elite = CharEliteLevel.two;
        charDataList.Add(d);

        d = new CharData();
        d.CharID = 3;
        d.CurrentExp = 50;
        d.CurrentLevel = 40;
        d.Elite = CharEliteLevel.one;
        charDataList.Add(d);

        d = new CharData();
        d.CharID = 4;
        d.CurrentExp = 37;
        d.CurrentLevel = 35;
        d.Elite = CharEliteLevel.one;
        charDataList.Add(d);

        d = new CharData();
        d.CharID = 5;
        d.CurrentExp = 150;
        d.CurrentLevel = 20;
        d.Elite = CharEliteLevel.zero;
        charDataList.Add(d);

        d = new CharData();
        d.CharID = 6;
        d.CurrentExp = 150;
        d.CurrentLevel = 20;
        d.Elite = CharEliteLevel.zero;
        charDataList.Add(d);

        _ownCharDataList = charDataList;
    }

    void SetDefaultItemData()
    {
        List<ItemData> itemDataList = new List<ItemData>();
        ItemData item = new ItemData();
        item.ItemID = Constant.ItemID.ItemID_Diamond;
        item.Count = 21;
        itemDataList.Add(item);

        item = new ItemData();
        item.ItemID = Constant.ItemID.ItemID_Jade;
        item.Count = 6543;
        itemDataList.Add(item);

        item = new ItemData();
        item.ItemID = Constant.ItemID.ItemID_Money;
        item.Count = 1234567;
        itemDataList.Add(item);

        for (int i = 3; i < 21; i++)
        {
            item = new ItemData();
            item.ItemID = i;
            item.Count = Random.Range(1,20000);
            itemDataList.Add(item);
        }

        _ownItemDataList = itemDataList;
    }

    #endregion
}
