using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 对象池管理器
    /// </summary>
    public sealed partial class ObjectPoolManager : ManagerBase
    {
        Dictionary<string, ObjectPoolBase> _ObjectPools;
        List<ObjectPoolBase> _CachedObjectPoolsList;

        public ObjectPoolManager()
        {
            _ObjectPools = new Dictionary<string, ObjectPoolBase>();
            _CachedObjectPoolsList = new List<ObjectPoolBase>();
        }

        internal override void Update(float deltaTime, float realDeltaTime)
        {
            foreach (ObjectPoolBase pool in _CachedObjectPoolsList)
            {
                pool.Update(deltaTime, realDeltaTime);
            }
        }

        internal override void ShutDown()
        {
            foreach (ObjectPoolBase pool in _CachedObjectPoolsList)
            {
                pool.ShutDown();
            }

            _CachedObjectPoolsList.Clear();
            _ObjectPools.Clear();
        }

        /// <summary>
        /// 是否存在指定对象池
        /// </summary>
        /// <typeparam name="T">对象池泛型类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>是否存在指定对象池</returns>
        public bool HasObjectPool<T>(string name = "") where T : ObjectBase
        {
            return _ObjectPools.ContainsKey(GetFullName<T>(name));
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <typeparam name="T">对象池泛型类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>要获取的对象池,若不存在则返回null</returns>
        public IObjectPool<T> GetObjectPool<T>(string name = "") where T : ObjectBase
        {
            if (HasObjectPool<T>())
            {
                return (IObjectPool<T>)_ObjectPools[GetFullName<T>(name)];
            }

            return null;
        }

        /// <summary>
        /// 获取对象池,若不存在则创建
        /// </summary>
        /// <typeparam name="T">对象池泛型类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>获取到的对象池</returns>
        public IObjectPool<T> GetOrCreateObjectPool<T>(string name = "") where T : ObjectBase
        {
            IObjectPool<T> pool = GetObjectPool<T>(name);
            if (pool == null)
            {
                pool = new ObjectPool<T>(name);
                _ObjectPools.Add(pool.FullName, (ObjectPoolBase)pool);
                _CachedObjectPoolsList.Add((ObjectPoolBase)pool);
            }

            return pool;
        }

        /// <summary>
        /// 获取符合条件的全部对象池
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>符合条件的对象池集合</returns>
        public ObjectPoolBase[] GetObjectPools(Predicate<ObjectPoolBase> condition)
        {
            return _CachedObjectPoolsList.FindAll(condition).ToArray();
        }

        /// <summary>
        /// 获取全部对象池
        /// </summary>
        /// <returns>全部对象池集合</returns>
        public ObjectPoolBase[] GetAllObjectPools()
        {
            return _CachedObjectPoolsList.ToArray();
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <typeparam name="T">对象池泛型类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>是否有销毁指定的对象池</returns>
        public bool DestoryObjectPool<T>(string name = "") where T : ObjectBase
        {
            if (!HasObjectPool<T>(name))
            {
                return false;
            }

            ObjectPoolBase pool = _ObjectPools[GetFullName<T>(name)];
            pool.ShutDown();
            _CachedObjectPoolsList.Remove(pool);
            _ObjectPools.Remove(pool.FullName);
            return true;
        }

        /// <summary>
        /// 释放全部对象池未使用的对象
        /// </summary>
        public void ReleaseAllUnused()
        {
            foreach (ObjectPoolBase pool in _CachedObjectPoolsList)
            {
                pool.ReleaseAllUnused();
            }
        }

        /// <summary>
        /// 根据类型T和name拼接出对象池全名
        /// </summary>
        /// <typeparam name="T">对象池泛型类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>T与name对应的对象池全名</returns>
        private string GetFullName<T>(string name)
        {
            return name + typeof(T).FullName;
        }
    }

}
