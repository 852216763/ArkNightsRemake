namespace Framework
{
    /// <summary>
    /// 模块管理器基类
    /// </summary>
    public abstract class ManagerBase
    {
        /// <summary>
        /// 模块优先级,值越高越优先轮询,并越晚关闭
        /// </summary>
        internal virtual int Priority
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 轮询模块逻辑
        /// </summary>
        /// <param name="deltaTime">逻辑间隔时间</param>
        /// <param name="realDeltaTime">真实间隔时间</param>
        internal virtual void Update(float deltaTime, float realDeltaTime)
        {

        }

        /// <summary>
        /// 关闭并清理模块
        /// </summary>
        internal abstract void ShutDown();
    }
}
