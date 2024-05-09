using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Mono单例模板
    /// </summary>
    /// <typeparam name="T">实例类型</typeparam>
    public abstract class MonoSingletonScript<T> : MonoBehaviour where T : MonoSingletonScript<T>
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    T[] objects = FindObjectsOfType<T>();
                    if(objects.Length == 0)
                    {
                        Debugger.LogError("未检测到单例对象: " + typeof(T).ToString());
                        return null;
                    }

                    _instance = objects[0];
                    if (objects.Length > 1)
                    {
                        Debugger.LogError("检测到多个单例对象: " + typeof(T).ToString());
                        return _instance;
                    }
                }

                return _instance;
            }
        }

        void OnDestroy()
        {
            _instance = null;
        }
    }
}
