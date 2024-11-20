using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Stage_Result : PopupBase
{
	[System.Serializable]
	public struct SSUI
	{
		public CanvasGroup[] CharacterAlpha;
		public Image[] CharacterPortraits;
		public Image[] CharacterGrades;
		public GameObject RewardCardPrefab;
		public Transform ItemBucket;
		public ScrollRect Scroll;
	}
	[System.Serializable]
	public struct SFUI
	{
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Desc;
		public Image Icon;
		public Item_Stage_Result_UserPer UserPer;
	}
	[Serializable]
	public struct SUUI
	{
		public TextMeshProUGUI AddExp;
		public Image Portrait;
		public TextMeshProUGUI Lv;
		public TextMeshProUGUI Exp;
		public Image ExpGague;
		public TextMeshProUGUI UpLv;
	}
	[SerializeField] Animator Anim;
	[SerializeField]
	SSUI m_SSUI;
	[SerializeField]
	SFUI m_SFUI;
	[SerializeField]
	SUUI m_SUUI;

	RES_REWARD_USEREXP m_UserExp = new RES_REWARD_USEREXP();
	bool m_RewardEnd;
	/// <summary>서버서 인벤토리 부족 체크</summary>
	bool m_IsInvenLowGoPost;
	List<Item_RewardList_Item> m_Rewards = new List<Item_RewardList_Item>();
	List<RES_REWARD_BASE> m_GetRewards = new List<RES_REWARD_BASE>();
	Dictionary<Res_RewardType, RectTransform> m_RewardPos = new Dictionary<Res_RewardType, RectTransform>();
	int m_BLv;
	long m_BExp;
	long m_AExp;
	private void Awake() {
		Time.timeScale = 1f;
	}
	/// <summary> aobjValue 0 : bool </summary>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		if (POPUP.GetMainUI() != null && POPUP.GetMainUI().m_Popup == PopupName.Stage) ((Main_Stage)POPUP.GetMainUI()).StopLoopSND();
		CharInfo randchar = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[UTILE.Get_Random(0, USERINFO.m_PlayDeck.GetDeckCharCnt())]);
		if (randchar != null) {
			SND_IDX clearvocidx = randchar.m_TData.GetVoice(TCharacterTable.VoiceType.StageClear);
			PlayVoiceSnd(new List<SND_IDX>() { clearvocidx });
		}

		//1-0, 2-0,1 3-0,1,3 4-0,2,3,4 5-0,1,2,3,4
		int charcnt = USERINFO.m_PlayDeck.GetDeckCharCnt();
		for (int i = 0; i < 5; i++) m_SSUI.CharacterAlpha[i].alpha = 0f;
		switch (charcnt) {
			case 1:
			case 2:
			case 5:
				for (int i = 0; i < charcnt; i++) {
					m_SSUI.CharacterPortraits[i].sprite = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]).m_TData.GetPortrait();
					m_SSUI.CharacterGrades[i].sprite = BaseValue.CharBG(USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]).m_Grade);
				}
				break;
			case 3:
				for (int i = 0, objpos = 0; i < charcnt; i++, objpos++) {
					if (i == 2) objpos++;
					m_SSUI.CharacterPortraits[objpos].sprite = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]).m_TData.GetPortrait();
				m_SSUI.CharacterGrades[objpos].sprite = BaseValue.CharBG(USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]).m_Grade);
				}
				break;
			case 4:
				for (int i = 0, objpos = 0; i < charcnt; i++, objpos++) {
					if (i == 1) objpos++;
					m_SSUI.CharacterPortraits[objpos].sprite = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]).m_TData.GetPortrait();
					m_SSUI.CharacterGrades[objpos].sprite = BaseValue.CharBG(USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]).m_Grade);
				}
				break;
		}
		if (STAGEINFO.m_StageModeType == StageModeType.Training)
			Anim.SetTrigger("Char0");
		else
			Anim.SetTrigger(string.Format("Char{0}", charcnt));

#if NOT_USE_NET
		StartCoroutine(Clear());
#else
		StartCoroutine(Clear((RES_STAGE_CLEAR)aobjValue[0]));
#endif
	}
	IEnumerator Clear()
	{
		PlayEffSound(SND_IDX.SFX_0298);
		//데이터
		TStageTable table = STAGEINFO.m_TStage;
		RES_REWARD_MONEY nrmoney;
		int Exp = table.m_ClearExp;
		int Money = table.m_ClearMoney;
		int Gold = table.m_ClearGold;

		if (Exp > 0) {
			Exp = (int)USERINFO.SetIngameExp(Exp, true);
			nrmoney = new RES_REWARD_MONEY();
			nrmoney.Type = Res_RewardType.Exp;
			nrmoney.Befor = USERINFO.m_Exp[0] - Exp;
			nrmoney.Now = USERINFO.m_Exp[0];
			nrmoney.Add = (int)(nrmoney.Now - nrmoney.Befor);
			m_GetRewards.Add(nrmoney);
		}
		if (Money > 0) {
			Money = (int)USERINFO.ChangeMoney(Money, true);
			nrmoney = new RES_REWARD_MONEY();
			nrmoney.Type = Res_RewardType.Money;
			nrmoney.Befor = USERINFO.m_Money - Money;
			nrmoney.Now = USERINFO.m_Money;
			nrmoney.Add = (int)(nrmoney.Now - nrmoney.Befor);
			m_GetRewards.Add(nrmoney);
		}
		if (Gold > 0) {
			USERINFO.GetCash(Gold);
			nrmoney = new RES_REWARD_MONEY();
			nrmoney.Type = Res_RewardType.Cash;
			nrmoney.Befor = USERINFO.m_Cash - Gold;
			nrmoney.Now = USERINFO.m_Cash;
			nrmoney.Add = (int)(nrmoney.Now - nrmoney.Befor);
			m_GetRewards.Add(nrmoney);
		}

		m_UserExp.AExp = table.m_ClearUserExp;
		m_UserExp.BExp = USERINFO.m_Exp[(int)EXPType.User];
		m_UserExp.BLV = USERINFO.m_LV;
		USERINFO.SetUserExp(m_UserExp.AExp);
		m_UserExp.NExp = USERINFO.m_Exp[(int)EXPType.User];
		m_UserExp.NLV = USERINFO.m_LV;

		//유아이
		if (STAGEINFO.m_StageModeType == StageModeType.Training)
			Anim.SetTrigger("Succ_Training_Start");
		else
			Anim.SetTrigger("Succ_Start");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(Anim));

		for (int i = 0; i < table.m_ClearReward.Count; i++) {
			switch (table.m_ClearReward[i].m_Kind)
			{
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
					TItemTable tdata = TDATA.GetItemTable(table.m_ClearReward[i].m_Idx);
					if (tdata.m_Type == ItemType.RandomBox || tdata.m_Type == ItemType.AllBox)
					{//박스는 바로 까서 주기
						List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
						TItemTable itemTable = TDATA.GetItemTable(table.m_ClearReward[i].m_Idx);
						for (int j = table.m_ClearReward[i].m_Count - 1; j > -1; j--) rewards.AddRange(TDATA.GetGachaItem(itemTable));
						for (int j = 0; j < rewards.Count; j++)
						{
							// 캐릭터 보상은 없음
							if (rewards[j].Type == Res_RewardType.Char) continue;
							m_GetRewards.Add(rewards[j]);
						}
					}
					else {
						ItemInfo iteminfo = USERINFO.InsertItem(table.m_ClearReward[i].m_Idx, table.m_ClearReward[i].m_Count);
						RES_REWARD_MONEY rmoney;
						RES_REWARD_ITEM ritem;
						switch (tdata.m_Type) {
							case ItemType.Exp:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Exp;
								rmoney.Befor = USERINFO.m_Exp[0] - table.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Exp[0];
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								m_GetRewards.Add(rmoney);
								break;
							case ItemType.Dollar:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Money;
								rmoney.Befor = USERINFO.m_Money - table.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Money;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								m_GetRewards.Add(rmoney);
								break;
							case ItemType.Cash:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Cash;
								rmoney.Befor = USERINFO.m_Cash - table.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Cash;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								m_GetRewards.Add(rmoney);
								break;
							case ItemType.Energy:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Energy;
								rmoney.Befor = USERINFO.m_Energy.Cnt - table.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Energy.Cnt;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								rmoney.STime = (long)USERINFO.m_Energy.STime;
								m_GetRewards.Add(rmoney);
								break;
							case ItemType.InvenPlus:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Inven;
								rmoney.Befor = USERINFO.m_InvenSize - table.m_ClearReward[i].m_Count;
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
			
		for(int i = 0;i< m_GetRewards.Count; i++) {
			AddReward(m_GetRewards[i]);
		}

		yield return RewardAction();
	}
	void AddReward(RES_REWARD_BASE _res, Action<GameObject> _cb = null) {
		Item_RewardList_Item reward = Utile_Class.Instantiate(m_SSUI.RewardCardPrefab, m_SSUI.ItemBucket).GetComponent<Item_RewardList_Item>();
		m_Rewards.Add(reward);
		reward.SetData(_res, _cb);
		reward.gameObject.SetActive(false);

		switch (_res.Type)
		{
		case Res_RewardType.Money:
		case Res_RewardType.Exp:
		case Res_RewardType.Cash:
			if (m_RewardPos.ContainsKey(_res.Type)) break;
			m_RewardPos.Add(_res.Type, (RectTransform)reward.transform);
			break;
		}
	}

	IEnumerator Clear(RES_STAGE_CLEAR res) {
		PlayEffSound(SND_IDX.SFX_0298);
		if (res.ClearPer == null) m_SFUI.UserPer.gameObject.SetActive(false);
		else
			m_SFUI.UserPer.SetData(res.ClearPer[0], res.ClearPer[1]);

		List<RES_REWARD_BASE> list = MAIN.GetRewardList(res.GetRewards());
		m_UserExp = (RES_REWARD_USEREXP)list.Find(o => o.Type == Res_RewardType.UserExp);
		if (m_UserExp == null) m_UserExp = new RES_REWARD_USEREXP();
		list.Remove(m_UserExp);
		//for (int i = res.Rewards.Count - 1; i > -1; i--)
		//{
		//	RES_REWARDS Base = res.Rewards[i];
		//	for (int j = Base.Rewards.Count - 1; j > -1 ; j--)
		//	{
		//		RES_REWARD reward = Base.Rewards[j];
		//		switch(reward.Type)
		//		{
		//			case Res_RewardType.UserExp:
		//				m_UserExp = (RES_REWARD_USEREXP)reward.Infos[0];
		//				break;
		//			default:
		//				list.AddRange(reward.Infos);
		//				break;
		//		}
		//	}
		//}

		//유아이
		if (STAGEINFO.m_StageModeType == StageModeType.Training)
			Anim.SetTrigger("Succ_Training_Start");
		else
			Anim.SetTrigger("Succ_Start");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(Anim));

		for (int i = 0; i < list.Count; i++) {
			if (list[i].result_code == EResultCode.SUCCESS_POST) m_IsInvenLowGoPost = true;
			AddReward(list[i]);
		}

		yield return RewardAction();
	}

	IEnumerator RewardAction()
	{
		if (m_IsInvenLowGoPost) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(535));
		bool ActiveRewardAction = m_RewardPos.Count > 0;

		RewardAssetAni pop = null;
		if (ActiveRewardAction)
			pop = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardAssetAni, (result, obj) => {
				ActiveRewardAction = false;
			}, m_RewardPos).GetComponent<RewardAssetAni>();

		//데이터
		TStageTable table = STAGEINFO.m_TStage;
		for (int i = 0; i < m_Rewards.Count; i++) {
			var item = m_Rewards[i];
			item.gameObject.SetActive(true);
			item.GetComponent<Animator>().SetTrigger("Start");
			if (pop != null)
			{
				// 위치가 변경되지 않도록 계산된 한프레임 뒤에 실행
				yield return new WaitForEndOfFrame();
				pop.StartAction(item.m_RewardType);
			}
			PlayEffSound(SND_IDX.SFX_0313);
			yield return new WaitForSeconds(0.2f);

			m_SSUI.Scroll.verticalNormalizedPosition = 0;
		}
		if (pop != null) pop.Dealay_Close(1.75f);
		//계정 경험치 받는 경우
		if (m_UserExp.AExp > 0) {
			yield return new WaitUntil(() => m_RewardEnd == true);
			m_RewardEnd = false;

			m_SUUI.Portrait.sprite = TDATA.GetUserProfileImage(USERINFO.m_Profile);
			m_SUUI.AddExp.text = string.Format("+ {0}", m_UserExp.AExp.ToString("N0"));
			m_SUUI.Exp.text = string.Format("{0} / {1}", 0, TDATA.GetExpTable(m_UserExp.NLV).m_UserExp.ToString("N0"));
			m_SUUI.Lv.text = m_SUUI.UpLv.text = string.Format("Lv. {0}", m_UserExp.NLV.ToString("N0"));

			Anim.SetTrigger("Succ_2_Exp");
			yield return new WaitForEndOfFrame();
			yield return new WaitUntil(() => !Utile_Class.IsAniPlay(Anim, 117f / 150f));
			float time = 1f;

			PlayEffSound(SND_IDX.SFX_1060);
			m_BLv = m_UserExp.BLV;
			m_BExp = m_UserExp.BExp;
			m_AExp = 0;
			long nowexpmax = TDATA.GetExpTable(m_BLv).m_UserExp;
			m_SUUI.Lv.text = string.Format("Lv. {0}", m_BLv.ToString("N0"));

			TExpTable bedata = TDATA.GetExpTable(m_BLv);
			if (USERINFO.m_Exp[0] >= bedata.m_UserExp) {
				m_SUUI.Exp.text = string.Format("{0} / {1}", bedata.m_UserExp, bedata.m_UserExp);
				m_SUUI.ExpGague.fillAmount = 1f;
			}
			else {
				m_SUUI.Exp.text = string.Format("{0} / {1}", m_BExp.ToString("N0"), nowexpmax.ToString("N0"));
				m_SUUI.ExpGague.fillAmount = Mathf.Clamp(m_BExp / (float)nowexpmax, 0f, 1f);
				iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", (float)m_UserExp.AExp, "onupdate", "TW_UserExp", "time", time, "easetype", "linear"));
			}

			yield return new WaitForSeconds(time + 0.2f);
			yield return new WaitUntil(() => !Utile_Class.IsAniPlay(Anim));
			//받은 경험치로 레벨업 하는 경우
			if (m_UserExp.BLV != m_UserExp.NLV) {
				Anim.SetTrigger("Succ_3_LvUp");
				yield return new WaitForEndOfFrame();
				yield return new WaitForSeconds(0.15f);
				TW_UserLvCounting(m_UserExp.NLV);

				yield return new WaitUntil(() => !Utile_Class.IsAniPlay(Anim));
			}
			else
				Anim.SetTrigger("Succ_2_Touch");
		}


		yield return new WaitUntil(() => m_RewardEnd == true);
		Anim.SetTrigger("Succ_End");
		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !Utile_Class.IsAniPlay(Anim));

		if (table.m_TalkDlg[1] == 0 || STAGEINFO.IS_ReplayStg) Close(0);
		else
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DL_Talk, (result, obj) => {
				Close(0);
			}, table.m_TalkDlg[1], table.GetName(), table.GetImg(true), table.GetImg(false), false, 2, table.m_Idx);
	}
	public void ClickActionEnd() {
		m_RewardEnd = true;
	}
	void TW_UserExp(float _amount) {
		float exp = _amount - m_AExp;
		float nowexp = m_BExp + exp;
		TExpTable tdata = TDATA.GetExpTable(m_BLv);
		if (tdata == null) return;
		long nowexpmax = tdata.m_UserExp;
		if (nowexp > nowexpmax) {
			nowexp = nowexpmax;
			m_AExp = (long)_amount;
			m_BExp = 0;
			if (m_BLv < BaseValue.CHAR_MAX_LV) {
				m_BLv++;
				TW_UserLvCounting(m_BLv);

				DelayPlayFXSND(1.5f, SND_IDX.SFX_0111);
			}
		}
		m_SUUI.Exp.text = string.Format("{0} / {1}", nowexp.ToString("N0"), nowexpmax.ToString("N0"));
		m_SUUI.ExpGague.fillAmount = Mathf.Clamp(nowexp / (float)nowexpmax, 0f, 1f);
	}
	void TW_UserLvCounting(float _amount) {
		m_SUUI.Lv.text = m_SUUI.UpLv.text = string.Format("Lv. {0}", _amount.ToString("N0"));
	}
}
