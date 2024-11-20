using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipSelect : PopupBase
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
		public GameObject Empty;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] Animator m_Ani;
	List<Item_EqChange> m_Items = new List<Item_EqChange>();
	ItemInfo m_Info;
	EquipType m_EQType;

	IEnumerator m_Action;
	List<ItemInfo> m_ItemList;
	public ItemInfo m_SelectItem;

#pragma warning restore 0649
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		if (m_Ani == null) {
			base.Close(Result);
			return;
		}
		m_Action = StartEndAni(Result);
		StartCoroutine(m_Action);
	}

	IEnumerator StartEndAni(int Result) {
		m_Ani.SetTrigger("Close");
		yield return Utile_Class.CheckAniPlay(m_Ani);

		base.Close(Result);
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_EQType = (EquipType)aobjValue[0];
		if (aobjValue[1] != null) m_Info = (ItemInfo)aobjValue[1];
		if (aobjValue[2] != null) m_ItemList = (List<ItemInfo>)aobjValue[2];
		base.SetData(pos, popup, cb, aobjValue);
		StartCoroutine(StartAniChekc());
	}

	IEnumerator StartAniChekc() {
		yield return Utile_Class.CheckAniPlay(m_Ani);
	}

	public override void SetUI() {
		base.SetUI();
		// 장비 정보
		SetItemInfo();

		SetItemList();
	}

	void SetItemInfo() {
		if (m_Info != null) {
			m_SUI.None.Icon.gameObject.SetActive(false);
			m_SUI.Eq.Active.SetActive(true);
			m_SUI.Eq.Item.SetData(m_Info);
			m_SUI.Eq.CP.text = Utile_Class.CommaValue(m_Info.m_CP);
			m_SUI.Eq.Name.text = m_Info.m_TData.GetName();
		}
		else {
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
		//for (int i = 0; i < m_ItemList.Count; i++) m_ItemList[i].GetCombatPower();
		m_ItemList.Sort((befor, after) => {
			// 일단 전투력순으로만 소팅
			return after.m_CP.CompareTo(befor.m_CP);
		});

		UTILE.Load_Prefab_List(m_ItemList.Count, m_SUI.List.Scroll.content, m_SUI.List.Prefab.transform);


		for (int i = 0; i < m_ItemList.Count; i++) {
			Item_EqChange item = m_SUI.List.Scroll.content.GetChild(i).GetComponent<Item_EqChange>();
			item.SetData(m_ItemList[i], ItemSelect);
			m_Items.Add(item);
		}
		m_SUI.Empty.SetActive(m_Items.Count < 1);
	}

	void ItemSelect(Item_EqChange item, int state) {

		if (TUTO.TouchCheckLock(TutoTouchCheckType.EquipChange, item, state)) return;
		// 장착 변경해주고
		if (m_SelectItem == null) {
			PlayEffSound(SND_IDX.SFX_0004);
			m_SelectItem = item.m_Info;
			Close(1);
		}
	}

	public GameObject GetEquipBtn(int idx) {
		return m_Items.Find(o => o.m_Info.m_Idx == idx).GetBtn();
	}
}
