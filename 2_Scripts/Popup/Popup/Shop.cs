using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;

[System.Serializable] public class DicStoreTabMenu : SerializableDictionary<Shop.Tab, Shop.STabUI> { }
public class Shop : ObjMng
{
	public enum Tab
	{
		Shop = 0,
		Pass,
		Auction,
		Package,
		End
	}

	[System.Serializable]
	public struct STabUI
	{
		public GameObject Btn;
		public Animator BtnAni;
		public Item_Store_Tab_Base Item;
	}
	[Header("*상점 생성시 SnapScroll 초기화 에러 관련* \n 탭중 Shop, Package 두개만 켜져있어야 함")]
	[Space(10)]
	[SerializeField] Item_MoneyUI MoneyUI;
	[SerializeField] Item_GoldUI GoldUI;
	[SerializeField] DicStoreTabMenu Menu;
	[SerializeField] Animator Ani;
	[SerializeField] GameObject[] Alarms = new GameObject[4];
	Tab m_Tab;
	public Tab GetTab { get { return m_Tab; } }
	Action m_ChangeCB;
	SND_IDX m_PreBGSND;

	private void Start() {
		for (Tab i = 0; i < Tab.End; i++) {
			Menu[i].Item?.gameObject.SetActive(false);
		}
	}
	public void SetData()
	{
		m_Tab = Tab.End;
		StateChange((int)Tab.Shop);
	}
	public void InitTab() {
		m_Tab = Tab.End;
	}
	/// <summary> 메인의 상점에서 다른 메뉴로 갈때 상점을 샵으로 초기화 </summary>
	public void SetInitTab() {
		Menu[m_Tab].Item.gameObject.SetActive(false);
		m_Tab = Tab.Shop;
		Menu[m_Tab].Item.gameObject.SetActive(true);
		SetSelectBtn(m_Tab);
	}

	public void StateChange(int Pos) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopMenu, Pos)) return;
		DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);
		bool IsChange = m_Tab != Tab.End;
		var tab = (Tab)Pos;
		if (m_Tab == tab) return;
		if(IsChange) SetScrollState(true);
		Menu[Tab.Pass].Btn.SetActive(USERINFO.m_ShopInfo.PassInfo.Count > 0);
		m_Tab = tab;
		if (Menu[m_Tab].Item == null) SetSelectBtn(m_Tab);
		else
		{
			Menu[m_Tab].Item.SetData(() => {
				switch (m_Tab)
				{
					case Tab.Shop: PlayEffSound(SND_IDX.SFX_0130); break;
					case Tab.Package: PlayEffSound(SND_IDX.SFX_0131); break;
					case Tab.Pass: PLAY.PlayEffSound(SND_IDX.SFX_0140); break;
					case Tab.Auction: PLAY.PlayEffSound(SND_IDX.SFX_1800); break;
				}
				Menu[m_Tab].Item.gameObject.SetActive(true);
				SetSelectBtn(m_Tab);
				if (IsChange) Ani.SetTrigger("Change");
				if (m_ChangeCB != null)
				{
					m_ChangeCB.Invoke();
					m_ChangeCB = null;
				}
				SetAlarm();
			});
		}
	}

	void SetSelectBtn(Tab tab)
	{
		Menu[tab].BtnAni.SetTrigger("On");
		for (Tab i = 0; i < Tab.End; i++)
		{
			if (i == tab) continue;
			Menu[i].Item?.gameObject.SetActive(false);
			Menu[i].BtnAni.SetTrigger("Off");
		}
	}

	public void StartPos(bool _callout, ShopGroup group, bool _usetw = false, Action _cb = null) {
		if (_callout) {
			m_ChangeCB = () => { ((Item_Store_Tab_Shop)Menu[Tab.Shop].Item).StartPos(group, _usetw, _cb); };
			StateChange((int)Tab.Shop);
		}
		else {
			m_ChangeCB = null;
			((Item_Store_Tab_Shop)Menu[Tab.Shop].Item).StartPos(group, _usetw, _cb);
		}
	}
	public void SetAlarm(Tab _tab = Tab.End) {
		if(_tab == Tab.End || _tab == Tab.Shop) Alarms[(int)Tab.Shop].SetActive(USERINFO.GetStoreSupplyBoxCheck());
		if (_tab == Tab.End || _tab == Tab.Pass) {
#if NOT_USE_NET
			Alarms[(int)Tab.Pass].SetActive(false);
#else
			bool passalarm = false;
			if (USERINFO.m_ShopInfo.PassInfo.Count > 0) {
				var pass = USERINFO.m_Mission.Get_Missions(MissionMode.Pass, USERINFO.m_ShopInfo.PassInfo[0].Idx);
				var reward = pass.FindAll(o => o.IS_Complete() && (o.State[0] != RewardState.Get || (USERINFO.m_ShopInfo.IsPassBuy() ? o.State[1] != RewardState.Get : false)));
				passalarm = reward.Count > 0;
			}
			Alarms[(int)Tab.Pass].SetActive(passalarm);
#endif
		}
		if (_tab == Tab.End || _tab == Tab.Auction) Alarms[(int)Tab.Auction].SetActive(USERINFO.GetCheckNewAuctionGoods());
		if (_tab == Tab.End || _tab == Tab.Package) Alarms[(int)Tab.Package].SetActive(USERINFO.GetDailyPack());
	}
	public Item_Store_Tab_Base GetPanel(Tab _tab) {
		return Menu[_tab].Item;
	}
	private void OnEnable() {
		m_PreBGSND = SND.GetNowBG;
		PlayBGSound(SND_IDX.BGM_0021);
	}
	private void OnDisable() {
		SetScrollState(true);
		PlayBGSound(m_PreBGSND);
	}
	public void SetScrollState(bool Active) {
		Menu[m_Tab].Item.SetScrollState(Active);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 튜토리얼용
	public GameObject GetGacha10Btn()
	{
		return ((Item_Store_Tab_Shop)Menu[Tab.Shop].Item).GetGacha10Btn();
	}
	public GameObject GetEquipGacha10Btn() {
		return ((Item_Store_Tab_Shop)Menu[Tab.Shop].Item).GetEquipGacha10Btn();
	}
	public GameObject GetPickupBtn() {
		return ((Item_Store_Tab_Shop)Menu[Tab.Shop].Item).GetPickupBtn();
	}
	/// <summary> 0:-, 1:패스, 2:-, 3:달러, 4:캐릭터뽑기, 5:-, 6:보급상자, 7:암시장, 8:-, 9:-, 10:장비뽑기 </summary>
	public GameObject GetPanel(int _pos)
	{
		return ((Item_Store_Tab_Shop)Menu[Tab.Shop].Item).GetPanel(_pos);
	}
	public GameObject GetSupplyBoxBtn()
	{
		return ((Item_Store_Tab_Shop)Menu[Tab.Shop].Item).GetSupplyBoxBtn();
	}
}
