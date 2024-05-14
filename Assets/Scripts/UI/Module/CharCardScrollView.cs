using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCardScrollView : MonoBehaviour
{
    [SerializeField]
    GameObject charCardItemPrefab;
    [SerializeField]
    Transform charCardContent;

    public List<CharCard> charCardItemList;
    public List<CharData> showedCharDataList;

    /// <summary>
    /// 刷新显示的角色Card
    /// </summary>
    /// <param name="showCharList">所有需要显示的角色数据</param>
    public void RefreshScrollContent(List<CharData> showCharList)
    {
        showedCharDataList = showCharList;
        if (charCardItemList == null)
        {
            charCardItemList = new List<CharCard>();
        }

        // 获取所有的选中状态的信息
        bool singleMode = true;
        Dictionary<int, int> selectTemp = new Dictionary<int, int>();
        foreach (CharCard item in charCardItemList)
        {
            if (item.IsSelected)
            {
                selectTemp.Add(item.Data.CharID, item.SelectedIndex);
                if (item.SelectedIndex > 0)
                {
                    singleMode = false;
                }
            }
        }

        int i = 0;
        while (i < showCharList.Count)
        {
            CharCard card;
            if (i >= charCardItemList.Count)
            {
                GameObject newItem = GameObject.Instantiate(charCardItemPrefab, charCardContent);
                card = newItem.GetComponent<CharCard>();
                charCardItemList.Add(card);
            }
            else
            {
                charCardItemList[i].gameObject.SetActive(true);
                card = charCardItemList[i].GetComponent<CharCard>();
            }
            card.UpdateCard(showCharList[i]);
            card.CancelSelect();

            // 还原缓存的选中信息
            if (selectTemp.ContainsKey(card.Data.CharID))
            {
                if (singleMode)
                {
                    card.Select();
                }
                else
                {
                    card.SelectMulti(selectTemp[card.Data.CharID]);
                }
                selectTemp.Remove(card.Data.CharID);
            }
            i++;
        }
        while (i < charCardItemList.Count)
        {
            charCardItemList[i].gameObject.SetActive(false);
            i++;
        }
    }

    /// <summary>
    /// 选择指定ID的角色Card
    /// </summary>
    /// <param name="charID">要选择的角色ID</param>
    /// <returns>选择到的Card对象</returns>
    public CharCard SelectSingleCharCard(int charID)
    {
        foreach (CharCard item in charCardItemList)
        {
            if (item.Data.CharID == charID)
            {
                item.Select();
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 按顺序选择charIDList中的角色Card
    /// </summary>
    /// <param name="charIDList">所有要选择的角色ID列表</param>
    /// <returns>选择到的Card对象列表</returns>
    public List<CharCard> SelectMultiCharCard(List<int> charIDList)
    {
        List<CharCard> selectList = new List<CharCard>();
        
        // 转换到map中,方便一次遍历完成选择
        Dictionary<int, int> map = new Dictionary<int, int>();
        int order = 1;
        for (int i = 0; i < charIDList.Count; i++)
        {
            if (!map.ContainsKey(charIDList[i]))
            {
                map.Add(charIDList[i], order);
                order++;
            }
        }

        foreach (CharCard item in charCardItemList)
        {
            if (map.ContainsKey(item.Data.CharID))
            {
                item.SelectMulti(map[item.Data.CharID]);
                map.Remove(item.Data.CharID);
                selectList.Add(item);
            }
        }
        return selectList;
    }
}
