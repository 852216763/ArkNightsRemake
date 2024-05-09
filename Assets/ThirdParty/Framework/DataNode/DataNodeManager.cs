using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 数据结点管理器
    /// 在运行时进行数据传递与存储
    /// </summary>
    public sealed partial class DataNodeManager : ManagerBase
    {
        private static readonly string[] EmptyStringArray = new string[] { };
        private static readonly string[] PathSplitSeparator = new string[] { ".", "/", "\\" };
        private const string _rootName = "DataNodeRoot";
        private DataNode _root;

        public IDataNode RootNode
        {
            get
            {
                return _root;
            }
        }

        public DataNodeManager()
        {
            _root = DataNode.Create(_rootName, null);
        }

        internal override void ShutDown()
        {
            ReferencePool.Release(_root);
            _root = null;
        }

        /// <summary>
        /// 根据类型获取数据结点的数据。
        /// </summary>
        /// <typeparam name="T">要获取的数据类型。</typeparam>
        /// <param name="path">相对于 relativeNode 的查找路径。默认为RootNode</param>
        /// <param name="relativeNode">查找起始结点。</param>
        /// <returns>指定类型的数据。</returns>
        public T GetData<T>(string path, IDataNode relativeNode = null)
        {
            IDataNode node = GetNode(path, relativeNode);
            if (node == null)
            {
                throw new Exception("要获取数据的结点不存在");
            }

            return node.GetData<T>();
        }

        /// <summary>
        /// 获取数据结点的数据
        /// </summary>
        /// <param name="path">相对于 relativeNode 的查找路径。默认为RootNode</param>
        /// <param name="relativeNode">查找起始结点</param>
        /// <returns>数据结点的数据</returns>
        public object GetData(string path, IDataNode relativeNode = null)
        {
            IDataNode node = GetNode(path, relativeNode);
            if (node == null)
            {
                throw new Exception("要获取数据的结点不存在");
            }

            return node.GetData();
        }

        /// <summary>
        /// 根据类型设置数据结点的数据
        /// </summary>
        /// <typeparam name="T">要设置的数据类型</typeparam>
        /// <param name="path">相对于relativeNode的路径。默认为RootNode</param>
        /// <param name="data">要设置的数据</param>
        /// <param name="relativeNode">查找起始结点</param>
        public void SetData<T>(string path, T data, IDataNode relativeNode = null)
        {
            IDataNode node = GetOrAddNode(path, relativeNode);
            node.SetData<T>(data);
        }

        /// <summary>
        /// 设置数据结点的数据
        /// </summary>
        /// <param name="path">相对于relativeNode的路径。默认为RootNode</param>
        /// <param name="data">要设置的数据</param>
        /// <param name="relativeNode">查找起始结点</param>
        public void SetData(string path, object data, IDataNode relativeNode = null)
        {
            IDataNode node = GetOrAddNode(path, relativeNode);
            node.SetData(data);
        }

        /// <summary>
        /// 获取数据结点。
        /// </summary>
        /// <param name="path">相对于 relativeNode 的查找路径。默认为RootNode</param>
        /// <param name="relativeNode">查找起始结点。默认为RootNode</param>
        /// <returns>指定位置的数据结点，如果没有找到，则返回空。</returns>
        public IDataNode GetNode(string path, IDataNode relativeNode = null)
        {
            if (relativeNode == null)
            {
                relativeNode = RootNode;
            }

            foreach (string splitStr in GetSplitPath(path))
            {
                relativeNode = relativeNode.GetChild(splitStr);
                if (relativeNode == null)
                {
                    return null;
                }
            }

            return relativeNode;
        }

        /// <summary>
        /// 获取数据结点, 如果没有找到,则增加相应的数据结点。
        /// </summary>
        /// <param name="path">相对于 relativeNode 的查找路径。</param>
        /// <param name="relativeNode">查找起始结点。默认为RootNode</param>
        /// <returns>指定位置的数据结点</returns>
        public IDataNode GetOrAddNode(string path, IDataNode relativeNode = null)
        {
            if (relativeNode == null)
            {
                relativeNode = RootNode;
            }

            foreach (string splitStr in GetSplitPath(path))
            {
                relativeNode = relativeNode.GetOrAddChild(splitStr);
            }

            return relativeNode;
        }

        /// <summary>
        /// 移除数据结点。
        /// </summary>
        /// <param name="path">相对于 relativeNode 的查找路径。默认为RootNode</param>
        /// <param name="relativeNode">查找起始结点。</param>
        public void RemoveNode(string path, IDataNode relativeNode = null)
        {
            IDataNode node = GetNode(path, relativeNode);
            if (node == null)
            {
                return;
            }

            IDataNode parent = node.Parent;
            if (parent != null)
            {
                parent.RemoveChild(node.Name);
            }
        }

        /// <summary>
        /// 清空所有数据与结点
        /// </summary>
        public void ClearAllDataNode()
        {
            _root.Clear();
        }

        /// <summary>
        /// 数据结点路径切分工具函数。
        /// </summary>
        /// <param name="path">要切分的数据结点路径。</param>
        /// <returns>切分后的字符串数组。</returns>
        private static string[] GetSplitPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return EmptyStringArray;
            }

            return path.Split(PathSplitSeparator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
