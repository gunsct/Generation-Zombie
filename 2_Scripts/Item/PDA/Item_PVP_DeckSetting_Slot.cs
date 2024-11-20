using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PVP_DeckSetting_Slot : ObjMng
{
    public enum State
	{
		ATK,
		SUP,
		Empty,
	}
	public enum Mode
	{
		Init,
		BTChange,
		Refresh
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_CharMember_PVP[] Chars;     //0:atk, 1,2:sup
		public TextMeshProUGUI[] BTCP;
		public TextMeshProUGUI AddCP;
		public Image SupFace;
	}
    [SerializeField] SUI m_SUI;
	public PVPPosType m_State = PVPPosType.None;
	CharInfo[] m_CInfo = new CharInfo[2];
	int m_Pos;
	Action<int> m_CB;
	string m_Animstr;

	public void SetData(int _pos, CharInfo _atk, CharInfo _sup, PVPPosType _state, Action<int> _cb, Mode _mode = Mode.BTChange) {
		m_Pos = _pos;
		m_CInfo[0] = _atk;
		m_CInfo[1] = _sup;
		m_State = _state;
		m_CB = _cb;

		for (int i = 0, pos = 0; i < 3; i++) {
			m_SUI.BTCP[pos].text = m_CInfo[pos] != null ? m_CInfo[pos].m_PVPCP.ToString() : "0";
			RES_PVP_CHAR cinfo = m_CInfo[pos] == null ? null : new RES_PVP_CHAR() {
				Idx = m_CInfo[pos].m_Idx,
				Grade = m_CInfo[pos].m_Grade,
				LV = m_CInfo[pos].m_LV,
				Pos = pos == 0 ? m_Pos : m_Pos + 5
			};
			m_SUI.Chars[i].SetData(cinfo);
			if (i == 0) pos++;
		}

		if(m_State != PVPPosType.None) {
			switch (_mode) {
				case Mode.Init:
					if(string.IsNullOrEmpty(m_Animstr)) m_Animstr = "Start";
					m_SUI.Anim.SetTrigger(m_Animstr);
					break;
				case Mode.BTChange:
					m_Animstr = m_State == PVPPosType.Combat ? "ToATK" : "ToSUP";
					m_SUI.Anim.SetTrigger(m_Animstr);
					break;
				case Mode.Refresh:
					m_Animstr = m_State == PVPPosType.Combat ? "ATK" : "SUP";
					m_SUI.Anim.SetTrigger(m_Animstr);
					if (m_State == PVPPosType.Supporter) {
						if (m_CInfo[1] != null) {
							m_SUI.Anim.SetTrigger("SUPSet");
							m_SUI.AddCP.text = m_CInfo[1].m_PVPCP.ToString();
							m_SUI.SupFace.sprite = m_CInfo[1].m_TData.GetPortrait();
						}
					}
					break;
			}
			m_Animstr = m_State == PVPPosType.Combat ? "ATK" : "SUP";
		}
	}
	public void SetAnim(string _trig) {
		m_SUI.Anim.SetTrigger(_trig);
	}
	public void ClickSlot() {
		if (m_CInfo[(int)m_State - 1] == null) return;

		//m_CInfo[(int)m_State - 1] = null;
		//m_SUI.Chars[(int)m_State - 1].SetData(null);
		m_CB?.Invoke(m_State == PVPPosType.Combat ? m_Pos : m_Pos + 5);
	}
	public CharInfo GetNowCInfo() {
		return m_CInfo[(int)m_State - 1];
	}
	public void OpenDetail(DeckInfo _dinfo) {
		CharInfo cinfo = m_CInfo[(int)m_State - 1];
		if (cinfo == null) return; 
		List<CharInfo> charinfos = new List<CharInfo>();
		for (int i = 0; i < _dinfo.GetDeckCharCnt(); i++) {
			CharInfo info = USERINFO.GetChar(_dinfo.m_Char[i]);
			charinfos.Add(info);
		}
		USERINFO.ShowCharInfo(cinfo.m_Idx, charinfos, (result, obj) => {
			DLGTINFO.f_RFDeckCharInfoCard?.Invoke();
		}, true);
	}
}
