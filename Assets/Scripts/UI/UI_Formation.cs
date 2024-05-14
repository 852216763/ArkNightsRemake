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

public class UI_Formation : UIForm
{
    [SerializeField]
    GameObject charCardItemPrefab;

    Transform _charPanel;

    protected override void OnInit(object userdata = null)
    {
        base.OnInit(userdata);

        transform.Find("BackBtn").GetComponent<Button>().onClick.AddListener(() => OnBackBtnClick());
        transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(() => OnDeleteBtnClick());
        transform.Find("QuickSelectBtn").GetComponent<Button>().onClick.AddListener(() => OnQuickSelectBtnClick());

        _charPanel = transform.Find("CharPanel");
        FrameworkEntry.DataNode.SetData("UI/Formation/MaxOperator", _charPanel.childCount);
        for (int i = 0; i < _charPanel.childCount; i++)
        {
            int indexAtFormationList = i;
            Button emptyItem = _charPanel.GetChild(indexAtFormationList).GetComponent<Button>();
           emptyItem.onClick.AddListener(() =>
            {
                FrameworkEntry.UI.ShowUI(Constant.UIAsset_CharSelect, indexAtFormationList);
            });
            GameObject charCardItem = GameObject.Instantiate(charCardItemPrefab, emptyItem.transform);
            charCardItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                FrameworkEntry.UI.ShowUI(Constant.UIAsset_CharSelect, indexAtFormationList);
            });
        }
        RefreshCharPanel();
    }

    protected override void OnShow(object userdata = null)
    {
        base.OnShow(userdata);
        FrameworkEntry.Event.Subscribe(FormationListChangedEventData.eventType, RefreshCharPanel);

        (transform as RectTransform).Fade(0);
        (transform as RectTransform).Fade(1, Constant.FadeTime);
    }

    protected override void OnHide(object userdata = null)
    {
        base.OnHide(userdata);
        FrameworkEntry.Event.Unsubscribe(FormationListChangedEventData.eventType, RefreshCharPanel);
    }

    /// <summary>
    /// 删除编队
    /// </summary>
    public void OnDeleteBtnClick()
    {
        PlayerData.Instance.FormationList = null;
    }

    /// <summary>
    /// 快速编辑编队
    /// </summary>
    public void OnQuickSelectBtnClick()
    {
        FrameworkEntry.UI.ShowUI(Constant.UIAsset_CharSelect);
    }

    /// <summary>
    /// 退出
    /// </summary>
    public void OnBackBtnClick()
    {
        FrameworkEntry.UI.HideUI(gameObject);
    }

    private void RefreshCharPanel(object sender = null, EventArgs e = null)
    {
        List<int> formation = PlayerData.Instance.FormationList;
        int index = 0;
        foreach (Transform item in _charPanel)
        {
            GameObject charCardItem = item.GetChild(0).gameObject;
            if (index < formation.Count)
            {
                charCardItem.SetActive(true);
                charCardItem.GetComponent<CharCard>().UpdateCard(
                    PlayerData.Instance.OwnCharDataList.Find(data => data.CharID == formation[index]));
            }
            else
            {
                charCardItem.SetActive(false);
            }
            index++;
        }
    }
}
