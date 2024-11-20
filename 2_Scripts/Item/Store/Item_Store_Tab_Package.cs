using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using System.Linq;

public class Item_Store_Tab_Package : Item_Store_Tab_Base
{
	[Serializable]
	public struct SUI //추천 상품
	{
		public Item_GoodsBanner_ChapterClear ChapClear;
		public Transform[] Buckets;	//1:추천,2:일일,3:생존자,4:성장,5:구조,6:충전,7:이벤트
		public GameObject[] Group;	//1:추천,2:일일,3:생존자,4:성장,5:구조,6:충전,7:이벤트
		public GameObject Empty;
	}
	[SerializeField] SUI m_SUI;
	List<RecommendGoodsInfo> m_GoodsInfos = new List<RecommendGoodsInfo>();
	Dictionary<PackageGroupType, List<Item_GoodsBanner_Store>> m_Banners = new Dictionary<PackageGroupType, List<Item_GoodsBanner_Store>>(); 

	public override void SetData(Action CB) {
		Shop shop = ((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
		m_Banners.Clear();
		for (int i = 0; i < m_SUI.Buckets.Length; i++) {
			for(int j = m_SUI.Buckets[i].childCount - 1; j > -1; j--) {
				Destroy(m_SUI.Buckets[i].GetChild(j).gameObject);
			}
		}

		//모든 샵 패키지 상품
		USERINFO.SetRecommendGoods(ShopAdviceCondition.Shop);
		m_GoodsInfos = USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop);
		USERINFO.SetRecommendGoods(ShopAdviceCondition.PopUp);
		m_GoodsInfos.AddRange(USERINFO.GetRecommendGoods(ShopAdviceCondition.PopUp).FindAll(o => m_GoodsInfos.Find(t => t.m_SIdx == o.m_SIdx) == null));
		//챕터 클리어
		List<RecommendGoodsInfo> chapterclear = m_GoodsInfos.FindAll(o => (PackageGroupType)o.m_STData.m_Level == PackageGroupType.ChapterClear && o.m_STData.m_Group == ShopGroup.Package);
		m_SUI.ChapClear.SetData(chapterclear, (info) => {
			if (TUTO.TouchCheckLock(TutoTouchCheckType.GoodsBanner)) return;
			if (info.m_STData.m_Group == ShopGroup.DailyPack) {
				RecommendGoodsInfo rginfo = m_GoodsInfos.Find(o => o.m_SIdx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
				RES_SHOP_DAILYPACK_INFO monthlypackinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
				if (monthlypackinfo == null || monthlypackinfo?.GetLastTime() <= 0) {
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Purchase_Monthly, (res, obj) => {
						if (res == 1) {
							WEB.SEND_REQ_SHOP_INFO((res) => {
								USERINFO.SetDATA(res);
								SetData(null);
							});
						}
					}, info);
				}
			}
			else {
				if (m_GoodsInfos.Count > 0) {
					int[] preticketcnt = new int[2] { USERINFO.GetItemCount(BaseValue.CHAR_GACHA_TICKET_IDX), USERINFO.GetItemCount(BaseValue.EQ_GACHA_TICKET_IDX) };
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Recommend, (result, obj) => {
						if (result == 1) {
							DLGTINFO?.f_RFCharTicketUI?.Invoke(USERINFO.GetItemCount(BaseValue.CHAR_GACHA_TICKET_IDX), preticketcnt[0]);
							DLGTINFO?.f_RFItemTicketUI?.Invoke(USERINFO.GetItemCount(BaseValue.EQ_GACHA_TICKET_IDX), preticketcnt[1]);
							WEB.SEND_REQ_SHOP_INFO((res) => {
								USERINFO.SetDATA(res);
								SetData(null);
							});
						}
					}, new List<RecommendGoodsInfo>() { info }, 0);
				}
			}
		});
		//챕터 클리어 이외 비구독형
		if (m_GoodsInfos.Count > 0) {
			m_GoodsInfos.Sort((RecommendGoodsInfo _before, RecommendGoodsInfo _after) => {
				return _before.m_STData.m_NoOrProb.CompareTo(_after.m_STData.m_NoOrProb);
			});

			for (int i = 0; i < m_GoodsInfos.Count; i++) {
				PackageGroupType packagegrup = (PackageGroupType)m_GoodsInfos[i].m_STData.m_Level;
				if (packagegrup == PackageGroupType.ChapterClear) continue;
				if (m_GoodsInfos[i].m_SSACTData == null) continue;
				GameObject obj = UTILE.LoadPrefab(string.Format("Item/Store/GoodsBanner/{0}_Store", m_GoodsInfos[i].m_SSACTData.m_BannerPrefab), true, m_SUI.Buckets[(int)packagegrup - 1]);
				if (obj == null) continue;
				Item_GoodsBanner_Store goods = obj.GetComponent<Item_GoodsBanner_Store>();
				if (!m_Banners.ContainsKey(packagegrup)) m_Banners.Add(packagegrup, new List<Item_GoodsBanner_Store>());
				m_Banners[packagegrup].Add(goods);
				goods.SetData(m_GoodsInfos[i], (info) => {
					if (TUTO.TouchCheckLock(TutoTouchCheckType.GoodsBanner)) return;
					
					if (info.m_STData.m_Group == ShopGroup.DailyPack) {
						RecommendGoodsInfo rginfo = m_GoodsInfos.Find(o => o.m_SIdx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);//USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop)
						RES_SHOP_DAILYPACK_INFO monthlypackinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
						if (monthlypackinfo == null || monthlypackinfo?.GetLastTime() <= 0) {
							POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Purchase_Monthly, (res, obj) => {
								if (res == 1) {
									WEB.SEND_REQ_SHOP_INFO((res) => {
										USERINFO.SetDATA(res);
										SetData(null);
									});
									//USERINFO.ITEM_BUY(info.m_STData.m_Idx, 1, (res) => {
									//	if (res.IsSuccess()) {
									//		MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
									//			SetData(null);
									//		});
									//	}
									//}, true);
								}
							}, info);
						}
					}
					else {
						if (m_GoodsInfos.Count > 0) {
							int[] preticketcnt = new int[2] { USERINFO.GetItemCount(BaseValue.CHAR_GACHA_TICKET_IDX), USERINFO.GetItemCount(BaseValue.EQ_GACHA_TICKET_IDX) };
							POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Recommend, (result, obj) => {
								if (result == 1) {
									DLGTINFO?.f_RFCharTicketUI?.Invoke(USERINFO.GetItemCount(BaseValue.CHAR_GACHA_TICKET_IDX), preticketcnt[0]);
									DLGTINFO?.f_RFItemTicketUI?.Invoke(USERINFO.GetItemCount(BaseValue.EQ_GACHA_TICKET_IDX), preticketcnt[1]);
									WEB.SEND_REQ_SHOP_INFO((res) => {
										USERINFO.SetDATA(res);
										SetData(null);
									});
								}
							}, new List<RecommendGoodsInfo>() { info }, 0);
						}
					}
				});
			}
		}
		//챕터 클리어 이외 구독형 패키지
		for (int i = 0; i < USERINFO.m_ShopInfo.PACKs.Count; i++) {
			//LS_Web.
			RES_SHOP_DAILYPACK_INFO packinfo = USERINFO.m_ShopInfo.PACKs[i];
			RES_SHOP_USER_BUY_INFO buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == packinfo.Idx);
			if (buyinfo != null && packinfo != null && packinfo.GetLastTime() > 0) {
				PackageGroupType packagegrup = (PackageGroupType)TDATA.GetShopTable(buyinfo.Idx).m_Level;
				if (packagegrup == PackageGroupType.ChapterClear) continue;
				RecommendGoodsInfo info = new RecommendGoodsInfo() {
					m_SIdx = buyinfo.Idx,
					m_UTime = buyinfo.UTime
				};
				GameObject obj = UTILE.LoadPrefab(string.Format("Item/Store/GoodsBanner/{0}_Store_2", info.m_SSACTData.m_BannerPrefab), true, m_SUI.Buckets[(int)packagegrup - 1]);
				if (obj == null) continue;
				Item_GoodsBanner_Store goods = obj.GetComponent<Item_GoodsBanner_Store>();
				if (!m_Banners.ContainsKey(packagegrup)) m_Banners.Add(packagegrup, new List<Item_GoodsBanner_Store>());
				m_Banners[packagegrup].Add(goods);
				goods.transform.SetAsFirstSibling();
				goods.SetData(info, null, () => {
					WEB.SEND_REQ_SHOP_INFO((res) => {
						USERINFO.SetDATA(res);
						SetData(null);
						CheckAlarm(Shop.Tab.Package);
					});
				});
			}
		}
		m_SUI.ChapClear.gameObject.SetActive(m_SUI.ChapClear.GetGoodsCnt > 0);
		for(int i = 0;i< m_SUI.Buckets.Length; i++) {
			bool on = m_Banners.ContainsKey((PackageGroupType)(i + 1));
			m_SUI.Buckets[i].gameObject.SetActive(on);
			m_SUI.Group[i].gameObject.SetActive(on);
		}
		
		m_SUI.Empty.SetActive(m_Banners.Values.SelectMany(o=>o).ToList().Count < 1 && chapterclear.Count < 1);
		CB?.Invoke();
	}

	private void Update() {
		//패키지는 따로
		for (int i = 0; i < m_SUI.Buckets.Length; i++) {
			for (int j = 0; j < m_SUI.Buckets[i].childCount; j++) {
				Transform child = m_SUI.Buckets[i].GetChild(j);
				if (!child.gameObject.activeSelf) continue;
				Item_GoodsBanner_Store goods = child.GetComponent<Item_GoodsBanner_Store>();
				float ypos = goods.transform.position.y;
				if (ypos <= Screen.height * 0.9f && ypos >= 0.25f) {
					if (!goods.m_ShopCenter) {
						goods.m_ShopCenter = true;
						goods.SetAnim("OnStart");
					}
				}
				else {
					if (goods.m_ShopCenter) {
						goods.m_ShopCenter = false;
						goods.SetAnim("Off");
					}
				}
			}
		}
	}
}
