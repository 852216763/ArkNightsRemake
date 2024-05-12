using DG.Tweening;
using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewSplitByPage : ScrollRect
{
    public bool autoScroll = true;
    [Tooltip("自动滚动间隔")]
    public float flipInterval = 2;
    [Tooltip("滚动动画时长")]
    public float flipDuration = 0.5f;
    [Tooltip("翻页灵敏度,仅对小幅拖动翻页有效"), Range(0,1)]
    public float sensitive = 1;
    private WaitForSeconds flipIntervalYield;

    public Transform toggleParent;
    List<Toggle> toggleList;

    float pageStepPercent;
    int currentPage;

    Tweener tween;

    protected override void Start()
    {
        base.Start();
        CalculatePageInfo();
        //horizontalNormalizedPosition = 0;
        //currentPage = 0;
        currentPage = Mathf.RoundToInt(horizontalNormalizedPosition / pageStepPercent);

        if (autoScroll)
        {
            flipIntervalYield = new WaitForSeconds(flipInterval);
        }

        if (toggleParent != null)
        {
            toggleList = new List<Toggle>();
            for (int i = 0; i < toggleParent.childCount; i++)
            {
                Toggle t = toggleParent.GetChild(i).GetComponent<Toggle>();
                if (t == null)
                {
                    Debugger.LogWarning("初始化滚动区选项时有空对象,终止初始化");
                    return;
                }
                toggleList.Add(t);
                if (currentPage == i)
                {
                    t.isOn = true;
                }
                int index = i;
                t.onValueChanged.AddListener(value =>
                {
                    if (value)
                    {
                        GotoPage(index);
                    }
                });
            }
        }

        if (autoScroll)
        {
            StartCoroutine(AutoFlipPage_CO());
        }
    }

    IEnumerator AutoFlipPage_CO()
    {
        while (true)
        {
            yield return flipIntervalYield;
            NextPage();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (tween != null)
        {
            tween.Kill();
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        Debug.Log("拖动结束");
        float horizon = horizontalNormalizedPosition;
        horizon = Mathf.Clamp(horizon, 0, 1);
        int page = Mathf.RoundToInt(horizon / pageStepPercent);
        if (page == currentPage)
        {
            float interval = horizontalNormalizedPosition / pageStepPercent - currentPage;
            if (interval < -sensitive && page > 0)
            {
                page--;
            }
            if (interval > sensitive && page < content.childCount - 1)
            {
                page++;
            }
        }
        if (toggleParent != null)
        {
            toggleList[page].isOn = false;
            toggleList[page].isOn = true;
            return;
        }
        GotoPage(page);
    }

    private void CalculatePageInfo()
    {
        int pageNum = content.childCount;
        if (content.childCount > 1)
        {
            pageNum--;
        }
        pageStepPercent = 1f / pageNum;
    }

    private void GotoPage(int pageNum)
    {
        if (pageNum >= content.childCount || pageNum < 0)
        {
            return;
        }
        if (tween != null)
        {
            tween.Kill();
        }
        currentPage = pageNum;
        float pagePercent = pageStepPercent * currentPage;
        tween = DOTween.To(
            () => horizontalNormalizedPosition,
            value => horizontalNormalizedPosition = value,
            pagePercent, flipDuration);
        tween.onKill = () => {tween = null; };
    }

    public void NextPage()
    {
        int nextPageNum = currentPage + 1;
        nextPageNum %= content.childCount;

        if (toggleParent != null)
        {
            toggleList[nextPageNum].isOn = true;
            return;
        }

        GotoPage(nextPageNum);
    }

    public void BeforePage()
    {
        int beforePageNum = currentPage - 1;
        if (beforePageNum < 0)
        {
            beforePageNum = content.childCount - 1;
        }

        if (toggleParent != null)
        {
            toggleList[beforePageNum].isOn = true;
            return;
        }

        GotoPage(beforePageNum);
    }
}
