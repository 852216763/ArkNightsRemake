using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static partial class ReferencePool
    {
        /// <summary>
        /// 引用集合
        /// </summary>
        private sealed class ReferenceCollection
        {
            private readonly Queue<IReference> _references;
            private readonly HashSet<IReference> _referenceCache;
            private readonly Type _referenceType;

            public ReferenceCollection(Type type)
            {
                _references = new Queue<IReference>();
                _referenceCache = new HashSet<IReference>();
                _referenceType = type;

                UsingReferenceCount = 0;
            }

            /// <summary>
            /// 所有引用数
            /// </summary>
            public int AllReferenceCount
            {
                get
                {
                    return UsingReferenceCount + UnusedReferenceCount;
                }
            }

            /// <summary>
            /// 正在使用的引用数
            /// </summary>
            public int UsingReferenceCount
            {
                get;
                private set;
            }

            /// <summary>
            /// 可用的引用
            /// </summary>
            public int UnusedReferenceCount
            {
                get
                {
                    return _references.Count;
                }
            }

            /// <summary>
            /// 获取引用
            /// </summary>
            /// <typeparam name="T">要获取的引用类型</typeparam>
            /// <returns>获取的引用对象</returns>
            public T Acquire<T>() where T : class,IReference, new()
            {
                if (_referenceType != typeof(T))
                {
                    throw new Exception("要获取的引用类型与引用池类型不一致");
                }

                UsingReferenceCount++;
                lock (_references)
                {
                    if (_references.Count > 0)
                    {
                        IReference refer = _references.Dequeue();
                        _referenceCache.Remove(refer);
                        return (T)refer;
                    }
                }

                return new T();
            }

            /// <summary>
            /// 获取引用
            /// </summary>
            /// <returns>获取的引用对象</returns>
            public IReference Acquire()
            {
                UsingReferenceCount++;
                lock (_references)
                {
                    if (_references.Count > 0)
                    {
                        IReference refer = _references.Dequeue();
                        _referenceCache.Remove(refer);
                        return refer;
                    }
                }

                return (IReference)Activator.CreateInstance(_referenceType);
            }

            /// <summary>
            /// 释放引用
            /// </summary>
            /// <param name="reference">要释放的引用对象</param>
            public void Release(IReference reference)
            {
                if (_referenceCache.Contains(reference))
                {
                    return;
                }

                reference.Clear();
                lock (_references)
                {
                    _references.Enqueue(reference);
                    _referenceCache.Add(reference);
                }

                UsingReferenceCount--;
                if (UsingReferenceCount < 0)
                {
                    UsingReferenceCount = 0;
                }
            }

            /// <summary>
            /// 移除引用池中所有未使用的引用
            /// </summary>
            /// <returns>移除的引用数量</returns>
            public int RemoveAllUnusedReference()
            {
                int count = _references.Count;
                lock (_references)
                {
                    _references.Clear();
                    _referenceCache.Clear();
                }

                return count;
            }

        }
    }
}
