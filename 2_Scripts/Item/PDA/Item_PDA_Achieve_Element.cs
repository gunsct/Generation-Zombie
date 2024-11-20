using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_PDA_Achieve_Element : ObjMng
{
	public enum State
	{
		/// <summary> 진행중 </summary>
		Play = 0,
		/// <summary> 받기 </summary>
		End
	}
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Animator Ani;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Name;
		public Image Gauge;
		public TextMeshProUGUI GaugeValue;
		public Item_RewardItem_Card Item;
	}

	[SerializeField] SUI m_SUI;
#pragma warning restore 0649
	public TAchievementTable m_Info;
	Action<Item_PDA_Achieve_Element, int> m_ClickCB;
	State m_State;
	bool m_IsFirstOpen;
	public bool SetData(TAchievementTable info, Action<Item_PDA_Achieve_Element, int> ClickCB)
	{
		m_Info = info;
		m_ClickCB = ClickCB;

		AchieveData data = USERINFO.m_Achieve.GetAchieveData(info);
		long Cnt = 0;
		if(data != null) Cnt = data.Cnt;
		m_SUI.LV.text = string.Format("Lv.{0}", info.m_LV);
		m_SUI.Name.text = info.GetName();

		m_SUI.Gauge.fillAmount = Mathf.Min((float)((double)Cnt / (double)info.m_Values[1]), 1f);
		m_SUI.GaugeValue.text = string.Format("{0} / {1}", Cnt, info.m_Values[1]);
		m_SUI.Item.SetData(info.m_Reward.Idx, info.m_Reward.Cnt, info.m_Reward.LV, info.m_Reward.Grade);
		if (Cnt >= info.m_Values[1])
		{
			// 완료상태
			m_State = State.End;
			m_SUI.Ani.SetTrigger("Complete");
		}
		else
		{
			m_State = State.Play;
			m_SUI.Ani.SetTrigger("Progress");
		}

		return m_State == State.End;
	}

	public void GetReward() {
		if (m_State == State.Play) return;
		m_ClickCB?.Invoke(this, 0);
	}
}
