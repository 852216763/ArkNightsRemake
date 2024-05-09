using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public partial class ObjectPoolManager : ManagerBase
    {
        /// <summary>
        /// 对象池
        /// </summary>
        private class ObjectPool<T> : ObjectPoolBase, IObjectPool<T> where T : ObjectBase
        {
            Dictionary<string, LinkedList<Object<T>>> _UnusedObjects;
            Dictionary<T, Object<T>> _ObjectsMap;

            string _name;
            int _capacity;
            float _expireTime_sec;
            float _autoReleaseInterval;

            float timer;

            public override Type ObjectType
            {
                get
                {
                    return typeof(T);
                }
            }

            public override string Name
            {
                get
                {
                    return _name;
                }
            }

            public override string FullName
            {
                get
                {
                    return Name + ObjectType.FullName;
                }
            }

            public override int CanSpawnCount
            {
                get
                {
                    int count = 0;
                    foreach (KeyValuePair<string, LinkedList<Object<T>>> item in _UnusedObjects)
                    {
                        count += item.Value.Count;
                    }
                    return count;
                }
            }

            public override int CanReleaseCount
            {
                get
                {
                    return GetCanReleaseObjects().Count;
                }
            }

            public override int Capacity
            {
                set
                {
                    _capacity = value;
                }
                get
                {
                    return _capacity;
                }
            }

            public override float ExpireTime_Sec
            {
                set
                {
                    _expireTime_sec = value;
                }
                get
                {
                    return _expireTime_sec;
                }
            }

            public override float AutoReleaseInterval
            {
                set
                {
                    _autoReleaseInterval = value;
                }
                get
                {
                    return _autoReleaseInterval;
                }
            }

            public ObjectPool(string name = "", int capacity = 20, float expireTime = 120, float autoReleaseInterval = 120)
            {
                _UnusedObjects = new Dictionary<string, LinkedList<Object<T>>>();
                _ObjectsMap = new Dictionary<T, Object<T>>();

                _name = name;
                _capacity = capacity;
                _expireTime_sec = expireTime;
                _autoReleaseInterval = autoReleaseInterval;

                timer = 0;
            }

            public bool CanSpawn(string name)
            {
                return _UnusedObjects.ContainsKey(name) && _UnusedObjects[name].Count > 0;
            }

            public void Register(T obj, bool isSpawned = true)
            {
                if (obj == null)
                {
                    Debugger.LogError($"要往对象池 {Name} 注册的对象为空!");
                    return;
                }

                if (!_UnusedObjects.ContainsKey(obj.Name))
                {
                    _UnusedObjects.Add(obj.Name, new LinkedList<Object<T>>());
                }

                Object<T> objT = Object<T>.Create(obj, isSpawned);
                _ObjectsMap.Add(obj, objT);
                if (!isSpawned)
                {
                    _UnusedObjects[obj.Name].AddFirst(objT);
                }
            }

            public void Release(T obj)
            {
                if (!_ObjectsMap.ContainsKey(obj))
                {
                    return;
                }

                Object<T> objT = _ObjectsMap[obj];
                _ObjectsMap.Remove(obj);
                if(!objT.IsInUse)
                {
                    _UnusedObjects[objT.Name].Remove(objT);
                }
                objT.Release();
            }

            public override void Release()
            {
                Object<T>[] objs = GetCanReleaseObjects().ToArray();
                for (int i = 0; i < objs.Length; i++)
                {
                    Object<T> objT = objs[i];
                    _ObjectsMap.Remove(objT.Peek());
                    _UnusedObjects[objT.Name].Remove(objT);
                    objT.Release();
                }
            }

            public override void ReleaseAllUnused(bool isShutDown = false)
            {
                foreach (KeyValuePair<string, LinkedList<Object<T>>> item in _UnusedObjects)
                {
                    foreach (Object<T> objT in item.Value)
                    {
                        _ObjectsMap.Remove(objT.Peek());
                        objT.Release(isShutDown);
                    }
                    item.Value.Clear();
                }
            }

            public T Spawn(string name)
            {
                if (!CanSpawn(name))
                {
                    return null;
                }

                Object<T> obj = _UnusedObjects[name].First.Value;
                _UnusedObjects[name].RemoveFirst();
                return obj.Spawn();
            }

            public void Unspawn(T obj)
            {
                if (obj == null)
                {
                    Debugger.LogError("要向对象池 {Name} 中回收的对象为空!");
                    return;
                }

                if (!_ObjectsMap.ContainsKey(obj))
                {
                    Register(obj, false);
                }

                _ObjectsMap[obj].UnSpawn();
                _UnusedObjects[obj.Name].AddFirst(_ObjectsMap[obj]);
            }

            private List<Object<T>> GetCanReleaseObjects()
            {
                List<Object<T>> objects = new List<Object<T>>();
                foreach (KeyValuePair<string, LinkedList<Object<T>>> item in _UnusedObjects)
                {
                    LinkedListNode<Object<T>> node = item.Value.Last;
                    // 对象池 获取和归还 均优先操作链表头部,因此Unused链表按最后使用时间由早到晚排序
                    while (node != null)
                    {
                        if ((DateTime.Now - node.Value.LastUseTime).Seconds >= ExpireTime_Sec)
                        {
                            objects.Add(node.Value);
                        }
                        else
                        {
                            break;
                        }
                        node = node.Previous;
                    }
                }

                return objects;
            }

            internal override void Update(float deltaTime, float realDeltaTime)
            {
                timer += realDeltaTime;
                if (timer > AutoReleaseInterval)
                {
                    Release();
                    timer = 0;
                }
            }

            internal override void ShutDown()
            {
                ReleaseAllUnused(true);
                _ObjectsMap = null;
                _UnusedObjects = null;
            }
        }
    }


}
