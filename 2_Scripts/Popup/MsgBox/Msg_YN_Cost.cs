using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_YN_Cost : Msg_YN
{
	[System.Serializable]
	struct GSUI
	{
		public Image Icon;
		public TextMeshProUGUI[] Cost;
		public Button YesBtn;
		public GameObject[] CostPanel;

		[Header("튜토리얼 전용")]
		public GameObject[] Panels;
	}
	[SerializeField]
	GSUI m_GSUI;
	int[] m_Val = new int[2];
	public bool IS_CanBuy { get { return m_Val[0] >= m_Val[1]; } }
	/// <summary>aobjValue 0: 재화 타입, 1: 재화 아이템 인덱스, 2:요구 재화량, 3:카운트 체크 여부</summary>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);


		switch ((PriceType)aobjValue[0]) {
			case PriceType.Money:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Dollar);
				m_Val[0] = (int)USERINFO.m_Money;
			break;
			case PriceType.Cash:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Cash);
				m_Val[0] = (int)USERINFO.m_Cash;
				break;
			case PriceType.Energy:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Energy); 
				m_Val[0] = (int)USERINFO.m_Energy.Cnt;
				break;
			case PriceType.PVPCoin:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.PVPCoin);
				m_Val[0] = (int)USERINFO.m_PVPCoin;
				break;
			case PriceType.GuildCoin:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Guild_Coin);
				m_Val[0] = (int)USERINFO.m_GCoin;
				break;
			case PriceType.Mileage:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Mileage);
				m_Val[0] = (int)USERINFO.m_Mileage;
				break;
			case PriceType.Item:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon((int)aobjValue[1]);
				m_Val[0] = (int)USERINFO.GetItemCount((int)aobjValue[1]);
				break;
		}
		//val[0] = (int)aobjValue[1];
		m_Val[1] = (int)aobjValue[2];
		m_GSUI.Cost[0].text = m_Val[0].ToString();
		m_GSUI.Cost[0].color = BaseValue.GetUpDownStrColor((long)m_Val[0], (long)m_Val[1]);
		m_GSUI.Cost[1].text = m_Val[1].ToString();
		bool cntckcnt = aobjValue.Length > 3 ? (bool)aobjValue[3] : true;
		m_GSUI.YesBtn.interactable = cntckcnt ? m_Val[0] >= m_Val[1] : true;

		m_GSUI.CostPanel[0].SetActive(m_Val[1] > 0);
		m_GSUI.CostPanel[1].SetActive(m_Val[1] < 1);
	}

	public GameObject GetBtn(EMsgBtn btn)
	{
		return m_GSUI.Panels[(int)btn];
	}
}
