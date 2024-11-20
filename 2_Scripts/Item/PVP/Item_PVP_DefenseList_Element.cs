using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_PVP_DefenseList_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Timer;
		public Image Portrait;
		public TextMeshProUGUI Lv;
		public Image Nation;
		public Text Name;
		public TextMeshProUGUI CP;
		public Image[] MatIcon;
		public TextMeshProUGUI[] MatCnt;
		public Color[] MatCntColor;			//0:없음,1:있음
		public TextMeshProUGUI BtnTxt;
	}
	[SerializeField] SUI m_SUI;
	REQ_CAMP_PLUNDER_LOG_DATA m_Data;
	Action<long, int> m_CB;

	public void SetData(REQ_CAMP_PLUNDER_LOG_DATA _data, Action<long, int> _cb) {
		m_Data = _data;
		m_CB = _cb;

		m_SUI.Portrait.sprite = TDATA.GetUserProfileImage(m_Data.Profile);
		m_SUI.Nation.sprite = BaseValue.GetNationIcon(m_Data.Nation);
		m_SUI.Name.text = m_Data.Name;
		m_SUI.CP.text = Utile_Class.CommaValue(m_Data.Power);
		m_SUI.Lv.text = m_Data.LV.ToString();
		List<RewardInfo> mats = m_Data.GetReward();
		for(int i = 0; i < 3; i++) {
			RewardInfo info = mats.Find(o => o.Idx == BaseValue.CAMP_RES_IDX(i));
			if (info == null) {
				m_SUI.MatCnt[i].text = "x0";
				m_SUI.MatCnt[i].color = m_SUI.MatCntColor[0];
			}
			else {
				m_SUI.MatCnt[i].text = string.Format("x{0}", info.Cnt);
				m_SUI.MatCnt[i].color = m_SUI.MatCntColor[1];
			}
		}
		switch (m_Data.State) {
			case CounterState.Idle: m_SUI.BtnTxt.text = TDATA.GetString(6243); break;
			case CounterState.Counter_WIN: m_SUI.BtnTxt.text = TDATA.GetString(6244); break;
			case CounterState.Counter_FAIL: 
			case CounterState.Counter_IDLE: 
				m_SUI.BtnTxt.text = TDATA.GetString(6245); break;
			case CounterState.Countered: m_SUI.BtnTxt.text = TDATA.GetString(6247); break;
		}
	}
	private void Update() {
		if (m_Data == null) return;
		m_SUI.Timer.text = string.Format(TDATA.GetString(10843), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, (UTILE.Get_ServerTime_Milli() - m_Data.BTime) * 0.001d));
	}
	public void Click_Revenge() {
		if (m_Data.State != CounterState.Idle) return;
		m_CB?.Invoke(m_Data.Target, m_Data.Idx);
	}
}
