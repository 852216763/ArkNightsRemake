using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework
{
    /// <summary>
    /// 音频组
    /// </summary>
    public sealed partial class SoundGroup
    {
        Dictionary<uint, SoundAgent> _localAudioAgentMap;
        Dictionary<uint, SoundAgent> _touchedAudioAgentMap;
        Queue<SoundAgent> _idleAudioAgentQueue;
        List<SoundAgent> _soundAgentCache;
        GameObject _groupRoot;
        string _groupName;
        AudioMixerGroup _outputMixerGroup;
        bool _isMuted;
        bool _isPaused;

        /// <summary>
        /// 音频组名
        /// </summary>
        public string GroupName
        {
            get
            {
                return _groupName;
            }
        }

        /// <summary>
        /// 最大音频播放数
        /// 默认为10
        /// </summary>
        public uint Capacity
        {
            get;
            set;
        }

        /// <summary>
        /// 无空闲播放器时是否顶替同优先级音频
        /// 默认为false
        /// </summary>
        public bool ReplaceOnEqualPriority
        {
            get;
            set;
        }

        /// <summary>
        /// 混音器输出源
        /// </summary>
        public AudioMixerGroup AudioMixerGroup
        {
            get
            {
                return _outputMixerGroup;
            }
            internal set
            {
                _outputMixerGroup = value;
                foreach (KeyValuePair<uint, SoundAgent> item in _localAudioAgentMap)
                {
                    item.Value.Output = _outputMixerGroup;
                }
                foreach (KeyValuePair<uint, SoundAgent> item in _touchedAudioAgentMap)
                {
                    item.Value.Output = _outputMixerGroup;
                }
            }
        }

        /// <summary>
        /// 音频代理数
        /// </summary>
        public int AudioAgentCount
        {
            get
            {
                return _touchedAudioAgentMap.Count + _localAudioAgentMap.Count + _idleAudioAgentQueue.Count;
            }
        }

        internal SoundGroup(GameObject groupRoot, string groupName)
        {
            _groupRoot = groupRoot;
            _groupName = groupName;
            _localAudioAgentMap = new Dictionary<uint, SoundAgent>();
            _touchedAudioAgentMap = new Dictionary<uint, SoundAgent>();
            _idleAudioAgentQueue = new Queue<SoundAgent>();
            _soundAgentCache = new List<SoundAgent>();

            Capacity = 10;
            ReplaceOnEqualPriority = false;
        }

        internal void Update(float deltaTime, float realDeltaTime)
        {
            // 销毁完成播放任务的附着播放器
            foreach (KeyValuePair<uint, SoundAgent> item in _touchedAudioAgentMap)
            {
                SoundAgent temp = item.Value;
                if (temp.Finish)
                {
                    _soundAgentCache.Add(temp);
                }
            }
            foreach (SoundAgent agent in _soundAgentCache)
            {
                _touchedAudioAgentMap.Remove(agent.ID);

                agent.OnFinishCallback?.Invoke();
                agent.Destroy();
            }
            _soundAgentCache.Clear();

            // 回收完成播放任务的本地播放器
            foreach (KeyValuePair<uint, SoundAgent> item in _localAudioAgentMap)
            {
                SoundAgent temp = item.Value;
                if (temp.Finish)
                {
                    // 若有任务队列则进行下一项任务
                    if (temp.HasNextPlayTask)
                    {
                        temp.PlayNext();
                    }
                    else
                    {
                        _soundAgentCache.Add(temp);
                    }
                }
            }
            foreach (SoundAgent agent in _soundAgentCache)
            {
                _localAudioAgentMap.Remove(agent.ID);
                _idleAudioAgentQueue.Enqueue(agent);

                agent.OnFinishCallback?.Invoke();
            }
            _soundAgentCache.Clear();
        }

        internal void ShutDown()
        {
            foreach (KeyValuePair<uint, SoundAgent> item in _localAudioAgentMap)
            {
                item.Value.Destroy();
            }
            foreach (KeyValuePair<uint, SoundAgent> item in _touchedAudioAgentMap)
            {
                item.Value.Destroy();
            }
            _localAudioAgentMap.Clear();
            _touchedAudioAgentMap.Clear();

            GameObject.Destroy(_groupRoot);
        }

        /// <summary>
        /// 是否存在对应id的音频播放任务
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <returns>是否存在</returns>
        internal bool HasSound(uint id)
        {
            if (_localAudioAgentMap.ContainsKey(id) || _touchedAudioAgentMap.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 播放指定音频
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <param name="clip">音频对象</param>
        /// <param name="sp">音频属性</param>
        /// <param name="delay">播放延迟秒数</param>
        internal void Play(uint id, AudioClip clip, SoundProps sp = null, float delay = 0f)
        {
            if (clip == null)
            {
                Debugger.LogError("要播放的音频为空!");
                return;
            }
            if (sp == null)
            {
                sp = new SoundProps();
            }
            if (_isMuted)
            {
                sp.mute = _isMuted;
            }
            if (_isPaused)
            {
                sp.pause = _isPaused;
            }

            SoundAgent agent = GetIdleAgent(id, sp);
            // 无可用代理,放弃本次播放
            if (agent == null)
            {
                Debugger.LogWarning($"播放 {clip.name} 失败,因为没有空闲的播放器");
                return;
            }

            _localAudioAgentMap.Add(id, agent);
            agent.Play(clip, sp, delay);
        }

        /// <summary>
        /// 播放指定音频
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <param name="clips">音频列表</param>
        /// <param name="sp">音频属性</param>
        internal void PlayList(uint id, List<AudioClip> clips, SoundProps sp = null)
        {
            if (clips == null || clips.Count < 1)
            {
                Debugger.LogError("要播放的音频列表为空!");
                return;
            }
            if (sp == null)
            {
                sp = new SoundProps();
            }
            if (_isMuted)
            {
                sp.mute = _isMuted;
            }
            if (_isPaused)
            {
                sp.pause = _isPaused;
            }

            SoundAgent agent = GetIdleAgent(id, sp);
            // 无可用代理,放弃本次播放
            if (agent == null)
            {
                Debugger.LogWarning($"播放列表 {clips[0].name} 失败,因为没有空闲的播放器");
                return;
            }

            _localAudioAgentMap.Add(id, agent);
            agent.PlayList(clips, sp);
        }

        /// <summary>
        /// 在指定物体上播放指定音频
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <param name="target">播放器附着对象</param>
        /// <param name="clip">音频对象</param>
        /// <param name="sp">音频属性</param>
        /// <param name="delay">播放延迟秒数</param>
        internal void PlayOnTarget(uint id, Transform target, AudioClip clip, SoundProps sp = null, float delay = 0f)
        {
            if (target == null)
            {
                Debugger.LogError("音频播放的附着目标物体为空!");
            }
            if (clip == null)
            {
                Debugger.LogError("要播放的音频为空!");
                return;
            }
            if (sp == null)
            {
                sp = new SoundProps();
                sp.mute = _isMuted;
                sp.pause = _isPaused;
            }

            AudioSource @as = _groupRoot.AddComponent<AudioSource>();
            SoundAgent agent = new SoundAgent(id, @as, AudioMixerGroup);
            _touchedAudioAgentMap.Add(id, agent);
            agent.Play(clip, sp, delay);
        }

        /// <summary>
        /// 设置指定音频暂停状态
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <param name="pause">暂停状态</param>
        public void SetPauseState(uint id, bool pause = true)
        {
            SoundAgent agent = GetAgent(id);
            agent.Pause = pause;
        }

        /// <summary>
        /// 设置全部音频暂停状态
        /// </summary>
        /// <param name="pause">暂停状态</param>
        public void SetAllPauseState(bool pause = true)
        {
            _isPaused = pause;
            foreach (KeyValuePair<uint, SoundAgent> item in _localAudioAgentMap)
            {
                item.Value.Pause = pause;
            }
            foreach (KeyValuePair<uint, SoundAgent> item in _touchedAudioAgentMap)
            {
                item.Value.Pause = pause;
            }
        }

        /// <summary>
        /// 设置指定音频静音状态
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <param name="mute">静音状态</param>
        public void SetMuteState(uint id, bool mute = true)
        {
            SoundAgent agent = GetAgent(id);
            agent.Mute = mute;
        }

        /// <summary>
        /// 设置全部音频静音状态
        /// </summary>
        /// <param name="mute">静音状态</param>
        public void SetAllMuteState(bool mute = true)
        {
            _isMuted = mute;
            foreach (KeyValuePair<uint, SoundAgent> item in _localAudioAgentMap)
            {
                item.Value.Mute = mute;
            }
            foreach (KeyValuePair<uint, SoundAgent> item in _touchedAudioAgentMap)
            {
                item.Value.Mute = mute;
            }
        }

        /// <summary>
        /// 停止播放指定音频
        /// </summary>
        /// <param name="id">音频序列号</param>
        public void Stop(uint id)
        {
            SoundAgent agent = GetAgent(id);
            if (agent == null)
            {
                return;
            }

            agent.Stop();
        }

        /// <summary>
        /// 停止播放所有音频
        /// </summary>
        public void StopAll()
        {
            foreach (KeyValuePair<uint, SoundAgent> item in _localAudioAgentMap)
            {
                item.Value.Stop();
            }
            foreach (KeyValuePair<uint, SoundAgent> item in _touchedAudioAgentMap)
            {
                item.Value.Stop();
            }
        }

        /// <summary>
        /// 获取指定音频的属性
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <returns>属性合集</returns>
        public SoundProps GetSoundProps(uint id)
        {
            SoundAgent agent = GetAgent(id);
            if (agent == null)
            {
                return null;
            }
            return agent.GetSoundProps();
        }

        /// <summary>
        /// 设置指定音频的属性
        /// </summary>
        /// <param name="id">音频序列号</param>
        /// <param name="props">属性合集</param>
        /// <returns>设置操作是否成功</returns>
        public bool SetSoundProps(uint id, SoundProps props)
        {
            SoundAgent agent = GetAgent(id);
            if (agent == null)
            {
                return false;
            }

            agent.SetSoundProps(props);
            return true;
        }

        private SoundAgent GetAgent(uint id)
        {
            SoundAgent agent = null;
            if (_localAudioAgentMap.ContainsKey(id))
            {
                agent = _localAudioAgentMap[id];
            }
            if (_touchedAudioAgentMap.ContainsKey(id))
            {
                agent = _touchedAudioAgentMap[id];
            }

            return agent;
        }

        private SoundAgent GetIdleAgent(uint id, SoundProps sp)
        {
            SoundAgent agent = null;
            // 检查是否有无任务的代理
            if (_idleAudioAgentQueue.Count > 0)
            {
                agent = _idleAudioAgentQueue.Dequeue();
                agent.ID = id;
            }
            else
            {
                // 音频代理数小于容量
                if (AudioAgentCount < Capacity)
                {
                    AudioSource @as = _groupRoot.AddComponent<AudioSource>();
                    agent = new SoundAgent(id, @as, AudioMixerGroup);
                }
                if (agent == null)
                {
                    // 检查是否有可顶替的正在播放的音频
                    foreach (KeyValuePair<uint, SoundAgent> item in _localAudioAgentMap)
                    {
                        if (item.Value.Priority > sp.priority ||
                            ReplaceOnEqualPriority && item.Value.Priority == sp.priority)
                        {
                            agent = item.Value;
                            agent.ID = id;
                            _localAudioAgentMap.Remove(item.Key);
                            break;
                        }
                    }
                }
            }

            return agent;
        }
    }
}
