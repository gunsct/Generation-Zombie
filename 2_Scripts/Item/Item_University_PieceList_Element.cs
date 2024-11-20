using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;

public class Item_University_PieceList_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Item_RewardList_Item Reward;
		public TextMeshProUGUI Name;
	}
	[SerializeField] SUI m_SUI;

	public int GetGrade { get { return m_SUI.Reward.GetGrade(); } }
	public void SetData(RES_REWARD_BASE _data, Action<GameObject> _selectcb = null, bool _IsStartEff = true) {
		m_SUI.Reward.SetData(_data, _selectcb, _IsStartEff);
		m_SUI.Name.text = _data.GetName();
	}
}
