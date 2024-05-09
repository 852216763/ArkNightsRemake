using DG.Tweening;
using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Splash : UIForm
{
    [SerializeField]
    Sprite[] splashSprite;
    [SerializeField]
    float duration;
    [SerializeField]
    Image img;

    protected override void OnShow(object userdata = null)
    {
        base.OnShow(userdata);
        img = transform.GetChild(0).GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("未找到开屏图片组件");
            FrameworkEntry.Procedure.ChangeProcedure<LoginProcedure>();
            return;
        }

        StartCoroutine(ShowSplash());
    }

    private IEnumerator ShowSplash()
    {
        foreach (Sprite sprite in splashSprite)
        {
            if (sprite == null)
            {
                continue;
            }

            img.sprite = sprite;
            yield return img.DOFade(1, 0.2f * duration).WaitForCompletion();
            yield return new WaitForSeconds(0.6f * duration);
            yield return img.DOFade(0, 0.2f * duration).WaitForCompletion();
        }
        FrameworkEntry.Procedure.ChangeProcedure<LoginProcedure>();
    }

}
