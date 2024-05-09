using DG.Tweening;
using Framework;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UI_Home : UIForm
{
    readonly Vector2 CanvasScalerResolution = new Vector2(1280, 720);
    uint? bgmID;

    #region 手机信息
    [SerializeField]
    TextMeshProUGUI sysTime;
    [SerializeField]
    Image sysBattery;
    [SerializeField]
    Sprite[] batteryIcons;
    int sysCurrentBatteryLevel = 0;
    #endregion

    #region UI晃动参数
    readonly float floatUIAnimDuration = 0.5f;
    readonly float resetDuration = 1f;
    readonly float floatUIBgMaxOffset = 20;
    readonly float xTriggerThreshold = 0.1f;
    readonly float yTriggerThreshold = 0.05f;
    readonly float defaultFloatUIAngle = 15;
    readonly float floatUIAngleMaxOffset = 5;
    readonly Vector3 floatUIPositionMaxOffset = new Vector3(30, 0);
    Vector3 defaultFloatUILeftPosition;
    Vector3 defaultFloatUIRightPosition;

    Sequence uiAnimSequence;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    float freshMousePosTimer = 0;
    Vector2 lastMousePos;
#endif
    #endregion

    #region 观景模式参数
    readonly float changeObserveModeDuration = 0.2f;

    [SerializeField]
    Button observeBtn;
    [SerializeField]
    Button exitObserveBtn;
    bool isObserveMode;
    #endregion

    #region 助理
    [SerializeField]
    RectTransform voiceDialog;
    TextMeshProUGUI voiceDialogText;
    readonly float voiceDialogFadeDuration = 0.5f;
    readonly float idleVoiceInterval = 30f;
    float idleTimer;

    Transform assistant;
    bool isDynamicSprite;
    SkeletonGraphic assistantSpine;
    readonly float assistantSpecialAnimInterval = 15f;
    WaitForSeconds assistantSpecialAnimYield;
    IEnumerator specialAnim_CO;
    #endregion



    Transform staticPanel;
    Transform floatPanel;
    Transform floatUILeft;
    Transform floatUIRight;

    Transform environmentPanel;
    Transform background;

    protected override void OnInit(object userdata = null)
    {
        base.OnInit(userdata);
        staticPanel = transform.Find("StaticPanel");
        floatPanel = transform.Find("FloatPanel");
        floatPanel.GetComponent<Canvas>().worldCamera = FrameworkEntry.UI.UICamera;
        floatUILeft = floatPanel.Find("LeftPanel");
        floatUIRight = floatPanel.Find("RightPanel");
        environmentPanel = transform.Find("EnvironmentPanel");
        environmentPanel.GetComponent<Canvas>().worldCamera = FrameworkEntry.UI.UICamera;
        background = environmentPanel.Find("Bg");
        assistant = environmentPanel.Find("Assistant");
        assistantSpine = assistant.GetComponent<SkeletonGraphic>();
        isDynamicSprite = assistantSpine != null;

        // UI
        AdaptFloatUIScale();
        defaultFloatUILeftPosition = floatUILeft.localPosition;
        defaultFloatUIRightPosition = floatUIRight.localPosition;

        // 观景模式
        isObserveMode = false;
        observeBtn.onClick.AddListener(() => { ChangeObserveMode(true); });
        exitObserveBtn.gameObject.SetActive(false);
        exitObserveBtn.onClick.AddListener(() => { ChangeObserveMode(false); });

        // 助理
        if (voiceDialog != null)
        {
            voiceDialog.GetComponent<Button>().onClick.AddListener(HideVoiceDialog);
            voiceDialogText = voiceDialog.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnClickAssistant(); });
        assistant.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    protected override void OnShow(object userdata = null)
    {
        base.OnShow(userdata);
        // bgm
        SoundProps sp = new SoundProps();
        sp.loop = true;
        bgmID = FrameworkEntry.Sound.PlayList(
            new List<string> { Constant.SoundAsset_sys_home_intro, Constant.SoundAsset_sys_home_loop },
            Constant.SoundGroupName_BGM, sp);

        if (isDynamicSprite)
        {
            assistantSpecialAnimYield = new WaitForSeconds(assistantSpecialAnimInterval);
            specialAnim_CO = AssistantSpecialAnim_CO();
            StartCoroutine(specialAnim_CO);
        }

        idleTimer = 0;
        ShowVoiceDialog("问候     开屏问候文本");



#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        lastMousePos = Input.mousePosition;
#endif
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        // 更新抬头系统信息
        UpdateSysInfo();

        // UI晃动
        UpdateFloatUIOffset();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR                                     
        FreshMousePos();
#endif

        // 主界面待机语音
        if (Input.GetMouseButtonDown(0))
        {
            idleTimer = 0;
        }
        idleTimer += Time.deltaTime;
        if (idleTimer > idleVoiceInterval)
        {
            idleTimer = 0;
            ShowVoiceDialog("你好闲啊  没班上吗?");
        }
    }

    protected override void OnHide(object userdata = null)
    {
        base.OnHide(userdata);

        if (isDynamicSprite)
        {
            StopCoroutine(specialAnim_CO);
        }
    }

    protected override void OnClose(object userdata = null)
    {
        base.OnClose(userdata);
    }

    #region 手机信息
    private void UpdateSysInfo()
    {
        int batteryLevel = Mathf.RoundToInt(SystemInfo.batteryLevel * (batteryIcons.Length - 1));
        if (batteryLevel != sysCurrentBatteryLevel)
        {
            sysBattery.sprite = batteryIcons[batteryLevel];
            sysCurrentBatteryLevel = batteryLevel;
        }

        sysTime.text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
    }

    #endregion

    #region UI晃动
    private void AdaptFloatUIScale()
    {
        float safeWidth = Screen.safeArea.width;
        float safeHeight = Screen.safeArea.height;
        float scaleX = safeWidth / CanvasScalerResolution.x;
        float scaleY = safeHeight / CanvasScalerResolution.y;
        Vector3 scale = new Vector3(scaleX, scaleY, 1);

        floatPanel.localScale = scale;
        environmentPanel.localScale = scale;
    }

    private void UpdateFloatUIOffset()
    {
        if (uiAnimSequence != null && uiAnimSequence.IsPlaying())
        {
            return;
        }

        float accX;
        float accY;
#if UNITY_ANDROID && false
        accX = Input.gyro.userAcceleration.x;
        accY = Input.gyro.userAcceleration.y;
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        accX = (Input.mousePosition.x - lastMousePos.x) / CanvasScalerResolution.x / 2;
        accY = (Input.mousePosition.y - lastMousePos.y) / CanvasScalerResolution.y / 3;
#endif

        if (Mathf.Abs(accX) <= xTriggerThreshold && Mathf.Abs(accY) <= yTriggerThreshold)
        {
            return;
        }

        if (uiAnimSequence != null)
        {
            uiAnimSequence.Kill();
        }
        uiAnimSequence = DOTween.Sequence();
        uiAnimSequence.SetAutoKill(false);

        if (accX > xTriggerThreshold)
        {
            uiAnimSequence.Join(floatUILeft.DOLocalRotate(Vector3.down * (defaultFloatUIAngle - floatUIAngleMaxOffset), floatUIAnimDuration));
            uiAnimSequence.Join(floatUILeft.DOLocalMove(defaultFloatUILeftPosition + floatUIPositionMaxOffset, floatUIAnimDuration));
            uiAnimSequence.Join(floatUIRight.DOLocalRotate(Vector3.up * (defaultFloatUIAngle + floatUIAngleMaxOffset), floatUIAnimDuration));
            uiAnimSequence.Join(floatUIRight.DOLocalMove(defaultFloatUIRightPosition + floatUIPositionMaxOffset, floatUIAnimDuration));
        }
        else if (accX < -xTriggerThreshold)
        {
            uiAnimSequence.Join(floatUILeft.DOLocalRotate(Vector3.down * (defaultFloatUIAngle + floatUIAngleMaxOffset), floatUIAnimDuration));
            uiAnimSequence.Join(floatUILeft.DOLocalMove(defaultFloatUILeftPosition - floatUIPositionMaxOffset, floatUIAnimDuration));
            uiAnimSequence.Join(floatUIRight.DOLocalRotate(Vector3.up * (defaultFloatUIAngle - floatUIAngleMaxOffset), floatUIAnimDuration));
            uiAnimSequence.Join(floatUIRight.DOLocalMove(defaultFloatUIRightPosition - floatUIPositionMaxOffset, floatUIAnimDuration));
        }
        if (accY > yTriggerThreshold)
        {
            uiAnimSequence.Join(background.DOMoveY(-floatUIBgMaxOffset, floatUIAnimDuration));
        }
        else if (accY < -yTriggerThreshold)
        {
            uiAnimSequence.Join(background.DOMoveY(floatUIBgMaxOffset, floatUIAnimDuration));
        }
        uiAnimSequence.AppendInterval(resetDuration / 2);
        uiAnimSequence.onComplete = () =>
        {
            uiAnimSequence.PlayBackwards();
        };
        uiAnimSequence.onKill = () =>
        {
            uiAnimSequence = null;

        };
        uiAnimSequence.PlayForward();
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    void FreshMousePos()
    {
        freshMousePosTimer += Time.deltaTime;
        if (freshMousePosTimer > 0.2f)
        {
            lastMousePos = Input.mousePosition;
            freshMousePosTimer = 0;
        }
    }
#endif
    #endregion

    #region 观景模式

    private void ChangeObserveMode(bool isOn = false)
    {
        if (isOn == isObserveMode)
        {
            return;
        }
        isObserveMode = isOn;

        if (isOn)
        {
            exitObserveBtn.gameObject.SetActive(true);
            Image obImg = exitObserveBtn.GetComponent<Image>();
            Color obColor = obImg.color;
            obColor.a = 0;
            obImg.color = obColor;
            obImg?.DOFade(1, changeObserveModeDuration);
            staticPanel.GetComponent<RectTransform>().Fade(0f, changeObserveModeDuration,
                () => { staticPanel.gameObject.SetActive(false); });
            floatPanel.GetComponent<RectTransform>().Fade(0f, changeObserveModeDuration,
                () => { floatPanel.gameObject.SetActive(false); });
        }
        else
        {
            Image obImg = exitObserveBtn.GetComponent<Image>();
            Color obColor = obImg.color;
            obColor.a = 1;
            obImg.color = obColor;
            obImg.DOFade(0, changeObserveModeDuration).onComplete = () => { exitObserveBtn.gameObject.SetActive(false); };
            staticPanel.gameObject.SetActive(true);
            staticPanel.GetComponent<RectTransform>().Fade(1f, changeObserveModeDuration);
            floatPanel.gameObject.SetActive(true);
            floatPanel.GetComponent<RectTransform>().Fade(1f, changeObserveModeDuration);
        }
    }

    #endregion

    #region 看板互动

    private void ShowVoiceDialog(string voiceText)
    {
        voiceDialogText.text = voiceText;

        voiceDialog.gameObject.SetActive(true);
        voiceDialog.Fade(1f, voiceDialogFadeDuration);
    }

    private void HideVoiceDialog()
    {
        voiceDialog.Fade(0f, voiceDialogFadeDuration, 
            () => { voiceDialog.gameObject.SetActive(false); });
    }

    private void OnClickAssistant()
    {
        if (isDynamicSprite)
        {
            assistantSpine.AnimationState.SetAnimation(0, "Interact", false)
                .Complete += trackEntry =>
                {
                    assistantSpine.AnimationState.SetAnimation(0, "Idle", true);
                };
        }

        ShowVoiceDialog("想办法配置点语音文本");
        // TODO 播放触摸语音

    }

    private IEnumerator AssistantSpecialAnim_CO()
    {
        yield return assistantSpecialAnimYield;
        if (isDynamicSprite)
        {
            assistantSpine.AnimationState.AddAnimation(0, "Special", false, 0)
                .Complete += trackEntry =>
                {
                    assistantSpine.AnimationState.SetAnimation(0, "Idle", true);
                };
        }
    }

    #endregion
}
