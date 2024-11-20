using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Item_RecommendGoodsBanner_Event : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Transform DotBucket;
		public Transform PageDotPrefab;
		public Transform BannerBucket; //Item_GoodsBanner_Element
	}
	[SerializeField] SUI m_SUI;
	public const float NEXT_TIME = 7.5f;
	int m_Step = 0;
	List<RecommendGoodsInfo> m_GoodsInfos = new List<RecommendGoodsInfo>();
	List<Item_PageDot_Element> m_Dots = new List<Item_PageDot_Element>();
	Item_GoodsBanner_Store m_Banner;
	List<int> m_ShopIdxs = new List<int>();
	public void SetData(bool _first, List<int> _idxs) {
		m_ShopIdxs = _idxs;
		List<RecommendGoodsInfo> pregoods = new List<RecommendGoodsInfo>();
		pregoods.AddRange(m_GoodsInfos);

		USERINFO.SetRecommendGoods(ShopAdviceCondition.PopUp);
		m_GoodsInfos = USERINFO.GetRecommendGoods(ShopAdviceCondition.PopUp).FindAll(o=>o.m_STData.m_Level == (int)PackageGroupType.Event && m_ShopIdxs.Contains(o.m_STData.m_Idx));

		if (m_GoodsInfos.Count < 1) {
			gameObject.SetActive(false);
			return;
		}
		m_GoodsInfos.Sort((RecommendGoodsInfo _before, RecommendGoodsInfo _after) => {
			return _before.m_STData.m_NoOrProb.CompareTo(_after.m_STData.m_NoOrProb);
		});


		if (m_GoodsInfos.Count < 2) {
			m_SUI.DotBucket.gameObject.SetActive(false);
		}
		else {
			m_SUI.DotBucket.gameObject.SetActive(true);//
			m_Step = _first ? UTILE.Get_Random(0, m_GoodsInfos.Count) : m_Step > 1 ? m_Step -= 1 : 0;
			m_Dots.Clear();
			UTILE.Load_Prefab_List(m_GoodsInfos.Count, m_SUI.DotBucket, m_SUI.PageDotPrefab);
			for (int i = 0; i < m_GoodsInfos.Count; i++) {
				Item_PageDot_Element dot = m_SUI.DotBucket.GetChild(i).GetComponent<Item_PageDot_Element>();
				dot.GaugeStop();
				dot.SetData(NEXT_TIME, Next, true);
				m_Dots.Add(dot);
			}

			m_Dots[m_Step].SetData(NEXT_TIME, Next);
		}
		StepSet();

	}
	void Next() {
		m_Step++;
		if (m_Step > m_GoodsInfos.Count - 1) m_Step = 0;
		if (m_GoodsInfos.Count < 1) {
			gameObject.SetActive(false);
			return;
		}
		if (m_Dots.Count > m_Step) m_Dots[m_Step].SetData(NEXT_TIME, Next);
		StepSet();
	}
	void StepSet() {
		if (m_Banner != null) Destroy(m_Banner.gameObject);
		if (m_Step > m_GoodsInfos.Count - 1) m_Step = 0;

		m_Banner = UTILE.LoadPrefab(string.Format("Item/Store/GoodsBanner/{0}_Store", m_GoodsInfos[m_Step].m_SSACTData.m_BannerPrefab), true, m_SUI.BannerBucket).GetComponent<Item_GoodsBanner_Store>();
		m_Banner.transform.localPosition = Vector3.zero;
		m_Banner.SetData(m_GoodsInfos[m_Step], (info) => {
			if (TUTO.TouchCheckLock(TutoTouchCheckType.GoodsBanner)) return;
			if (m_Dots.Count > m_Step) m_Dots[m_Step].GaugePause(true);
			if (m_GoodsInfos.Count > 0) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Recommend, (result, obj) => {
					WEB.SEND_REQ_SHOP_INFO((res) => {
						USERINFO.SetDATA(res);
						SetData(false, m_ShopIdxs);
					});
				}, m_GoodsInfos, m_Step);
			}
		});
		m_Banner.SetEventOff();
	}
}
