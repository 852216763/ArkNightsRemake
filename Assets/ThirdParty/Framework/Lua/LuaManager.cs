using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Framework
{
    public sealed class LuaManager : ManagerBase
    {
        LuaEnv _env;

        public LuaEnv Env
        {
            get
            {
                return _env;
            }
        }

        public LuaManager()
        {
            _env = new LuaEnv();
            _env.AddLoader(Loader);
        }

        internal override void Update(float deltaTime, float realDeltaTime)
        {
            base.Update(deltaTime, realDeltaTime);
            _env.Tick();
        }

        internal override void ShutDown()
        {
            // 记得清除注册的事件委托
            // ...

            _env.Dispose();
        }



        /// <summary>
        /// 自定义的Lua脚本加载器
        /// </summary>
        /// <param name="path">脚本路径</param>
        /// <returns>读取到的脚本字节流</returns>
        private byte[] Loader(ref string path)
        {
            // 通过各种途径读取到lua内容(从网络请求,本地读取等)
            string luaContent = null;

            // TODO 实现Lua脚本加载

            // 返回lua代码转化的字节流
            return System.Text.Encoding.UTF8.GetBytes(luaContent);
        }
    }

}
