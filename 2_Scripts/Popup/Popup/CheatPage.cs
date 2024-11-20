using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class CheatPage : PopupBase
{
	[System.Serializable]
	public struct DNA
	{
		public int Idx;
		public int Grade;
	}
	[Serializable]
	public struct SUI
	{
		public TMP_InputField[] Input;
	}
	[SerializeField] SUI m_SUI;

	////////////
#pragma warning disable 0414
	[SerializeField, ReName("테스트 LL 캐릭터 인덱스")] int m_TestStageChar_LL = 1001;
	[SerializeField, ReName("테스트 LL 캐릭터 레벨")] int m_TestStageChar_LL_Lv = 1;
	[SerializeField, ReName("테스트 LL 캐릭터 랭크")] int m_TestStageChar_LL_Rank = 1;
	[SerializeField, ReName("테스트 LL 모든 장비 인덱스")] int[] m_TestStageChar_LL_EquipIdx = new int[5];
	[SerializeField, ReName("테스트 LL 모든 장비 등급")] int m_TestStageChar_LL_EquipGrade = 1;
	[SerializeField, ReName("테스트 LL 모든 장비 레벨")] int m_TestStageChar_LL_EquipLv = 1;
	[SerializeField, ReName("테스트 LL DNA 인덱스")] DNA[] m_TestStageChar_LL_DNA = new DNA[5];
	[SerializeField, ReName("테스트 L 캐릭터 인덱스")] int m_TestStageChar_L = 1004;
	[SerializeField, ReName("테스트 L 캐릭터 레벨")] int m_TestStageChar_L_Lv = 1;
	[SerializeField, ReName("테스트 L 캐릭터 랭크")] int m_TestStageChar_L_Rank = 1;
	[SerializeField, ReName("테스트 L 모든 장비 인덱스")] int[] m_TestStageChar_L_EquipIdx = new int[5];
	[SerializeField, ReName("테스트 L 모든 장비 등급")] int m_TestStageChar_L_EquipGrade = 1;
	[SerializeField, ReName("테스트 L 모든 장비 레벨")] int m_TestStageChar_L_EquipLv = 1;
	[SerializeField, ReName("테스트 L DNA 인덱스")] DNA[] m_TestStageChar_L_DNA = new DNA[5];
	[SerializeField, ReName("테스트 C 캐릭터 인덱스")] int m_TestStageChar_C = 1005;
	[SerializeField, ReName("테스트 C 캐릭터 레벨")] int m_TestStageChar_C_Lv = 1;
	[SerializeField, ReName("테스트 C 캐릭터 랭크")] int m_TestStageChar_C_Rank = 1;
	[SerializeField, ReName("테스트 C 모든 장비 인덱스")] int[] m_TestStageChar_C_EquipIdx = new int[5];
	[SerializeField, ReName("테스트 C 모든 장비 등급")] int m_TestStageChar_C_EquipGrade = 1;
	[SerializeField, ReName("테스트 C 모든 장비 레벨")] int m_TestStageChar_C_EquipLv = 1;
	[SerializeField, ReName("테스트 C DNA 인덱스")] DNA[] m_TestStageChar_C_DNA = new DNA[5];
	[SerializeField, ReName("테스트 R 캐릭터 인덱스")] int m_TestStageChar_R = 1006;
	[SerializeField, ReName("테스트 R 캐릭터 레벨")] int m_TestStageChar_R_Lv = 1;
	[SerializeField, ReName("테스트 R 캐릭터 랭크")] int m_TestStageChar_R_Rank = 1;
	[SerializeField, ReName("테스트 R 모든 장비 인덱스")] int[] m_TestStageChar_R_EquipIdx = new int[5];
	[SerializeField, ReName("테스트 R 모든 장비 등급")] int m_TestStageChar_R_EquipGrade = 1;
	[SerializeField, ReName("테스트 R 모든 장비 레벨")] int m_TestStageChar_R_EquipLv = 1;
	[SerializeField, ReName("테스트 R DNA 인덱스")] DNA[] m_TestStageChar_R_DNA = new DNA[5];
	[SerializeField, ReName("테스트 RR 캐릭터 인덱스")] int m_TestStageChar_RR = 1007;
	[SerializeField, ReName("테스트 RR 캐릭터 레벨")] int m_TestStageChar_RR_Lv = 1;
	[SerializeField, ReName("테스트 RR 캐릭터 랭크")] int m_TestStageChar_RR_Rank = 1;
	[SerializeField, ReName("테스트 RR 모든 장비 인덱스")] int[] m_TestStageChar_RR_EquipIdx = new int[5];
	[SerializeField, ReName("테스트 RR 모든 장비 등급")] int m_TestStageChar_RR_EquipGrade = 1;
	[SerializeField, ReName("테스트 RR 모든 장비 레벨")] int m_TestStageChar_RR_EquipLv = 1;
	[SerializeField, ReName("테스트 RR DNA 인덱스")] DNA[] m_TestStageChar_RR_DNA = new DNA[5];
#pragma warning restore 0414
	///////////
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_SUI.Input[0].text = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx.ToString();
		m_SUI.Input[3].text = USERINFO.GetDifficulty().ToString();
		m_SUI.Input[2].text = USERINFO.m_LV.ToString();
	}
	public void ClickStgIdxApply() {
		int idx = int.Parse(m_SUI.Input[0].text);
		if(TDATA.GetStageTable(idx, int.Parse(m_SUI.Input[3].text)) == null) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, "없는 스테이지 입니다");
			return;
		}
		USERINFO.m_Stage[StageContentType.Stage].Idxs[int.Parse(m_SUI.Input[3].text)].Idx = idx;
		USERINFO.m_Stage[StageContentType.Stage].Idxs[int.Parse(m_SUI.Input[3].text)].PlayCount = 1;
		PlayerPrefs.SetInt($"StageDifficulty_{USERINFO.m_UID}", int.Parse(m_SUI.Input[3].text));
	}
	public void Stage_Making_AddMaterial() {
		if(POPUP.GetMainUI() != null && POPUP.GetMainUI().m_Popup == PopupName.Stage)
			for (StageMaterialType i = StageMaterialType.Bullet; i < StageMaterialType.None; i++) STAGE.AddMaterial(i, 999);
	}
	public void Content_Count_Reset() {
		USERINFO.OutStageCntInit();
	}
	public void Char_All_Get() {
		POPUP.Set_MsgBox(PopupName.Msg_YN, string.Empty, "해당 기능은 되돌리려면 앱을 재설치 해야 합니다.", (result, obj) => {
			if (result == 1)
				for (int i = 1001; i < 1057; i++) {
					//if(i == 1002 || i == 1003 || i == 1008 || i == 1009 || i == 1013 || i == 1014 || i == 1015 || i == 1019 || i == 1021 || i == 1022 || i == 1026
					//    || i == 1034 || i == 1036 || i == 1037 || i == 1040 || i == 1046 || i == 1047 || i == 1050 || i == 1051) continue;
					if (TDATA.GetCharacterTable(i) != null)
						USERINFO.InsertChar(i);
				}
		});
	}
	public void Cash100M() {
		USERINFO.GetCash(Math.Max(0, 1000000L - USERINFO.m_Cash));
		USERINFO.ChangeMoney(Math.Max(0, 10000000 - (int)USERINFO.m_Money));
		USERINFO.SetIngameExp(Math.Max(0, 10000000 - (int)USERINFO.m_Exp[(int)EXPType.Ingame]));
		USERINFO.GetShell(Math.Max(0, 1000 - (int)USERINFO.m_Energy.Cnt));
	}

	public void InsertEtc() {

		List<TItemTable> items = TDATA.GetAllItemIdxs();
		for (int i = items.Count - 1; i > -1; i--)
		{
			TItemTable item = items[i];
			if (item.GetInvenGroupType() == ItemInvenGroupType.Equipment) continue;
			switch(item.m_Type)
			{
				case ItemType.Exp: 
				case ItemType.RandomBox:
				case ItemType.AllBox:
				case ItemType.Cash:
				case ItemType.Energy: 
				case ItemType.InvenPlus:
					continue;
			}
			USERINFO.InsertItem(items[i].m_Idx, BaseValue.ITEM_MAXCNT);
		}
	}
	public void InsertEQ()
	{
		List<TItemTable> items = TDATA.GetAllItemIdxs();
		//for (int i = 0; i < 10; i++) {
		//	for (int j = 0; j < 5; j++) {
		//		USERINFO.InsertItem(10010 + 1000 * i, 1);
		//		USERINFO.InsertItem(10011 + 1000 * i, 1);
		//		USERINFO.InsertItem(10012 + 1000 * i, 1);
		//		USERINFO.InsertItem(10013 + 1000 * i, 1);
		//		USERINFO.InsertItem(10014 + 1000 * i, 1);
		//	}
		//}
		for (int i = items.Count - 1; i > -1; i--) {
			if (items[i].GetInvenGroupType() != ItemInvenGroupType.Equipment) continue;
			//일부장비만 받아볼때 해제
			//if (USERINFO.m_Items.Find((t) => t.m_TData.GetEquipType() == items[i].GetEquipType() && t.m_TData.m_Grade == items[i].m_Grade) != null) continue;
			USERINFO.InsertItem(items[i].m_Idx, 1);
		}
	}

	public void Content_All_Unlock() {
		POPUP.Set_MsgBox(PopupName.Msg_YN, string.Empty, "해당 기능은 되돌리려면 계정정보 초기화를 해야 합니다.", (result, obj) => {
			if (result == 1)
				PlayerPrefs.SetInt($"ContentUnlock_{USERINFO.m_UID}", 1);
		});
	}


	public void Stage_PlayCount_Reset() {
		USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].PlayCount = 1;
	}

	public void DungeonInit()
	{
		for(StageContentType mode = StageContentType.Bank; mode < StageContentType.End; mode++)
		{
			USERINFO.m_Stage[mode].PlayLimit[0] = USERINFO.m_Stage[mode].GetItemMax(false);
			USERINFO.m_Stage[mode].PlayLimit[1] = USERINFO.m_Stage[mode].GetItemMax(true);
		}
	}

	public void InfoReset() {
		POPUP.Set_MsgBox(PopupName.Msg_YN, string.Empty, "계정 정보를 초기화 하면 되돌릴 수 없습니다.", (result, obj) => {
			if (result == 1) {
				PlayerPrefs.DeleteAll();
				MAIN.m_UserInfo = new UserInfo();
				MAIN.Save_UserInfo();
				MAIN.ReStart();
			}
		});
	}

	public void ShowPostList()
	{
		Close();
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PostList);
	}
	
	public void ClickAllEquipLv() {
		int lv = int.Parse(m_SUI.Input[1].text);
		for(int i = 0; i < USERINFO.m_Chars.Count; i++) {
			for(int j = 0; j < USERINFO.m_Chars[i].m_EquipUID.Length; j++) {
				if (USERINFO.GetItem(USERINFO.m_Chars[i].m_EquipUID[j]) == null) continue;
				USERINFO.GetItem(USERINFO.m_Chars[i].m_EquipUID[j]).m_Lv = lv;
			}
		}
	}

	public void SetUserLV()
	{
		int LV = Math.Min(int.Parse(m_SUI.Input[2].text), BaseValue.CHAR_MAX_LV);
		USERINFO.m_LV = LV;
		m_SUI.Input[2].text = USERINFO.m_LV.ToString();
		MAIN.Save_UserInfo();
	}

	[ContextMenu("TEST")]
	void TEST() {
		USERINFO.GetShell(-40);// m_Energy.Cnt = 0;
	}

	public void InsertZombieAndDNA()
	{
		var datas = TDATA.GetAllDnaTable();
		for (int i = 0; i < datas.Count; i++) {
			DNAInfo dnaInfo = new DNAInfo(datas[i].m_Idx, UTILE.Get_Random(1,7));
			USERINFO.m_DNAs.Add(dnaInfo);
		}

		USERINFO.InsertZombie(1);
		USERINFO.InsertZombie(9);
		USERINFO.InsertZombie(33);
		
		MAIN.Save_UserInfo();
	}

	public void ViewChallenge()
	{
		StartCoroutine(StartChallengePopup());
	}

	IEnumerator StartChallengePopup()
	{
		Sprite blurimg = null;
		//yield return UTILE.GetCaptureBlurSprite((img) => { blurimg = img; }, 15);
		yield return UTILE.GetCaptureResizeSprite((img) => { blurimg = img; }, 0.025f);

		WEB.SEND_REQ_CHALLENGEINFO_ALL((res) => {
			// 챌린지 변경 알림
			MyChallenge info = USERINFO.m_MyChallenge;
			if (info.Now != null && info.Now.No > 0) POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_Main, null, blurimg);
			else POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_Next, null, info.Next, info.NextSTime);
			Close();
		});
	}
}
