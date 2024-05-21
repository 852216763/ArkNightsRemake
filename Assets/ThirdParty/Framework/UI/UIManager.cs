/*
 * 临时写的UI管理器,凑合着用
 * 复杂的组管理,UI深度,遮挡覆盖,以及暂停等功能需要再拓展
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class UIManager : ManagerBase
    {
        ResourceManager _resource;
        Transform _uiManagerRoot;
        Camera _uiCamera;
        Transform _uiRoot;

        Dictionary<string, GameObject> _uiMap;
        LinkedList<GameObject> _activeUIList;
        LinkedList<UIForm> _uiLogicList;

        /// <summary>
        /// 当前激活的UI数
        /// </summary>
        public int ActiveUICount
        {
            get
            {
                return _activeUIList.Count;
            }
        }

        /// <summary>
        /// 顶部UI对象,没有显示UI则返回null
        /// </summary>
        public GameObject TopUI
        {
            get
            {
                if (_activeUIList.Count > 0)
                {
                    return _activeUIList.First.Value;
                }
                return null;
            }
        }

        /// <summary>
        /// UI相机
        /// </summary>
        public Camera UICamera
        {
            get
            {
                return _uiCamera;
            }
        }

        internal override void Update(float deltaTime, float realDeltaTime)
        {
            base.Update(deltaTime, realDeltaTime);
            foreach (UIForm form in _uiLogicList)
            {
                form.OnUpdate();
            }
        }

        internal override void ShutDown()
        {
        }

        public UIManager()
        {
            _resource = FrameworkEntry.Resource;
            _uiMap = new Dictionary<string, GameObject>();
            _activeUIList = new LinkedList<GameObject>();
            _uiLogicList = new LinkedList<UIForm>();

            GameObject root = FrameworkEntry.Instance.gameObject;
            _uiManagerRoot = root.transform.Find("UIManagerRoot");
            if (_uiManagerRoot == null)
            {
                throw new System.Exception("没有找到UI根节点!");
            }

            _uiCamera = _uiManagerRoot.GetComponentInChildren<Camera>();
            if (_uiCamera == null)
            {
                throw new System.Exception("没有找到UI相机!");
            }

            _uiRoot = _uiManagerRoot.Find("UIRoot");
            if (_uiRoot == null)
            {
                throw new System.Exception("没有找到UI画布!");
            }

        }

        /// <summary>
        /// 显示UI至最上层
        /// </summary>
        /// <param name="uiAssetPath">UI资源路径</param>
        /// <param name="userdata">用户数据</param>
        /// <returns></returns>
        public GameObject ShowUI(string uiAssetPath, object userdata = null)
        {
            if (string.IsNullOrEmpty(uiAssetPath))
            {
                Debugger.LogError($"要显示的UI路径为空!");
                return null;
            }

            GameObject uiObj;
            UIForm uiLogic;
            if (_uiMap.ContainsKey(uiAssetPath))
            {
                uiObj = _uiMap[uiAssetPath];
                if (_activeUIList.First.Value == uiObj)
                {
                    Debugger.Log($"要显示的UI {uiAssetPath} 已经在最上层了");
                    return uiObj;
                }
                if (_activeUIList.Contains(uiObj))
                {
                    _activeUIList.Remove(uiObj);
                }
            }
            else
            {
                uiObj = _resource.LoadGameObject(uiAssetPath, _uiRoot);
                if (uiObj == null)
                {
                    Debugger.LogError($"加载UI资源 {uiAssetPath} 失败");
                    return null;
                }

                _uiMap.Add(uiAssetPath, uiObj);

                uiObj.transform.TryGetComponent<UIForm>(out uiLogic);
                if (uiLogic != null)
                {
                    uiLogic.OnInit();
                }
            }

            uiObj.SetActive(true);
            uiObj.transform.SetAsLastSibling();
            _activeUIList.AddFirst(uiObj);
            uiObj.transform.TryGetComponent<UIForm>(out uiLogic);
            if (uiLogic != null)
            {
                _uiLogicList.AddFirst(uiLogic);
                uiLogic.OnShow(userdata);
            }
            return uiObj;
        }

        /// <summary>
        /// 隐藏指定路径的UI
        /// </summary>
        /// <param name="uiAssetPath">UI资源路径</param>
        /// <param name="userdata">用户数据</param>
        public void HideUI(string uiAssetPath, object userdata = null)
        {
            if (string.IsNullOrEmpty(uiAssetPath))
            {
                Debugger.LogError($"要隐藏的UI路径为空!");
                return;
            }

            GameObject uiObj;
            if (!_uiMap.ContainsKey(uiAssetPath))
            {
                Debugger.LogWarning($"想要隐藏的UI {uiAssetPath} 没有加载过");
            }
            uiObj = _uiMap[uiAssetPath];
            HideUI(uiObj, userdata);
        }

        /// <summary>
        /// 隐藏指定UI对象
        /// </summary>
        /// <param name="uiObj">UI对象</param>
        /// <param name="userdata">用户数据</param>
        public void HideUI(GameObject uiObj, object userdata = null)
        {
            if (uiObj == null || !_activeUIList.Contains(uiObj))
            {
                Debugger.LogWarning($"想要隐藏的UI {uiObj?.name} 没有显示");
            }
            uiObj.SetActive(false);
            _activeUIList.Remove(uiObj);

            uiObj.transform.TryGetComponent<UIForm>(out UIForm uiLogic);
            if (uiLogic != null)
            {
                uiLogic.OnHide(userdata);
                _uiLogicList.Remove(uiLogic);
            }
        }

        /// <summary>
        /// 关闭指定路径的UI
        /// </summary>
        /// <param name="uiAssetPath">UI资源路径</param>
        /// <param name="userdata">用户数据</param>
        public void CloseUI(string uiAssetPath, object userdata = null)
        {
            if (string.IsNullOrEmpty(uiAssetPath))
            {
                Debugger.LogError($"要关闭的UI路径为空!");
                return;
            }

            _uiMap.TryGetValue(uiAssetPath, out GameObject uiObj);
            CloseUI(uiObj, userdata);
        }

        /// <summary>
        /// 关闭指定UI对象
        /// </summary>
        /// <param name="uiObj">UI对象</param>
        /// <param name="userdata">用户数据</param>
        public void CloseUI(GameObject uiObj, object userdata = null)
        {
            if (uiObj == null || !_uiMap.ContainsValue(uiObj))
            {
                Debugger.LogError($"想要关闭的UI对象为空!");
            }

            foreach (KeyValuePair<string, GameObject> item in _uiMap)
            {
                if (item.Value == uiObj)
                {
                    _activeUIList.Remove(uiObj);
                    _uiMap.Remove(item.Key);

                    uiObj.transform.TryGetComponent<UIForm>(out UIForm uiLogic);
                    if (uiLogic != null)
                    {
                        if (_activeUIList.Contains(uiObj))
                        {
                            uiLogic.OnHide();
                        }
                        uiLogic.OnClose(userdata);
                        _uiLogicList.Remove(uiLogic);
                    }

                    _resource.ReleaseGameObject(uiObj);
                    return;
                }
            }

            Debugger.LogError($"想要关闭的UI对象 {uiObj.name} 没有加载过");
        }

        /// <summary>
        /// 将指定资源路径的UI界面显示在最前
        /// </summary>
        /// <param name="uiAssetPath">UI资源路径</param>
        public void MoveUIToViewTop(string uiAssetPath)
        {
            if (string.IsNullOrEmpty(uiAssetPath))
            {
                Debugger.LogError($"要置顶的UI路径为空!");
                return;
            }
            if (!_uiMap.ContainsKey(uiAssetPath))
            {
                Debugger.LogError($"想要置顶的UI {uiAssetPath} 没有加载过");
                return;
            }

            GameObject uiObj = _uiMap[uiAssetPath];
            MoveUIToViewTop(uiObj);
        }

        /// <summary>
        /// 将的UI界面显示在最前
        /// </summary>
        /// <param name="uiObj">UI对象</param>
        public void MoveUIToViewTop(GameObject uiObj)
        {
            if (uiObj == null || !_activeUIList.Contains(uiObj))
            {
                Debugger.LogError($"想要置顶的对象 {uiObj?.name} 没有显示");
            }

            uiObj.transform.SetAsLastSibling();
            // 若有UI逻辑脚本,将其遍历顺序移至首位
            uiObj.TryGetComponent<UIForm>(out UIForm logic);
            if (logic != null && _uiLogicList.First.Value != logic)
            {
                _uiLogicList.Remove(logic);
                _uiLogicList.AddFirst(logic);
            }
        }
    }

}
