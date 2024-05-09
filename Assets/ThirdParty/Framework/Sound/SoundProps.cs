using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 音频的属性合集
    /// </summary>
    public class SoundProps
    {
        // 向此类中添加新属性后,记得修改SoundAgent类对应的取值与赋值方法
        [Range(0, 1)]
        public float volume;
        public bool playOnAwake;
        public bool mute;
        public bool loop;
        public float pitch;
        public float time;
        public bool isPlaying;
        [Range(0, 256)]
        public int priority;
        [Range(0, 1)]
        public float spatialBlend;
        public float minDistance;
        public float maxDistance;

        // 是否暂停中
        public bool pause;
        // 是否播放完毕
        public bool finish;
        // 播放完毕回调
        public Action onFinishCallback;


        public SoundProps()
        {
            volume = 1f;
            playOnAwake = false;
            mute = false;
            loop = false;
            pitch = 1;
            time = 0;
            isPlaying = true;
            priority = 128;
            spatialBlend = 0;
            minDistance = 1;
            maxDistance = 500;

            pause = false;
            finish = false;
            onFinishCallback = null;
        }

    }

}
