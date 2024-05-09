using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginProcedure : ProcedureBase
{
    uint? bgmID;

    protected override void OnEnter(IFsm<ProcedureManager> fsm)
    {
        base.OnEnter(fsm);
        SceneManager.LoadScene("UIOnlyScene");
        FrameworkEntry.UI.ShowUI(Constant.UIAsset_Login);

        // bgm
        SoundProps sp = new SoundProps();
        sp.loop = true;
        bgmID = FrameworkEntry.Sound.PlayList(
            new List<string> { Constant.SoundAsset_sys_title_intro, Constant.SoundAsset_sys_title_loop },
            Constant.SoundGroupName_BGM, sp);
    }

    protected override void OnLeave(IFsm<ProcedureManager> fsm, bool isShutdown)
    {
        base.OnLeave(fsm, isShutdown);
        FrameworkEntry.UI.CloseUI(Constant.UIAsset_Login);
    }

    protected override void OnUpdate(IFsm<ProcedureManager> fsm, float deltaTime, float realDeltaTime)
    {
        // base.OnUpdate(fsm, deltaTime, realDeltaTime);
    }
}
