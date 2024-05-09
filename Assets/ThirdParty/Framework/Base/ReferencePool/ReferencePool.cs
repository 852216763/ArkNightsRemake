using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 引用池 管理所有引用集合
    /// </summary>
    public static partial class ReferencePool
    {
        static readonly Dictionary<Type, ReferenceCollection> _referenceCollections = new Dictionary<Type, ReferenceCollection>();

        public static int ReferencePoolCount
        {
            get
            {
                return _referenceCollections.Count;
            }
        }

        /// <summary>
        /// 获取引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>获取到的引用对象</returns>
        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }

        /// <summary>
        /// 获取引用
        /// </summary>
        /// <param name="type">引用类型</param>
        /// <returns>获取到的引用对象</returns>
        public static IReference Acquire(Type type)
        {
            return GetReferenceCollection(type).Acquire();
        }

        /// <summary>
        /// 释放引用
        /// </summary>
        /// <param name="reference">要释放的引用对象</param>
        public static void Release(IReference reference)
        {
            if (reference == null)
            {
                throw new Exception("要释放的引用对象为空");
            }

            Type type = reference.GetType();
            GetReferenceCollection(type).Release(reference);
        }

        /// <summary>
        /// 释放所有引用池的未使用引用对象
        /// </summary>
        public static void ClearAll()
        {
            lock (_referenceCollections)
            {
                foreach (KeyValuePair<Type, ReferenceCollection> keyValuePair in _referenceCollections)
                {
                    keyValuePair.Value.RemoveAllUnusedReference();

                    if (keyValuePair.Value.UsingReferenceCount == 0)
                    {
                        _referenceCollections.Remove(keyValuePair.Key);
                    }
                }
            }
        }

        /// <summary>
        /// 获取引用池对象
        /// </summary>
        /// <param name="type">引用池类型</param>
        /// <returns>获取到的引用池对象</returns>
        private static ReferenceCollection GetReferenceCollection(Type type)
        {
            if (type == null)
            {
                throw new Exception("要获取的引用池类型为空");
            }
            ReferenceCollection referenceCollection;
            lock (_referenceCollections)
            {
                if (!_referenceCollections.TryGetValue(type,out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection(type);
                    _referenceCollections.Add(type, referenceCollection);
                }
            }

            return referenceCollection;
        }
    }

}
