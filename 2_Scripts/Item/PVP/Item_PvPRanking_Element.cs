using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_PVPRanking_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image Portrait;
		public Text Name;
		public TextMeshProUGUI CombatPower;
	}
	[SerializeField] SUI m_SUI;

	public void SetData() {

	}
	public void ClickViewInfo() {
		//상세정보 팝업 연결
	}
}
