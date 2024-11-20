using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;

public class Item_StgReplay_Stage_Element : ObjMng
{
	[Serializable]
	public struct SIUI {
		public GameObject Active;
		public Image Icon;
		public TextMeshProUGUI Name;
	}
	[Serializable]
	public struct SUI {
		public SIUI[] Infos;
		public Item_RewardList_Item Reward;
		public TextMeshProUGUI Energy;
		public GameObject GoBtn;
	}
	[SerializeField] SUI m_SUI;
	int m_EnergyCnt = 1;
	Action<UserInfo.StageIdx> m_PassCB;
	Action<UserInfo.StageIdx> m_PlayCB;
	UserInfo.StageIdx m_Info;

	public GameObject GetRewardCard { get { return m_SUI.Reward.gameObject; } }
	public GameObject GetGoBtn { get { return m_SUI.GoBtn; } }
	public void SetData(UserInfo.StageIdx _info, Action<UserInfo.StageIdx> _passcb, Action<UserInfo.StageIdx> _playcb) {
		m_Info = _info;
		m_PassCB = _passcb;
		m_PlayCB = _playcb;

		TStageTable tdata = TDATA.GetStageTable(m_Info.Idx);
		m_EnergyCnt = tdata.m_Energy;
		for (int i = 0;i< m_SUI.Infos.Length; i++) {
			if(i < tdata.m_Gimmick.Count) {
				m_SUI.Infos[i].Icon.sprite = tdata.GetGimmickImg(i);
				m_SUI.Infos[i].Name.text = tdata.GetGimminkName(i);
				m_SUI.Infos[i].Active.SetActive(true);
			}
			else m_SUI.Infos[i].Active.SetActive(false);
		}

		m_SUI.Energy.text = string.Format("<size=140%>-</size> {0}", m_EnergyCnt);
		List<RES_REWARD_BASE> rewards = MAIN.GetRewardData(tdata.m_ReplayReward[0].m_Kind, tdata.m_ReplayReward[0].m_Idx, tdata.m_ReplayReward[0].m_Count, false);
		m_SUI.Reward.SetData(rewards[0]);
	}
	//소탕
	public void Click_Pass() {
		if (TUTO.IsTutoPlay()) return;
		m_PassCB?.Invoke(m_Info);
	}
	//플레이
	public void Click_Play() {
		if (TUTO.IsTutoPlay()) return;
		m_PlayCB?.Invoke(m_Info);
	}
}
