using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_PVP_Storage_Element : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI[] Cnt;
		public GameObject[] LockGroup;
		public TextMeshProUGUI LockDesc;
	}
	[SerializeField] SUI m_SUI;
	/// <summary> _cnt 0¿Ã∏È ¿·±Ë</summary>
	public void SetData(int _lv, int _pos, int _cnt) {
		TItemTable idata = TDATA.GetItemTable(BaseValue.CAMP_RES_IDX(_pos));
		m_SUI.Icon.sprite = idata.GetItemImg();
		m_SUI.Name.text = idata.GetName();

		TPVP_Camp_Storage stdata = TDATA.GetPVP_Camp_Storage(_lv);
		if (stdata.m_SaveMat[_pos] == 0) {
			if(m_SUI.LockGroup[0] != null) m_SUI.LockGroup[0].SetActive(false);
			if (m_SUI.LockGroup[1] != null) m_SUI.LockGroup[1].SetActive(true);
			int nextlv = _lv;
			while(TDATA.GetPVP_Camp_Storage(nextlv).m_SaveMat[_pos] == 0) {
				nextlv++;
			}
			m_SUI.LockDesc.text = string.Format(TDATA.GetString(6215), TDATA.GetString(6207), nextlv);
		}
		else {
			if (m_SUI.LockGroup[0] != null) m_SUI.LockGroup[0].SetActive(true);
			if (m_SUI.LockGroup[1] != null) m_SUI.LockGroup[1].SetActive(false);
			m_SUI.Cnt[0].text = Utile_Class.CommaValue(_cnt);
			m_SUI.Cnt[1].text = Utile_Class.CommaValue(stdata.m_SaveMat[_pos]);
		}
	}
}
