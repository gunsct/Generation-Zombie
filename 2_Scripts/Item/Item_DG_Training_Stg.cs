using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_DG_Training_Stg : ObjMng
{
	public enum State
	{
		Select,
		NotSelect,
		NotOpen,
		Lock
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI[] Num;
		public GameObject LockObj;
		public TextMeshProUGUI LockTxt;
		public Item_RewardList_Item[] Rewards;
	}
	[SerializeField] SUI m_SUI;
	State m_State;
	int m_Lv;
	Action<int> m_CB;
	private void OnEnable() {
		m_SUI.Anim.SetTrigger(m_State.ToString());
	}
	public void SetData(int _lv, int _limitstg, StageDifficultyType _diff, int _pos, List<RES_REWARD_BASE> _rewards, Action<int> _cb) {
		m_Lv = _lv;
		_lv++;
		m_CB = _cb;
		for(int i = 0;i< m_SUI.Num.Length;i++)
			m_SUI.Num[i].text = (m_Lv + 1).ToString();
		m_SUI.LockObj.SetActive(USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)_diff].Idx < _limitstg);
		m_SUI.LockTxt.text = string.Format("{0}-{1}", _limitstg / 100, _limitstg % 100);

		switch (_pos) {//³ëÃÊ»¡ÆÄ
			case 0: m_SUI.Anim.SetTrigger("Yellow"); break;
			case 1: m_SUI.Anim.SetTrigger("Green"); break;
			case 2: m_SUI.Anim.SetTrigger("Red"); break;
			case 3: m_SUI.Anim.SetTrigger("Blue"); break;
		}
		for(int i = 0; i < m_SUI.Rewards.Length; i++) {
			m_SUI.Rewards[i].SetData(_rewards[i], null, false);
		}
	}

	public void SetAnim(State _state) {
		m_State = _state;
		m_SUI.Anim.SetTrigger(m_State.ToString());
	}

	public void Click() {
		m_CB?.Invoke(m_Lv);
	}
}
