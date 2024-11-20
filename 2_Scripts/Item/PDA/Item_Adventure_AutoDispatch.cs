using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using static LS_Web;

public class Item_Adventure_AutoDispatch : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public GameObject NotListTxt;
		public Transform Bucket;
		public GameObject ListPrefab;
		public ScrollRect Scroll;
		public Button GoBtn;
	}
	[SerializeField]
	SUI m_SUI;
	List<Item_AdventureList_Auto> m_Lists = new List<Item_AdventureList_Auto>();

	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);
		m_Lists.Clear();
		List<Item_AdventrueList> canlist = (List<Item_AdventrueList>)args[0];
		m_Lists.Clear();
		UTILE.Load_Prefab_List(canlist.Count, m_SUI.Bucket, m_SUI.ListPrefab.transform);

		List<long> playchars = USERINFO.GetAdventureChars();
		var chars = USERINFO.m_Chars.FindAll(o => !playchars.Any(x => x == o.m_UID));
		chars.Sort((CharInfo befor, CharInfo after) => after.m_TData.m_Grade.CompareTo(befor.m_TData.m_Grade));

		for (int i = canlist.Count - 1; i > -1; i--)
		{
			List<long> targets = GetCanAddChar(canlist[i].m_TData, chars);
			if (targets == null)
			{
				m_SUI.Bucket.GetChild(i).gameObject.SetActive(false);
				continue;
			}
			Item_AdventureList_Auto item = m_SUI.Bucket.GetChild(i).GetComponent<Item_AdventureList_Auto>();
			item.SetData(canlist[i].m_Info, targets);
			chars.RemoveAll(o => targets.Any(x => x == o.m_UID));
			m_Lists.Add(item);
		}

		m_SUI.Scroll.verticalNormalizedPosition = 1f;

		m_SUI.GoBtn.interactable = m_Lists.Count > 0;
		m_SUI.NotListTxt.SetActive(m_Lists.Count < 1);
	}
	/// <summary> 탐험 가능 캐릭터들 </summary>
	/// <param name="charinfos">탐험을 하지 않고있는 케릭터들</param>
	public List<long> GetCanAddChar(TAdventureTable _table, List<CharInfo> charinfos) {
		//전체 캐릭터중 탐험
		List<long> canchars = new List<long>();

		//조건에 맞는 캐릭터 체크
		int needcnt = 0;
		int partycount = 0;
		//필수 등급
		for (int i = 0; i < 2; i++) {
			//0:필수등급 인원, 1:남은 아무등급 인원
			if (i == 1 && needcnt != _table.m_PartyGradeCount) break;//필수 등급 인원이 안차있으면 멈춤
			for (int j = charinfos.Count - 1; j > -1; j--) {//등급 낮은것부터 뺴감

				if (i == 0 && charinfos[j].m_Grade < _table.m_PartyGrade) continue;//필수 등급 인원 체크

				canchars.Add(charinfos[j].m_UID);
				charinfos.Remove(charinfos[j]);
				partycount++;
				if (i == 0)
				{//필수등급
					needcnt++;
					if (needcnt == _table.m_PartyGradeCount) break;
				}
				if (partycount == _table.m_PartyCount) break;
			}
			if (partycount == _table.m_PartyCount) break;
		}
		if (canchars.Count == _table.m_PartyCount) return canchars;
		else return null;
	}
	/// <summary> 돌아가기 </summary>
	public void ClickExit() {
		PlayEffSound(SND_IDX.SFX_0121);
		OnClose();
	}
	/// <summary> 파견 버튼 </summary>
	public void ClickDispatch() {
		PlayEffSound(SND_IDX.SFX_0122);
#if NOT_USE_NET
		for (int i = 0; i < m_Lists.Count; i++) {
			if (m_Lists[i].m_IsConfirm) m_Lists[i].m_Info.SetPlay(m_Lists[i].m_Chars);
		}
		USERINFO.Check_Mission(MissionType.ADV, 0, 0, m_Lists.Count);
		//m_Lists.Select(o => { if (o.m_IsConfirm)  o.m_Info.SetPlay(o.m_Chars); return o; } );
		OnClose();
#else
		var list = m_Lists.FindAll(o => o.m_IsConfirm ).Select(o => { return new REQ_ADV_START_INFO() { UID = o.m_Info.m_UID, CUIDS = o.m_Chars }; } ).ToList();

		WEB.SEND_REQ_ADV_START((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				return;
			}
			OnClose();
		}, list);
#endif
	}
	public override void OnClose()
	{
		var auto = GetComponentsInChildren<Item_AdventureList_Auto>();
		foreach (var itemAdventureListAuto in auto)
		{
			itemAdventureListAuto.ClearBucket();
		}
		
		m_CloaseCB?.Invoke(Item_PDA_Adventure.State.Main, null);
	}
}
