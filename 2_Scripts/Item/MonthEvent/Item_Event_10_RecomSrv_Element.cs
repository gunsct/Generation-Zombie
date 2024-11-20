using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Event_10_RecomSrv_Element : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Name;
		public Image CharIcon;
		public Image RewardIcon;
		public TextMeshProUGUI RewardCnt;
	}
	[SerializeField] SUI m_SUI;
	
	public void SetData(FAEventData_StageChar _info) {
		TCharacterTable cdata = TDATA.GetCharacterTable(_info.Idx);
		m_SUI.Name.text = cdata.GetCharName();
		m_SUI.CharIcon.sprite = cdata.GetPortrait();
		m_SUI.RewardIcon.sprite = TDATA.GetItemTable(_info.Reward.Idx).GetItemImg();
		m_SUI.RewardCnt.text = _info.Reward.Cnt.ToString();
	}
}
