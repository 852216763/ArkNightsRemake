using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 有限状态机基类
    /// 不同状态机持有者类型不同,实际使用的有限状态机都继承该类,方便Manager管理
    /// </summary>
    internal abstract class FsmBase
    {
        private string _name;

        /// <summary>
        /// 有限状态机名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            protected set
            {
                _name = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 有限状态机持有者类型
        /// </summary>
        public abstract Type OwnerType
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机中状态的数量
        /// </summary>
        public abstract int FsmStateCount
        {
            get;
        }

        /// <summary>
        /// 当前状态名称
        /// </summary>
        public abstract string CurrentStateName
        {
            get;
        }

        /// <summary>
        /// 当前状态持续时间
        /// </summary>
        public abstract float CurrentStateTime
        {
            get;
            protected set;
        }

        /// <summary>
        /// 状态机是否正在运行
        /// </summary>
        public abstract bool IsRunning
        {
            get;
        }

        /// <summary>
        /// 轮询
        /// </summary>
        /// <param name="deltaTime">逻辑间隔时间</param>
        /// <param name="realDeltaTime">真实间隔时间</param>
        internal abstract void Update(float deltaTime, float realDeltaTime);

        /// <summary>
        /// 关闭并清理状态机
        /// </summary>
        internal abstract void ShutDown();
    }
}
