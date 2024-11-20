using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Stage_Making_Card : ObjMng
{
#pragma warning disable 0649

	[System.Serializable]
	public struct SUI
	{
		public Image Card;
		public TextMeshProUGUI Name;
	}

	[SerializeField] SUI m_SUI;
#pragma warning restore 0649

	TStageMakingTable m_TData;
	Action<TStageMakingTable> m_SelectCB;

	public void SetData(TStageMakingTable data, Action<TStageMakingTable> selectcb = null)
	{
		m_TData = data;
		m_SelectCB = selectcb;
		TStageCardTable table = TDATA.GetStageCardTable(data.m_CardIdx);
		m_SUI.Card.sprite = table.GetImg();
		m_SUI.Name.text = table.GetName();
	}
	
	public void OnClick()
	{
		m_SelectCB?.Invoke(m_TData);
	}

}
