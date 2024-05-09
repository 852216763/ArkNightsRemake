using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 框架事件数据基类
    /// </summary>
    public abstract class FrameworkEventBase : EventArgs, IReference
    {
        /// <summary>
        /// 清理数据
        /// </summary>
        public abstract void Clear();
    }
}