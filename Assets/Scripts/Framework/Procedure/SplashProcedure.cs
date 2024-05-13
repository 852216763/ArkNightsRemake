using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashProcedure : ProcedureBase
{
    protected override bool IsEntryProcedure
    {
        get
        {
            return true;
        }
    }

    protected override void OnInit(IFsm<ProcedureManager> fsm)
    {
        base.OnInit(fsm);
        InitSoundGroups();

    }

    protected override void OnEnter(IFsm<ProcedureManager> fsm)
    {
        base.OnEnter(fsm);
        SceneManager.LoadScene("UIOnlyScene");
        FrameworkEntry.UI.ShowUI(Constant.UIAsset_Splash);
    }

    protected override void OnLeave(IFsm<ProcedureManager> fsm, bool isShutdown)
    {
        base.OnLeave(fsm, isShutdown);
        FrameworkEntry.UI.CloseUI(Constant.UIAsset_Splash);
    }

    protected override void OnUpdate(IFsm<ProcedureManager> fsm, float deltaTime, float realDeltaTime)
    {
        // base.OnUpdate(fsm, deltaTime, realDeltaTime);
    }


    // TODO 初始化音频组写死不太好,抽空用配表解决
    private void InitSoundGroups()
    {
        SoundGroup group = null;
        // BGM
        group = FrameworkEntry.Sound.AddSoundGroup(Constant.SoundGroupName_BGM);
        group.Capacity = 1;
        group.ReplaceOnEqualPriority = true;
        group.SetAllMuteState(PlayerPrefs.GetInt(Constant.SettingPlayerPrefs_MusicMute, 0) == 1);

        // UISFX
        group = FrameworkEntry.Sound.AddSoundGroup(Constant.SoundGroupName_UISFX);
        group.SetAllMuteState(PlayerPrefs.GetInt(Constant.SettingPlayerPrefs_UISFXMute, 0) == 1);

        // BattleSFX
        group = FrameworkEntry.Sound.AddSoundGroup(Constant.SoundGroupName_BattleSFX);
        group.Capacity = 20;
        group.ReplaceOnEqualPriority = true;

        // CharacterVoice
        group = FrameworkEntry.Sound.AddSoundGroup(Constant.SoundGroupName_CharacterVoice);
        group.Capacity = 1;
        group.ReplaceOnEqualPriority = true;
        group.SetAllMuteState(PlayerPrefs.GetInt(Constant.SettingPlayerPrefs_VoiceMute, 0) == 1);
    }
}
