/*
 * 本想设计一个可以灵活切换具体实现(原生或WWise,FMod等中间件)的管理器
 * Manager持有一个Helper,不同插件的音频操作实现在继承HelperBase的子类中
 * 想要切换音频插件只需要更换一个Helper类型
 * 无奈功力着实有限,且对WWise等常用中间件完全不熟悉
 * 只能凑合凑合了
 */

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework
{
    /// <summary>
    /// 音频管理器
    /// </summary>
    public sealed partial class SoundManager : ManagerBase
    {
        ResourceManager _resourceManager;
        Transform _soundManagerRoot;
        AudioMixer _mainMixer;

        Dictionary<string, SoundGroup> _soundGroupMap;
        uint _serialId;

        public SoundManager()
        {
            _resourceManager = FrameworkEntry.Resource;

            GameObject root = FrameworkEntry.Instance.gameObject;
            _soundManagerRoot = new GameObject("SoundManagerRoot").transform;
            _soundManagerRoot.SetParent(root.transform);

            _mainMixer = _resourceManager.LoadAsset<AudioMixer>("mainMixer");

            _soundGroupMap = new Dictionary<string, SoundGroup>();
            _serialId = 0;
        }

        internal override void Update(float deltaTime, float realDeltaTime)
        {
            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                item.Value.Update(deltaTime, realDeltaTime);
            }
        }

        internal override void ShutDown()
        {
            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                item.Value.ShutDown();
            }
            _resourceManager = null;
            _soundGroupMap = null;
            _serialId = 0;
            _mainMixer = null;
            GameObject.Destroy(_soundManagerRoot);
        }

        #region 音频组相关

        /// <summary>
        /// 查询是否存在指定名称的音频组
        /// </summary>
        /// <param name="groupName">音频组名</param>
        /// <returns>是否存在指定名称的音频组</returns>
        public bool HasSoundGroup(string groupName)
        {
            return _soundGroupMap.ContainsKey(groupName);
        }

        /// <summary>
        /// 添加音频组
        /// </summary>
        /// <param name="groupName">音频组名称</param>
        /// <returns>音频组</returns>
        public SoundGroup AddSoundGroup(string groupName)
        {
            SoundGroup sg = GetSoundGroup(groupName);
            if (sg == null)
            {
                GameObject groupRoot = new GameObject(groupName);
                groupRoot.transform.SetParent(_soundManagerRoot);

                sg = new SoundGroup(groupRoot, groupName);
                _soundGroupMap.Add(groupName, sg);

                // 尝试获取默认(与音频组同名的)混音组
                if (_mainMixer != null)
                {
                    AudioMixerGroup[] groups = _mainMixer.FindMatchingGroups(groupName);
                    if (groups.Length > 0)
                    {
                        sg.AudioMixerGroup = groups[0];
                    }
                }
            }

            return sg;
        }

        /// <summary>
        /// 移除音频组
        /// </summary>
        /// <param name="groupName">音频组名称</param>
        public void RemoveSoundGroup(string groupName)
        {
            if (!HasSoundGroup(groupName))
            {
                return;
            }

            SoundGroup sg = _soundGroupMap[groupName];
            sg.ShutDown();
            _soundGroupMap.Remove(groupName);
        }

        /// <summary>
        /// 获取指定名称的音频组
        /// </summary>
        /// <param name="groupName">音频组名</param>
        /// <returns>音频组,不存在则返回null</returns>
        public SoundGroup GetSoundGroup(string groupName)
        {
            if (_soundGroupMap.ContainsKey(groupName))
            {
                return _soundGroupMap[groupName];
            }

            return null;
        }

        /// <summary>
        /// 获取指定id的音频所在音频组的名称
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <returns>音频组名称,不存在则返回null</returns>
        public string GetSoundGroupName(uint id)
        {
            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                if (item.Value.HasSound(id))
                {
                    return item.Key;
                }
            }

            return null;
        }

        #endregion

        #region 混音器相关

        /// <summary>
        /// 设置混音器暴露的属性值
        /// </summary>
        /// <param name="propName">属性名</param>
        /// <param name="value">值</param>
        /// <returns>是否设置成功</returns>
        public bool SetExposedPropValue(string propName, float value)
        {
            return _mainMixer.SetFloat(propName, value);
        }

        /// <summary>
        /// 获取混音器暴露的属性值
        /// </summary>
        /// <param name="propName">属性名</param>
        /// <returns>属性值</returns>
        public float GetExposedPropValue(string propName)
        {
            _mainMixer.GetFloat(propName, out float val);
            return val;
        }

        /// <summary>
        /// 重置混音器暴露的属性为默认值
        /// </summary>
        /// <param name="propName">属性名</param>
        public void ResetExposedPropValue(string propName)
        {
            _mainMixer.ClearFloat(propName);
        }

        /// <summary>
        /// 切换至指定名称的音频快照
        /// </summary>
        /// <param name="snapshotName">快照名</param>
        /// <param name="reachTime">切换时间</param>
        public void TransitionSnapshot(string snapshotName, float reachTime = 0f)
        {
            AudioMixerSnapshot snapshot = _mainMixer.FindSnapshot(snapshotName);
            if (snapshot == null)
            {
                Debugger.LogWarning("要切换的音频快照不存在!");
                return;
            }

            snapshot.TransitionTo(reachTime);
        }

        /// <summary>
        /// 切换至依据权重的多个快照混合状态
        /// </summary>
        /// <param name="snapshotNames">全部混合的快照名</param>
        /// <param name="weights">快照权重</param>
        /// <param name="reachTime">切换时间</param>
        public void TransitionMixedSnapshots(string[] snapshotNames, float[] weights, float reachTime)
        {
            List<AudioMixerSnapshot> snapshots = new List<AudioMixerSnapshot>();
            foreach (string snapName in snapshotNames)
            {
                snapshots.Add(_mainMixer.FindSnapshot(snapName));
            }

            if (snapshots.Count == 0)
            {
                Debugger.LogError("混合切换快照时,指定的快照不存在!");
                return;
            }
            if (snapshots.Count != weights.Length)
            {
                Debugger.LogWarning("混合切换快照时,指定的快照数与权重数不匹配");
            }
            _mainMixer.TransitionToSnapshots(snapshots.ToArray(), weights, reachTime);
        }

        /// <summary>
        /// 声音滑动条数值映射为分贝值
        /// </summary>
        /// <param name="x">滑动条值[0,1]</param>
        /// <returns>db值[-80,0]</returns>
        public static float Remap01ToDB(float x)
        {
            if (x < 0.01f) x = 0.0001f;
            return Mathf.Log10(x) * 20.0f;
        }

        #endregion

        #region 音频相关

        /// <summary>
        /// 播放指定资源路径的音频
        /// </summary>
        /// <param name="soundAssetPath">资源路径</param>
        /// <param name="groupName">指定播放的音频组</param>
        /// <param name="soundProp">音频属性</param>
        /// <returns>播放器id</returns>
        public uint Play(string soundAssetPath, string groupName, SoundProps soundProp = null)
        {
            return PlayDelayed(soundAssetPath, groupName, 0f, soundProp);
        }

        /// <summary>
        /// 延迟delay秒后播放指定资源路径的音频
        /// </summary>
        /// <param name="soundAssetPath">资源路径</param>
        /// <param name="groupName">指定播放的音频组</param>
        /// <param name="delay">延迟秒数</param>
        /// <param name="soundProp">音频属性</param>
        /// <returns>播放器id</returns>
        public uint PlayDelayed(string soundAssetPath, string groupName, float delay, SoundProps soundProp = null)
        {
            if (!HasSoundGroup(groupName))
            {
                Debugger.LogError($"要播放的音频 {soundAssetPath} 其指定音频组 {groupName} 不存在!");
            }

            InternalPlay(++_serialId, soundAssetPath, groupName, delay, soundProp);

            return _serialId;
        }

        /// <summary>
        /// 顺序播放指定资源路径的音频
        /// </summary>
        /// <param name="soundAssetPaths"></param>
        /// <param name="groupName"></param>
        /// <param name="soundProp"></param>
        /// <returns></returns>
        public uint PlayList(List<string> soundAssetPaths, string groupName, SoundProps soundProp = null)
        {
            if (soundAssetPaths == null || soundAssetPaths.Count < 1)
            {
                Debugger.LogError("要串联播放的音频列表为空! ");
            }
            if (!HasSoundGroup(groupName))
            {
                Debugger.LogError($"要播放的音频 {soundAssetPaths[0]} 其指定音频组 {groupName} 不存在!");
            }
            uint id = _serialId++;

            InternalPlayList(id, soundAssetPaths, groupName, soundProp);

            return id;
        }

        /// <summary>
        /// 在指定物体上播放指定资源路径的音频
        /// </summary>
        /// <param name="soundAssetPath">资源路径</param>
        /// <param name="groupName">指定播放的音频组</param>
        /// <param name="target">音频附着的物体</param>
        /// <param name="soundProp">音频属性</param>
        /// <returns>播放器id</returns>
        public uint PlayOnTarget(string soundAssetPath, string groupName, Transform target, SoundProps soundProp = null)
        {
            return PlayOnTargetDelayed(soundAssetPath, groupName, target, 0f, soundProp);
        }

        /// <summary>
        /// 延迟delay秒后在指定物体上播放指定资源路径的音频
        /// </summary>
        /// <param name="soundAssetPath">资源路径</param>
        /// <param name="groupName">指定播放的音频组</param>
        /// <param name="target">音频附着的物体</param>
        /// <param name="delay">延迟秒数</param>
        /// <param name="soundProp">音频属性</param>
        /// <returns>播放器id</returns>
        public uint PlayOnTargetDelayed(string soundAssetPath, string groupName, Transform target, float delay, SoundProps soundProp = null)
        {
            if (!HasSoundGroup(groupName))
            {
                Debugger.LogError($"要播放的音频 {soundAssetPath} 其指定音频组 {groupName} 不存在!");
            }
            uint id = _serialId++;

            InternalPlayOnTarget(id, soundAssetPath, groupName, target, delay, soundProp);

            return id;
        }

        /// <summary>
        /// 设置指定音频的暂停状态
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <param name="pause">暂停状态</param>
        public void SetPauseState(uint id, bool pause = true)
        {
            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                if (item.Value.HasSound(id))
                {
                    item.Value.SetPauseState(id, pause);
                    break;
                }
            }
        }

        /// <summary>
        /// 设置指定音频组的暂停状态
        /// </summary>
        /// <param name="groupName">音频组名</param>
        /// <param name="pause">暂停状态</param>
        public void SetGroupPauseState(string groupName, bool pause = true)
        {
            if (!HasSoundGroup(groupName))
            {
                Debugger.LogWarning($"要暂停播放的音频组 {groupName} 不存在!");
                return;
            }

            _soundGroupMap[groupName].SetAllPauseState(pause);
        }

        /// <summary>
        /// 设置全部音频的暂停状态
        /// </summary>
        /// <param name="pause">暂停状态</param>
        public void SetAllPauseState(bool pause = true)
        {
            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                item.Value.SetAllPauseState(pause);
            }
        }

        /// <summary>
        /// 设置指定音频的静音状态
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <param name="mute">静音状态</param>
        public void SetMuteState(uint id, bool mute = true)
        {
            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                if (item.Value.HasSound(id))
                {
                    item.Value.SetMuteState(id, mute);
                    break;
                }
            }
        }

        /// <summary>
        /// 设置指定音频组的静音状态
        /// </summary>
        /// <param name="groupName">音频组名</param>
        /// <param name="mute">静音状态</param>
        public void SetGroupMuteState(string groupName, bool mute = true)
        {
            if (!HasSoundGroup(groupName))
            {
                Debugger.LogWarning($"要静音播放的音频组 {groupName} 不存在!");
                return;
            }

            _soundGroupMap[groupName].SetAllMuteState(mute);
        }

        /// <summary>
        /// 设置全部音频的静音状态
        /// </summary>
        /// <param name="mute">静音状态</param>
        public void SetAllMuteState(bool mute = true)
        {
            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                item.Value.SetAllMuteState(mute);
            }
        }

        /// <summary>
        /// 停止播放指定音频
        /// </summary>
        /// <param name="id">音频序列号</param>
        public void Stop(uint id)
        {
            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                if (item.Value.HasSound(id))
                {
                    item.Value.Stop(id);
                    break;
                }
            }
        }

        /// <summary>
        /// 停止播放指定音频组的声音
        /// </summary>
        /// <param name="groupName">音频组名</param>
        public void StopGroup(string groupName)
        {
            if (!HasSoundGroup(groupName))
            {
                Debugger.LogWarning($"要停止播放音频的音频组{groupName}不存在!");
                return;
            }

            _soundGroupMap[groupName].StopAll();
        }

        /// <summary>
        /// 停止播放全部音频
        /// </summary>
        public void StopAll()
        {
            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                item.Value.StopAll();
            }
        }

        /// <summary>
        /// 获取指定音频的属性
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <returns>指定音频的属性,不存在则返回null</returns>
        public SoundProps GetSoundProp(uint id)
        {
            SoundProps sp = null;
            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                sp = item.Value.GetSoundProps(id);
                if (sp != null)
                {
                    break;
                }
            }
            return sp;
        }

        /// <summary>
        /// 设置指定音频的属性
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <param name="sp">要设置的属性</param>
        public void SetSoundProp(uint id, SoundProps sp)
        {
            if (sp == null)
            {
                return;
            }

            foreach (KeyValuePair<string, SoundGroup> item in _soundGroupMap)
            {
                if (item.Value.SetSoundProps(id, sp))
                {
                    break;
                }
            }
        }

        #endregion

        private async void InternalPlay(uint id, string soundAssetPath, string groupName, float delay, SoundProps soundProp = null)
        {
            Task<AudioClip> task = _resourceManager.LoadAssetAsync<AudioClip>(soundAssetPath);
            await task;
            if (task.Result != null)
            {
                _soundGroupMap[groupName].Play(id, task.Result, soundProp, delay);
            }
            else
            {
                Debugger.LogError($"音频{soundAssetPath}加载失败!");
            }
        }

        private async void InternalPlayList(uint id, List<string> soundAssetPaths, string groupName, SoundProps soundProp = null)
        {
            Task<List<AudioClip>> task = _resourceManager.LoadAssetsAsync<AudioClip>(soundAssetPaths);
            await task;
            if (task.Result != null)
            {
                _soundGroupMap[groupName].PlayList(id, task.Result, soundProp);
            }
            else
            {
                Debugger.LogError($"音频列表 {soundAssetPaths} 加载失败!");
            }
        }

        private async void InternalPlayOnTarget(uint id, string soundAssetPath, string groupName, Transform target, float delay, SoundProps soundProp = null)
        {
            Task<AudioClip> task = _resourceManager.LoadAssetAsync<AudioClip>(soundAssetPath);
            await task;
            if (task.Result != null)
            {
                _soundGroupMap[groupName].PlayOnTarget(id, target, task.Result, soundProp, delay);
            }
            else
            {
                Debugger.LogError($"音频{soundAssetPath}加载失败!");
            }
        }
    }

}
