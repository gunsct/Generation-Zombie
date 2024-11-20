using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

[System.Serializable] public class DicInvenTabMenu : SerializableDictionary<Inventory.EMenu, Item_Tab> { }
public class Inventory : PopupBase
{
	public enum EMenu
	{
		None = -1,
		ETC = 0,
		Equip,
		DNA,
		Piece,
		End
	}
	public enum EState
	{
		Normal = 0,
		Sell,
		EqLVUP,
	}
#pragma warning disable 0649
	[System.Serializable]
	struct SSellUI
	{
		public GameObject Active;
		public TextMeshProUGUI Price;
		public GameObject NoSelectMsg;
		public Button Btn;
	}
	[System.Serializable]
	struct STabUI
	{
		public GameObject Active;
		public DicInvenTabMenu Menus;
	}

	[System.Serializable]
	struct SSizeUI
	{
		public Image Icon;
		public TextMeshProUGUI Cnt;
		public GameObject BuyBtn;
		public Color[] Colors;
	}

	[System.Serializable]
	struct SUI
	{
		public Animator Ani;
		public STabUI Tab;
		public SSellUI Sell;
		public GameObject[] Btns;//0:백버튼 1:처분버튼 2:DNA제작 바로가기 버튼
		public Item_SortingGroup SortingGroup;
		public GameObject Empty;
		public SSizeUI Size;
		public ScrollReck_ViewItemController ScrollController;
		public GameObject ItemPrefab;
		public GameObject DNALock;
		public TextMeshProUGUI DNALockTxt;
	}

	[SerializeField] SUI m_sUI;
	/// <summary> 메뉴별 아이템 데이터 </summary>
	Dictionary<EMenu, List<ItemInfo>> m_Datas = new Dictionary<EMenu, List<ItemInfo>>() { { EMenu.ETC, new List<ItemInfo>() }, { EMenu.Equip, new List<ItemInfo>() }, { EMenu.Piece, new List<ItemInfo>() } };
	List<DNAInfo> m_DNADatas = new List<DNAInfo>();
	/// <summary> 아이템 리스트 </summary>
	List<Item_Inventory_Item> m_Items = new List<Item_Inventory_Item>();
	/// <summary> 판매 등록 아이템 리스트 </summary>
	Dictionary<ItemInfo, int> m_SellItem = new Dictionary<ItemInfo, int>();
	IEnumerator m_Action;
	EMenu m_Menu;
	EState m_State;

	bool m_IsSell = false;

#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_sUI.ScrollController.SetData(0, m_sUI.ItemPrefab.transform as RectTransform, SetList);
		for (int i = 0; i < m_sUI.ScrollController.GetViewCnt(); i++) {
			m_Items.Add(m_sUI.ScrollController.GetItem<Item_Inventory_Item>(i));
		}

		base.SetData(pos, popup, cb, aobjValue);

		PLAY.PlayEffSound(SND_IDX.SFX_1900);

		int[] tabnames = { 237, 112, 340, 113 };
		SetMenuList();
		for (EMenu i = EMenu.ETC; i < EMenu.End; i++) {
			m_sUI.Tab.Menus[i].SetData((int)i, TDATA.GetString(tabnames[(int)i]), SelectMenu);
			SetMenuAlarm(i);
		}

		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);

		m_sUI.SortingGroup.SetData(SetSort);

		m_Menu = EMenu.None;

		StateChange((EMenu)aobjValue[0], EState.Normal, true);
		m_sUI.Tab.Menus[m_Menu].SetActive(true);
		m_sUI.SortingGroup.gameObject.SetActive(m_Menu == EMenu.Equip);

		//DNALock
		int openidx = BaseValue.CONTENT_OPEN_IDX(ContentType.CharDNA);
		m_sUI.DNALockTxt.text = string.Format(TDATA.GetString(1030), openidx / 100, openidx % 100);

		PlayerPrefs.SetInt($"InvenNewAlarm_{USERINFO.m_UID}", 0);//인벤토리 알림 끔
		for(EMenu i = EMenu.ETC; i< EMenu.End;i++) CheckAlram(i);

		ScrollSet();
	}
	void ScrollSet(bool initpos = true) {
		int cnt = m_Menu == EMenu.DNA ? m_DNADatas.Count : m_Datas[m_Menu].Count;
		if(initpos) m_sUI.ScrollController.InitPosition();
		m_sUI.ScrollController.SetItemCount(cnt);
	}
	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_State == EState.Sell) {
			m_IsSell = false;
			StateChange(m_Menu, EState.Normal); 
			return;
		}
		if (m_Action != null) return;
		// 애니 사용 안함
		base.Close(Result);
		//m_Action = CloaseAction(Result);
		//StartCoroutine(m_Action);
	}

	IEnumerator StartAction()
	{
		m_sUI.Btns[0].SetActive(false);
		m_sUI.Btns[1].SetActive(false);
		m_sUI.Btns[2].SetActive(false);
		yield return new WaitForSeconds(0.5f);
		m_sUI.Btns[0].SetActive(true);
		bool IsActive = m_State == EState.Normal && m_Menu != EMenu.DNA;
		if (m_sUI.Btns[1].activeSelf != IsActive) m_sUI.Btns[1].SetActive(IsActive);
		m_sUI.Btns[2].SetActive(m_Menu == EMenu.DNA && USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= BaseValue.CONTENT_OPEN_IDX(ContentType.CharDNA) + 1);
	}

	IEnumerator CloaseAction(int Result)
	{
		m_sUI.Ani.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_sUI.Ani));
		base.Close(Result);
	}

	void StateChange(EMenu Menu, EState state, bool Start = false)
	{
		m_State = state;
		if (m_State == EState.Sell) {
			for (int i = 0; i < USERINFO.m_Items.Count; i++) {
				USERINFO.m_Items[i].m_TempCnt = 0;
			}
			SellItemChange();
			SetMenu(m_Menu, false);
		}
		else SetMenu(Menu, false);

		m_sUI.Sell.Active.SetActive(m_State == EState.Sell);
		m_sUI.Tab.Active.SetActive(m_State == EState.Normal);
		if (Start) StartCoroutine(StartAction());
		else m_sUI.Btns[1].SetActive(m_State == EState.Normal && m_Menu != EMenu.DNA);
		m_sUI.Btns[2].SetActive(m_Menu == EMenu.DNA && USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= BaseValue.CONTENT_OPEN_IDX(ContentType.CharDNA) + 1);
		SetCntUI();

		SetMenuAlarm(Menu);

		CheckAlram(Menu); 
	}
	void SetMenuAlarm(EMenu Menu) {
		switch (Menu) {
			case EMenu.ETC:
				PlayerPrefs.SetInt($"InvenNewAlarm_ETC_{USERINFO.m_UID}", 0);
				break;
			case EMenu.Equip:
				PlayerPrefs.SetInt($"InvenNewAlarm_Equip_{USERINFO.m_UID}", 0);
				break;
			case EMenu.DNA:
				PlayerPrefs.SetInt($"InvenNewAlarm_DNA_{USERINFO.m_UID}", 0);
				break;
			case EMenu.Piece:
				PlayerPrefs.SetInt($"InvenNewAlarm_Piece_{USERINFO.m_UID}", 0);
				break;
		}
		PlayerPrefs.Save();
	}
	public void CheckAlram(EMenu _menu) {
		switch (_menu) {
			case EMenu.ETC:
				m_sUI.Tab.Menus[EMenu.ETC].SetAlram(PlayerPrefs.GetInt($"InvenNewAlarm_ETC_{USERINFO.m_UID}") == 1); 
				break;
			case EMenu.Equip:
				m_sUI.Tab.Menus[EMenu.Equip].SetAlram(PlayerPrefs.GetInt($"InvenNewAlarm_Equip_{USERINFO.m_UID}") == 1);
				break;
			case EMenu.DNA:
				m_sUI.Tab.Menus[EMenu.DNA].SetAlram(PlayerPrefs.GetInt($"InvenNewAlarm_DNA_{USERINFO.m_UID}") == 1);
				break;
			case EMenu.Piece:
				m_sUI.Tab.Menus[EMenu.Piece].SetAlram(PlayerPrefs.GetInt($"InvenNewAlarm_Piece_{USERINFO.m_UID}") == 1); 
				break;
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////
	// 메뉴
	void SetMenuList(EMenu _menu = EMenu.None) {
		m_Datas[EMenu.ETC].Clear();
		m_Datas[EMenu.Equip].Clear();
		m_Datas[EMenu.Piece].Clear();
		for (int i = 0; i < USERINFO.m_Items.Count; i++) {
			ItemInfo item = USERINFO.m_Items[i];
			ItemInvenGroupType group = item.m_TData.GetInvenGroupType();
			switch (group) {
				case ItemInvenGroupType.Equipment:
					// 장착된 아이템은 제거
					if (USERINFO.IsUseEquipItem(item.m_Uid)) continue;
					AddItem(EMenu.Equip, item);
					break;
				case ItemInvenGroupType.CharaterPiece:
					AddItem(EMenu.Piece, item);
					break;
				default:
					AddItem(EMenu.ETC, item);
					break;
			}
		}

		m_DNADatas.Clear();
		m_DNADatas.AddRange(USERINFO.m_DNAs.FindAll(o => !USERINFO.m_EqDNAs.Contains(o.m_UID)));
		
		if (_menu == EMenu.None) {
			//정렬
			//SetSort(EMenu.Equip);
			//SetSort(EMenu.ETC);
			//SetSort(EMenu.Piece);
			//SetSort(EMenu.DNA);
		}
		else {
			SetSort(_menu);
		}
	}

	void SetSort(EMenu _menu) {
		switch (_menu)
		{
		case EMenu.Equip:
				// 내림차순 기준 정렬
			//for (int i = 0; i < m_Datas[EMenu.Equip].Count; i++) m_Datas[EMenu.Equip][i].GetCombatPower();
			switch (m_sUI.SortingGroup.m_Condition)
			{
			case SortingType.Grade:
				m_Datas[EMenu.Equip].Sort((befor, after) =>
				{
					//새로얻은게 가장 앞으로
					if (befor.m_GetAlarm != after.m_GetAlarm) return after.m_GetAlarm.CompareTo(befor.m_GetAlarm);
					// 등급이 높은순
					int aGrade = after.m_Grade;
					int bGrade = befor.m_Grade;
					if (aGrade != bGrade) return aGrade.CompareTo(bGrade);
					// 레벨이 높은순
					if (befor.m_Lv != after.m_Lv) return after.m_Lv.CompareTo(befor.m_Lv);
					// 전투력 순
					int aftercp = after.m_CP;
					int beforcp = befor.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					// 장비 순서에 맞춰 셋팅
					int bno = GetSortNo_Eq(befor);
					int ano = GetSortNo_Eq(after);
					if (bno != ano) return ano.CompareTo(bno);
					if(befor.m_Idx != after.m_Idx) befor.m_Idx.CompareTo(after.m_Idx);
					return befor.m_Uid.CompareTo(after.m_Uid);
				});
				break;
			case SortingType.CombatPower:
				m_Datas[EMenu.Equip].Sort((befor, after) =>
				{
					//새로얻은게 가장 앞으로
					if (befor.m_GetAlarm != after.m_GetAlarm) return befor.m_GetAlarm.CompareTo(after.m_GetAlarm);
					// 전투력 순
					int aftercp = after.m_CP;
					int beforcp = befor.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					// 등급이 높은순
					int aGrade = after.m_Grade;
					int bGrade = befor.m_Grade;
					if (aGrade != bGrade) return aGrade.CompareTo(bGrade);
					// 레벨이 높은순
					if (befor.m_Lv != after.m_Lv) return after.m_Lv.CompareTo(befor.m_Lv);
					// 장비 순서에 맞춰 셋팅
					int bno = GetSortNo_Eq(befor);
					int ano = GetSortNo_Eq(after);
					if (bno != ano) return ano.CompareTo(bno);
					if (befor.m_Idx != after.m_Idx) befor.m_Idx.CompareTo(after.m_Idx);
					return befor.m_Uid.CompareTo(after.m_Uid);
				});
				break;
			case SortingType.Level:
				m_Datas[EMenu.Equip].Sort((befor, after) =>
				{
					//새로얻은게 가장 앞으로
					if (befor.m_GetAlarm != after.m_GetAlarm) return befor.m_GetAlarm.CompareTo(after.m_GetAlarm);
					// 레벨이 높은순
					if (befor.m_Lv != after.m_Lv) return after.m_Lv.CompareTo(befor.m_Lv);
					// 등급이 높은순
					int aGrade = after.m_Grade;
					int bGrade = befor.m_Grade;
					if (aGrade != bGrade) return aGrade.CompareTo(bGrade);
					// 전투력 순
					int aftercp = after.m_CP;
					int beforcp = befor.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					// 장비 순서에 맞춰 셋팅
					int bno = GetSortNo_Eq(befor);
					int ano = GetSortNo_Eq(after);
					if (bno != ano) return ano.CompareTo(bno);
					if (befor.m_Idx != after.m_Idx) befor.m_Idx.CompareTo(after.m_Idx);
					return befor.m_Uid.CompareTo(after.m_Uid);
				});
				break;
			default:
				m_Datas[EMenu.Equip].Sort((befor, after) =>
				{
					int aGrade = after.m_Grade;
					int bGrade = befor.m_Grade;
					if (aGrade != bGrade) return aGrade.CompareTo(bGrade);
					// 레벨이 높은순
					if (befor.m_Lv != after.m_Lv) return after.m_Lv.CompareTo(befor.m_Lv);
					// 전투력 순
					int aftercp = after.m_CP;
					int beforcp = befor.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					// 장비 순서에 맞춰 셋팅
					int bno = GetSortNo_Eq(befor);
					int ano = GetSortNo_Eq(after);
					if (bno != ano) return ano.CompareTo(bno);
					if (befor.m_Idx != after.m_Idx) befor.m_Idx.CompareTo(after.m_Idx);
					return befor.m_Uid.CompareTo(after.m_Uid);
				});
				break;
			}
			if (m_sUI.SortingGroup.m_Ascending) m_Datas[EMenu.Equip].Reverse();
			break;
		case EMenu.DNA:
				m_DNADatas.Sort((befor, after) => {//새로얻은게 가장 앞으로
					if (befor.m_GetAlarm != after.m_GetAlarm) return after.m_GetAlarm.CompareTo(befor.m_GetAlarm);
					int aGrade = after.m_Grade;
					int bGrade = befor.m_Grade;
					if (aGrade != bGrade) return aGrade.CompareTo(bGrade);
					// 레벨이 높은순
					if (befor.m_Lv != after.m_Lv) return after.m_Lv.CompareTo(befor.m_Lv);
					if (befor.m_Idx != after.m_Idx) befor.m_Idx.CompareTo(after.m_Idx);
					return befor.m_UID.CompareTo(after.m_UID);
				});
			break;
		default:
			m_Datas[_menu].Sort((befor, after) =>
			{
				int bsortno = GetSortNo_Etc(befor);
				int asortno = GetSortNo_Etc(after);

				// 기본 소팅번호 순
				if (bsortno != asortno) return bsortno.CompareTo(asortno);
				// 등급순
				int aGrade = after.m_Grade;
				int bGrade = befor.m_Grade;
				if (aGrade != bGrade) return aGrade.CompareTo(bGrade);
				if (befor.m_Idx != after.m_Idx) befor.m_Idx.CompareTo(after.m_Idx);
				return befor.m_Uid.CompareTo(after.m_Uid);
			});
			break;
		}
	}
	public void ChangeSort() {
		SetSort(EMenu.Equip);
	}
	void SetSort() {
		m_sUI.ScrollController.InitPosition();
		SetMenu(m_Menu, false);
	}
	int GetSortNo_Eq(ItemInfo item)
	{
		if (item.m_TSpecialStat == null) return 1;
		return 0;
	}
	int GetSortNo_Etc(ItemInfo item)
	{
		if (item.m_Idx == BaseValue.COMMON_PERSONNELFILE_IDX) return 0;
		if (item.m_TData.Is_UseItem()) return 1;
		return 2;
	}

	void AddItem(EMenu menu, ItemInfo item)
	{
		List<ItemInfo> items = m_Datas[menu];
		if (items.Contains(item)) return;
		items.Add(item);
	}

	bool SelectMenu(Item_Tab tab)
	{
		return SelectMenu(tab.m_Pos);
	}
	public bool SelectMenu(int _pos) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Inventory, 0, (EMenu)_pos)) return false;
		if (m_Menu != EMenu.None) m_sUI.Tab.Menus[m_Menu].SetActive(false);
		m_sUI.ScrollController.InitPosition();
		SetMenu((EMenu)_pos, true);
		m_sUI.SortingGroup.gameObject.SetActive((EMenu)_pos == EMenu.Equip); 
		m_sUI.Btns[1].SetActive(m_State == EState.Normal && m_Menu != EMenu.DNA);
		m_sUI.Btns[2].SetActive(m_Menu == EMenu.DNA && USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= BaseValue.CONTENT_OPEN_IDX(ContentType.CharDNA) + 1);
		return true;
	}

	void SetMenu(EMenu menu, bool _initpos)
	{
		// 탭간이동 로딩을 줄이기위해 최대 개수를 초과할때만 로드해주고 아니면 꺼주기만한다.
		SetSort(menu);
		m_Menu = menu;
		ScrollSet(_initpos);
	}
	void SetList(ScrollReck_ViewItemController.RefreshMode mode) {
		int cnt = 0;
		if (m_Menu == EMenu.DNA) {
			m_sUI.DNALock.SetActive(!USERINFO.CheckContentUnLock(ContentType.CharDNA, false));

			var listcnt = m_sUI.ScrollController.GetViewCnt();
			int offset = m_sUI.ScrollController.GetViewLine() * m_sUI.ScrollController.GetOneLineItemCnt();
			for (int i = 0; i < listcnt; i++, offset++) {
				Item_Inventory_Item item = m_sUI.ScrollController.GetItem<Item_Inventory_Item>(i);
				if (item == null) continue;
				if (offset > -1 && offset < m_DNADatas.Count) {
					DNAInfo info = m_DNADatas[offset];
					item.SetData(info, ViewInfo);
					item.StateChange(EState.Normal);
					//획득알람
					item.SetNewAlarm(info.m_GetAlarm);
					info.m_GetAlarm = false;
					item.gameObject.SetActive(true);
				}
				else item.gameObject.SetActive(false);
			}
			cnt = m_DNADatas.Count;
		}
		else {
			m_sUI.DNALock.SetActive(false);
			if (!m_Datas.ContainsKey(m_Menu)) m_Datas.Add(m_Menu, new List<ItemInfo>());
			List<ItemInfo> infos = m_Datas[m_Menu];

			var listcnt = m_sUI.ScrollController.GetViewCnt();
			int offset = m_sUI.ScrollController.GetViewLine() * m_sUI.ScrollController.GetOneLineItemCnt();
			for (int i = 0; i < listcnt; i++, offset++) {
				Item_Inventory_Item item = m_sUI.ScrollController.GetItem<Item_Inventory_Item>(i);
				if (item == null) continue;
				if (offset > -1 && offset < infos.Count) {
					item.gameObject.SetActive(true);
					ItemInfo info = infos[offset];
					if (m_IsSell) item.SetData(info, SellItemChange, null);
					else item.SetData(info, null, ViewInfo);//, info.m_TempCnt
					item.StateChange(m_IsSell ? EState.Sell : EState.Normal);
					if (m_SellItem.ContainsKey(item.m_ItemInfo) && m_SellItem[item.m_ItemInfo] != 0) {
						item.m_ItemInfo.m_TempCnt = m_SellItem[item.m_ItemInfo];
						item.SetCntUI();
					}
					//획득알람
					item.SetNewAlarm(info.m_GetAlarm);
					info.m_GetAlarm = false;
				}
				else item.gameObject.SetActive(false);
			}
			cnt = infos.Count;
		}

		m_sUI.Empty.SetActive(cnt < 1);
		SetMenuAlarm(m_Menu);
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////
	// 가방 사이즈
	void SetCntUI()
	{
		bool IsOver = USERINFO.m_InvenUseSize >= USERINFO.m_InvenSize;
		Color color = m_sUI.Size.Colors[IsOver ? 1 : 0];
		m_sUI.Size.Cnt.text = string.Format("{0} / {1}", USERINFO.m_InvenUseSize, USERINFO.m_InvenSize);
		m_sUI.Size.Cnt.color = color;
		m_sUI.Size.Icon.color = color;

		if (USERINFO.m_InvenSize < BaseValue.INVEN_SLOT_MAX) {
			m_sUI.Size.BuyBtn.SetActive(m_State == EState.Normal);
		}
		else {
			m_sUI.Size.BuyBtn.SetActive(false);
		}
	}

	public void AddBag()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Inventory, 3)) return;
		POPUP.StartInvenBuyPopup((result, obj) =>
		{
			SetCntUI();
		});
	}

	void ViewInfo(GameObject obj)
	{
		Item_Inventory_Item item = obj.GetComponent<Item_Inventory_Item>();
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Inventory, 2, m_Menu, item)) return;
		if (m_Menu == EMenu.DNA) {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_DNA, (result, obj)=> {
				SetCntUI();
				SetMenuList(m_Menu);
				SetMenu(m_Menu, false);
				//SetSort(m_Menu);
				//ScrollSet(false);
			}, item.m_DNAInfo);
		}
		else {
			Action<Info_Item.InfoChange, object[]> changeInfo = (state, args) => {
				// 가방 상태 갱신
				SetCntUI();
				SetMenuList(m_Menu);
				SetMenu(m_Menu, false);
				//SetSort(m_Menu);
				//ScrollSet(false);
			};

			ItemInfo info = item.m_ItemInfo;
			POPUP.ViewItemInfo(null, new object[] { info, m_Popup, changeInfo });
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////
	// 판매
	public void SetSellUI() {
		m_IsSell = true;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Inventory, 1, m_Menu)) return;
		StateChange(m_Menu, EState.Sell);
	}

	int SellItemChange(ItemInfo item = null, int Cnt = 0)
	{
		long Price = 0;
		if (item != null) {
			Price = GetPrice() + item.GetCellPrice(Cnt);
			if (!m_SellItem.ContainsKey(item)) m_SellItem.Add(item, Cnt);
			else m_SellItem[item] = item.m_TempCnt + Cnt;
		}
		else {
			for (int i = m_SellItem.Count - 1; i > -1; i--) {
				var info = m_SellItem.ElementAt(i);
				if (USERINFO.GetItem(info.Key.m_Uid) == null) m_Datas[m_Menu].Remove(info.Key);
			}
			m_SellItem.Clear();
		}
		m_sUI.Sell.Price.text = Utile_Class.CommaValue(Price);
		m_sUI.Sell.Btn.interactable = Price > 0;
		m_sUI.Sell.NoSelectMsg.SetActive(Price <= 0);
		return Cnt;
	}

	public long GetPrice()
	{
		long Price = 0;
		for (int i = m_SellItem.Count - 1; i > -1; i--)
		{
			var info = m_SellItem.ElementAt(i);
			Price += info.Key.m_TData.GetPrice() * info.Value;
		}
		return Price;
	}

	public void OnSell()
	{
		if (m_SellItem.Count < 1) return;

		if (new List<ItemInfo>(m_SellItem.Keys).Find(o => o.m_Lv > 1 && o.m_TData.GetEquipType() == EquipType.End) != null)
		{
			POPUP.Set_MsgBox(PopupName.Msg_YN, TDATA.GetString(252), TDATA.GetString(253), (btn, obj) =>
			{
				if ((EMsgBtn)btn == EMsgBtn.BTN_YES)
				{
					SEND_REQ_ITEM_SELL();
				}
			});
			return;
		}

		SEND_REQ_ITEM_SELL();
	}

	void SEND_REQ_ITEM_SELL()
	{

#if NOT_USE_NET
		long Price = GetPrice();
		
		for (int i = m_SellItem.Count - 1; i > -1; i--) {
			var info = m_SellItem.ElementAt(i);
			USERINFO.DeleteItem(info.Key.m_Uid, info.Value);
		}
		USERINFO.ChangeMoney(Price);
		SellItemChange();
		SetMenu(m_Menu, false);
		SetCntUI();
		//ScrollSet();
		PlayEffSound(SND_IDX.SFX_0109);
#else
		List<REQ_USE_ITEM> items = new List<REQ_USE_ITEM>();
		for (int i = m_SellItem.Count - 1; i > -1; i--)
		{
			var info = m_SellItem.ElementAt(i);
			items.Add(new REQ_USE_ITEM()
			{
				UID = info.Key.m_Uid,
				Cnt = info.Value
			});
		}

		WEB.SEND_REQ_ITEM_SELL((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			
			SellItemChange();
			SetMenu(m_Menu, false);
			SetCntUI();
			//ScrollSet();
			PlayEffSound(SND_IDX.SFX_0109);
		}, items);
#endif
	}
	public void ClickGoDNAMaking() {
		if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.CONTENT_OPEN_IDX(ContentType.CharDNA) + 1) return;
		PopupBase popup = POPUP.GetMainUI();
		if(popup.m_Popup == PopupName.Play) {
			Main_Play main = popup.GetComponent<Main_Play>();
			Item_PDA_Menu pda = main.GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			if(main.m_State == MainMenuType.PDA) {
				if (pda.m_State != Item_PDA_Menu.State.ZombieFarm) pda.ClickMenu(4);
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNAMaking, (result, obj) => {
				}, DNAMaking.PreState.ZombieFarm);
			}
			else {
				//main.MenuChange((int)MainMenuType.PDA, false);
				//pda.ClickMenu(4);
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNAMaking, (result, obj) => {
				}, DNAMaking.PreState.Inventory);
			}
		}
		Close(0);
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////
	// 튜토리얼 전용
	public GameObject GetTab(EMenu menu)
	{
		return m_sUI.Tab.Menus[menu].gameObject;
	}

	public GameObject GetItem(int idx)
	{
		return m_Items.Find(o => o.gameObject.activeSelf && o.m_ItemInfo.m_Idx == idx).gameObject;
	}
}
