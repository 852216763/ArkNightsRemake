using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    public partial class DataNodeManager
    {
        /// <summary>
        /// 数据结点  数据结点管理器的内部类
        /// </summary>
        private class DataNode : IDataNode, IReference
        {
            private object _data;
            private readonly List<DataNode> _childs;
            private readonly List<IDataNode> _tempList;

            /// <summary>
            /// 数据结点名称
            /// </summary>
            public string Name
            {
                get;
                private set;
            }

            /// <summary>
            /// 数据结点完整名称(含路径名)
            /// </summary>
            public string FullName
            {
                get
                {
                    return Parent == null ? Name : string.Format("{0}{1}{2}", Parent.FullName, PathSplitSeparator[0], Name);
                }
            }

            /// <summary>
            /// 数据结点的父节点
            /// </summary>
            public IDataNode Parent
            {
                get;
                private set;
            }

            /// <summary>
            /// 数据结点的子节点数
            /// </summary>
            public int ChildCount
            {
                get
                {
                    return _childs.Count;
                }
            }

            public DataNode()
            {
                Name = null;
                Parent = null;
                _data = null;
                _childs = new List<DataNode>();
                _tempList = new List<IDataNode>();
            }

            /// <summary>
            /// 创建新的数据结点
            /// </summary>
            /// <param name="name">数据结点名</param>
            /// <param name="parent">父节点</param>
            /// <returns>新创建的节点</returns>
            public static DataNode Create(string name, DataNode parent)
            {
                if (!IsValidName(name))
                {
                    throw new Exception("要创建的数据结点名称非法");
                }

                DataNode newNode = ReferencePool.Acquire<DataNode>();
                newNode.Name = name;
                newNode.Parent = parent;
                return newNode;
            }

            /// <summary>
            /// 根据类型获取数据结点的数据
            /// </summary>
            /// <typeparam name="T">要获取的数据类型</typeparam>
            /// <returns>指定类型的数据</returns>
            public T GetData<T>()
            {
                return (T)_data;
            }

            /// <summary>
            /// 获取数据结点的数据
            /// </summary>
            /// <returns>数据结点数据</returns>
            public object GetData()
            {
                return _data;
            }

            /// <summary>
            /// 设置数据结点的数据
            /// </summary>
            /// <typeparam name="T">要设置的数据类型</typeparam>
            /// <param name="data">要设置的数据</param>
            public void SetData<T>(T data)
            {
                _data = data;
            }

            /// <summary>
            /// 设置数据结点的数据
            /// </summary>
            /// <param name="data">要设置的数据</param>
            public void SetData(object data)
            {
                _data = data;
            }

            /// <summary>
            /// 根据名称检查是否存在子数据结点
            /// </summary>
            /// <param name="name">子数据结点名称</param>
            /// <returns>是否存在子数据结点</returns>
            public bool HasChild(string name)
            {
                return false;
            }

            /// <summary>
            /// 根据名称获取子数据结点
            /// </summary>
            /// <param name="name">子数据结点名称</param>
            /// <returns>指定名称的子数据结点，如果没有找到，则返回空</returns>
            public IDataNode GetChild(string name)
            {
                if (!IsValidName(name))
                {
                    throw new Exception("要获取的子数据结点名称非法");
                }

                foreach (DataNode node in _childs)
                {
                    if (node.Name == name)
                    {
                        return node;
                    }
                }

                return null;
            }

            /// <summary>
            /// 根据名称获取子数据结点, 如果对应名称的子数据结点不存在，则增加子数据结点
            /// </summary>
            /// <param name="name">子数据结点名称</param>
            /// <returns>指定名称的子数据结点</returns>
            public IDataNode GetOrAddChild(string name)
            {
                DataNode child = (DataNode)GetChild(name);
                if (child != null)
                {
                    return child;
                }

                child = Create(name, this);
                _childs.Add(child);

                return child;
            }

            /// <summary>
            /// 获取所有子数据结点
            /// </summary>
            /// <returns>所有子数据结点</returns>
            public List<IDataNode> GetAllChild()
            {
                _tempList.Clear();
                foreach (DataNode data in _childs)
                {
                    _tempList.Add(data);
                }

                return _tempList;
            }

            /// <summary>
            /// 根据名称移除子数据结点
            /// </summary>
            /// <param name="name">子数据结点名称。</param>
            public void RemoveChild(string name)
            {
                DataNode child = (DataNode)GetChild(name);
                if (child == null)
                {
                    return;
                }

                _childs.Remove(child);
                ReferencePool.Release(child);
            }

            /// <summary>
            /// 清理节点数据与所有子节点
            /// </summary>
            public void Clear()
            {
                _data = null;

                foreach (DataNode child in _childs)
                {
                    ReferencePool.Release(child);
                }
                _childs.Clear();
                _tempList.Clear();
            }

            /// <summary>
            /// 判断节点名称是否合法
            /// </summary>
            /// <param name="name">要判断的名称</param>
            /// <returns>是否合法</returns>
            private static bool IsValidName(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return false;
                }

                return true;
            }

            void IReference.Clear()
            {
                Parent = null;
                Name = null;

                Clear();
            }
        }
    }
}