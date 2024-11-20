using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_Info_DNA_Stat : ObjMng
{
	[Serializable] 
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI LvTxt;
		public TextMeshProUGUI StatTxt;
		public TextMeshProUGUI StatGradeTxt;
		public GameObject[] EssentialObjs;
	}
	[SerializeField] SUI m_SUI;
	int m_ColorPos = 0;

	private void OnEnable() {
		if(m_ColorPos > 0) m_SUI.Anim.SetTrigger(string.Format("Grade_{0}", m_ColorPos));
	}
	public void SetData(int _lv, DNAInfo _dnainfo, bool _change = false) {
		TDnaTable tdata = _dnainfo.m_TData;
		TDNALevelTable tldata = TDATA.GetDNALevelTable(tdata.m_BGType, tdata.m_Grade, _lv);
		int pos = TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, _lv) - 1;
		bool essential = tldata.m_EssentialStatGrant > 0;
		bool lockstat = _dnainfo.m_AddStat.Count <= pos;

		m_SUI.LvTxt.text = string.Format("<size=80%><color=#aaaaaa>Lv. </color></size>{0}", _lv);
		for (int i = 0; i < m_SUI.EssentialObjs.Length; i++) m_SUI.EssentialObjs[i].SetActive(essential);

		m_SUI.Anim.SetTrigger("Normal");
		m_SUI.StatGradeTxt.gameObject.SetActive(lockstat);
		m_SUI.StatGradeTxt.gameObject.SetActive(!lockstat);
		if (lockstat) {
			int essentialidx = tldata.m_EssentialStatGrant;
			StatType randstat = essentialidx > 0 ? TDATA.GetRandomStatTable(essentialidx).m_Stat : StatType.None;
			m_SUI.StatTxt.text = essential ? string.Format("{0} +???", TDATA.GetStatString(randstat)) :"???";
			m_SUI.Anim.SetTrigger("Grade_0");
		}
		else {
			ItemStat stat = _dnainfo.m_AddStat[pos];
			TRandomStatTable trand = TDATA.GetRandomStatTable(_dnainfo.m_TData.m_RandStatGroup, stat.m_Stat);
			float ratio = (stat.m_Val - trand.m_Val[0]) / (trand.m_Val[1] - trand.m_Val[0]);
			KeyValuePair<int, string> gradeinfo = BaseValue.StatGradeName(ratio);
			m_ColorPos = gradeinfo.Key;
			m_SUI.StatTxt.text = stat.GetStatString();
			m_SUI.StatGradeTxt.text = gradeinfo.Value;
			if (_change) m_SUI.Anim.SetTrigger("Change");
			m_SUI.Anim.SetTrigger(string.Format(_change ? "Change_G_{0}" : "Grade_{0}", m_ColorPos));
		}
	}

	public void SetNone(int _lv, DNAInfo _dnainfo)
	{
		TDnaTable tdata = _dnainfo.m_TData;
		TDNALevelTable tldata = TDATA.GetDNALevelTable(tdata.m_BGType, tdata.m_Grade, _dnainfo.m_Lv);
		TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, _lv);
		bool essential = tldata.m_EssentialStatGrant > 0;

		m_SUI.LvTxt.text = string.Format("<size=80%><color=#aaaaaa>Lv. </color></size>{0}", _lv);
		for (int i = 0; i < m_SUI.EssentialObjs.Length; i++) m_SUI.EssentialObjs[i].SetActive(essential);

		int essentialidx = tldata.m_EssentialStatGrant;

		StatType randstat = essentialidx > 0 ? TDATA.GetRandomStatTable(essentialidx).m_Stat : StatType.None;
		m_SUI.StatTxt.text = essential ? string.Format("{0} +???", TDATA.GetStatString(randstat)) : "???";
		m_SUI.Anim.SetTrigger("Grade_0");
	}

	public bool CheckEndAnim() {
		return Utile_Class.IsAniPlay(m_SUI.Anim);
	}
	public void ClickViewEssential() {

	}
}
