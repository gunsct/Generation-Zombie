using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using System.Linq;

[System.Serializable] public class DicFriendTabMenu : SerializableDictionary<Friend.State, Friend.STabUI> { }
public class Friend : PopupBase
{
	public enum State
	{
		None = 0,
		Friends,
		Find,
		Invited,
		End
	}
	[System.Serializable]
	public struct STabUI
	{
		public int NameIdx;
		public GameObject Active;
		public Item_Tab Menu;
	}

	[Serializable]
	public struct SRot
	{
		public Vector2 Pos;
		public Vector3 Rot;
	}

	[Serializable]
	public struct SFriendUI
	{
		public TextMeshProUGUI Cnt;
		public TextMeshProUGUI ReciveCnt;
		public GameObject Empty;
		public ScrollRect Scroll;
		public RectTransform LoadPanel;
		public RectTransform Prefab;
		public GameObject[] ButtomGroup;
	}

	[Serializable]
	public struct SFindUI
	{
		public TextMeshProUGUI MyCode;
		public TextMeshProUGUI Cnt;
		public ScrollRect Scroll;
		public RectTransform LoadPanel;
		public RectTransform Prefab;
		public TMP_InputField Input;

		[HideInInspector] public List<RES_RECOMMEND_USER> Recommends;
	}

	[Serializable]
	public struct SInvitedUI
	{
		public TextMeshProUGUI Cnt;
		public GameObject Empty;
		public ScrollRect Scroll;
		public RectTransform LoadPanel;
		public RectTransform Prefab;
	}

	[Serializable]
    public struct SUI
	{
		public DicFriendTabMenu Tab;

		public SFriendUI Friend;
		public SFindUI Find;
		public SInvitedUI Invited;
		

		public SRot[] Pos;
	}

	[SerializeField] SUI m_SUI;

	State m_Page;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		PLAY.PlayEffSound(SND_IDX.SFX_0200);
		m_Page = State.None;
		m_SUI.Find.Recommends = new List<RES_RECOMMEND_USER>();
		for (State i = State.Friends; i < State.End; i++)
		{
			m_SUI.Tab[i].Menu.SetData((int)i, TDATA.GetString(m_SUI.Tab[i].NameIdx), SelectMenu);
			m_SUI.Tab[i].Active.SetActive(false);
		}
		StateChange(State.Friends);
		base.SetData(pos, popup, cb, aobjValue);
	}

	public void CheckAlram()
	{
		CheckFriendsAlram();

		var friends = USERINFO.m_Friend.Friends.FindAll(o => o.State == Friend_State.Friend || o.State == Friend_State.Deleted);
		int remaincnt = TDATA.GetConfig_Int32(ConfigType.MaxFriendReceiveCount) - friends.Count;
		m_SUI.Tab[State.Find].Menu.SetAlram(remaincnt > 0);
		CheckInvitedAlram(USERINFO.m_Friend.Friends.Count(o => o.State == Friend_State.Idle));
	}

	void CheckFriendsAlram(List<RES_FRIENDINFO> list = null)
	{
		if(list == null) list = USERINFO.m_Friend.Friends.FindAll(o => o.State == Friend_State.Friend || o.State == Friend_State.Deleted);
		var cnt = list.Count(o => o.GetGiftState() == Friend_Gift_State.Get);
		// 남은 획득 가능한 개수
		var max = Math.Min(TDATA.GetConfig_Int32(ConfigType.MaxFriendReceiveCount), list.Count) - cnt;
		var rev = list.Count(o => o.GetGiftState() == Friend_Gift_State.Gift);

		m_SUI.Tab[State.Friends].Menu.SetAlram(max > 0 && rev > 0);
	}

	void CheckInvitedAlram(int cnt) {
		m_SUI.Tab[State.Invited].Menu.SetAlram(cnt > 0);
	}

	bool SelectMenu(Item_Tab tab)
	{
		//if (TUTO.TouchCheckLock(TutoTouchCheckType.Inventory, 0, (EMenu)tab.m_Pos)) return false;
		State state = (State)tab.m_Pos;
		if (m_Page == state) return false;
		StateChange((State)tab.m_Pos);
		return true;
	}

	void SetFriendData(Action CB)
	{
		UTILE.Load_Prefab_List(0, m_SUI.Friend.LoadPanel, m_SUI.Friend.Prefab);
		WEB.SEND_REQ_FRIEND((res) =>
		{
			CB?.Invoke();
		});
		//CB?.Invoke();
	}

	void StateChange(State state)
	{
		if (m_Page == state) return;

		CheckAlram();

		if (m_Page != State.None)
		{
			m_SUI.Tab[m_Page].Active.SetActive(false);
			m_SUI.Tab[m_Page].Menu.SetActive(false);
		}
		m_Page = state;
		switch (state)
		{
		case State.Friends:
			SetFriendData(SetFriendsUI);
			m_SUI.Friend.Scroll.verticalNormalizedPosition = 1f;
			break;
		case State.Find:
			m_SUI.Find.Scroll.verticalNormalizedPosition = 1f;
			m_SUI.Find.Input.text = "";
			SetRecommendUsers();
			break;
		case State.Invited:
			SetFriendData(SetInvitedUI);
			m_SUI.Invited.Scroll.verticalNormalizedPosition = 1f;
			break;
		}
		m_SUI.Tab[m_Page].Active.SetActive(true);
	}

	IEnumerator ListAction;
	void StartListAction(Transform panel)
	{
		if(ListAction != null)
		{
			StopCoroutine(ListAction);
			ListAction = null;
		}
		ListAction = ListAniAction(panel);
		StartCoroutine(ListAction);
	}

	IEnumerator ListAniAction(Transform panel)
	{
		// 제거까지 완료되려면 시간이 걸리므로
		yield return new WaitForEndOfFrame();
		// 중간에 수락등 제거되는 이슈가 발생하여 nullpoint 에러발생 연출 체크 방식 변경
		for(int i = panel.childCount - 1; i > -1; i--) panel.GetChild(i).GetComponent<Item_Friend_Element>().ViewStart(0.2f * (i + 1));
	}

	#region Find
	void SetFindUI_Base()
	{
		// 내 추천코드
		m_SUI.Find.MyCode.text = USERINFO.MyRefCode;

		// 내 동료 수
		m_SUI.Find.Cnt.text = string.Format(TDATA.GetString(655), USERINFO.m_Friend.Friends.Count(o => o.State == Friend_State.Friend), 30);
	}
	void SetFindUI()
	{
		var cnt = m_SUI.Find.Recommends.Count;
		UTILE.Load_Prefab_List(cnt, m_SUI.Find.LoadPanel, m_SUI.Find.Prefab);
		GridLayoutGroup group = m_SUI.Find.LoadPanel.GetComponent<GridLayoutGroup>();
		float W = m_SUI.Find.Scroll.content.rect.width;
		int vcnt = Mathf.RoundToInt((W - group.padding.left - group.padding.right) / group.cellSize.x);
		vcnt = (cnt + vcnt - 1) / vcnt;
		float H = group.padding.top + group.padding.bottom + vcnt * group.cellSize.y;
		if (vcnt > 0) H += (vcnt - 1) * group.spacing.y;
		m_SUI.Find.LoadPanel.sizeDelta = new Vector2(W, H);
		for(int i = 0; i < cnt; i++)
		{
			int pos = i % m_SUI.Pos.Length;
			m_SUI.Find.LoadPanel.GetChild(i).GetComponent<Item_Friend_Element>().SetData(Item_Friend_Element.BtnMode.Find, m_SUI.Find.Recommends[i], m_SUI.Pos[pos].Pos, m_SUI.Pos[pos].Rot, false, true, null);
		}

		SetFindUI_Base();

		StartListAction(m_SUI.Find.LoadPanel);
	}

	void SetRecommendUsers()
	{
		WEB.SEND_REQ_FRIEND_RECOMMEND((res) => {
			if (res.IsSuccess()) m_SUI.Find.Recommends = res.Users;
			SetFindUI();
		});
		SetFindUI_Base();
	}

	public void MyCodeCopy()
	{
		Utile_Class.Copy_Clipboard(USERINFO.MyRefCode);
		POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(661));
	}

	public void FindFriend()
	{
		string code = m_SUI.Find.Input.text;
		if(string.IsNullOrEmpty(code))
		{
			// 코드 입력이 안되어있을때
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(662));
			PlayCommVoiceSnd(VoiceType.Fail);
			return;
		}
		if(code.Equals(USERINFO.MyRefCode))
		{
			// 자신의 코드 입력
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(663));
			PlayCommVoiceSnd(VoiceType.Fail);
			return;
		}

		long user_no = Utile_Class.UserNoDecrypt(code);
		if(user_no == 0)
		{
			// 잘못된 코드일때
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(663));
			PlayCommVoiceSnd(VoiceType.Fail);
			return;
		}

		WEB.SEND_REQ_FRIEND_FIND((res) => {
			if (res.IsSuccess())
			{
				if (res.Users.Count > 0) POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_User, null, res.Users[0]);
				else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(663));
			}
			else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(663));
		}, code);
	}
	#endregion

	#region MyFriends

	void FriendUI(bool Start)
	{
		var list = USERINFO.m_Friend.Friends.FindAll(o => o.State == Friend_State.Friend);
		var cnt = list.Count;
		for (int i = 0; i < m_SUI.Friend.ButtomGroup.Length; i++) m_SUI.Friend.ButtomGroup[i].SetActive(true);

		m_SUI.Friend.ButtomGroup[0].GetComponent<Button>().interactable = cnt > 0 && list.Find(o => o.GetGiftState() == Friend_Gift_State.Gift || o.IsSendGift()) != null;

		UTILE.Load_Prefab_List(cnt, m_SUI.Friend.LoadPanel, m_SUI.Friend.Prefab);
		for (int i = 0; i < cnt; i++)
		{
			int pos = i % m_SUI.Pos.Length;
			m_SUI.Friend.LoadPanel.GetChild(i).GetComponent<Item_Friend_Element>().SetData(Item_Friend_Element.BtnMode.Friend, list[i], m_SUI.Pos[pos].Pos, m_SUI.Pos[pos].Rot, true, Start, SetFriendsResetUI);
		}

		// 내 동료 수
		m_SUI.Friend.Cnt.text = string.Format(TDATA.GetString(655), cnt, 30);

		var rev = USERINFO.m_Friend.Friends.FindAll(o => o.State == Friend_State.Friend || o.State == Friend_State.Deleted);
		m_SUI.Friend.ReciveCnt.text = string.Format("{0}/{1}", rev.Count(o => o.GetGiftState() == Friend_Gift_State.Get), TDATA.GetConfig_Int32(ConfigType.MaxFriendReceiveCount));

		if (cnt < 1)
		{
			m_SUI.Friend.Empty.SetActive(true);
			m_SUI.Friend.Scroll.gameObject.SetActive(false);
		}
		else
		{
			m_SUI.Friend.Empty.SetActive(false);
			m_SUI.Friend.Scroll.gameObject.SetActive(true);
			if(Start) StartListAction(m_SUI.Friend.LoadPanel);
		}
		CheckFriendsAlram(rev);
	}

	void SetFriendsUI()
	{
		FriendUI(true);
	}

	void SetFriendsResetUI()
	{
		FriendUI(false);
	}

	public void AllSend()
	{
		AllGive(() =>
		{
			AllGet(() =>
			{
				SetFriendsResetUI();
			});
		});
	}

	void AllGive(Action EndCB)
	{
		var friends = USERINFO.m_Friend.Friends.FindAll(o => o.State == Friend_State.Friend);
		// 보낼 수 있는 유저들
		var list = friends.FindAll(o => o.IsSendGift()).Select(o => o.UserNo).ToList();
		if(list.Count < 1)
		{
			EndCB?.Invoke();
			return;
		}
		WEB.SEND_REQ_FRIEND_GIFT_GIVE((res) => {
			PLAY.PlayEffSound(SND_IDX.SFX_0115);
			if(res.IsSuccess())
			{
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(781));
			}
			EndCB?.Invoke();
		}, list);
	}

	void AllGet(Action EndCB)
	{
		var friends = USERINFO.m_Friend.Friends.FindAll(o => o.State == Friend_State.Friend || o.State == Friend_State.Deleted);
		var cnt = friends.Count(o => o.GetGiftState() == Friend_Gift_State.Get);
		// 남은 획득 가능한 개수
		var max = TDATA.GetConfig_Int32(ConfigType.MaxFriendReceiveCount) - cnt;
		if (max < 1)
		{
			EndCB?.Invoke();
			return;
		}

		var list = friends.FindAll(o => o.State == Friend_State.Friend && o.GetGiftState() == Friend_Gift_State.Gift).Select(o => o.UserNo).ToList();
		cnt = list.Count;
		if (cnt < 1)
		{
			EndCB?.Invoke();
			return;
		}
		list = list.GetRange(0, Math.Min(cnt, max));

		WEB.SEND_REQ_FRIEND_GIFT_GET((res) => {
			if (res.Rewards != null)
			{
				List<RES_REWARD_BASE> Rewards = res.GetRewards();
				if (Rewards.Count > 0)
				{
					MAIN.SetRewardList(new object[] { Rewards }, () => {});
				}
			}
			EndCB?.Invoke();
		}, list);
	}
	#endregion

	#region Invited

	void InvateUI(bool Start)
	{
		var list = USERINFO.m_Friend.Friends.FindAll(o => o.State == Friend_State.Idle);
		var cnt = list.Count;
		UTILE.Load_Prefab_List(cnt, m_SUI.Invited.LoadPanel, m_SUI.Invited.Prefab);
		for (int i = 0; i < cnt; i++)
		{
			int pos = i % m_SUI.Pos.Length;
			m_SUI.Invited.LoadPanel.GetChild(i).GetComponent<Item_Friend_Element>().SetData(Item_Friend_Element.BtnMode.Invite, list[i], m_SUI.Pos[pos].Pos, m_SUI.Pos[pos].Rot, false, Start, SetInvitedResetUI);
		}

		// 내 동료 수
		m_SUI.Invited.Cnt.text = string.Format(TDATA.GetString(655), USERINFO.m_Friend.Friends.Count(o => o.State == Friend_State.Friend), 30);
		if (cnt < 1)
		{
			m_SUI.Invited.Empty.SetActive(true);
			m_SUI.Invited.Scroll.gameObject.SetActive(false);
		}
		else
		{
			m_SUI.Invited.Empty.SetActive(false);
			m_SUI.Invited.Scroll.gameObject.SetActive(true);
			if(Start) StartListAction(m_SUI.Invited.LoadPanel);
		}
		CheckInvitedAlram(cnt);
	}
	void SetInvitedUI()
	{
		InvateUI(true);
	}

	void SetInvitedResetUI()
	{
		// 수락 제거만있으므로 연출 다시안하고 친구수만 다시하기
		m_SUI.Invited.Cnt.text = string.Format(TDATA.GetString(655), USERINFO.m_Friend.Friends.Count(o => o.State == Friend_State.Friend), 30);
		//InvateUI(false);
	}
	#endregion
}
