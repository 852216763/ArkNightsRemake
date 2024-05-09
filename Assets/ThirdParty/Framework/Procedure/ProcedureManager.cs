using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 流程管理器
    /// </summary>
    public sealed class ProcedureManager : ManagerBase
    {
        FsmManager _fsmManager;
        IFsm<ProcedureManager> _procedureFsm;

        internal override int Priority
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取当前流程
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                if (_procedureFsm == null)
                {
                    throw new Exception("流程状态机为空, 无法获取当前流程; \n需要初始化ProcedureManager");
                }

                return (ProcedureBase)_procedureFsm.CurrentState;
            }
        }

        /// <summary>
        /// 获取当前流程运行时间
        /// </summary>
        public float CurrentProcedureTime
        {
            get
            {
                if (_procedureFsm == null)
                {
                    throw new Exception("流程状态机为空, 无法获取当前流程运行时间; \n需要初始化ProcedureManager");
                }

                return _procedureFsm.CurrentStateTime;
            }
        }

        public ProcedureManager()
        {
            _fsmManager = FrameworkEntry.GetManager<FsmManager>();

            List<Type> procedureTypes = new List<Type>();
            Type entryProcedure = null;

            Type baseType = typeof(ProcedureBase);
            Assembly assembly = Assembly.Load("Assembly-CSharp");
            if (assembly == null)
            {
                Debugger.LogError("初始化流程管理器失败,没有获取到CSharp程序集");
            }
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type))
                {
                    procedureTypes.Add(type);
                    if ((Activator.CreateInstance(type) as ProcedureBase).IsEntryProcedure)
                    {
                        entryProcedure = type;
                    }
                }
            }
            _procedureFsm = _fsmManager.CreateFsm<ProcedureManager>(this, procedureTypes.ToArray());
            _procedureFsm.Start(entryProcedure);
        }

        internal override void ShutDown()
        {
            if (_fsmManager != null)
            {
                if (_procedureFsm != null)
                {
                    _fsmManager.DestroyFsm<ProcedureManager>(_procedureFsm);
                    _procedureFsm = null;
                }

                _fsmManager = null;
            }
        }

        /// <summary>
        /// 切换至指定流程
        /// </summary>
        /// <typeparam name="T">切换的目标流程类型</typeparam>
        public void ChangeProcedure<T>() where T : ProcedureBase
        {
            _procedureFsm.ChangeState<T>();
        }

        /// <summary>
        /// 切换至指定流程
        /// </summary>
        /// <param name="type">切换的目标流程类型</param>
        public void ChangeProcedure(Type type)
        {
            if (type == null)
            {
                throw new Exception("没有指定要获得的流程");
            }

            if (!typeof(ProcedureBase).IsAssignableFrom(type))
            {
                throw new Exception("要获得的指定类型不是一个流程类型");
            }

            _procedureFsm.ChangeState(type);
        }

        /// <summary>
        /// 获取指定流程
        /// </summary>
        /// <typeparam name="T">要获取的流程类型</typeparam>
        /// <returns>获取到的流程</returns>
        public T GetState<T>() where T : ProcedureBase
        {
            return _procedureFsm.GetState<T>();
        }

        /// <summary>
        /// 获取指定流程
        /// </summary>
        /// <param name="type">要获取的流程类型</param>
        /// <returns>获取到的流程</returns>
        public ProcedureBase GetState(Type type)
        {
            if (type == null)
            {
                throw new Exception("没有指定要获得的流程");
            }

            if (!typeof(ProcedureBase).IsAssignableFrom(type))
            {
                throw new Exception("要获得的指定类型不是一个流程类型");
            }

            return (ProcedureBase)_procedureFsm.GetState(type);
        }

    }
}

