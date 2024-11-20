using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PDA_Menu : ObjMng
{
	public enum State
	{
		/// <summary> 메인 화면 </summary>
		Main = 0,
		/// <summary> 탐사 </summary>
		Adventure,
		/// <summary> 연구 </summary>
		Research,
		/// <summary> 제작 </summary>
		Making,
		/// <summary> 좀비 키우기 </summary>
		ZombieFarm,
		/// <summary> 업적 </summary>
		Achieve,
		/// <summary> 업적 </summary>
		Ranking,
		/// <summary> 옵션 </summary>
		Option,
		/// <summary>  </summary>
		End
	}
	public enum Ani
	{
		/// <summary> 시작 </summary>
		Start = 0,
		/// <summary> 종료 </summary>
		End
	}
	[Serializable]
	public struct SUserInfoUI
	{
		public Image Profile;
		public Image ExpGuage;
		public TextMeshProUGUI Exp;
		public Text Name;
		public TextMeshProUGUI LV;

	}

	[Serializable]
	public struct SUI
	{
		public Animator[] Anim;
		public GameObject MainMenu;
		public Transform MenuPanel;
		[ReName("Adventure", "Research", "Making", "Zombie", "Achieve", "Option")]
		public GameObject[] Alrams;
		public GameObject[] MenuLocks;
		public GameObject[] MenuLockFX;
		public SUserInfoUI UserInfo;
		public TextMeshProUGUI Ver;

		public RectTransform NoticePanel;
		[ReName("ZombieResearchExp")]
		public GameObject[] Notices;

		[Header("튜토리얼 전용")]
		public GameObject[] MenuBtns;
	}
	[SerializeField] SUI m_SUI;
	public State m_State = State.End;
	Item_PDA_Base m_MenuItem = null;
	public Item_PDA_Base GetMenuObj { get { return m_MenuItem; } }


	public bool OnBack()
	{
		if (m_MenuItem == null) return false;
		return m_MenuItem.OnBack();
	}

	/// <summary> 메인화면으로 돌리기 </summary>
	public void SetData() {
		SetUserInfoUI();
		InitNotice();
		StateChange(State.Main);
		for(int i = 0;i< m_SUI.MenuLockFX.Length; i++) {
			m_SUI.MenuLockFX[i].SetActive(false);
		}

		m_SUI.Ver.text = APPINFO.m_strVersion;
	}
	public void InitNotice() {
		for (int i = m_SUI.NoticePanel.childCount - 1; i > -1; i--) {
			Destroy(m_SUI.NoticePanel.GetChild(i).gameObject);
		}
	}
	void SetUserInfoUI() {
		m_SUI.UserInfo.Profile.sprite = TDATA.GetUserProfileImage(USERINFO.m_Profile);
		m_SUI.UserInfo.Name.text = string.Format(TDATA.GetString(298), USERINFO.m_Name);
		m_SUI.UserInfo.LV.text = string.Format("Lv. {0}", USERINFO.m_LV.ToString("N0"));
		m_SUI.UserInfo.ExpGuage.fillAmount = USERINFO.m_Exp[(int)EXPType.User] / TDATA.GetExpTable(USERINFO.m_LV).m_UserExp;
		var exdata = TDATA.GetExpTable(USERINFO.m_LV);
		m_SUI.UserInfo.Exp.text = string.Format("{0} / {1}", Math.Min(USERINFO.m_Exp[(int)EXPType.User], exdata.m_UserExp).ToString("N0"), exdata.m_UserExp.ToString("N0"));
	}
	public IEnumerator EndCheck()
	{
		//m_SUI.Anim[0].SetTrigger("End");
		//yield return Utile_Class.CheckAniPlay(m_SUI.Anim[0]);
		m_State = State.End;
		yield break;
	}

	public void CheckAlram()
	{
		m_SUI.Alrams[0].SetActive(USERINFO.IsCompAdventuring() && USERINFO.CheckContentUnLock(ContentType.Explorer, false));
		m_SUI.Alrams[1].SetActive((USERINFO.IsCompResearching(ResearchType.Remodeling) || USERINFO.IsCompResearching(ResearchType.Research) 
			|| USERINFO.IsCompResearching(ResearchType.Training)) && USERINFO.CheckContentUnLock(ContentType.Research, false));

		List<TMakingTable> alltdata = TDATA.GetAllMakingTable();
		int LV = Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.MakingLevelUp));
		List<TMakingTable> setmaketdata = TDATA.GetAllMakingTable().FindAll(o => o.m_Group == MakingGroup.PrivateEquip && o.m_LV == LV);
		bool cansetmak = setmaketdata.Find(o => o.GetCanMake()) != null && USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= BaseValue.CHAREQUIP_MAKING_OPEN;
		m_SUI.Alrams[2].SetActive((USERINFO.IsCompMaking() || cansetmak) && USERINFO.CheckContentUnLock(ContentType.Making, false));
		m_SUI.Alrams[3].SetActive((USERINFO.IsCompZombieFarm() || USERINFO.IS_CanMakeAnyDNA())&& USERINFO.CheckContentUnLock(ContentType.ZombieFarm, false));
		m_SUI.Alrams[4].SetActive(USERINFO.m_Achieve.IsAlram() || USERINFO.m_Collection.IsAlram());
		m_SUI.Alrams[5].SetActive(PlayerPrefs.GetInt($"OptionAuthAlarm_{USERINFO.m_UID}", 0) == 0);
		DLGTINFO?.f_RFPDABtnUI?.Invoke();
	}

	public void StateChange(object state, params object[] args) {
		State BState = m_State;
		// 임시 제작 버튼
		m_State = (State)state;
		m_SUI.MainMenu.SetActive(m_State == State.Main);
		m_SUI.MenuPanel.gameObject.SetActive(m_State != State.Main);
		switch(m_State)
		{
		case State.Main:
			m_SUI.MenuLocks[0].SetActive(!USERINFO.CheckContentUnLock(ContentType.Explorer, false));
			m_SUI.MenuLocks[1].SetActive(!USERINFO.CheckContentUnLock(ContentType.Research, false));
			m_SUI.MenuLocks[2].SetActive(!USERINFO.CheckContentUnLock(ContentType.Making, false));
			m_SUI.MenuLocks[3].SetActive(!USERINFO.CheckContentUnLock(ContentType.ZombieFarm, false));
			for (int i = m_SUI.MenuPanel.childCount - 1; i > -1; i--) Destroy(m_SUI.MenuPanel.GetChild(i).gameObject);
			CheckAlram();
			StartCoroutine(StartAction(BState));
			break;
		case State.Adventure:
			m_MenuItem = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Adventure", true, m_SUI.MenuPanel).GetComponent<Item_PDA_Adventure>();
			break;
		case State.Research:
			m_MenuItem = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Research", true, m_SUI.MenuPanel).GetComponent<Item_PDA_Research>();
			break;
		case State.Making:
			m_MenuItem = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Making", true, m_SUI.MenuPanel).GetComponent<Item_PDA_Making>();
			break;
		case State.ZombieFarm:
			m_MenuItem = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_ZombieFarm", true, m_SUI.MenuPanel).GetComponent<Item_PDA_ZombieFarm>();
			break;
		case State.Achieve:
			m_MenuItem = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Achieve", true, m_SUI.MenuPanel).GetComponent<Item_PDA_Achieve>();
			break;
		case State.Ranking:
			WEB.SEND_REQ_RANKING((res) =>
			{
				if(res.IsSuccess())
				{
					m_MenuItem = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Ranking_Main", true, m_SUI.MenuPanel).GetComponent<Item_PDA_Ranking_Main>();
					m_MenuItem.SetData(StateChange, new object[] { RankType.Power, res });
				}
				else StateChange(Item_PDA_Menu.State.Main, null);
			}, RankType.Power);
			return;
		case State.Option:
			m_MenuItem = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Option", true, m_SUI.MenuPanel).GetComponent<Item_PDA_Option>();
			break;
			
		}

		if(m_MenuItem != null) m_MenuItem.SetData(StateChange, args);
	}
	public void SetLockFX(ContentType _type) {
		switch (_type) {
			case ContentType.Explorer: m_SUI.MenuLockFX[0].SetActive(true);break;
			case ContentType.Research: m_SUI.MenuLockFX[1].SetActive(true); break;
			case ContentType.Making: m_SUI.MenuLockFX[2].SetActive(true); break;
			case ContentType.ZombieFarm: m_SUI.MenuLockFX[3].SetActive(true); break;
		}
		PlayEffSound(SND_IDX.SFX_0190);
	}
	public void ClickMenu(int state) {

		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_PDA_MainMenu, (State)state)) return;
		switch((State)state)
		{
		case State.Adventure:
			//if (USERINFO.ResearchValue(ResearchEff.AdventureOpen) == 0) return;
			if (!USERINFO.CheckContentUnLock(ContentType.Explorer, true)) {
				PlayCommVoiceSnd(VoiceType.Fail);
				return;
			}
#if NOT_USE_NET
			USERINFO.Check_AdvList();
			PlayEffSound(SND_IDX.SFX_0121);
			StateChange((State)state);
#else
			WEB.SEND_REQ_ADVINFO((res) =>
			{
				if (!res.IsSuccess())
				{
					WEB.StartErrorMsg(res.result_code, (btn, obj) => {});
					return;
				}
				PlayEffSound(SND_IDX.SFX_0121);
				StateChange((State)state);
			});
#endif
			return;
		case State.Making:
			if (!USERINFO.CheckContentUnLock(ContentType.Making, true)) {
				PlayCommVoiceSnd(VoiceType.Fail);
				return;
			}
			//if (USERINFO.ResearchValue(ResearchEff.MakingOpen) == 0) return;
			break;
		case State.ZombieFarm:
			if(!USERINFO.CheckContentUnLock(ContentType.ZombieFarm, true)) {
				PlayCommVoiceSnd(VoiceType.Fail);
				return;
			}
			break;
		case State.Research:
			if (!USERINFO.CheckContentUnLock(ContentType.Research, true)) {
				PlayCommVoiceSnd(VoiceType.Fail);
				return;
			}
			break;
		}

		PlayEffSound(SND_IDX.SFX_0121);
		StateChange((State)state);
	}

	IEnumerator StartAction(State BState)
	{
		m_SUI.Anim[1].SetTrigger(BState == State.End ? "Ready" : "Normal");
		if (BState == State.End) PlayEffSound(SND_IDX.SFX_0120);
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim[1]);
		if (TUTO.IsTuto(TutoKind.Making, (int)TutoType_Making.PDA_Action)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Research, (int)TutoType_Research.PDA_Action)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Zombie, (int)TutoType_Zombie.PDA_Action)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.DNA_Make, (int)TutoType_DNA_Make.PDA_Action)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Adventure, (int)TutoType_Adventure.PDA_Action)) TUTO.Next();
	}

	public GameObject GetMainMenu(State state)
	{
		return m_SUI.MenuBtns[(int)state - 1];
	}

	public GameObject GetMenuItem()
	{
		return m_SUI.MenuPanel.GetChild(0).gameObject;
	}

	public void ChangeProfile()
	{
		TStageTable stgtable = TDATA.GetStageTable(201, 0);
		if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < 201) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(273), stgtable.m_Idx / 100, stgtable.m_Idx % 100, stgtable.GetName()));
			return;
		}
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.UserProfile, (result, obj) => {
			m_SUI.UserInfo.Profile.sprite = TDATA.GetUserProfileImage(USERINFO.m_Profile);
			m_SUI.UserInfo.Name.text = string.Format(TDATA.GetString(298), USERINFO.m_Name); 
			DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);
		}, false);
	}
	public void OnCopyUID()
	{
#if UNITY_EDITOR || NOT_USE_NET || TEST_SERVER || DEV_SERVER || OUT_SERVER || LOCAL_SERVER
		Utile_Class.Copy_Clipboard(USERINFO.m_UID.ToString());
#else
		Utile_Class.Copy_Clipboard(USERINFO.MyRefCode);
#endif
	}
}
