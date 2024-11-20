using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_CharMember_PVP : ObjMng
{
	public enum State
	{
		Set,
		Empty
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image Portrait;
		public Image BG;
		public TextMeshProUGUI Lv;
		public Item_GradeGroup Grade;
		public Image[] WpAmIcon;		//0:무기 타입 :1:방어구타입 (어태커만 쓰임)
	}
	[SerializeField] SUI m_SUI; 
	RES_PVP_CHAR m_Info;
	public State m_State;

	public void SetData(RES_PVP_CHAR _info) {
		if (_info == null) {//empty
			m_SUI.Anim.SetTrigger(m_Info != null ? "Out" : "NormalOut");
			m_State = State.Empty;
		}
		else {
			TCharacterTable ctdata = TDATA.GetCharacterTable(_info.Idx);
			TPVPSkillTable pstdata = TDATA.GeTPVPSkillTable(ctdata.m_PVPSkillIdx);
			m_SUI.Portrait.sprite = ctdata.GetPortrait();
			m_SUI.BG.sprite = BaseValue.CharBG(_info.Grade);
			m_SUI.Lv.text = _info.LV.ToString();
			m_SUI.Grade.SetData(_info.Grade);
			if(_info.Pos < 5) {
				m_SUI.WpAmIcon[0].sprite = BaseValue.GetPVPEquipAtkIcon(pstdata.m_AtkType);
				m_SUI.WpAmIcon[1].sprite = BaseValue.GetPVPEquipDefIcon(ctdata.m_PVPArmorType);
			}
			m_SUI.Anim.SetTrigger(m_Info == null ? "In" : "Normal");
			m_State = State.Set;
		}
		m_Info = _info;
	}
}
