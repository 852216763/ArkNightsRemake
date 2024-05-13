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
    public List<CharData> charDataList;

    public void RefreshScrollContent(List<CharData> showCharList)
    {
        charDataList = showCharList;
        if (charCardItemList == null)
        {
            charCardItemList = new List<CharCard>();
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
            i++;
        }
        while (i < charCardItemList.Count)
        {
            charCardItemList[i].gameObject.SetActive(false);
            i++;
        }
    }


}
