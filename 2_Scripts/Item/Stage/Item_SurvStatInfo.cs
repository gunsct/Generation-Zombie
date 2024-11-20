using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Item_SurvStatInfo : ObjMng
{
	[Serializable]
	public struct SUUI
	{
		public Sprite MatImgSP;
		public int MatNameIdxs;
		public Sprite[] UtileImgsSP;
		public int[] UtileNameIdxs;
	}
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Amount;
		public Image StatIcon;
		public TextMeshProUGUI StatName;
		public Image DebuffIcon;
		public TextMeshProUGUI DebuffName;
		public TextMeshProUGUI DebuffDesc;

		public Sprite[] StatIconSP;
		public int[] StatNameIdxs;
		public Sprite[] DebuffIconSP;
		public int[] DebuffNameIdxs;
		public int[] DebuffDescIdxs;

		public Image MatImg;
		public Image[] UtileImgs;
		public TextMeshProUGUI MatName;
		public TextMeshProUGUI[] UtileNames;
		public SUUI[] MatUtiles;
	}
	[SerializeField] SUI m_SUI;

	/// <summary> 최초 현재값과 최대값 세팅 </summary>
	public void SetData(StatType _type, float _crnt, float _max) {
		m_SUI.Amount.text = string.Format("{0} / {1} ({2}%)", Mathf.RoundToInt(_crnt), Mathf.RoundToInt(_max), Mathf.RoundToInt(_crnt / _max * 100));
		int pos = (int)_type;
		m_SUI.StatIcon.sprite = m_SUI.StatIconSP[pos];
		m_SUI.StatName.text = TDATA.GetString(ToolData.StringTalbe.UI, m_SUI.StatNameIdxs[pos]);
		m_SUI.DebuffIcon.sprite = m_SUI.DebuffIconSP[pos];
		m_SUI.DebuffName.text = TDATA.GetString(ToolData.StringTalbe.UI, m_SUI.DebuffNameIdxs[pos]);
		m_SUI.DebuffDesc.text = TDATA.GetString(ToolData.StringTalbe.UI, m_SUI.DebuffDescIdxs[pos]);
		m_SUI.MatImg.sprite = m_SUI.MatUtiles[pos].MatImgSP;
		m_SUI.MatName.text = TDATA.GetString(ToolData.StringTalbe.Etc, m_SUI.MatUtiles[pos].MatNameIdxs);
		for (int i = 0; i < 3; i++) {
			m_SUI.UtileImgs[i].sprite = m_SUI.MatUtiles[pos].UtileImgsSP[i];
			m_SUI.UtileNames[i].text = TDATA.GetString(ToolData.StringTalbe.Etc, m_SUI.MatUtiles[pos].UtileNameIdxs[i]);
		}
	}
}

