using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_Union_JoinList_Element : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Animator Ani;
		public Image Mark;
		public Image Nation;

		public TextMeshProUGUI LV;
		public Text Name;
		public TextMeshProUGUI MemberCnt;
		public Text Intro;
		public GameObject NoneIntro;
	}

	[SerializeField] SUI m_sUI;
	Action<RES_GUILDINFO_SIMPLE> m_ClickCB;
	RES_GUILDINFO_SIMPLE m_Guild;
	bool IsMyJoin;
	bool IsStart;
#pragma warning restore 0649
	void Start()
	{
		StartAni();
		IsStart = true;
	}
	public void SetData(RES_GUILDINFO_SIMPLE info, Action<RES_GUILDINFO_SIMPLE> clickCB, bool IsJoin)
	{
		m_Guild = info;
		m_ClickCB = clickCB;
		IsMyJoin = IsJoin;
		SetUI();
		if(IsStart) StartAni();
	}

	void StartAni()
	{
		Utile_Class.AniResetAllTriggers(m_sUI.Ani);
		m_sUI.Ani.SetTrigger(IsMyJoin ? "Waiting" : "Normal");
	}

	void SetUI()
	{
		m_sUI.Mark.sprite = m_Guild.GetGuilMark();
		m_sUI.Nation.sprite = BaseValue.GetNationIcon(m_Guild.Nation);
		int LV = 1;
		long Exp = 0;
		m_Guild.Calc_Exp(out LV, out Exp);
		m_sUI.LV.text = string.Format("Lv.{0}", LV.ToString());
		m_sUI.Name.text = m_Guild.Name;
		m_sUI.MemberCnt.text = string.Format("{0}/{1}", m_Guild.UserCnt, m_Guild.MaxUserCnt);
		if(!string.IsNullOrWhiteSpace(m_Guild.Intro))
		{
			m_sUI.Intro.gameObject.SetActive(true);
			m_sUI.NoneIntro.SetActive(false);
			m_sUI.Intro.text = m_Guild.Intro;
		}
		else
		{
			m_sUI.Intro.gameObject.SetActive(false);
			m_sUI.NoneIntro.SetActive(true);
		}
	}

	public void OnClick()
	{
		m_ClickCB?.Invoke(m_Guild);
	}

}
