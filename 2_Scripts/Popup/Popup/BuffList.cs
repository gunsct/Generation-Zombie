using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffList : PopupBase
{
	[System.Serializable]
	public struct SUI
	{
		public GameObject Card;
		public Transform Parent;
		public TextMeshProUGUI Count;
	}
	[SerializeField]
	SUI m_SUI;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_SUI.Count.text = STAGE_USERINFO.m_Buffs.Count.ToString();
		for (int i = 0; i < STAGE_USERINFO.m_Buffs.Count; i++) {
			StageBuff buff = STAGE_USERINFO.m_Buffs[i];
			switch (buff.m_Kind)
			{
			case EStageBuffKind.Stage:
				TStageCardTable tstageatble = TDATA.GetStageCardTable(buff.m_Idx);
				Utile_Class.Instantiate(m_SUI.Card, m_SUI.Parent).GetComponent<Item_Default_Card>().SetData(tstageatble.GetImg(), tstageatble.GetName());
				break;
			case EStageBuffKind.Synergy:
				TSynergyTable tsynergytable = TDATA.GetSynergyTable((JobType)buff.m_Idx);
				Utile_Class.Instantiate(m_SUI.Card, m_SUI.Parent).GetComponent<Item_Default_Card>().SetData(tsynergytable.GetImg(), tsynergytable.GetName());
				break;
			}
		}
	}
}
