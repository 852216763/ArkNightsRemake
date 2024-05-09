using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public sealed class EventManager : ManagerBase
    {
        private EventPool<EventBase> _eventPool;

        internal override int Priority
        {
            get
            {
                return 0;
            }
        }

        public EventManager()
        {
            _eventPool = new EventPool<EventBase>();
        }

        internal override void Update(float deltaTime, float realDeltaTime)
        {
            _eventPool.Update(deltaTime, realDeltaTime);
        }

        internal override void ShutDown()
        {
            _eventPool.ShutDown();
            _eventPool = null;
        }

        /// <summary>
        /// 检查事件是否存在对应的处理函数
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="checkHandler">要检查的事件处理函数</param>
        /// <returns>是否存在事件处理函数</returns>
        public bool CheckExist(Type eventType, EventHandler<EventBase> checkHandler)
        {
            return _eventPool.CheckExist(eventType, checkHandler);
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventHandler">事件处理函数</param>
        public void Subscribe(Type eventType, EventHandler<EventBase> eventHandler)
        {
            _eventPool.Subscribe(eventType, eventHandler);
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventHandler">事件处理函数</param>
        public void Unsubscribe(Type eventType, EventHandler<EventBase> eventHandler)
        {
            _eventPool.Unsubscribe(eventType, eventHandler);
        }

        /// <summary>
        /// 抛出事件  在抛出事件的下一帧处理事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventArgs">事件数据</param>
        public void Fire(object sender, EventBase eventArgs)
        {
            _eventPool.Fire(sender, eventArgs);
        }

        /// <summary>
        /// 立刻抛出事件 在当前帧处理事件,非线程安全
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventArgs">事件数据</param>
        public void FireNow(object sender, EventBase eventArgs)
        {
            _eventPool.FireNow(sender, eventArgs);
        }
    }
}