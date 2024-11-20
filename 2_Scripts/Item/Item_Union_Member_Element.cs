using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using static Union_Main;

[System.Serializable] public class DicGuildMemberMngBtn : SerializableDictionary<Item_Union_Member_Element.CallMode, GameObject> { }

public class Item_Union_Member_Element : ObjMng
{
	public enum BtnMode
	{
		Normal = 0,
		Mng
	}

	public enum CallMode
	{
		Remove = 0,
		Master,
		End
	}

    [Serializable] 
    public struct SUI
	{
		public Image Profile;
		public Image Nation;
		public Text Name;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Power;
		public TextMeshProUGUI Point;
		public TextMeshProUGUI Time;

		public GameObject MasterMark;
		public DicGuildMemberMngBtn Btn;
	}

	[SerializeField] SUI m_SUI;
	RES_GUILD_USER m_User;
	Action<CallMode, RES_GUILD_USER> m_CB;
	bool m_IsMaster;
	public void SetData(RES_GUILD_USER user, Action<CallMode, RES_GUILD_USER> _cb) {
		m_User = user;
		m_IsMaster = user.Grade == GuildGrade.Master;
		m_CB = _cb;
		m_SUI.Profile.sprite = TDATA.GetUserProfileImage(user.Profile);
		m_SUI.Name.text = user.m_Name;
		m_SUI.LV.text = user.LV.ToString();
		m_SUI.Time.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.ago_join, (UTILE.Get_ServerTime_Milli() - user.UTime) / 1000D);
		m_SUI.Power.text = Utile_Class.CommaValue(user.Power);
		m_SUI.Point.text = Utile_Class.CommaValue(user.Point);
		m_SUI.Nation.sprite = BaseValue.GetNationIcon(user.Nation);
		m_SUI.MasterMark.SetActive(m_IsMaster);
	}


	public void Click_Master()
	{
		m_CB?.Invoke(CallMode.Master, m_User);
	}

	public void Click_Remove()
	{
		m_CB?.Invoke(CallMode.Remove, m_User);
	}

	public void ChangeMode(MemberListMode Mode)
	{
		switch(Mode)
		{
		case MemberListMode.Normal:
			m_SUI.Btn[CallMode.Master].SetActive(false);
			m_SUI.Btn[CallMode.Remove].SetActive(!m_IsMaster && m_User.UserNo == USERINFO.m_UID);
			break;
		case MemberListMode.Mng:
			m_SUI.Btn[CallMode.Master].SetActive(!m_IsMaster);
			m_SUI.Btn[CallMode.Remove].SetActive(USERINFO.m_Guild.IsUserKickState());
			break;
		}
	}
}
