using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Info_Char_DNAStat_Set : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image BG;
		public Color[] BGcolors;
		public TextMeshProUGUI Name;// 색 DNA (n/n)
		public Item_Info_Char_DNAStat_None NoInfo;
		public Transform SubBucket;
		public Transform SubElement;//Item_Info_Char_DNAStat_Sub
	}
	[SerializeField] SUI m_SUI;

	public void SetData(DNABGType _color, int _cnt = 0) {
		TDNASetEffectTable tdata = TDATA.GetDNASetFXTable(_color, _cnt);

		m_SUI.NoInfo.gameObject.SetActive(tdata == null);
		m_SUI.SubBucket.gameObject.SetActive(tdata != null);
		if (tdata != null) {
			int needcnt = 0;
			List<TDNASetEffectTable> datas = TDATA.GetDNASetFXTables(_color);
			TDNASetEffectTable nextdata = datas.Find(o => o.m_Idx > tdata.m_Idx);
			if (_cnt >= tdata.m_Cnt && nextdata != null) needcnt = nextdata.m_Cnt;
			else needcnt = tdata.m_Cnt;
			m_SUI.Name.text = string.Format(TDATA.GetString(937), string.Format("{0} DNA ({1}/{2})", BaseValue.GetDNAColorName(_color), _cnt, needcnt));

			List<TDNASetEffectTable> setdatas = datas.FindAll(o => o.m_Idx <= tdata.m_Idx);
			UTILE.Load_Prefab_List(setdatas.Count, m_SUI.SubBucket, m_SUI.SubElement);
			for(int i = 0; i < setdatas.Count; i++) {
				m_SUI.SubBucket.GetChild(i).GetComponent<Item_Info_Char_DNAStat_Sub>().SetData(setdatas[i].m_SetFXType, setdatas[i].m_SetFXVal);
			}
		}
		else {
			if (_color == DNABGType.None)
				m_SUI.Name.text = string.Format(TDATA.GetString(937), BaseValue.GetDNAColorName(_color));
			else {
				tdata = TDATA.GetDNASetFXTables(_color)[0];
				m_SUI.Name.text = string.Format(TDATA.GetString(937), string.Format("{0} DNA ({1}/{2})", BaseValue.GetDNAColorName(_color), _cnt, tdata.m_Cnt));
			}
			m_SUI.NoInfo.SetData(false, TDATA.GetString(939));
		}
		m_SUI.BG.color = m_SUI.BGcolors[(int)_color];
	}

	public void SetData_Info(DNABGType _color, int _cnt = 0, int pos = 0)
	{
		List<TDNASetEffectTable> datas = TDATA.GetDNASetFXTables(_color);
		if (datas.Count <= pos) gameObject.SetActive(false);
		else
		{
			gameObject.SetActive(true);
			TDNASetEffectTable tdata = datas[pos];
			m_SUI.Name.text = string.Format(TDATA.GetString(937), string.Format("{0} DNA ({1}/{2})", BaseValue.GetDNAColorName(_color), Math.Clamp(_cnt, 0, tdata.m_Cnt), tdata.m_Cnt));

			UTILE.Load_Prefab_List(1, m_SUI.SubBucket, m_SUI.SubElement);
			m_SUI.SubBucket.GetChild(0).GetComponent<Item_Info_Char_DNAStat_Sub>().SetData(tdata.m_SetFXType, tdata.m_SetFXVal);
		}
		m_SUI.BG.color = m_SUI.BGcolors[(int)_color];
	}


	public void ClickInfo() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Help_Info_Char_DNASet);
	}
}
