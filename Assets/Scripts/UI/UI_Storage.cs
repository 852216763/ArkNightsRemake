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
using XLua;

[LuaCallCSharp]
public class UI_Storage : LuaUIForm
{
    [SerializeField]
    Color selectSortTMPColor;

    TextMeshProUGUI _currentSortSelectTMP;
    ScrollRect _itemSR;

    Button _backBtn;


    protected override void OnInit(object userdata = null)
    {
        base.OnInit(userdata);
        _backBtn.targetGraphic.color = Color.white;
    }

    protected override void OnShow(object userdata = null)
    {
        base.OnShow(userdata);

        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.Fade(0);
        rect.Fade(1, Constant.FadeTime);
        PlayerData.Instance.OwnItemDataList.OrderBy(x => { return true; });
    }

    protected override void OnHide(object userdata = null)
    {
        base.OnHide(userdata);



    }

    ///// <summary>
    ///// 按等级排序角色列表
    ///// </summary>
    //public void SortCardByLevel()
    //{
    //    _currentSortSelectTMP.color = Color.white;
    //    _currentSortSelectTMP = _sortByLevelBtn.GetComponentInChildren<TextMeshProUGUI>();
    //    _currentSortSelectTMP.color = selectSortTMPColor;

    //    List<CharData> displayableData = PlayerData.Instance.OwnCharDataList;
    //    if (_singleSelectMode)
    //    {
    //        displayableData = displayableData.
    //            Where(x => x.CharID == _currentSelectData?.CharID
    //            || !PlayerData.Instance.FormationList.Contains(x.CharID)).ToList();
    //    }
    //    // 按照选中状态精英阶段,等级,稀有度降序排序
    //    List<CharData> data = displayableData
    //        .OrderByDescending(x => _currentFormationList.Contains(x.CharID))
    //        .ThenByDescending(x => x.Elite)
    //        .ThenByDescending(x => x.CurrentLevel)
    //        .ThenByDescending(x => x.Meta.Rarity).ToList();

    //    _charCardSV.RefreshScrollContent(data);
    //    ArrayList arr = new ArrayList();
    //    arr.ToArray().OrderByDescending(x => true);
    //    RegisteCardBtnClickEvent();
    //}

    ///// <summary>
    ///// 退出
    ///// </summary>
    //public void OnBackBtnClick()
    //{
    //    FrameworkEntry.UI.HideUI(gameObject);
    //}

    //private void OnConfirmBtnClick()
    //{
    //    FrameworkEntry.UI.HideUI(gameObject);
    //}

    ///// <summary>
    ///// 跳转至当前选中的角色Info界面
    ///// </summary>
    //private void GoToSelectedCharInfo()
    //{
    //    if (_currentSelectData != null)
    //    {
    //        UI_CharInfo charInfo = FrameworkEntry.UI.ShowUI(Constant.UIAsset_CharInfo).GetComponent<UI_CharInfo>();
    //        charInfo?.UpdateData(new List<CharData> { _currentSelectData }, _currentSelectData);
    //    }
    //}

    ///// <summary>
    ///// 注册/更新所有Card的点击事件
    ///// </summary>
    //private void RegisteCardBtnClickEvent()
    //{
    //    List<CharCard> cardList = _charCardSV.charCardItemList;
    //    foreach (CharCard item in cardList)
    //    {
    //        CharCard card = item;
    //        Button btn = card.GetComponent<Button>();
    //        btn.onClick.RemoveAllListeners();
    //    }
    //}
}
