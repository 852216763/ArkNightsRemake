using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 有限状态机类
    /// </summary>
    /// <typeparam name="T">状态机持有者类型</typeparam>
    internal sealed class Fsm<T> : FsmBase, IReference, IFsm<T> where T : class
    {
        readonly Dictionary<Type, FsmStateBase<T>> _states;
        readonly Dictionary<string, object> _datas;

        /// <summary>
        /// 有限状态机持有者
        /// </summary>
        public T Owner
        {
            get;
            private set;
        }

        /// <summary>
        /// 有限状态机持有者类型
        /// </summary>
        public override Type OwnerType
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public FsmStateBase<T> CurrentState
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前状态名称
        /// </summary>
        public override string CurrentStateName
        {
            get
            {
                return CurrentState == null ? null : CurrentState.GetType().FullName;
            }
        }

        /// <summary>
        /// 当前状态持续时间
        /// </summary>
        public override float CurrentStateTime
        {
            get;
            protected set;
        }

        /// <summary>
        /// 状态机是否正在运行
        /// </summary>
        public override bool IsRunning
        {
            get
            {
                return CurrentState != null;
            }
        }

        /// <summary>
        /// 获取有限状态机中状态的数量
        /// </summary>
        public override int FsmStateCount
        {
            get
            {
                return _states.Count;
            }
        }

        public Fsm()
        {
            _states = new Dictionary<Type, FsmStateBase<T>>();
            _datas = new Dictionary<string, object>();
            Name = null;
            Owner = null;
            CurrentState = null;
            CurrentStateTime = 0;
        }

        /// <summary>
        /// 创建有限状态机
        /// </summary>
        /// <param name="name">状态机名</param>
        /// <param name="owner">状态机持有者</param>
        /// <param name="fsmStateTypes">状态机状态类型列表</param>
        /// <returns>新创建的有限状态机</returns>
        public static Fsm<T> Create(string name, T owner, params Type[] fsmStateTypes)
        {
            if (owner == null)
            {
                throw new Exception("创建状态机的owner为null !");
            }
            if (fsmStateTypes == null || fsmStateTypes.Length < 1)
            {
                throw new Exception("创建的状态机状态数为0!");
            }

            Fsm<T> fsm = ReferencePool.Acquire<Fsm<T>>();

            fsm.Name = name;
            fsm.Owner = owner;
            foreach (Type fsmStateType in fsmStateTypes)
            {
                if (fsmStateType == null)
                {
                    throw new Exception("创建状态机时 有状态为null!");
                }
                if (!typeof(FsmStateBase<T>).IsAssignableFrom(fsmStateType))
                {
                    throw new Exception($"创建状态机时 {fsmStateType.FullName} 不为状态类或其子类!");
                }
                if (fsm._states.ContainsKey(fsmStateType))
                {
                    throw new Exception($"创建状态机时 {fsmStateType.FullName} 状态重复!");
                }

                FsmStateBase<T> instance = (FsmStateBase<T>)Activator.CreateInstance(fsmStateType);
                fsm._states.Add(fsmStateType, instance);
                instance.OnInit(fsm);
            }
            return fsm;
        }

        /// <summary>
        /// 轮询有限状态机
        /// </summary>
        /// <param name="deltaTime">逻辑间隔时间</param>
        /// <param name="realDeltaTime">真实间隔时间</param>
        internal override void Update(float deltaTime, float realDeltaTime)
        {
            if (CurrentState == null)
            {
                return;
            }

            CurrentStateTime += deltaTime;
            CurrentState.OnUpdate(this, deltaTime, realDeltaTime);
        }

        /// <summary>
        /// 关闭有限状态机
        /// </summary>
        internal override void ShutDown()
        {
            ReferencePool.Release(this);
        }

        /// <summary>
        /// 清理有限状态机
        /// </summary>
        public void Clear()
        {
            if (CurrentState!=null)
            {
                CurrentState.OnLeave(this, true);
}

            foreach (KeyValuePair<Type, FsmStateBase<T>> state in _states)
            {
                state.Value.OnDestroy(this);
            }

            _states.Clear();
            _datas.Clear();

            Name = null;
            Owner = null;
            CurrentState = null;
            CurrentStateTime = 0;
        }

        /// <summary>
        /// 以指定初始状态启动有限状态机
        /// </summary>
        /// <typeparam name="TState">启动状态类型</typeparam>
        public void Start<TState>() where TState : FsmStateBase<T>
        {
            if (IsRunning)
            {
                throw new Exception("状态机已经启动, 无法重复启动");
            }

            TState startState = GetState<TState>();
            if (startState == null)
            {
                throw new Exception("状态机中不存在指定启动状态");
            }

            CurrentStateTime = 0;
            CurrentState = startState;
            CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 以指定初始状态启动有限状态机
        /// </summary>
        /// <param name="type">启动状态类型</param>
        public void Start(Type type)
        {
            if (IsRunning)
            {
                throw new Exception("状态机已经启动, 无法重复启动");
            }

            if (type == null)
            {
                throw new Exception("传入的状态机启动状态为空");
            }

            if (!typeof(FsmStateBase<T>).IsAssignableFrom(type))
            {
                throw new Exception("传入的状态机启动状态并不是状态类型");
            }

            FsmStateBase<T> startState = GetState(type);
            if (startState == null)
            {
                throw new Exception("状态机中不存在指定启动状态");
            }

            CurrentStateTime = 0;
            CurrentState = startState;
            CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 切换至指定状态
        /// </summary>
        /// <typeparam name="TState">切换的目标状态类型</typeparam>
        public void ChangeState<TState>() where TState : FsmStateBase<T>
        {
            if (CurrentState == null)
            {
                throw new Exception("状态机未启动,无法切换状态");
            }

            TState startState = GetState<TState>();
            if (startState == null)
            {
                throw new Exception("状态机中不存在要切换的指定状态");
            }
            CurrentState.OnLeave(this);
            CurrentStateTime = 0;
            CurrentState = startState;
            CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 切换至指定状态
        /// </summary>
        /// <param name="type">切换的目标状态类型</param>
        public void ChangeState(Type type)
        {
            if (CurrentState == null)
            {
                throw new Exception("状态机未启动,无法切换状态");
            }

            if (!typeof(FsmStateBase<T>).IsAssignableFrom(type))
            {
                throw new Exception("要切换的类型并不是一个状态类型");
            }

            FsmStateBase<T> startState = GetState(type);
            if (startState == null)
            {
                throw new Exception("状态机中不存在要切换的指定状态");
            }
            CurrentState.OnLeave(this);
            CurrentStateTime = 0;
            CurrentState = startState;
            CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 检查是否存在指定状态
        /// </summary>
        /// <typeparam name="TState">要检查的状态类型</typeparam>
        /// <returns>是否存在指定状态</returns>
        public bool HasState<TState>() where TState : FsmStateBase<T>
        {
            return _states.ContainsKey(typeof(TState));
        }

        /// <summary>
        /// 检查是否存在指定状态
        /// </summary>
        /// <param name="type">要检查的状态类型</param>
        /// <returns>是否存在指定状态</returns>
        public bool HasState(Type type)
        {
            if (type == null)
            {
                throw new Exception("要检查的状态类型为空");
            }

            if (!typeof(FsmStateBase<T>).IsAssignableFrom(type))
            {
                throw new Exception("要检查的类型并不是一个状态类型");
            }

            return _states.ContainsKey(type);
        }

        /// <summary>
        /// 获取指定状态
        /// </summary>
        /// <typeparam name="TState">要获取的状态类型</typeparam>
        /// <returns>获取到的状态</returns>
        public TState GetState<TState>() where TState : FsmStateBase<T>
        {
            if (_states.TryGetValue(typeof(TState), out FsmStateBase<T> value))
            {
                return (TState)value;
            }

            return null;
        }

        /// <summary>
        /// 获取指定状态
        /// </summary>
        /// <param name="type">要获取的状态类型</param>
        /// <returns>获取到的状态</returns>
        public FsmStateBase<T> GetState(Type type)
        {
            if (type == null)
            {
                throw new Exception("没有指定要获得的状态");
            }

            if (!typeof(FsmStateBase<T>).IsAssignableFrom(type))
            {
                throw new Exception("要获得的指定类型不是一个状态类型");
            }

            if (_states.TryGetValue(type, out FsmStateBase<T> value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机中的所有状态
        /// </summary>
        /// <returns>返回有限状态机中的所有状态</returns>
        public List<FsmStateBase<T>> GetAllStates()
        {
            List<FsmStateBase<T>> list = new List<FsmStateBase<T>>();
            foreach (KeyValuePair<Type, FsmStateBase<T>> keyValuePair in _states)
            {
                list.Add(keyValuePair.Value);
            }

            return list;
        }

        /// <summary>
        /// 检查是否存在指定数据
        /// </summary>
        /// <param name="dataName">数据名</param>
        /// <returns>是否存在指定数据</returns>
        public bool HasData(string dataName)
        {
            if (string.IsNullOrEmpty(dataName))
            {
                throw new Exception("要检测的数据名为空");
            }

            return _datas.ContainsKey(dataName);
        }

        /// <summary>
        /// 获取有限状态机指定数据
        /// </summary>
        /// <param name="dataName">数据名</param>
        /// <returns>要获取的数据</returns>
        public object GetData(string dataName)
        {
            if (string.IsNullOrEmpty(dataName))
            {
                throw new Exception("要获取的数据名为空");
            }

            _datas.TryGetValue(dataName, out object value);
            return value;
        }

        /// <summary>
        /// 设置有限状态机数据
        /// </summary>
        /// <param name="dataName">数据名</param>
        /// <param name="data">数据</param>
        public void SetData(string dataName, object data)
        {
            if (string.IsNullOrEmpty(dataName))
            {
                throw new Exception("要设置的数据名为空");
            }

            _datas.Add(dataName, data);
        }

        /// <summary>
        /// 移除有限状态机数据
        /// </summary>
        /// <param name="dataName">数据名</param>
        /// <returns>移除是否成功</returns>
        public bool RemoveData(string dataName)
        {
            if (string.IsNullOrEmpty(dataName))
            {
                throw new Exception("要移除的数据名为空");
            }

            return _datas.Remove(dataName);
        }
    }
}

