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

public class UI_Character : UIForm
{
    [SerializeField]
    CharCardScrollView cardContent;
    [SerializeField]
    Color selectSortTMPColor;

    TextMeshProUGUI currentSortSelectTMP;
    Button sortByLevelBtn;
    Button sortByRarityBtn;

    protected override void OnInit(object userdata = null)
    {
        base.OnInit(userdata);

        transform.Find("BackBtn").GetComponent<Button>().onClick.AddListener(() => OnBackBtnClick());
        sortByLevelBtn = transform.Find("SortByLevelBtn").GetComponent<Button>();
        sortByLevelBtn.onClick.AddListener(() => SortCardByLevel());
        sortByRarityBtn = transform.Find("SortByRarityBtn").GetComponent<Button>();
        sortByRarityBtn.onClick.AddListener(() => SortCardByRarity());
        currentSortSelectTMP = sortByLevelBtn.GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void OnShow(object userdata = null)
    {
        base.OnShow(userdata);


        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.Fade(0);
        rect.Fade(1, 0.3f);
        SortCardByLevel();
    }

    /// <summary>
    /// 按稀有度排序角色列表
    /// </summary>
    public void SortCardByRarity()
    {
        currentSortSelectTMP.color = Color.white;
        currentSortSelectTMP = sortByRarityBtn.GetComponentInChildren<TextMeshProUGUI>();
        currentSortSelectTMP.color = selectSortTMPColor;

        // 按照稀有度,精英阶段,等级降序排序
        List<CharData> data = PlayerData.Instance.OwnCharDataList.OrderByDescending(x => x.Meta.Rarity)
            .ThenByDescending(x => x.Elite)
            .ThenByDescending(x => x.CurrentLevel).ToList();

        cardContent.RefreshScrollContent(data);
        RegisteCardBtnClickEvent();
    }

    /// <summary>
    /// 按等级排序角色列表
    /// </summary>
    public void SortCardByLevel()
    {
        currentSortSelectTMP.color = Color.white;
        currentSortSelectTMP = sortByLevelBtn.GetComponentInChildren<TextMeshProUGUI>();
        currentSortSelectTMP.color = selectSortTMPColor;

        // 按照精英阶段,等级,稀有度降序排序
        List<CharData> data = PlayerData.Instance.OwnCharDataList.OrderByDescending(x => x.Elite)
            .ThenByDescending(x => x.CurrentLevel)
            .ThenByDescending(x => x.Meta.Rarity).ToList();

        cardContent.RefreshScrollContent(data);
        RegisteCardBtnClickEvent();
    }

    /// <summary>
    /// 退出
    /// </summary>
    public void OnBackBtnClick()
    {
        FrameworkEntry.UI.HideUI(gameObject);
    }

    private void RegisteCardBtnClickEvent()
    {
        List<CharCard> cardList = cardContent.charCardItemList;
        foreach (CharCard item in cardList)
        {
            Button btn = item.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                UI_CharInfo charInfo = FrameworkEntry.UI.ShowUI(Constant.UIAsset_CharInfo).GetComponent<UI_CharInfo>();
                charInfo?.UpdateData(cardContent.charDataList, item.Data);
            });
        }
    }
}
