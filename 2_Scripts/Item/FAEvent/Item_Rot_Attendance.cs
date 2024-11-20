using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_Rot_Attendance : Item_FAEvent
{
#pragma warning disable 0649
	[System.Serializable]
	struct SContinueNumUI
	{
		public RectTransform rectTransform;
		[ReName("Off", "On")]
		public GameObject[] Active;
	}
	[System.Serializable]
	struct SContinueNumRewardUI
	{
		public RectTransform rectTransform;
		public Item_RewardList_Item Reward;
		[ReName("Grow", "GetMark")]
		public GameObject[] Actives;
	}
	[System.Serializable]
	struct SUI
	{
		public List<Item_Atd_Element> AttItems;
		[ReName("1", "2", "3", "4", "5", "6", "7", "8", "9", "10")]
		public List<SContinueNumUI> ContinueNum;
		[ReName("5", "10")]
		public List<SContinueNumRewardUI> ContinueReward;
		public TextMeshProUGUI DayTxt;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	public override void SetData(MyFAEvent data)
	{
		base.SetData(data);
		SetUI();
	}

	void SetUI()
	{
		DateTime today = DateTime.Today;
		m_sUI.DayTxt.text = string.Format("{0} / {1} / {2}", today.Year, today.Month, today.Day);

		FAEventData_Rot_Attendance data = m_Data.GetRealInfo<FAEventData_Rot_Attendance>();

		int add = m_Data.IsReward() ? 1 : 0;
		// 출석 보상
		for (int i = 0; i < m_sUI.AttItems.Count; i++)
		{
			if (i < data.Lists.Count)
			{
				m_sUI.AttItems[i].gameObject.SetActive(true);
				m_sUI.AttItems[i].SetData(data.Lists[i].Reward[0], data.Lists[i].No, true);
				bool IsNow = false;
				bool IsGet = true;
				if (data.Lists[i].No == m_Data.Values[0] + add)
				{
					IsNow = true;
					IsGet = add == 0;
				}
				else if (data.Lists[i].No > m_Data.Values[0] + add) IsGet = false;
				else PlayEffSound(SND_IDX.SFX_0170);
				m_sUI.AttItems[i].SetState(IsNow, IsGet);
			}
			else
			{
				m_sUI.AttItems[i].gameObject.SetActive(false);
			}
		}

		// 연속 출석 보상
		int nowday = (int)m_Data.Values[1] + (m_Data.IsReward() ? 1 : 0);
		for (int i = 0, offset = 1; i < m_sUI.ContinueNum.Count; i++, offset++)
		{
			bool IsNow = offset <= nowday;
			m_sUI.ContinueNum[i].Active[0].SetActive(!IsNow);
			m_sUI.ContinueNum[i].Active[1].SetActive(IsNow);
		}

		// 보상 셋팅
		for(int i = 0; i < m_sUI.ContinueReward.Count; i++)
		{
			if(i < data.Continue.Count)
			{
				var reward = data.Continue[i].Reward[0];
				m_sUI.ContinueReward[i].rectTransform.gameObject.SetActive(true);
				m_sUI.ContinueReward[i].Reward.SetData(reward.Get_RES_REWARD_BASE(), IsStartEff:false);
				bool IsMark = data.Continue[i].No < nowday || (data.Continue[i].No == nowday && !m_Data.IsReward());
				m_sUI.ContinueReward[i].Actives[1].SetActive(IsMark);
			}
			else
			{
				m_sUI.ContinueReward[i].rectTransform.gameObject.SetActive(false);
			}
		}
	}

	public override void CheckItemAction(Action CB)
	{
		FAEventData_Rot_Attendance data = m_Data.GetRealInfo<FAEventData_Rot_Attendance>();
		for (int i = 0; i < data.Lists.Count; i++)
		{
			if (data.Lists[i].No == m_Data.Values[0])
			{
				m_sUI.AttItems[i].StartGetAction(CB);
				break;
			}
		}

		for (int i = 0; i < m_sUI.ContinueReward.Count; i++)
		{
			if (data.Continue[i].No == m_Data.Values[1])
			{
				m_sUI.ContinueReward[i].Actives[1].SetActive(true);
				break;
			}
		}
	}
}
