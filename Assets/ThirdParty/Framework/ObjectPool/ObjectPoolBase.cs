using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 对象池基类
    /// 预留的数据统计访问入口,方便获取无关对象类型的对象池数据信息(用以Debug等)
    /// </summary>
    public abstract class ObjectPoolBase
    {
        /// <summary>
        /// 对象池中对象类型
        /// </summary>
        public abstract Type ObjectType
        {
            get;
        }

        /// <summary>
        /// 池名称字段
        /// 预留字段,用以应对需要 多个同类型对象池 的需求
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// 作为Manager字典中Key的对象池全名
        /// 以Name+Type拼接
        /// </summary>
        public abstract string FullName
        {
            get;
        }

        /// <summary>
        /// 池中可用对象数
        /// </summary>
        public abstract int CanSpawnCount
        {
            get;
        }

        /// <summary>
        /// 可释放对象数
        /// </summary>
        public abstract int CanReleaseCount
        {
            get;
        }

        /// <summary>
        /// 池容量,超出部分在归还时将直接释放
        /// </summary>
        public abstract int Capacity
        {
            set;
            get;
        }

        /// <summary>
        /// 池对象过期时间(秒)
        /// </summary>
        public abstract float ExpireTime_Sec
        {
            set;
            get;
        }

        /// <summary>
        /// 自动释放池中过期对象的间隔
        /// </summary>
        public abstract float AutoReleaseInterval
        {
            set;
            get;
        }

        /// <summary>
        /// 释放符合释放条件的对象
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// 释放池中全部未使用的对象
        /// </summary>
        /// <param name="isShutDown">是否是关闭对象池时触发</param>
        public abstract void ReleaseAllUnused(bool isShutDown = false);

        internal abstract void Update(float deltaTime, float realDeltaTime);

        internal abstract void ShutDown();
    }

}

