using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 状态机管理器
    /// </summary>
    public sealed class FsmManager : ManagerBase
    {
        private readonly Dictionary<string, FsmBase> _fsms;
        private readonly List<FsmBase> _tempFsms;

        public FsmManager()
        {
            _fsms = new Dictionary<string, FsmBase>();
            _tempFsms = new List<FsmBase>();
        }

        internal override void Update(float deltaTime, float realDeltaTime)
        {
            if (_fsms.Count <= 0)
            {
                return;
            }

            _tempFsms.Clear();
            foreach (KeyValuePair<string, FsmBase> fsm in _fsms)
            {
                _tempFsms.Add(fsm.Value);
            }

            foreach (FsmBase fsm in _tempFsms)
            {
                if (!fsm.IsRunning)
                {
                    continue;
                }

                fsm.Update(deltaTime, realDeltaTime);
            }
        }

        internal override void ShutDown()
        {
            foreach (KeyValuePair<string, FsmBase> keyValuePair in _fsms)
            {
                keyValuePair.Value.ShutDown();
            }

            _fsms.Clear();
            _tempFsms.Clear();
        }

        /// <summary>
        /// 创建有限状态机
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型</typeparam>
        /// <param name="owner">有限状态机持有者</param>
        /// <param name="fsmStateTypes">有限状态机状态类型列表</param>
        /// <returns>创建完成的有限状态机</returns>
        public IFsm<T> CreateFsm<T>(T owner, params Type[] fsmStateTypes) where T : class
        {
            return CreateFsm<T>(typeof(T).FullName, owner, fsmStateTypes);
        }

        /// <summary>
        /// 创建有限状态机
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型</typeparam>
        /// <param name="name">有限状态机名称</param>
        /// <param name="owner">有限状态机持有者</param>
        /// <param name="fsmStateTypes">有限状态机状态类型列表</param>
        /// <returns>创建完成的有限状态机</returns>
        public IFsm<T> CreateFsm<T>(string name, T owner, params Type[] fsmStateTypes) where T : class
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).FullName;
            }

            if (HasFsm(name))
            {
                throw new Exception("已经存在想要创建的有限状态机: " + name);
            }

            Fsm<T> fsm = Fsm<T>.Create(name, owner, fsmStateTypes);
            _fsms.Add(name, fsm);
            return fsm;
        }

        /// <summary>
        /// 检查是否存在默认名称的有限状态机
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型</typeparam>
        /// <returns>是否存在默认名称的有限状态机</returns>
        public bool HasFsm<T>()
        {
            return HasFsm(typeof(T).FullName);
        }

        /// <summary>
        /// 检查是否存在有限状态机
        /// </summary>
        /// <param name="name">有限状态机名称</param>
        /// <returns>是否存在有限状态机</returns>
        public bool HasFsm(string name = null)
        {
            return _fsms.ContainsKey(name);
        }

        /// <summary>
        /// 获取有限状态机
        /// </summary>
        /// <typeparam name="T">持有者类型</typeparam>
        /// <param name="name">有限状态机名称</param>
        /// <returns>获取到的有限状态机</returns>
        public IFsm<T> GetFsm<T>(string name = null) where T : class
        {
            if(name == null)
            {
                name = typeof(T).FullName;
            }

            if (_fsms.TryGetValue(name, out FsmBase value))
            {
                return (IFsm<T>)value;
            }

            return null;
        }

        /// <summary>
        /// 销毁有限状态机
        /// </summary>
        /// <typeparam name="T">持有者类型</typeparam>
        /// <param name="fsm">要销毁的状态机</param>
        /// <returns>是否销毁成功</returns>
        public bool DestroyFsm<T>(IFsm<T> fsm = null) where T : class
        {
            string name;
            if(fsm == null)
            {
                name = typeof(T).FullName;
            }
            else
            {
                name = fsm.Name;
            }

            return DestroyFsm(name);
        }

        /// <summary>
        /// 销毁指定名称的有限状态机
        /// </summary>
        /// <param name="name">有限状态机名称</param>
        /// <returns>是否销毁成功</returns>
        public bool DestroyFsm(string name)
        {
            if (_fsms.TryGetValue(name, out FsmBase value))
            {
                value.ShutDown();
                return _fsms.Remove(name);
            }

            return false;
        }


    }
}
