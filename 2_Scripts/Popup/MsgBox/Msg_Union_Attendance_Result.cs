using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Msg_Union_Attendance_Result : MsgBoxBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Item_RewardList_Item[] Items;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	List<RES_REWARD_BASE> m_Rewards;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_Rewards = (List<RES_REWARD_BASE>)aobjValue[0];
		base.SetData(pos, popup, cb, aobjValue);
		PlayEffSound(SND_IDX.SFX_0170);
	}

	public override void SetUI()
	{
		base.SetUI();

		for(int i = 0, iMax = m_sUI.Items.Length; i < iMax; i++)
		{

			if (m_Rewards.Count > i)
			{
				m_sUI.Items[i].gameObject.SetActive(true);
				m_sUI.Items[i].SetData(m_Rewards[i]);
			}
			else m_sUI.Items[i].gameObject.SetActive(false);
		}
	}
}
