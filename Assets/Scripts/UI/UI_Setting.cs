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

public class UI_Setting : UIForm
{
    [SerializeField]
    Sprite toggleBgSprite;

    // 游戏
    [SerializeField]
    Toggle keepX2;

    // 声音
    [SerializeField]
    Toggle musicVolume;
    [SerializeField]
    Scrollbar musicScrollBar;
    [SerializeField]
    Toggle sfxVolume;
    [SerializeField]
    Scrollbar sfxScrollBar;
    [SerializeField]
    Toggle voiceVolume;
    [SerializeField]
    Scrollbar voiceScrollBar;

    RenderTexture _blurCache;
    RawImage _bgImg;

    protected override void OnInit(object userdata = null)
    {
        base.OnInit(userdata);

        transform.Find("BackBtn").GetComponent<Button>().onClick.AddListener(() => OnBackBtnClick());
        _bgImg = transform.Find("BgImg").GetComponent<RawImage>();

        Transform mainPanel = transform.Find("MainPanel");
        Transform tabs = mainPanel.Find("Tabs");
        Transform gameSettingTab = tabs.Find("GameTab");
        Transform soundSettingTab = tabs.Find("SoundTab");

        // 标签组按钮
        Transform toggleGroup = mainPanel.Find("ToggleGroup");
        Transform gameSettingToggle = toggleGroup.Find("GameSetting");
        gameSettingToggle.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
        {
            Image bg = gameSettingToggle.Find("Bg").GetComponent<Image>();
            Image icon = gameSettingToggle.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI label = gameSettingToggle.Find("Label").GetComponent<TextMeshProUGUI>();
            if (isOn)
            {
                bg.sprite = null;
                icon.color = new Color(86 / 255f, 86 / 255f, 86 / 255f);
                label.color = Color.black;
                gameSettingTab.gameObject.SetActive(true);
            }
            else
            {
                bg.sprite = toggleBgSprite;
                icon.color = Color.white;
                label.color = icon.color;
                gameSettingTab.gameObject.SetActive(false);
            }
        });

        Transform soundSettingToggle = toggleGroup.Find("SoundSetting");
        soundSettingToggle.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
        {
            Image bg = soundSettingToggle.Find("Bg").GetComponent<Image>();
            Image icon = soundSettingToggle.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI label = soundSettingToggle.Find("Label").GetComponent<TextMeshProUGUI>();
            if (isOn)
            {
                bg.sprite = null;
                icon.color = new Color(86 / 255f, 86 / 255f, 86 / 255f);
                label.color = Color.black;
                soundSettingTab.gameObject.SetActive(true);
            }
            else
            {
                bg.sprite = toggleBgSprite;
                icon.color = Color.white;
                label.color = icon.color;
                soundSettingTab.gameObject.SetActive(false);
            }
        });

        // 保持二倍速
        keepX2.isOn = PlayerPrefs.GetInt(Constant.SettingPlayerPrefs_KeepX2, 0) == 1;
        if (!keepX2.isOn)
        {
            keepX2.transform.Find("Handler").localPosition *= -1;
        }
        float tweenDuration = 0.3f;
        keepX2.onValueChanged.AddListener(isOn =>
        {
            Transform handler = keepX2.transform.Find("Handler");
            handler.DOComplete();
            handler.DOLocalMoveX(-handler.transform.localPosition.x, tweenDuration);
            if (isOn)
            {
                PlayerPrefs.SetInt(Constant.SettingPlayerPrefs_KeepX2, 1);
            }
            else
            {
                PlayerPrefs.SetInt(Constant.SettingPlayerPrefs_KeepX2, 0);
            }
        });

        // 音乐静音与音量
        musicVolume.isOn = PlayerPrefs.GetInt(Constant.SettingPlayerPrefs_MusicMute, 0) == 0;
        if (!musicVolume.isOn)
        {
            musicVolume.transform.Find("Handler").localPosition *= -1;
        }
        musicVolume.onValueChanged.AddListener(isOn =>
        {
            Transform handler = musicVolume.transform.Find("Handler");
            handler.DOComplete();
            handler.DOLocalMoveX(-handler.transform.localPosition.x, tweenDuration);
            PlayerPrefs.SetInt(Constant.SettingPlayerPrefs_MusicMute, isOn ? 0 : 1);
            FrameworkEntry.Sound.SetGroupMuteState(Constant.SoundGroupName_BGM, !isOn);
        });
        musicScrollBar.value = PlayerPrefs.GetFloat(Constant.SettingPlayerPrefs_MusicVolume, 1);
        musicScrollBar.transform.GetComponentInChildren<TextMeshProUGUI>(true).text = ((int)(musicScrollBar.value * 100)).ToString();
        musicScrollBar.onValueChanged.AddListener(value =>
        {
            value = Mathf.Clamp(value, 0, 1);
            musicScrollBar.transform.GetComponentInChildren<TextMeshProUGUI>(true).text = ((int)(value * 100)).ToString();
            PlayerPrefs.SetFloat(Constant.SettingPlayerPrefs_MusicVolume, value);
            FrameworkEntry.Sound.SetExposedPropValue(Constant.SettingPlayerPrefs_MusicVolume, SoundManager.Remap01ToDB(value));
        });


        // 音效静音与音量
        sfxVolume.isOn = PlayerPrefs.GetInt(Constant.SettingPlayerPrefs_UISFXMute, 0) == 0;
        if (!sfxVolume.isOn)
        {
            sfxVolume.transform.Find("Handler").localPosition *= -1;
        }
        sfxVolume.onValueChanged.AddListener(isOn =>
        {
            Transform handler = sfxVolume.transform.Find("Handler");
            handler.DOComplete();
            handler.DOLocalMoveX(-handler.transform.localPosition.x, tweenDuration);
            PlayerPrefs.SetInt(Constant.SettingPlayerPrefs_UISFXMute, isOn ? 0 : 1);
            FrameworkEntry.Sound.SetGroupMuteState(Constant.SoundGroupName_UISFX, !isOn);
        });
        sfxScrollBar.value = PlayerPrefs.GetFloat(Constant.SettingPlayerPrefs_UISFXVolume, 1);
        sfxScrollBar.transform.GetComponentInChildren<TextMeshProUGUI>(true).text = ((int)(sfxScrollBar.value * 100)).ToString();
        sfxScrollBar.onValueChanged.AddListener(value =>
        {
            value = Mathf.Clamp(value, 0, 1);
            sfxScrollBar.transform.GetComponentInChildren<TextMeshProUGUI>(true).text = ((int)(value * 100)).ToString();
            PlayerPrefs.SetFloat(Constant.SettingPlayerPrefs_UISFXVolume, value);
            FrameworkEntry.Sound.SetExposedPropValue(Constant.SettingPlayerPrefs_UISFXVolume, SoundManager.Remap01ToDB(value));
        });

        // 角色语音静音与音量
        voiceVolume.isOn = PlayerPrefs.GetInt(Constant.SettingPlayerPrefs_VoiceMute, 0) == 0;
        if (!voiceVolume.isOn)
        {
            voiceVolume.transform.Find("Handler").localPosition *= -1;
        }
        voiceVolume.onValueChanged.AddListener(isOn =>
        {
            Transform handler = voiceVolume.transform.Find("Handler");
            handler.DOComplete();
            handler.DOLocalMoveX(-handler.transform.localPosition.x, tweenDuration);
            PlayerPrefs.SetInt(Constant.SettingPlayerPrefs_VoiceMute, isOn ? 0 : 1);
            FrameworkEntry.Sound.SetGroupMuteState(Constant.SoundGroupName_CharacterVoice, !isOn);
        });
        voiceScrollBar.value = PlayerPrefs.GetFloat(Constant.SettingPlayerPrefs_VoiceVolume, 1);
        voiceScrollBar.transform.GetComponentInChildren<TextMeshProUGUI>(true).text = ((int)(voiceScrollBar.value * 100)).ToString();
        voiceScrollBar.onValueChanged.AddListener(value =>
        {
            value = Mathf.Clamp(value, 0, 1);
            voiceScrollBar.transform.GetComponentInChildren<TextMeshProUGUI>(true).text = ((int)(value * 100)).ToString();
            PlayerPrefs.SetFloat(Constant.SettingPlayerPrefs_VoiceVolume, value);
            FrameworkEntry.Sound.SetExposedPropValue(Constant.SettingPlayerPrefs_VoiceVolume, SoundManager.Remap01ToDB(value));
        });

    }

    protected override void OnShow(object userdata = null)
    {
        base.OnShow(userdata);
        _bgImg.color = new Color(1, 1, 1, 0.2f);
        FrameworkEntry.UI.UICamera.GetComponent<UIBlurEffect>().EnableBlurRender(null, rt =>
        {
            _blurCache = rt;
            _bgImg.texture = _blurCache;
            _bgImg.color = Color.white;
        });

        (transform as RectTransform).Fade(0);
        (transform as RectTransform).Fade(1, Constant.FadeTime);
    }

    protected override void OnHide(object userdata = null)
    {
        base.OnHide(userdata);
        if (_blurCache != null)
        {
            _bgImg.texture = null;
            RenderTexture.ReleaseTemporary(_blurCache);
            _blurCache = null;
        }
        _bgImg.color = Color.white;
    }

    /// <summary>
    /// 退出
    /// </summary>
    public void OnBackBtnClick()
    {
        FrameworkEntry.UI.HideUI(gameObject);
    }
}
