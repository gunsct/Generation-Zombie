using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;

public class Item_Event_10_Stage_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image[] StageNum;
		public Image[] RewardIcon;
		public TextMeshProUGUI[] RewardCnt;
		public GameObject[] RewardGroup;
		public TextMeshProUGUI LockTxt;
		public GameObject[] RemdSynergyGroup;
		public Image[] RemdSynergy;
	}
	[SerializeField] SUI m_SUI;
	int m_Pos;
	Action<int> m_CB;
	public TStageTable m_TData;
	public bool IS_Lock;

	public void SetData(int _pos, TStageTable _tdata, int _limitstg, Action<int> _cb, int _nowpos) {
		m_Pos = _pos;
		m_CB = _cb;
		m_TData = _tdata;
		bool limit = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < _limitstg;
		IS_Lock = m_Pos > _nowpos || limit;

		m_SUI.Anim.SetTrigger(limit ? "Lock" : m_Pos == _nowpos ? "On" : "Off");
		m_SUI.StageNum[0].sprite = UTILE.LoadImg(string.Format("Font/Event/Font_Event_10_Number_{0}", m_Pos / 10), "png");
		m_SUI.StageNum[1].sprite = UTILE.LoadImg(string.Format("Font/Event/Font_Event_10_Number_{0}", m_Pos % 10), "png");

		for (int i = 0; i < m_SUI.RewardGroup.Length; i++) {
			if (i < m_TData.m_ClearReward.Count) {
				List<RES_REWARD_BASE> rewards = TDATA.GetGachaItemList(TDATA.GetItemTable(m_TData.m_ClearReward[i].m_Idx));
				rewards.Sort((RES_REWARD_BASE _befor, RES_REWARD_BASE _after) => {
					return ((RES_REWARD_ITEM)_befor).Cnt.CompareTo(((RES_REWARD_ITEM)_after).Cnt);
				});
				m_SUI.RewardIcon[i].sprite = TDATA.GetItemTable(rewards[0].GetIdx()).GetItemImg();
				m_SUI.RewardCnt[i].text = string.Format("{0}~{1}", ((RES_REWARD_ITEM)rewards[0]).Cnt, ((RES_REWARD_ITEM)rewards[rewards.Count - 1]).Cnt);
				m_SUI.RewardGroup[i].SetActive(true);
			}
			else m_SUI.RewardGroup[i].SetActive(false);
		}
		for(int i = 0; i < m_SUI.RemdSynergyGroup.Length; i++) {
			if (i < m_TData.m_RecommendJob.Count) {
				TSynergyTable table = TDATA.GetSynergyTable(m_TData.m_RecommendJob[i]);
				m_SUI.RemdSynergy[i].sprite = table.GetIcon();
				m_SUI.RemdSynergyGroup[i].SetActive(true);
			}
			else m_SUI.RemdSynergyGroup[i].SetActive(false);
		}
		m_SUI.LockTxt.text = string.Format(TDATA.GetString(163), _limitstg / 100, _limitstg % 100);
	}
	public void Click_DeckSetting() {
		if (IS_Lock) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(2008));
			return;
		}
		m_CB?.Invoke(m_Pos);
	}
}
