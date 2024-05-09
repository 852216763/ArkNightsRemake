using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeProcedure : ProcedureBase
{

    protected override void OnEnter(IFsm<ProcedureManager> fsm)
    {
        base.OnEnter(fsm);

        SceneManager.LoadScene("UIOnlyScene");
        FrameworkEntry.UI.ShowUI(Constant.UIAsset_Home);


    }

    protected override void OnLeave(IFsm<ProcedureManager> fsm, bool isShutdown)
    {
        base.OnLeave(fsm, isShutdown);

        FrameworkEntry.UI.CloseUI(Constant.UIAsset_Home);
    }

    protected override void OnUpdate(IFsm<ProcedureManager> fsm, float deltaTime, float realDeltaTime)
    {
        base.OnUpdate(fsm, deltaTime, realDeltaTime);
    }
}
