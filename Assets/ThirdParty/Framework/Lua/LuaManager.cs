using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

namespace Framework
{
    public sealed class LuaManager : ManagerBase
    {
        private static string luaScriptPath = "Assets/LuaScript/";
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

            DoLuaFile("Global");
            // 加载热更新脚本
        }

        internal override void Update(float deltaTime, float realDeltaTime)
        {
            base.Update(deltaTime, realDeltaTime);
            _env.Tick();
        }

        internal override void ShutDown()
        {
            // 记得清除注册的热更新事件委托
            // ...
            _env.Dispose();
        }

        /// <summary>
        /// 执行指定路径的Lua文件
        /// </summary>
        /// <param name="path">要加执行的lua文件相对路径,不需要包含后缀</param>
        /// <param name="chunkName">代码块名称</param>
        /// <param name="envTbl">环境表</param>
        /// <returns>导入模块的返回值</returns>
        public object DoLuaFile(string path, string chunkName = "chunk", LuaTable envTbl = null)
        {
            return _env.DoString(LoadLuaString(path), chunkName, envTbl);
        }

        /// <summary>
        /// 自定义的Lua脚本加载器
        /// </summary>
        /// <param name="path">脚本相对路径</param>
        /// <returns>读取到的脚本字节流</returns>
        private byte[] Loader(ref string path)
        {
            // 通过各种途径读取到lua内容(从网络请求,本地读取等)
            string luaContent = LoadLuaString(path);

            // 返回lua代码转化的字节流
            return System.Text.Encoding.UTF8.GetBytes(luaContent);
        }

        private string LoadLuaString(string path)
        {
            string fullPath = Path.Combine(luaScriptPath, path);
            if (!fullPath.EndsWith(".lua.txt"))
            {
                if (fullPath.EndsWith(".lua"))
                    fullPath += ".txt";
                else
                    fullPath += ".lua.txt";
            }
            TextAsset textAsset = FrameworkEntry.Resource.LoadAsset<TextAsset>(fullPath);

            return textAsset.text;
        }
    }

}
