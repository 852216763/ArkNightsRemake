using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 流程基类
    /// </summary>
    public abstract class ProcedureBase : FsmStateBase<ProcedureManager>
    {
        /// <summary>
        /// 是否为入口流程
        /// </summary>
        protected internal virtual bool IsEntryProcedure
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 进入流程
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal override void OnEnter(IFsm<ProcedureManager> fsm)
        {
            base.OnEnter(fsm);
            Debugger.Log("进入流程" + GetType().FullName);
        }

        /// <summary>
        /// 轮询流程
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="deltaTime">逻辑间隔时间</param>
        /// <param name="realDeltaTime">真实间隔时间</param>
        protected internal override void OnUpdate(IFsm<ProcedureManager> fsm, float deltaTime, float realDeltaTime)
        {
            base.OnUpdate(fsm, deltaTime, realDeltaTime);
        }

        /// <summary>
        /// 离开流程
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="isShutdown">是否是关闭有限状态机时触发</param>
        protected internal override void OnLeave(IFsm<ProcedureManager> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            Debugger.Log("离开流程: " + GetType().FullName);
        }
    }

}
