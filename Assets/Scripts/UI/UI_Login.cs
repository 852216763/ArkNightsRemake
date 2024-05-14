using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Login : UIForm
{
    [SerializeField]
    TextMeshProUGUI leftProgressText;
    [SerializeField]
    TextMeshProUGUI rightProgressText;
    [SerializeField]
    Slider leftProgress;
    [SerializeField]
    Slider rightProgress;

    bool Ka60 = true;
    Transform mainPanel;
    Button loginBtn;

    protected override void OnShow(object userdata = null)
    {
        base.OnShow(userdata);

        mainPanel = transform.Find("Main");
        loginBtn = mainPanel.GetChild(0).GetComponent<Button>();

        loginBtn.onClick.AddListener(OnStartBtnClick);
    }

    public void OnStartBtnClick()
    {
        mainPanel?.gameObject.SetActive(false);
        StartCoroutine(Login());

        FrameworkEntry.Sound.Play(Constant.SoundAsset_CharVoice((int)Constant.CharID.skadi2, Constant.CharVoiceType.Title), Constant.SoundGroupName_CharacterVoice);
    }

    IEnumerator Login()
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime * 0.7f;
            progress = Mathf.Clamp01(progress);
            // 卡60%加载是明日方舟的一环,不得不品尝
            if (Mathf.Abs(progress - 0.6f) < 0.05f && Ka60)
            {
                Ka60 = false;
                progress = 0.6f;
                leftProgress.value = progress;
                rightProgress.value = progress;
                leftProgressText.text = string.Format("{0}%", Mathf.Floor(progress * 100));
                rightProgressText.text = string.Format("{0}%", Mathf.Floor(progress * 100));
                yield return new WaitForSeconds(1);
            }
            else
            {
                leftProgress.value = progress;
                rightProgress.value = progress;
                leftProgressText.text = string.Format("{0}%", Mathf.Floor(progress * 100));
                rightProgressText.text = string.Format("{0}%", Mathf.Floor(progress * 100));
                yield return 0;
            }
        }
        FrameworkEntry.Procedure.ChangeProcedure<HomeProcedure>();
    }
}
