using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;

public class Item_Store_Tab_Shop : Item_Store_Tab_Base
{
	public class Rewards
	{
		public RewardKind m_Type;
		public int m_Idx;
		public int m_Grade;
		public int m_Cnt;
	}
	[Serializable]
	public struct SPUI //패스 상품
	{
		public GameObject Active;
		public Item_Store_Buy_Button Price;
		public TextMeshProUGUI Time;
		public TextMeshProUGUI Discount;
		public GameObject DiscoundGroup;
	}
	[Serializable]
	public struct SGUI //캐릭터 뽑기
	{
		public Animator Anim;
		public Item_ShopGatch_Element Goods;
	}
	[Serializable]
	public struct SRUI //추천 상품
	{
		public Transform Bucket;
	}
	[Serializable]
	public struct SSUI //보급 상자
	{
		public Animator Anim;
		public Image[] LvImg;
		public GameObject GuideGroup;
		public TextMeshProUGUI Guide;
		public Image Icon;

		public TextMeshProUGUI BtnName;	//보급 상자 열기, 광고 보고 열기, 월정액 상자 열기
		public TextMeshProUGUI BtnDesc; //open 남은 횟수 time 남은 시간
		public Color[] DescColor;		//일반, 광고, 월정액, 타임
		public Image BtnBg;
		public Sprite[] BgSprite;	//일반, 광고, 월정액, 타임
		public GameObject[] EtcObj;    //0:openfx, 1:timeicon 2:adicon
		public Image[] OpenFxImg;		//0:glow, 1:line
		public Color[] OpenFxColor;     //glow,line * 일반, 광고, 월정액
		public Image TimeIcon;
		public Color[] TimeIconColor;

		public GameObject ToolTip;
		public Transform[] ToolTipPos;
		public GameObject ToolTipTime;
		public TextMeshProUGUI[] ToolTipDesc;   //0:name,1:time
		public Image[] ToolTipGauge;			//0일반,1광고,2월정액
		public GameObject ToolTipMonthlyLock;
	}
	[Serializable]
	public struct SBUI //암시장
	{
		public Animator Anim;
		public GameObject Goods;
		public Transform Bucket;
		public TextMeshProUGUI[] Timer;
	}
	[Serializable]
	public struct SAUI //서버 경매
	{
		public Animator Anim;
	}
	[Serializable]
	public struct SCUI //금니(캐시)
	{
		public Animator Anim;
		public GameObject Goods;
		public Transform Bucket;
		public GameObject Group;
	}
	[Serializable]
	public struct SDUI //달러
	{
		public Animator Anim;
		public GameObject Goods;
		public Transform Bucket;
	}
	[Serializable]
	public struct SIGUI //장비 뽑기
	{
		public Animator Anim;
		public Item_ShopGatch_Element Goods;
	}
	[Serializable]
	public struct SMUI	//마일리지 상점
	{
		public TextMeshProUGUI Mileage;
	}
	[Serializable]
	public struct SMPUI	//월정액 패키지
	{
		public GameObject Active;
		public Item_GoodsBanner_Store Package;//3일 이하 남았을때 구매팝업
	}
	[Serializable]
    public struct SUI
	{
		/// <summary> 시즌 패스 </summary>
		public SPUI Pass;
		/// <summary> 캐릭터 뽑기 </summary>
		public SGUI Gacha;
		/// <summary> 추천 상품 </summary>
		public SRUI Recommand;
		/// <summary> 보급 상자 </summary>
		public SSUI Supply;
		/// <summary> 암시장 </summary>
		public SBUI Blackmarket;
		/// <summary> 서버 경매 </summary>
		public SAUI Auction;
		/// <summary> 금니 </summary>
		public SCUI Cash;
		/// <summary> 달러 </summary>
		public SDUI Dollar;
		/// <summary> 장비 뽑기 </summary>
		public SIGUI ItemGacha;
		/// <summary> 마일리지 </summary>
		public SMUI Mileage;
		/// <summary> 월정액제 상품 </summary>
		public SMPUI Monthly;

		public GameObject[] ObjGroup;
		public bool[] ObjCenter;
		public ScrollRect Scroll;
		public ScrollRect CashScroll;
		public ScrollRect DollarScroll;
		//튜토리얼용
		public GameObject SupplyboxBtn;
	}
	[SerializeField] SUI m_SUI;
	List<GameObject> m_AllProduct = new List<GameObject>();
	List<Item_Store_BlackMarket_Element> m_BlackMarketProduct = new List<Item_Store_BlackMarket_Element>();
	List<TShopTable> m_TGachas = new List<TShopTable>();
	List<TShopTable> m_TBlackMarkets = new List<TShopTable>();
	TShopTable m_TBlackMarketRF { get { return TDATA.GetShopTable(BaseValue.BLACKMARKET_REFRHES_SHOP_IDX); } }
	TShopTable m_TAdsBlackMarketRF { get { return TDATA.GetShopTable(BaseValue.BLACKMARKET_ADSREFRHES_SHOP_IDX); } }

	List<TShopTable> m_TCashs = new List<TShopTable>();
	List<TShopTable> m_TDollars = new List<TShopTable>();
	TShopTable m_TFreeSupplyNow;
	TShopTable m_TAdsSupplyNow;
	TShopTable m_TMonthlySupplyNow;
	List<TShopTable> m_TSupplyBoxs = new List<TShopTable>();
	RES_SHOP_USER_BUY_INFO m_SupplyBuyInfo { get { return USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_TFreeSupplyNow.m_Idx); } }
	RES_SHOP_USER_BUY_INFO m_AdsSupplyBuyInfo { get { return USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_TAdsSupplyNow.m_Idx); } }
	RES_SHOP_USER_BUY_INFO m_MonthlySupplyBuyInfo { get { return m_TMonthlySupplyNow == null ? null : USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_TMonthlySupplyNow.m_Idx); } }
	int m_FreeSupplyGrade = 0;
	int m_SupplyAlarmPos = 0;
	float m_SupplyAlarmTimer;
	double SUPPLY_TIME { get { return m_TFreeSupplyNow != null ? m_TFreeSupplyNow.GetPrice() * 60 * (1f - USERINFO.GetSkillValue(SkillKind.SupplyBoxDown)) : 0; } }
	double ADSSUPPLY_TIME { get { return m_TAdsSupplyNow != null ? m_TAdsSupplyNow.GetPrice() * 60 * (1f - USERINFO.GetSkillValue(SkillKind.SupplyBoxDown)) : 0; } }
	double MONTHLYSUPPLY_TIME { get { return m_TMonthlySupplyNow != null ? m_TMonthlySupplyNow.GetPrice() * 60 * (1f - USERINFO.GetSkillValue(SkillKind.SupplyBoxDown)) : 0; } }
	TSupplyBoxTable m_TSBData { get { return TDATA.GetSupplyBoxTable(m_FreeSupplyGrade); } }
	const int BLACKMARKET_TIMER = 86400;
	RES_SHOP_USER_BUY_INFO m_AdsBlackRefreshBuyInfo { get { return USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_TAdsBlackMarketRF.m_Idx); } }
	RES_SHOP_USER_BUY_INFO m_AdsGachaOneInfo { get { return USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.ADSGACHAONE_SHOP_IDX); } }
	List<TShopTable> m_TItemGachas = new List<TShopTable>();
	RES_SHOP_USER_BUY_INFO m_AdsItemGachaOneInfo { get { return USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.ADSITEMGACHAONE_SHOP_IDX); } }

	List<RecommendGoodsInfo> m_GoodsInfos = new List<RecommendGoodsInfo>();
	IEnumerator m_StartPosCor = null;

	bool IsMonthlySell;
	bool Is_Buying = false;
	/// <summary> 0 : none, 1 : pass, 2: cash, 3:dollar, 4:chargacha, 5:none, 6:supplybox, 7:blackmarket, 8:none, 9:equipgacha, 10~12:none, 13:mileage </summary>
	public GameObject GetPanel(int _pos) {
		return m_SUI.ObjGroup[_pos];
	}
	public GameObject GetSupplyBoxBtn() {
		return m_SUI.SupplyboxBtn;
	}
	public GameObject GetGacha10Btn() {
		return m_SUI.Gacha.Goods.GetGacha10Btn();
	}
	public GameObject GetEquipGacha10Btn() {
		return m_SUI.ItemGacha.Goods.GetComponent<Item_ShopGatch_Element>().GetEquipGacha10Btn();
	}
	public GameObject GetPickupBtn() {
		return m_SUI.Gacha.Goods.GetComponent<Item_ShopGatch_Element>().GetPickupBtn();
	}
	public void SetItemGachaGauge() {
		m_SUI.ItemGacha.Goods.SetItemGachaLv();
	}
	void Init(bool _scrollInit = true) {
		Is_Buying = false;

		if (_scrollInit) {
			m_SUI.Scroll.verticalNormalizedPosition = 1;
			m_SUI.CashScroll.horizontalNormalizedPosition = 0;
			m_SUI.DollarScroll.horizontalNormalizedPosition = 0;
		}
		for (int i = m_AllProduct.Count - 1; i > -1; i--) Destroy(m_AllProduct[i]);
		m_AllProduct.Clear();
		SetSupplyToolTip(0, false);

		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
		DLGTINFO?.f_RFCharTicketUI?.Invoke(USERINFO.GetItemCount(BaseValue.CHAR_GACHA_TICKET_IDX), USERINFO.GetItemCount(BaseValue.CHAR_GACHA_TICKET_IDX));
		DLGTINFO?.f_RFItemTicketUI?.Invoke(USERINFO.GetItemCount(BaseValue.EQ_GACHA_TICKET_IDX), USERINFO.GetItemCount(BaseValue.EQ_GACHA_TICKET_IDX));
	}
	public override void SetData(Action CB) {
#if NOT_USE_NET
		m_TGachas = TDATA.GetGroupShopTable(ShopGroup.Gacha).FindAll(o=>o.m_PriceType == PriceType.Cash);
		m_TItemGachas = TDATA.GetGroupShopTable(ShopGroup.ItemGacha).FindAll(o => o.m_PriceType == PriceType.Cash);
		m_TBlackMarkets = TDATA.GetGroupShopTable(ShopGroup.BlackMarket).FindAll(o => o.m_Rewards[0].m_ItemIdx != 0);
		m_TDollars = TDATA.GetGroupShopTable(ShopGroup.Money);
		m_TCashs = TDATA.GetGroupShopTable(ShopGroup.Cash);
		m_TFreeSupplyNow = TDATA.GetGroupShopTable(ShopGroup.SupplyBox).Find(o=>o.m_PriceType == PriceType.Time);
		m_TAdsSupplyNow = TDATA.GetGroupShopTable(ShopGroup.SupplyBox).Find(o => o.m_PriceType == PriceType.AD_AddTime);

		m_FreeSupplyGrade = USERINFO.m_SupplyBoxLV + Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.SupplyBoxGradeUp));
		Init();

		SetUI();
		CB?.Invoke();
#else
		WEB.SEND_REQ_SHOP_INFO((res) => {
			USERINFO.SetDATA(res);
			IAP.Init();
			m_TGachas = new List<TShopTable>();
			m_TItemGachas = new List<TShopTable>();
			m_TBlackMarkets = new List<TShopTable>();
			m_TDollars = new List<TShopTable>();
			m_TCashs = new List<TShopTable>();
			m_TSupplyBoxs = new List<TShopTable>();
			//툴데이터 기준 뽑고 그다음 인포에 없는건 그냥 나오고 있는건 논인지 타임인지 처리
			List<TShopTable> shoptables = TDATA.GetAllShopTable().Values.ToList();
			shoptables = shoptables.FindAll(o => o.m_Group != ShopGroup.Event_BlackMarket);
			for (int i = 0;i< shoptables.Count; i++) {
				RES_SHOP_ITEM_INFO info = USERINFO.m_ShopInfo.UseInfos.Find(o => o.Idx == shoptables[i].m_Idx);
				if (info != null && info.UseType == ShopUseType.None) continue;//샵정보는 있지만 안보여줄거는 넘김
				switch (shoptables[i].m_Group) {
					case ShopGroup.Gacha:
						if(shoptables[i].m_PriceType == PriceType.Cash) m_TGachas.Add(shoptables[i]);
						break;
					case ShopGroup.ItemGacha:
						if (shoptables[i].m_PriceType == PriceType.Cash) m_TItemGachas.Add(shoptables[i]);
						break;
					case ShopGroup.BlackMarket:
						m_TBlackMarkets.Add(shoptables[i]);
						break;
					case ShopGroup.Money:
						m_TDollars.Add(shoptables[i]);
						break;
					case ShopGroup.Cash:
						m_TCashs.Add(shoptables[i]);
						break;
					case ShopGroup.SupplyBox:
						if (shoptables[i].m_Idx == BaseValue.NORMAL_SUPPLYBOX_SHOP_IDX) m_TFreeSupplyNow = shoptables[i];
						else if(shoptables[i].m_Idx == BaseValue.ADS_SUPPLYBOX_SHOP_IDX) m_TAdsSupplyNow = shoptables[i];
						else if (shoptables[i].m_Idx == BaseValue.MONTHLY_SUPPLYBOX_SHOP_IDX) m_TMonthlySupplyNow = shoptables[i];
						else m_TSupplyBoxs.Add(shoptables[i]);
						break;
				}
			}
			m_FreeSupplyGrade = USERINFO.m_SupplyBoxLV + Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.SupplyBoxGradeUp));
			IsMonthlySell = USERINFO.m_ShopInfo.Infos.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE) == null;

			Init(CB != null);
			SetUI();
			CB?.Invoke();
		});
#endif
	}
	public override void SetUI() {
		base.SetUI();
		//패스
		SeasonPassUI();
		//가챠
		GachaUI();
		//장비 가챠
		ItemGachaUI();
		//보급상자
		SupplyBoxUI();
		//암시장
		BlackMarketUI();
		//달러
		DollarUI();
		//달러
		CashUI();
		//마일리지
		MileageUI();
		//월정액제
		MonthlyUI();
	}
	void GachaUI() {
		if (m_TGachas == null) return;
		m_SUI.Gacha.Goods.SetData(new List<TShopTable>() { m_TGachas[0], m_TGachas[1] }, ClickGacha);
	}
	void ItemGachaUI() {
		if (m_TItemGachas == null) return;
		m_SUI.ItemGacha.Goods.SetData(m_TItemGachas, ClickItemGacha);
	}
	void SupplyBoxUI() {
		m_SUI.Supply.Icon.sprite = UTILE.LoadImg(string.Format("UI/UI_Store/Icon_SupplyBox_{0}", Mathf.Min((int)(m_FreeSupplyGrade / 2) + 1, 5)), "png");
		m_SUI.Supply.Anim.SetTrigger(UTILE.Get_ServerTime_Milli() - (m_SupplyBuyInfo == null ? 0 : m_SupplyBuyInfo.UTime) * 0.001 > SUPPLY_TIME ? "Open" : "Wait");
		m_SUI.Supply.LvImg[0].sprite = UTILE.LoadImg(string.Format("UI/UI_Store/SupplyBox_NumberFont_{0}", m_FreeSupplyGrade % 10), "png");
		m_SUI.Supply.LvImg[1].sprite = UTILE.LoadImg(string.Format("UI/UI_Store/SupplyBox_NumberFont_{0}", m_FreeSupplyGrade % 100 / 10), "png");
		m_SUI.Supply.LvImg[2].sprite = UTILE.LoadImg(string.Format("UI/UI_Store/SupplyBox_NumberFont_{0}", m_FreeSupplyGrade / 100), "png");
		if (m_FreeSupplyGrade < TDATA.GetSupplyBoxMaxLV()) {
			TSupplyBoxTable sdata =  TDATA.GetSupplyBoxTable(m_FreeSupplyGrade + 1);
			m_SUI.Supply.Guide.text = string.Format(TDATA.GetString(904), sdata.OpenValue / 100, sdata.OpenValue % 100);
			m_SUI.Supply.GuideGroup.SetActive(true);
		}
		else m_SUI.Supply.GuideGroup.SetActive(false);

		SetSupplyBtn();
	}
	void SetSupplyBtn() {
		bool isopen = false;
		bool isadsicon = false; 
		bool ispass = USERINFO.m_ShopInfo.IsPassBuy();
		//월정액, 일반, 광고 순으로 체크
		RES_SHOP_DAILYPACK_INFO monthlypackinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		double monthlytime = MONTHLYSUPPLY_TIME - (UTILE.Get_ServerTime_Milli() - (m_MonthlySupplyBuyInfo == null ? 0 : m_MonthlySupplyBuyInfo.UTime)) * 0.001;
		double normaltime = SUPPLY_TIME - (UTILE.Get_ServerTime_Milli() - (m_SupplyBuyInfo == null ? 0 : m_SupplyBuyInfo.UTime)) * 0.001;
		double adstime = ADSSUPPLY_TIME - (UTILE.Get_ServerTime_Milli() - (m_AdsSupplyBuyInfo == null ? 0 : m_AdsSupplyBuyInfo.UTime)) * 0.001;

		bool IsPlayPackage = IsMonthlySell && monthlypackinfo != null && monthlypackinfo.IsPlayPack();

		m_SUI.Supply.ToolTipPos[2].gameObject.SetActive(IsMonthlySell);

		//오픈된것
		if (IsPlayPackage && monthlytime <= 0) {
			isopen = true;
			m_SUI.Supply.BtnName.text = TDATA.GetString(10887);
			m_SUI.Supply.BtnDesc.text = string.Format("{0}{1}", 1, TDATA.GetString(5103));//m_TMonthlySupplyNow.m_LimitCnt - (m_MonthlySupplyBuyInfo == null ? 0 : m_MonthlySupplyBuyInfo.Cnt)
			m_SUI.Supply.BtnDesc.color = m_SUI.Supply.DescColor[2];
			m_SUI.Supply.BtnBg.sprite = m_SUI.Supply.BgSprite[2];
			m_SUI.Supply.OpenFxImg[0].color = m_SUI.Supply.OpenFxColor[4];
			m_SUI.Supply.OpenFxImg[1].color = m_SUI.Supply.OpenFxColor[5];
		}
		else if (normaltime <= 0) {
			isopen = true;
			m_SUI.Supply.BtnName.text = TDATA.GetString(5002);
			m_SUI.Supply.BtnDesc.text = string.Format("{0}{1}", 1, TDATA.GetString(5103));//m_TFreeSupplyNow.m_LimitCnt - (m_SupplyBuyInfo == null ? 0 : m_SupplyBuyInfo.Cnt)
			m_SUI.Supply.BtnDesc.color = m_SUI.Supply.DescColor[0];
			m_SUI.Supply.BtnBg.sprite = m_SUI.Supply.BgSprite[0];
			m_SUI.Supply.OpenFxImg[0].color = m_SUI.Supply.OpenFxColor[0];
			m_SUI.Supply.OpenFxImg[1].color = m_SUI.Supply.OpenFxColor[1];
		}
		else if (adstime <= 0) {
			isopen = true;
			isadsicon = true;
			m_SUI.Supply.BtnName.text = TDATA.GetString(ispass ? 1110 : 1046);
			m_SUI.Supply.BtnDesc.text = string.Format("{0}{1}", 1, TDATA.GetString(5103));//m_TAdsSupplyNow.m_LimitCnt - (m_AdsSupplyBuyInfo == null ? 0 : m_AdsSupplyBuyInfo.Cnt)
			m_SUI.Supply.BtnDesc.color = m_SUI.Supply.DescColor[1];
			m_SUI.Supply.BtnBg.sprite = m_SUI.Supply.BgSprite[1];
			m_SUI.Supply.OpenFxImg[0].color = m_SUI.Supply.OpenFxColor[2];
			m_SUI.Supply.OpenFxImg[1].color = m_SUI.Supply.OpenFxColor[3];
		}
		else isopen = false;

		if (!isopen) {
			if (IsPlayPackage && monthlytime > 0) {
				m_SUI.Supply.BtnName.text = TDATA.GetString(10887);
				m_SUI.Supply.BtnDesc.text = UTILE.GetSecToTimeStr(monthlytime);
				m_SUI.Supply.TimeIcon.color = m_SUI.Supply.TimeIconColor[2];
			}
			else if (normaltime > 0) {
				m_SUI.Supply.BtnName.text = TDATA.GetString(5002);
				m_SUI.Supply.BtnDesc.text = UTILE.GetSecToTimeStr(normaltime);
				m_SUI.Supply.TimeIcon.color = m_SUI.Supply.TimeIconColor[0];
			}
			else if (adstime > 0) {
				isadsicon = true;
				m_SUI.Supply.BtnName.text = TDATA.GetString(ispass ? 1110 : 1046);
				m_SUI.Supply.BtnDesc.text = UTILE.GetSecToTimeStr(adstime);
				m_SUI.Supply.TimeIcon.color = m_SUI.Supply.TimeIconColor[1];
			}
			m_SUI.Supply.BtnDesc.color = m_SUI.Supply.DescColor[3];
			m_SUI.Supply.BtnBg.sprite = m_SUI.Supply.BgSprite[3];
		}

		m_SUI.Supply.EtcObj[0].SetActive(isopen);
		m_SUI.Supply.EtcObj[1].SetActive(!isopen);
		m_SUI.Supply.EtcObj[2].SetActive(isadsicon);

		m_SUI.Supply.ToolTipMonthlyLock.SetActive(!IsPlayPackage);
		m_SUI.Supply.ToolTipGauge[0].fillAmount = 1f - Mathf.Clamp((float)(normaltime / SUPPLY_TIME), 0f, 1f);
		m_SUI.Supply.ToolTipGauge[1].fillAmount = 1f - Mathf.Clamp((float)(adstime / ADSSUPPLY_TIME), 0f, 1f);
		m_SUI.Supply.ToolTipGauge[2].fillAmount = 1f - Mathf.Clamp((float)(monthlytime / MONTHLYSUPPLY_TIME), 0f, 1f);
		
		if (m_SupplyAlarmPos == 0) {//일반
			m_SUI.Supply.ToolTipDesc[0].text = TDATA.GetString(5117);
			m_SUI.Supply.ToolTipDesc[1].text = UTILE.GetSecToTimeStr(normaltime);
		}
		else if (m_SupplyAlarmPos == 1) {//광고
			m_SUI.Supply.ToolTipDesc[0].text = TDATA.GetString(5118);
			m_SUI.Supply.ToolTipDesc[1].text = UTILE.GetSecToTimeStr(adstime);
		}
		else if (m_SupplyAlarmPos == 2) {//월정액
			m_SUI.Supply.ToolTipDesc[0].text = TDATA.GetString(!IsPlayPackage ? 5120 : 5116);
			m_SUI.Supply.ToolTipDesc[1].text = UTILE.GetSecToTimeStr(monthlytime);
		}

		if (m_SUI.Supply.ToolTip.activeSelf) {
			m_SupplyAlarmTimer += Time.deltaTime;
			if (m_SupplyAlarmTimer >= 3f) SetSupplyToolTip(0, false);
		}
	}
	public void Click_SupplyBoxToopTip(int _pos) {
		if (TUTO.IsTutoPlay()) return;
		if (_pos == 2) {
			RecommendGoodsInfo rginfo = USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop).Find(o => o.m_SIdx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
			RES_SHOP_DAILYPACK_INFO monthlypackinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
			bool IsPlayPackage = monthlypackinfo != null && monthlypackinfo.IsPlayPack();
			if (!IsPlayPackage) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Purchase_Monthly, (res, obj) => {
					if (res == 1) {
						SetData(null);
					}
				}, rginfo);
			}
			else
				SetSupplyToolTip(_pos, true);
		}
		else
			SetSupplyToolTip(_pos, true);
	}
	void SetSupplyToolTip(int _pos, bool _active) {
		m_SupplyAlarmPos = _pos;
		m_SupplyAlarmTimer = 0f;
		m_SUI.Supply.ToolTip.SetActive(_active);
		if (_active) {
			bool timeview = m_SUI.Supply.ToolTipGauge[_pos].fillAmount < 1f;
			RES_SHOP_DAILYPACK_INFO monthlypackinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
			bool IsPlayPackage = monthlypackinfo != null && monthlypackinfo.IsPlayPack();
			m_SUI.Supply.ToolTipTime.SetActive(_pos != 2 ? timeview : timeview && IsPlayPackage);
		}
		m_SUI.Supply.ToolTip.transform.position = m_SUI.Supply.ToolTipPos[_pos].position + new Vector3(0f, 222f, 0f);
	}
	void BlackMarketUI() {
		if (m_TBlackMarkets == null) return;
		List<TShopTable> tdatas = new List<TShopTable>();

		if (USERINFO.m_BlackMarketBuy.Count > 0) {
			for (int i = 0; i < USERINFO.m_BlackMarketBuy.Count; i++) {
				tdatas.Add(TDATA.GetShopTable(USERINFO.m_BlackMarketBuy.ElementAt(i).Key));
			}
		}
		else {
			for(int i = m_AllProduct.Count - 1; i > -1; i--) {
				if (m_AllProduct[i].GetComponent<Item_Store_BlackMarket_Element>() != null) {
					GameObject obj = m_AllProduct[i];
					m_AllProduct.Remove(obj);
					Destroy(obj);
				}
				m_BlackMarketProduct.Clear();
			} 
			List<int> idxs = new List<int>();
			for (int i = 0; i < 6; i++) {
				List<TShopTable> blacktables = m_TBlackMarkets.FindAll(o => !idxs.Contains(o.m_Idx));
				int probsum = blacktables.Sum(o => o.m_NoOrProb);
				int prob = UTILE.Get_Random(0, probsum);
				int preprob = 0;
				for (int j = 0; j < blacktables.Count; j++) {
					preprob += blacktables[j].m_NoOrProb;
					if (preprob >= prob) {
						idxs.Add(blacktables[j].m_Idx);
						tdatas.Add(blacktables[j]);
						USERINFO.InsertBlackMarket(blacktables[j].m_Idx, false);
						break;
					}
				}
			}
		}
		double blackmarkettime = double.Parse(PlayerPrefs.GetString($"SHOP_BLACKMERKET_TIME_{USERINFO.m_UID}"));
		for (int i = 0; i < tdatas.Count; i++) {
			//상점 갱신 시간 이후에 산것
			bool bought = USERINFO.m_ShopInfo.BUYs.Find(o => blackmarkettime < o.UTime  && o.Idx == tdatas[i].m_Idx) != null;
			Item_Store_BlackMarket_Element goods = Utile_Class.Instantiate(m_SUI.Blackmarket.Goods, m_SUI.Blackmarket.Bucket).GetComponent<Item_Store_BlackMarket_Element>();
			goods.SetData(tdatas[i], MAIN.GetRewardBase(tdatas[i], RewardKind.Item)[0], bought, ClickBlackMarket);
			m_BlackMarketProduct.Add(goods);
			m_AllProduct.Add(goods.gameObject);
		}
	}
	void DollarUI() {
		if (m_TDollars == null) return;
		for (int i = 0; i < m_TDollars.Count; i++) {
			Item_Store_Dollar_Element goods = Utile_Class.Instantiate(m_SUI.Dollar.Goods, m_SUI.Dollar.Bucket).GetComponent<Item_Store_Dollar_Element>();
			goods.SetData(m_TDollars[i], ClickDollar);
			m_AllProduct.Add(goods.gameObject);
		}
	}
	void CashUI() {
		m_SUI.Cash.Group.SetActive(m_TCashs != null && m_TCashs.Count > 0);
		if (m_TCashs == null) return;
		for (int i = 0; i < m_TCashs.Count; i++) {
			Item_Store_Cash_Element goods = Utile_Class.Instantiate(m_SUI.Cash.Goods, m_SUI.Cash.Bucket).GetComponent<Item_Store_Cash_Element>();
			goods.SetData(m_TCashs[i], ClickDollar);
			m_AllProduct.Add(goods.gameObject);
		}
	}
	void MileageUI() {
		m_SUI.Mileage.Mileage.text = Utile_Class.CommaValue(USERINFO.m_Mileage);
	}
	void MonthlyUI() {
		int sidx = BaseValue.SHOP_IDX_MONTHLY_PACKAGE;
		RecommendGoodsInfo rginfo = USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop).Find(o => o.m_SIdx == sidx);
		RES_SHOP_ITEM_INFO info = USERINFO.m_ShopInfo.GetInfos(ShopGroup.DailyPack).Find(o => o.Idx == sidx);
		RES_SHOP_USER_BUY_INFO buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == sidx);
		RES_SHOP_DAILYPACK_INFO packinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == sidx);

		//구매안했거나 3일 이하 남았을때
		if (IsMonthlySell && rginfo != null && (packinfo == null || packinfo?.GetLastTime() * 0.001d <= 86400 * 3)) {
			m_SUI.Monthly.Active.SetActive(true);
			m_SUI.Monthly.Package.SetData(rginfo, (info) => {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Purchase_Monthly, (res, obj) => {
					if (res == 1) {
						SetData(null);
					}
				}, info);
			});
		}
		else m_SUI.Monthly.Active.SetActive(false);
	}
	private void Update() {
		SetActiveAnim();
		//CheckDoubleScrollLock();

		//시즌패스 시간
		var list = USERINFO.m_ShopInfo.PassInfo;
		if(list.Count > 0) {
			var pass = list[0];
			long time = (long)((pass.Times[1] - UTILE.Get_ServerTime_Milli()) * 0.001f);
			m_SUI.Pass.Time.text = TimeSpan.FromSeconds(time).ToString(TDATA.GetString(5028));
		}

		SetSupplyBtn();
		
		double blackmarkettime = double.Parse(PlayerPrefs.GetString($"SHOP_BLACKMERKET_TIME_{USERINFO.m_UID}"));
		double difftime = BLACKMARKET_TIMER - (UTILE.Get_ServerTime_Milli() - blackmarkettime) * 0.001;
		if (difftime < 0) BlackMarketRefresh();
		m_SUI.Blackmarket.Timer[0].text = m_SUI.Blackmarket.Timer[1].text = m_SUI.Blackmarket.Timer[2].text = UTILE.GetSecToTimeStr(difftime);

		//시작점이 있을 경우 오브젝트 활성화되면 수행해줌
		if (gameObject.activeInHierarchy && m_StartPosCor != null) {
			StartCoroutine(m_StartPosCor);
			m_StartPosCor = null;
		}
	}
	void SetActiveAnim() {
		for (ShopGroup i = ShopGroup.Pass; i < ShopGroup.Monthly; i++) {
			if (i == ShopGroup.Event_BlackMarket) continue;
			if (m_SUI.ObjGroup[(int)i] == null) continue;

			float ypos = m_SUI.ObjGroup[(int)i].transform.position.y;
			if (ypos <= Screen.height * 0.8f && ypos >= Screen.height * 0.25f) {
				if (!m_SUI.ObjCenter[(int)i]) {
					m_SUI.ObjCenter[(int)i] = true;
					m_SUI.ObjGroup[(int)i].GetComponent<Animator>().SetTrigger("OnStart");
				}
			}
			else {
				if (m_SUI.ObjCenter[(int)i]) {
					m_SUI.ObjCenter[(int)i] = false;
					m_SUI.ObjGroup[(int)i].GetComponent<Animator>().SetTrigger("Off");
				}
			}
		}
		////패키지는 따로
		//for(int i = 0; i < m_SUI.Recommand.Bucket.childCount; i++) {
		//	Transform child = m_SUI.Recommand.Bucket.GetChild(i);
		//	if (!child.gameObject.activeSelf) continue;
		//	Item_GoodsBanner_Store goods = child.GetComponent<Item_GoodsBanner_Store>();
		//	float ypos = goods.transform.position.y;
		//	if (ypos <= Screen.height * 0.75f && ypos >= Screen.height * 0.25f) {
		//		if (!goods.m_ShopCenter) {
		//			goods.m_ShopCenter = true;
		//			goods.SetAnim("OnStart");
		//		}
		//	}
		//	else {
		//		if (goods.m_ShopCenter) {
		//			goods.m_ShopCenter = false;
		//			goods.SetAnim("Off");
		//		}
		//	}
		//}
	}
	public void StartPos(ShopGroup group, bool _usetw = false, Action _cb = null) {
		m_StartPosCor = StartPosDelay(group, _usetw, _cb);
	}
	IEnumerator StartPosDelay(ShopGroup group, bool _usetw = false, Action _cb = null) {
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(GetComponent<Animator>(), 0.2f));
		yield return new WaitWhile(() => !m_SUI.Scroll.content.gameObject.activeSelf);
		yield return new WaitForEndOfFrame();

		int pos = (int)group;
		Canvas.ForceUpdateCanvases();
		if (m_SUI.ObjGroup[pos] != null) {
			float y = Mathf.Abs(((RectTransform)m_SUI.ObjGroup[pos].transform).anchoredPosition.y) - ((RectTransform)m_SUI.ObjGroup[pos].transform).rect.height;
			float time = Mathf.Abs((y - m_SUI.Scroll.content.anchoredPosition.y) / m_SUI.Scroll.content.rect.height * 6f);
			if (_usetw) {
				iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.content.anchoredPosition.y, "to", y, "onupdate", "TW_Scrolling", "time", time));
				yield return new WaitForSeconds(time);
				_cb?.Invoke();
			}
			else {
				m_SUI.Scroll.content.anchoredPosition = new Vector2(0f, y);
				_cb?.Invoke();
			}
		}
	}
	void TW_Scrolling(float _amount) {
		m_SUI.Scroll.content.anchoredPosition = new Vector2(0f, _amount);
	}
	/// <summary> 아이템 구매 </summary>
	public void ItemBuy(TShopTable _table, bool popup = false) {
		int[] preticketcnt = new int[2] { USERINFO.GetItemCount(BaseValue.CHAR_GACHA_TICKET_IDX), USERINFO.GetItemCount(BaseValue.EQ_GACHA_TICKET_IDX) };

		USERINFO.ITEM_BUY(_table.m_Idx, 1, (res) => {
#if NOT_USE_NET
			PlayEffSound(SND_IDX.SFX_1010);

			//FireBase-Analytics
			MAIN.GoldToothStatistics(GoldToothContentsType.Shop, _table.m_Idx);

			//지급
			switch (_table.m_Group) {
				case ShopGroup.Gacha: 
					PickDraw(_table);
					break;
				case ShopGroup.ItemGacha:
					PickDraw(_table);
					//PickItemDraw(_table);
					break;
				case ShopGroup.BlackMarket:
					USERINFO.InsertItem(_table.m_Rewards[0].m_ItemIdx, _table.m_Rewards[0].m_ItemCnt);
					RES_SHOP_USER_BUY_INFO info = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == _table.m_Idx);
					if (info == null) USERINFO.m_ShopInfo.BUYs.Add(new RES_SHOP_USER_BUY_INFO() {
						Idx = _table.m_Idx,
						Cnt = 1,
						UTime = (long) UTILE.Get_ServerTime_Milli()
					});
					else {
						info.Cnt += 1;
						info.UTime = (long) UTILE.Get_ServerTime_Milli();
					}
					USERINFO.SetShopInfo();
					Item_Store_BlackMarket_Element element = m_BlackMarketProduct.Find(o => o.m_TData == _table);
					if (element != null) element.SetData(_table, MAIN.GetRewardBase(_table, RewardKind.Item)[0], true, ClickBlackMarket);
					MAIN.SetRewardList(new object[] { new List<RES_REWARD_BASE>() { MAIN.GetRewardBase(_table, RewardKind.Item)[0] } }, null);
					break;
				case ShopGroup.None:
					if (_table.m_Idx == m_TBlackMarketRF.m_Idx || _table.m_Idx == m_TAdsBlackMarketRF.m_Idx) {
						BlackMarketRefresh();
					}
					break;
				default: 
					USERINFO.InsertItem(_table.m_Rewards[0].m_ItemIdx, _table.m_Rewards[0].m_ItemCnt);
					MAIN.SetRewardList(new object[] { new List<RES_REWARD_BASE>() { MAIN.GetRewardBase(_table, RewardKind.Item)[0] } }, null);
					break;
			}
			//재화 소모
			switch (_table.m_PriceType) {
				case PriceType.Cash: USERINFO.GetCash(-_table.GetPrice()); break;
				case PriceType.Money: USERINFO.ChangeMoney(-_table.GetPrice()); break;
				case PriceType.AD: break;
				case PriceType.Pay:break;
				case PriceType.Energy:USERINFO.GetShell(-_table.GetPrice()); break;
			}
			if (_table.m_Group == ShopGroup.BlackMarket) USERINFO.Check_Mission(MissionType.BlackMarket, 0, 0, 1);
			//서버로 데이터 보내기
			MAIN.Save_UserInfo();
			SetData(null);
#else
			switch (_table.m_Group) {
				case ShopGroup.Pass:
					PlayEffSound(SND_IDX.SFX_1010);
					ClickPass();
					break;
				default:
					SuccessBuy(res, _table);
					break;
			}

			DLGTINFO?.f_RFCharTicketUI?.Invoke(USERINFO.GetItemCount(BaseValue.CHAR_GACHA_TICKET_IDX), preticketcnt[0]);
			DLGTINFO?.f_RFItemTicketUI?.Invoke(USERINFO.GetItemCount(BaseValue.EQ_GACHA_TICKET_IDX), preticketcnt[1]); 
			DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);
#endif
		}, popup);
	}

	void SuccessBuy(RES_BASE res, TShopTable _table) {
		//FireBase-Analytics
		MAIN.GoldToothStatistics(GoldToothContentsType.Shop, _table.m_Idx);

		if (res.Rewards == null) return;

		switch (_table.m_Group) {
			case ShopGroup.Gacha:
				var items = res.GetRewards().Select(o => {
					if (o.Type == Res_RewardType.Char) {
						RES_REWARD_CHAR info = (RES_REWARD_CHAR)o;
						return new OpenItem() { m_Type = OpenItemType.Character, m_Idx = info.Idx, m_Grade = new int[2] { info.Grade, info.Grade } };
					}
					else if (o.Type == Res_RewardType.Item) {
						RES_REWARD_ITEM info = (RES_REWARD_ITEM)o;
						return new OpenItem() { m_Type = OpenItemType.Item, m_Idx = info.Idx, m_Cnt = info.Cnt, m_Grade = new int[2] { info.Grade, info.Grade } };
					}
					else {
						RES_REWARD_MONEY info = (RES_REWARD_MONEY)o;
						return new OpenItem() { m_Type = OpenItemType.Mileage, m_Idx = info.GetIdx(), m_Cnt = info.Add, m_Grade = new int[2] { info.GetGrade(), info.GetGrade() } };
					}
				}).ToList();
				OpenItem chmileage = items.Find(o => o.m_Type == OpenItemType.Mileage);
				if(chmileage != null) items.Remove(chmileage);

				Action<int, TShopTable> rcb = (res, tdata) => { 
					if (res == 1) {
						ItemBuy(tdata);
					}
				};
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.CharDraw, (result, obj) => {
					if(result != 1)
						SetData(null);
					//if (result == 1) {//연차 다시
					//	ItemBuy(obj.GetComponent<CharDraw>().GetTable);
					//}
					//else {
					//	SetData(null);
					//}
					if (chmileage != null) POPUP.Set_MsgBox(PopupName.Msg_Store_GatchaMileage_Alarm, (btn, obj) => { }, chmileage.m_Cnt);
				}, items, _table, rcb);
				break;
			case ShopGroup.ItemGacha:
				List<RES_REWARD_BASE> rewards = res.GetRewards();
				RES_REWARD_MONEY eqmileage = (RES_REWARD_MONEY)rewards.Find(o => o.Type == Res_RewardType.Mileage);
				if(eqmileage != null) rewards.Remove(eqmileage);
				rcb = (res, tdata) => {
					if (res == 1) {
						ItemBuy(tdata);
					}
				};
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_EquipGachaResult, (result, obj) => {
					SetData(null);
					if (result == 0) {
						m_SUI.ItemGacha.Goods.SetItemGachaLv();
					}
					if(eqmileage != null) POPUP.Set_MsgBox(PopupName.Msg_Store_GatchaMileage_Alarm, (btn, obj) => { }, eqmileage.Add);
				}, rewards, _table, rcb);
				break;
			case ShopGroup.SupplyBox:
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_SupplyBoxOpen, (result, obj) => {
					SupplyBoxUI();
					POPUP.GetMainUI().GetComponent<Main_Play>().MenuAlarmRefresh();
					CheckAlarm(Shop.Tab.Shop);
					SetData(null);
				}, res.GetRewards(), m_FreeSupplyGrade);
				break;
			case ShopGroup.BlackMarket:
				Item_Store_BlackMarket_Element element = m_BlackMarketProduct.Find(o => o != null && o.m_TData.m_Idx == _table.m_Idx);
				if (element != null) element.SetData(_table, MAIN.GetRewardBase(_table, RewardKind.Item)[0], true, ClickBlackMarket);
				MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
					SetData(null);
				});
				break;
			case ShopGroup.None:
				if (_table.m_Idx == m_TBlackMarketRF.m_Idx || _table.m_Idx == m_TAdsBlackMarketRF.m_Idx) BlackMarketRefresh();
				break;
			case ShopGroup.Pass:
				SeasonPassUI();
				break;
			//case ShopGroup.Package:
			//	Shop shop = ((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
			//	((Item_Store_Tab_Package)shop.GetPanel(Shop.Tab.Shop)).SetData(null);
			//	break;
			default:
				MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
					SetData(null);
				});
				break;
		}
		Is_Buying = false;
	}
	class GachaRewardCli
	{
		public List<OpenItem> openitems = new List<OpenItem>();
		public List<RES_REWARD_BASE> openitem = new List<RES_REWARD_BASE>();
		public bool CharGacha = false;

		public void Add(GachaRewardCli _reward) {
			openitems.AddRange(_reward.openitems);
			openitem.AddRange(_reward.openitem);
		}
	}
	/// <summary> 뽑기 아이템 추출 클라용 </summary>
	void PickDraw(TShopTable _shoptable) {
		GachaRewardCli reward = new GachaRewardCli();
		if (_shoptable.m_Group == ShopGroup.Gacha) {
			reward = PickProcess(TDATA.GetItemTable(_shoptable.m_Rewards[0].m_ItemIdx).m_Value, _shoptable.m_Rewards[0].m_ItemCnt);
			if (_shoptable.m_Rewards.Count > 1 && _shoptable.m_Rewards[1].m_ItemCnt != 0) {
				reward.Add(PickProcess(TDATA.GetItemTable(_shoptable.m_Rewards[1].m_ItemIdx).m_Value, _shoptable.m_Rewards[1].m_ItemCnt));
			}
		}
		else if(_shoptable.m_Group == ShopGroup.ItemGacha) {
			reward = PickProcess(TDATA.GetEquipGachaTable(USERINFO.GetEquipGachaLv().GetLv).m_Gid, _shoptable.m_Rewards[0].m_ItemCnt);
		}
		if (reward.CharGacha) {
			Action<int, GameObject> rcb = (res, obj) => {
				if (res == 1) {
					ItemBuy(obj.GetComponent<CharDraw>().GetTable);
				}
			};
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.CharDraw, (result, obj) => {
				//if (result == 1) {//10연차 다시
				//	ItemBuy(obj.GetComponent<CharDraw>().GetTable);
				//}
			}, reward.openitems, _shoptable, rcb);
		}
		else POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_EquipGachaResult, (result, obj) => {
#if NOT_USE_NET
			USERINFO.m_ShopEquipGachaExp += obj.GetComponent<Store_EquipGachaResult>().GetTable.m_Rewards[0].m_ItemIdx;
			MAIN.Save_UserInfo();
#endif
			if (result == 1) {//연차 다시\
				ItemBuy(obj.GetComponent<Store_EquipGachaResult>().GetTable);
			}
			else m_SUI.ItemGacha.Goods.SetItemGachaLv();
		}, reward.openitem, _shoptable);
	}
	//가챠에서 템 뽑는것 클라용
	GachaRewardCli PickProcess(int idx, int cnt) {
		GachaRewardCli reward = new GachaRewardCli();

		for (int i = 0; i < cnt; i++) {
			// GroupID

			TGachaGroupTable group = TDATA.GetGachaGroupTable(idx);
			switch (group.MRewardKind) {
				case RewardKind.Character:
					reward.CharGacha = true;
					CharInfo info = USERINFO.m_Chars.Find(t => t.m_Idx == group.m_RewardIdx);
					if (info == null) {//미보유 캐릭터이면
						info = USERINFO.InsertChar(group.m_RewardIdx, group.m_RewardGrade);
						reward.openitems.Add(new OpenItem() { m_Type = OpenItemType.Character, m_Idx = group.m_RewardIdx, m_Grade = new int[2] { info.m_Grade, info.m_Grade } });
					}
					else {//이미 보유중이면 더 높은 등급 나오면 등급만 올림 이하 등급이면 조각으로
						if (info.m_Grade >= group.m_RewardGrade) {
							USERINFO.InsertItem(info.m_TData.m_PieceIdx, BaseValue.STAR_OVERLAP(group.m_RewardGrade));
							reward.openitems.Add(new OpenItem() { m_Type = OpenItemType.Item, m_Idx = info.m_TData.m_PieceIdx, m_Cnt = BaseValue.STAR_OVERLAP(group.m_RewardGrade), m_Grade = new int[2] { info.m_Grade, info.m_Grade } });
						}
						else {
							int pregrade = info.m_Grade;
							info.m_Grade = group.m_RewardGrade;
							reward.openitems.Add(new OpenItem() { m_Type = OpenItemType.Character, m_Idx = group.m_RewardIdx, m_Grade = new int[2] { pregrade, info.m_Grade } });
						}
					}
					break;
				case RewardKind.Item:
					reward.CharGacha = false;
					if (TDATA.GetItemTable(group.m_RewardIdx).m_Type == ItemType.RandomBox) {//박스는 바로 까서 주기
						List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
						TItemTable itemTable = TDATA.GetItemTable(group.m_RewardIdx);
						for (int j = group.m_RewardCount - 1; j > -1; j--) rewards.AddRange(TDATA.GetGachaItem(itemTable));
						for (int j = 0; j < rewards.Count; j++) {
							// 캐릭터 보상은 없음
							if (rewards[j].Type == Res_RewardType.Char) continue;
							RES_REWARD_ITEM item = (RES_REWARD_ITEM)rewards[j];
							reward.openitem.Add(rewards[j]);
						}
					}
					else {
						ItemInfo iteminfo = USERINFO.InsertItem(group.m_RewardIdx, group.m_RewardCount);
						TItemTable tdata = TDATA.GetItemTable(group.m_RewardIdx);
						RES_REWARD_MONEY rmoney;
						RES_REWARD_ITEM ritem;
						switch (tdata.m_Type) {
							case ItemType.Dollar:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Money;
								rmoney.Befor = USERINFO.m_Money - group.m_RewardCount;
								rmoney.Now = USERINFO.m_Money;
								rmoney.Add = group.m_RewardCount;
								reward.openitem.Add(rmoney);
								break;
							case ItemType.Cash:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Cash;
								rmoney.Befor = USERINFO.m_Cash - group.m_RewardCount;
								rmoney.Now = USERINFO.m_Cash;
								rmoney.Add = group.m_RewardCount;
								reward.openitem.Add(rmoney);
								break;
							case ItemType.Energy:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Energy;
								rmoney.Befor = USERINFO.m_Energy.Cnt - group.m_RewardCount;
								rmoney.Now = USERINFO.m_Energy.Cnt;
								rmoney.Add = group.m_RewardCount;
								rmoney.STime = (long)USERINFO.m_Energy.STime;
								reward.openitem.Add(rmoney);
								break;
							case ItemType.InvenPlus:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Inven;
								rmoney.Befor = USERINFO.m_InvenSize - group.m_RewardCount;
								rmoney.Now = USERINFO.m_InvenSize;
								rmoney.Add = group.m_RewardCount;
								reward.openitem.Add(rmoney);
								break;
							default:
								ritem = new RES_REWARD_ITEM();
								ritem.Type = Res_RewardType.Item;
								ritem.UID = iteminfo.m_Uid;
								ritem.Idx = group.m_RewardIdx;
								ritem.Cnt = group.m_RewardCount;
								reward.openitem.Add(ritem);
								break;
						}
						break;
					}
					break;
				case RewardKind.Zombie:
					ZombieInfo zombieInfo = USERINFO.InsertZombie(group.m_RewardIdx);
					RES_REWARD_ZOMBIE zombie = new RES_REWARD_ZOMBIE();
					zombie.UID = zombieInfo.m_UID;
					zombie.Idx = zombieInfo.m_Idx;
					zombie.Grade = zombieInfo.m_Grade;
					reward.openitem.Add(zombie);
					break;
				case RewardKind.DNA:
					TDnaTable dnaTable = TDATA.GetDnaTable(group.m_RewardIdx);
					DNAInfo dnaInfo = new DNAInfo(dnaTable.m_Idx);
					USERINFO.m_DNAs.Add(dnaInfo);
					RES_REWARD_DNA dna = new RES_REWARD_DNA();
					dna.UID = dnaInfo.m_UID;
					dna.Idx = dnaInfo.m_Idx;
					dna.Grade = dnaInfo.m_Grade;
					reward.openitem.Add(dna);
					break;
			}
		}
		return reward;
	}
	/// <summary> 캐릭터 뽑기 </summary>
	public void ClickDraw(int _pos) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 3)) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy_Gacha, _pos)) return;
		string msg = Utile_Class.StringFormat(TDATA.GetString(790), BaseValue.GetPriceTypeName(m_TGachas[_pos].m_PriceType, m_TGachas[_pos].m_PriceIdx)); //string msg = m_TGachas[_pos].m_PriceType == PriceType.Item ? TDATA.GetItemTable(m_TGachas[_pos].m_PriceIdx).GetName() : TDATA.GetString(122);
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, msg, (result, obj) => {
			if (result == 1) {
				if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
					///아이템 획득 연출
					ItemBuy(m_TGachas[_pos]);
				}
				else {
					POPUP.StartLackPop(m_TGachas[_pos].GetPriceIdx());
				}
			}
		}, m_TGachas[_pos].m_PriceType, m_TGachas[_pos].m_PriceIdx, m_TGachas[_pos].GetPrice(), false);
		if (TUTO.IsTuto(TutoKind.ShopSupplyBox, (int)TutoType_ShopSupplyBox.Select_10Gacha)) TUTO.Next();
	}
	public void ClickGacha(TShopTable _tdata) {
		//if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 3)) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy_Gacha, _tdata.m_Group != ShopGroup.Gacha ? 2 : (_tdata.m_Rewards[0].m_ItemCnt > 1 ? 0 : 1))) return;
		if (_tdata.m_Idx == BaseValue.ADSGACHAONE_SHOP_IDX) {
			ADS.ShowAds((result)=>{
				if (result == IAds.ResultCode.Succecss) {
#if NOT_USE_NET
					ItemBuy(_tdata);
					if (m_AdsGachaOneInfo == null) {
						USERINFO.m_ShopInfo.BUYs.Add(new RES_SHOP_USER_BUY_INFO() {
							Idx = BaseValue.ADSGACHAONE_SHOP_IDX,
							UTime = (long)UTILE.Get_ServerTime_Milli()
						});
					}
					else m_AdsGachaOneInfo.UTime = (long)UTILE.Get_ServerTime_Milli();
					USERINFO.SetShopInfo();
#else
					ItemBuy(_tdata);
#endif
				}
			});
		}
		else {
			int getcnt = USERINFO.GetItemCount(_tdata.m_PriceIdx);
			int needcnt = _tdata.GetPrice();
			bool canbuy = getcnt >= needcnt;
			string msg = string.Empty;
			if (canbuy) {
				msg = Utile_Class.StringFormat(TDATA.GetString(790), BaseValue.GetPriceTypeName(_tdata.m_PriceType, _tdata.m_PriceIdx)); //_tdata.m_PriceType == PriceType.Item ? TDATA.GetItemTable(_tdata.m_PriceIdx).GetName() : TDATA.GetString(122);
				POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, msg, (result, obj) => {
					if (result == 1) {
						if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
							///아이템 획득 연출
							ItemBuy(_tdata);
						}
						else {
							POPUP.StartLackPop(_tdata.GetPriceIdx());
						}
					}
				}, _tdata.m_PriceType, _tdata.m_PriceIdx, _tdata.GetPrice(), false);
			}
			else {
				TShopTable ticketdata = TDATA.GetShopTable(BaseValue.SHOP_IDX_CHARGACHA_TICKET);
				msg = Utile_Class.StringFormat(TDATA.GetString(1044), BaseValue.GetPriceTypeName(_tdata.m_PriceType, _tdata.m_PriceIdx));
				POPUP.Set_MsgBox(PopupName.Msg_Store_Ticket_Buy, string.Empty, msg, (result, obj) => {
					if (result == 1) {
						if (obj.GetComponent<Msg_Store_Ticket_Buy>().IS_CanBuy) {
							///아이템 획득 연출
							USERINFO.ITEM_BUY(ticketdata.m_Idx, needcnt - getcnt, (res)=> {
								if (res != null && res.IsSuccess()) {
									ItemBuy(_tdata);
								}
							});
						}
						else {
							POPUP.StartLackPop(ticketdata.GetPriceIdx());
						}
					}
				}, ticketdata.m_PriceType, ticketdata.m_PriceIdx, ticketdata.GetPrice(needcnt - getcnt), _tdata.m_PriceIdx, needcnt,false);
			}
			if (TUTO.IsTuto(TutoKind.ShopSupplyBox, (int)TutoType_ShopSupplyBox.Select_10Gacha)) TUTO.Next();
		}
	}
	public void ClickItemGacha(TShopTable _tdata) {
		int val = 3;
		if (_tdata.m_Group != ShopGroup.ItemGacha) val = 3;
		else {
			if (_tdata.m_Rewards[0].m_ItemCnt >= 30) val = 2;
			else if (_tdata.m_Rewards[0].m_ItemCnt >= 10) val = 1;
			else if (_tdata.m_Rewards[0].m_ItemCnt >= 1) val = 0;
		}
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy_ItemGacha, val)) return;

		if (_tdata.m_Idx == BaseValue.ADSITEMGACHAONE_SHOP_IDX) {
			ADS.ShowAds((result) => {
				if (result == IAds.ResultCode.Succecss) {
#if NOT_USE_NET
					ItemBuy(_tdata);
					if (m_AdsItemGachaOneInfo == null) {
						USERINFO.m_ShopInfo.BUYs.Add(new RES_SHOP_USER_BUY_INFO() {
							Idx = BaseValue.ADSITEMGACHAONE_SHOP_IDX,
							UTime = (long)UTILE.Get_ServerTime_Milli()
						});
					}
					else m_AdsItemGachaOneInfo.UTime = (long)UTILE.Get_ServerTime_Milli();
					USERINFO.SetShopInfo();
#else
					ItemBuy(_tdata);
#endif
				}
			});
		}
		else {
			int getcnt = USERINFO.GetItemCount(_tdata.m_PriceIdx);
			int needcnt = _tdata.GetPrice();
			bool canbuy = getcnt >= needcnt;
			string msg = string.Empty;
			if (canbuy) {
				msg = Utile_Class.StringFormat(TDATA.GetString(790), BaseValue.GetPriceTypeName(_tdata.m_PriceType, _tdata.m_PriceIdx));//_tdata.m_PriceType == PriceType.Item ? TDATA.GetItemTable(_tdata.m_PriceIdx).GetName() : TDATA.GetString(122);
				POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, msg, (result, obj) => {
					if (result == 1) {
						if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
							///아이템 획득 연출
							ItemBuy(_tdata);
						}
						else {
							POPUP.StartLackPop(_tdata.GetPriceIdx());
						}
					}
				}, _tdata.m_PriceType, _tdata.m_PriceIdx, _tdata.GetPrice(), false);
			}
			else {
				TShopTable ticketdata = TDATA.GetShopTable(BaseValue.SHOP_IDX_ITEMGACHA_TICKET);
				msg = Utile_Class.StringFormat(TDATA.GetString(1045), BaseValue.GetPriceTypeName(_tdata.m_PriceType, _tdata.m_PriceIdx));
				POPUP.Set_MsgBox(PopupName.Msg_Store_Ticket_Buy, string.Empty, msg, (result, obj) => {
					if (result == 1) {
						if (obj.GetComponent<Msg_Store_Ticket_Buy>().IS_CanBuy) {
							///아이템 획득 연출
							USERINFO.ITEM_BUY(ticketdata.m_Idx, needcnt - getcnt, (res) => {
								if (res != null && res.IsSuccess()) {
									ItemBuy(_tdata);
								}
							});
						}
						else {
							POPUP.StartLackPop(ticketdata.GetPriceIdx());
						}
					}
				}, ticketdata.m_PriceType, ticketdata.m_PriceIdx, ticketdata.GetPrice(needcnt - getcnt), _tdata.m_PriceIdx, needcnt, false);
			}
		}
		if (TUTO.IsTuto(TutoKind.ShopEquipGacha, (int)TutoType_ShopEquipGacha.Select_10Gacha)) TUTO.Next();
	}
	public void ClickGoResearch() {
		Main_Play main = POPUP.GetMainUI().GetComponent<Main_Play>();
		main.MenuChange((int)MainMenuType.PDA, false);
		main.GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>().ClickMenu((int)Item_PDA_Menu.State.Research);
	}
	/// <summary> 보급 상자</summary>
	public void ClickSupplyBox(bool _ads = false) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 4)) return;
		if (Is_Buying) return;
		Is_Buying = true;
		int sidx = 0;
		//월정액, 일반, 광고 순으로 체크
		RES_SHOP_DAILYPACK_INFO monthlypackinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		double monthlytime = MONTHLYSUPPLY_TIME - (UTILE.Get_ServerTime_Milli() - (m_MonthlySupplyBuyInfo == null ? 0 : m_MonthlySupplyBuyInfo.UTime)) * 0.001;
		double normaltime = SUPPLY_TIME - (UTILE.Get_ServerTime_Milli() - (m_SupplyBuyInfo == null ? 0 : m_SupplyBuyInfo.UTime)) * 0.001;
		double adstime = ADSSUPPLY_TIME - (UTILE.Get_ServerTime_Milli() - (m_AdsSupplyBuyInfo == null ? 0 : m_AdsSupplyBuyInfo.UTime)) * 0.001;
		if (monthlytime <= 0 && monthlypackinfo?.GetLastTime() > 0) {
			sidx = BaseValue.MONTHLY_SUPPLYBOX_SHOP_IDX;
			ItemBuy(TDATA.GetShopTable(sidx));
		}
		else if(normaltime <= 0) {
			sidx = BaseValue.NORMAL_SUPPLYBOX_SHOP_IDX;
			ItemBuy(TDATA.GetShopTable(sidx));
		}
		else if(adstime <= 0) {
			sidx = BaseValue.ADS_SUPPLYBOX_SHOP_IDX;
			ADS.ShowAds((result) => {
				if (result == IAds.ResultCode.Succecss)
					ItemBuy(TDATA.GetShopTable(sidx));
				else
					Is_Buying = false;
			});
		}
		if (sidx == 0) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(5005));
			Is_Buying = false;
		}
	}
	public void ClickViewSupplyBoxList() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 9)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Box_Preview, null, m_TSBData, m_FreeSupplyGrade);
	}
	/// <summary> 추천 상품 </summary>
	public void ClickRecommand() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 1)) return;
		POPUP.Set_MsgBox(PopupName.Msg_CommingSoon, string.Empty, TDATA.GetString(185));
	}
	public void ClickBlackMarket(TShopTable _tdata) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 5)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_PurchaseConfirm, (res, obj) => {
			if (res == 1) {
				if (obj.GetComponent<Store_PurchaseConfirm>().IS_CanBuy) {
					///아이템 획득 연출
					ItemBuy(_tdata);
				}
				else {
					POPUP.StartLackPop(_tdata.GetPriceIdx());
				}
			}
		}, MAIN.GetRewardBase(_tdata, RewardKind.Item)[0], _tdata, false);
	}
	void BlackMarketRefresh() {
		USERINFO.InitBlackMarket(true);
		BlackMarketUI();
	}
	public void ClickBlackMarketRefresh() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 8)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_RefreshConfirm, (result, obj) => {//0은 취소, 1:현금, 2:광고
			if (result == 1) {
#if NOT_USE_NET
				USERINFO.GetCash(-m_TBlackMarketRF.GetPrice());
				BlackMarketRefresh();
#else
				ItemBuy(m_TBlackMarketRF);
#endif
			}
			else if(result == 2) {
#if NOT_USE_NET
				BlackMarketRefresh();
				if (m_AdsBlackRefreshBuyInfo == null) {
					USERINFO.m_ShopInfo.BUYs.Add(new RES_SHOP_USER_BUY_INFO() {
						Idx = m_TAdsBlackMarketRF.m_Idx,
						UTime = (long)UTILE.Get_ServerTime_Milli()
					});
				}
				if (!UTILE.IsSameDay(m_AdsBlackRefreshBuyInfo.UTime) && m_AdsBlackRefreshBuyInfo.Cnt >= m_TAdsBlackMarketRF.m_LimitCnt) m_AdsBlackRefreshBuyInfo.Cnt = 0;
				else m_AdsBlackRefreshBuyInfo.Cnt++;

				m_AdsBlackRefreshBuyInfo.UTime = (long)UTILE.Get_ServerTime_Milli();
				USERINFO.SetShopInfo();
#else
				ItemBuy(m_TAdsBlackMarketRF);
#endif
			}
			else if(result == 3) {
				//SetData(null);
			}
		}, m_TBlackMarketRF, m_TAdsBlackMarketRF, m_AdsBlackRefreshBuyInfo);
	}
	/// <summary> 경매 </summary>
	public void ClickAction() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 7)) return;
		POPUP.Set_MsgBox(PopupName.Msg_CommingSoon, string.Empty, TDATA.GetString(185));
	}
	/// <summary> 금니 </summary>
	public void ClickCash() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 6)) return;
		POPUP.Set_MsgBox(PopupName.Msg_CommingSoon, string.Empty, TDATA.GetString(185));
	}
	/// <summary> 달러 </summary>
	public void ClickDollar(TShopTable _tdata) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 2)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_PurchaseConfirm, (res, obj) => {
			if (res == 1) {
				if (obj.GetComponent<Store_PurchaseConfirm>().IS_CanBuy) {
					///아이템 획득 연출
					ItemBuy(_tdata);
				}
				else {
					POPUP.StartLackPop(_tdata.GetPriceIdx());
				}
			}
		}, MAIN.GetRewardBase(_tdata, RewardKind.Item)[0], _tdata, false);
	}
	/// <summary> 클라에서만 쓰이고 보상 지급할때 씀 </summary>
	List<RES_REWARD_BASE> ClientGetReward(List<Rewards> _rewards) {
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();

		for (int i = 0; i < _rewards.Count; i++) {
			switch (_rewards[i].m_Type) {
				case RewardKind.None:
					break;
				case RewardKind.Character:
					CharInfo charinfo = USERINFO.m_Chars.Find(t => t.m_Idx == _rewards[i].m_Idx);
					if (charinfo != null) {
						POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(186));
						ItemInfo pieceitem = USERINFO.InsertItem(charinfo.m_TData.m_PieceIdx, BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade));
						rewards.Add(new RES_REWARD_ITEM() {
							Type = Res_RewardType.Item,
							UID = pieceitem.m_Uid,
							Idx = pieceitem.m_Idx,
							Cnt = pieceitem.m_TData.GetEquipType() == EquipType.End ? BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade) : 1,
							result_code = EResultCode.SUCCESS_REWARD_PIECE
						});
					}
					else {
						CharInfo charInfo = USERINFO.InsertChar(_rewards[i].m_Idx);
						RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
						rchar.SetData(charInfo);
						rewards.Add(rchar);
					}
					break;
				case RewardKind.Item:
					TItemTable tdata = TDATA.GetItemTable(_rewards[i].m_Idx);
					if (tdata.m_Type == ItemType.RandomBox || tdata.m_Type == ItemType.AllBox) {//박스는 바로 까서 주기
						TItemTable itemTable = TDATA.GetItemTable(_rewards[i].m_Idx);
						for (int j = _rewards[i].m_Cnt - 1; j > -1; j--) rewards.AddRange(TDATA.GetGachaItem(itemTable));
						for (int j = 0; j < rewards.Count; j++) {
							// 캐릭터 보상은 없음
							if (rewards[j].Type == Res_RewardType.Char) continue;
							rewards.Add(rewards[j]);
						}
					}
					else {
						ItemInfo iteminfo = USERINFO.InsertItem(_rewards[i].m_Idx, _rewards[i].m_Cnt);
						RES_REWARD_MONEY rmoney;
						RES_REWARD_ITEM ritem;
						switch (tdata.m_Type) {
							case ItemType.Exp:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Exp;
								rmoney.Befor = USERINFO.m_Exp[0] - _rewards[i].m_Cnt;
								rmoney.Now = USERINFO.m_Exp[0];
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								rewards.Add(rmoney);
								break;
							case ItemType.Dollar:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Money;
								rmoney.Befor = USERINFO.m_Money - _rewards[i].m_Cnt;
								rmoney.Now = USERINFO.m_Money;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								rewards.Add(rmoney);
								break;
							case ItemType.Cash:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Cash;
								rmoney.Befor = USERINFO.m_Cash - _rewards[i].m_Cnt;
								rmoney.Now = USERINFO.m_Cash;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								rewards.Add(rmoney);
								break;
							case ItemType.Energy:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Energy;
								rmoney.Befor = USERINFO.m_Energy.Cnt - _rewards[i].m_Cnt;
								rmoney.Now = USERINFO.m_Energy.Cnt;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								rmoney.STime = (long)USERINFO.m_Energy.STime;
								rewards.Add(rmoney);
								break;
							case ItemType.InvenPlus:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Inven;
								rmoney.Befor = USERINFO.m_InvenSize - _rewards[i].m_Cnt;
								rmoney.Now = USERINFO.m_InvenSize;
								rmoney.Add = (int)(rmoney.Now - rmoney.Befor);
								rewards.Add(rmoney);
								break;
							default:
								ritem = new RES_REWARD_ITEM();
								ritem.Type = Res_RewardType.Item;
								ritem.UID = iteminfo.m_Uid;
								ritem.Idx = _rewards[i].m_Idx;
								ritem.Cnt = _rewards[i].m_Cnt;
								rewards.Add(ritem);
								break;
						}
						break;
					}
					break;
				case RewardKind.Zombie:
					ZombieInfo zombieInfo = USERINFO.InsertZombie(_rewards[i].m_Idx);
					RES_REWARD_ZOMBIE zombie = new RES_REWARD_ZOMBIE();
					zombie.UID = zombieInfo.m_UID;
					zombie.Idx = zombieInfo.m_Idx;
					zombie.Grade = zombieInfo.m_Grade;
					rewards.Add(zombie);
					break;
				case RewardKind.DNA:
					TDnaTable dnaTable = TDATA.GetDnaTable(_rewards[i].m_Idx);
					DNAInfo dnaInfo = new DNAInfo(dnaTable.m_Idx);
					USERINFO.m_DNAs.Add(dnaInfo);
					RES_REWARD_DNA dna = new RES_REWARD_DNA();
					dna.UID = dnaInfo.m_UID;
					dna.Idx = dnaInfo.m_Idx;
					dna.Grade = dnaInfo.m_Grade;
					rewards.Add(dna);
					break;
			}
		}
		return rewards;
	}
#region Season Pass
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 시즌패스
	public void SeasonPassUI()
	{
#if NOT_USE_NET
		m_SUI.Pass.Active.SetActive(false);
#else
		var passlist = USERINFO.m_ShopInfo.PassInfo;
		if (passlist.Count < 1)
		{
			m_SUI.Pass.Active.SetActive(false);
			return;
		}

		var tpass = passlist[0];
		var buy = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == tpass.Idx);
		// 이미 구매함
		if (buy != null && buy.UTime >= tpass.Times[0])
		{
			m_SUI.Pass.Active.SetActive(false);
			return;
		}
		m_SUI.Pass.Active.SetActive(true);
		m_SUI.Pass.Price.SetData(tpass.Idx);

		var pinfo = USERINFO.m_ShopInfo.PIDs.Find(o => o.Idx == tpass.Idx);
		if (pinfo != null) {
			var price = IAP.GetPrice(pinfo.PID);
			m_SUI.Pass.Discount.text = string.Format("{0}{1}", pinfo.SaleText, TDATA.GetString(5127));
			m_SUI.Pass.DiscoundGroup.SetActive(!string.IsNullOrEmpty(pinfo.SaleText));
		}
		else
			m_SUI.Pass.DiscoundGroup.SetActive(false);
#endif
	}
	/// <summary> 시즌 패스 </summary>
	public void ClickPass()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 0)) return;
		((Main_Play)POPUP.GetMainUI()).GoSeasonPass();
	}


	/// <summary> 시즌 패스 </summary>
	public void BuyPass()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 0)) return;
		var passlist = USERINFO.m_ShopInfo.PassInfo;
		if (passlist.Count < 1)
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(718));
			return;
		}

		var tpass = passlist[0];
		var buy = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == tpass.Idx);
		// 이미 구매함
		if (buy != null && buy.UTime >= tpass.Times[0])
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(718));
			return;
		}

		ItemBuy(TDATA.GetShopTable(USERINFO.m_ShopInfo.PassInfo[0].Idx), true);
	}
#endregion

	public void Click_MileageShop() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopBuy, 10)) return;
		if(USERINFO.m_ShopInfo.NowSeason == null) {
			POPUP.Set_MsgBox(PopupName.Msg_CommingSoon, string.Empty, TDATA.GetString(876));
			return;
		}
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_GatchaMileage, (res, obj) => {
			MileageUI();
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 튜토리얼용
	public GameObject GetGachaBtn() {
		return m_SUI.ObjGroup[(int)ShopGroup.Gacha];
	}

	public override void SetScrollState(bool Active) {
		m_SUI.Scroll.enabled = Active;
	}
}
