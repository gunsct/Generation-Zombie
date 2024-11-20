using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Timer : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public TextMeshProUGUI[] TimerTxt;
	}

	[SerializeField]
	SUI m_SUI;
	double m_Time;
	void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFModeTimer += SetData;
		}
	}
	void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFModeTimer -= SetData;
		}
	}
	public void SetData(double _time, bool _continue = false) {
		if (!_continue || m_Time == 0) m_Time = _time;
		else if (m_Time > 0 && m_Time > _time) m_Time = _time;
		for (int i = 0; i < 2; i++) m_SUI.TimerTxt[i].text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.mm_ss_ff, m_Time);
	}
}
