using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtend
{
    /// <summary>
    /// 淡入淡出
    /// </summary>
    /// <param name="endAlpha">最终透明度</param>
    /// <param name="duration">过渡持续时间</param>
    /// <param name="onFinishCallback">完成回调</param>
    public static void Fade(this RectTransform transform,float endAlpha, float duration = 0f, Action onFinishCallback = null)
    {
        CanvasGroup group = transform.GetOrAddComponent<CanvasGroup>();
        Tween t = DOTween.To(() => group.alpha, value => group.alpha = value, endAlpha, duration);
        t.onComplete = () => { onFinishCallback?.Invoke(); };
    }

    /// <summary>
    /// 拉近拉远
    /// </summary>
    /// <param name="beginScale">起始大小</param>
    /// <param name="endScale">结束大小</param>
    /// <param name="duration">过渡持续时间</param>
    /// <param name="onFinishCallback">完成回调</param>
    public static void Zoom(this RectTransform transform,float beginScale, float endScale, float duration = 0f, Action onFinishCallback = null)
    {
        transform.localScale = Vector3.one * beginScale;
        Tween t = DOTween.To(
            () => transform.localScale,
            value => transform.localScale = value,
            Vector3.one * endScale,
            duration);
        t.onComplete = () => { onFinishCallback?.Invoke(); };
    }
}
