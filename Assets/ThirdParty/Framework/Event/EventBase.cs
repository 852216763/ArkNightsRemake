using System;

namespace Framework
{
    /// <summary>
    /// 事件类型基类
    /// </summary>
    public abstract class EventBase : FrameworkEventBase
    {
        /// <summary>
        /// 触发的事件类型
        /// </summary>
        public abstract Type EventType
        {
            get;
        }
    }
}

