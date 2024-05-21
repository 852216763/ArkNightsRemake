using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using XLua;

namespace Framework
{
    public class LuaUIForm : UIForm
    {
        public string luaScriptPath;

        private LuaTable _scriptEnv;
        private Action<object> _luaInit;
        private Action<object> _luaShow;
        private Action _luaUpdate;
        private Action<object> _luaHide;
        private Action<object> _luaClose;

        #region UI生命周期

        /// <summary>
        /// 新建UI时调用
        /// </summary>
        /// <param name="userdata">用户数据</param>
        protected internal override void OnInit(object userdata = null)
        {
            base.OnInit(userdata);
            LuaEnv env = FrameworkEntry.Lua.Env;
            // 创建一个环境表
            _scriptEnv = env.NewTable();
            LuaTable meta = env.NewTable();
            meta.Set("__index", env.Global);
            _scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            _scriptEnv.Set("self", this);
            FrameworkEntry.Lua.DoLuaFile(luaScriptPath, "chunk", _scriptEnv);

            _scriptEnv.Get("OnInit", out _luaInit);
            _scriptEnv.Get("OnShow", out _luaShow);
            _scriptEnv.Get("OnUpdate", out _luaUpdate);
            _scriptEnv.Get("OnHide", out _luaHide);
            _scriptEnv.Get("OnClose", out _luaClose);

            _luaInit?.Invoke(userdata);
        }

        protected internal override void OnShow(object userdata = null)
        {
            base.OnShow(userdata);
            _luaShow?.Invoke(userdata);
        }

        protected internal override void OnUpdate()
        {
            base.OnUpdate();
            _luaUpdate?.Invoke();
        }

        protected internal override void OnHide(object userdata = null)
        {
            base.OnHide(userdata);
            _luaHide?.Invoke(userdata);
        }

        protected internal override void OnClose(object userdata = null)
        {
            base.OnClose(userdata);
            _luaClose?.Invoke(userdata);

            _luaInit = null;
            _luaShow = null;
            _luaUpdate = null;
            _luaHide = null;
            _luaClose = null;

            _scriptEnv.Dispose();
        }

        // OnCover OnPause等周期用得着再扩展
        #endregion
    }

}
