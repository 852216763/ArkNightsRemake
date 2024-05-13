using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant : MonoBehaviour
{
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
    public const string SoundAssetPath_Skadi2 = SoundAssetPath_Voice + "char_001_skadi2/";
    public const string SoundAsset_Voice_Title = "CN_037.ogg";
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
    #endregion

    #region 通用UI动画参数
    public const float FadeOutTime = 0.4f;

    #endregion

}
