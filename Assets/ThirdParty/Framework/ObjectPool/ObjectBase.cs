namespace Framework
{
    /// <summary>
    /// 对象池对象基类
    /// </summary>
    public abstract class ObjectBase : IReference
    {
        protected object _target;

        /// <summary>
        /// 对象名
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// 对象实例
        /// </summary>
        public object Target
        {
            get
            {
                return _target;
            }
        }

        /// <summary>
        /// 获取对象的回调
        /// </summary>
        protected internal virtual void OnSpawn() { }

        /// <summary>
        /// 回收对象的回调
        /// </summary>
        protected internal virtual void OnUnspawn() { }

        /// <summary>
        /// 释放对象
        /// </summary>
        public virtual void Release(bool isShutdown = false)
        {
            ReferencePool.Release(this);
        }

        /// <summary>
        /// 清理引用
        /// </summary>
        public void Clear()
        {
            _target = null;
        }
    }
}
