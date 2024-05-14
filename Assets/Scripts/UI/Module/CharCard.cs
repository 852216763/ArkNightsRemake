using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharCard : MonoBehaviour
{
    CharData _data;

    #region 卡片组件元素
    [SerializeField]
    Image bgImage;
    [SerializeField]
    Image portraitImage;
    [SerializeField]
    Image cornerTopImage;
    [SerializeField]
    Image professionIconImage;
    [SerializeField]
    Image rarityImage;
    [SerializeField]
    Image cornerLightImage;
    [SerializeField]
    Image beltImage;
    [SerializeField]
    Image eliteLevelImage;
    [SerializeField]
    Image expImage;

    [SerializeField]
    TextMeshProUGUI levelTMP;
    [SerializeField]
    TextMeshProUGUI nameTMP;

    [SerializeField]
    GameObject SelectedFrame;
    #endregion

    #region 资源
    [SerializeField]
    Sprite[] bgSprite;
    [SerializeField]
    Sprite[] cornerTopSprite;
    [SerializeField]
    Sprite[] professionIconSprite;
    [SerializeField]
    Sprite[] raritySprite;
    [SerializeField]
    Sprite[] cornerLightSprite;
    [SerializeField]
    Sprite[] beltSprite;
    [SerializeField]
    Sprite[] eliteLevelSprite;
    #endregion

    /// <summary>
    /// 是否被选中
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// 被选中时的序号
    /// </summary>
    public int SelectedIndex { get; set; }

    public CharData Data { get => _data; }



    /// <summary>
    /// 更新卡片信息
    /// </summary>
    /// <param name="data"></param>
    public void UpdateCard(CharData data)
    {
        if (data == null)
        {
            throw new System.Exception("更新角色卡时数据为空!");
        }
        _data = data;
        CharMeta meta = _data.Meta;
        if (meta.Rarity < 1)
        {
            Debug.Log($"角色数据为空 ID: {data.CharID}");
            return;
        }
        // 稀有度影响的卡片元素
        bgImage.sprite = bgSprite[Mathf.Clamp(meta.Rarity - 3, 0, 3)];
        cornerTopImage.sprite = cornerTopSprite[meta.Rarity - 1];
        professionIconImage.sprite = professionIconSprite[(int)meta.Profession];
        rarityImage.sprite = raritySprite[meta.Rarity - 1];
        cornerLightImage.sprite = cornerLightSprite[meta.Rarity - 1];
        beltImage.sprite = beltSprite[Mathf.Clamp(meta.Rarity - 3, 0, 3)];
        // 精英化图标
        if (_data.Elite == CharEliteLevel.zero)
        {
            eliteLevelImage.gameObject.SetActive(false);
        }
        else
        {
            eliteLevelImage.sprite = eliteLevelSprite[(int)_data.Elite - 1];
            eliteLevelImage.gameObject.SetActive(true);
        }
        // 等级填充
        expImage.fillAmount = data.MaxLevelUpExp == 0 ? 1 : data.CurrentExp / data.MaxLevelUpExp;

        portraitImage.sprite = meta.Portraits[0];
        levelTMP.text = _data.CurrentLevel.ToString();
        nameTMP.text = meta.ChineseName;

    }

    public CharData Select()
    {
        IsSelected = true;
        SelectedFrame.transform.GetChild(0).gameObject.SetActive(false);
        SelectedFrame.SetActive(true);
        return _data;
    }

    public CharData SelectMulti(int index)
    {
        IsSelected = true;
        SelectedIndex = index;
        TextMeshProUGUI tmp = SelectedFrame.GetComponentInChildren<TextMeshProUGUI>(true);
        tmp.text = index.ToString();
        tmp.gameObject.SetActive(true);
        SelectedFrame.SetActive(true);
        return _data;
    }

    public void CancelSelect()
    {
        IsSelected = false;
        SelectedIndex = 0;
        SelectedFrame.SetActive(false);
    }

}
