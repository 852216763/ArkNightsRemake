using UnityEngine;

namespace Framework
{
    internal partial class EventPool<T> where T : EventBase
    {
        /// <summary>
        /// 事件消息
        /// </summary>
        private class Event : IReference
        {

            /// <summary>
            /// 事件发送者
            /// </summary>
            public object Sender
            {
                get;
                private set;
            }

            /// <summary>
            /// 事件数据
            /// </summary>
            public T EventArgs
            {
                get;
                private set;
            }

            public Event()
            {
                Sender = null;
                EventArgs = null;
            }

            /// <summary>
            /// 创建事件消息
            /// </summary>
            /// <param name="sender">事件发送者</param>
            /// <param name="eventArgs">事件数据</param>
            /// <returns></returns>
            public static Event Create(object sender, T eventArgs)
            {
                Event e = ReferencePool.Acquire<Event>();
                e.Sender = sender;
                e.EventArgs = eventArgs;
                return e;
            }

            /// <summary>
            /// 清理引用
            /// </summary>
            public void Clear()
            {
                if (EventArgs != null)
                {
                    ReferencePool.Release(EventArgs);
                }
                Sender = null;
                EventArgs = null;
            }
        }
    }
}