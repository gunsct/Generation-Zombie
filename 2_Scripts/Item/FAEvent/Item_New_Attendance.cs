using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_New_Attendance : Item_FAEvent
{
#pragma warning disable 0649

	[System.Serializable]
	struct SUI
	{
		public List<Item_Atd_Element> Items;
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
		List<FAEventData_Attendance> list = m_Data.GetRealInfo<List<FAEventData_Attendance>>();
		for (int i = 0; i < m_sUI.Items.Count; i++) {
			if (i < list.Count) {
				m_sUI.Items[i].gameObject.SetActive(true);
				m_sUI.Items[i].SetData(list[i].Reward[0], list[i].No);

				int add = m_Data.IsReward() ? 1 : 0;
				bool IsNow = false;
				bool IsGet = true;
				if (list[i].No == m_Data.Values[0] + add) {
					IsNow = true;
					IsGet = add == 0;
				}
				else if (list[i].No > m_Data.Values[0] + add) IsGet = false;
				else PlayEffSound(SND_IDX.SFX_0170);
				m_sUI.Items[i].SetState(IsNow, IsGet);
			}
			else {
				m_sUI.Items[i].gameObject.SetActive(false);
			}
		}
	}

	public override void CheckItemAction(Action CB)
	{
		List<FAEventData_Attendance> list = m_Data.GetRealInfo<List<FAEventData_Attendance>>();
		for(int i = 0; i < list.Count; i++)
		{
			if(list[i].No == m_Data.Values[0])
			{
				m_sUI.Items[i].StartGetAction(CB);
				break;
			}
		}
		
	}
}
