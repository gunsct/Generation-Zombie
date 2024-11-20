
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static LS_Web;

public class DeckSetting : PopupBase
{
	public enum State
	{
		None,
		CardDrag,
		Hold,
		GoStage
	}
	[Serializable]
	public struct SSUI
	{
		public GameObject[] UnlockObjs;
		public GameObject LockObj;
	}
	[Serializable]
	public struct ISUI
	{
		public Image[] InfoIcon;			//0:전투력 ,1:턴, 2:시간
		public TextMeshProUGUI[] InfoVal;	//0:전투력, 1:턴,2:시간
		public GameObject[] GoalObj;
		public Image[] GoalBG;
		public Material GoalBGMat;
		public TextMeshProUGUI[] GoalTxt;
		public GameObject[] GimmikBucket;
		public GameObject[] GimmikGroups;
		public Image[] GimmickCard;
		public TextMeshProUGUI[] GimmickName;
	}
	[Serializable]
	public struct RSUI
	{
		public GameObject Group;
		public Item_RewardList_Item[] Rewards;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		[Header("스테이지")]
		public TextMeshProUGUI ChapterNum;
		public TextMeshProUGUI StageNum;
		public Transform BackGroup;
		public Animator LightAnim;
		public RectTransform FrontReck;
		public Animator FrontGroupAnim;
		public GameObject[] WarningBtnFx;
		public ISUI m_Info;
		public GameObject NoRescueMsg;
		public RSUI Reward;
		[Header("덱")]
		public Transform PageGroup;
		public Animator PageAnim;
		public TextMeshProUGUI RecommandCP;
		public TextMeshProUGUI CPDanger;
		public Animator DeckCPAnim;
		public TextMeshProUGUI DeckCP;
		public TextMeshProUGUI[] DeckSurvStat;
		public SSUI[] m_Stats;
		public Item_CharDeckCard[] DeckCards;
		public Item_PowerMark[] DeckPowerMark;
		public GameObject[] SlotLock;
		public Item_Synergy_Card[] SynergyName;
		public GameObject NotSynergy;
		public GameObject DeckPage;
		public ScrollRect SurvScroll;
		public GameObject TitleGroup;
		public GameObject ListOnBtn;
		public GameObject ListOffBtn;
		public Animator ListOnBtnAnim;
		public Animator ListOffBtnAnim;
		public Animator[] SortGroupAnim;
		[Header("그외Animator")]
		public TextMeshProUGUI[] Energy;
		public Button[] GoBtn;
		public Item_SortingGroup SortingGroup;
		public GameObject Empty;
		[Header("생존자 목록")]
		public GameObject CharCardPrefab;
		public Transform CharObjBucket;
		[Header("튜토리얼용")]
		[ReName("Synergy", "StageStart", "ListOn", "ListOff", "StageStartZeroEnergy")]
		public GameObject[] Btns;
		public GameObject TabGroupTuto;
		public GameObject TabFront;
		public GameObject TabBack;
		public GameObject TutoLock;
		public GameObject SlotGroup;
		public GameObject Synergy;
	}
	[SerializeField]
	SUI m_SUI;
#pragma warning disable 0414
	State m_State = State.None;
#pragma warning restore 0162
	TStageTable m_TData_Stage;
	List<Item_CharManageCard> m_AllChar = new List<Item_CharManageCard>();
	Item_CharManageCard m_SelectChar;
	[SerializeField] Item_CharDeckCard m_SelectDeckChar;

	GraphicRaycaster m_GR;
	[SerializeField] float m_HoldTime = 0f;     //드래그드랍 조건 시간
	int m_DeckSlotCnt;
	int m_PreDeckCP;
	int m_RecommandCP;
	int[] m_PreSurvStat = new int[3];
	RectTransform m_TrPageGroup { get { return (RectTransform)m_SUI.PageGroup.transform; } }
	float m_PageMoveAmount { get { return m_TrPageGroup.rect.height; } }
	bool m_CanClick = false;
	float m_ClickDelay;
	StageContentType m_Content;
	public bool IS_Event;
	public StageContentType GetContentType { get { return m_Content; } }
	TStageExceptTable m_Exceptdata { get { return TDATA.GeTStageExceptTable(m_TData_Stage.m_Idx, (StageDifficultyType)m_TData_Stage.m_Difficulty); } }
	IEnumerator m_Action; //end ani check

	[SerializeField] ETouchState m_TouchState;
	double m_PressTime;
	Vector3 m_TouchPoint;
	Vector3 m_MoveGap;
	Vector3 m_SwipGap;
	bool m_Dir;
	List<int> m_NeedChars = new List<int>();
	public GameObject GetDeckSlot { get { return m_SUI.SlotGroup; } }
	public GameObject GetSynergy { get { return m_SUI.Synergy; } }
	public GameObject GetCharListOnBtn { get { return m_SUI.Btns[2]; } }
	public GameObject GetCharListOffBtn { get { return m_SUI.Btns[3]; } }
	public GameObject GetChar1013 { get { return m_AllChar.Find(o => o.m_Info.m_Idx == 1013).gameObject; } }
	public GameObject GetChar1021 { get { return m_AllChar.Find(o => o.m_Info.m_Idx == 1021).gameObject; } }
	public GameObject GetChar1033 { get { return m_AllChar.Find(o => o.m_Info.m_Idx == 1033).gameObject; } }
	public GameObject GetChar1052 { get { return m_AllChar.Find(o => o.m_Info.m_Idx == 1052).gameObject; } }
	public GameObject GetStageInfo { get { return m_SUI.m_Info.GimmickCard[0].gameObject; } }
	public GameObject GetSlot(int _pos) { return m_SUI.DeckCards[_pos].gameObject; }
	bool IsClonDeck;

	private void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFCharInfoCard += CharCardRefresh;
			DLGTINFO.f_RFDeckCharInfoCard += DeckCharCardRefresh;
		}

		for (int i = 0; i < m_SUI.SlotLock.Length; i++) m_SUI.SlotLock[i].SetActive(false);
		for(int i = 0; i < m_SUI.m_Info.GimmikGroups.Length; i++) m_SUI.m_Info.GimmikGroups[i].SetActive(false);
		m_SUI.Empty.SetActive(false);
		m_SUI.NotSynergy.SetActive(false);
		//m_SUI.TitleGroup.SetActive(false);

		for (int i = 0; i < m_SUI.SortGroupAnim.Length; i++) {
			m_SUI.SortGroupAnim[i].SetTrigger("Off");
		}
		//튜토용
		m_SUI.TabGroupTuto.SetActive(false);
		m_SUI.TutoLock.SetActive(false);
	}
	private void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFCharInfoCard -= CharCardRefresh;
			DLGTINFO.f_RFDeckCharInfoCard -= DeckCharCardRefresh;
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		IsClonDeck = TUTO.CheckUseCloneDeck();
		base.SetData(pos, popup, cb, aobjValue);

		m_TData_Stage = (TStageTable)aobjValue[0];
		m_Content = (StageContentType)aobjValue[1];
		USERINFO.SetDeckIdx((StageContentType)aobjValue[1], (DayOfWeek)aobjValue[2], (int)aobjValue[3]);
		IS_Event = aobjValue.Length > 4 ? (bool)aobjValue[4] : false;
		//지하철은 권장 전투력 공식으로 대체
		m_RecommandCP = m_Content == StageContentType.Subway ? BaseValue.GetNeedCombatPower(USERINFO.m_LV) : m_TData_Stage.m_RecommandCP;

		m_SUI.NoRescueMsg.SetActive(m_TData_Stage.m_Mode != StageModeType.Stage || m_Content != StageContentType.Stage || m_TData_Stage.m_NoRescue || m_TData_Stage.m_Fail.m_Type == StageFailType.TurmoilCount);

		//튜토 시작할때 덱 다 빼기
		if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Focus_StartBtn)) {
			for (int i = 0; i < 5; i++) {
				CharInfo charinfo = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]);
				if (charinfo != null && charinfo.m_Idx == 1052) USERINFO.m_PlayDeck.m_Char[i] = 0;
			}
			USERINFO.m_PlayDeck.SetSynergy();
			TUTO.Next();
		}
		else if (TUTO.IsTuto(TutoKind.DeckSynergy, (int)TutoType_DeckSynergy.Decksetting_Delay)) {
			for (int i = 0; i < 5; i++) {
				USERINFO.m_PlayDeck.m_Char[i] = i == 0 ? USERINFO.GetChar(1033).m_UID : 0;
			}
			USERINFO.m_PlayDeck.SetSynergy();
		}
		else if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.DeckSorting)) {
			for (int i = 0; i < 5; i++) {
				USERINFO.m_PlayDeck.m_Char[i] = 0;
			}
			USERINFO.m_PlayDeck.SetSynergy();
		}
		//스테이지 정보
		m_SUI.DeckPage.SetActive((StageContentType)aobjValue[1] == StageContentType.Stage && !IS_Event);
		m_SUI.m_Info.InfoIcon[0].color = Utile_Class.GetCodeColor(m_Content != StageContentType.Factory ? "#FFFFFF" : "#5C5C5C");
		m_SUI.m_Info.InfoVal[0].text = m_Content != StageContentType.Factory ? TDATA.GetString((StageContentType)aobjValue[1] == StageContentType.Stage ? 151 : 685) : "-";
		m_SUI.m_Info.InfoVal[0].color = Utile_Class.GetCodeColor(m_Content != StageContentType.Factory ? "#B5B5AD" : "#5C5C5C");

		m_SUI.m_Info.InfoIcon[1].color = Utile_Class.GetCodeColor(m_TData_Stage.m_LimitTurn > 0 ? "#B5B5AD" : "#5C5C5C");
		m_SUI.m_Info.InfoVal[1].text = m_TData_Stage.m_LimitTurn > 0 ? m_TData_Stage.m_LimitTurn.ToString() : "-";
		m_SUI.m_Info.InfoVal[1].color = Utile_Class.GetCodeColor(m_TData_Stage.m_LimitTurn > 0 ? "#B5B5AD" : "#5C5C5C");

		m_SUI.m_Info.InfoIcon[2].color = Utile_Class.GetCodeColor(m_TData_Stage.m_Fail.m_Type == StageFailType.Time ? "#B5B5AD" : "#5C5C5C");
		m_SUI.m_Info.InfoVal[2].text = m_TData_Stage.m_Fail.m_Type == StageFailType.Time ? UTILE.GetSecToTimeStr(0, m_TData_Stage.m_Fail.m_Value) : "-";
		m_SUI.m_Info.InfoVal[2].color = Utile_Class.GetCodeColor(m_TData_Stage.m_Fail.m_Type == StageFailType.Time ? "#B5B5AD" : "#5C5C5C");


		for (int i = 0; i < m_SUI.m_Info.GoalObj.Length; i++) {
			if (i >= m_TData_Stage.m_Clear.Count) {
				m_SUI.m_Info.GoalObj[i].gameObject.SetActive(false);
				continue;
			}
			if (i > 0 && m_TData_Stage.m_ClearMethod == ClearMethodType.Continuity) {
				m_SUI.m_Info.GoalTxt[i].text = TDATA.GetString(1054);
				m_SUI.m_Info.GoalTxt[i].color = Utile_Class.GetCodeColor("#656565");
				m_SUI.m_Info.GoalBG[i].color = Utile_Class.GetCodeColor("#0E0E0E");
				m_SUI.m_Info.GoalBG[i].material = null;
			}
			else {
				TStageCondition<StageClearType> clear = m_TData_Stage.m_Clear[i];
				TGuideTable gdata = TDATA.GetGuideTable(clear.m_Type);
				string desctxt = string.Empty;
				string cnttxt = string.Empty;
				switch (clear.m_Type) {
					case StageClearType.KillEnemy_Turn:
						desctxt = string.Format(m_TData_Stage.GetClearDesc(i), clear.m_Value);
						break;
					default:
						desctxt = m_TData_Stage.GetClearDesc(i);
						break;
				}
				cnttxt = string.Format("(0/{0})", clear.m_Cnt);
				m_SUI.m_Info.GoalTxt[i].text = string.Format("{0} {1}", desctxt, cnttxt);
				m_SUI.m_Info.GoalTxt[i].color = Utile_Class.GetCodeColor("#B5B5AD");
				m_SUI.m_Info.GoalBG[i].color = Utile_Class.GetCodeColor("#009F1F");
				m_SUI.m_Info.GoalBG[i].material = m_SUI.m_Info.GoalBGMat;
			}
		}


		for (int i = 0; i < m_TData_Stage.m_Gimmick.Count; i++) {
			m_SUI.m_Info.GimmickCard[i].sprite = m_TData_Stage.GetGimmickImg(i);
			m_SUI.m_Info.GimmickName[i].text = m_TData_Stage.GetGimminkName(i);
			if (m_TData_Stage.m_Gimmick[i].Name == 0) m_SUI.m_Info.GimmikGroups[i].gameObject.SetActive(false);
		}
		StartCoroutine(GimmickAction());
		for (int i = 0; i < m_SUI.m_Info.GimmikBucket.Length; i++) m_SUI.m_Info.GimmikBucket[i].SetActive(m_TData_Stage.m_Gimmick.Count > 0);

		SetReward();

		m_SUI.SortingGroup.SetData(SetSort);
		string chapnum = string.Empty;
		if (TDATA.GetModeTable(m_TData_Stage.m_Idx) == null) {
			chapnum = string.Format("CHAPTER {0}", Mathf.RoundToInt(m_TData_Stage.m_Idx / 100));
		}
		else {
			chapnum = m_TData_Stage.GetName();
		}
		if (IS_Event) {
			m_SUI.ChapterNum.text = TDATA.GetString(2017);
		}
		else m_SUI.ChapterNum.text = chapnum;

		TModeTable towertdata = TDATA.GetModeTable(m_TData_Stage.m_Idx);
		int stagenum = Mathf.RoundToInt(m_TData_Stage.m_Idx % (towertdata != null && towertdata.m_Content == StageContentType.Tower ? 1000 : 100));
		m_SUI.StageNum.text = string.Format("{0} {1}", TDATA.GetString(83), stagenum);

		switch (USERINFO.GetDifficulty()) {
			case 0: m_SUI.LightAnim.SetTrigger("Normal"); break;
			case 1: m_SUI.LightAnim.SetTrigger("Hard"); break;
			case 2: m_SUI.LightAnim.SetTrigger("Nightmare"); break;
		}

		if (IsClonDeck) m_DeckSlotCnt = USERINFO.m_PlayDeck.GetDeckCharCnt();
		else m_DeckSlotCnt = m_TData_Stage.m_DeckCharLimit;


		for (int i = 0; i < 5 - m_DeckSlotCnt; i++) m_SUI.SlotLock[i].SetActive(true);

		for (int i = 0; i < m_SUI.DeckCards.Length; i++) {
			m_SUI.DeckCards[i].m_Pos = i;
			//if (i > 2) {//최소 3칸
				m_SUI.DeckCards[i].gameObject.SetActive(m_DeckSlotCnt > i);
			//}
			if(i > m_DeckSlotCnt - 1)  USERINFO.m_PlayDeck.SetChar(i, 0);
		}

		m_GR = POPUP.GetComponent<GraphicRaycaster>();//캔버스 레이케스트용

		//필요캐릭터 세팅
		SetNeedChars(false);
		//모든 캐릭터와 유저가 보유한 캐릭터 세팅
		for (int i = 0; i < TDATA.GetAllCharacterTable().Count; i++) {
			int idx = TDATA.GetAllCharacterTable().ElementAt(i).Key;
			Item_CharManageCard card = null;
			List<CharInfo> charinfos = new List<CharInfo>();
			if (IsClonDeck) charinfos = TUTO.m_CloneChars;
			else charinfos = USERINFO.m_Chars;
			if (charinfos.Find((t) => t.m_Idx == idx) != null) {//획득 캐릭터
				card = Utile_Class.Instantiate(m_SUI.CharCardPrefab, m_SUI.CharObjBucket).GetComponent<Item_CharManageCard>();
				card.SetData(idx, m_AllChar, IS_Event ? Item_CharManageCard.Mode.Event : Item_CharManageCard.Mode.DeckSetting, Click_CB);
				card.transform.SetAsFirstSibling();
				card.SetCheckExcept(m_Exceptdata != null && m_Exceptdata.m_Chars.Contains(idx) ? true : false);
				if (m_TData_Stage.m_NeedChars.Contains(card.m_Info.m_Idx)) card.SetCheckNeed(true);
				else {
					for (int j = 0; j < m_TData_Stage.m_RecommendJob.Count; j++) {
						if (card.m_Info.m_TData.m_Job.Contains(m_TData_Stage.m_RecommendJob[j])) {
							card.SetCheckRecommand(true);
							break;
						}
					}
				}
			}
			if(card != null)
				m_AllChar.Add(card);
		}
		//필수 캐릭터 자동세팅, USERINFO.m_SelectDeck 기준 아마 마지막 사용한 덱포스일듯
		SetAutoNeed(USERINFO.m_SelectDeck);
		//덱 페이지 세팅
		SetPageDeck(USERINFO.m_SelectDeck);
		if (m_Content == StageContentType.Stage && !IS_Event) m_SUI.FrontGroupAnim.SetTrigger(string.Format("Tab_{0}", USERINFO.m_SelectDeck + 1));

		iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", m_RecommandCP, "onupdate", "TW_RecommandCP", "time", 1f));

		SetStartBtn();
		m_SUI.Energy[1].text = m_SUI.Energy[0].text = string.Format("<size=140%>-</size>{0}",  m_TData_Stage.m_Energy);

		m_SUI.ListOnBtnAnim.SetTrigger("On");
		m_SUI.ListOffBtnAnim.SetTrigger("Off");

		for (int i = 0; i < m_SUI.SortGroupAnim.Length; i++) {
			m_SUI.SortGroupAnim[i].SetTrigger("Off");
		}
		//m_SUI.TitleGroup.SetActive(false);
		if (IsClonDeck) {
			m_SUI.TabGroupTuto.SetActive(true);
			m_SUI.TabFront.SetActive(false);
			m_SUI.TabBack.SetActive(false);
			//m_SUI.TitleGroup.SetActive(false);
			m_SUI.ListOnBtn.SetActive(false);
			m_SUI.ListOffBtn.SetActive(false);
			m_SUI.TutoLock.SetActive(true);
		}
		//if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Focus_StartBtn)) TUTO.Next();
		if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Focus_StartBtn)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Decksetting_Delay)) {
			for (int i = 0; i < 5; i++) {
				if (m_SUI.DeckCards[i].m_Info != null && m_SUI.DeckCards[i].m_Info.m_Idx == 1052) {
					SlotOut(i);
					break;
				}
			}
			TUTO.Next();
		}
		//else if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.Decksetting_Delay)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.DeckSynergy, (int)TutoType_DeckSynergy.Decksetting_Delay)) {
			for (int i = 0; i < 5; i++) {
				if (m_SUI.DeckCards[i].m_Info != null && m_SUI.DeckCards[i].m_Info.m_Idx == 1013) {
					SlotOut(i);
					break;
				}
			}
			TUTO.Next();
		}
		//else if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.Decksetting_Delay)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.DeckCharInfo, (int)TutoType_DeckCharInfo.Decksetting_Delay)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_204, (int)TutoType_Stage_204.Decksetting_Delay)) TUTO.Next();
		else if (m_TData_Stage.m_NeedChars.Count > 0 && m_Content != StageContentType.Stage) {
			var idxs = USERINFO.m_Deck[USERINFO.m_SelectDeck].m_Char.Select(o => USERINFO.GetChar(o)?.m_Idx).ToList();
			var contains = m_TData_Stage.m_NeedChars.FindAll(o => idxs.Contains(o));
			if (m_TData_Stage.m_NeedChars.Count != contains.Count) {
				for (int i = 0; i < 5; i++) {
					if (m_SUI.DeckCards[i].m_Info != null && !m_TData_Stage.m_NeedChars.Contains(m_SUI.DeckCards[i].m_Info.m_Idx)) {
						SlotOut(i);
					}
				}
			}
		}
	}

	IEnumerator GimmickAction() {
		for(int i = 0;i< m_TData_Stage.m_Gimmick.Count; i++) {
			yield return new WaitForSeconds((40f + 10f * i) / 86f);
			m_SUI.m_Info.GimmikGroups[i].SetActive(true);
		}
	}

	void SetReward() {
		if(m_Content != StageContentType.Stage) {
			m_SUI.Reward.Group.SetActive(false);
			return;
		}

		List<RES_REWARD_BASE> getrewards = new List<RES_REWARD_BASE>();

		if (m_TData_Stage.m_ClearExp > 0) {
			RES_REWARD_MONEY exp = new RES_REWARD_MONEY();
			exp.Type = Res_RewardType.Exp;
			exp.Befor = USERINFO.m_Exp[0] - m_TData_Stage.m_ClearExp;
			exp.Now = USERINFO.m_Exp[0];
			exp.Add = (int)(exp.Now - exp.Befor);
			getrewards.Add(exp);
		}
		//if (table.m_ClearUserExp > 0) {
		//	RES_REWARD_USEREXP userexp = new RES_REWARD_USEREXP();
		//	userexp.AExp = table.m_ClearUserExp;
		//	userexp.BExp = USERINFO.m_Exp[(int)EXPType.User];
		//	userexp.BLV = USERINFO.m_LV;
		//	getrewards.Add(userexp);
		//}

		if (m_TData_Stage.m_ClearMoney > 0) {
			RES_REWARD_MONEY money = new RES_REWARD_MONEY();
			money.Type = Res_RewardType.Money;
			money.Befor = USERINFO.m_Money - m_TData_Stage.m_ClearMoney;
			money.Now = USERINFO.m_Money;
			money.Add = (int)(money.Now - money.Befor);
			getrewards.Add(money);
		}

		if (m_TData_Stage.m_ClearGold > 0) {
			RES_REWARD_MONEY cash = new RES_REWARD_MONEY();
			cash.Type = Res_RewardType.Cash;
			cash.Befor = USERINFO.m_Cash - m_TData_Stage.m_ClearGold;
			cash.Now = USERINFO.m_Cash;
			cash.Add = (int)(cash.Now - cash.Befor);
			getrewards.Add(cash);
		}

		for (int i = 0; i < m_TData_Stage.m_ClearReward.Count; i++) {
			switch (m_TData_Stage.m_ClearReward[i].m_Kind) {
				case RewardKind.None:
					break;
				case RewardKind.Character:
					CharInfo charinfo = USERINFO.m_Chars.Find(t => t.m_Idx == m_TData_Stage.m_ClearReward[i].m_Idx);
					if (charinfo != null) {
						getrewards.Add(new RES_REWARD_ITEM() {
							Type = Res_RewardType.Item,
							Idx = charinfo.m_TData.m_PieceIdx,
							Grade = charinfo.m_TData.m_Grade,
							Cnt = TDATA.GetItemTable(charinfo.m_TData.m_PieceIdx).GetEquipType() == EquipType.End ? BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade) : 1,
							result_code = EResultCode.SUCCESS_REWARD_PIECE
						});
					}
					else {
						RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
						rchar.SetData(charinfo);
						getrewards.Add(rchar);
					}
					break;
				case RewardKind.Item:
					TItemTable tdata = TDATA.GetItemTable(m_TData_Stage.m_ClearReward[i].m_Idx);
					if (tdata.m_Type == ItemType.RandomBox || tdata.m_Type == ItemType.AllBox) {//박스는 바로 까서 주기
						List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
						TItemTable itemTable = TDATA.GetItemTable(m_TData_Stage.m_ClearReward[i].m_Idx);
						for (int j = m_TData_Stage.m_ClearReward[i].m_Count - 1; j > -1; j--) rewards.AddRange(TDATA.GetGachaItem(itemTable, false));
						for (int j = 0; j < rewards.Count; j++) {
							// 캐릭터 보상은 없음
							if (rewards[j].Type == Res_RewardType.Char) continue;
							getrewards.Add(rewards[j]);
						}
					}
					else {
						RES_REWARD_MONEY rmoney;
						RES_REWARD_ITEM ritem;
						switch (tdata.m_Type) {
							case ItemType.Exp:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Exp;
								rmoney.Befor = USERINFO.m_Exp[0] - m_TData_Stage.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Exp[0];
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								getrewards.Add(rmoney);
								break;
							case ItemType.Dollar:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Money;
								rmoney.Befor = USERINFO.m_Money - m_TData_Stage.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Money;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								getrewards.Add(rmoney);
								break;
							case ItemType.Cash:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Cash;
								rmoney.Befor = USERINFO.m_Cash - m_TData_Stage.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Cash;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								getrewards.Add(rmoney);
								break;
							case ItemType.Energy:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Energy;
								rmoney.Befor = USERINFO.m_Energy.Cnt - m_TData_Stage.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Energy.Cnt;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								rmoney.STime = (long)USERINFO.m_Energy.STime;
								getrewards.Add(rmoney);
								break;
							case ItemType.InvenPlus:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Inven;
								rmoney.Befor = USERINFO.m_InvenSize - m_TData_Stage.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_InvenSize;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								getrewards.Add(rmoney);
								break;
							default:
								ritem = new RES_REWARD_ITEM();
								ritem.Type = Res_RewardType.Item;
								ritem.Idx = m_TData_Stage.m_ClearReward[i].m_Idx;
								ritem.Cnt = m_TData_Stage.m_ClearReward[i].m_Count;
								getrewards.Add(ritem);
								break;
						}
						break;
					}
					break;
				case RewardKind.Zombie:
					TZombieTable ztable = TDATA.GetZombieTable(m_TData_Stage.m_ClearReward[i].m_Idx);
					RES_REWARD_ZOMBIE zombie = new RES_REWARD_ZOMBIE();
					zombie.Idx = ztable.m_Idx;
					zombie.Grade = ztable.m_Grade;
					getrewards.Add(zombie);
					break;
				case RewardKind.DNA:
					TDnaTable dnaTable = TDATA.GetDnaTable(m_TData_Stage.m_ClearReward[i].m_Idx);
					RES_REWARD_DNA dna = new RES_REWARD_DNA();
					dna.Idx = dnaTable.m_Idx;
					dna.Grade = dnaTable.m_Grade;
					getrewards.Add(dna);
					break;
			}
		}

		for (int i = 0;i< m_SUI.Reward.Rewards.Length; i++) {
			m_SUI.Reward.Rewards[i].gameObject.SetActive(i < getrewards.Count);
			if (i < getrewards.Count) {
				m_SUI.Reward.Rewards[i].SetData(getrewards[i]);
			}
		}
	}

	[SerializeField] float[] m_MousePos = new float[2];	//0:현재 1:이전
	private void Update() {
		if (m_State == State.GoStage) return;
		m_SUI.BackGroup.localPosition = new Vector3(0f, m_TrPageGroup.anchoredPosition.y / m_PageMoveAmount * Screen.height * 0.3f, 0f);

		if (IsClonDeck) return;
		if (POPUP.GetPopup() != this) return;
		//아무 상태 아닐때 터치하고 있는게 캐릭터 카드일떄
		//잡고있는 캐릭터 카드가 0.25초 이상 지속된 경우 드래그 앤 드랍 상태로 변경
		//드래그앤드랍동안 슬롯 이펙트와, 원본카드 버킷과 복사카드 자체 포지션 이동
		//드랍할때 포지션 체크해서 빈 슬롯이면 슬롯에 채워주기
		if (m_TouchState == ETouchState.END) {
			// 스크롤의 릴리즈만 체크
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
			if(Input.touchCount == 1)
#else
			if (Input.GetMouseButton(0))
#endif
			{
				List<RaycastResult> results = new List<RaycastResult>();
				PointerEventData ped = new PointerEventData(null);
				ped.position = Input.mousePosition;
				m_GR.Raycast(ped, results);

				for (int i = 0; i < results.Count; i++) {
					if (results[i].gameObject.GetComponent<RayCastIgnore>() != null) break;
					if (results[i].gameObject.name.Equals("Item_CharManageCard_Bucket") && m_SUI.SurvScroll.velocity.magnitude < 30) {
						Item_CharManageCard card = results[i].gameObject.transform.parent.GetComponent<Item_CharManageCard>();
						if (card == null) continue;

						if (m_SelectChar == card) m_HoldTime += Time.deltaTime;
						else m_HoldTime = 0f;
						m_SelectChar = card;

						if (m_HoldTime > 0.5f && m_SelectChar.m_Info != null) {
							m_State = State.Hold;
							if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= BaseValue.CONTENT_OPEN_IDX(ContentType.Character)) {//TUTO.IsEndTuto(TutoKind.EquipCharLVUP)
								card.OpenDetail();
								AutoScrolling(m_SelectChar.transform);
							}
							m_SelectChar = null;
						}
						break;
					}
				}
			}
		}
		else if (m_TouchState == ETouchState.PRESS) {
			List<RaycastResult> results = new List<RaycastResult>();
			PointerEventData ped = new PointerEventData(null);
			ped.position = Input.mousePosition;
			m_GR.Raycast(ped, results);
			RayCastIgnore ignore = null;
			for (int i = 0; i < results.Count; i++) {
				ignore = results[i].gameObject.GetComponent<RayCastIgnore>();
				if (ignore != null) {
					break;
				}
			}

			// 이동 갭이 생겼을때 이동으로 판정, 이동중 스크롤을 잡았다 놔도 이동처리
			if (ignore == null && (Mathf.Abs(m_TouchPoint.y - Input.mousePosition.y) > 50f || (Mathf.Abs(m_TouchPoint.y - Input.mousePosition.y) <= 50f && m_TrPageGroup.anchoredPosition.y < m_PageMoveAmount - 0.2f && m_TrPageGroup.anchoredPosition.y > 0f))) {
				m_TouchState = ETouchState.MOVE;
				m_HoldTime = 0;
				m_Dir = m_TouchPoint.y < Input.mousePosition.y;
				m_TouchPoint = Input.mousePosition;
				m_SelectDeckChar = null;
				HoldReset();
				return;
			}
			else
			{
				if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_CharManageCard_Select, State.Hold, m_SelectChar)) return;
				Item_CharDeckCard check_card = null;
				
				for (int i = 0; i < results.Count; i++)
				{
					check_card = results[i].gameObject.GetComponent<Item_CharDeckCard>();
					if (check_card != null) break;
				}

				if (m_SelectDeckChar != null)
				{
					if (m_SelectDeckChar == check_card)
					{
						if (m_HoldTime > 0.5f)
						{
							m_State = State.Hold;
							if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= BaseValue.CONTENT_OPEN_IDX(ContentType.Character)) {
								if (m_SelectDeckChar.m_Info != null) m_SelectDeckChar.OpenDetail();
							}
							//SwipEnd();
							return;
						}

						m_HoldTime += Time.deltaTime;
					}
					else if (ignore == null) {
						// 선택이 달라졌다면 이동으로 판정
						m_TouchState = ETouchState.MOVE;
						m_HoldTime = 0;
						m_Dir = m_TouchPoint.y < Input.mousePosition.y;
						m_TouchPoint = Input.mousePosition;
						m_SelectDeckChar = null;
						HoldReset();
						return;
					}
				}
				else
				{
					m_SelectDeckChar = check_card;
					HoldReset();
				}
			}
		}
		else if (m_TouchState == ETouchState.MOVE) {
			if (TUTO.TouchCheckLock(TutoTouchCheckType.DeckSetting_ListPage, 2)) return;
			else if (TUTO.TouchCheckLock(TutoTouchCheckType.DeckSetting, 0)) return;
			else if (TUTO.TouchCheckLock(TutoTouchCheckType.SynergyDeck, 0)) return;

			// 이동
			if (!m_Dir && m_TrPageGroup.anchoredPosition.y <= 0f) { }
			else if (m_Dir && m_TrPageGroup.anchoredPosition.y >= m_PageMoveAmount) { }
			else {
				m_TrPageGroup.anchoredPosition = new Vector3(0, Input.mousePosition.y, 0) + m_SwipGap;
			}
			if(Mathf.Abs(m_TouchPoint.y - Input.mousePosition.y) > 2f)
			{
				m_Dir = m_TouchPoint.y < Input.mousePosition.y;
				m_TouchPoint = Input.mousePosition;
			}
			//if (m_TrPageGroup.anchoredPosition.y >= m_PageMoveAmount - 0.2f) m_CanClick = true;
		}
	}
	public void SetListPage(bool _on) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.DeckSetting_ListPage, _on ? 0 : 1)) return;
		if (_on && Utile_Class.IsPlayiTween(gameObject, "ListOn")) return;
		if (!_on && Utile_Class.IsPlayiTween(gameObject, "ListOff")) return;

		if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Focus_CharListOnBtn) && _on) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Focus_CharListOffBtn) && !_on) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.DeckSynergy, (int)TutoType_DeckSynergy.Focus_CharListOnBtn) && _on) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.DeckSynergy, (int)TutoType_DeckSynergy.Focus_CharListOffBtn) && !_on) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.Focus_CharListOnBtn) && _on)  TUTO.Next();
		MoveListPage(_on);
	}

	void MoveListPage(bool _on) {
		if (m_State == State.GoStage) return;
		if (_on) {
			iTween.StopByName(gameObject, "ListOff");
			m_CanClick = false;
			m_SUI.ListOnBtnAnim.SetTrigger("Off");
			m_SUI.ListOffBtnAnim.SetTrigger("On");
			//m_SUI.TitleGroup.SetActive(true);
			for(int i = 0; i < m_SUI.SortGroupAnim.Length; i++) {
				m_SUI.SortGroupAnim[i].SetTrigger("On");
			}
			iTween.ValueTo(gameObject, iTween.Hash("from", m_TrPageGroup.anchoredPosition.y, "to", m_PageMoveAmount, "onupdate", "TW_ListPos", "oncomplete", "TW_ListOnEnd", "time", 1f, "easetype", "easeoutquart", "name", "ListOn"));
		}
		else if (!_on)
		{
			iTween.StopByName(gameObject, "ListOn");
			m_CanClick = false;
			m_SUI.ListOnBtnAnim.SetTrigger("On");
			m_SUI.ListOffBtnAnim.SetTrigger("Off");
			//m_SUI.TitleGroup.SetActive(false);
			for (int i = 0; i < m_SUI.SortGroupAnim.Length; i++) {
				m_SUI.SortGroupAnim[i].SetTrigger("Off");
			}
			iTween.ValueTo(gameObject, iTween.Hash("from", m_TrPageGroup.anchoredPosition.y, "to", 0f, "onupdate", "TW_ListPos", "oncomplete", "TW_ListOnEnd", "time", 1f, "easetype", "easeoutquart", "name", "ListOff"));
		}
	}
	void TW_ListPos(float _amount) {
		m_TrPageGroup.anchoredPosition = new Vector3(0f, _amount, 0f);
	}
	void TW_ListOnEnd() {
		if (TUTO.IsTuto(TutoKind.DeckSynergy, (int)TutoType_DeckSynergy.Delay_ListOn)) TUTO.Next();
		m_CanClick = true; 
		SwipEnd();
	}
	void SwipOnOff() {
		m_MousePos[0] = Input.mousePosition.y;
		float fronttop = m_SUI.FrontReck.rect.yMin * MAIN.m_UIMng.GetRatioH() + m_SUI.FrontReck.position.y;
		if (fronttop < m_MousePos[0] && fronttop < m_MousePos[1]) {
			if (m_MousePos[1] - m_MousePos[0] > Screen.height * 0.05f && m_SUI.TitleGroup.activeSelf) {
				SetListPage(false);
				m_MousePos[1] = m_MousePos[0] = 0f;
			}
			else if (m_MousePos[1] - m_MousePos[0] < -Screen.height * 0.05f && !m_SUI.TitleGroup.activeSelf) {
				SetListPage(true);
				m_MousePos[1] = m_MousePos[0] = 0f;
			}
		}
		if (Mathf.Abs(m_MousePos[0] - m_MousePos[1]) >= Screen.height * 0.05f) m_MousePos[1] = m_MousePos[0];
	}
	public GameObject GetDeckPanel()
	{
		return m_SUI.DeckCards[0].transform.parent.gameObject;
	}
	public GameObject GetCharCard(int idx)
	{
		return m_AllChar.Find(o=>o.m_Info.m_Idx == idx).gameObject;
	}

	public GameObject GetSynergyBtn()
	{
		return m_SUI.Btns[0];
	}

	public GameObject GetStartBtn()
	{
		if (m_TData_Stage.m_Energy < 1) return m_SUI.Btns[4];
		return m_SUI.Btns[1];
	}
	public void HoldReset() {
		m_HoldTime = 0f;
	}
	/// <summary> 고정 캐릭터 있을때 자동 배치, 필수캐릭 우선, 나머지 전투력 순</summary>
	void SetAutoNeed(int _pos) {
		if(m_TData_Stage.m_NeedChars.Count > 0 && m_Content == StageContentType.Stage) {
			int maxcnt = m_TData_Stage.m_DeckCharLimit;
			int pos = 0;
			
			DeckInfo deck = USERINFO.m_Deck[_pos];
			List<CharInfo> allchars = new List<CharInfo>();
			List<long> predeck = new List<long>();
			predeck.AddRange(deck.m_Char);
			for (int i = 0; i < m_SUI.DeckCards.Length; i++) m_SUI.DeckCards[i].m_State = Item_CharDeckCard.State.Empty;
			for (int i = 0; i < deck.m_Char.Length; i++) deck.SetChar(i, 0);

			allchars.AddRange(USERINFO.m_Chars);
			//for (int i = 0; i < allchars.Count; i++) allchars[i].GetCombatPower();
			allchars.Sort((CharInfo _before, CharInfo _after) => {
				if (_before.m_CP != _after.m_CP) return _after.m_CP.CompareTo(_before.m_CP);
				return _before.m_Idx.CompareTo(_after.m_Idx);
			});

			for (int i = 0; i < m_TData_Stage.m_NeedChars.Count; i++) {
				int cidx = m_TData_Stage.m_NeedChars[i];
				CharInfo needchar = allchars.Find(o => o.m_Idx == cidx);
				if (needchar == null) continue;
				deck.SetChar(pos, needchar.m_UID);
				if (predeck.Contains(needchar.m_UID)) predeck.Remove(needchar.m_UID);
				allchars.Remove(needchar);
				pos++;
				maxcnt--;
			}
			for (int i = 0; i < Math.Min(predeck.Count, maxcnt); i++) {
				if (predeck[i] == 0) continue;
				CharInfo prechar = allchars.Find(o => o.m_UID == predeck[i]);
				deck.SetChar(pos, prechar.m_UID);
				allchars.Remove(prechar);
				pos++;
				maxcnt--;
			}
			for (int i = 0; i < maxcnt; i++, pos++) {
				deck.SetChar(pos, allchars[i].m_UID);
			}
		}
	}
	/// <summary> 페이지 단위 덱과 대기 캐릭터 세팅</summary>
	public void SetPageDeck(int _pos) {
		if (USERINFO.m_SelectDeck != _pos && TUTO.TouchCheckLock(TutoTouchCheckType.DeckSetting_SelectDeck, _pos)) return;
		if (m_State == State.GoStage) return;

		m_SUI.PageAnim.SetTrigger("TabChange");
		m_SUI.PageAnim.SetTrigger(string.Format("Tab_{0}", _pos + 1));

		USERINFO.SetDeckIdx(_pos);
		if (m_Content == StageContentType.Stage) USERINFO.SetStageDeckIdx(_pos);
		//애니메이션 트리커때문에 SetData
		for (int i = 0; i < m_SUI.DeckCards.Length; i++) m_SUI.DeckCards[i].m_State = Item_CharDeckCard.State.Empty;

		for (int i = 0;i< m_AllChar.Count; i++) {
			m_AllChar[i].gameObject.SetActive(true);
			for (int j = 0; j < m_SUI.DeckCards.Length; j++) {
				if (m_AllChar[i].m_Info.m_UID == USERINFO.m_PlayDeck.m_Char[j]) {
					if (m_TData_Stage.m_NeedChars.Contains(m_AllChar[i].m_Idx))
						m_SUI.DeckCards[j].SetData(m_AllChar[i].m_Info, m_AllChar[i].m_Idx, m_Content == StageContentType.Stage ? 0 : 1);
					else
						m_SUI.DeckCards[j].SetData(m_AllChar[i].m_Info);
					m_AllChar[i].gameObject.SetActive(false);

					TStageTable stagetable = TDATA.GetStageTable(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx, USERINFO.GetDifficulty());
					for (int k = 0;k< stagetable.m_LockJob.Count; k++) {
						if (m_AllChar[i].m_Info.m_TData.m_Job.Contains(stagetable.m_LockJob[k])) {
							m_AllChar[i].gameObject.SetActive(true);
							m_SUI.DeckCards[j].m_State = Item_CharDeckCard.State.Empty;
							USERINFO.m_PlayDeck.SetChar(j, 0);
							break;
						}
					}
					break;
				}
			}
		}
		for (int i = 0; i < m_SUI.DeckCards.Length; i++) {
			if (m_SUI.DeckCards[i].m_State != Item_CharDeckCard.State.Set) {
				m_SUI.DeckCards[i].SetData();
				m_SUI.DeckPowerMark[i].SetData(m_SUI.SortingGroup.m_Condition, null);
			}
			else {
				m_SUI.DeckPowerMark[i].SetData(m_SUI.SortingGroup.m_Condition, m_SUI.DeckCards[i].m_Info);
			}
		}

		//필수 캐릭 세팅
		SetNeedChars();

		if (TDATA.GetModeTable(m_TData_Stage.m_Idx) == null) {
			for (int i = 0; i < m_AllChar.Count; i++) {
				m_AllChar[i].SetUseLock(false);

				TStageTable stagetable = TDATA.GetStageTable(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx, USERINFO.GetDifficulty());
				for (int j = 0; j < stagetable.m_LockJob.Count; j++) {
					if (m_AllChar[i].m_Info.m_TData.m_Job.Contains(stagetable.m_LockJob[j])) {
						m_AllChar[i].SetUseLock(true, stagetable.m_LockJob[j]);
					}
				}
			}
		}
		//정렬 세팅
		SetSort();

		//리스트 수
		SetCharCount();
		//전투력 표기
		SetCP();
		//생존스텟 표기
		SetSurvStat();
		//시너지 갱신
		SetSynergy(true);
	}
	/// <summary> 대기 캐릭터 표기 덱에없는 획득 캐릭 / 전체 캐릭 </summary>
	void SetCharCount() {
		int nowcnt = m_AllChar.FindAll(t => t.gameObject.activeSelf == true).Count;
		m_SUI.Empty.SetActive(nowcnt < 1);
	}
	void SetSort() {
		//for (int i = 0; i < m_AllChar.Count; i++) m_AllChar[i].m_Info.GetCombatPower();
		switch (m_SUI.SortingGroup.m_Condition) {
			case SortingType.Grade:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					if (after.m_UseLock != befor.m_UseLock) return after.m_UseLock.CompareTo(befor.m_UseLock);
					if (m_Exceptdata != null && m_Exceptdata.IS_Contain(after.m_Idx) != m_Exceptdata.IS_Contain(befor.m_Idx)) return m_Exceptdata.IS_Contain(after.m_Idx).CompareTo(m_Exceptdata.IS_Contain(befor.m_Idx));
					if (after.m_Need != befor.m_Need) return after.m_Need.CompareTo(befor.m_Need);
					if (after.m_Recommand != befor.m_Recommand) return after.m_Recommand.CompareTo(befor.m_Recommand);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					int aftercp = after.m_Info.m_CP;
					int beforcp = befor.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.CombatPower:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					if (after.m_UseLock != befor.m_UseLock) return after.m_UseLock.CompareTo(befor.m_UseLock);
					if (m_Exceptdata != null && m_Exceptdata.IS_Contain(after.m_Idx) != m_Exceptdata.IS_Contain(befor.m_Idx)) return m_Exceptdata.IS_Contain(after.m_Idx).CompareTo(m_Exceptdata.IS_Contain(befor.m_Idx));
					if (after.m_Need != befor.m_Need) return after.m_Need.CompareTo(befor.m_Need);
					if (after.m_Recommand != befor.m_Recommand) return after.m_Recommand.CompareTo(befor.m_Recommand);
					int aftercp = after.m_Info.m_CP;
					int beforcp = befor.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.Level:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					if (after.m_UseLock != befor.m_UseLock) return after.m_UseLock.CompareTo(befor.m_UseLock);
					if (m_Exceptdata != null && m_Exceptdata.IS_Contain(after.m_Idx) != m_Exceptdata.IS_Contain(befor.m_Idx)) return m_Exceptdata.IS_Contain(after.m_Idx).CompareTo(m_Exceptdata.IS_Contain(befor.m_Idx));
					if (after.m_Need != befor.m_Need) return after.m_Need.CompareTo(befor.m_Need);
					if (after.m_Recommand != befor.m_Recommand) return after.m_Recommand.CompareTo(befor.m_Recommand);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					int aftercp = after.m_Info.m_CP;
					int beforcp = befor.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.Men:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					if (after.m_UseLock != befor.m_UseLock) return after.m_UseLock.CompareTo(befor.m_UseLock);
					if (m_Exceptdata != null && m_Exceptdata.IS_Contain(after.m_Idx) != m_Exceptdata.IS_Contain(befor.m_Idx)) return m_Exceptdata.IS_Contain(after.m_Idx).CompareTo(m_Exceptdata.IS_Contain(befor.m_Idx));
					if (after.m_Need != befor.m_Need) return after.m_Need.CompareTo(befor.m_Need);
					if (after.m_Recommand != befor.m_Recommand) return after.m_Recommand.CompareTo(befor.m_Recommand);
					float aftermen = after.m_Info.m_Stat[StatType.Men];
					float beformen = befor.m_Info.m_Stat[StatType.Men];
					if (beformen != aftermen) return aftermen.CompareTo(beformen);
					int aftercp = after.m_Info.m_CP;
					int beforcp = befor.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.Hyg:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					if (after.m_UseLock != befor.m_UseLock) return after.m_UseLock.CompareTo(befor.m_UseLock);
					if (m_Exceptdata != null && m_Exceptdata.IS_Contain(after.m_Idx) != m_Exceptdata.IS_Contain(befor.m_Idx)) return m_Exceptdata.IS_Contain(after.m_Idx).CompareTo(m_Exceptdata.IS_Contain(befor.m_Idx));
					if (after.m_Need != befor.m_Need) return after.m_Need.CompareTo(befor.m_Need);
					if (after.m_Recommand != befor.m_Recommand) return after.m_Recommand.CompareTo(befor.m_Recommand);
					float afterhyg = after.m_Info.m_Stat[StatType.Hyg];
					float beforhyg = befor.m_Info.m_Stat[StatType.Hyg];
					if (beforhyg != afterhyg) return afterhyg.CompareTo(beforhyg);
					int aftercp = after.m_Info.m_CP;
					int beforcp = befor.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.Sat:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					if (after.m_UseLock != befor.m_UseLock) return after.m_UseLock.CompareTo(befor.m_UseLock);
					if (m_Exceptdata != null && m_Exceptdata.IS_Contain(after.m_Idx) != m_Exceptdata.IS_Contain(befor.m_Idx)) m_Exceptdata.IS_Contain(after.m_Idx).CompareTo(m_Exceptdata.IS_Contain(befor.m_Idx));
					if (after.m_Need != befor.m_Need) return after.m_Need.CompareTo(befor.m_Need);
					if (after.m_Recommand != befor.m_Recommand) return after.m_Recommand.CompareTo(befor.m_Recommand);
					float aftersat = after.m_Info.m_Stat[StatType.Sat];
					float beforsat = befor.m_Info.m_Stat[StatType.Sat];
					if (beforsat != aftersat) return aftersat.CompareTo(beforsat);
					int aftercp = after.m_Info.m_CP;
					int beforcp = befor.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
		}

		if (m_SUI.SortingGroup.m_Ascending) m_AllChar.Reverse();
		if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Decksetting_Delay)) {
			Item_CharManageCard tutocard = null;
			var temp = new List<Item_CharManageCard>();
			tutocard = GetCharCard(1052).GetComponent<Item_CharManageCard>();
			temp.Add(tutocard);
			m_AllChar.Remove(tutocard);
			temp.AddRange(m_AllChar);
			m_AllChar = temp;
		}
		else if (TUTO.IsTuto(TutoKind.DeckSynergy, (int)TutoType_DeckSynergy.Decksetting_Delay)) {
			Item_CharManageCard tutocard = null;
			var temp = new List<Item_CharManageCard>();
			tutocard = GetCharCard(1013).GetComponent<Item_CharManageCard>();
			temp.Add(tutocard);
			m_AllChar.Remove(tutocard);
			temp.AddRange(m_AllChar);
			m_AllChar = temp;
		}
		else if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.DeckSorting)) {
			Item_CharManageCard tutocard = null;
			// 1024,1036 캐릭터만 앞으로 이동
			var temp = new List<Item_CharManageCard>();
			for (int i = 0; i < 2; i++) {
				tutocard = GetCharCard(i == 0 ? 1024 : 1036).GetComponent<Item_CharManageCard>();
				temp.Add(tutocard);
				m_AllChar.Remove(tutocard);
			}
			temp.AddRange(m_AllChar);
			m_AllChar = temp;
		}

		for (int i = 0;i< m_AllChar.Count; i++) {
			m_AllChar[i].transform.SetAsLastSibling();

			if (m_AllChar[i] != null) {
				m_AllChar[i].SetSortVal(m_SUI.SortingGroup.m_Condition);
			}
		}
		for (int i = 0; i < m_SUI.DeckCards.Length; i++) {
			if (m_SUI.DeckCards[i].m_Info != null) {
				m_SUI.DeckPowerMark[i].SetData(m_SUI.SortingGroup.m_Condition, m_SUI.DeckCards[i].m_Info);
			}
		}
	}
	/// <summary> 전투력 갱신 </summary>
	void SetCP() {
		int CombatPower = 0;
		for (int i = 0; i < USERINFO.m_PlayDeck.m_Char.Length; i++) {
			if (USERINFO.m_PlayDeck.m_Char[i] != 0) {
				CombatPower += USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]).m_CP;
			}
		}
		iTween.StopByName(gameObject, "DeckCP");
		iTween.ValueTo(gameObject, iTween.Hash("from", m_PreDeckCP, "to", CombatPower, "onupdate", "TW_DeckCP", "time", 1f, "name", "DeckCP"));
		m_PreDeckCP = CombatPower;
		string trig = "Normal";
		bool warningfx = false;
		if (m_RecommandCP == 0 || m_Content == StageContentType.Factory) trig = "Normal";
		else {
			float ratio = ((float)CombatPower - (float)m_RecommandCP) / (float)m_RecommandCP;
			if (m_Content == StageContentType.Stage) {
				if (ratio >= BaseValue.SAFE_BP_GAP) trig = "Green";
				else if (m_RecommandCP > CombatPower) {
					trig = "Red";
					warningfx = true;
				}
			}
			else {
				if (ratio >= BaseValue.SAFE_BP_GAP) trig = "Green";
				else if (ratio <= BaseValue.RISK_BP_GAP && ratio > BaseValue.DANGER_BP_GAP) trig = "Yellow";
				else if (ratio <= BaseValue.DANGER_BP_GAP) {
					trig = "Red";
					warningfx = true;
				}
			}
			if ( ratio >= 0.2f) {
				m_SUI.CPDanger.text = TDATA.GetString(895);
			}
			else if (ratio >= 0.1f && ratio < 0.2f) {
				m_SUI.CPDanger.text = TDATA.GetString(894);
			}
			else if (ratio >= 0f && ratio < 0.1f) {
				m_SUI.CPDanger.text = TDATA.GetString(893);
			}
			else if (ratio >= -0.1f && ratio < 0f) {
				m_SUI.CPDanger.text = TDATA.GetString(m_Content == StageContentType.Stage ? 553 : 552);
			}
			else if (ratio < -0.1f) {
				m_SUI.CPDanger.text = TDATA.GetString(553);
			}
			m_SUI.DeckCPAnim.SetTrigger(trig);

		}
		m_SUI.DeckCPAnim.SetTrigger(trig);
		m_SUI.DeckCPAnim.SetTrigger(trig.Equals("Normal") ? "NoChange" : "ChangeLoop");
		m_SUI.WarningBtnFx[0].SetActive(warningfx);
		m_SUI.WarningBtnFx[1].SetActive(warningfx);
	}
	void TW_RecommandCP(float _amount) {
		m_SUI.RecommandCP.text = string.Format(Mathf.RoundToInt(_amount) > 0 ? "{0:#,###}" : "{0}", Mathf.RoundToInt(_amount));
	}
	void TW_DeckCP(float _amount) {
		m_SUI.DeckCP.text = string.Format(Mathf.RoundToInt(_amount) > 0 ? "{0:#,###}" : "{0}", Mathf.RoundToInt(_amount));
	}
	void SetSurvStat() {
		for(StatType i = StatType.Men;i< StatType.SurvEnd; i++) {
			bool isopen = false;
			if (i == StatType.Men && TUTO.IsEndTuto(TutoKind.Stage_801)) isopen = true;
			else if(i == StatType.Hyg && TUTO.IsEndTuto(TutoKind.Stage_601)) isopen = true;
			else if (i == StatType.Sat && TUTO.IsEndTuto(TutoKind.Stage_401)) isopen = true;

			for (int j = 0;j< m_SUI.m_Stats[(int)i].UnlockObjs.Length; j++) {
				m_SUI.m_Stats[(int)i].UnlockObjs[j].SetActive(isopen);
			}
			m_SUI.m_Stats[(int)i].LockObj.SetActive(!isopen);

			if (!isopen) continue;
			float stat = 0;
			for (int j = 0; j < USERINFO.m_PlayDeck.m_Char.Length; j++) {
				if (USERINFO.m_PlayDeck.m_Char[j] != 0) {
					stat += USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[j]).GetStat(i);
				}
			}
			stat = Mathf.RoundToInt(stat);
			string twname = string.Format("Deck{0}", i.ToString());
			string onupname = string.Format("TW_Deck{0}", i.ToString());
			iTween.StopByName(gameObject, twname);
			iTween.ValueTo(gameObject, iTween.Hash("from", m_PreSurvStat[(int)i], "to", stat, "onupdate", onupname, "time", 1f, "name", twname));
			m_PreSurvStat[(int)i] = (int)stat;
		}
	}
	void TW_DeckMen(float _amount) {
		m_SUI.DeckSurvStat[(int)StatType.Men].text = string.Format(Mathf.RoundToInt(_amount) > 0 ? "{0:#,###}" : "{0}", Mathf.RoundToInt(_amount));
	}
	void TW_DeckHyg(float _amount) {
		m_SUI.DeckSurvStat[(int)StatType.Hyg].text = string.Format(Mathf.RoundToInt(_amount) > 0 ? "{0:#,###}" : "{0}", Mathf.RoundToInt(_amount));
	}
	void TW_DeckSat(float _amount) {
		m_SUI.DeckSurvStat[(int)StatType.Sat].text = string.Format(Mathf.RoundToInt(_amount) > 0 ? "{0:#,###}" : "{0}", Mathf.RoundToInt(_amount));
	}
	/// <summary> 시너지 갱신 </summary>
	void SetSynergy(bool _first) {
		m_SUI.NotSynergy.SetActive(USERINFO.m_PlayDeck.m_Synergy.Count == 0);
		for (int i = 0, pos = -1;i< m_SUI.SynergyName.Length; i++) {
			if (i < USERINFO.m_PlayDeck.m_Synergy.Count) {
				m_SUI.SynergyName[i].SetData(USERINFO.m_PlayDeck.m_Synergy[++pos]);
				m_SUI.SynergyName[i].gameObject.SetActive(true);
				if (!_first) m_SUI.SynergyName[i].SetAnim("In");
			}
			else m_SUI.SynergyName[i].gameObject.SetActive(false);
		}
	}

	/// <summary> 필요 캐릭터 세팅 </summary>
	void SetNeedChars(bool _set = true) {
		m_NeedChars.Clear();
		m_NeedChars.AddRange(m_TData_Stage.m_NeedChars);

		for (int i = 0; i < m_DeckSlotCnt; i++) {
			CharInfo charinfo = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]);
			if (charinfo == null) continue;
			int idx = charinfo.m_Idx;
			if (m_NeedChars.Contains(idx)) m_NeedChars.Remove(idx);
		}
		if (!_set) return;
		for (int i = 0, needpos = 0; i < m_SUI.DeckCards.Length; i++) {
			if (m_SUI.DeckCards[i].m_State != Item_CharDeckCard.State.Set) {
				int needidx = 0;
				if (m_NeedChars.Count - 1 < needpos) needidx = 0;
				else {
					needidx = m_NeedChars[needpos];
					needpos++;
				}
				m_SUI.DeckCards[i].SetData(null, needidx, m_Content == StageContentType.Stage ? 0 : 1);
				m_SUI.DeckPowerMark[i].SetData(m_SUI.SortingGroup.m_Condition, null);
			}
		}
	}

	/// <summary> 던전 입장 </summary>
	public void ClickGoDungeon() {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, 1, m_Popup)) return;
		if (m_State == State.GoStage) return;
		//if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Focus_GoStageBtn)) TUTO.Next();
		if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Focus_GoStageBtn)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Focus_GoStageBtn)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.Focus_GoStageBtn)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_204, (int)TutoType_Stage_204.Focus_GoStageBtn)) TUTO.Next();

		if (m_TData_Stage.m_Energy > 0 && USERINFO.m_Energy.Cnt < m_TData_Stage.m_Energy) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.StartLackPop(BaseValue.ENERGY_IDX, false, (res)=> {
				if(res == EResultCode.SUCCESS) SetStartBtn();
			});
			return;
		}

		for (int i = 0; i < m_DeckSlotCnt; i++) {
			if (USERINFO.m_PlayDeck.m_Char[i] <= 0) {
				PlayCommVoiceSnd(VoiceType.Fail);
				POPUP.Set_Popup(PopupPos.MSGBOX, PopupName.Msg_CenterAlarm).GetComponent<MsgBoxBase>().SetMsg(string.Empty, TDATA.GetString(44));
				return;
			}
		}

		if (m_NeedChars.Count > 0) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_Popup(PopupPos.MSGBOX, PopupName.Msg_CenterAlarm).GetComponent<MsgBoxBase>().SetMsg(string.Empty, TDATA.GetString(868));
			return;
		}
#if !NOT_USE_STAGE_GROWTHWAY
		if (!TUTO.IsTutoPlay()) {//튜토때는 그냥 전투력 모자라도 진입하게
			if ((m_Content == StageContentType.Stage || m_Content == StageContentType.Replay || m_Content == StageContentType.ReplayHard || m_Content == StageContentType.ReplayNight) && USERINFO.m_PlayDeck.GetCombatPower(false, true) < m_TData_Stage.m_RecommandCP) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.GrowthWay_Warning);
				return;
			}
		}
#endif

		if (!TUTO.IsTutoPlay() && (m_SUI.WarningBtnFx[0].activeSelf || m_SUI.WarningBtnFx[1].activeSelf)) {
			POPUP.Set_MsgBox(PopupName.Msg_StageWarning, TDATA.GetString(252), TDATA.GetString(875), (result, obj) => { 
				if((EMsgBtn)result == EMsgBtn.BTN_YES) {
#if NOT_USE_NET
					USERINFO.GetShell(-m_TData_Stage.m_Energy);
#endif
					PlayEffSound(SND_IDX.SFX_0301);
					m_State = State.GoStage;
					Close(1);
				}
			});
		}
		else {
#if NOT_USE_NET
			USERINFO.GetShell(-m_TData_Stage.m_Energy);
#endif
			if(!IS_Event) PlayEffSound(SND_IDX.SFX_0301);
			m_State = State.GoStage;
			Close(1);
		}
	}
	
	/// <summary> 스테이지 정보 </summary>
	public void Click_StageInfo() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.DeckSetting, 1)) return;
		if (m_State == State.GoStage) return;
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Info, null, m_TData_Stage);
	}
	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return; 
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _res) {
		if (_res > 0) m_SUI.Anim.SetTrigger("GameStart");
		else m_SUI.Anim.SetTrigger("End");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));


		if (IsClonDeck)
		{
			if (_res == 0) {
				DLGTINFO?.f_OBJSndOn.Invoke();
				base.Close(_res);
			}
			else m_EndCB?.Invoke(_res, gameObject);
		}
		else {
#if NOT_USE_NET
			MAIN.Save_UserInfo();
			if (_res == 0) {
				DLGTINFO?.f_OBJSndOn.Invoke();
				base.Close(_res);
			}
			else m_EndCB?.Invoke(_res, gameObject);
#else
		Action<int> cb = base.Close;
		WEB.SEND_REQ_DECK_SET((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					WEB.SEND_REQ_DECK((res2) => {
						// 다시 보여주기
						SetPageDeck(USERINFO.m_SelectDeck);
					}, USERINFO.m_UID);
				});
				return;
			}
			for (int i = 0; i < USERINFO.m_Deck.Length; i++) USERINFO.m_Deck[i].IsChange = false;
			if(_res == 0){
				DLGTINFO?.f_OBJSndOn?.Invoke();
				cb?.Invoke(_res);
			}
			else m_EndCB?.Invoke(_res, gameObject);
		});
#endif
		}
	}
	/// <summary> 캐릭터 카드를 액션 콜백 </summary>
	void Click_CB(Item_CharManageCard _card)
	{
		//if (!m_CanClick) return;
		if (m_State == State.GoStage) return;
		switch (_card.m_State) {
			case Item_CharManageCard.State.Click:
				int cnt = Mathf.Clamp(m_DeckSlotCnt, 1, 6);
				bool remainneed = true;
				for(int i = 0;i< cnt; i++) {
					if (m_SUI.DeckCards[i].Is_Need && m_SUI.DeckCards[i].m_NeedIdx != _card.m_Idx) continue;
					if(m_SUI.DeckCards[i].m_State == Item_CharDeckCard.State.Empty) {
						SND.StopAllVoice();
						SND_IDX vocidx = _card.m_Info.m_TData.GetVoice(TCharacterTable.VoiceType.InDeck);
						PlayVoiceSnd(new List<SND_IDX>() { vocidx });

						m_SelectChar = _card;
						//선택한 캐릭터 덱에 추가하고 목록에서 꺼주기
						DeckSetChar(i);
						if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Select_FirstChar1052)) TUTO.Next();
						else if (TUTO.IsTuto(TutoKind.DeckSynergy, (int)TutoType_DeckSynergy.Select_FirstChar1013)) TUTO.Next();
						remainneed = false;
						break;
					}
				}
				if (remainneed && m_TData_Stage.m_DeckCharLimit > USERINFO.m_PlayDeck.GetDeckCharCnt()) {
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(868));
				}
				break;
		}
		_card.m_State = Item_CharManageCard.State.None;
	}

	void DeckSetChar(int pos)
	{
		if (m_SelectChar == null) return;
		if (m_TData_Stage.m_NeedChars.Contains(m_SelectChar.m_Info.m_Idx))
			m_SUI.DeckCards[pos].SetData(m_SelectChar.m_Info, m_SelectChar.m_Info.m_Idx, m_Content == StageContentType.Stage ? 0 : 1);
		else
			m_SUI.DeckCards[pos].SetData(m_SelectChar.m_Info);
		m_SUI.DeckPowerMark[pos].SetData(m_SUI.SortingGroup.m_Condition, m_SelectChar.m_Info);
		USERINFO.m_PlayDeck.SetChar(pos, m_SelectChar.m_Info.m_UID);
		m_SelectChar.gameObject.SetActive(false);
		//필수 캐릭 세팅
		SetNeedChars();
		//리스트 수
		SetCharCount();
		//전투력 표기
		SetCP();
		//생존스텟 표기
		SetSurvStat();
		//시너지 갱신
		SetSynergy(false);
		AutoScrolling(m_SelectChar.transform);
		m_SelectChar = null;
	}
	/// <summary> 차있는 슬롯 클릭시 덱에서 제거 </summary>
	public void ClickSlot(int _pos) {
		if(m_TData_Stage.m_NeedChars.Count > 0) {//m_Content == StageContentType.Stage && 
			if (m_TData_Stage.m_NeedChars.Contains(m_SUI.DeckCards[_pos].m_Idx)) {
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(868));
				return;
			}
		}
		SlotOut(_pos);
		SetListPage(true); 
	}
	void SlotOut(int _pos) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.DeckSetting_SelectDeckSlot, _pos)) return;
		if (m_State == State.GoStage) return;
		if (IsClonDeck) return;
		if (m_SUI.DeckCards[_pos].m_State == Item_CharDeckCard.State.Set) {//Click
			m_SUI.DeckPowerMark[_pos].SetData(m_SUI.SortingGroup.m_Condition, null);
			//리스트에서 켜주고 덱에서 제거
			m_SelectChar = null;

			m_SUI.DeckCards[_pos].SetData();
			m_SUI.DeckCards[_pos].OutSlot();

			Item_CharManageCard card = m_AllChar.Find(t => t.m_Info.m_UID == USERINFO.m_PlayDeck.m_Char[_pos]);
			card?.gameObject.SetActive(true);
			USERINFO.m_PlayDeck.SetChar(_pos, 0);

			//필수 캐릭 세팅
			SetNeedChars();
			SetSort();
			//리스트 수
			SetCharCount();
			//전투력 표기
			SetCP();
			//생존스텟 표기
			SetSurvStat();
			//시너지 갱신
			SetSynergy(false);
		}
	}
	/// <summary> 현재 덱 시너지 보기 </summary>
	public void ClickSynergyView()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.DeckSetting, 0)) return;
		if (TUTO.IsTuto(TutoKind.DeckSynergy, (int)TutoType_DeckSynergy.Focus_DeckSynergy)) TUTO.Next();
		if (m_State == State.GoStage) return;

		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Synergy_Applied, (result, obj)=> {
		});
	}
	/// <summary> 카드가 버킷에서 벗어난 경우 즉각 자동으로 이동 </summary>
	void AutoScrolling(Transform _trans) {
		float posy = 0f;

		float buckettop = m_SUI.SurvScroll.transform.position.y + m_SUI.SurvScroll.GetComponent<RectTransform>().rect.height / 2;
		float bucketbottom = m_SUI.SurvScroll.transform.position.y - m_SUI.SurvScroll.GetComponent<RectTransform>().rect.height / 2;

		float cardtop = _trans.position.y + _trans.GetComponent<RectTransform>().rect.height / 2 + 68;
		float cardbottom = _trans.position.y - _trans.GetComponent<RectTransform>().rect.height / 2 - 250;

		if (buckettop < cardtop) {//카드 위가 잘릴 경우
			posy = cardtop - buckettop;
			m_SUI.CharObjBucket.localPosition -= new Vector3(0f, posy, 0f);
		}
		if (bucketbottom > cardbottom) {// 카드 아래가 잘릴 경우
			posy = bucketbottom - cardbottom;
			m_SUI.CharObjBucket.localPosition += new Vector3(0f, posy, 0f);
		}
	}
	///<summary> 캐릭터 카드 갱신 </summary>
	public void CharCardRefresh() {
		for (int i = 0; i < m_AllChar.Count; i++) {
			Item_CharManageCard card = m_AllChar[i];
			if (card != null) {
				card.SetLvGrade();
				card.SetRankUpAlarm();
				card.SetSortVal(m_SUI.SortingGroup.m_Condition);
			}
		}

		SetCP();
		SetSurvStat();
		SetSort();
	}
	
	public void DeckCharCardRefresh() {
		for (int i = 0; i < m_SUI.DeckCards.Length; i++) {
			if (m_SUI.DeckCards[i].m_Info != null) {
				m_SUI.DeckCards[i].Refresh(m_SUI.DeckCards[i].m_Info);
				m_SUI.DeckPowerMark[i].SetData(m_SUI.SortingGroup.m_Condition, m_SUI.DeckCards[i].m_Info);
			}
		}

		DLGTINFO.f_RFCharInfoCard?.Invoke();//CharCardRefresh
		//SetCP();
		//SetSurvStat();
	}

	void SetStartBtn() {
		bool IsActive = USERINFO.m_Energy.Cnt >= m_TData_Stage.m_Energy;
		m_SUI.GoBtn[0].gameObject.SetActive(m_TData_Stage.m_Energy > 0 && IsActive);
		m_SUI.GoBtn[2].gameObject.SetActive(m_TData_Stage.m_Energy < 1 && IsActive);
		m_SUI.GoBtn[1].gameObject.SetActive(!IsActive);
	}

	public void OnProfileDown()
	{
		if (m_State == State.GoStage) return;
		if (IsClonDeck) return;
		if (Utile_Class.IsPlayiTween(gameObject, "ListOff")) iTween.StopByName(gameObject, "ListOff");
		if (Utile_Class.IsPlayiTween(gameObject, "ListOn")) iTween.StopByName(gameObject, "ListOn");
		m_TouchState = ETouchState.PRESS;
		m_TouchPoint = Input.mousePosition;
		m_SwipGap = m_TrPageGroup.position - new Vector3(0, m_TouchPoint.y, 0);
		m_MoveGap = Vector3.zero;
		m_Dir = m_TrPageGroup.anchoredPosition.y <= 0.1f;
		m_PressTime = UTILE.Get_Time();
		HoldReset();
	}

	public void OnProfileUp()
	{
		if (m_State == State.GoStage) return;
		if (IsClonDeck) return;
		SwipEnd();
	}

	void SwipEnd() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.DeckSetting_ListPage, 2)) return;
		if (m_TouchState == ETouchState.MOVE) {
			// 이동 상태일때라면 해당 방향으로 이동
			MoveListPage(m_Dir);
		}
		else if (m_TouchState == ETouchState.PRESS && UTILE.Get_Time() - m_PressTime < 0.5f) {
			// 클릭 판정

			EventTrigger check_event = null;
			Button check_btn = null;
			List<RaycastResult> results = new List<RaycastResult>();
			PointerEventData ped = new PointerEventData(null);
			ped.position = Input.mousePosition;
			m_GR.Raycast(ped, results);

			for (int i = 0; i < results.Count; i++) {
				// 버튼 체크
				check_btn = results[i].gameObject.GetComponent<Button>();
				if (check_btn != null) {
					check_btn.onClick?.Invoke();//시너지 튜토가 유아이는 스와이프 영역이 가렸는데 강제로 버튼 여기서 활성화시킴
					break;
				}
				check_event = results[i].gameObject.GetComponent<EventTrigger>();
				if (check_event != null && check_event.GetComponent<Item_CharDeckCard>() != null) {
					check_event.OnPointerClick(new PointerEventData(EventSystem.current));
					break;
				}
			}
		}
		m_SelectDeckChar = null;
		m_TouchState = ETouchState.END;
		m_HoldTime = 0;
		m_State = State.None;
	}
}
