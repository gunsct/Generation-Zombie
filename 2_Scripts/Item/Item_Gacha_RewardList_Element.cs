using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_Gacha_RewardList_Element : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Item_RewardList_Item Item;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Prob;
		public GameObject Pickup;
	}
	[SerializeField] SUI m_SUI;
	RES_REWARD_BASE m_Info;

	public void SetData(RES_REWARD_BASE _item, float _prob, bool _pickup = false) {
		m_Info = _item;
		m_SUI.Item.SetData(_item, null, false);
		m_SUI.Name.text = m_Info.Type == Res_RewardType.Item ? string.Format(TDATA.GetString(683), m_Info.GetGrade(), _item.GetName()) : _item.GetName();
		m_SUI.Prob.text = string.Format("{0:0.000}%", _prob * 100f);
		m_SUI.Pickup.SetActive(_pickup);
		m_SUI.Prob.color = _pickup ? Utile_Class.GetCodeColor("#FFCC10") : Utile_Class.GetCodeColor("#BEBEBA");
	}
}
