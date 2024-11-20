using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;

public class Item_Adventure_Detail : Item_PDA_Base
{

	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Desc;
		public Item_DifficultyGroup DiffGroup;
		public GameObject ItemPrefab;
		public Material ItemMaterial;
		public Transform Bucket;
		public Item_GradeGroup PartyGroup;
		public Item_Adv_CharCountGroup CountGroup;
		public Item_CharDeckCard[] CharDeck;
		public GameObject[] CharDeckPlus;
		public TextMeshProUGUI TimeTxt;
		public Button AutoBtn;
		public Button GoBtn;
	}

	[SerializeField]
	SUI m_SUI;
	TAdventureTable m_TData;
	Item_AdventrueList m_ADList;
	List<long> m_Chars = new List<long>();
	List<GameObject> m_Rewards = new List<GameObject>();

	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);

		m_ADList = (Item_AdventrueList)args[0];
		m_TData = m_ADList.m_TData;
		m_Chars = new List<long>();
		//탐험 이름, 설명
		m_SUI.Name.text = m_TData.GetName();
		m_SUI.Desc.text = m_TData.GetDesc();
		//등급
		m_SUI.DiffGroup.SetData(m_TData.m_AdventureGrade);
		//보상
		for (int i = m_Rewards.Count - 1; i > -1; i--) {
			GameObject obj = m_Rewards[i];
			m_Rewards.Remove(obj);
			Destroy(obj);
		}
		m_Rewards.Clear();
		for (int i = 0; i < m_TData.m_Reward.Count; i++) {
			Item_RewardItem_Card card = Utile_Class.Instantiate(m_SUI.ItemPrefab, m_SUI.Bucket).GetComponent<Item_RewardItem_Card>();
			card.SetMaterial(m_SUI.ItemMaterial);
			TAdventureTable.ADReward reward = m_TData.m_Reward[i];
			card.SetData(reward.m_Idx, reward.m_Cnt);
			m_Rewards.Add(card.gameObject);
		}
		//필요 파티 등급
		m_SUI.PartyGroup.SetData(m_TData.m_PartyGrade);
		for (int i = 0; i < m_SUI.CharDeck.Length; i++) {
			m_SUI.CharDeck[i].gameObject.SetActive(i < m_TData.m_PartyCount);
			m_SUI.CharDeck[i].SetData();
			m_SUI.CharDeckPlus[i].SetActive(true);
		}
		//인원수
		SetCharCnt();
		//기본 시간
		m_SUI.TimeTxt.text = string.Format(TDATA.GetString(226), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.day_hr_min_sec, m_TData.GetTime() * 0.001f));
		RefreshChar();
	}
   
	private void OnEnable() {
		RefreshChar();
	}
	/// <summary> 캐릭터 슬롯 갱신 </summary>
	public void RefreshChar() {
		if (m_TData == null) return;
		for (int i = 0; i < m_TData.m_PartyCount; i++) {
			if (m_Chars != null && m_Chars.Count > i) {
				m_SUI.CharDeck[i].SetData(USERINFO.GetChar(m_Chars[i]));
				m_SUI.CharDeckPlus[i].SetActive(false);
			}
			else {
				m_SUI.CharDeck[i].SetData();
				m_SUI.CharDeckPlus[i].SetActive(true);
			}
		}
	}
	void SetCharCnt() {
		//인원 표시 및 버튼 활성
		int gradecnt = 0;
		for (int i = 0; i < m_Chars.Count; i++) {
			if (USERINFO.GetChar(m_Chars[i]).m_Grade >= m_TData.m_PartyGrade) gradecnt++;
		}
		m_SUI.CountGroup.SetData(gradecnt, m_TData.m_PartyGradeCount);
		//m_SUI.GoBtn.interactable = m_Chars.Count == m_TData.m_PartyCount && gradecnt >= m_TData.m_PartyGradeCount ? true : false;
		//m_SUI.AutoBtn.interactable = m_Chars.Count == m_TData.m_PartyCount && gradecnt >= m_TData.m_PartyGradeCount ? false : true;
	}
	bool CheckDispatch() {
		int gradecnt = 0;
		for (int i = 0; i < m_Chars.Count; i++) {
			if (USERINFO.GetChar(m_Chars[i]).m_Grade >= m_TData.m_PartyGrade) gradecnt++;
		}
		return m_Chars.Count == m_TData.m_PartyCount && gradecnt >= m_TData.m_PartyGradeCount;
	}
	/// <summary> 탐험 가능 캐릭터들 </summary>
	public List<long> GetCanAddChar(TAdventureTable _table, List<long> _alreadychars) {
		//전체 캐릭터중 탐험
		List<long> canchars = new List<long>();
		List<CharInfo> charinfos = new List<CharInfo>();
		charinfos.AddRange(USERINFO.m_Chars);
		charinfos.Sort((CharInfo _a, CharInfo _b) => {//등급 내림차순 정렬
			if (_a.m_Grade < _b.m_Grade) return 1;
			else if (_a.m_Grade > _b.m_Grade) return -1;
			else return 0;
		});
		//조건에 맞는 캐릭터 체크
		int needcnt = 0;
		int partycount = 0;
		//필수 등급
		for (int i = 0; i < 2; i++) {//0:필수등급 인원, 1:남은 아무등급 인원
			if (i == 1 && needcnt != _table.m_PartyGradeCount) break;//필수 등급 인원이 안차있으면 멈춤
			for (int j = charinfos.Count - 1; j > -1; j--) {//등급 낮은것부터 뺴감
				if (!_alreadychars.Contains(charinfos[j].m_UID)) {//탐험 안나간 캐릭터
					if (i == 0 && charinfos[j].m_Grade < _table.m_PartyGrade) continue;//필수 등급 인원 체크

					canchars.Add(charinfos[j].m_UID);
					charinfos.Remove(charinfos[j]);
					partycount++;
					if (i == 0) {//필수등급
						needcnt++;
						if (needcnt == _table.m_PartyGradeCount) break;
					}
					if (partycount == _table.m_PartyCount) break;
				}
			}
			if (partycount == _table.m_PartyCount) break;
		}
		return canchars;
	}
	/// <summary> 슬롯 클릭시 덱세팅창 켜줌 </summary>
	public void ClickSlot() {
		PlayEffSound(SND_IDX.SFX_0121);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DeckSetting_Adventure, (result, obj) => {
			m_Chars = new List<long>(obj.GetComponent<DeckSetting_Adventure>().m_Chars.Values);
			RefreshChar();
			SetCharCnt();
		}, m_TData, m_Chars);
	}
	/// <summary> 덱 자동 세팅 </summary>
	public void ClickAutoDeck() {
		PlayEffSound(SND_IDX.SFX_0121);
		m_Chars = new List<long>();
		m_Chars = GetCanAddChar(m_TData, USERINFO.GetAdventureChars());

		if (!CheckDispatch()) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(231));
		}
		RefreshChar();
		SetCharCnt();
	}
	/// <summary> 파견 버튼 </summary>
	public void ClickDispatch() {
		if (!CheckDispatch()) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(873));
			return;
		}
		PlayEffSound(SND_IDX.SFX_0122);

#if NOT_USE_NET
		m_ADList.m_Info.SetPlay(m_Chars);
		MAIN.Save_UserInfo();
		USERINFO.Check_Mission(MissionType.ADV, 0, 0, 1);
		OnClose();
#else
		WEB.SEND_REQ_ADV_START((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				return;
			}
			OnClose();
		}, new List<REQ_ADV_START_INFO>() { new REQ_ADV_START_INFO() { UID = m_ADList.m_Info.m_UID, CUIDS = m_Chars } });
#endif
	}

	/// <summary> 돌아가기 </summary>
	public void ClickExit() {
		PlayEffSound(SND_IDX.SFX_0121);
		OnClose();
	}
	public override void OnClose() {
		m_CloaseCB?.Invoke(Item_PDA_Adventure.State.Main, null);
	}
}
