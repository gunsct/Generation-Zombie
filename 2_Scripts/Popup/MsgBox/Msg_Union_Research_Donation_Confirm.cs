using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Item_PDA_Collection_Main;


public class Msg_Union_Research_Donation_Confirm : MsgBoxBase
{
#pragma warning disable 0649

	[System.Serializable]
	struct SUI
	{
		public Item_RewardList_Item Item;
		public TextMeshProUGUI MyItemCnt;

		public TextMeshProUGUI Msg;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	int m_ItemIdx;
	int m_ItemTotalCnt;
	int m_UseCnt;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_ItemIdx = (int)aobjValue[0];
		m_ItemTotalCnt = (int)aobjValue[1];
		m_UseCnt = (int)aobjValue[2];
		m_sUI.Item.SetData(new LS_Web.RES_REWARD_ITEM() { Idx = m_ItemIdx, Cnt = m_UseCnt }, null, false);
		m_sUI.MyItemCnt.text = string.Format("({0}:{1})", TDATA.GetString(243), m_ItemTotalCnt);
		base.SetData(pos, popup, cb, aobjValue);
	}


	public override void SetMsg(string Title, string Msg)
	{
		base.SetMsg(Title, Msg);
		m_sUI.Msg.text = Msg;
	}
}
