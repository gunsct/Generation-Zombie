using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_PVP_Resource_Element : ObjMng
{
	public enum State
	{
		Idle,
		Play,
		Get,
		Lock
	}
	[Serializable]
	public struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Cnt;
		public TextMeshProUGUI Time;
		public Image TimeIcon;
		public Image TimerGauge;
		public GameObject[] LockGroup;
		public TextMeshProUGUI LockDesc;
		public GameObject[] BtnPanel;		//0:생산 버튼, 1:생산 중, 2:획득
		public Item_Store_Buy_Button MakeBtn;
	}
	[SerializeField] SUI m_SUI;
	State m_State;
	CampBuildInfo m_Info;
	int m_Pos;
	Action<int> m_MCB;
	Action<int> m_GCB;
	TPVP_Camp_Resource m_TData { get { return TDATA.GetPVP_Camp_Resource(m_Info.LV); } }

	private void Update() {
		if (m_Info == null) return;
		if(m_State == State.Play) {
			double remain = Math.Max(0, (m_Info.Values[m_Pos] - UTILE.Get_ServerTime_Milli()));
			m_SUI.TimerGauge.fillAmount = 1f - (float)(remain / m_TData.m_Mat[m_Pos].GetNeedTime);
			m_SUI.Time.text = UTILE.GetSecToTimeStr(remain * 0.001d);
			if(remain == 0)
				SetData(m_Info, m_Pos, m_MCB, m_GCB);
		}
	}
	public void SetData(CampBuildInfo _info, int _pos, Action<int> _mcb, Action<int> _gcb) {
		m_Info = _info;
		m_Pos = _pos;
		m_MCB = _mcb;
		m_GCB = _gcb;

		TItemTable idata = TDATA.GetItemTable(BaseValue.CAMP_RES_IDX(_pos));
		m_SUI.Icon.sprite = idata.GetItemImg();
		m_SUI.Name.text = idata.GetName();

		if (m_TData.m_Mat[_pos].MakeCnt == 0) {
			m_State = State.Lock;
			m_SUI.LockGroup[0].SetActive(false);
			m_SUI.LockGroup[1].SetActive(true);
			m_SUI.BtnPanel[0].SetActive(false);
			m_SUI.BtnPanel[1].SetActive(false);
			m_SUI.BtnPanel[2].SetActive(false);
			int nextlv = m_Info.LV;
			while (TDATA.GetPVP_Camp_Resource(nextlv).m_Mat[_pos].MakeCnt == 0) {
				nextlv++;
			}
			m_SUI.LockDesc.text = string.Format(TDATA.GetString(6215), TDATA.GetString(6206), nextlv);
		}
		else {
			m_SUI.LockGroup[0].SetActive(true);
			m_SUI.LockGroup[1].SetActive(false);
			m_SUI.Cnt.text = string.Format("x{0}", m_TData.m_Mat[_pos].MakeCnt);
			m_SUI.Time.text = UTILE.GetSecToTimeStr(m_TData.m_Mat[_pos].GetNeedTime * 0.001d);
			//생산가능할때
			if (m_Info.IS_CanMakeTime(_pos)) {
				m_State = State.Idle;
				m_SUI.BtnPanel[0].SetActive(true);
				m_SUI.BtnPanel[1].SetActive(false);
				m_SUI.BtnPanel[2].SetActive(false);
				m_SUI.MakeBtn.SetData(PriceType.Money, m_TData.m_Mat[_pos].NeedMoney);

				m_SUI.TimeIcon.color = Utile_Class.GetCodeColor("#C5BAB9");
				m_SUI.Time.color = Utile_Class.GetCodeColor("#A4A4A4");

			}//보상받을수있을때
			else if (m_Info.IS_CanGetTime(_pos)) {
				m_State = State.Get;
				m_SUI.BtnPanel[0].SetActive(false);
				m_SUI.BtnPanel[1].SetActive(false);
				m_SUI.BtnPanel[2].SetActive(true);

				m_SUI.TimeIcon.color = Utile_Class.GetCodeColor("#C5BAB9");
				m_SUI.Time.color = Utile_Class.GetCodeColor("#A4A4A4");
			}
			else {//생산중일때
				m_State = State.Play;
				m_SUI.BtnPanel[0].SetActive(false);
				m_SUI.BtnPanel[1].SetActive(true);
				m_SUI.BtnPanel[2].SetActive(false);

				m_SUI.TimeIcon.color = Utile_Class.GetCodeColor("#FFAD00");
				m_SUI.Time.color = Utile_Class.GetCodeColor("#FEAD00");
			}
		}
	}
	public void Click_Make() {
		if (!m_Info.IS_CanMakeTime(m_Pos)) return;
		if (m_SUI.MakeBtn.CheckLack()) {
			m_MCB?.Invoke(m_Pos);
		}
	}
	public void Click_Get() {
		//if (!m_Info.IS_CanGetTime(m_Pos)) return;
		m_GCB?.Invoke(m_Pos);
	}
}
