using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_Event_11_Minigame : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI RewardLv;
		public Image RewardLvBar;
		public Item_RewardList_Item Reward;
		public Image TurkeyIcon;
		public TextMeshProUGUI Lv;
		public TextMeshProUGUI Exp;
		public Image ExpBar;
		public TextMeshProUGUI ItemCnt;
	}
	[SerializeField] SUI m_SUI;
	MyFAEvent m_Event;
	FAEventData_GrowUP m_GrowUp;
	List<FAEventData_GrowUP.LVInfo> m_ExpInfos;
	int m_Lv, m_Exp;

	public void SetData(MyFAEvent _event) {
		m_Event = _event;

		SetUI();
	}
	public void SetUI() {
		m_Lv = (int)m_Event.Values[2];
		m_Exp = (int)m_Event.Values[3];

		//m_SUI.TurkeyIcon.sprite = UTILE.LoadImg("UI/Icon/Event/", "png");
		m_GrowUp = (FAEventData_GrowUP)m_Event.RealData;
		m_ExpInfos = m_GrowUp.ExpInfo;
		FAEventData_GrowUP.LVInfo nowinfo = m_ExpInfos.Find(o => o.LV == m_Lv);
		RES_REWARD_BASE reward = MAIN.GetRewardData(nowinfo.Reward.Kind, nowinfo.Reward.Idx, nowinfo.Reward.Cnt)[0];
		m_SUI.Reward.SetData(reward, null, false);
		m_SUI.RewardLv.text = string.Format("{0}/{1}", m_Lv, m_ExpInfos.Count);
		m_SUI.RewardLvBar.fillAmount = (float)m_Lv / (float)m_ExpInfos.Count;

		m_SUI.Lv.text = string.Format("Lv{0}", m_Lv);
		m_SUI.Exp.text = string.Format("{0}/{1}", m_Exp, nowinfo.Cnt);
		m_SUI.ExpBar.fillAmount = (float)m_Exp / (float)nowinfo.Cnt;
		m_SUI.ItemCnt.text = USERINFO.GetItemCount(BaseValue.EVENT_11_ITEMIDX).ToString();
	}
	/// <summary> 사료 주기 </summary>
	public void Click_GiveFood(int _cnt) {
		if(USERINFO.GetItemCount(BaseValue.EVENT_11_ITEMIDX) < _cnt) {
			return;
		}
		WEB.SEND_REQ_EVENT_GROWUP((res) => {
			if (res.IsSuccess()) {
				SetUI();
			}
		}, m_Event, _cnt);
	}
	/// <summary> 보상 목록 </summary>
	public void Click_RewardList() {

	}
}
