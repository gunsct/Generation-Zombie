using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Item_Help_Info_Char_DNASet_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Sprite[] BGImg;
		public Color[] FrameColors;
		public Color[] TitleColors;
		public Image GradeBG;
		public Image FrameBG;
		public Image TitleBg;
		public TextMeshProUGUI TitleTxt;
		public Item_Info_Char_DNAStat_Sub[] Stats;
	}
	[SerializeField] SUI m_SUI;
	public void SetData(DNABGType _type, List<TDNASetEffectTable> _tdatas) {
		m_SUI.GradeBG.sprite = m_SUI.BGImg[(int)_type - 1];
		m_SUI.FrameBG.color = m_SUI.FrameColors[(int)_type - 1];
		m_SUI.TitleBg.color = m_SUI.TitleColors[(int)_type - 1];
		m_SUI.TitleTxt.text = string.Format(TDATA.GetString(937), string.Format("{0} DNA", BaseValue.GetDNAColorName(_type)));
		Dictionary<StatType, List<float>> stats = new Dictionary<StatType, List<float>>();
		for(int i = 0;i< _tdatas.Count; i++) {
			if (!stats.ContainsKey(_tdatas[i].m_SetFXType)) stats.Add(_tdatas[i].m_SetFXType, new List<float>());
			stats[_tdatas[i].m_SetFXType].Add(_tdatas[i].m_SetFXVal);
		}
		m_SUI.Stats[0].gameObject.SetActive(false);
		m_SUI.Stats[1].gameObject.SetActive(false);
		for (int i = 0; i < stats.Count; i++) {
			m_SUI.Stats[i].gameObject.SetActive(true);
			m_SUI.Stats[i].SetData(stats.ElementAt(i).Key, stats.ElementAt(i).Value);
		}
	}
}
