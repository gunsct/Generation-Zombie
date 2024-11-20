using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;

public class Item_GoodsBanner_ChapterClear_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
        public TextMeshProUGUI ChapNum;
        public Item_GoodsBanner_PriceGroup PriceGroup;
        public Item_RewardList_Item[] Rewards;
    }
    [SerializeField] SUI m_SUI;
    RecommendGoodsInfo m_Info;
    Action<RecommendGoodsInfo> m_CB;

    public void SetData(RecommendGoodsInfo _info, Action<RecommendGoodsInfo> _cb) {
        m_Info = _info;
        m_CB = _cb;
        m_SUI.ChapNum.text = (_info.m_SSACTData.m_Val.Find(o => o.m_Type == ShopAdviceConditionType.StageClear).m_Val / 100).ToString();
        string[] pricetxt = _info.m_STData.GetPriceTxt();
        var pinfo = USERINFO.m_ShopInfo.PIDs.Find(o => o.Idx == m_Info.m_SIdx);
        if (pinfo != null) {
            var price = IAP.GetPrice(pinfo.PID);
            pricetxt[0] = string.IsNullOrEmpty(price) ? pinfo.PriceText : price;
            pricetxt[2] = pinfo.SaleText;
        }
        m_SUI.PriceGroup.SetData(pricetxt[0], pricetxt[2]);

        List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
        for (int i = 0; i < _info.m_STData.m_Rewards.Count; i++)
            if (_info.m_STData.m_Rewards[i].m_ItemIdx != 0) rewards.AddRange(MAIN.GetRewardData(_info.m_STData.m_Rewards[i].m_ItemType, _info.m_STData.m_Rewards[i].m_ItemIdx, 1, true, false));
        
        for(int i = 0; i < m_SUI.Rewards.Length; i++) {
            if (i >= rewards.Count) m_SUI.Rewards[i].transform.parent.gameObject.SetActive(false);
            else m_SUI.Rewards[i].SetData(rewards[i]);
        }
    }
    public void ClickView() {
        m_CB?.Invoke(m_Info);
    }
}
