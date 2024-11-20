using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Stage_Continue : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public SBUI Btns;
		public SGUI[] Goals;
	}
	[Serializable]
	public struct SGUI
	{
		public GameObject Group;
		public Item_Guide_GoalInfo Desc;
		public Item_Guide_GoalCard Card;
	}
	[Serializable]
	public struct SBUI
	{
		public Button ADBtn;
		public Image ADBtnBG;
		public TextMeshProUGUI[] ADTitle;	//0:그림자, 1:일반
		public TextMeshProUGUI ADDesc;		//0:광고시청, 1:남은시간
		//0:광고, 1:시간
		public Sprite[] ADBtnBGImg;				
		public Image[] ADIcon;
		public Color[] ADTitleColor;
		public Color[] ADTitleShadowColor;
		public Color[] ADDescColor;
		public TextMeshProUGUI[] TicketTitle;
		public TextMeshProUGUI TicketCnt;
		public Color[] TicketCntColor;

	}
	[SerializeField] SUI m_SUI;
	List<int> m_Cnts = new List<int>();
	TShopTable m_ShopTableTicket { get { return TDATA.GetShopTable(BaseValue.CONTINUETICKET_SHOP_IDX); } }
	int GetPrice { get {
			var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.CONTINUETICKET_GOLD_SHOP_IDX);
			int add = 0;
			int goldcnt = buyinfo == null ? 0 : buyinfo.Cnt;
			if (goldcnt > 0 && m_ShopTableTicket.m_UpPrice > 0) {
				for (int i = 0; i < m_UseCnt; i++) {
					if (i == 0) {
						goldcnt -= 1;
						if (goldcnt >= 0) add++;
					}
					else if (i == 1) {
						goldcnt -= 2;
						if (goldcnt >= 0) add++;
					}
					else if (i >= 2) {
						goldcnt -= 3;
						if (goldcnt >= 0) add++;
					}
					if (goldcnt < 0) break;
				}
			}
			int price = m_ShopTableTicket.GetPrice() + add;
			if(m_ShopTableTicket.m_MaxPrice > 0) price = Math.Min(price, m_ShopTableTicket.m_MaxPrice);
			return price;
		} }
	TShopTable m_ShopTableAD { get { return TDATA.GetShopTable(BaseValue.CONTINUEAD_SHOP_IDX); } }

	IEnumerator m_Action;
	IEnumerator m_SFXCor;
	int m_UseCnt = 0;//이어하기 제한량 계산때문에 쓰였었음
	SND_IDX m_PreBG;
	RES_SHOP_USER_BUY_INFO m_AdsBuyInfo { get { return USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.CONTINUEAD_SHOP_IDX); } }

	private void Update() {
		SetBtn();
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		//m_UseCnt = (int)aobjValue[0];
		m_Cnts = (List<int>)aobjValue[0];
		m_PreBG = SND.GetNowBG;
		SND.PlayBgSound(SND_IDX.BGM_0700);

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		SetGoal();

		SND.StopEffSound(SND_IDX.SFX_0352);
		if (m_SFXCor != null) StopCoroutine(m_SFXCor);
		StartCoroutine(m_SFXCor = IE_SFX());

		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
	}
	void SetBtn() {
		//광고 버튼
		int price = m_ShopTableAD.GetPrice();
		double timezone = UTILE.Get_ZeroTime();
		double timeinterval = price > 0 ? price * 60000 : 86400000;
		double remaintime = (price > 0 ? timeinterval - (UTILE.Get_ServerTime_Milli() - timezone) % timeinterval : timezone + timeinterval - UTILE.Get_ServerTime_Milli()) * 0.001d;
		//0초 됬을때
		if (m_AdsBuyInfo != null && UTILE.Get_ServerTime_Milli() + remaintime * 1000 - timeinterval < m_AdsBuyInfo.UTime) {
			m_SUI.Btns.ADDesc.text = UTILE.GetSecToTimeStr(remaintime);
			m_SUI.Btns.ADIcon[0].gameObject.SetActive(false);
			m_SUI.Btns.ADIcon[1].gameObject.SetActive(true);
			m_SUI.Btns.ADBtn.interactable = false;
		}
		else {
			m_SUI.Btns.ADDesc.text = USERINFO.m_ShopInfo.IsPassBuy() ? "FREE" : TDATA.GetString(5101);
			m_SUI.Btns.ADIcon[0].gameObject.SetActive(true);
			m_SUI.Btns.ADIcon[1].gameObject.SetActive(false);
			m_SUI.Btns.ADBtn.interactable = true;
		}
		if (m_AdsBuyInfo != null && timezone < m_AdsBuyInfo.UTime && m_AdsBuyInfo.Cnt >= m_ShopTableAD.m_LimitCnt) {
			m_SUI.Btns.ADBtn.interactable = false;
		}

		int pos = m_SUI.Btns.ADBtn.interactable ? 0 : 1;
		m_SUI.Btns.ADBtnBG.sprite = m_SUI.Btns.ADBtnBGImg[pos];
		m_SUI.Btns.ADDesc.color = m_SUI.Btns.ADDescColor[pos];
		m_SUI.Btns.ADTitle[0].color = m_SUI.Btns.ADTitleColor[pos];
		m_SUI.Btns.ADTitle[1].color = m_SUI.Btns.ADTitleShadowColor[pos];

		//티켓 버튼


		int ticketcnt = USERINFO.GetItemCount(BaseValue.CONTINUETICKET_IDX);
		pos = ticketcnt >= GetPrice ? 0 : 1;
		m_SUI.Btns.TicketCnt.text = string.Format("{0}/{1}", GetPrice, ticketcnt);
		m_SUI.Btns.TicketCnt.color = m_SUI.Btns.TicketCntColor[pos];
		m_SUI.Btns.TicketTitle[0].text = m_SUI.Btns.TicketTitle[1].text = TDATA.GetString(pos == 0 ? 10822 : 10823);
	}
	void SetGoal() {
		var guidui = STAGE?.m_MainUI?.GetGuideUI();
		if (guidui == null) guidui = TOWER?.m_MainUI?.GetGuideUI();
		Item_Guide guide = guidui.GetComponent<Item_Guide>();

		TStageTable stage = STAGEINFO.m_TStage;
		List<TStageCondition<StageClearType>> _Clear = stage.m_Clear;
		bool is_continuity = STAGEINFO.m_TStage.m_ClearMethod == ClearMethodType.Continuity;

		for (int i = 0; i < m_SUI.Goals.Length; i++) {
			if (_Clear.Count > i) {
				int maxcnt = STAGE.m_Check.GetClearMaxCnt(i);
				int nowcnt = m_Cnts[i];
				bool clear = maxcnt <= nowcnt;
				bool blind = is_continuity && i > guide.GetInfos[0].m_Pos;

				m_SUI.Goals[i].Group.SetActive(true);
				m_SUI.Goals[i].Card.SetData(_Clear[i], i, false);
				m_SUI.Goals[i].Desc.SetData(_Clear[i].m_Type, i, !blind, maxcnt, false);

				if (clear) {
					m_SUI.Goals[i].Card.SetAnim("Complete");
					m_SUI.Goals[i].Card.SetAnim("Not");
					m_SUI.Goals[i].Desc.SetAnim("Complete");
				}
				else {
					if (blind) {
						m_SUI.Goals[i].Card.SetAnim("Blind");
						m_SUI.Goals[i].Card.SetAnim("Loop");
						m_SUI.Goals[i].Desc.SetAnim("Normal");
					}
					else {
						m_SUI.Goals[i].Card.SetAnim((float)nowcnt / (float)maxcnt >= 0.8f ? "Change" : "Normal");
						m_SUI.Goals[i].Card.SetAnim("Loop");
						m_SUI.Goals[i].Desc.SetAnim("Normal");
					}
				}
			}
			else m_SUI.Goals[i].Group.SetActive(false);
		}
	}

	IEnumerator IE_SFX() {
		AudioSource auso = null;
		PlayEffSound(SND_IDX.SFX_0350);
		yield return new WaitForSeconds(1f);

		auso = PlayEffSound(SND_IDX.SFX_0351);
		yield return new WaitForSeconds(auso.clip.length - 0.5f);

		auso = PlayEffSound(SND_IDX.SFX_0352);
		for (int i = 0; i < Math.Ceiling((float)BaseValue.CONTINUE_COUNTDOWN / auso.clip.length) - 1; i++) {
			yield return new WaitForSeconds(auso.clip.length * 0.9f);
			auso = PlayEffSound(SND_IDX.SFX_0352);
		}
	}
	public void Click_ADS() {
		if (m_Action != null) return;
		ADS.ShowAds((result) => {
			if (result == IAds.ResultCode.Succecss) {
				ClickContinue(1);
			}
		});
	}

	void SetContinue(int _type)
	{
		switch (_type)
		{
		case 0:
#if NOT_USE_NET
			USERINFO.DeleteItem(m_ShopTableTicket.m_PriceIdx, GetPrice);
			MAIN.Save_UserInfo();
#else
			WEB.SEND_REQ_ANALYTICS(AnalyticsType.continue_rescureflare_button, USERINFO.m_LV, STAGEINFO.m_Idx, STAGEINFO.m_Pos);
#endif
			break;
		case 1:
#if NOT_USE_NET
			if (m_AdsBuyInfo == null)
			{
				USERINFO.m_ShopInfo.BUYs.Add(new RES_SHOP_USER_BUY_INFO()
				{
					Idx = m_ShopTableAD.m_Idx,
					Cnt = 1,
					UTime = (long)UTILE.Get_ServerTime_Milli()
				});
			}
			else
			{
				if (!UTILE.IsSameDay(m_AdsBuyInfo.UTime) && m_AdsBuyInfo.Cnt >= m_ShopTableAD.m_LimitCnt) m_AdsBuyInfo.Cnt = 0;
				else m_AdsBuyInfo.Cnt++;
				m_AdsBuyInfo.UTime = (long)UTILE.Get_ServerTime_Milli();
			}

			USERINFO.SetShopInfo();
#else
			WEB.SEND_REQ_ANALYTICS(AnalyticsType.continue_ad_button, USERINFO.m_LV, STAGEINFO.m_Idx, STAGEINFO.m_Pos);
#endif
			break;
		}
		Close(1);
	}
	/// <summary> 0골드 1광고 2티켓</summary>
	public void ClickContinue(int _type) {
		if (m_Action != null) return;
		switch (_type) {
			case 0:
				int price = GetPrice;
				int get = USERINFO.GetItemCount(BaseValue.CONTINUETICKET_IDX);
				if (get >= price) {
					USERINFO.ITEM_BUY(BaseValue.CONTINUETICKET_SHOP_IDX, 1, (res) => {
						SetContinue(_type);
					});
				}
				else {
					POPUP.StartLackPop(BaseValue.CONTINUETICKET_IDX, false, (result) => {
						if (result == EResultCode.SUCCESS) {
							USERINFO.ITEM_BUY(BaseValue.CONTINUETICKET_SHOP_IDX, 1, (res) => {
								SetContinue(_type);
							});
						}
					},null, price - get);
				}
				break;
			case 1:
				USERINFO.ITEM_BUY(BaseValue.CONTINUEAD_SHOP_IDX, 1, (res) => {
					SetContinue(_type);
				});
				break;
		}
	}
	/// <summary> 패스 구매 </summary>
	public void Click_BuyPass() {
		if (m_Action != null) return;
		StartCoroutine(IE_BuyPass());
	}
	IEnumerator IE_BuyPass() {
		bool buy = false;
		USERINFO.ITEM_BUY(USERINFO.m_ShopInfo.PassInfo[0].Idx, 1, (res) => {
			buy = true;
		}, true);

		GameObject passpopup = POPUP.GetPopup().gameObject;
		yield return new WaitWhile(() => passpopup != null);

		if (buy) {
			SetUI();
		}
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		SND.PlayBgSound(m_PreBG);
		StopCoroutine(m_SFXCor);
		SND.StopEffSound(SND_IDX.SFX_0352);
		base.Close(_result);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;

		if (m_Action != null) return;
#if !NOT_USE_NET
		if (Result == 0) WEB.SEND_REQ_ANALYTICS(AnalyticsType.continue_giveup_button, USERINFO.m_LV, STAGEINFO.m_Idx, STAGEINFO.m_Pos);
#endif
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
}
