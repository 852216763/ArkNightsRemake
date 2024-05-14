using DG.Tweening;
using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CharInfo : UIForm
{
    [SerializeField]
    Sprite[] starsArray;
    [SerializeField]
    Sprite[] professionIconArray;
    [SerializeField]
    Sprite[] eliteIconArray;
    [SerializeField]
    Sprite[] potentialIconArray;


    Transform _leftPanel;
    Transform _rightPanel;
    Transform _charSpritePanel;

    // 左Panel
    //Image _trustFillImg;
    //TextMeshProUGUI _trustValueTMP;
    TextMeshProUGUI _maxHPTMP;
    Image _maxHPFillImg;
    TextMeshProUGUI _attackTMP;
    Image _attackFillImg;
    TextMeshProUGUI _defenceTMP;
    Image _defenceFillImg;
    TextMeshProUGUI _magicResistanceTMP;
    Image _magicResistanceFillImg;
    TextMeshProUGUI _deployCDTMP;
    TextMeshProUGUI _costTMP;
    TextMeshProUGUI _blockTMP;
    TextMeshProUGUI _attackSpeedTMP;
    Image _starsImg;
    TextMeshProUGUI _charENNameTMP;
    TextMeshProUGUI _charCNNameTMP;
    Image _professionImg;
    TextMeshProUGUI _charPositionTMP;
    TextMeshProUGUI _charTagTMP;

    // 右Panel
    Image _expFillImg;
    TextMeshProUGUI _charLv;
    TextMeshProUGUI _charMaxLv;
    TextMeshProUGUI _charExp;
    Image _eliteIcon;
    //Image _potentialIcon;
    Transform _talentItemGroup;
    TextMeshProUGUI[] _charTalentTMP;

    // 立绘
    [Tooltip("翻页灵敏度"), Range(0.1f,1)]
    public float flipSensitive = 0.3f;
    public float flipDuration = 0.2f;
    ScrollRect _charSpriteScrollView;
    Image[] _charSpriteImg;
    float _horizonCache;
    Tween flipTween;

    // 数据
    List<CharData> _dataList;
    CharData _currentData;
    int _currentDataIndex;

    // Dotween
    Sequence _attrNarrowSequence;
    bool isNarrowed = false;
    Sequence _panelHideSequence;

    int CurrentDataIndex
    {
        get => _currentDataIndex;
        set
        {
            if (value >= _dataList.Count || value < 0)
            {
                throw new System.Exception("角色Info数组越界");
            }
            _currentDataIndex = value;
            _currentData = _dataList[value];
        }
    }

    protected override void OnInit(object userdata = null)
    {
        base.OnInit(userdata);

        transform.Find("BackBtn").GetComponent<Button>().onClick.AddListener(() => OnBackBtnClick());
        _leftPanel = transform.Find("LeftPanel");
        _rightPanel = transform.Find("RightPanel");
        _charSpritePanel = transform.Find("CharSpritePanel");

        // 左Panel
        Transform attrValueBar;
        Transform attrPanel = _leftPanel.Find("AttributePanel");
        attrPanel.GetComponent<Button>().onClick.AddListener(() => SwitchAttrPanelState());
        // 最大生命
        Transform maxHP = attrPanel.Find("MaxHP");
        attrValueBar = maxHP.Find("ValueBar");
        _maxHPFillImg = attrValueBar.Find("ValueFill").GetComponent<Image>();
        _maxHPTMP = attrValueBar.Find("Value").GetComponent<TextMeshProUGUI>();
        // 攻击
        Transform attack = attrPanel.Find("Attack");
        attrValueBar = attack.Find("ValueBar");
        _attackFillImg = attrValueBar.Find("ValueFill").GetComponent<Image>();
        _attackTMP = attrValueBar.Find("Value").GetComponent<TextMeshProUGUI>();
        // 防御
        Transform defence = attrPanel.Find("Defence");
        attrValueBar = defence.Find("ValueBar");
        _defenceFillImg = attrValueBar.Find("ValueFill").GetComponent<Image>();
        _defenceTMP = attrValueBar.Find("Value").GetComponent<TextMeshProUGUI>();
        // 法抗
        Transform magicResistance = attrPanel.Find("MagicResistance");
        attrValueBar = magicResistance.Find("ValueBar");
        _magicResistanceFillImg = attrValueBar.Find("ValueFill").GetComponent<Image>();
        _magicResistanceTMP = attrValueBar.Find("Value").GetComponent<TextMeshProUGUI>();
        // 再部署
        Transform deployCD = maxHP.Find("DeployCD");
        attrValueBar = deployCD.Find("ValueBar");
        _deployCDTMP = attrValueBar.Find("Value").GetComponent<TextMeshProUGUI>();
        // 费用
        Transform cost = attack.Find("Cost");
        attrValueBar = cost.Find("ValueBar");
        _costTMP = attrValueBar.Find("Value").GetComponent<TextMeshProUGUI>();
        // 阻挡数
        Transform block = defence.Find("Block");
        attrValueBar = block.Find("ValueBar");
        _blockTMP = attrValueBar.Find("Value").GetComponent<TextMeshProUGUI>();
        // 攻速
        Transform attackSpeed = magicResistance.Find("AttackSpeed");
        attrValueBar = attackSpeed.Find("ValueBar");
        _attackSpeedTMP = attrValueBar.Find("Value").GetComponent<TextMeshProUGUI>();

        _starsImg = _leftPanel.Find("Stars").GetComponent<Image>();
        _charENNameTMP = _leftPanel.Find("CharENName").GetComponent<TextMeshProUGUI>();
        _charCNNameTMP = _leftPanel.Find("CharCNName").GetComponent<TextMeshProUGUI>();
        _professionImg = _leftPanel.Find("Profession").GetChild(0).GetComponent<Image>();
        _charPositionTMP = _leftPanel.Find("Postion").GetComponentInChildren<TextMeshProUGUI>();
        _charTagTMP = _leftPanel.Find("Tag").GetComponentInChildren<TextMeshProUGUI>();

        // 右Panel
        Transform expImg = _rightPanel.Find("ExpImg");
        _expFillImg = expImg.Find("Fill").GetComponent<Image>();
        _charLv = expImg.Find("LvValue").GetComponent<TextMeshProUGUI>();
        _charExp = _rightPanel.Find("ExpBtn").Find("ExpTMP").GetComponent<TextMeshProUGUI>();
        _charMaxLv = _rightPanel.Find("MaxLvTMP").GetComponent<TextMeshProUGUI>();
        _eliteIcon = _rightPanel.Find("EliteBtn").Find("EliteIcon").GetComponent<Image>();
        _talentItemGroup = _rightPanel.Find("Talent").Find("TalentItemGroup");
        _charTalentTMP = _talentItemGroup.GetComponentsInChildren<TextMeshProUGUI>();
        // 立绘ScrollView
        _charSpriteScrollView = _charSpritePanel.GetComponent<ScrollRect>();
        _charSpriteImg = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            _charSpriteImg[i] = _charSpriteScrollView.content.GetChild(i).GetComponent<Image>();
        }
        DragEventHandler handler = _charSpriteScrollView.transform.GetComponent<DragEventHandler>();
        handler.onBeginDrag = OnCharSpriteBeginDrag;
        handler.onDrag = OnCharSpriteDrag;
        handler.onEndDrag = OnCharSpriteEndDrag;

        // Tween动画
        float duration = 0.3f;
        _attrNarrowSequence = DOTween.Sequence();
        _attrNarrowSequence.SetAutoKill(false);
        _attrNarrowSequence.Join(maxHP.GetComponent<RectTransform>().DOSizeDelta(Vector2.one * 25, duration));
        _attrNarrowSequence.Join(maxHP.Find("AttrDesc").GetComponent<TextMeshProUGUI>().DOFade(0, duration));
        _attrNarrowSequence.Join(attack.GetComponent<RectTransform>().DOSizeDelta(Vector2.one * 25, duration));
        _attrNarrowSequence.Join(attack.Find("AttrDesc").GetComponent<TextMeshProUGUI>().DOFade(0, duration));
        _attrNarrowSequence.Join(defence.GetComponent<RectTransform>().DOSizeDelta(Vector2.one * 25, duration));
        _attrNarrowSequence.Join(defence.Find("AttrDesc").GetComponent<TextMeshProUGUI>().DOFade(0, duration));
        _attrNarrowSequence.Join(magicResistance.GetComponent<RectTransform>().DOSizeDelta(Vector2.one * 25, duration));
        _attrNarrowSequence.Join(magicResistance.Find("AttrDesc").GetComponent<TextMeshProUGUI>().DOFade(0, duration));
        _attrNarrowSequence.Join(deployCD.GetComponent<RectTransform>().DOSizeDelta(Vector2.one * 25, duration));
        _attrNarrowSequence.Join(deployCD.Find("AttrDesc").GetComponent<TextMeshProUGUI>().DOFade(0, duration));
        _attrNarrowSequence.Join(cost.GetComponent<RectTransform>().DOSizeDelta(Vector2.one * 25, duration));
        _attrNarrowSequence.Join(cost.Find("AttrDesc").GetComponent<TextMeshProUGUI>().DOFade(0, duration));
        _attrNarrowSequence.Join(block.GetComponent<RectTransform>().DOSizeDelta(Vector2.one * 25, duration));
        _attrNarrowSequence.Join(block.Find("AttrDesc").GetComponent<TextMeshProUGUI>().DOFade(0, duration));
        _attrNarrowSequence.Join(attackSpeed.GetComponent<RectTransform>().DOSizeDelta(Vector2.one * 25, duration));
        _attrNarrowSequence.Join(attackSpeed.Find("AttrDesc").GetComponent<TextMeshProUGUI>().DOFade(0, duration));
        _attrNarrowSequence.Pause();

        duration = 0.6f;
        _panelHideSequence = DOTween.Sequence();
        _panelHideSequence.SetAutoKill(false);
        CanvasGroup leftPanelCanvasGroup = _leftPanel.GetOrAddComponent<CanvasGroup>();
        _panelHideSequence.Join(leftPanelCanvasGroup.DOFade(0, duration));
        _panelHideSequence.Join(_leftPanel.DOLocalMoveX(_leftPanel.localPosition.x - 200, duration));
        CanvasGroup rightPanelCanvasGroup = _rightPanel.GetOrAddComponent<CanvasGroup>();
        _panelHideSequence.Join(rightPanelCanvasGroup.DOFade(0, duration));
        _panelHideSequence.Join(_rightPanel.DOLocalMoveX(_rightPanel.localPosition.x + 200, duration));
        _panelHideSequence.Pause();


    }

    protected override void OnShow(object userdata = null)
    {
        base.OnShow(userdata);

        (transform as RectTransform).Fade(0);
        (transform as RectTransform).Fade(1, Constant.FadeTime);
    }

    protected override void OnHide(object userdata = null)
    {
        base.OnHide(userdata);
    }

    public void UpdateData(List<CharData> dataList, CharData currentData)
    {
        _dataList = dataList;
        for (int i = 0; i < _dataList.Count; i++)
        {
            if (currentData == _dataList[i])
            {
                CurrentDataIndex = i;
                break;
            }
        }

        RefreshInfo();
    }

    /// <summary>
    /// 退出
    /// </summary>
    public void OnBackBtnClick()
    {
        FrameworkEntry.UI.HideUI(gameObject);
    }

    private void RefreshInfo()
    {
        NarrowAttrPanel();
        // 左边
        CharAttribute currentAttr = _currentData.CurrentAttribute;
        CharMeta currentMeta = _currentData.Meta;

        _maxHPFillImg.fillAmount = Mathf.Clamp(currentAttr.MaxHealth / 5000, 0, 1);
        _maxHPTMP.text = currentAttr.MaxHealth.ToString();
        _attackFillImg.fillAmount = Mathf.Clamp(currentAttr.Attack / 2000, 0, 1);
        _attackTMP.text = currentAttr.Attack.ToString();
        _defenceFillImg.fillAmount = Mathf.Clamp(currentAttr.Defence / 2000, 0, 1);
        _defenceTMP.text = currentAttr.Defence.ToString();
        _magicResistanceFillImg.fillAmount = Mathf.Clamp(currentAttr.MagicResistance / 2000, 0, 1);
        _magicResistanceTMP.text = currentAttr.MagicResistance.ToString();

        _deployCDTMP.text = currentMeta.DeployCD > 60 ? "慢" : "快";
        _costTMP.text = currentMeta.Cost.ToString();
        _blockTMP.text = currentMeta.Block.ToString();
        _attackSpeedTMP.text = currentMeta.AttackInterval > 1.2f ? "慢" : "快";

        _starsImg.sprite = starsArray[currentMeta.Rarity - 1];
        _charENNameTMP.text = currentMeta.EnglishName;
        _charCNNameTMP.text = currentMeta.ChineseName;
        _professionImg.sprite = professionIconArray[(int)currentMeta.Profession];
        _charPositionTMP.text = currentMeta.CharPosition.GetDescription();
        StringBuilder sb = new StringBuilder();
        foreach (CharTag item in currentMeta.Tags)
        {
            sb.Append(item.GetDescription());
            sb.Append(' ');
        }
        _charTagTMP.text = sb.ToString().TrimEnd();

        // 右边
        _charLv.text = _currentData.CurrentLevel.ToString();
        _charMaxLv.text = "/" + currentMeta.GetMaxLevel()[(int)_currentData.Elite].ToString();
        _charExp.text = $"<color=#FFD802>{_currentData.CurrentExp}</color>/{_currentData.MaxLevelUpExp}";
        _eliteIcon.sprite = eliteIconArray[(int)_currentData.Elite];
        //_potentialIcon.sprite = potentialIconArray[_currentData.]
        for (int i = 0; i < _charTalentTMP.Length; i++)
        {
            if (i >= currentMeta.Talent.Length)
            {
                _charTalentTMP[i].transform.parent.gameObject.SetActive(false);
                continue;
            }
            _charTalentTMP[i].transform.parent.gameObject.SetActive(true);
            _charTalentTMP[i].text = currentMeta.Talent[i];
            LayoutRebuilder.ForceRebuildLayoutImmediate(_charTalentTMP[i].transform.parent as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_charTalentTMP[i].transform as RectTransform);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_talentItemGroup.transform as RectTransform);

        // 立绘
        if (CurrentDataIndex <= 0)
        {
            _charSpriteImg[2].gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_charSpriteScrollView.content);
        }
        else if (CurrentDataIndex >= _dataList.Count - 1)
        {
            _charSpriteImg[0].gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_charSpriteScrollView.content);
        }
        else
        {
            _charSpriteImg[0].gameObject.SetActive(true);
            _charSpriteImg[0].sprite = _dataList[CurrentDataIndex - 1].Meta.CharSprites[0];
            _charSpriteScrollView.horizontalNormalizedPosition = 0.5f;
            _charSpriteImg[1].sprite = _dataList[CurrentDataIndex].Meta.CharSprites[0];
            _charSpriteImg[2].gameObject.SetActive(true);
            _charSpriteImg[2].sprite = _dataList[CurrentDataIndex + 1].Meta.CharSprites[0];
        }
    }

    private void NarrowAttrPanel()
    {
        if (_attrNarrowSequence.IsPlaying())
        {
            _attrNarrowSequence.Complete();
        }
        isNarrowed = true;
        _attrNarrowSequence.Play();
    }

    private void SwitchAttrPanelState()
    {
        if (_attrNarrowSequence.IsPlaying())
        {
            _attrNarrowSequence.Complete();
        }
        if (isNarrowed)
        {
            _attrNarrowSequence.PlayBackwards();
        }
        else
        {
            _attrNarrowSequence.PlayForward();
        }
        isNarrowed = !isNarrowed;
    }

    private void OnCharSpriteBeginDrag(PointerEventData eventData)
    {
        if (_panelHideSequence.IsPlaying())
        {
            _panelHideSequence.Complete();
        }
        _panelHideSequence.PlayForward();
        if (flipTween != null)
        {
            flipTween.Complete();
        }
        _horizonCache = _charSpriteScrollView.horizontalNormalizedPosition;
    }

    private void OnCharSpriteDrag(PointerEventData eventData)
    {
        List<Image> activeChild = new List<Image>();
        foreach (Image item in _charSpriteImg)
        {
            if (item.gameObject.activeSelf)
            {
                activeChild.Add(item);
            }
        }
        if (activeChild.Count <= 1)
        {
            return;
        }
        float step = 1f / (activeChild.Count - 1);
        for (int i = 0; i < activeChild.Count; i++)
        {
            // 当前拖拽的位置与第i个图的距离
            float distance = Mathf.Abs(i * step - _charSpriteScrollView.horizontalNormalizedPosition);
            // 移动20%的距离才开始淡出
            float startFadeDistance = step * 0.2f;
            distance = Mathf.Clamp01(distance - startFadeDistance);
            float alpha = Mathf.Lerp(1, 0.3f, distance / step);
            activeChild[i].color = new Color(1, 1, 1, alpha);
        }
    }

    private void OnCharSpriteEndDrag(PointerEventData eventData)
    {
        foreach (Image item in _charSpriteImg)
        {
            item.color = Color.white;
        }
        float moveDistance = _charSpriteScrollView.horizontalNormalizedPosition - _horizonCache;
        // 下一个角色Info
        if (moveDistance > flipSensitive)
        {
            flipTween = DOTween.To(() => _charSpriteScrollView.horizontalNormalizedPosition,
                x => _charSpriteScrollView.horizontalNormalizedPosition = x,
                1, flipDuration);
            flipTween.onComplete = () => { CurrentDataIndex++; RefreshInfo(); };
        }
        // 上一个角色Info
        else if (moveDistance < -flipSensitive)
        {
            flipTween = DOTween.To(() => _charSpriteScrollView.horizontalNormalizedPosition,
                x => _charSpriteScrollView.horizontalNormalizedPosition = x,
                0, flipDuration);
            flipTween.onComplete = () => { CurrentDataIndex--; RefreshInfo(); };
        }
        // 滚回当前角色Info
        else
        {
            float pos = 0.5f;
            if (_currentDataIndex <= 0)
            {
                pos = 0;
            }
            else if (_currentDataIndex >= _dataList.Count - 1)
            {
                pos = 1;
            }
            flipTween = DOTween.To(() => _charSpriteScrollView.horizontalNormalizedPosition,
                x => _charSpriteScrollView.horizontalNormalizedPosition = x,
                pos, flipDuration);
        }

        if (_panelHideSequence.IsPlaying())
        {
            _panelHideSequence.Complete();
        }
        _panelHideSequence.PlayBackwards();

    }
}
