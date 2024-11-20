using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Coffee.UIEffects;

public class Item_CharacterCard : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public Image[] JobIcon;
		public Vector3[] JobIconPos;
		public Image CharImg;
		public TextMeshProUGUI Lv;
		public GameObject LvGroup;
		public Item_GradeGroup GradeGroup;
		public Image GradeBG;
		public Image GrageFrame;
		public Image GradeOriginFrame;
	}
	[SerializeField] protected SUI m_SUI;
	public int m_Idx;
	public CharInfo m_Info;
	public TCharacterTable m_TData { get { return TDATA.GetCharacterTable(m_Idx); } }
	int m_TUTOAddLV { get { return TUTO.CheckUseCloneDeck() ? 99 : 0; } }
	public virtual void SetData(int _idx) {
		m_Idx = _idx;
		for(int i = 0; i < m_SUI.JobIcon.Length; i++) {
			if (i < m_TData.m_Job.Count) {
				m_SUI.JobIcon[i].sprite = m_TData.GetJobIcon()[i];
				m_SUI.JobIcon[i].transform.parent.gameObject.SetActive(true);
			}
			else
				m_SUI.JobIcon[i].transform.parent.gameObject.SetActive(false);
		}
		m_SUI.CharImg.sprite = m_TData.GetPortrait();
		m_SUI.Lv.text = (1 + m_TUTOAddLV).ToString();
		SetGrade(0);
		SetCharState(false);
	}

	public virtual void SetData(CharInfo _info) {
		m_Info = _info;
		SetData(_info.m_Idx);
		m_SUI.Lv.text = (_info.m_LV + m_TUTOAddLV).ToString();
		SetGrade(_info.m_Grade);
		SetCharState(true);
	}

	public void SetGrade(int _grade = 0) {
		int grade = _grade;
		m_SUI.GradeBG.sprite = BaseValue.CharBG(grade);
		if (m_SUI.GrageFrame != null) m_SUI.GrageFrame.sprite = BaseValue.CharFrame(grade);
		if (m_SUI.GradeOriginFrame != null) {
			Sprite frame = BaseValue.CharOriginFrame(m_TData.m_Grade, grade);
			m_SUI.GradeOriginFrame.sprite = frame;
			m_SUI.GradeOriginFrame.gameObject.SetActive(frame != null);
		}

		grade = _grade == 0 ? m_TData.m_Grade : _grade;
		m_SUI.GradeGroup.SetData(Mathf.Max(grade, 1));
	}
	public void SetCharState(bool _had) {
		Color setcolor = Color.white;
		if (_had) {
			ColorUtility.TryParseHtmlString("#ffffff", out setcolor);
			if (m_SUI.CharImg.GetComponent<UIEffect>()) {
				m_SUI.CharImg.GetComponent<UIEffect>().effectMode = EffectMode.None;
				m_SUI.CharImg.GetComponent<UIEffect>().colorFactor = 0f;
			}
		}
		else {
			ColorUtility.TryParseHtmlString("#29251c", out setcolor);
			if (m_SUI.CharImg.GetComponent<UIEffect>()) {
				m_SUI.CharImg.GetComponent<UIEffect>().effectMode = EffectMode.Grayscale;
				m_SUI.CharImg.GetComponent<UIEffect>().effectFactor = 1f;
				m_SUI.CharImg.GetComponent<UIEffect>().colorFactor = 0.406f;
			}
		}
		m_SUI.CharImg.color = setcolor;
	}
	public void SetLvOnOff(bool _on) {
		m_SUI.LvGroup.SetActive(_on);
	}
}
