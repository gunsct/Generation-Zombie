using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class AddEvent : PopupBase
{
	/// <summary> 암상인 유아이</summary>
	[Serializable]
	public struct SBUI {
		public Animator PurchaseAnim;
		public Item_RewardItem_Card SellItemCard;
		public Image[] PayIcon;
		public TextMeshProUGUI SellItemName;
		public TextMeshProUGUI[] SellPrice;//0 : 할인전 1:할인후
		public TextMeshProUGUI SellDiscount;
		public TextMeshProUGUI BuyBtnTxt;
		public Button BuyBtn;
		public GameObject MoneyGoldGroup;
		public GameObject[] MoneyGolds;
	}
	/// <summary> 엔피씨 대화 유아이</summary>
	[Serializable]
	public struct SNUI {
		public Select[] Selects;
		public Speech[] Speechs;    //엔피씨와 유저 대사 0:유저 1:플레이어
		public Image UserIcon;
		public TextMeshProUGUI UserName;
	}
	[Serializable]
	public class Speech {
		public GameObject OBJ;
		public Animator Anim;
		public Item_Talk_Talk Talk;
		public TextMeshProUGUI Txt;
		public bool Delete;
	}
	[Serializable]
	public class Select {
		public Animator Anim;
		public GameObject OBJ;
		public TextMeshProUGUI Txt;
	}
	[Serializable]
	public struct SUI {
		public Animator Anim;
		public GameObject[] Panels;
		public Image NPCPortrait;
		public Image EnemyPortrait;
		public Animator RaidAnim;
		public GameObject SelectGroup;
		public GameObject PurchaseGroup;
		public GameObject Block;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SBUI m_SBUI;
	[SerializeField] SNUI m_SNUI;

	int m_Idx;
	float m_SpeechTimer = 3f;
	TEventTable m_TData { get { return TDATA.GetEventTable(m_Idx); } }
	TShopTable m_TSData;
	List<TCaseSelectTable> m_TCSTDatas = new List<TCaseSelectTable>();
	List<RES_REWARD_BASE> m_AddEventReward = new List<RES_REWARD_BASE>();
	IEnumerator m_TalkCor;
	IEnumerator m_SpeechCor;
	IEnumerator m_Action;
	int[] m_SpeechPos = new int[2];
	int m_MiddleDLIdx = 0;
	bool m_GoBattle = false;

	private void Awake() {
		for (int i = 0; i < 2; i++) {
			m_SUI.Panels[i].SetActive(false);
			m_SNUI.Speechs[i].OBJ.SetActive(false);
		}
		for(int i = 0;i<4;i++)
			m_SNUI.Selects[i].OBJ.SetActive(false);
		m_SUI.SelectGroup.SetActive(false);
		m_SUI.PurchaseGroup.SetActive(false);
		m_SUI.Block.SetActive(false);
		m_SBUI.MoneyGoldGroup.SetActive(false);
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_Idx = (int)aobjValue[0];

		m_SUI.NPCPortrait.sprite = m_TData.GetIcon();
		m_SUI.NPCPortrait.color = Utile_Class.GetCodeColor("#FFFFFF");
		SetUser();
		m_SUI.Panels[m_TData.m_EventType == AddEventType.Battle ? 1 : 0].SetActive(true);

		switch (m_TData.m_EventType) {
			case AddEventType.BlackMarket:
				SetBlackMarket();
				break;
			case AddEventType.NPC:
				StartCoroutine(SetNPC(m_TData.m_EventType));
				break;
			case AddEventType.Battle:
				SetAtk();
				break;
		}
#if NOT_USE_NET
		USERINFO.m_AddEvent = 0;
		MAIN.Save_UserInfo();
#endif
	}
	void SetAtk() {
		m_SUI.EnemyPortrait.sprite = m_TData.GetIcon();
		m_SUI.RaidAnim.SetTrigger("Start");
		TEnemyTable enemy = TDATA.GetEnemyTable(m_TData.m_EnemyIdx);
		switch (enemy.m_Tribe) {
			case EEnemyTribe.Human:
			case EEnemyTribe.Mutant:
			case EEnemyTribe.Zombie:
			case EEnemyTribe.Animal:
				SND.DelayPlayEffSound(1.5f, SND_IDX.VOC_1500);
				break;
		}
		StartCoroutine(SuddenAtkAction());
	}
	void SetUser(TDialogTable _table = null) {
		if(_table != null) {
			TTalkerTable talk = TDATA.GetTalkerTable(_table.m_TalkerIdx);
			if (talk == null) {
				SetUser();
				return;
			}
			m_SNUI.UserIcon.sprite = talk.GetSprPortrait();
			m_SNUI.UserName.text = talk.GetName();
		}
		else {
			m_SNUI.UserIcon.sprite = TDATA.GetUserProfileImage(USERINFO.m_Profile);
			m_SNUI.UserName.text = USERINFO.m_Name;
		}
	}
	IEnumerator SuddenAtkAction() {
		SetUser();
		StartCoroutine(m_SpeechCor = SetSpeech(2, m_TData.GetTitle(), true));
		yield return new WaitWhile(() => m_SpeechCor != null);

		//전투여부 판단
		m_GoBattle = false;
		TEnemyTable enemy = TDATA.GetEnemyTable(m_TData.m_EnemyIdx);
		switch (enemy.m_Tribe) {
			case EEnemyTribe.Human:
			case EEnemyTribe.Mutant:
				m_TalkCor = Talk(m_TData.m_Val);
				yield return m_TalkCor;
				break;
			case EEnemyTribe.Zombie:
			case EEnemyTribe.Animal:
				m_GoBattle = true;
				break;
		}

		yield return new WaitWhile(() => m_TalkCor != null);
		yield return new WaitWhile(() => m_Action != null);

		if (m_GoBattle) {//전투 후 보상
			m_SUI.RaidAnim.SetTrigger("In");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.RaidAnim));

			USERINFO.m_AddEvent = 0;
			if (PlayerPrefs.GetInt($"NoteBattleGuide_{USERINFO.m_UID}", 0) < 1) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Tuto_Video, (result, obj) => {
					PlayerPrefs.SetInt($"NoteBattleGuide_{USERINFO.m_UID}", 1);
					PlayerPrefs.Save();
					PlaySuddenAtk();
				}, TutoVideoType.NoteBattle, STAGEINFO.m_Idx);
			}
			else PlaySuddenAtk();
		}
	}
	void PlaySuddenAtk() {
		PLAY.SuddenAtkEvent(m_TData.m_EnemyIdx, (result) => {
			if (result == 1) {
#if NOT_USE_NET
				AddEventReward(AddEventType.Battle, m_TData.m_Rewards);
#else
				WEB.SEND_REQ_ADD_EVENT_SUDDENENEMY((res) => {
					if (!res.IsSuccess()) {
						WEB.StartErrorMsg(res.result_code);
						return;
					}
					if (res.Rewards == null) return;
					m_AddEventReward.AddRange(res.GetRewards());
					if (m_AddEventReward.Count < 1) return;
					ViewEventReward();
				}, true);
#endif
			}
			else {
#if NOT_USE_NET
				m_Action = IE_CloseAction(0);
				StartCoroutine(m_Action);
#else
				WEB.SEND_REQ_ADD_EVENT_SUDDENENEMY((res) => {
					if (!res.IsSuccess()) {
						WEB.StartErrorMsg(res.result_code);
						return;
					}
					m_Action = IE_CloseAction(0);
					StartCoroutine(m_Action);
				}, false);
#endif
			}
		});
	}
	IEnumerator SetNPC(AddEventType _type) {
		PlayEffSound(SND_IDX.SFX_1910);

		string npcspeech = string.Format(m_TData.GetTitle(), USERINFO.m_Name, USERINFO.m_AddEventName);
		SetUser();
		StartCoroutine(m_SpeechCor = SetSpeech(1, npcspeech, true));
		yield return new WaitWhile(() => m_SpeechCor != null);

		//선택지 나올 경우 선택지 키면서 애니 다 넣어줘야함
		m_TalkCor = Talk(m_TData.m_Val);
		StartCoroutine(m_TalkCor);
	}
	IEnumerator Talk(int _dlidx) {
		TDialogTable dltable = TDATA.GetDialogTable(_dlidx);
		if (dltable == null) {
			m_TalkCor = null;
			yield break;
		}
		if (dltable.m_SelectGID != 0) {
			m_TCSTDatas = TDATA.GetCaseSelectGroupTable(dltable.m_SelectGID);
			for (int i = 0; i < m_TCSTDatas.Count; i++) {
				m_SNUI.Selects[i].Txt.text = m_TCSTDatas[i].GetString();
			}
			m_SUI.NPCPortrait.color = Utile_Class.GetCodeColor("#FFFFFF");
			m_SUI.SelectGroup.SetActive(true);
			m_SUI.Block.SetActive(false);

			for (int i = 0; i < m_TCSTDatas.Count; i++) {
				m_SNUI.Selects[i].OBJ.SetActive(true);
				m_SNUI.Selects[i].Anim.SetTrigger("Start");
				PlayEffSound(SND_IDX.SFX_0340);
				//yield return new WaitForSeconds(0.5f);
			}
			yield break;
		}
		else {
			//left - npc, right - user
			string npcspeech = string.Format(dltable.GetDesc(), USERINFO.m_Name, USERINFO.m_AddEventName);

			switch (dltable.m_Dir) {
				case DialogTalkDir.Right:
					SetUser(dltable);
					StartCoroutine(m_SpeechCor = SetSpeech(0, npcspeech, dltable.m_SelectGID == 0));
					yield return new WaitWhile(() => m_SpeechCor != null);
					break;
				case DialogTalkDir.Left:
					m_SNUI.Speechs[1].OBJ.transform.localPosition = new Vector3(0f, dltable.m_SelectGID == 0 ? 100f : 281f, 0f);
					SetUser();
					StartCoroutine(m_SpeechCor = SetSpeech(1, npcspeech, dltable.m_SelectGID == 0));
					yield return new WaitWhile(() => m_SpeechCor != null);
					break;
			}
		}
		if (dltable.m_NextDLIdx != 0) {
			yield return m_TalkCor = Talk(dltable.m_NextDLIdx);
		}
		else if (dltable.m_SelectGID == 0) {//비선택 완료시
			if (m_MiddleDLIdx == 0) {
#if NOT_USE_NET
				if (m_TData.m_Rewards.Count > 0) //특정보상은 이벤트 테이블
					AddEventReward(m_TData.m_EventType, m_TData.m_Rewards);
				else //랜덤보상
					PickRandomReward();
#else
			WEB.SEND_REQ_ADDEVENT_END((res) => {
				if (!res.IsSuccess()) {
					WEB.StartErrorMsg(res.result_code);
					return;
				}

				if (res.Rewards == null) return;
				m_AddEventReward.AddRange(res.GetRewards());
				if (m_AddEventReward.Count < 1) return;
				ViewEventReward(true);
			});
#endif
			}
			else {
				m_TalkCor = null;
				yield break;
			}
		}
	}

	void SetBlackMarket() {
		PlayEffSound(SND_IDX.SFX_1910);
		m_SBUI.MoneyGoldGroup.SetActive(true);

		List<TShopTable> shopgroup = TDATA.GetGroupShopTable(ShopGroup.Event_BlackMarket);
		shopgroup.FindAll(o => o.m_FirstPrice != o.GetPrice() && o.m_NoOrProb > 0);
		m_TSData = shopgroup[UTILE.Get_Random(0, shopgroup.Count)];

		TItemTable itemtable = TDATA.GetItemTable(m_TSData.m_Rewards[0].m_ItemIdx);
		m_SBUI.SellItemCard.SetData(itemtable.m_Idx, m_TSData.m_Rewards[0].m_ItemCnt);
		m_SBUI.SellItemName.text = itemtable.GetName();
		//할인률은 아이템인덱스1 을 그룹으로 첫번째꺼가 0%할인이니 그거 기준
		float defaultprice = m_TSData.m_FirstPrice;
		m_SBUI.SellPrice[0].text = Utile_Class.CommaValue(defaultprice);
		m_SBUI.SellPrice[1].text = Utile_Class.CommaValue(m_TSData.GetPrice());
		m_SBUI.SellDiscount.text = string.Format("{0} %", Mathf.RoundToInt((defaultprice - (float)m_TSData.GetPrice()) / defaultprice * 100f));
		string pricetypename = string.Empty;
		switch (m_TSData.m_PriceType) {
			case PriceType.Money:
				m_SBUI.MoneyGolds[1].SetActive(false);
				DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
				m_SBUI.PayIcon[0].sprite = m_SBUI.PayIcon[1].sprite = BaseValue.GetItemIcon(ItemType.Dollar);
				pricetypename = TDATA.GetItemTable(BaseValue.DOLLAR_IDX).GetName();
				m_SBUI.BuyBtn.interactable = USERINFO.m_Money >= m_TSData.GetPrice();
				m_SBUI.BuyBtnTxt.text = TDATA.GetString(m_SBUI.BuyBtn.interactable ? 429 : 436);
				break;
			case PriceType.Cash:
				m_SBUI.MoneyGolds[0].SetActive(false);
				DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
				m_SBUI.PayIcon[0].sprite = m_SBUI.PayIcon[1].sprite = BaseValue.GetItemIcon(ItemType.Cash);
				pricetypename = TDATA.GetItemTable(BaseValue.CASH_IDX).GetName();
				m_SBUI.BuyBtn.interactable = USERINFO.m_Cash >= m_TSData.GetPrice();
				m_SBUI.BuyBtnTxt.text = TDATA.GetString(m_SBUI.BuyBtn.interactable ? 429 : 436);
				break;
		}
		m_SNUI.Speechs[1].OBJ.transform.localPosition = new Vector3(0f, 400f, 0f);
		SetUser();
		StartCoroutine(m_SpeechCor = SetSpeech(1, Utile_Class.StringFormat(TDATA.GetString(428), Utile_Class.CommaValue(m_TSData.GetPrice()), pricetypename), true));
		m_SUI.Block.SetActive(false);

		m_SUI.PurchaseGroup.SetActive(true);
		m_SBUI.PurchaseAnim.SetTrigger("Start");
	}
	IEnumerator SetSpeech(int _pos, string _txt, bool _del) {
		m_SNUI.Speechs[_pos].OBJ.SetActive(false);
		m_SpeechPos[0] = _pos;
		m_SUI.NPCPortrait.color = Utile_Class.GetCodeColor(_pos == 0 ? "#808080" : "#FFFFFF");

		if(m_SNUI.Speechs[_pos].Txt != null) m_SNUI.Speechs[_pos].Txt.text = string.Empty;
		m_SNUI.Speechs[_pos].Delete = _del;
		m_SUI.Block.SetActive(_del);

		if (m_TData.m_EventType == AddEventType.Battle)
			yield return new WaitForSeconds(0.8f);

		PlayEffSound(SND_IDX.SFX_0345);
		m_SNUI.Speechs[_pos].OBJ.SetActive(true);

		DialogTalkType talktype = DialogTalkType.Normal;
		switch (_pos) {
			case 0:
			case 1: talktype = DialogTalkType.Normal; break;
			case 2: talktype = DialogTalkType.Shout; break;
		}
		if (m_SNUI.Speechs[_pos].Talk != null) m_SNUI.Speechs[_pos].Talk.SetData(talktype, _txt, true, 1.5f);
		m_SNUI.Speechs[_pos].Anim.SetTrigger("Start");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SNUI.Speechs[_pos].Anim));

		yield return new WaitForSeconds(m_SpeechTimer);

		m_SNUI.Speechs[_pos].Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SNUI.Speechs[_pos].Anim));

		m_SNUI.Speechs[_pos].OBJ.SetActive(false);
		m_SpeechPos[1] = m_SpeechPos[0];
		m_SpeechCor = null;
	}
	public void ClickSelect(int _pos) {
		if (m_Action != null) return;
		m_Action = SelectAction(_pos);
		StartCoroutine(m_Action);
	}
	IEnumerator SelectAction(int _pos) {
		Animator anim = null;
		m_SNUI.Selects[_pos].Anim.SetTrigger("Touch");
		PlayEffSound(SND_IDX.SFX_0341);

		for (int i = 0; i < 4; i++) {
			if (m_SNUI.Selects[i].OBJ.activeSelf && i != _pos) {
				yield return new WaitForSeconds(0.3f);
				anim = m_SNUI.Selects[i].Anim;
				anim.SetTrigger("End");
			}
		}

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SNUI.Selects[_pos].Anim) && Utile_Class.IsAniPlay(anim));

		for (int i = 0; i < 4; i++) {
			m_SNUI.Selects[i].OBJ.SetActive(false);
		}

		m_MiddleDLIdx = m_TCSTDatas[_pos].m_MiddleDLIdx;
		if (m_MiddleDLIdx != 0) {
			yield return m_TalkCor = Talk(m_MiddleDLIdx);
			yield return new WaitWhile(() => m_TalkCor != null);
		}
		if(m_TCSTDatas[_pos].m_Rewards.Find(o=>o.m_RewardType == RewardKind.Enemy) != null) {
			m_GoBattle = true;
			m_Action = null;
			yield break;
		}
		else { //지정 보상
#if NOT_USE_NET
			if (m_TCSTDatas[_pos].m_Rewards.Count > 0) { //지정 보상
				List<RewardInfo> infos = new List<RewardInfo>();
				for (int i = 0; i < m_TCSTDatas[_pos].m_Rewards.Count; i++) {
					if (m_TCSTDatas[_pos].m_Rewards[i].m_RewardType == RewardKind.Item) {
						infos.Add(new RewardInfo() {
							Kind = RewardKind.Item,
							Idx = m_TCSTDatas[_pos].m_Rewards[i].m_Value,
							LV = 1,
							Grade = 1,
							Cnt = m_TCSTDatas[_pos].m_Rewards[i].m_Count
						});
					}
				}
				AddEventReward(m_TData.m_EventType, infos);
			}
			else //랜덤 보상
				PickRandomReward();
#else
		WEB.SEND_REQ_ADD_EVENT_NPC((res) => {
			if (!res.IsSuccess()) {
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			
			if (res.Rewards == null) return;
			m_AddEventReward.AddRange(res.GetRewards());
			if (m_AddEventReward.Count < 1) return;
			ViewEventReward();
		}, m_TCSTDatas[_pos].m_Idx);
#endif
		}
	}
	public void ClickBuy() {
		if (m_Action != null) return;
		m_Action = BuyEnd();
#if NOT_USE_NET
		switch (m_TSData.m_PriceType) {
			case PriceType.Money:
				USERINFO.ChangeMoney(-m_TSData.GetPrice());
				break;
			case PriceType.Cash:
				USERINFO.GetCash(-m_TSData.GetPrice());
				break;
		}
		USERINFO.InsertItem(m_TSData.m_Rewards[0].m_ItemIdx);
		StartCoroutine(m_Action);
#else
		WEB.SEND_REQ_ADD_EVENT_BLACKMARKET((res) => {
			if (!res.IsSuccess())
			{
				WEB.SEND_REQ_ALL_INFO((res2) => { SetUI(); });
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			PlayEffSound(SND_IDX.SFX_0321);
			PlayEffSound(SND_IDX.SFX_1010);
			WEB.SEND_REQ_ADDEVENT_END((res2) => {
				MAIN.SetRewardList(new object[] { res.GetRewards() }, 
					() => { StartCoroutine(m_Action); });
			});
		},m_TSData.m_Idx);
#endif
	}
	IEnumerator BuyEnd() {
		yield return new WaitForSeconds(1f);

		m_SBUI.PurchaseAnim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SBUI.PurchaseAnim));
		yield return IE_CloseAction(0);
	}
	void PickRandomReward() {
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
		for(int i = 0;i< m_TData.m_Rewards.Count; i++) {
			TItemTable itemTable = TDATA.GetItemTable(m_TData.m_Rewards[i].Idx);
			for (int j = m_TData.m_Rewards[i].Cnt - 1; j > -1; j--) rewards.AddRange(TDATA.GetGachaItem(itemTable));
		}
		AddEventReward(m_TData.m_EventType, null, rewards);
	}
	void AddEventReward(AddEventType _type, List<RewardInfo> _rewards = null, List<RES_REWARD_BASE> _resrewards = null) {
		if(_rewards != null)
			for (int i = 0; i < _rewards.Count; i++) {
				switch (_rewards[i].Kind) {
					case RewardKind.Item:
						if (TDATA.GetItemTable(_rewards[i].Idx).m_Type == ItemType.RandomBox) {//박스는 바로 까서 주기
							List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
							TItemTable itemTable = TDATA.GetItemTable(_rewards[i].Idx);
							for (int j = _rewards.Count - 1; j > -1; j--) rewards.AddRange(TDATA.GetGachaItem(itemTable));
							for (int j = 0; j < rewards.Count; j++) {
								// 캐릭터 보상은 없음
								if (rewards[j].Type == Res_RewardType.Char) continue;
								RES_REWARD_ITEM item = (RES_REWARD_ITEM)rewards[j];
								m_AddEventReward.Add(rewards[j]);
							}
						}
						else {
							TItemTable itemtable = TDATA.GetItemTable(_rewards[i].Idx);
							ItemInfo item = USERINFO.InsertItem(_rewards[i].Idx, _rewards[i].Cnt);
							switch (itemtable.m_Type) {
								case ItemType.Dollar:
									m_AddEventReward.Add(new RES_REWARD_MONEY() {
										Type = Res_RewardType.Money,
										Befor = USERINFO.m_Money - _rewards[i].Cnt,
										Now = USERINFO.m_Money,
										Add = _rewards[i].Cnt
									});
									break;
								case ItemType.Cash:
									m_AddEventReward.Add(new RES_REWARD_MONEY() {
										Type = Res_RewardType.Cash,
										Befor = USERINFO.m_Cash - _rewards[i].Cnt,
										Now = USERINFO.m_Cash,
										Add = _rewards[i].Cnt
									});
									break;
								case ItemType.Exp:
									m_AddEventReward.Add(new RES_REWARD_MONEY() {
										Type = Res_RewardType.Exp,
										Befor = USERINFO.m_Exp[1] - _rewards[i].Cnt,
										Now = USERINFO.m_Exp[1],
										Add = _rewards[i].Cnt
									});
									break;
								case ItemType.Energy:
									m_AddEventReward.Add(new RES_REWARD_MONEY() {
										Type = Res_RewardType.Energy,
										Befor = USERINFO.m_Energy.Cnt - _rewards[i].Cnt,
										Now = USERINFO.m_Energy.Cnt,
										Add = _rewards[i].Cnt
									});
									break;
								case ItemType.InvenPlus:
									m_AddEventReward.Add(new RES_REWARD_MONEY() {
										Type = Res_RewardType.Inven,
										Befor = USERINFO.m_InvenSize - _rewards[i].Cnt,
										Now = USERINFO.m_InvenSize,
										Add = _rewards[i].Cnt
									});
									break;
								default:
									m_AddEventReward.Add(new RES_REWARD_ITEM() {
										Type = Res_RewardType.Item,
										UID = item.m_Uid,
										Idx = item.m_Idx,
										Cnt = itemtable.GetEquipType() == EquipType.End ? _rewards[i].Cnt : 1
									});
									break;
							}
						}
						break;
					case RewardKind.Character:
						CharInfo info = USERINFO.m_Chars.Find(t => t.m_Idx == _rewards[i].Idx);
						if (info != null) {
							POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(186));
							ItemInfo pieceitem = USERINFO.InsertItem(info.m_TData.m_PieceIdx, BaseValue.STAR_OVERLAP(info.m_TData.m_Grade));
							m_AddEventReward.Add(new RES_REWARD_ITEM() {
								Type = Res_RewardType.Item,
								UID = pieceitem.m_Uid,
								Idx = pieceitem.m_Idx,
								Cnt = pieceitem.m_TData.GetEquipType() == EquipType.End ? BaseValue.STAR_OVERLAP(info.m_TData.m_Grade) : 1,
								result_code = EResultCode.SUCCESS_REWARD_PIECE
							});
						}
						else {
							CharInfo charInfo = USERINFO.InsertChar(_rewards[i].Idx);
							RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
							rchar.SetData(charInfo);
							m_AddEventReward.Add(rchar);
						}
						break;
					case RewardKind.Zombie:
						ZombieInfo zombieInfo = USERINFO.InsertZombie(_rewards[i].Idx);
						m_AddEventReward.Add(new RES_REWARD_ZOMBIE {
							Grade = zombieInfo.m_Grade,
							Idx = zombieInfo.m_Idx,
							UID = zombieInfo.m_UID
						});
						break;
					case RewardKind.DNA:
						DNAInfo dnaInfo = USERINFO.InsertDNA(_rewards[i].Idx, 1);
						m_AddEventReward.Add(new RES_REWARD_DNA {
							Grade = dnaInfo.m_Grade,
							Idx = dnaInfo.m_Idx,
							UID = dnaInfo.m_UID
						});
						break;
				}
			}
		if (_resrewards != null) m_AddEventReward.AddRange(_resrewards);
		MAIN.Save_UserInfo();

		ViewEventReward();
	}
	void ViewEventReward(bool _sendend = false) {
		MAIN.SetRewardList(new object[] { m_AddEventReward }, () => {
			m_Action = IE_CloseAction(0);
			StartCoroutine(m_Action);
		});
	}
	public void ClickSkip() {
		m_SpeechPos[1] = m_SpeechPos[0];

		if (m_SNUI.Speechs[m_SpeechPos[1]].Talk.IsAction()) {
			m_SNUI.Speechs[m_SpeechPos[1]].Talk.Set_Action_Finishied();
		}
		else {
			m_SNUI.Speechs[m_SpeechPos[1]].OBJ.SetActive(false);
			if (m_SpeechCor != null) {
				StopCoroutine(m_SpeechCor);
				m_SpeechCor = null;
			}
		}
	}
	public void ClickClose() {
		if (m_Action != null) return;
		if (m_TData.m_EventType == AddEventType.BlackMarket) PlayEffSound(SND_IDX.SFX_0341);
		m_Action = IE_CloseAction(0);
		StartCoroutine(m_Action);
	}
	IEnumerator IE_CloseAction(int _result) {
		//if (m_TData.m_EventType == AddEventType.Battle) {
		//	m_SUI.RaidAnim.SetTrigger("End");
		//	yield return new WaitForEndOfFrame();
		//	yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.RaidAnim));
		//}
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		Close(_result);
	}
}
