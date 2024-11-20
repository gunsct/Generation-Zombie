using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_Event_10_Mission_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Face;
		public TextMeshProUGUI[] Desc;			//목표등급, 현재등급
		public Item_RewardList_Item Reward;
	}
	[SerializeField] SUI m_SUI;
	MissionData m_Info;
	Action<MissionData> m_CB;

	public void SetData(MissionData _info, Action<MissionData> _cb) {
		m_Info = _info;
		m_CB = _cb;

		TMissionTable.TMissionCheck check = m_Info.m_TData.m_Check[0];
		TCharacterTable tdata = TDATA.GetCharacterTable(check.m_Val[1]);
		CharInfo cinfo = USERINFO.GetChar(check.m_Val[1]);
		m_SUI.Face.sprite = tdata.GetPortrait();
		m_SUI.Desc[0].text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, _info.m_TData.m_Name), tdata.GetCharName(), check.m_Val[0]);
		m_SUI.Desc[1].text = cinfo == null ? TDATA.GetString(2032) : (check.m_Type == MissionType.CharLevel ? string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, _info.m_TData.m_Desc), cinfo.m_LV)
																	: string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, _info.m_TData.m_Desc), cinfo.m_Grade));

		PostReward cleardata = m_Info.m_TData.m_Rewards[0];
		List<RES_REWARD_BASE> reward = MAIN.GetRewardData(cleardata.Kind, cleardata.Idx, cleardata.Cnt);
		m_SUI.Reward.SetData(reward[0], null, false);

		switch (m_Info.State[0]) {
			case RewardState.Idle:
				m_SUI.Anim.SetTrigger(m_Info.IS_Complete() ? "Active" : "Deactive");
				break;
			case RewardState.Get:
				m_SUI.Anim.SetTrigger("Complete");
				break;
			case RewardState.None:
				m_SUI.Anim.SetTrigger("Deactive");
				break;
		}
	}
	public void Click_GetReward() {
		m_CB?.Invoke(m_Info);
	}
}
