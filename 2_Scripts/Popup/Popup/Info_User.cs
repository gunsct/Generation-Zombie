using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using System.Linq;

[System.Serializable] public class DicInfoUserBtn : SerializableDictionary<Info_User.Btn, GameObject> { }
public class Info_User : PopupBase
{
	public enum Btn
	{
		None = 0,
		Invite,
		Delete,
		End
	}

	[Serializable]
    public struct SUI
	{
		public Animator Ani;
		public Image Profile;
		public Image Nation;
		public Text Name;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Time;
		public TextMeshProUGUI Stage;
		public TextMeshProUGUI Power;

		public GameObject ActiveBtn;
		public DicInfoUserBtn Btns;
	}

	[SerializeField] SUI m_sUI;
	public RES_RECOMMEND_USER m_Info;
	Action<Btn> m_BtnCB;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{

		m_Info = (RES_RECOMMEND_USER)aobjValue[0];
		// 유저 검색일때 내친구를 검색 할 수 있으므로 찾아서 바꿔줌
		var user = USERINFO.m_Friend.Friends.Find(o => o.UserNo == m_Info.UserNo);
		if(user != null) m_Info = user;

		m_sUI.Profile.sprite = TDATA.GetUserProfileImage(m_Info.Profile);
		m_sUI.Name.text = m_Info.m_Name;
		m_sUI.LV.text = m_Info.LV.ToString();
		m_sUI.Time.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.ago, (UTILE.Get_ServerTime_Milli() - m_Info.UTime) / 1000D);
		m_sUI.Stage.text = string.Format("{0} {1}-{2}", TDATA.GetString(83), m_Info.Stage / 100, m_Info.Stage % 100);
		m_sUI.Power.text = Utile_Class.CommaValue(m_Info.Power);
		m_sUI.Nation.sprite = BaseValue.GetNationIcon(m_Info.Nation);
		SetBtn();
		base.SetData(pos, popup, cb, aobjValue);
	}

	public void SetBtnCB(Action<Btn> cb)
	{
		m_BtnCB = cb;
	}

	void SetBtn()
	{
		var check = USERINFO.m_Friend.Friends.Find(o => o.UserNo == m_Info.UserNo);
		Btn mode = Btn.Invite;
		if(check != null)
		{
			switch(check.State)
			{
			case Friend_State.Friend:	mode = Btn.Delete; break;
			default:					mode = Btn.None; break;
			}
		}

		m_sUI.ActiveBtn.SetActive(mode != Btn.None);
		for (Btn i = Btn.Invite; i < Btn.End; i++)
		{
			m_sUI.Btns[i].SetActive(i == mode);
		}
	}

	public void OnClick(int Pos)
	{
		if (m_Action != null) return;
		Btn btn = (Btn)Pos;
		switch(btn)
		{
		case Btn.Invite:
			USERINFO.m_Friend.Invate(m_Info, (res) =>
			{
				if (res == EResultCode.SUCCESS) m_BtnCB?.Invoke(btn);
				SetBtn();
			});
			break;
		case Btn.Delete:
			USERINFO.m_Friend.Delete((RES_FRIENDINFO)m_Info, (res) => {
				if (res == EResultCode.SUCCESS) m_BtnCB?.Invoke(btn);
				SetBtn();
			});
			break;
		}
	}

	IEnumerator m_Action;

	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		m_Action = Close((EMsgBtn)Result);
		StartCoroutine(m_Action);
	}


	public IEnumerator Close(EMsgBtn btn)
	{
		m_sUI.Ani.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_sUI.Ani));
		base.Close((int)btn);
	}
}
