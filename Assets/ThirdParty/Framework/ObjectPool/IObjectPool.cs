using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 对象池接口
    /// </summary>
    public interface IObjectPool<T> where T : ObjectBase
    {
        /// <summary>
        /// 池名称字段
        /// 预留字段,用以应对需要 多个同类型对象池 的需求
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// 作为Manager字典中Key的对象池全名
        /// 以Name+Type拼接
        /// </summary>
        public string FullName
        {
            get;
        }

        /// <summary>
        /// 池中可用对象数
        /// </summary>
        public int CanSpawnCount
        {
            get;
        }

        /// <summary>
        /// 可释放对象数
        /// </summary>
        public int CanReleaseCount
        {
            get;
        }

        /// <summary>
        /// 池容量,超出部分在归还时将直接释放
        /// </summary>
        public int Capacity
        {
            set;
            get;
        }

        /// <summary>
        /// 池对象过期时间(秒)
        /// </summary>
        public float ExpireTime_Sec
        {
            set;
            get;
        }

        /// <summary>
        /// 自动释放池中过期对象的间隔
        /// </summary>
        public float AutoReleaseInterval
        {
            set;
            get;
        }

        /// <summary>
        /// 检查是否存在可获取的对象
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <returns>是否存在可获取的对象</returns>
        public bool CanSpawn(string name = null);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <returns>要获取的对象,无可获取的对象时返回null</returns>
        public T Spawn(string name);

        /// <summary>
        /// 归还对象
        /// </summary>
        /// <param name="obj">要归还的对象</param>
        public void Unspawn(T obj);

        /// <summary>
        /// 注册新对象
        /// </summary>
        /// <param name="obj">要注册的对象</param>
        /// <param name="isSpawned">是否已经在使用</param>
        public void Register(T obj, bool isSpawned = true);

        /// <summary>
        /// 释放指定对象
        /// </summary>
        /// <param name="obj">要释放的对象</param>
        public void Release(T obj);

        /// <summary>
        /// 释放符合释放条件的对象
        /// </summary>
        public void Release();

        /// <summary>
        /// 释放池中全部未使用的对象
        /// </summary>
        /// <param name="isShutDown">是否在关闭对象池时触发</param>
        public void ReleaseAllUnused(bool isShutDown = false);
    }
}
