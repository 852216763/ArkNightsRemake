using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class TransformExtensions
    {
        /// <summary>
        /// 获取指定类型的组件,若不存在则添加
        /// </summary>
        /// <typeparam name="T">要获取的组件类型</typeparam>
        /// <returns>获取到的组件</returns>
        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
        {
            transform.TryGetComponent<T>(out T component);
            if (component == null)
            {
                component = transform.gameObject.AddComponent<T>();
            }
            return component;
        }
    }
}
