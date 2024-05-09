using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWithSe : Button
{
    [SerializeField]
    string SEAssetPath;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (SEAssetPath != null)
        {
            FrameworkEntry.Sound.Play(SEAssetPath, Constant.SoundGroupName_UISFX);
        }

    }

}
