using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipChange : PopupBase
{
#pragma warning disable 0649

	[System.Serializable]
	public struct SNotEQUI
	{
		public Image Icon;
		public TextMeshProUGUI Name;
	}

	[System.Serializable]
	public struct SItemUI
	{
		public GameObject Active;
		public Item_Item_Card Item;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI CP;
	}

	[System.Serializable]
	public struct SListUI
	{
		public ScrollRect Scroll;
		public GameObject Prefab;
	}

	[System.Serializable]
	public struct SUI
	{
		public SNotEQUI None;
		public SItemUI Eq;
		public SListUI List;
		public Item_SortingGroup SortingGroup;
		public GameObject Empty;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] Animator m_Ani;
	List<Item_EqChange> m_Items = new List<Item_EqChange>();
	ItemInfo m_Info;
	CharInfo m_Char;
	EquipType m_EQType;

	IEnumerator m_Action;
#pragma warning restore 0649
	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		if (m_Ani == null)
		{
			base.Close(Result);
			return;
		}
		m_Action = StartEndAni(Result);
		StartCoroutine(m_Action);
	}

	IEnumerator StartEndAni(int Result)
	{
		m_Ani.SetTrigger("Close");
		yield return Utile_Class.CheckAniPlay(m_Ani);

		base.Close(Result);
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_EQType = (EquipType)aobjValue[0];
		if(aobjValue[1] != null) m_Info = (ItemInfo)aobjValue[1];
		m_Char = (CharInfo)aobjValue[2];
		m_SUI.SortingGroup.SetData(SetSort);
		base.SetData(pos, popup, cb, aobjValue);
		StartCoroutine(StartAniChekc());
	}

	IEnumerator StartAniChekc()
	{
		yield return Utile_Class.CheckAniPlay(m_Ani);
	}

	public override void SetUI()
	{
		base.SetUI();
		// 장비 정보
		SetItemInfo();

		SetItemList();
	}

	void SetItemInfo()
	{
		if(m_Info != null)
		{
			m_SUI.None.Icon.gameObject.SetActive(false);
			m_SUI.Eq.Active.SetActive(true);
			m_SUI.Eq.Item.SetData(m_Info);
			m_SUI.Eq.CP.text = Utile_Class.CommaValue(m_Info.m_CP);
			m_SUI.Eq.Name.text = m_Info.m_TData.GetName();
		}
		else
		{
			m_SUI.None.Icon.gameObject.SetActive(true);
			m_SUI.Eq.Active.SetActive(false);

			m_SUI.None.Icon.sprite = UTILE.LoadImg(string.Format("UI/Icon/Char_Equipslot_{0}", (int)m_EQType), "png");
			m_SUI.None.Name.text = TDATA.GetEquipTypeName(m_EQType);
		}
	}

	void SetItemList() {
		m_Items.Clear();
		// 장비 리스트 얻어내기
		// 착용 한놈이 있어도 갈아 끼워주므로 검색 대상에 넣어준다.
		List<ItemInfo> infos = new List<ItemInfo>(
			USERINFO.m_Items.FindAll(
			o => {
				if (m_Info != null && o.m_Uid == m_Info.m_Uid) return false;
				if (o.m_TData.GetEquipType() != m_EQType) return false;
				return o.m_TData.m_Value == 0 || o.m_TData.m_Value == m_Char.m_Idx;
				//return o.m_TData.GetEquipType() == m_EQType;
			}) );
		//for (int i = 0; i < infos.Count; i++) infos[i].GetCombatPower();
		infos.Sort((befor, after) => {
			// 일단 전투력순으로만 소팅
			return after.m_CP.CompareTo(befor.m_CP);
		});

		UTILE.Load_Prefab_List(infos.Count, m_SUI.List.Scroll.content, m_SUI.List.Prefab.transform);


		for (int i = 0; i < infos.Count; i++)
		{
			Item_EqChange item = m_SUI.List.Scroll.content.GetChild(i).GetComponent<Item_EqChange>();
			item.SetData(infos[i], ItemChange);
			m_Items.Add(item);
		}
		m_SUI.Empty.SetActive(m_Items.Count < 1);

		//정렬
		SetSort();
	}

	void SetSort() {
		//for (int i = 0; i < m_Items.Count; i++) m_Items[i].m_Info.GetCombatPower();
		switch (m_SUI.SortingGroup.m_Condition) {
			case SortingType.Grade:
				m_Items.Sort((befor, after) => {
					// 등급이 높은순
					int bGrade = befor.m_Info.m_Grade;
					int aGrade = after.m_Info.m_Grade;
					if (aGrade != bGrade) return aGrade.CompareTo(bGrade);
					// 레벨이 높은순
					if (after.m_Info.m_Lv != befor.m_Info.m_Lv) return after.m_Info.m_Lv.CompareTo(befor.m_Info.m_Lv);
					// 전투력 순
					int beforcp = befor.m_Info.m_CP;
					int aftercp = after.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					return befor.m_Info.m_Idx.CompareTo(after.m_Info.m_Idx);
				});
				break;
			case SortingType.CombatPower:
				m_Items.Sort((befor, after) => {
					// 전투력 순
					int beforcp = befor.m_Info.m_CP;
					int aftercp = after.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					// 등급이 높은순
					int bGrade = befor.m_Info.m_Grade;
					int aGrade = after.m_Info.m_Grade;
					if (aGrade != bGrade) return aGrade.CompareTo(bGrade);
					// 레벨이 높은순
					if (after.m_Info.m_Lv != befor.m_Info.m_Lv) return after.m_Info.m_Lv.CompareTo(befor.m_Info.m_Lv);
					return befor.m_Info.m_Idx.CompareTo(after.m_Info.m_Idx);
				});
				break;
			case SortingType.Level:
				m_Items.Sort((befor, after) => {
					// 레벨이 높은순
					if (after.m_Info.m_Lv != befor.m_Info.m_Lv) return after.m_Info.m_Lv.CompareTo(befor.m_Info.m_Lv);
					// 등급이 높은순
					int bGrade = befor.m_Info.m_Grade;
					int aGrade = after.m_Info.m_Grade;
					if (aGrade != bGrade) return aGrade.CompareTo(bGrade);
					// 전투력 순
					int beforcp = befor.m_Info.m_CP;
					int aftercp = after.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					return befor.m_Info.m_Idx.CompareTo(after.m_Info.m_Idx);
				});
				break;
		}

		if (m_SUI.SortingGroup.m_Ascending) m_Items.Reverse();

		for (int i = 0; i < m_Items.Count; i++) {
			m_Items[i].transform.SetAsLastSibling();
		}
	}
	
	void ItemChange(Item_EqChange item, int state)
	{

		if (TUTO.TouchCheckLock(TutoTouchCheckType.EquipChange, item, state)) return;
		// 장착 변경해주고
		if (state == 0)
		{
			ItemInfo changeitem = item.m_Info;
#if NOT_USE_NET
			if(item.m_Char != null)
			{
				// 장착하고있던 캐릭터에서 장착 해제해준다.
				USERINFO.RemoveEquipUID(item.m_Info);
			}

			if(m_Char.m_EquipUID[(int)changeitem.m_TData.GetEquipType()] != 0)
			{
				USERINFO.RemoveEquipUID(m_Char.m_EquipUID[(int)changeitem.m_TData.GetEquipType()]);
			}
			m_Char.m_EquipUID[(int)changeitem.m_TData.GetEquipType()] = changeitem.m_Uid;
			m_Char.IS_SetEquip();
			USERINFO.AddEquipUID(changeitem.m_Uid);
			MAIN.Save_UserInfo();
			PlayEffSound(SND_IDX.SFX_0004);
			Close(1);
#else
			WEB.SEND_REQ_CHAR_EQUIP((res) =>
			{
				if (!res.IsSuccess())
				{
					WEB.StartErrorMsg(res.result_code, (btn, obj) => {
						WEB.SEND_REQ_ALL_INFO((res2) => { SetUI(); });
					});
					return;
				}
				Close(1);
			}, m_Char.m_UID, new List<long>() { changeitem.m_Uid });
#endif
		}
	}
	public void Click_Make() {
		Main_Play play = POPUP.GetMainUI().GetComponent<Main_Play>();
		Item_PDA_Menu pda = play.GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
		play.MenuChange((int)MainMenuType.PDA, false);
		pda.ClickMenu(3);
		MAIN.GoPDAMaking(pda, 0);
		POPUP.Init_PopupUI();
	}

	public GameObject GetEquipBtn(int idx)
	{
		return m_Items.Find(o => o.m_Info.m_Idx == idx).GetBtn();
	}
}
