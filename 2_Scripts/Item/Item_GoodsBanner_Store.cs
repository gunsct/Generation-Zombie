using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;

public class Item_GoodsBanner_Store : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_GoodsBanner_Element Banner;
		public TextMeshProUGUI RemainDay;
		public GameObject Time;
		public GameObject Extension;
		public GameObject[] PreBuyFx;
		public GameObject[] RecieveOnObj;
		public GameObject[] RecieveOffObj;
		public Item_GoodsBanner_PriceGroup Price;
		public GameObject[] EventOff;
		public GameObject RemainBuyTimeGroup;
		public TextMeshProUGUI RemainBuyTime;
	}
	[SerializeField] SUI m_SUI;
	public bool m_ShopCenter;
	RecommendGoodsInfo m_Info;
	RES_SHOP_USER_BUY_INFO m_BuyInfo;
	RES_SHOP_DAILYPACK_INFO m_PackInfo;
	Action<RecommendGoodsInfo> m_CB;
	Action m_RecieveCB;
	bool Is_Recieve;
	GameObject m_NewMark;
	private void Update() {

		m_SUI.RemainBuyTimeGroup.SetActive(m_Info.GetRemainTime > 0);
		if (m_Info.GetRemainTime > 0) {
			m_SUI.RemainBuyTime.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, m_Info.GetRemainTime));
		}
		if (m_SUI.RemainDay != null && m_PackInfo?.GetLastTime() > 0) {
			if (m_SUI.Time != null) m_SUI.Time.SetActive(m_PackInfo?.GetLastTime() > 86400 * 3);
			m_SUI.RemainDay.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, m_PackInfo.GetLastTime() * 0.001));//TimeSpan.FromSeconds(m_PackInfo.GetLastTime() * 0.001).ToString(TDATA.GetString(5028));
			for (int i = 0; i < m_SUI.RecieveOnObj.Length; i++)
				m_SUI.RecieveOnObj[i].SetActive(m_PackInfo.GetTime() > 0);
			for (int i = 0; i < m_SUI.RecieveOffObj.Length; i++)
				m_SUI.RecieveOffObj[i].SetActive(m_PackInfo.GetTime() <= 0);
			//3일 이하 남으면 연장 표시
			if (m_SUI.Extension != null) m_SUI.Extension.SetActive(m_PackInfo.GetLastTime() * 0.001 <= 259200);
		}
		string[] pricetxt = m_Info.m_STData.GetPriceTxt();
		var pinfo = USERINFO.m_ShopInfo.PIDs.Find(o => o.Idx == m_Info.m_SIdx);
		if (pinfo != null)
		{
			var price = IAP.GetPrice(pinfo.PID);
			pricetxt[0] = string.IsNullOrEmpty(price) ? pinfo.PriceText : price;
			pricetxt[2] = pinfo.SaleText;
		}
		if (m_SUI.Price != null) {
			m_SUI.Price.gameObject.SetActive(m_PackInfo == null || m_PackInfo?.GetLastTime() <= 86400 * 3);
			m_SUI.Price.SetData(pricetxt[0], pricetxt[2]);
		}
		for (int i = 0; i < m_SUI.PreBuyFx.Length; i++) {
			m_SUI.PreBuyFx[i].SetActive(m_PackInfo?.GetLastTime() * 0.001d > 86400 * 3);
		}
	}

	public void SetData(RecommendGoodsInfo _info, Action<RecommendGoodsInfo> _cb, Action _recieveCB = null) {
		m_Info = _info;
		m_CB = _cb;
		m_RecieveCB = _recieveCB;
		m_SUI.Banner.SetData(m_Info, AddCB);

		m_BuyInfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_Info.m_SIdx);
		m_PackInfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == m_Info.m_SIdx);

		//매일매일 받는 상품 시간표기 관련, 표기 안하는 상품은 컴포넌트가 빠져있어 예외처리함
		if(m_SUI.Time != null) m_SUI.Time.SetActive(false);
		
		if(PlayerPrefs.GetInt(string.Format("NEW_STORE_GOODS_{0}_{1}", USERINFO.m_UID, m_Info.m_SIdx), 0) == 1) m_NewMark = UTILE.LoadPrefab(string.Format("Item/Store/GoodsBanner/Item_GoodsBanner_Mark_New"), true, transform);
	}
	void AddCB(RecommendGoodsInfo _info) {
		m_CB?.Invoke(_info);
		if (m_NewMark != null) m_NewMark.SetActive(false);
	}
	public void SetAnim(string _anim) {
		m_SUI.Anim.SetTrigger(_anim);
	}

	/// <summary> 데일리 팩 보상 받기 </summary>
	public void ClickRecieve() {
		if (m_PackInfo != null && m_PackInfo.GetTime() > 0 && m_PackInfo.GetLastTime() > 0) return;
		if (Is_Recieve) return;
		Is_Recieve = true;
		WEB.SEND_REQ_SHOP_GET_DAILYPACKITEM((res) => {
			if (!res.IsSuccess()) {
				Is_Recieve = false;
				return;
			}
			MAIN.SetRewardList(new object[] { res.GetRewards() }, ()=> {
				m_RecieveCB?.Invoke();
				Is_Recieve = false;
			});
		}, m_Info.m_SIdx);
	}
	public void SetEventOff() {
		for(int i = 0;i < m_SUI.EventOff.Length; i++) {
			m_SUI.EventOff[i].SetActive(false);
		}
		
	}
}
