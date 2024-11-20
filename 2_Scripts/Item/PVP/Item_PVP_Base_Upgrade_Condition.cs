using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Item_PVP_Base_Upgrade_Condition : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public GameObject Obj;
		public TextMeshProUGUI Name;
	}
	[SerializeField] SUI m_SUI;
	public bool Is_Can;

	public void SetData(TPVP_Camp_NodeLevel _tdata, int _pos) {
		var condition = _tdata.m_Condition[_pos];
		if (condition.m_Type != CampBuildType.None) {
			Is_Can = USERINFO.m_CampBuild[condition.m_Type].LV >= condition.m_Lv;
			string name = string.Empty;
			switch (condition.m_Type) {
				case CampBuildType.Camp: name = TDATA.GetString(6205); break;
				case CampBuildType.Storage: name = TDATA.GetString(6207); break;
				case CampBuildType.Resource: name = TDATA.GetString(6206); break;
			}
			m_SUI.Name.text = string.Format("{0} Lv.{1}", name, condition.m_Lv);
			m_SUI.Name.color = Utile_Class.GetCodeColor(Is_Can ? "#53BC42" : "#FF4739");
		}

	}
}
