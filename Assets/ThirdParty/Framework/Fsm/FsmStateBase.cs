using System;

namespace Framework
{
    /// <summary>
    /// 有限状态机状态基类
    /// </summary>
    /// <typeparam name="T">状态持有者类型</typeparam>
    public abstract class FsmStateBase<T> where T : class
    {
        /// <summary>
        /// 初始化有限状态机状态
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal virtual void OnInit(IFsm<T> fsm) { }

        /// <summary>
        /// 进入有限状态机状态
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal virtual void OnEnter(IFsm<T> fsm) { }

        /// <summary>
        /// 轮询有限状态机状态
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="deltaTime">逻辑间隔时间</param>
        /// <param name="realDeltaTime">真实间隔时间</param>
        protected internal virtual void OnUpdate(IFsm<T> fsm, float deltaTime, float realDeltaTime) { }

        /// <summary>
        /// 离开有限状态机状态
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="isShutdown">是否是关闭有限状态机时触发</param>
        protected internal virtual void OnLeave(IFsm<T> fsm, bool isShutdown = false) { }

        /// <summary>
        /// 销毁有限状态机状态
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal virtual void OnDestroy(IFsm<T> fsm) { }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        /// <param name="fsm">有限状态机引用。</param>
        protected void ChangeState<TState>(IFsm<T> fsm) where TState : FsmStateBase<T>
        {
            if (fsm == null)
            {
                throw new Exception(" 指定状态机为空");
            }

            fsm.ChangeState<TState>();
        }
    }

}