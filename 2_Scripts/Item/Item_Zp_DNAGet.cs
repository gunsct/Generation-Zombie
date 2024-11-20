using System;
using TMPro;
using UnityEngine;
using static LS_Web;

public class Item_Zp_DNAGet : ObjMng
{
    [Serializable]
    public struct SUI
    {
        public Item_RewardList_Item RewardCard;
        public TextMeshProUGUI NameTxt;
    }

    [SerializeField] private SUI m_SUI;
    
    public void SetData(RES_REWARD_BASE _rewardbase)
    {
        m_SUI.RewardCard.SetData(_rewardbase, null, false);
        m_SUI.NameTxt.text = m_SUI.RewardCard.GetRewardName();
    }
}