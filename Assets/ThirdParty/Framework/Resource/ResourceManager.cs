using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework
{
    // TODO 完善资源加载模块
    public sealed class ResourceManager : ManagerBase
    {
        internal override int Priority
        {
            get
            {
                return 0;
            }
        }

        internal override void Update(float deltaTime, float realDeltaTime)
        {
            base.Update(deltaTime, realDeltaTime);
        }

        internal override void ShutDown()
        {
        }

        public ResourceManager()
        {

        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T">要加载的资源类型</typeparam>
        /// <param name="assetPath">资源地址</param>
        /// <returns>资源</returns>
        public T LoadAsset<T>(string assetPath) where T : class
        {
            AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetPath);
            T result = handler.WaitForCompletion();
            if (result == null)
            {
                Addressables.Release(handler);
                Debugger.LogError($"要加载的资源 {assetPath} 不存在或加载失败");
            }

            return result;
        }

        /// <summary>
        /// 加载游戏对象实例
        /// </summary>
        /// <param name="assetPath">对象资源路径</param>
        /// <returns>创建的实例</returns>
        public GameObject LoadGameObject(string assetPath)
        {
            AsyncOperationHandle<GameObject> handler = Addressables.InstantiateAsync(assetPath);
            GameObject result = handler.WaitForCompletion();
            if (result == null)
            {
                Addressables.Release(handler);
                Debugger.LogError($"要加载的资源 {assetPath} 不存在或加载失败");
            }

            return result;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">要加载的资源类型</typeparam>
        /// <param name="assetPath">资源地址</param>
        /// <returns>资源</returns>
        public async Task<T> LoadAssetAsync<T>(string assetPath) where T : class
        {
            AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetPath);
            await handler.Task;
            if (handler.Status == AsyncOperationStatus.Succeeded)
            {
                return handler.Result;
            }
            else
            {
                Addressables.Release(handler);
                Debugger.LogError($"加载资源 {assetPath} 失败");
                return null;
            }
        }

        /// <summary>
        /// 异步加载多个资源
        /// </summary>
        /// <typeparam name="T">要加载的资源类型</typeparam>
        /// <param name="assetPaths">资源地址列表</param>
        /// <returns>符合条件的资源</returns>
        public async Task<List<T>> LoadAssetsAsync<T>(List<string> assetPaths) where T : class
        {
            List<T> result = new List<T>();
            foreach (string assetPath in assetPaths)
            {
                AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetPath);
                await handler.Task;
                if (handler.Status == AsyncOperationStatus.Succeeded)
                {
                    result.Add(handler.Result);
                }
                else
                {
                    Addressables.Release(handler);
                    ReleaseAssets<T>(result);
                    Debugger.LogError($"加载资源集失败, 失败的资源为 {assetPath}");
                    return null;
                }
            }
            return result;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="T">要释放的资源类型</typeparam>
        /// <param name="asset">要释放的资源</param>
        public void ReleaseAsset<T>(T asset)
        {
            if (asset == null)
            {
                Debugger.LogError("要释放的资源为空!");
                return;
            }

            Addressables.Release<T>(asset);
        }

        /// <summary>
        /// 释放多个资源
        /// </summary>
        /// <typeparam name="T">要释放的资源类型</typeparam>
        /// <param name="assets">要释放的资源列表</param>
        public void  ReleaseAssets<T>(List<T> assets)
        {
            foreach (T item in assets)
            {
                ReleaseAsset<T>(item);
            }
        }

        /// <summary>
        /// 释放游戏对象实例
        /// </summary>
        /// <param name="obj">要释放的实例</param>
        public void ReleaseGameObject(GameObject obj)
        {
            Addressables.ReleaseInstance(obj);
        }

    }

}