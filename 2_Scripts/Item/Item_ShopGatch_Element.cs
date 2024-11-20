using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_ShopGatch_Element : ObjMng
{
	[Serializable]
	public struct PSUI
	{
		public Animator Anim;
		public TextMeshProUGUI Cnt;
	}
	[Serializable]
	public struct ASUI
	{
		public Image Bg;
		public Sprite[] BgImg;
		public TextMeshProUGUI TitleTxt;
		public Color[] TitleColor;
		public Image TimeIcon;
		public Color[] TimeColor;
		public TextMeshProUGUI PayTxt;
		public Color[] PayColor;
		public LayoutElement PayLayout;
		public Image ADIcon;
		public Color[] ADColor;
	}
	[Serializable]
	public struct SUI {
		public Item_Store_Buy_Button[] TicketBtns;
		public GameObject[] OneBtns;//0:광고, 1:티켓
		public Item_AD_Btn AdsBtn;
		public GameObject TenBtns;
		public GameObject ThirtyBtns;
		public GameObject[] BtnFx;//0: 1광고, 1: 10티켓, 2: 30티켓
		public Sprite[] NumImg;
		public GameObject[] LvGroup;
		public Image[] LvImg;
		public Slider ExpGauge;
		public TextMeshProUGUI[] ExpCnt;
		public int[] TicketIdxs;
		public PSUI PickUpUI;
		public ASUI AdsBtnUI;
		//튜토용
		public GameObject[] Btns;
	}
	[SerializeField] SUI m_SUI;
	List<TShopTable> m_TDatas;
	int m_Idx;
	Action<TShopTable> m_CB;
	TShopTable m_TAdsOne { get { return TDATA.GetShopTable(m_ShopIdx); } }
	RES_SHOP_USER_BUY_INFO m_AdsBuyInfo { get { return USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_ShopIdx); } }

	int m_ShopIdx;
	int[] m_PreExp = new int[3];
	int m_PreLv;
	bool Is_NotFirstInit;
	bool Is_AdsOn;

	int m_IGBLv;
	long m_IGBExp;
	long m_IGAExp;

	string m_PickUpTrig = string.Empty;
	public GameObject GetGacha10Btn() {
		return m_SUI.Btns[1];
	}
	public GameObject GetEquipGacha10Btn() {
		return m_SUI.Btns[1];
	}
	public GameObject GetPickupBtn() {
		return m_SUI.Btns[2];
	}
	private void OnEnable() {
		if (!string.IsNullOrEmpty(m_PickUpTrig) && m_SUI.PickUpUI.Anim != null) m_SUI.PickUpUI.Anim.SetTrigger(m_PickUpTrig);
	}
	public void SetData(List<TShopTable> _tdata, Action<TShopTable> _cb) {
		m_TDatas = _tdata;
		m_CB = _cb;
		m_ShopIdx = m_TDatas[0].m_Group == ShopGroup.Gacha ? BaseValue.ADSGACHAONE_SHOP_IDX : BaseValue.ADSITEMGACHAONE_SHOP_IDX;

		for (int i = 0; i < m_SUI.TicketBtns.Length; i++) {
			TShopTable tdata = TDATA.GetShopTable(m_SUI.TicketIdxs[i]);
			int getcnt = USERINFO.GetItemCount(tdata.m_PriceIdx);
			int needcnt = tdata.GetPrice();
			//if (getcnt >= needcnt) m_SUI.TicketBtns[i].SetData(tdata.m_PriceType, tdata.GetPrice(), tdata.m_PriceIdx);
			//else {
				TShopTable ticketdata = TDATA.GetShopTable(m_TDatas[0].m_Group == ShopGroup.Gacha ? BaseValue.SHOP_IDX_CHARGACHA_TICKET : BaseValue.SHOP_IDX_ITEMGACHA_TICKET);
				m_SUI.TicketBtns[i].SetData(new PriceType[2] { tdata.m_PriceType, ticketdata.m_PriceType }, new int[2] { getcnt > 0 ? Math.Min(needcnt, getcnt) : 0, ticketdata.GetPrice(Math.Max(0, needcnt - getcnt)) }, new int[2] { tdata.m_PriceIdx, ticketdata.m_PriceIdx });
			//}
		}
		SetPickUp();

		if (!Is_NotFirstInit) SetItemGachaLv();
		Is_NotFirstInit = true;
	}
	private void Update() {
		if (m_SUI.TicketIdxs.Length > 0) {
			if (m_TAdsOne == null) return;
			double cooltime = 60000L * m_TAdsOne.GetPrice();
			Is_AdsOn = m_AdsBuyInfo == null || (m_AdsBuyInfo != null && m_AdsBuyInfo.Cnt < m_TAdsOne.m_LimitCnt && m_AdsBuyInfo.UTime + cooltime <= UTILE.Get_ServerTime_Milli());
			double time = 0;
			if (!Is_AdsOn) {
				if (m_AdsBuyInfo.Cnt < m_TAdsOne.m_LimitCnt) time = (m_AdsBuyInfo.UTime + cooltime - UTILE.Get_ServerTime_Milli()) * 0.001d;
				else time = (m_AdsBuyInfo.GetTime() - UTILE.Get_ServerTime_Milli()) * 0.001d;
			}
			string adslabel = time > 0 ? UTILE.GetSecToTimeStr(time) : TDATA.GetString(5101);
			m_SUI.AdsBtn.SetLabel(adslabel);
			string adsdescadd = string.Format(" ({0}/{1})", m_TAdsOne.m_LimitCnt - (m_AdsBuyInfo != null ? m_AdsBuyInfo.Cnt : 0), m_TAdsOne.m_LimitCnt);
			m_SUI.AdsBtn.SetDesc(null, adsdescadd);
			int pos = Is_AdsOn ? 0 : 1;
			m_SUI.AdsBtnUI.Bg.sprite = m_SUI.AdsBtnUI.BgImg[pos];
			m_SUI.AdsBtnUI.TitleTxt.color = m_SUI.AdsBtnUI.TitleColor[pos];
			m_SUI.AdsBtnUI.TimeIcon.color = m_SUI.AdsBtnUI.TimeColor[pos];
			m_SUI.AdsBtnUI.PayTxt.color = m_SUI.AdsBtnUI.PayColor[pos];
			m_SUI.AdsBtnUI.ADIcon.color = m_SUI.AdsBtnUI.ADColor[pos];
			m_SUI.AdsBtnUI.PayTxt.fontSizeMax = Is_AdsOn ? 35f : 45f;
			m_SUI.AdsBtnUI.PayLayout.enabled = Is_AdsOn;


			if (m_SUI.BtnFx[0] != null) m_SUI.BtnFx[0].SetActive(Is_AdsOn);
		}
		if (m_SUI.TicketIdxs.Length > 1) {
			if (m_SUI.BtnFx[1] != null) m_SUI.BtnFx[1].SetActive(m_SUI.TicketBtns[1].Is_CanBuy);
		}
		if (m_SUI.TicketIdxs.Length > 2) {
			if (m_SUI.BtnFx[2] != null) m_SUI.BtnFx[2].SetActive(m_SUI.TicketBtns[2].Is_CanBuy);
		}
	}
	/// <summary> 장비 뽑기 레벨, 경험치 세팅 </summary>
	public void SetItemGachaLv() {
		if (m_TDatas[0].m_Group != ShopGroup.ItemGacha) return;

		var data = USERINFO.GetEquipGachaLv();
		int nexp = Mathf.RoundToInt(data.GetExp);
		int needexp = TDATA.GetEquipGachaTable(data.GetLv).m_Exp;
		if (!Is_NotFirstInit) {
			m_PreLv = data.GetLv;
			m_PreExp[0] = nexp;
			m_PreExp[1] = needexp;
			m_PreExp[2] = (int)USERINFO.m_ShopEquipGachaExp;
		}
		TW_Lv(m_PreLv);
		if (m_PreExp[1] == 0) {
			m_SUI.ExpCnt[0].text = m_SUI.ExpCnt[1].text = "<size=140%>Max</size>";
			m_SUI.ExpGauge.value = 1f;
		}
		else {
			m_SUI.ExpCnt[0].text = m_SUI.ExpCnt[1].text = string.Format("<size=140%>{0}</size> / {1}", m_PreExp[0], m_PreExp[1]);
			m_SUI.ExpGauge.value = (float)m_PreExp[0] / (float)m_PreExp[1];
		}
		if (!gameObject.activeInHierarchy) return;
		StartCoroutine(LvExpAction(data, nexp, needexp));
	}
	IEnumerator LvExpAction(UserInfo.ShopEquipGachaLvExp _data, int _nexp, int _needexp) { 
		var data = _data;
		int nexp = _nexp;
		int needexp = _needexp;
		int addexp = (int)USERINFO.m_ShopEquipGachaExp - m_PreExp[2];
		int remainmax = 0;
		for(int i = m_PreLv; ; i++) {
			if (i == m_PreLv) remainmax = m_PreExp[1] - m_PreExp[0];
			else {
				var tdata = TDATA.GetEquipGachaTable(i);
				if (tdata != null) {
					remainmax += tdata.m_Exp;
				}
				else break;
			}
		}
		addexp = Mathf.Min(addexp, Mathf.Max(remainmax, 0));
		if (m_PreLv != data.GetLv) {
			PLAY.PlayEffSound(SND_IDX.SFX_0111);
			GameObject popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_EquipGachaLvUp, null, data.GetLv).gameObject;
			yield return new WaitWhile(() => popup != null);
		}
		if (addexp > 0) {
			float time = 1f;

			PLAY.PlayEffSound(SND_IDX.SFX_1060);
			m_IGBLv = m_PreLv;
			m_IGBExp = m_PreExp[0];
			m_IGAExp = 0;
			iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", addexp, "onupdate", "TW_UserExp", "time", time, "easetype", "linear", "name", "EquipGauge"));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject, "EquipGauge"));

			m_PreLv = data.GetLv;
			m_PreExp[0] = nexp;
			m_PreExp[1] = needexp;
			m_PreExp[2] = (int)USERINFO.m_ShopEquipGachaExp;

			if (needexp == 0) {
				m_SUI.ExpCnt[0].text = m_SUI.ExpCnt[1].text = "<size=140%>Max</size>";
				m_SUI.ExpGauge.value = 1f;
			}
			else {
				m_SUI.ExpCnt[0].text = m_SUI.ExpCnt[1].text = string.Format("<size=140%>{0}</size> / {1}", nexp, needexp);
				m_SUI.ExpGauge.value = (float)nexp / (float)needexp;
			}
		}
	}
	void TW_Lv(float _amount) {
		int lv = Mathf.RoundToInt(_amount);
		if (lv < 100) {
			m_SUI.LvGroup[0].SetActive(true);
			m_SUI.LvGroup[1].SetActive(false);
			m_SUI.LvGroup[2].SetActive(true);
			m_SUI.LvImg[0].sprite = m_SUI.NumImg[lv / 10 >= 1 ? lv / 10 : 10];
			m_SUI.LvImg[2].sprite = m_SUI.NumImg[lv % 10];
		}
		else {
			m_SUI.LvGroup[0].SetActive(true);
			m_SUI.LvGroup[1].SetActive(true);
			m_SUI.LvGroup[2].SetActive(true);
			m_SUI.LvImg[0].sprite = m_SUI.NumImg[lv / 100];
			m_SUI.LvImg[1].sprite = m_SUI.NumImg[lv %100 / 10];
			m_SUI.LvImg[2].sprite = m_SUI.NumImg[lv % 10];
		}
		m_PreLv = lv;
	}
	void TW_UserExp(float _amount) {
		float exp = _amount - m_IGAExp;
		float nowexp = m_IGBExp + exp;
		long nowexpmax = TDATA.GetEquipGachaTable(m_IGBLv).m_Exp;
		if (nowexp >= nowexpmax) {
			nowexp = 0;
			m_IGAExp = (long)_amount;
			m_IGBExp = 0;
			m_IGBLv++;
			TW_Lv(m_IGBLv);
		}
		if (TDATA.GetEquipGachaTable(m_IGBLv).m_Exp == 0) {
			m_SUI.ExpCnt[0].text = m_SUI.ExpCnt[1].text = "<size=140%>Max</size>";
			m_SUI.ExpGauge.value = 1f;
		}
		else {
			m_SUI.ExpCnt[0].text = m_SUI.ExpCnt[1].text = string.Format("<size=140%>{0}</size> / {1}", Mathf.RoundToInt(nowexp), nowexpmax);
			m_SUI.ExpGauge.value = (float)nowexp / (float)nowexpmax;
		}
	}
	void SetPickUp() {
		if (m_TDatas[0].m_Group != ShopGroup.Gacha) return;
		int nowcnt = USERINFO.GetGachaPickUp().Count;
		int maxcnt = BaseValue.GetSelectivePickupOpenCnt(USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx);
		m_PickUpTrig = maxcnt <= nowcnt || maxcnt < 1 ? "Normal" : "Highlight";
		if (m_SUI.PickUpUI.Anim != null) m_SUI.PickUpUI.Anim.SetTrigger(m_PickUpTrig);
		if (m_SUI.PickUpUI.Cnt != null) m_SUI.PickUpUI.Cnt.text = string.Format("{0}/{1}", nowcnt, maxcnt);
	}
	public void ClickBuy(int _pos) {
		m_CB?.Invoke(m_TDatas[_pos]);
	}

	public void Click_ADS() {
		if (!Is_AdsOn) {
			if (m_AdsBuyInfo.Cnt >= m_TAdsOne.m_LimitCnt) {
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(5129));
				return;
			}
			else {
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(m_TDatas[0].m_Group == ShopGroup.Gacha ? 5131 : 5132));
				return;
			}
		}
		m_CB?.Invoke(m_TAdsOne);
	}
	public void Click_Ticket(int _pos) {
		TShopTable tdata = TDATA.GetShopTable(m_SUI.TicketIdxs[_pos]);
		m_CB?.Invoke(tdata);
	}
	public void Click_List() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopGachaProp, m_TDatas[0].m_Group == ShopGroup.Gacha ? 0 : 1)) return;
		if(m_TDatas[0].m_Group == ShopGroup.Gacha)
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Gacha_RewardList, null, TDATA.GetItemTable(m_TDatas[0].m_Rewards[0].m_ItemIdx).m_Value, true);
		else
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Gacha_RewardList, null, TDATA.GetEquipGachaTable(USERINFO.GetEquipGachaLv().GetLv).m_Gid, false);
	}
	public void Click_SelectChar() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PickupGacha_Btn)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Gacha_Pickup, (res, obj)=> { SetPickUp(); });
	}
	public void Click_PieceMakerInfo() {
		if(TUTO.TouchCheckLock(TutoTouchCheckType.PieceMakerInfo)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_EquipGacha_Bonus);
	}
}