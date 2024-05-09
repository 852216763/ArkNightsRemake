using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
    public partial class ObjectPoolManager
    {
        /// <summary>
        /// 对象的对象池状态
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        private sealed class Object<T> : IReference where T : ObjectBase
        {
            T _objectBase;
            int _spawnCount;
            DateTime _lastUseTime;

            /// <summary>
            /// 对象名
            /// 用以区分同一对象池中不同模型
            /// </summary>
            public string Name
            {
                get
                {
                    return _objectBase.Name;
                }
            }

            /// <summary>
            /// 使用计数
            /// </summary>
            public int SpawnCount
            {
                get
                {
                    return _spawnCount;
                }
            }

            /// <summary>
            /// 是否正在使用
            /// </summary>
            public bool IsInUse
            {
                get
                {
                    return _spawnCount > 0;
                }
            }

            /// <summary>
            /// 最后使用时间
            /// </summary>
            public DateTime LastUseTime
            {
                get
                {
                    return _lastUseTime;
                }
            }

            /// <summary>
            /// 创建新的对象状态实例
            /// </summary>
            /// <param name="objectBase">对象实例</param>
            /// <param name="spawned">是否已被获取</param>
            /// <returns>创建的对象实例</returns>
            public static Object<T> Create(T objectBase, bool spawned)
            {
                if (objectBase == null)
                {
                    throw new Exception("要生成的对象实例为空");
                }

                Object<T> obj = ReferencePool.Acquire<Object<T>>();
                obj._lastUseTime = DateTime.Now;
                obj._objectBase = objectBase;
                if (spawned)
                {
                    obj._spawnCount = 1;
                    objectBase.OnSpawn();
                }
                else
                {
                    obj._spawnCount = 0;
                }

                return obj;
            }

            /// <summary>
            /// 获取对象引用
            /// </summary>
            /// <returns></returns>
            public T Peek()
            {
                return _objectBase;
            }

            /// <summary>
            /// 从对象池获取对象
            /// </summary>
            /// <returns></returns>
            public T Spawn()
            {
                _spawnCount++;
                _objectBase.OnSpawn();
                return _objectBase;
            }

            /// <summary>
            /// 回收对象
            /// </summary>
            public void UnSpawn()
            {
                _spawnCount--;
                if (_spawnCount < 0)
                {
                    _spawnCount = 0;
                }

                _lastUseTime = DateTime.Now;
                _objectBase.OnUnspawn();
            }

            /// <summary>
            /// 释放对象
            /// </summary>
            /// <param name="isShutdown">是否是关闭对象池时触发</param>
            public void Release(bool isShutdown = false)
            {
                if (IsInUse)
                {
                    UnSpawn();
                }
                _objectBase.Release(isShutdown);
                ReferencePool.Release(this);
            }

            /// <summary>
            /// 清理引用
            /// </summary>
            public void Clear()
            {
                _objectBase = null;
                _spawnCount = 0;
                _lastUseTime = default(DateTime);
            }
        }
    }
}