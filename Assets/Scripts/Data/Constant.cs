using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Constant : MonoBehaviour
{
    #region 设置
    public const string SettingPlayerPrefs_KeepX2 = "Setting_KeepX2";
    public const string SettingPlayerPrefs_MusicMute = "Setting_MusicMute";
    public const string SettingPlayerPrefs_MusicVolume = "MusicVolume";
    public const string SettingPlayerPrefs_UISFXMute = "Setting_UISFXMute";
    public const string SettingPlayerPrefs_UISFXVolume = "UISFXVolume";
    public const string SettingPlayerPrefs_VoiceMute = "Setting_VoiceMute";
    public const string SettingPlayerPrefs_VoiceVolume = "VoiceVolume";

    #endregion

    #region 音频资源路径
    private const string SoundAssetPath_BGM = "Assets/Audio/Bgm/";
    public const string SoundAsset_sys_title_intro = SoundAssetPath_BGM + "sys_title_intro.ogg";
    public const string SoundAsset_sys_title_loop = SoundAssetPath_BGM + "sys_title_loop.ogg";
    public const string SoundAsset_sys_home_intro = SoundAssetPath_BGM + "sys_fesready_intro.ogg";
    public const string SoundAsset_sys_home_loop = SoundAssetPath_BGM + "sys_fesready_loop.ogg";

    private const string SoundAssetPath_UI = "Assets/Audio/UI/";
    public static readonly string[] SoundAsset_UISFX_ButtonSE =
    {
        SoundAssetPath_UI + "btn_click_n.ogg",
        SoundAssetPath_UI + "btn_click_confirm.ogg",
        SoundAssetPath_UI + "btn_back.ogg",

    };


    private const string SoundAssetPath_Voice = "Assets/Audio/Voice/";

    public static string SoundAsset_CharVoice(int id, CharVoiceType type)
    {
        return $"{SoundAssetPath_Voice}{id}/CN_{(int)type:000}.ogg";
    }

    public enum CharVoiceType
    {
        /// <summary>
        /// 闲置
        /// </summary>
        [Description("闲置")]
        Idle = 10,
        /// <summary>
        /// 戳一下
        /// </summary>
        [Description("戳一下")]
        Poke = 34,
        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        Title = 37,
        /// <summary>
        /// 问候
        /// </summary>
        [Description("问候")]
        Greeting = 42,
    }

    public enum CharID
    {
        /// <summary>
        /// 浊蒂
        /// </summary>
        skadi2 = 1,
        /// <summary>
        /// 风笛
        /// </summary>
        bpipe = 2,
        /// <summary>
        /// 星极
        /// </summary>
        astesi = 3,
        /// <summary>
        /// 砾
        /// </summary>
        gravel = 4,
        /// <summary>
        /// 卡缇
        /// </summary>
        cardigan = 5,
        /// <summary>
        /// 夜刀
        /// </summary>
        yato = 6,
    }
    #endregion

    #region 音频组名
    public const string SoundGroupName_BGM = "Bgm";
    public const string SoundGroupName_BattleSFX = "BattleSFX";
    public const string SoundGroupName_UISFX = "UISFX";
    public const string SoundGroupName_CharacterVoice = "CharacterVoice";

    #endregion

    #region 图片资源路径

    #endregion

    #region UI资源路径
    private const string UIAssetPath = "Assets/Prefabs/UI/";
    public const string UIAsset_Splash = UIAssetPath + "UI_splash.prefab";
    public const string UIAsset_Login = UIAssetPath + "UI_login.prefab";
    public const string UIAsset_Home = UIAssetPath + "UI_home.prefab";
    public const string UIAsset_Character = UIAssetPath + "UI_character.prefab";
    public const string UIAsset_CharInfo = UIAssetPath + "UI_charInfo.prefab";
    public const string UIAsset_Setting = UIAssetPath + "UI_setting.prefab";
    #endregion

    #region 通用UI动画参数
    public const float FadeOutTime = 0.4f;

    #endregion

}
