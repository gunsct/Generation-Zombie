using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;

public class Item_DNAList_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Item_RewardList_Item Card;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Desc;
		public TextMeshProUGUI MakePerTxt;
		public GameObject MakePerGroup;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(int _idx, float _makeper) {
		TDnaTable tdata = TDATA.GetDnaTable(_idx);
		RES_REWARD_DNA dna = new RES_REWARD_DNA() {
			UID = 0,
			Idx = _idx,
			Grade = tdata.m_Grade,
			Lv = 1,
			Type = Res_RewardType.DNA
		};
		m_SUI.Card.SetData(dna, null, false);
		m_SUI.Name.text = tdata.GetName();
		m_SUI.Desc.text = tdata.GetDesc();
		m_SUI.MakePerGroup.SetActive(_makeper > 0f);
		m_SUI.MakePerTxt.text = string.Format("{0:0.00}%", _makeper * 100f);
	}
}
