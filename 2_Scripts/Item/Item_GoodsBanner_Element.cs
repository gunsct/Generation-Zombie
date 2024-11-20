using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_GoodsBanner_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI[] Name;
		public TextMeshProUGUI[] Desc;
		public TextMeshProUGUI[] Step;
		public Item_RewardList_Item[] RewardCards;
		public Image[] RewardImgs;
		public TextMeshProUGUI[] RewardCnts;
		public Image[] Portrait;
		public TextMeshProUGUI[] CharName;
	}
	[SerializeField] SUI m_SUI;
	RecommendGoodsInfo m_Info;
	Action<RecommendGoodsInfo> m_CB;

	[SerializeField] bool Is_UseCardCnt;
	public void SetData(RecommendGoodsInfo _info, Action<RecommendGoodsInfo> _cb) {
		m_Info = _info;
		m_CB = _cb;

		TItemTable idata = TDATA.GetItemTable(_info.m_STData.m_Rewards[0].m_ItemIdx);
		if (idata != null && m_Info.m_STData.m_Group != ShopGroup.DailyPack) {
			//상품명
			for (int i = 0; i < m_SUI.Name.Length; i++) {
				if (m_SUI.Name[i] != null) m_SUI.Name[i].text = idata.GetName();
			}
			//상품 설명
			for (int i = 0; i < m_SUI.Desc.Length; i++) {
				if (m_SUI.Desc[i] != null) m_SUI.Desc[i].text = idata.GetInfo();
			}
			//단계
			for (int i = 0; i < m_SUI.Step.Length; i++) {
				if (m_SUI.Step[i] != null && m_Info.m_STData.m_Info > 0) m_SUI.Step[i].text = TDATA.GetString(ToolData.StringTalbe.Etc, m_Info.m_STData.m_Info);
			}
			//구성상품
			List<RES_REWARD_BASE> rewards = MAIN.GetRewardData(RewardKind.Item, idata.m_Idx, _info.m_STData.m_Rewards[0].m_ItemCnt, true, false);
			for (int i = 0; i < m_SUI.RewardCards.Length; i++) {
				if (m_SUI.RewardCards[i] == null) continue;
				if (i < rewards.Count) {
					m_SUI.RewardCards[i].SetData(rewards[i], null, false);
					m_SUI.RewardCards[i].SetCntActive(Is_UseCardCnt);
				}
				else m_SUI.RewardCards[i].gameObject.SetActive(false);
			}
			for (int i = 0; i < m_SUI.RewardImgs.Length; i++) {
				if (m_SUI.RewardImgs[i] == null) continue;
				if (i < rewards.Count) {
					m_SUI.RewardImgs[i].sprite = rewards[i].GetImage();
				}
				else m_SUI.RewardImgs[i].gameObject.SetActive(false);
			}
			for (int i = 0; i < m_SUI.RewardCnts.Length; i++) {
				if (m_SUI.RewardCnts[i] == null) continue;
				if (i < rewards.Count) {
					m_SUI.RewardCnts[i].text = rewards[i].GetCnt().ToString();
				}
				else m_SUI.RewardCnts[i].gameObject.SetActive(false);
			}
			//캐릭터 패키지일 때 초상화
			ShopAdviceConditionValue val = TDATA.GetShopAdviceTableToSIdx(m_Info.m_SIdx).m_Val.Find(o => o.m_Type == ShopAdviceConditionType.HaveChar);
			if (val != null) {
				TCharacterTable tdata = TDATA.GetCharacterTable(val.m_Val);
				for (int i = 0; i < m_SUI.Portrait.Length; i++) {
					if (m_SUI.Portrait[i] != null) {
						m_SUI.Portrait[i].sprite = tdata.GetPortrait();
					}
				}
				for (int i = 0; i < m_SUI.CharName.Length; i++) {
					m_SUI.CharName[i].text = tdata.GetCharName();
				}
			}
		}
		else {
			for (int i = 0; i < m_SUI.Name.Length; i++) {
				if (m_SUI.Name[i] != null) m_SUI.Name[i].text = m_Info.m_STData.GetName();
			}
			//상품 설명
			for (int i = 0; i < m_SUI.Desc.Length; i++) {
				if (m_SUI.Desc[i] != null) m_SUI.Desc[i].text = m_Info.m_STData.GetInfo();
			}
		}
	}

	public void ClickView() {
		m_CB?.Invoke(m_Info);
	}
}
