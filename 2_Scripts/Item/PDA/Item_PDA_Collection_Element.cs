using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PDA_Collection_Element : ObjMng
{
	public enum Ani
	{
		Progress = 0,
		Progress_Lv0,
		Complete,
		LvMax
	}
#pragma warning disable 0649
	[System.Serializable]
	struct SColletion
	{
		public GameObject[] Active;
		public Item_RewardList_Item Item;
	}

	[System.Serializable]
	struct SUI
	{
		public Animator Ani;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Reward;

		public SColletion[] Collections;
	}

	[SerializeField] SUI m_SUI;
#pragma warning restore 0649
	public TCollectionTable m_Info;
	Action<Item_PDA_Collection_Element, int> m_ClickCB;
	Ani m_State;
	bool m_IsFirstOpen;
	private void OnEnable() {
		m_SUI.Ani.SetTrigger(m_State.ToString());
	}
	public bool SetData(TCollectionTable info, bool _comp, Action<Item_PDA_Collection_Element, int> ClickCB)
	{
		m_Info = info;
		m_ClickCB = ClickCB;

		bool isSuc = USERINFO.m_Collection.IsSuccess(info);
		int MaxLV = USERINFO.m_Collection.GetMaxLV(info.m_Idx);
		bool IsMax = info.m_LV >= MaxLV;

		m_SUI.LV.text = string.Format("Lv. {0}", info.m_LV + 1);
		m_SUI.Name.text = info.GetName();
		TCollectionTable next = TDATA.GetCollectionTable(m_Info.m_Idx, m_Info.m_LV + 1);
		float val = next.m_Stat.m_Value - (m_Info.m_Stat != null ? m_Info.m_Stat.m_Value : 0);
		m_SUI.Reward.text = string.Format(TDATA.GetString(1033), TDATA.GetStatString(next.m_Stat.m_Type), next.m_Stat.m_Type == StatType.Critical ? string.Format("{0:0.0}%", val * 100f) : Mathf.RoundToInt(val).ToString());
		m_State = Ani.Progress;
		if (IsMax || _comp) m_State = Ani.LvMax;
		else if (isSuc) m_State = Ani.Complete;
		//else if (info.m_LV == 0) m_State = Ani.Progress_Lv0;

		if (IsMax) info = TDATA.GetCollectionTable(info.m_Idx, info.m_LV - 1);

		for (int i = 0; i < m_SUI.Collections.Length; i++)
		{
			if (info.m_Colloets.Count > i)
			{
				m_SUI.Collections[i].Active[0].SetActive(true);
				int idx = info.m_Colloets[i];
				bool iscollect = !IsMax && USERINFO.m_Collection.GetCollectionValue(info.m_Type, idx) >= info.m_Grade;
				m_SUI.Collections[i].Active[1].SetActive(iscollect);
				m_SUI.Collections[i].Active[2].SetActive(!iscollect);
				RES_REWARD_BASE res;
				switch (info.m_Type)
				{
				case CollectionType.Zombie:
					res = new RES_REWARD_ZOMBIE() { Idx = idx, Grade = TDATA.GetZombieTable(idx).m_Grade };
					break;
				case CollectionType.DNA:
					res = new RES_REWARD_DNA() { Idx = idx, Grade = info.m_Grade };
					break;
				default:
					res = new RES_REWARD_CHAR() { Idx = idx, Grade = info.m_Grade };
					break;
				}
				m_SUI.Collections[i].Item.SetData(res, null, false);
			}
			else m_SUI.Collections[i].Active[0].SetActive(false);
		}
		m_SUI.Ani.SetTrigger(m_State.ToString());
		return m_State == Ani.Complete;
	}

	public void GetReward() {
		if (!USERINFO.m_Collection.IsSuccess(m_Info) || m_State != Ani.Complete) return;
		m_ClickCB?.Invoke(this, 0);
	}
}
