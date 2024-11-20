using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Reassembly_List_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Item_ItemSquare_RewardGroup Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Prob;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(ItemType _type, int _grade, float _prob, bool _normal) {
		m_SUI.Icon.SetData(_type, new int[] { _grade, _grade }, 0, !_normal);
		//{0}등급 {1} // 전용 : {0}
		string gname = _normal ? BaseValue.GetGroupItemName(_type) : string.Format(TDATA.GetString(248), BaseValue.GetGroupItemName(_type));
		m_SUI.Name.text = string.Format(TDATA.GetString(683), _grade, gname);
		m_SUI.Prob.text = string.Format("{0:0.###}%", _prob * 100);
	}
}
