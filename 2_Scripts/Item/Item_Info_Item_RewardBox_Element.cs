using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;

public class Item_Info_Item_RewardBox_Element : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Item_RewardList_Item Item;
		public Item_RewardList_Group ItemGroup;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Per;
		public RectTransform[] BG;
	}
	[SerializeField] SUI m_SUI;
	RES_REWARD_BASE m_Info;
	public int GetGrade { get { return m_SUI.Item.GetGrade(); } }
	public void SetData(RES_REWARD_BASE _data, Action<GameObject> _selectcb = null, bool _IsStartEff = true, string _prob = null) {
		m_Info = _data;
		m_SUI.ItemGroup.gameObject.SetActive(false);
		m_SUI.Item.SetData(_data, _selectcb, _IsStartEff);
		m_SUI.Name.text = _data.GetName();
		SetProb(_prob);
	}
	public void SetData(ItemType _type, int[] _grades, string _prob = null) {
		m_SUI.Item.gameObject.SetActive(false);
		m_SUI.ItemGroup.SetData(Res_RewardType.Item, new object[] { _type, _grades });//, m_Info.GetIdx()
		m_SUI.Name.text = string.Format("{0}{1} {2}", _grades[1], TDATA.GetString(274), BaseValue.GetGroupItemName(_type));
		SetProb(_prob);
	}
	void SetProb(string _prob = null) {
		if (string.IsNullOrEmpty(_prob)) {
			m_SUI.BG[0].sizeDelta = new Vector2(813f, m_SUI.BG[0].sizeDelta.y);
			m_SUI.BG[1].gameObject.SetActive(false);
			((RectTransform)m_SUI.Name.transform).sizeDelta = new Vector2(770f, m_SUI.BG[0].sizeDelta.y);
			m_SUI.Per.gameObject.SetActive(false);
		}
		else {
			m_SUI.BG[0].sizeDelta = new Vector2(608, m_SUI.BG[0].sizeDelta.y);
			m_SUI.BG[1].gameObject.SetActive(true);
			((RectTransform)m_SUI.Name.transform).sizeDelta = new Vector2(564f, m_SUI.BG[0].sizeDelta.y);
			m_SUI.Per.gameObject.SetActive(true);
			m_SUI.Per.text = _prob;
		}
	}
}