using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 事件池
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    internal partial class EventPool<T> where T : EventBase
    {
        private readonly Dictionary<Type, EventHandler<T>> _eventHandlers;
        private readonly Queue<Event> _events;

        public EventPool()
        {
            _eventHandlers = new Dictionary<Type, EventHandler<T>>();
            _events = new Queue<Event>();
        }

        /// <summary>
        /// 轮询事件队列
        /// </summary>
        /// <param name="deltaTime">逻辑间隔时间</param>
        /// <param name="realDeltaTime">真实间隔时间</param>
        public void Update(float deltaTime, float realDeltaTime)
        {
            if (_events.Count > 0)
            {
                Event e = null;
                lock (_events)
                {
                    e = _events.Dequeue();
                }

                HandleEvent(e.Sender, e.EventArgs);
                ReferencePool.Release(e);
            }
        }

        /// <summary>
        /// 关闭并清理事件池
        /// </summary>
        public void ShutDown()
        {
            _eventHandlers.Clear();
            _events.Clear();
        }

        /// <summary>
        /// 检查对于事件id是否存在对应的事件处理函数
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="checkHandler">要检查的事件处理函数</param>
        /// <returns>是否存在事件处理函数</returns>
        public bool CheckExist(Type eventType, EventHandler<T> checkHandler)
        {
            if (checkHandler == null)
            {
                Debugger.LogError("要检查的事件处理函数为空");
                return false;
            }

            if (_eventHandlers.TryGetValue(eventType, out EventHandler<T> e))
            {
                if (e == null)
                {
                    return false;
                }

                foreach (EventHandler<T> handler in e.GetInvocationList())
                {
                    if (handler == checkHandler)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventHandler">事件处理函数</param>
        public void Subscribe(Type eventType, EventHandler<T> eventHandler)
        {
            if (eventHandler == null)
            {
                Debugger.LogError($"要订阅的事件 {eventType} 处理函数为空");
                return;
            }
            if (!typeof(EventBase).IsAssignableFrom(eventType))
            {
                Debugger.LogError($"要订阅的事件 {eventType} 并非EventBase的子类");
                return;
            }

            if (!_eventHandlers.TryGetValue(eventType, out EventHandler<T> e))
            {
                _eventHandlers[eventType] = eventHandler;
            }
            else
            {
                if (CheckExist(eventType, eventHandler))
                {
                    Debug.LogError($"要订阅的事件 {eventType} 已存在");
                }
                else
                {
                    _eventHandlers[eventType] += eventHandler;
                }
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="eventHandler">事件处理函数</param>
        public void Unsubscribe(Type eventType, EventHandler<T> eventHandler)
        {
            if (eventHandler == null)
            {
                throw new Exception($"要取消订阅的事件 {eventType} 处理函数为空");
            }

            if (CheckExist(eventType, eventHandler))
            {
                _eventHandlers[eventType] -= eventHandler;
            }
            else
            {
                Debugger.LogError($"要取消订阅的事件 {eventType} 没有被订阅");
            }
        }

        /// <summary>
        /// 抛出事件  在抛出事件的下一帧处理事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventArgs">事件数据</param>
        public void Fire(object sender, T eventArgs)
        {
            if (eventArgs == null)
            {
                throw new Exception("抛出的事件为空");
            }

            Event e = Event.Create(sender, eventArgs);
            lock (_events)
            {
                _events.Enqueue(e);
            }
        }

        /// <summary>
        /// 立刻抛出事件 在当前帧处理事件,非线程安全
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventArgs">事件数据</param>
        public void FireNow(object sender, T eventArgs)
        {
            if (eventArgs == null)
            {
                throw new Exception("抛出的事件为空");
            }

            HandleEvent(sender, eventArgs);
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventArgs">事件数据</param>
        private void HandleEvent(object sender, T eventArgs)
        {
            if (_eventHandlers.TryGetValue(eventArgs.EventType, out EventHandler<T> handler))
            {
                if (handler == null)
                {
                    throw new Exception(string.Format("事件类型 {0} 不存在对应事件处理函数", eventArgs.EventType));
                }
                else
                {
                    handler(sender, eventArgs);
                }
            }

            ReferencePool.Release(eventArgs);
        }
    }
}