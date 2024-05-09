using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 有限状态机接口
    /// </summary>
    public interface IFsm<T> where T : class
    {
        /// <summary>
        /// 有限状态机名称
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 有限状态机持有者类型
        /// </summary>
        Type OwnerType
        {
            get;
        }

        /// <summary>
        /// 有限状态机持有者
        /// </summary>
        T Owner
        {
            get;
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        FsmStateBase<T> CurrentState
        {
            get;
        }

        /// <summary>
        /// 当前状态名称
        /// </summary>
        string CurrentStateName
        {
            get;
        }

        /// <summary>
        /// 当前状态持续时间
        /// </summary>
        float CurrentStateTime
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机中状态的数量。
        /// </summary>
        int FsmStateCount
        {
            get;
        }

        /// <summary>
        /// 状态机是否正在运行
        /// </summary>
        bool IsRunning
        {
            get;
        }

        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <typeparam name="TState">作为启动状态的状态类型</typeparam>
        void Start<TState>() where TState : FsmStateBase<T>;

        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <param name="type">作为启动状态的状态类型</param>
        void Start(Type type);

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="TState">要切换至的状态类型</typeparam>
        void ChangeState<TState>() where TState : FsmStateBase<T>;

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="type">要切换至的状态类型</param>
        void ChangeState(Type type);

        /// <summary>
        /// 是否存在有限状态机状态
        /// </summary>
        /// <typeparam name="TState">要检查的有限状态机状态类型</typeparam>
        /// <returns>是否存在有限状态机状态</returns>
        bool HasState<TState>() where TState : FsmStateBase<T>;

        /// <summary>
        /// 是否存在有限状态机状态
        /// </summary>
        /// <param name="type">要检查的有限状态机状态类型</param>
        /// <returns>是否存在有限状态机状态</returns>
        bool HasState(Type type);

        /// <summary>
        /// 获取指定有限状态机状态
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型</typeparam>
        /// <returns>获取的有限状态机状态</returns>
        TState GetState<TState>() where TState : FsmStateBase<T>;

        /// <summary>
        /// 获取指定有限状态机状态
        /// </summary>
        /// <param name="type">要获取的有限状态机状态类型</param>
        /// <returns>获取的有限状态机状态</returns>
        FsmStateBase<T> GetState(Type type);

        /// <summary>
        /// 获取全部有限状态机状态
        /// </summary>
        /// <returns>有限状态机全部状态的列表</returns>
        List<FsmStateBase<T>> GetAllStates();

        /// <summary>
        /// 是否存在有限状态机数据
        /// </summary>
        /// <param name="dataName">要检查的数据名</param>
        /// <returns>是否存在有限状态机数据</returns>
        bool HasData(string dataName);

        /// <summary>
        /// 获取指定有限状态机数据
        /// </summary>
        /// <param name="dataName">要获取的数据名</param>
        /// <returns>获取的指定有限状态机数据</returns>
        object GetData(string dataName);

        /// <summary>
        /// 设置有限状态机数据
        /// </summary>
        /// <param name="dataName">要设置的数据名</param>
        /// <param name="data">要设置的数据</param>
        void SetData(string dataName, object data);

        /// <summary>
        /// 移除有限状态机数据
        /// </summary>
        /// <param name="dataName">要移除的数据名</param>
        /// <returns>是否移除成功</returns>
        bool RemoveData(string dataName);
    }
}
