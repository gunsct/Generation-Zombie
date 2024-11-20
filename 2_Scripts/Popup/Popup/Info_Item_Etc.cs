using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Info_Item_Etc : Info_Item
{
#pragma warning disable 0649

	[System.Serializable]
	public struct SUseUI {
		public GameObject[] Active;

		public TextMeshProUGUI Cnt;
		public TextMeshProUGUI Info;

		public TextMeshProUGUI UseCnt;
		public TextMeshProUGUI GetValue;
		public Image GetIcon;
		public Slider Guage;
		public RectTransform Scroll;
	}
	[Serializable]
	public struct SRouteUI {
		public TextMeshProUGUI Cnts;
		public Transform Bucket;
		public GameObject ElementPrefab;//Item_EtcGetGuide
		public TextMeshProUGUI BuyDesc;
		public TextMeshProUGUI Price;
		public GameObject[] RouteGroup;
	}
	[Serializable]
	public struct BUI
	{
		public GameObject FileChangeBtn;
		public GameObject UseBtn;
	}
	[SerializeField] SUseUI m_sUseUI;
	TGachaGroupTable m_TUsedata;
	[SerializeField] SRouteUI m_sRoutUI;
	[SerializeField] BUI m_BUI;
	int m_NeedCnt = -1;
	int m_BuyCnt;
	TShopTable m_TShopData { get { return TDATA.GetShopTable(m_Info.m_TData.m_ShopIdx); } }

#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{

		if (aobjValue.Length > 3) {
			m_NeedCnt = (int)aobjValue[3];
		}
		for(int i = 0; i < m_sRoutUI.RouteGroup.Length; i++) {
			m_sRoutUI.RouteGroup[i].SetActive(aobjValue.Length > 3);
		}

		base.SetData(pos, popup, cb, aobjValue);

		m_sUseUI.Guage.minValue = 1;
		m_sUseUI.Guage.maxValue = m_Info.m_Stack;
		SetValue(1);
	}

	public override void SetUI()
	{
		base.SetUI();

		m_sUseUI.Cnt.text = m_Info.m_Stack.ToString();
		m_sUseUI.Info.text = m_Info.m_TData.GetInfo();

		bool IsUseItem = m_Info.m_TData.Is_UseItem();
		m_sUseUI.Active[0].SetActive(IsUseItem);
		m_BUI.UseBtn.SetActive(IsUseItem);
		m_sUseUI.Active[1].SetActive(m_Info.m_TData.m_Type == ItemType.CharaterPiece || IsUseItem || m_NeedCnt > -1);
		m_BUI.FileChangeBtn.SetActive(false);//m_Info.m_TData.m_Type == ItemType.CharaterPiece

		bool scrolllong = !m_sUseUI.Active[0].activeSelf && !m_sRoutUI.RouteGroup[0].activeSelf;
		m_sUseUI.Scroll.sizeDelta = new Vector2(m_sUseUI.Scroll.sizeDelta.x, scrolllong ? 720f : 295f);

		// 이이템 효과 아이콘 셋팅
		if (m_sUseUI.Active[0].activeSelf)
		{
			// 사용 아니템은 보상이 1개만 됨
			// 여러개일경우는 다른방식으로 해야될듯
			
			//TGachaTable tgdata = TDATA.GetGachaTable(m_Info.m_Idx);
			//GachaGroup group = TDATA.getGachaGroup(tgdata.m_GachaGroup[0].m_Idx);
			
			GachaGroup group = TDATA.GetGachaGroup(m_Info.m_TData.m_Value);
			//
			
			m_TUsedata = group.m_List[0];
			TItemTable item = TDATA.GetItemTable(m_TUsedata.m_RewardIdx);
			m_sUseUI.GetIcon.sprite = BaseValue.GetItemIcon(item.m_Type);
		}
		if (m_NeedCnt > -1) SetGetGuide();
	}

	public void SetValue(int value)
	{
		// 값이 1이면 OnUseValueChange() 호출이 되지 않아 강제호출
		if (m_sUseUI.Guage.value == value) OnUseValueChange();
		else m_sUseUI.Guage.value = value;
	}

	public void OnUseValueChange()
	{
		int value = Mathf.RoundToInt(m_sUseUI.Guage.value);
		m_sUseUI.UseCnt.text = string.Format("{0} / {1}", value, m_Info.m_Stack);
		if(m_TUsedata != null) m_sUseUI.GetValue.text = Utile_Class.CommaValue(value * m_TUsedata.m_RewardCount);
	}

	public void OnPlus()
	{
		m_sUseUI.Guage.value++;
	}

	public void OnMinus()
	{
		m_sUseUI.Guage.value--;
	}

	public void OnUse()
	{
		m_Info.Use(Mathf.RoundToInt(m_sUseUI.Guage.value), ()=> {
			m_ChangeCB?.Invoke(InfoChange.Use, new object[] { this, m_Info });
			if (m_Info.m_Stack < 1) Close();
			else {
				// UI 갱신
				SetUI();
				m_sUseUI.Guage.maxValue = m_Info.m_Stack;
				SetValue(1);
			}
		});
	}

	void SetGetGuide() { 
		int getcnt = USERINFO.GetItemCount(m_Info.m_Idx);
		m_sRoutUI.Cnts.text = string.Format("<color={0}>{1}</color> / {2}", getcnt >= m_NeedCnt ? "#498E41" : "#D2533C", getcnt, m_NeedCnt);

		m_sRoutUI.RouteGroup[1].SetActive(getcnt < m_NeedCnt && m_TShopData != null);
		if (m_sRoutUI.RouteGroup[1].activeSelf) {
			m_BuyCnt = m_NeedCnt - getcnt;
			m_sRoutUI.BuyDesc.text = string.Format("{0} ({1} {2})", TDATA.GetString(513), TDATA.GetString(848), m_BuyCnt);
			m_sRoutUI.Price.text = Utile_Class.CommaValue(m_TShopData.GetPrice(m_BuyCnt));
			m_sRoutUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_Cash, m_TShopData.GetPrice(m_BuyCnt), "#D2533C", "#FFFFFF");
		}
		List<TGetGuideTable> guidelist = TDATA.GetGetGuideGroupTable(m_Info.m_TData.m_GetGuideGid);
		if (guidelist == null) return;
		for (int i = 0;i< guidelist.Count; i++) {
			Item_EtcGetGuide guide = Utile_Class.Instantiate(m_sRoutUI.ElementPrefab, m_sRoutUI.Bucket).GetComponent<Item_EtcGetGuide>();
			guide.SetData(guidelist[i].m_Idx, GoGet);
		}
	}

	void GoGet(ContentType _type) {
		if (m_Action != null) return;
		Main_Play play = POPUP.GetMainUI().GetComponent<Main_Play>();
		Item_PDA_Menu pda = play.GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
		Shop shop = play.GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
		Item_DungeonMenu dungeon = play.GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>();

		if (!USERINFO.CheckContentUnLock(_type, true)) return;
		switch (_type) {
			case ContentType.Store:
				play.MenuChange((int)MainMenuType.Shop, false);
				break;
			case ContentType.ZombieFarm:
				play.MenuChange((int)MainMenuType.PDA, false);
				pda.ClickMenu(4);
				break;
			case ContentType.Making:
				play.MenuChange((int)MainMenuType.PDA, false);
				pda.ClickMenu(3);
				break;
			case ContentType.Explorer:
				play.MenuChange((int)MainMenuType.PDA, false);
				pda.ClickMenu(1);
				break;
			case ContentType.Character:
				play.MenuChange((int)MainMenuType.Character, false);
				break;
			case ContentType.Bank:
				play.MenuChange((int)MainMenuType.Dungeon, false);
				dungeon.ClickGoDungeon((int)StageContentType.Bank);
				break;
			case ContentType.Academy:
				play.MenuChange((int)MainMenuType.Dungeon, false);
				dungeon.ClickGoDungeon((int)StageContentType.Academy);
				break;
			case ContentType.University:
				play.MenuChange((int)MainMenuType.Dungeon, false);
				dungeon.ClickGoDungeon((int)StageContentType.University);
				break;
			case ContentType.Tower:
				play.MenuChange((int)MainMenuType.Dungeon, false);
				dungeon.ClickGoDungeon((int)StageContentType.Tower);
				break;
			case ContentType.Cemetery:
				play.MenuChange((int)MainMenuType.Dungeon, false);
				dungeon.ClickGoDungeon((int)StageContentType.Cemetery);
				break;
			case ContentType.Factory:
				play.MenuChange((int)MainMenuType.Dungeon, false);
				dungeon.ClickGoDungeon((int)StageContentType.Factory);
				break;
			case ContentType.Subway:
				play.MenuChange((int)MainMenuType.Dungeon, false);
				dungeon.ClickGoDungeon((int)StageContentType.Subway);
				break;
		}
		Close(0);
	}
	/// <summary> 즉시 구매 </summary>
	public void ClickBuy() {
		if (m_Action != null) return;
		if (m_TShopData.GetPrice(m_BuyCnt) > USERINFO.m_Cash) {
			POPUP.StartLackPop(BaseValue.CASH_IDX);
			return;
		}
		USERINFO.ITEM_BUY(m_TShopData.m_Idx, m_BuyCnt, (res) => {
#if NOT_USE_NET
		USERINFO.InsertItem(m_TShopData.m_Rewards[0].m_ItemIdx, m_BuyCnt);
		USERINFO.GetCash(-m_TShopData.GetPrice(m_BuyCnt));
		MAIN.Save_UserInfo();
		Close(0);
#else
			Close(0);
#endif
		});
	}

	/// <summary> 인사 파일을 무기명 파일로 변환</summary>
	public void Click_ChangeFile() {
		if (m_Action != null) return;
		if (m_Info.m_TData.m_Type != ItemType.CharaterPiece) return;
		if (m_Info.m_Idx == BaseValue.COMMON_PERSONNELFILE_IDX) {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PersonnelFileList, (result, obj) => {
				SetUI();
				m_ChangeCB?.Invoke(InfoChange.Use, new object[] { this, m_Info });
			});
		}
		else {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PersonnelFileChange, (result, obj) => {
				if (result == 1) {
					m_ChangeCB?.Invoke(InfoChange.Use, new object[] { this, m_Info });
					if (USERINFO.GetItemCount(m_Info.m_Idx) < 1) Close(0);
					else SetUI();
				}
			}, m_Info.m_Idx, BaseValue.COMMON_PERSONNELFILE_IDX, m_Info.m_Grade);
		}
	}
	public override void OnlyInfo() {
		m_sUseUI.Active[0].SetActive(false);
		m_sUseUI.Active[1].SetActive(false);
	}
}
