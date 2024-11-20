using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_Event_11_Stage_Element : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image[] StageNum;
		public TextMeshProUGUI RewardCnt;
		public GameObject[] RemdSynergyGroup;
		public Image[] RemdSynergy;
		public TextMeshProUGUI LockTxt;
	}
	[SerializeField] SUI m_SUI;
	int m_Pos;
	TStageTable m_TData;
	Action<int> m_CB;
	public bool IS_Lock;
	public void SetData(int _pos, TStageTable _tdata, int _limitstg, Action<int> _cb, int _nowpos) {//
		m_Pos = _pos;
		m_TData = _tdata;
		bool limit = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < _limitstg;
		IS_Lock = m_Pos > _nowpos || limit;
		m_CB = _cb;

		SetAnim(limit ? "Lock" : m_Pos == _nowpos ? "On" : "Off");
		m_SUI.StageNum[0].sprite = UTILE.LoadImg(string.Format("Font/Event/Font_Event_10_Number_{0}", m_Pos / 10), "png");
		m_SUI.StageNum[1].sprite = UTILE.LoadImg(string.Format("Font/Event/Font_Event_10_Number_{0}", m_Pos % 10), "png");
		m_SUI.LockTxt.text = string.Format(TDATA.GetString(163), _limitstg / 100, _limitstg % 100);

		//보상
		var reward = m_TData.m_ClearReward.Find(o => o.m_Idx == BaseValue.EVENT_11_ITEMIDX);
		m_SUI.RewardCnt.text = string.Format("x{0}", reward == null ? 0 : reward.m_Count);

		//추천 시너지
		for (int i = 0; i < m_SUI.RemdSynergyGroup.Length; i++) {
			if (i < m_TData.m_RecommendJob.Count) {
				TSynergyTable table = TDATA.GetSynergyTable(m_TData.m_RecommendJob[i]);
				m_SUI.RemdSynergy[i].sprite = table.GetIcon();
				m_SUI.RemdSynergyGroup[i].SetActive(true);
			}
			else m_SUI.RemdSynergyGroup[i].SetActive(false);
		}
	}
	public void Click_DeckSetting() {
		if (IS_Lock) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(2008));
			return;
		}
		m_CB?.Invoke(m_Pos);
	}
	public void SetAnim(string _trig) {
		m_SUI.Anim.SetTrigger(_trig);
	}
}
