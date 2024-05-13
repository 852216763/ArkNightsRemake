using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerData
{
    public static PlayerData _instance;

    int _ap;
    int _userLevel;
    string _userName;
    int _userID;
    int _money;
    int _jade;
    int _diamond;
    List<CharData> _ownCharDataList;
    // TODO 持有道具列表

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
            return _money;
        }
        set
        {
            _money = value;
            PlayerPrefs.SetInt("Money", _money);
        }
    }

    /// <summary>
    /// 合成玉
    /// </summary>
    public int Jade
    {
        get
        {
            return _jade;
        }
        set
        {
            _jade = value;
            PlayerPrefs.SetInt("Jade", _jade);
        }
    }

    /// <summary>
    /// 源石
    /// </summary>
    public int Diamond
    {
        get
        {
            return _diamond;
        }
        set
        {
            _diamond = value;
            PlayerPrefs.SetInt("Diamond", _diamond);
        }
    }

    /// <summary>
    /// 持有干员列表
    /// </summary>
    public List<CharData> OwnCharDataList
    {
        get
        {
            return _ownCharDataList;
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

        _money = PlayerPrefs.GetInt("Money", 11451419);
        _jade = PlayerPrefs.GetInt("Jade", 198);
        _diamond = PlayerPrefs.GetInt("Diamond", 10);

        string ownCharDataListJson = PlayerPrefs.GetString("OwnCharData", "");
        _ownCharDataList = JsonUtilityExtend.FromJson<CharData>(ownCharDataListJson);
        if (_ownCharDataList.Count <= 0)
        {
            SetDefaultCharData();
        }
    }

    public void SaveCharDataChange()
    {
        PlayerPrefs.SetString("OwnCharData", JsonUtilityExtend.ToJson<CharData>(_ownCharDataList));
    }

    #region 测试用

    void SetDefaultCharData()
    {
        _ownCharDataList = new List<CharData>();
        CharData d = new CharData();
        d.CharID = 1;
        d.CurrentExp = 500;
        d.CurrentLevel = 20;
        d.Elite = CharEliteLevel.one;
        _ownCharDataList.Add(d);

        d = new CharData();
        d.CharID = 2;
        d.CurrentExp = 200;
        d.CurrentLevel = 15;
        d.Elite = CharEliteLevel.two;
        _ownCharDataList.Add(d);

        d = new CharData();
        d.CharID = 3;
        d.CurrentExp = 50;
        d.CurrentLevel = 40;
        d.Elite = CharEliteLevel.one;
        _ownCharDataList.Add(d);

        d = new CharData();
        d.CharID = 4;
        d.CurrentExp = 37;
        d.CurrentLevel = 35;
        d.Elite = CharEliteLevel.one;
        _ownCharDataList.Add(d);

        d = new CharData();
        d.CharID = 5;
        d.CurrentExp = 150;
        d.CurrentLevel = 20;
        d.Elite = CharEliteLevel.zero;
        _ownCharDataList.Add(d);

        d = new CharData();
        d.CharID = 6;
        d.CurrentExp = 150;
        d.CurrentLevel = 20;
        d.Elite = CharEliteLevel.zero;
        _ownCharDataList.Add(d);

        SaveCharDataChange();
    }

    #endregion
}
