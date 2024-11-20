using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;

public class Item_Union_JoinMail : ObjMng
{
    [Serializable] 
    public struct SUI
	{
		public Image Profile;
		public Image Nation;
		public Text Name;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Time;
		public TextMeshProUGUI Power;
		[ReName("Normal", "Remove")]
		public GameObject[] Btn;
		public Item_Button Apply;
	}

	[SerializeField] SUI m_SUI;
	RES_GUILD_REQUSER m_User;
	Action<RES_GUILD_REQUSER> m_CB;
	public void SetData(RES_GUILD_REQUSER user, Action<RES_GUILD_REQUSER> _cb) {
		m_User = user;
		m_CB = _cb;
		m_SUI.Profile.sprite = TDATA.GetUserProfileImage(user.Profile);
		m_SUI.Name.text = user.m_Name;
		m_SUI.LV.text = user.LV.ToString();
		m_SUI.Time.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.ago_join, (UTILE.Get_ServerTime_Milli() - user.UTime) / 1000D);
		m_SUI.Power.text = Utile_Class.CommaValue(user.Power);
		m_SUI.Nation.sprite = BaseValue.GetNationIcon(user.Nation);
		m_SUI.Apply.SetBG(USERINFO.m_Guild.MemberCnt >= USERINFO.m_Guild.MaxUserCnt ? UIMng.BtnBG.Not : UIMng.BtnBG.Green);
	}

	public void Click() {
		m_CB?.Invoke(m_User);
	}

	public void ChangeMode(Union_Mng.ListMode Mode)
	{
		for(int i = 0; i < m_SUI.Btn.Length; i++)
		{
			bool Active = i == (int)Mode;
			m_SUI.Btn[i].SetActive(Active);
		}
	}
}
