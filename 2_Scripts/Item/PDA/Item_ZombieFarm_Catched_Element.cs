using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using System.Linq;

public class Item_ZombieFarm_Catched_Element : ObjMng
{
	public enum State
	{
		Check,
		Set
	}
	[Serializable]
	public struct SUI
	{
		public Item_RewardList_Item Card;
		public TextMeshProUGUI Name;
		public Transform Bucket;
		public Transform Element;       //Item_PDA_RNA_Element
		public GameObject[] Panels;     //0:setbtn,1:CheckBox,2:Select

		public GameObject[] TutoObj;	//0:RNA 리스트, 1:배치 버튼
	}
	[SerializeField] SUI m_SUI;
	State m_State;
	public ZombieInfo m_Info;
	Action<bool, ZombieInfo> m_CB;
	public bool Is_Check;

	public void SetData(State _state, ZombieInfo _info, Action<bool, ZombieInfo> _cb) {
		m_State = _state;
		m_Info = _info;
		m_CB = _cb;

		m_SUI.Panels[0].SetActive(m_State != State.Check);
		m_SUI.Panels[1].SetActive(m_State == State.Check);
		m_SUI.Panels[2].SetActive(false);

		RES_REWARD_ZOMBIE zinfo = new RES_REWARD_ZOMBIE() {
			Idx = _info.m_Idx,
			UID = _info.m_UID,
			Grade = _info.m_Grade,
			Type = Res_RewardType.Zombie
		};
		m_SUI.Card.SetData(zinfo, null, false);
		m_SUI.Name.text = _info.m_TData.GetName();

		Dictionary<int, float> timerewards = m_Info.GetTimeReward();
		UTILE.Load_Prefab_List(timerewards.Count, m_SUI.Bucket, m_SUI.Element);
		for (int i = 0; i < timerewards.Count; i++) {
			KeyValuePair<int, float> reward = timerewards.ElementAt(i);
			Item_PDA_RNA_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_PDA_RNA_Element>();
			element.SetData(reward.Key, reward.Value);
		}
	}
	public void ClickSet()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_ZombieFarm_Catched_Element, 0, transform.GetSiblingIndex())) return;
		if (m_State == State.Check) {
			Is_Check = !Is_Check;
			m_SUI.Panels[2].SetActive(Is_Check);
			m_CB?.Invoke(Is_Check, m_Info);
		}
		else m_CB?.Invoke(true, m_Info);
	}
	///////튜토용
	public GameObject GetTutoObj(int _idx)
	{
		return m_SUI.TutoObj[_idx];
	}
}
