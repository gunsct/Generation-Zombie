using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;
using UnityEngine.UI;

[System.Serializable] public class DicPass_Element_UI : SerializableDictionary<Item_Pass_Element.Pos, Item_Pass_Element.SRewardUI> { }
public class Item_Pass_Element : ObjMng
{
	public enum AniName
	{
		Now = 0,
		Opened,
		NotOpen
	}
	public enum Pos
	{
		Normal = 0,
		VIP
	}
	[Serializable]
	public struct SRewardUI
	{
		public Item_RewardList_Item Reward;
		public GameObject Active;
		public GameObject Btn;
		public GameObject[] Get;
		public GameObject[] Lock;
		public GameObject[] Eff;
	}

	[Serializable]
    public struct SUI
	{
		public Animator Ani;
		public DicPass_Element_UI UI;
		public TextMeshProUGUI LV;
		public GameObject LvUpBtn;
	}
	[SerializeField] SUI m_SUI;
	MissionData[] Infos = new MissionData[2];
	bool IsBuy;
	int m_MaxLv;
	Action<Item_Pass_Element, MissionData, int> GetCB;
	Action<int> m_LvUpCB;
	AniName Ani = AniName.NotOpen;
	bool IsStart = false;
	TMissionTable[] m_TDatas = new TMissionTable[2];

	void Start()
	{
		IsStart = true;
		SetState(Ani);
	}

	public void SetData(MissionData Befor, MissionData Now, bool IsBuy, int _maxlv, Action<Item_Pass_Element, MissionData, int> CB, Action<int> _lvupcb) {
		Infos[0] = Befor;
		Infos[1] = Now;
		if (Infos[0] != null) m_TDatas[0] = TDATA.GetMissionTable(Infos[0].Idx);
		if (Infos[1] != null) m_TDatas[1] = TDATA.GetMissionTable(Infos[1].Idx);
		this.IsBuy = IsBuy;
		m_MaxLv = _maxlv;
		GetCB = CB;
		m_LvUpCB = _lvupcb;
		SetUI();
	}

	public void SetUI()
	{
		var data = Infos[1];
		var tdata = TDATA.GetMissionTable(data.Idx);
		int pos = (int)Pos.Normal;
		if (tdata.m_Rewards[pos] == null)
		{
			m_SUI.UI[Pos.Normal].Active.SetActive(false);
			m_SUI.UI[Pos.Normal].Btn.SetActive(false);
		}
		else
		{
			m_SUI.UI[Pos.Normal].Active.SetActive(true);
			m_SUI.UI[Pos.Normal].Reward.SetData(tdata.m_Rewards[pos].Get_RES_REWARD_BASE(), null, false);
			m_SUI.UI[Pos.Normal].Get[0].SetActive(data.State[pos] != RewardState.Idle);
			m_SUI.UI[Pos.Normal].Get[1].SetActive(false);
		}
		
		pos = (int)Pos.VIP;
		if(tdata.m_Rewards[pos] == null)
		{
			m_SUI.UI[Pos.VIP].Active.SetActive(false);
			m_SUI.UI[Pos.VIP].Btn.SetActive(false);
		}
		else
		{
			m_SUI.UI[Pos.VIP].Active.SetActive(true);
			m_SUI.UI[Pos.VIP].Reward.SetData(tdata.m_Rewards[pos].Get_RES_REWARD_BASE(), null, false);
			m_SUI.UI[Pos.VIP].Get[0].SetActive(IsBuy && data.State[pos] != RewardState.Idle);
			m_SUI.UI[Pos.VIP].Get[1].SetActive(false);
			m_SUI.UI[Pos.VIP].Eff[0].SetActive(data.State[pos] == RewardState.Idle);
			m_SUI.UI[Pos.VIP].Eff[1].SetActive(data.State[pos] == RewardState.Idle);
		}

		m_SUI.LV.text = tdata.m_LinkIdx.ToString();
	}

	public void SetState(AniName ani)
	{
		Ani = ani;
		if (!IsStart) return;
		var data = Infos[1];
		var tdata = TDATA.GetMissionTable(data.Idx);
		bool isSuccess = data.IS_Complete();
		m_SUI.LvUpBtn.SetActive(Ani == AniName.Now && !isSuccess);
		m_SUI.Ani.SetTrigger(ani.ToString());
	}

	public void SetUnLock(bool Lock, bool Eff = false)
	{
		var data = Infos[1];
		var tdata = TDATA.GetMissionTable(data.Idx);
		bool isSuccess = data.IS_Complete();
		m_SUI.UI[Pos.Normal].Lock[0].SetActive(Lock || Ani != AniName.Opened);
		m_SUI.UI[Pos.Normal].Btn.SetActive(!Lock && data.State[0] == RewardState.Idle && isSuccess);
		m_SUI.UI[Pos.Normal].Get[1].SetActive(m_SUI.UI[Pos.Normal].Btn.activeSelf);

		if (!Lock) Lock = !IsBuy;

		m_SUI.UI[Pos.VIP].Lock[0].SetActive(Lock || Ani != AniName.Opened);
		m_SUI.UI[Pos.VIP].Lock[1].SetActive(Eff && !Lock || Ani != AniName.Opened);
		m_SUI.UI[Pos.VIP].Btn.SetActive(!Lock && data.State[1] == RewardState.Idle && isSuccess);
		m_SUI.UI[Pos.VIP].Get[1].SetActive(m_SUI.UI[Pos.VIP].Btn.activeSelf);
	}


	public void OnReward(int pos)
	{
		GetCB?.Invoke(this, Infos[1], pos);
	}

	public int GetLV()
	{
		return Infos[1].m_TData.m_LinkIdx;
	}
	public void ClickLvUp() {
		if (m_TDatas[1] == null) return;
		m_LvUpCB?.Invoke(m_TDatas[1].m_LinkIdx);
	}
}
