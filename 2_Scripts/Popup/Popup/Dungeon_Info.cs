using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Linq;
using static LS_Web;

public class Dungeon_Info : PopupBase
{
	[Serializable]
	public struct SCUI
	{
		public Button PassBtn;
		public GameObject GoBtn;
		public TextMeshProUGUI PassBtnName;
		public TextMeshProUGUI GoBtnName;
		public TextMeshProUGUI GoBtnCnt;
		public GameObject GoBtnIcon;
		public Sprite[] GoBtnBGs;
		public Image GoBtnBG;
	}
	[SerializeField] protected SCUI m_SCUI;
	protected IEnumerator m_Action;
	protected StageContentType m_Content;
	protected DayOfWeek m_Day = DayOfWeek.Sunday;
	protected int m_Pos = 0;
	protected int m_ClearLv;
	[SerializeField]
	protected int[] m_Lv = new int[3];//0:현재 선택 레벨, 1:최대 레벨, 2:제한 레벨
	int m_LimitStg;
	protected List<Item_RewardList_Item> m_RewardCards = new List<Item_RewardList_Item>();
	protected List<RES_REWARD_BASE> m_GetRewards = new List<RES_REWARD_BASE>();
	protected List<TModeTable> m_Modetables = new List<TModeTable>();
	protected TModeTable m_Modetable { get { return m_Content == StageContentType.Subway ? TDATA.GetModeTable(USERINFO.GetSubwayStgIdx().m_StgIdx) : TDATA.GetModeTable(m_Content, m_Day, m_Pos)[Math.Min(m_Lv[0], m_Lv[1] - 1)]; } }
	protected TStageTable m_Stagetable { get { return TDATA.GetStageTable(m_Modetable.m_StageIdx); } }
	protected bool m_CanClick;
	protected bool m_ChangeStart;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Content = (StageContentType)aobjValue[0];
		if (m_Content == StageContentType.University || m_Content == StageContentType.Academy) {
			m_Day = (DayOfWeek)aobjValue[1];
			m_Pos = (int)aobjValue[2];
		}
		else if (m_Content == StageContentType.Subway) {
			m_Day = (DayOfWeek)m_Modetable.m_OpenDay;//(DayOfWeek)aobjValue[1];
			m_Pos = m_Modetable.m_Pos;
		}
		else if(m_Content == StageContentType.Tower) {
			m_ChangeStart = (bool)aobjValue[1];
		}
		m_Modetables = TDATA.GetModeTable(m_Content, m_Day, m_Pos);
		base.SetData(pos, popup, cb, aobjValue);
	}
	protected virtual void SetLv(int _lv) {
		m_LimitStg = m_Modetables[_lv].m_StageLimit;
		if (m_LimitStg > USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)m_Modetables[_lv].m_DiffType].Idx) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(273), m_LimitStg / 100, m_LimitStg % 100, TDATA.GetStageTable(m_LimitStg).GetName()));
			return;
		}
		if (m_ClearLv < _lv) {
			PlayCommVoiceSnd(VoiceType.Fail);
			return;
		}
		if(m_Lv[0] == _lv && m_Content != StageContentType.Academy) Click_GoBuy(); 
		else m_Lv[0] = _lv;
	}

	public void GetLv() {
		switch (m_Content) {
			case StageContentType.Academy:
			case StageContentType.University:
				m_ClearLv = USERINFO.m_Stage[m_Content].Idxs.Find(t => t.Week == m_Day && t.Pos == m_Pos).Clear;
				break;
			case StageContentType.Subway:
				m_ClearLv = m_Modetable.m_Difficulty;//USERINFO.m_Stage[m_Content].Idxs.Find(t => t.Week == m_Day && t.Pos == m_Pos).Clear;
				break;
			case StageContentType.Tower:
			case StageContentType.Cemetery:
			case StageContentType.Bank:
			case StageContentType.Factory:
				m_ClearLv = USERINFO.m_Stage[m_Content].Idxs[0].Clear;
				break;
		}
		m_Lv[1] = m_Modetables.Count;
		for (int i = m_ClearLv; i < m_Lv[1]; i++) {
			m_LimitStg = m_Modetables[i].m_StageLimit;
			if (m_LimitStg > USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)m_Modetables[i].m_DiffType].Idx) {
				m_Lv[2] = i;
				break;
			}
		}
		if (m_Content == StageContentType.Subway)
			m_Lv[0] = m_ClearLv;
		else if (m_Content == StageContentType.Tower)
			m_Lv[0] = Math.Min(m_ClearLv, m_Lv[1]);
		else {
			int lv = m_ClearLv;
			int now = 0;
			if (m_Content == StageContentType.Academy || m_Content == StageContentType.University)
				now = USERINFO.m_Stage[m_Content].Idxs.Find(t => t.Week == m_Day && t.Pos == m_Pos).Idx;
			else now = USERINFO.m_Stage[m_Content].Idxs[0].Idx;
			if (STAGEINFO.m_LastLv == now && STAGEINFO.m_LastPlayLv > -1) {
				lv = STAGEINFO.m_LastPlayLv - 1;
			}
			m_Lv[0] = Mathf.Clamp(m_Lv[2] == 0 ? lv : (lv < m_Lv[2] ? lv : lv - 1), 0, m_Lv[1] - 1);
		}
	}
	//바뀌지 않는 기본 유아이들
	protected virtual void SetCommUI() {

	}
	//레벨에 따라 바뀌는 유아이들
	protected virtual void SetNotCommUI(bool _first = false) {

	}
	//소탕권, 입장권 버튼 유아이들
	protected virtual void SetButtUI() {
		if (m_SCUI.GoBtnIcon != null) m_SCUI.GoBtnIcon.SetActive(true);
		m_SCUI.GoBtnBG.sprite = m_SCUI.GoBtnBGs[0];
		bool canpass = m_Lv[0] < m_ClearLv && USERINFO.m_Stage[m_Content].IS_CanGoStage();
		if(m_SCUI.PassBtnName != null) m_SCUI.PassBtnName.text = TDATA.GetString(canpass ? 84 : 468);

		if (USERINFO.m_Stage[m_Content].IS_CanGoStage()) {
			if (m_SCUI.PassBtn != null) m_SCUI.PassBtn.interactable = m_Lv[0] < m_ClearLv;
			if (m_SCUI.GoBtnName != null) m_SCUI.GoBtnName.text = TDATA.GetString(152);//152 157
			if (m_SCUI.GoBtnCnt != null) m_SCUI.GoBtnCnt.text = string.Format("{0}/{1}", USERINFO.m_Stage[m_Content].GetItemCnt(false), USERINFO.m_Stage[m_Content].GetItemMax(false));
		}
		else {
			if (USERINFO.m_Stage[m_Content].IS_CanBuy()) {
				if (m_SCUI.PassBtn != null) m_SCUI.PassBtn.interactable = m_Lv[0] < m_ClearLv;
				if (m_SCUI.GoBtnName != null) m_SCUI.GoBtnName.text = TDATA.GetString(156);//152 157
				if (m_SCUI.GoBtnCnt != null) m_SCUI.GoBtnCnt.text = string.Format("{0}/{1}", USERINFO.m_Stage[m_Content].GetItemCnt(true), USERINFO.m_Stage[m_Content].GetItemMax(true));
			}
			else {
				if (m_SCUI.PassBtn != null) m_SCUI.PassBtn.interactable = false;
				if (m_SCUI.GoBtnName != null) m_SCUI.GoBtnName.text = TDATA.GetString(157);//152 157
				m_SCUI.GoBtnBG.sprite = m_SCUI.GoBtnBGs[1];
				if(m_SCUI.GoBtnIcon != null) m_SCUI.GoBtnIcon.SetActive(false);
			}
		}
	}
	//보상 리스트업(유아이용)
	protected List<RES_REWARD_BASE> GetRewards(TStageTable _table, bool _unboxing = true) {
		m_GetRewards.Clear();
		//RES_REWARD_BASE reward = null;

		//예상 보상 넣어주기, 캐릭터조각(박스) : 요일, 완제 아이템 : 호드, 난투, 달러 : 달러, 경험치 : 경험치
		if (_table.m_ClearGold > 0) {
			RES_REWARD_MONEY rmoney;
			rmoney = new RES_REWARD_MONEY();
			rmoney.Type = Res_RewardType.Cash;
			rmoney.Befor = USERINFO.m_Cash - _table.m_ClearGold;
			rmoney.Now = USERINFO.m_Cash;
			rmoney.Add = _table.m_ClearGold;
			m_GetRewards.Add(rmoney);
		}
		if (_table.m_ClearMoney > 0) {
			RES_REWARD_MONEY rmoney;
			float per = 1f + USERINFO.GetSkillValue(SkillKind.GetDoller) + USERINFO.ResearchValue(ResearchEff.DollarUp);
			int reward = Mathf.RoundToInt(_table.m_ClearMoney * per);
			rmoney = new RES_REWARD_MONEY();
			rmoney.Type = Res_RewardType.Money;
			rmoney.Befor = USERINFO.m_Money - reward;
			rmoney.Now = USERINFO.m_Money;
			rmoney.Add = reward;
			m_GetRewards.Add(rmoney);
		}
		if (_table.m_ClearExp > 0) {
			RES_REWARD_MONEY rmoney;
			float per = 1f + USERINFO.GetSkillValue(SkillKind.GetExp) + USERINFO.ResearchValue(ResearchEff.ExpUp);
			int reward = Mathf.RoundToInt(_table.m_ClearExp * per);
			rmoney = new RES_REWARD_MONEY();
			rmoney.Type = Res_RewardType.Exp;
			rmoney.Befor = USERINFO.m_Exp[0] - reward;
			rmoney.Now = USERINFO.m_Exp[0];
			rmoney.Add = reward;
			m_GetRewards.Add(rmoney);
		}
		for (int i = 0; i < _table.m_ClearReward.Count; i++) {
			m_GetRewards.AddRange(MAIN.GetRewardData(_table.m_ClearReward[i].m_Kind, _table.m_ClearReward[i].m_Idx, _table.m_ClearReward[i].m_Count, _unboxing));
		}

		m_GetRewards.Sort((_before, _after) => {
			if(_before.Type != _after.Type) return _after.Type.CompareTo(_before.Type);
			if ((_before.Type == Res_RewardType.Item && _after.Type == Res_RewardType.Item) || (_before.Type == Res_RewardType.DNA && _after.Type == Res_RewardType.DNA))
				return TDATA.GetItemTable(_after.GetIdx()).m_Grade.CompareTo(TDATA.GetItemTable(_before.GetIdx()).m_Grade);
			if (_before.Type == Res_RewardType.Zombie && _after.Type == Res_RewardType.Zombie) return TDATA.GetZombieTable(_after.GetIdx()).m_Grade.CompareTo(TDATA.GetZombieTable(_before.GetIdx()).m_Grade);
			return _after.GetIdx().CompareTo(_before.GetIdx());
		});
		return m_GetRewards;
	}

	public void Click_GoBuy()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Dungeon_Info, 0)) return;
		if (m_Action != null) return;
		if (!m_CanClick) return;
		if (m_Content == StageContentType.Tower && USERINFO.m_Stage[StageContentType.Tower].Idxs[0].Clear >= m_Lv[1]) return;
		if (USERINFO.m_Stage[m_Content].IS_CanGoStage()) Ready();
		else if (USERINFO.m_Stage[m_Content].IS_CanBuy()) BuyTicket();
		else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(564));
	}
	//준비, 덱세팅으로 넘어가기
	void Ready() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Dungeon_Detail, 0)) return;
		if (m_Stagetable.m_Energy > 0 &&USERINFO.m_Energy.Cnt < m_Stagetable.m_Energy) {
			POPUP.StartLackPop(BaseValue.ENERGY_IDX);
			return;
		}

		if (m_Content == StageContentType.Academy) {
			Click_GoDungeon();
		}
		else {
			DLGTINFO?.f_OBJSndOff?.Invoke();
			SND.StopEff();
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DeckSetting, (result, obj) => {
				if (result == 1) {
					List<int> notetutostgidx = new List<int>() { 1002001, 1002003, 1002006, 1002008, 1002011 };
					if (notetutostgidx.Contains(m_Stagetable.m_Idx) && m_Content == StageContentType.Bank && PlayerPrefs.GetInt($"NoteBattleGuide_{USERINFO.m_UID}_{m_Stagetable.m_Idx}", 0) < 1) {
						PlayerPrefs.SetInt($"NoteBattleGuide_{USERINFO.m_UID}_{m_Stagetable.m_Idx}", 1);
						PlayerPrefs.Save();
						POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Tuto_Video, (result, obj) => { Click_GoDungeon(); }, TutoVideoType.NoteBattle, m_Stagetable.m_Idx);
					}
					else if(m_Content == StageContentType.Tower) {
						UserInfo.StageIdx stgidx = USERINFO.m_Stage[StageContentType.Tower].Idxs[0];
#if NOT_USE_NET
						USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, m_Stagetable.m_Energy);
						PLAY.GoModeStage(StageContentType.Tower, stgidx.Clear + 1);
#else
						PLAY.GetStagePlayCode((result) =>
						{
							if (result != EResultCode.SUCCESS)
							{
								SetUI();
								return;
							}
							PLAY.GoModeStage(StageContentType.Tower, stgidx.Idx);
						}, StageContentType.Tower, stgidx.Idx);
#endif
					}
					else Click_GoDungeon();
				}
			}, m_Stagetable, m_Content, m_Day, m_Pos);
		}
	}
	//시작, 덱세팅에서 시작할때
	public void Click_GoDungeon() {
		m_CanClick = false;
#if NOT_USE_NET
		USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, STAGEINFO.m_TStage.m_Energy);
		switch (m_Content) {
			case StageContentType.Bank: PLAY.GoModeStage(StageContentType.Bank, USERINFO.m_Stage[m_Content].Idxs[0].Idx = m_Lv[0] + 1); break;
			case StageContentType.Academy: PLAY.GoModeStage(StageContentType.Academy, USERINFO.m_Stage[m_Content].Idxs[0].Idx = m_Lv[0] + 1, m_Pos); break;
			case StageContentType.Cemetery: PLAY.GoModeStage(StageContentType.Cemetery, USERINFO.m_Stage[m_Content].Idxs[0].Idx = m_Lv[0] + 1); break;
			case StageContentType.Factory: PLAY.GoModeStage(StageContentType.Factory, USERINFO.m_Stage[m_Content].Idxs[0].Idx = m_Lv[0] + 1); break;
			case StageContentType.University: PLAY.GoWeekModeStage(StageContentType.University, m_Day, m_Pos, USERINFO.m_Stage[m_Content].Idxs.Find(t => t.Week == m_Day && t.Pos == m_Pos).Idx = m_Lv[0] + 1); break;
			case StageContentType.Subway: PLAY.GoModeStage(StageContentType.Subway, m_Lv[0], m_Pos); break;
		}
#else
		PLAY.GetStagePlayCode((result) => {
			if (result != EResultCode.SUCCESS) {
				SetNotCommUI();
				return;
			}
			switch (m_Content) {
				case StageContentType.University:
					PLAY.GoWeekModeStage(StageContentType.University, m_Day, m_Pos, Math.Min(m_Lv[0] + 1, m_Lv[1]));
					break;
				case StageContentType.Subway: 
					PLAY.GoModeStage(StageContentType.Subway, m_Lv[0], m_Pos); break;
				default:
					PLAY.GoModeStage(m_Content, Math.Min(m_Lv[0] + 1, m_Lv[1]), m_Pos);
					//PLAY.GoModeStage(m_Content, m_Lv[0]);
					break;
			}
		}, m_Content, m_Content == StageContentType.Subway ? m_Lv[0] : Math.Min(m_Lv[0] + 1, m_Lv[1]), m_Day, m_Pos);
#endif
	}
	void BuyTicket(bool _usepass = false) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Dungeon_Detail, 2)) return;
		long ticketprice = USERINFO.m_Stage[m_Content].GetTicketPrice();
		string msg = Utile_Class.StringFormat(TDATA.GetString(790), BaseValue.GetPriceTypeName(PriceType.Cash, BaseValue.CASH_IDX));
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, TDATA.GetString(156), msg, (result, obj) => {
			if (result == 1) {
				if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
#if NOT_USE_NET
				USERINFO.GetCash(-ticketprice);
				USERINFO.m_Stage[m_Content].ButItem();
				
				//FireBase-Analytics
				MAIN.GoldToothStatistics(GoldToothContentsType.DownTownTicket, m_Stagetable.m_Idx);

				//버튼 세팅
				SetButtUI();
				if (_usepass) Click_UsePassTicket();
#else
					WEB.SEND_REQ_STAGE_BUY_LIMIT((res) => {
						if (!res.IsSuccess()) {
							WEB.SEND_REQ_STAGE((res2) => {
								//버튼 세팅
								SetButtUI();
							});
							WEB.StartErrorMsg(res.result_code);
							return;
						}
						//FireBase-Analytics
						MAIN.GoldToothStatistics(GoldToothContentsType.DownTownTicket, m_Stagetable.m_Idx);

						//버튼 세팅
						SetButtUI();
						if (_usepass) Click_UsePassTicket();
					}, USERINFO.m_Stage[m_Content].UID);
#endif
				}
				else {
					POPUP.StartLackPop(BaseValue.CASH_IDX);
				}
			}
		}, PriceType.Cash, BaseValue.CASH_IDX, (int)ticketprice, false);
	}
	/// <summary> 소탕권 사용 </summary>
	public void Click_UsePassTicket() {
		if (m_Action != null) return;
		if (!m_CanClick) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Dungeon_Detail, 1)) return;
		if (!USERINFO.m_Stage[m_Content].IS_CanGoStage() && USERINFO.m_Stage[m_Content].IS_CanBuy()) {
			BuyTicket(true);
			return;
		}

		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Pass_Use, (result, obj) => {
			if (result > 0) {
				//보상만 줌
#if NOT_USE_NET
				//소탕권 소모
				USERINFO.DeleteItem(BaseValue.CLEARTICKET_IDX, result); 
				USERINFO.m_Stage[m_Content].UseItem();
				int lv = 0;
				lv = m_Content == StageContentType.University ? USERINFO.m_Stage[m_Content].Idxs.Find(t => t.Week == m_Day && t.Pos == m_Pos).Idx = m_Lv[0] + 1 : USERINFO.m_Stage[m_Content].Idxs[0].Idx = m_Lv[0] + 1;
				TModeTable modetable = TDATA.GetModeTable(m_Content, lv, m_Day, m_Pos);
				STAGEINFO.SetStage(StagePlayType.OutContent, TDATA.GetStageTable(modetable.m_StageIdx).m_Mode, modetable.m_StageIdx, lv, m_Day, m_Pos);
				
				USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, STAGEINFO.m_TStage.m_Energy * result);

				for(int i = 0;i< result;i++) USERINFO.OutGameClear();

				TStageTable table = STAGEINFO.m_TStage;
				List<RES_REWARD_BASE> m_GetRewards = new List<RES_REWARD_BASE>();
				int Exp = table.m_ClearExp;
				int Money = table.m_ClearMoney;
				int Gold = table.m_ClearGold;
				int UserExp = table.m_ClearUserExp;

				Exp = (int)USERINFO.SetIngameExp(Exp * result, true);
				Money = (int)USERINFO.ChangeMoney(Money * result, true);
				Gold *= result;
				UserExp *= result;
				USERINFO.GetCash(Gold);
				USERINFO.SetUserExp(UserExp);

				for (int ticketcnt = 0; ticketcnt < result; ticketcnt++) {
					for (int i = 0; i < table.m_ClearReward.Count; i++) {

						switch (table.m_ClearReward[i].m_Kind) {
							case RewardKind.None:
								break;
							case RewardKind.Character:
								CharInfo charinfo = USERINFO.m_Chars.Find(t => t.m_Idx == table.m_ClearReward[i].m_Idx);
								if (charinfo != null) {
									POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(186));
									ItemInfo pieceitem = USERINFO.InsertItem(charinfo.m_TData.m_PieceIdx, BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade));
									m_GetRewards.Add(new RES_REWARD_ITEM() {
										Type = Res_RewardType.Item,
										UID = pieceitem.m_Uid,
										Idx = pieceitem.m_Idx,
										Cnt = pieceitem.m_TData.GetEquipType() == EquipType.End ? BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade) : 1,
										result_code = EResultCode.SUCCESS_REWARD_PIECE
									});
								}
								else {
									CharInfo charInfo = USERINFO.InsertChar(table.m_ClearReward[i].m_Idx);
									RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
									rchar.SetData(charInfo);
									m_GetRewards.Add(rchar);
								}
								break;
							case RewardKind.Item:
								TItemTable itemTable = TDATA.GetItemTable(table.m_ClearReward[i].m_Idx);
								if (itemTable.m_Type == ItemType.RandomBox || itemTable.m_Type == ItemType.AllBox) {//박스는 바로 까서 주기
									List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
									for (int j = table.m_ClearReward[i].m_Count - 1; j > -1; j--) rewards.AddRange(TDATA.GetGachaItem(itemTable));
									for (int j = 0; j < rewards.Count; j++) {
										// 캐릭터 보상은 없음
										if (rewards[j].Type == Res_RewardType.Char) continue;
										//RES_REWARD_ITEM item = (RES_REWARD_ITEM)rewards[j];
										m_GetRewards.Add(rewards[j]);
									}
								}
								else {
									ItemInfo iteminfo = USERINFO.InsertItem(table.m_ClearReward[i].m_Idx, table.m_ClearReward[i].m_Count);
									TItemTable tdata = TDATA.GetItemTable(table.m_ClearReward[i].m_Idx);
									RES_REWARD_MONEY rmoney;
									RES_REWARD_ITEM ritem;
									switch (tdata.m_Type) {
										case ItemType.Dollar:
											rmoney = new RES_REWARD_MONEY();
											rmoney.Type = Res_RewardType.Money;
											rmoney.Befor = USERINFO.m_Money;
											rmoney.Now = USERINFO.m_Money;
											rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
											m_GetRewards.Add(rmoney);
											break;
										case ItemType.Cash:
											rmoney = new RES_REWARD_MONEY();
											rmoney.Type = Res_RewardType.Cash;
											rmoney.Befor = USERINFO.m_Cash;
											rmoney.Now = USERINFO.m_Cash;
											rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
											m_GetRewards.Add(rmoney);
											break;
										case ItemType.Energy:
											rmoney = new RES_REWARD_MONEY();
											rmoney.Type = Res_RewardType.Energy;
											rmoney.Befor = USERINFO.m_Energy.Cnt;
											rmoney.Now = USERINFO.m_Energy.Cnt;
											rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
											rmoney.STime = (long)USERINFO.m_Energy.STime;
											m_GetRewards.Add(rmoney);
											break;
										case ItemType.InvenPlus:
											rmoney = new RES_REWARD_MONEY();
											rmoney.Type = Res_RewardType.Inven;
											rmoney.Befor = USERINFO.m_InvenSize;
											rmoney.Now = USERINFO.m_InvenSize;
											rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
											m_GetRewards.Add(rmoney);
											break;
										default:
											ritem = new RES_REWARD_ITEM();
											ritem.Type = Res_RewardType.Item;
											ritem.UID = iteminfo.m_Uid;
											ritem.Idx = table.m_ClearReward[i].m_Idx;
											ritem.Cnt = table.m_ClearReward[i].m_Count;
											m_GetRewards.Add(ritem);
											break;
									}
									break;
								}
								break;
							case RewardKind.Zombie:
								ZombieInfo zombieInfo = USERINFO.InsertZombie(table.m_ClearReward[i].m_Idx);
								RES_REWARD_ZOMBIE zombie = new RES_REWARD_ZOMBIE();
								zombie.UID = zombieInfo.m_UID;
								zombie.Idx = zombieInfo.m_Idx;
								zombie.Grade = zombieInfo.m_Grade;
								m_GetRewards.Add(zombie);
								break;
							case RewardKind.DNA:
								TDnaTable dnaTable = TDATA.GetDnaTable(table.m_ClearReward[i].m_Idx);
								DNAInfo dnaInfo = new DNAInfo(dnaTable.m_Idx);
								USERINFO.m_DNAs.Add(dnaInfo);
								RES_REWARD_DNA dna = new RES_REWARD_DNA();
								dna.UID = dnaInfo.m_UID;
								dna.Idx = dnaInfo.m_Idx;
								dna.Grade = dnaInfo.m_Grade;
								m_GetRewards.Add(dna);
								break;
						}
					}
				}
				
				SetButtUI(); 
				POPUP.GetMainUI().GetComponent<Main_Play>().CB_Dungeon_Refresh();

				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Pass_Result, (res, obj)=> { 
				}, Exp, Money, Gold, UserExp, m_GetRewards);
#else
				WEB.SEND_REQ_STAGE_CLEAR_TICKET((res) => {
					if (!res.IsSuccess()) {
						WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
						return;
					}
					SetButtUI();
					USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, m_Stagetable.m_Energy * result);

					POPUP.GetMainUI().GetComponent<Main_Play>().CB_Dungeon_Refresh();
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Pass_Result, (res, obj) => {
					}, res);
				}, USERINFO.m_Stage[m_Content], m_Day, m_Pos, m_Content == StageContentType.Subway ? m_Lv[0] : m_Lv[0] + 1, result);
#endif
			}
		}, (int)USERINFO.m_Stage[m_Content].GetItemCnt(false), USERINFO.m_Stage[m_Content].GetItemMax(false), m_Stagetable.m_Energy, false);
	}
	public virtual void ScrollLock(bool _lock) {

	}
}
