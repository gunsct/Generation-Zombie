using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PersonnelFileChange : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Item_Item_Card File;
		public TextMeshProUGUI Cnt;
		public Image Gauge;
		public TextMeshProUGUI Name;
	}
	[SerializeField] SUI m_SUI;
	public int m_Idx;
	public int m_Grade;
	public int m_Cnt;
	Action m_CB;

	public void SetData(int _idx, int _grade, int _cnt, Action _cb) {
		m_Idx = _idx;
		m_Grade = _grade;
		m_Cnt = _cnt;
		m_CB = _cb;
		m_SUI.File.SetData(_idx, 0, _grade);
		m_SUI.Cnt.text = string.Format("{0}", _cnt);
		m_SUI.Gauge.fillAmount = (float)_cnt / (float)999;
		m_SUI.Name.text = TDATA.GetItemTable(_idx).GetName();
	}
	/// <summary> 무기명 파일로 선택한 파일 교환</summary>
	public void Click_Select() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PersonnelFileChange, (result, obj) => { 
		if(result == 1) m_CB?.Invoke();
		}, BaseValue.COMMON_PERSONNELFILE_IDX, m_Idx, m_Grade);
	}
}
