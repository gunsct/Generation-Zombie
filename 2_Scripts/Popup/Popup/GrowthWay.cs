using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[System.Serializable] public class DicGrowthWayAlarm : SerializableDictionary<GrowthWayType, GameObject> { }

public class GrowthWay : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public DicGrowthWayAlarm Alarms;
		public GameObject[] Locks;      //0:긴급임무 1:생존자성장 2:다운타운 3:상점
		public TextMeshProUGUI[] LockTxts;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;
	int m_CharIdx;

	private void Awake()
	{
		for (GrowthWayType i = (GrowthWayType)0; i < GrowthWayType.End; i++)
		{
			if (m_SUI.Alarms.ContainsKey(i)) continue;
			m_SUI.Alarms[i].SetActive(false);
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		for (GrowthWayType i = GrowthWayType.CharacterUp; i < GrowthWayType.End; i++) {
			if (!m_SUI.Alarms.ContainsKey(i)) continue;
			bool unlock = false;
			switch (i) {
				case GrowthWayType.CharacterUp: unlock = USERINFO.CheckContentUnLock(ContentType.Character); break;
				case GrowthWayType.GetMaterial: unlock = USERINFO.CheckContentUnLock(ContentType.Factory); break;
				case GrowthWayType.Shop: unlock = USERINFO.CheckContentUnLock(ContentType.Store); break;
				case GrowthWayType.Replay: unlock = USERINFO.CheckContentUnLock(ContentType.Replay); break;
			}
			m_SUI.Alarms[i].SetActive(CheckSolution(i) && unlock);
		}
		m_SUI.Locks[0].SetActive(!USERINFO.CheckContentUnLock(ContentType.Character));
		m_SUI.LockTxts[0].text = string.Format("[{0}-{1}]", BaseValue.CONTENT_OPEN_IDX(ContentType.Character) / 100, BaseValue.CONTENT_OPEN_IDX(ContentType.Character) % 100);
		m_SUI.Locks[1].SetActive(!USERINFO.CheckContentUnLock(ContentType.Factory));
		m_SUI.LockTxts[1].text = string.Format("[{0}-{1}]", BaseValue.CONTENT_OPEN_IDX(ContentType.Factory) / 100, BaseValue.CONTENT_OPEN_IDX(ContentType.Factory) % 100);
		m_SUI.Locks[2].SetActive(!USERINFO.CheckContentUnLock(ContentType.Store));
		m_SUI.LockTxts[2].text = string.Format("[{0}-{1}]", BaseValue.CONTENT_OPEN_IDX(ContentType.Store) / 100, BaseValue.CONTENT_OPEN_IDX(ContentType.Store) % 100);
		m_SUI.Locks[3].SetActive(!USERINFO.CheckContentUnLock(ContentType.Replay));
		m_SUI.LockTxts[3].text = string.Format("[{0}-{1}]", BaseValue.CONTENT_OPEN_IDX(ContentType.Replay) / 100, BaseValue.CONTENT_OPEN_IDX(ContentType.Replay) % 100);
	}

	[EnumAction(typeof(GrowthWayType))]
	public void ClickSolution(int _type) {
		if (m_Action != null) return;
		MAIN.GoGrowthWay((GrowthWayType)_type, m_CharIdx);
		//switch ((GrowthWayType)_type) {
		//	/// <summary> 캐릭터 레벨 업 가능 시, 캐릭터 장비 레벨 업 가능 시, 캐릭터 승급 가능 시 </summary>
		//	case GrowthWayType.CharacterUp: GoCharInfo(); break;
		//	/// <summary> 현재 플레이 할 수 있는 다운타운 스테이지가 있을 경우 </summary>
		//	case GrowthWayType.GetMaterial: GoDownTown(); break;
		//	/// <summary> 상점은 알람에서 제외 </summary>
		//	case GrowthWayType.Shop: GoShop(); break;
		//}
	}

	//[ContextMenu("GoCharInfo")]
	//void GoCharInfo() {
	//	((Main_Play)POPUP.GetMainUI()).ClickMenuButton((int)MainMenuType.Character);
	//	if (((Main_Play)POPUP.GetMainUI()).m_State == MainMenuType.Character) {
	//		if (m_CharIdx > 0) {
	//			Item_CharManageCard card = ((Main_Play)POPUP.GetMainUI()).GetSrvMng.GetCharCard(m_CharIdx);
	//			card.OpenDetailStrSol();
	//		}
	//		POPUP.Init_PopupUI();
	//	}
	//}
	//[ContextMenu("GoDownTown")]
	//void GoDownTown() {
	//	((Main_Play)POPUP.GetMainUI()).ClickMenuButton((int)MainMenuType.Dungeon);
	//	if (((Main_Play)POPUP.GetMainUI()).m_State == MainMenuType.Dungeon) POPUP.Init_PopupUI();
	//}
	//[ContextMenu("GoShop")]
	//void GoShop() {
	//	((Main_Play)POPUP.GetMainUI()).ClickMenuButton((int)MainMenuType.Shop);
	//	if (((Main_Play)POPUP.GetMainUI()).m_State == MainMenuType.Shop) POPUP.Init_PopupUI();
	//}
	/// <summary> 방법 표기 여부 체크 </summary>
	bool CheckSolution(GrowthWayType _type) {
		switch (_type) {
			/// <summary> 캐릭터 레벨 업 가능 시, 캐릭터 장비 레벨 업 가능 시, 캐릭터 승급 가능 시 </summary>
			case GrowthWayType.CharacterUp: 
				for(int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
					CharInfo info = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]);
					bool cansol = false;
					if (info.IS_CanLvUP()) cansol = true;
					else if (info.IS_CanRankUP()) cansol = true;
					else if (info.IS_CanEquipLvUP()) cansol = true;

					if (cansol) {
						m_CharIdx = info.m_Idx;
						return true;
					}
				}
				if (USERINFO.m_PlayDeck.GetDeckCharCnt() > 0) m_CharIdx = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[0]).m_Idx;
				return false;
			/// <summary> 현재 플레이 할 수 있는 다운타운 스테이지가 있을 경우 </summary>
			case GrowthWayType.GetMaterial:
				//다운타운들 도전 횟수 남아있고 오픈되있다
				if (USERINFO.m_Stage[StageContentType.Bank].IS_CanGoStage() && USERINFO.CheckContentUnLock(ContentType.Bank)) return true;
				//else if (USERINFO.m_Stage[StageContentType.Academy].IS_CanGoStage() && USERINFO.CheckContentUnLock(ContentType.Academy)) return true;
				else if (USERINFO.m_Stage[StageContentType.University].IS_CanGoStage() && USERINFO.CheckContentUnLock(ContentType.University)) return true;
				else if (USERINFO.m_Stage[StageContentType.Tower].IS_CanGoStage() && USERINFO.CheckContentUnLock(ContentType.Tower)) return true;
				else if (USERINFO.m_Stage[StageContentType.Cemetery].IS_CanGoStage() && USERINFO.CheckContentUnLock(ContentType.Cemetery)) return true;
				else if (USERINFO.m_Stage[StageContentType.Factory].IS_CanGoStage() && USERINFO.CheckContentUnLock(ContentType.Factory)) return true;
				else if (USERINFO.m_Stage[StageContentType.Subway].IS_CanGoStage() && USERINFO.CheckContentUnLock(ContentType.Subway)) return true;
				return false;
			/// <summary> 상점은 알람에서 제외 </summary>
			default: return true;
		}
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
