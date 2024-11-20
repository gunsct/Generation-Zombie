using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_Friend_Element : ObjMng
{
	public enum BtnMode
	{
		Find = 0,
		Friend,
		Invite,
		End
	}

	public enum Btn
	{
		Invite = 0,
		Accept,
		Delete,
		Give,
		Get,
		Profile
	}

#pragma warning disable 0649
	[System.Serializable]
	struct SModeUI
	{
		public SBtnUI[] Btns;
	}

	[System.Serializable]
	struct SBtnUI
	{
		public GameObject Active;
		public GameObject Comple;
	}

	[System.Serializable]
	struct SUI
	{
		public Animator Ani;
		public RectTransform Panel;
		public Image Profile;
		public Image Nation;
		public Text Name;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Time;
		public TextMeshProUGUI Stage;
		public TextMeshProUGUI Power; 
		[ReName("찾기", "친구", "초대")]
		public SModeUI[] Modes;
	}

	[SerializeField] SUI m_sUI;
	BtnMode m_Mode;
	public RES_RECOMMEND_USER m_Info;
	Action m_CB;
	bool m_UseProfileBtn;
#pragma warning restore 0649

	public void SetData(BtnMode Mode, RES_RECOMMEND_USER info, Vector2 Pos, Vector3 Rot, bool UseProfileBtn, bool Start, Action ResetUI)
	{
		StopStartAction();
		m_Info = info;
		m_CB = ResetUI;
		m_Mode = Mode;
		m_UseProfileBtn = UseProfileBtn;
		m_sUI.Panel.anchoredPosition = Pos;
		m_sUI.Panel.eulerAngles = Rot;
		m_sUI.Profile.sprite = TDATA.GetUserProfileImage(info.Profile);
		m_sUI.Name.text = info.m_Name;
		m_sUI.LV.text = info.LV.ToString();
		m_sUI.Time.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.ago, (UTILE.Get_ServerTime_Milli() - info.UTime) / 1000D);
		m_sUI.Stage.text = string.Format("{0} {1}-{2}", TDATA.GetString(83), info.Stage / 100, info.Stage % 100);
		m_sUI.Power.text = Utile_Class.CommaValue(info.Power);
		m_sUI.Nation.sprite = BaseValue.GetNationIcon(info.Nation);
		SetBtn();
		if(Start && !m_sUI.Ani.GetCurrentAnimatorStateInfo(0).IsName("Empty")) m_sUI.Ani.SetTrigger("Empty");
	}

	void SetBtn()
	{
		bool Active;
		switch (m_Mode)
		{
		case BtnMode.Find:
			Active = USERINFO.m_Friend.Friends.Find(o => o.UserNo == m_Info.UserNo) == null;
			m_sUI.Modes[(int)BtnMode.Find].Btns[0].Active.SetActive(Active);
			m_sUI.Modes[(int)BtnMode.Find].Btns[1].Active.SetActive(!Active);
			break;
		case BtnMode.Friend:
			RES_FRIENDINFO info = (RES_FRIENDINFO)m_Info;
			bool comple = !info.IsSendGift();
			m_sUI.Modes[(int)BtnMode.Friend].Btns[0].Active.SetActive(true);
			m_sUI.Modes[(int)BtnMode.Friend].Btns[0].Active.GetComponent<Button>().enabled = !comple;
			m_sUI.Modes[(int)BtnMode.Friend].Btns[0].Comple.SetActive(comple);

			comple = info.GetGiftState() != Friend_Gift_State.Gift; 
			m_sUI.Modes[(int)BtnMode.Friend].Btns[1].Active.SetActive(true);
			m_sUI.Modes[(int)BtnMode.Friend].Btns[1].Active.GetComponent<Button>().enabled = !comple;
			m_sUI.Modes[(int)BtnMode.Friend].Btns[1].Comple.SetActive(comple);
			break;
		case BtnMode.Invite:
			m_sUI.Modes[(int)BtnMode.Invite].Btns[0].Active.SetActive(true);
			m_sUI.Modes[(int)BtnMode.Invite].Btns[1].Active.SetActive(true);
			break;
		}

		for (int i = 0; i < m_sUI.Modes.Length; i++)
		{
			if ((BtnMode)i != m_Mode)
			{
				for (int j = 0; j < m_sUI.Modes[i].Btns.Length; j++) m_sUI.Modes[i].Btns[j].Active.SetActive(false);
			}
		}
	}

	IEnumerator StartAction;

	public void StopStartAction()
	{
		if (StartAction != null)
		{
			StopCoroutine(StartAction);
			StartAction = null;
		}
	}

	public void ViewStart(float delay)
	{
		StopStartAction();
		StartAction = DelayStart(delay);
		StartCoroutine(DelayStart(delay));
	}

	IEnumerator DelayStart(float delay)
	{
		yield return new WaitForSeconds(delay);
		m_sUI.Ani.SetTrigger("Start");
	}


	public void OnClick(int Pos)
	{
		Btn btn = (Btn)Pos;
		Debug.Log(btn.ToString());
		Friend_State state = m_Mode != BtnMode.Find ? ((RES_FRIENDINFO)m_Info).State : Friend_State.End;
		switch (btn)
		{
		case Btn.Invite:
			USERINFO.m_Friend.Invate(m_Info, (res) =>
			{
				SetBtn();
				m_CB?.Invoke();
			});
			return;
		case Btn.Accept:
			WEB.SEND_REQ_FRIEND_ACCEPT((res) => {
				if (!res.IsSuccess())
				{
					switch(res.result_code)
					{
					case EResultCode.ERROR_FRIEND_CNT:
						POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(669));
						break;
					case EResultCode.ERROR_MY_FRIEND_CNT:
						POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(670));
						break;
					default:
						WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
						break;
					}
					return;
				}
				GameObject.Destroy(gameObject);
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(684));
				PlayEffSound(SND_IDX.SFX_0117);
				// 리셋 호출을 하기때문에 연출 문제가 생김 제거만 할것
				m_CB?.Invoke();
			}, m_Info.UserNo);
			return;
		case Btn.Delete:
			USERINFO.m_Friend.Delete((RES_FRIENDINFO)m_Info, (res) =>
			{
				if (res == EResultCode.SUCCESS)
				{
					((RES_FRIENDINFO)m_Info).State = Friend_State.Deleted;
					GameObject.Destroy(gameObject);
					// 리셋 호출을 하기때문에 연출 문제가 생김 제거만 할것
					m_CB?.Invoke();
				}
			});
			return;
		case Btn.Give:
			// 보낼 수 있는 유저들
			WEB.SEND_REQ_FRIEND_GIFT_GIVE((res) => {
				if (res.IsSuccess()) {
					PLAY.PlayEffSound(SND_IDX.SFX_0115);
					SetBtn();
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(781));
					m_CB?.Invoke();
				}
			}, new List<long>() { m_Info.UserNo });
			return;
		case Btn.Get:
			var cnt = USERINFO.m_Friend.Friends.Count(o => o.State == Friend_State.Friend && o.GetGiftState() == Friend_Gift_State.Get);
			// 남은 획득 가능한 개수
			var max = TDATA.GetConfig_Int32(ConfigType.MaxFriendReceiveCount) - cnt;
			if (max < 1)
			{
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(676), TDATA.GetConfig_Int32(ConfigType.MaxFriendReceiveCount)));
				return;
			}
			WEB.SEND_REQ_FRIEND_GIFT_GET((res) => {
				if(res.IsSuccess()) SetBtn();
				List<RES_REWARD_BASE> Rewards = res.GetRewards();
				if (Rewards.Count > 0) MAIN.SetRewardList(new object[] { Rewards }, () => { });
				m_CB?.Invoke();
			}, new List<long>() { m_Info.UserNo });
			return;
		case Btn.Profile:
			if (!m_UseProfileBtn) return;
			PlayEffSound(SND_IDX.SFX_0112);
			var pop = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_User, null, m_Info);
			pop.GetComponent<Info_User>().SetBtnCB((btn) =>
			{
				switch (btn)
				{
				case Info_User.Btn.Delete:
					((RES_FRIENDINFO)m_Info).State = Friend_State.Deleted;
					GameObject.Destroy(gameObject);
					pop.Close(0);
					// 리셋 호출을 하기때문에 연출 문제가 생김 제거만 할것
					m_CB?.Invoke();
					break;
				}
			});
			return;
		}
		m_CB?.Invoke();
	}

	void ProfileAction(Btn btn)
	{

	}
}
