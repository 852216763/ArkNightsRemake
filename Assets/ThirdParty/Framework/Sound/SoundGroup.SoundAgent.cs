using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework
{
    public sealed partial class SoundGroup
    {
        /// <summary>
        /// 音频播放器代理
        /// </summary>
        private class SoundAgent
        {
            AudioSource @as;
            List<AudioClip> _audioClips;
            bool _paused;
            int index;
            bool cachedLoop;

            /// <summary>
            /// 任务ID
            /// </summary>
            public uint ID
            {
                get;
                set;
            }

            /// <summary>
            /// 是否播放完毕
            /// </summary>
            public bool Finish
            {
                get
                {
                    return @as == null || !_paused && !@as.loop && !@as.isPlaying && @as.time == 0;
                }
            }

            public bool HasNextPlayTask
            {
                get
                {
                    return index + 1 < _audioClips.Count;
                }
            }

            /// <summary>
            /// 音频暂停
            /// </summary>
            public bool Pause
            {
                get
                {
                    return _paused;
                }
                set
                {
                    if (@as == null)
                    {
                        return;
                    }
                    if (_paused == value)
                    {
                        return;
                    }

                    _paused = value;
                    if (_paused)
                    {
                        @as.Pause();
                    }
                    else
                    {
                        @as.UnPause();
                    }
                }
            }

            /// <summary>
            /// 音频静音
            /// </summary>
            public bool Mute
            {
                get
                {
                    return @as == null ? @as.mute : false;
                }
                set
                {
                    if (@as == null)
                    {
                        return;
                    }
                    if (_paused == value)
                    {
                        return;
                    }

                    @as.mute = value;
                }
            }

            /// <summary>
            /// 音频输出的混音器组
            /// </summary>
            public AudioMixerGroup Output
            {
                get
                {
                    return @as.outputAudioMixerGroup;
                }
                set
                {
                    if (@as == null)
                    {
                        return;
                    }
                    @as.outputAudioMixerGroup = value;
                }
            }

            /// <summary>
            /// 音频代理优先级
            /// </summary>
            public int Priority
            {
                get
                {
                    return @as.priority;
                }
                set
                {
                    @as.priority = value;
                }
            }

            /// <summary>
            /// 播放完毕回调
            /// </summary>
            public Action OnFinishCallback;

            public SoundAgent(uint id, AudioSource @as, AudioMixerGroup output = null)
            {
                ID = id;

                if (@as == null)
                {
                    Debugger.LogError("创建音频代理时, AudioSource组件为空!");
                    return;
                }
                this.@as = @as;
                _audioClips = new List<AudioClip>();
                Output = output;
            }

            /// <summary>
            /// 设置音频的属性
            /// </summary>
            /// <param name="props">音频属性合集</param>
            public void SetSoundProps(SoundProps props)
            {
                @as.volume = props.volume;
                @as.playOnAwake = props.playOnAwake;
                @as.loop = props.loop;
                @as.pitch = props.pitch;
                @as.time = props.time;
                @as.priority = props.priority;
                @as.spatialBlend = props.spatialBlend;
                @as.minDistance = props.minDistance;
                @as.maxDistance = props.maxDistance;

                Pause = props.pause;
                Mute = props.mute;
                OnFinishCallback = props.onFinishCallback;
            }

            /// <summary>
            /// 获取音频的属性
            /// </summary>
            /// <returns>音频属性合集</returns>
            public SoundProps GetSoundProps()
            {
                SoundProps props = new SoundProps();
                props.volume = @as.volume;
                props.playOnAwake = @as.playOnAwake;
                props.mute = @as.mute;
                props.loop = @as.loop;
                props.pitch = @as.pitch;
                props.time = @as.time;
                props.isPlaying = @as.isPlaying;
                props.priority = @as.priority;
                props.spatialBlend = @as.spatialBlend;
                props.minDistance = @as.minDistance;
                props.maxDistance = @as.maxDistance;

                props.pause = false;
                props.finish = false;
                props.onFinishCallback = OnFinishCallback;

                return props;
            }

            /// <summary>
            /// 播放音频
            /// </summary>
            /// <param name="clip">音频片段</param>
            /// <param name="props">音频属性合集</param>
            /// <param name="delay">播放延迟(秒)</param>
            public void Play(AudioClip clip, SoundProps props, float delay = 0f)
            {
                Stop();

                _audioClips.Add(clip);
                @as.clip = clip;
                SetSoundProps(props);
                if (delay == 0f)
                {
                    @as.Play();
                }
                else
                {
                    @as.PlayDelayed(delay);
                }
            }

            /// <summary>
            /// 按顺序播放音频
            /// </summary>
            /// <param name="clips">音频列表</param>
            /// <param name="props">音频属性合集</param>
            public void PlayList(List<AudioClip> clips, SoundProps props)
            {
                Stop();

                _audioClips.AddRange(clips);
                @as.clip = clips[0];
                cachedLoop = props.loop;
                SetSoundProps(props);
                @as.loop = false;

                index = -1;
                PlayNext();
            }

            /// <summary>
            /// 进行下一个播放任务
            /// </summary>
            public void PlayNext()
            {
                if (!HasNextPlayTask)
                {
                    Debugger.LogError("不存在下一个播放任务!");
                    return;
                }

                index++;
                @as.clip = _audioClips[index];
                if (!HasNextPlayTask)
                {
                    @as.loop = cachedLoop;
                }
                @as.Play();
            }

            /// <summary>
            /// 停止播放音频
            /// </summary>
            public void Stop()
            {
                // 注销播放完成回调
                index = 0;
                OnFinishCallback = null;
                @as.Stop();

                foreach (AudioClip item in _audioClips)
                {
                    FrameworkEntry.Resource.ReleaseAsset<AudioClip>(item);
                }
                _audioClips.Clear();
            }

            /// <summary>
            /// 重置音频代理属性设置
            /// </summary>
            public void ResetSoundProps()
            {
                SetSoundProps(new SoundProps());
            }
            
            /// <summary>
            /// 销毁音频代理
            /// </summary>
            public void Destroy()
            {
                Stop();
                if (@as != null)
                {
                    GameObject.Destroy(@as);
                }
                _audioClips = null;
            }
        }
    }


}
