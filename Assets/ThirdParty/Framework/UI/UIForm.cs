using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class UIForm : MonoBehaviour
    {
        #region UI生命周期

        /// <summary>
        /// 新建UI时调用
        /// </summary>
        /// <param name="userdata">用户数据</param>
        internal protected virtual void OnInit(object userdata = null)
        {

        }

        /// <summary>
        /// 激活UI时调用
        /// </summary>
        /// <param name="userdata">用户数据</param>
        internal protected virtual void OnShow(object userdata = null)
        {

        }

        /// <summary>
        /// 由UIManager每帧调用
        /// </summary>
        internal protected virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 隐藏UI时调用
        /// </summary>
        /// <param name="userdata">用户数据</param>
        internal protected virtual void OnHide(object userdata = null)
        {

        }

        /// <summary>
        /// 关闭(销毁)UI时调用
        /// </summary>
        /// <param name="userdata">用户数据</param>
        internal protected virtual void OnClose(object userdata = null)
        {

        }

        // OnCover OnPause等周期用得着再扩展
        #endregion
    }

}
