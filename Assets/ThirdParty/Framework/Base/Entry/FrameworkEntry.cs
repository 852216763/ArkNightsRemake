using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Framework
{
    /// <summary>
    /// 框架入口,维护所有模块管理器
    /// </summary>
    public partial class FrameworkEntry : MonoSingletonScript<FrameworkEntry>
    {
        static LinkedList<ManagerBase> _managers = new LinkedList<ManagerBase>();

        public ProcedureBase procedureClass;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = "FrameworkRoot";

            // 启动流程管理器
            _procedure = Procedure;
        }

        void Update()
        {
            foreach (ManagerBase manager in _managers)
            {
                manager.Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        void OnDestroy()
        {
            for (LinkedListNode<ManagerBase> manager = _managers.Last; manager != null; manager = manager.Previous)
            {
                manager.Value.ShutDown();
            }
            _managers.Clear();
        }

        /// <summary>
        /// 获取指定类型管理器
        /// </summary>
        /// <typeparam name="T">管理器类型</typeparam>
        /// <returns>获取到的管理器</returns>
        public static T GetManager<T>() where T : ManagerBase
        {
            Type managerType = typeof(T);
            foreach (ManagerBase manager in _managers)
            {
                if (manager.GetType() == managerType)
                {
                    return manager as T;
                }
            }

            //没找到就创建
            return CreateManager(managerType) as T;
        }

        /// <summary>
        /// 创建管理器
        /// </summary>
        /// <param name="managerType">管理器类型</param>
        /// <returns>创建的管理器</returns>
        private static ManagerBase CreateManager(Type managerType)
        {
            ManagerBase manager = (ManagerBase)Activator.CreateInstance(managerType);
            if (manager == null)
            {
                Debugger.LogError("无法创建管理器: " + managerType.Name);
            }

            // 根据优先级插入管理器链表
            LinkedListNode<ManagerBase> current = _managers.First;
            while (current != null)
            {
                if (manager.Priority > current.Value.Priority)
                {
                    break;
                }
                current = current.Next;
            }
            if (current != null)
            {
                _managers.AddBefore(current, manager);
            }
            else
            {
                _managers.AddLast(manager);
            }

            return manager;
        }
    }
}
