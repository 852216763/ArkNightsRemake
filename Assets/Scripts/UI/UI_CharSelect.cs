using DG.Tweening;
using Framework;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UI_CharSelect : UIForm
{
    [SerializeField]
    Color selectSortTMPColor;

    CharCardScrollView _charCardSV;
    Transform _detailPanel;
    Transform _topPanel;
    Transform _toggleGroup;
    Transform _tabs;


    #region TopPanel
    TextMeshProUGUI _enNameTMP;
    TextMeshProUGUI _cnNameTMP;
    TextMeshProUGUI _levelTMP;
    Button _cultivateBtn;
    #endregion

    #region 属性标签页
    TextMeshProUGUI _attrHPTMP;
    TextMeshProUGUI _attrAtkTMP;
    TextMeshProUGUI _attrDefTMP;
    TextMeshProUGUI _attrMagicResistanceTMP;
    TextMeshProUGUI _attrDeployCDTMP;
    TextMeshProUGUI _attrCostTMP;
    TextMeshProUGUI _attrBlockTMP;
    TextMeshProUGUI _attrAtkSpeedTMP;
    #endregion

    TextMeshProUGUI _currentSortSelectTMP;
    Button _sortByLevelBtn;
    Button _sortByRarityBtn;

    #region 编队
    bool _singleSelectMode;
    int _operateFormationIndex;
    int _maxOperator;
    CharData _currentSelectData;
    List<int> _currentFormationList;
    List<CharCard> _currentSelectCardList;
    #endregion

    protected override void OnInit(object userdata = null)
    {
        base.OnInit(userdata);

        transform.Find("BackBtn").GetComponent<Button>().onClick.AddListener(OnBackBtnClick);
        transform.Find("ConfirmBtn").GetComponent<Button>().onClick.AddListener(OnConfirmBtnClick);

        _charCardSV = transform.Find("CharScrollView").GetComponent<CharCardScrollView>();
        _detailPanel = transform.Find("DetailPanel");

        _topPanel = _detailPanel.Find("TopPanel");
        _enNameTMP = _topPanel.Find("ENNameTMP").GetComponent<TextMeshProUGUI>();
        _cnNameTMP = _topPanel.Find("CNNameTMP").GetComponent<TextMeshProUGUI>();
        _levelTMP = _topPanel.Find("LevelTMP").GetComponent<TextMeshProUGUI>();
        _cultivateBtn = _topPanel.Find("CultivateBtn").GetComponent<Button>();
        _cultivateBtn.onClick.AddListener(() => { GoToSelectedCharInfo(); });

        _toggleGroup = _detailPanel.Find("ToggleGroup");
        _tabs = _detailPanel.Find("Tabs");
        for (int i = 0; i < _toggleGroup.childCount && i < _tabs.childCount; i++)
        {
            Toggle t = _toggleGroup.GetChild(i).GetComponent<Toggle>();
            int index = i;
            t.onValueChanged.AddListener(isOn =>
            {
                t.transform.Find("Label").GetComponent<TextMeshProUGUI>().color = isOn ? Color.black : Color.white;
                _tabs.GetChild(index).gameObject.SetActive(isOn);
            });
        }
        Transform _attrTab = _tabs.Find("AttrTab");
        _attrHPTMP = _attrTab.Find("HPTMP").GetComponent<TextMeshProUGUI>();
         _attrAtkTMP = _attrTab.Find("AtkTMP").GetComponent<TextMeshProUGUI>();
        _attrDefTMP = _attrTab.Find("DefTMP").GetComponent<TextMeshProUGUI>();
        _attrMagicResistanceTMP = _attrTab.Find("MagicResistanceTMP").GetComponent<TextMeshProUGUI>();
        _attrDeployCDTMP = _attrTab.Find("DeployCDTMP").GetComponent<TextMeshProUGUI>();
        _attrCostTMP = _attrTab.Find("CostTMP").GetComponent<TextMeshProUGUI>();
        _attrBlockTMP = _attrTab.Find("BlockTMP").GetComponent<TextMeshProUGUI>();
        _attrAtkSpeedTMP = _attrTab.Find("AttackSpeedTMP").GetComponent<TextMeshProUGUI>();

        _sortByLevelBtn = transform.Find("SortByLevelBtn").GetComponent<Button>();
        _sortByLevelBtn.onClick.AddListener(() => SortCardByLevel());
        _sortByRarityBtn = transform.Find("SortByRarityBtn").GetComponent<Button>();
        _sortByRarityBtn.onClick.AddListener(() => SortCardByRarity());
        _currentSortSelectTMP = _sortByLevelBtn.GetComponentInChildren<TextMeshProUGUI>();

    }

    protected override void OnShow(object userdata = null)
    {
        base.OnShow(userdata);

        _maxOperator = FrameworkEntry.DataNode.GetData<int>("UI/Formation/MaxOperator");
        _currentFormationList = new List<int>();
        _currentSelectCardList = new List<CharCard>();
        if (userdata != null)
        {
            _singleSelectMode = true;
            _operateFormationIndex = (int)userdata;
            if (_operateFormationIndex < PlayerData.Instance.FormationList.Count)
            {
                _currentFormationList.Add(PlayerData.Instance.FormationList[_operateFormationIndex]);
            }
        }
        else
        {
            _singleSelectMode = false;
            if (PlayerData.Instance.FormationList != null)
            {
                _currentFormationList.AddRange(PlayerData.Instance.FormationList);
            }
        }
        // 获取编队列表第一个干员的数据
        if (_currentFormationList.Count > 0)
        {
            foreach (CharData data in PlayerData.Instance.OwnCharDataList)
            {
                if (data.CharID == _currentFormationList[0])
                {
                    _currentSelectData = data;
                    break;
                }
            }
        }



        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.Fade(0);
        rect.Fade(1, Constant.FadeTime);

        SortCardByLevel();
        if (_currentSelectData != null)
        {
            if (_singleSelectMode)
            {
                _currentSelectCardList.Add(_charCardSV.SelectSingleCharCard(_currentSelectData.CharID));
            }
            else
            {
                _currentSelectCardList = _charCardSV.SelectMultiCharCard(_currentFormationList);
            }
        }
        RefreshDetailPanel();
    }

    protected override void OnHide(object userdata = null)
    {
        base.OnHide(userdata);

        // 关闭界面时清理选中状态
        foreach (CharCard item in _currentSelectCardList)
        {
            item.CancelSelect();
        }

        _singleSelectMode = false;
        _operateFormationIndex = 0;
        _maxOperator = 0;
        _currentSelectData = null;
        _currentFormationList = null;
        _currentSelectCardList = null;


    }

    /// <summary>
    /// 按稀有度排序角色列表
    /// </summary>
    public void SortCardByRarity()
    {
        _currentSortSelectTMP.color = Color.white;
        _currentSortSelectTMP = _sortByRarityBtn.GetComponentInChildren<TextMeshProUGUI>();
        _currentSortSelectTMP.color = selectSortTMPColor;

        List<CharData> displayableData = PlayerData.Instance.OwnCharDataList;
        if (_singleSelectMode)
        {
            displayableData = displayableData.
                Where(x => x.CharID == _currentSelectData?.CharID 
                || !PlayerData.Instance.FormationList.Contains(x.CharID)).ToList();
            foreach (CharData item in displayableData)
            {
                Debug.Log($"可显示角色:{item.Meta.ChineseName}");
            }
        }
        // 按照选中状态,稀有度,精英阶段,等级降序排序
        List<CharData> data = displayableData
            .OrderByDescending(x => _currentFormationList.Contains(x.CharID))
            .ThenByDescending(x => x.Meta.Rarity)
            .ThenByDescending(x => x.Elite)
            .ThenByDescending(x => x.CurrentLevel).ToList();

        _charCardSV.RefreshScrollContent(data);
        RegisteCardBtnClickEvent();
    }

    /// <summary>
    /// 按等级排序角色列表
    /// </summary>
    public void SortCardByLevel()
    {
        _currentSortSelectTMP.color = Color.white;
        _currentSortSelectTMP = _sortByLevelBtn.GetComponentInChildren<TextMeshProUGUI>();
        _currentSortSelectTMP.color = selectSortTMPColor;

        List<CharData> displayableData = PlayerData.Instance.OwnCharDataList;
        if (_singleSelectMode)
        {
            displayableData = displayableData.
                Where(x => x.CharID == _currentSelectData?.CharID 
                || !PlayerData.Instance.FormationList.Contains(x.CharID)).ToList();
        }
        // 按照选中状态精英阶段,等级,稀有度降序排序
        List<CharData> data = displayableData
            .OrderByDescending(x => _currentFormationList.Contains(x.CharID))
            .ThenByDescending(x => x.Elite)
            .ThenByDescending(x => x.CurrentLevel)
            .ThenByDescending(x => x.Meta.Rarity).ToList();

        _charCardSV.RefreshScrollContent(data);
        RegisteCardBtnClickEvent();
    }

    /// <summary>
    /// 退出
    /// </summary>
    public void OnBackBtnClick()
    {
        FrameworkEntry.UI.HideUI(gameObject);
    }

    private void OnConfirmBtnClick()
    {
        List<int> formation = PlayerData.Instance.FormationList;
        if (_singleSelectMode)
        {
            if (_currentSelectData == null)
            {
                if (_operateFormationIndex < formation.Count)
                {
                    formation.RemoveAt(_operateFormationIndex);
                }
            }
            else
            {
                if (_operateFormationIndex < formation.Count)
                {
                    formation[_operateFormationIndex] = _currentSelectData.CharID;
                }
                else
                {
                    formation.Add(_currentSelectData.CharID);
                }
            }
            PlayerData.Instance.FormationList = formation;
        }
        else
        {
            PlayerData.Instance.FormationList = _currentFormationList;
        }
        FrameworkEntry.UI.HideUI(gameObject);
    }

    /// <summary>
    /// 更新角色详情界面的数据
    /// </summary>
    private void RefreshDetailPanel()
    {
        bool hasData = _currentSelectData != null;
        _topPanel.gameObject.SetActive(hasData);
        _toggleGroup.GetComponent<ToggleGroup>().allowSwitchOff = !hasData;
        Toggle t = _toggleGroup.GetChild(0).GetComponent<Toggle>();
        t.isOn = hasData;
        t.interactable = hasData;

        if (hasData)
        {
            CharMeta meta = _currentSelectData.Meta;
            CharAttribute attr = _currentSelectData.CurrentAttribute;

            _enNameTMP.text = meta.EnglishName;
            _cnNameTMP.text = meta.ChineseName;
            _levelTMP.text = $"<color=#02A3ED>{_currentSelectData.CurrentLevel}</color>/{meta.GetMaxLevel()[(int)_currentSelectData.Elite]}";

            _attrHPTMP.text = attr.MaxHealth.ToString();
            _attrAtkTMP.text = attr.Attack.ToString();
            _attrDefTMP.text = attr.Defence.ToString();
            _attrMagicResistanceTMP.text = attr.MagicResistance.ToString();
            _attrDeployCDTMP.text = meta.DeployCD.ToString();
            _attrCostTMP.text = meta.Cost.ToString();
            _attrBlockTMP.text = meta.Block.ToString();
            _attrAtkSpeedTMP.text = meta.AttackInterval.ToString();

        }
    }

    /// <summary>
    /// 跳转至当前选中的角色Info界面
    /// </summary>
    private void GoToSelectedCharInfo()
    {
        if (_currentSelectData != null)
        {
            UI_CharInfo charInfo = FrameworkEntry.UI.ShowUI(Constant.UIAsset_CharInfo).GetComponent<UI_CharInfo>();
            charInfo?.UpdateData(new List<CharData> { _currentSelectData }, _currentSelectData);
        }
    }

    /// <summary>
    /// 注册/更新所有Card的点击事件
    /// </summary>
    private void RegisteCardBtnClickEvent()
    {
        List<CharCard> cardList = _charCardSV.charCardItemList;
        foreach (CharCard item in cardList)
        {
            CharCard card = item;
            Button btn = card.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                if (card.IsSelected)
                {
                    // 取消选择时更新选择列表的序号
                    if (!_singleSelectMode)
                    {
                        foreach (CharCard item in _currentSelectCardList)
                        {
                            if (item.SelectedIndex > card.SelectedIndex)
                            {
                                item.SelectMulti(item.SelectedIndex - 1);
                            }
                        }
                    }
                    
                    card.CancelSelect();
                    _currentFormationList.Remove(card.Data.CharID);
                    _currentSelectCardList.Remove(card);
                    _currentSelectData = null;
                }
                else
                {
                    // 单选模式下,选择当前项时要取消选择已选择项
                    if (_singleSelectMode)
                    {
                        if (_currentSelectCardList.Count > 0)
                        {
                            _currentSelectCardList[0].CancelSelect();
                        }
                        _currentFormationList.Clear();
                        _currentSelectCardList.Clear();
                        _currentSelectData = card.Select();
                    }
                    else
                    {
                        // 达到编队上限后不再继续选中
                        if (_currentSelectCardList.Count >= _maxOperator)
                        {
                            return;
                        }
                        _currentSelectData = card.SelectMulti(_currentSelectCardList.Count + 1);
                    }
                    _currentFormationList.Add(_currentSelectData.CharID);
                    _currentSelectCardList.Add(card);
                }
                RefreshDetailPanel();
            });
        }
    }
}
