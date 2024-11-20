using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Item_RecommendGoodsBanner : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Transform DotBucket;
		public Transform PageDotPrefab;
		public Transform BannerBucket; //Item_GoodsBanner_Element
		public GameObject TimeGroup;
		public TextMeshProUGUI Time;
		public TextMeshProUGUI Name;
	}
	[SerializeField] SUI m_SUI;
	public const float NEXT_TIME = 7.5f;
	int m_Step = 0;
	List<RecommendGoodsInfo> m_GoodsInfos = new List<RecommendGoodsInfo>();
	List<Item_PageDot_Element> m_Dots = new List<Item_PageDot_Element>();
	Item_GoodsBanner_Element m_Banner;
	private void Update() {
		if (m_SUI.TimeGroup != null && m_SUI.TimeGroup.activeSelf && m_GoodsInfos.Count > m_Step && m_GoodsInfos[m_Step] != null && m_GoodsInfos[m_Step].GetRemainTime > 0) {
			m_SUI.Time.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, m_GoodsInfos[m_Step].GetRemainTime));
		}
	}
	public void SetData(bool _first) {
		m_GoodsInfos.Clear();
		USERINFO.SetRecommendGoods(ShopAdviceCondition.PopUp);
		m_GoodsInfos = USERINFO.GetRecommendGoods(ShopAdviceCondition.PopUp);
		USERINFO.SetRecommendGoods(ShopAdviceCondition.Shop);
		m_GoodsInfos.AddRange(USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop).FindAll(o=>o.m_STData.m_Group != ShopGroup.DailyPack && m_GoodsInfos.Find(t=>t.m_SIdx == o.m_SIdx) == null));

		if (m_GoodsInfos.Count < 1) {
			gameObject.SetActive(false);
			return;
		}
		if (m_GoodsInfos.Count > 1) {
			m_GoodsInfos.Sort((RecommendGoodsInfo _before, RecommendGoodsInfo _after) => {
				return _before.m_STData.m_NoOrProb.CompareTo(_after.m_STData.m_NoOrProb);
			});

			m_SUI.DotBucket.gameObject.SetActive(true);
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

		else {
			m_SUI.DotBucket.gameObject.SetActive(false);
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

		m_Banner = UTILE.LoadPrefab(string.Format("Item/Store/GoodsBanner/{0}", m_GoodsInfos[m_Step].m_SSACTData.m_BannerPrefab), true, m_SUI.BannerBucket).GetComponent<Item_GoodsBanner_Element>();
		m_Banner.SetData(m_GoodsInfos[m_Step], (info) => {
			if (TUTO.TouchCheckLock(TutoTouchCheckType.GoodsBanner)) return;
			if (m_Dots.Count > m_Step) m_Dots[m_Step].GaugePause(true);
			if (m_GoodsInfos.Count > 0) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Recommend, (result, obj) => {
					WEB.SEND_REQ_SHOP_INFO((res) => {
						USERINFO.SetDATA(res);
						SetData(false);
					});
				}, m_GoodsInfos, m_Step);
			}
		});

		if (m_GoodsInfos[m_Step].m_STData.m_Group == ShopGroup.DailyPack) {
			TPackageRewardTable tdata = TDATA.GeTPackageRewardGroupTable(m_GoodsInfos[m_Step].m_STData.m_Idx)[0];
			m_SUI.Name.text = Utile_Class.Remove_RichTextTag(tdata.GetName()).Replace("\n", " ").Replace("\r", " ");
		}
		else {
			TItemTable itemdata = TDATA.GetItemTable(m_GoodsInfos[m_Step].m_STData.m_Rewards[0].m_ItemIdx);
			m_SUI.Name.text = itemdata == null ? Utile_Class.Remove_RichTextTag(m_GoodsInfos[m_Step].m_STData.GetName()).Replace("\n", " ").Replace("\r", " ") 
				: Utile_Class.Remove_RichTextTag(itemdata.GetName()).Replace("\n", " ").Replace("\r", " ");
		}

		m_SUI.TimeGroup.SetActive(m_GoodsInfos[m_Step].m_SSACTData.m_CloseType == ShopAdviceCloseType.Time);
	}
	public void CheckNewGoods() {
		int newgoodsidx = int.Parse(PlayerPrefs.GetString(string.Format("NEW_RECOMMEND_GOODS_{0}", USERINFO.m_UID), "-1"));
		if (newgoodsidx > -1) {
			var newgoods = m_GoodsInfos.Find(o => o.m_SIdx == newgoodsidx);
			if(newgoods == null || newgoods.m_SSACTData == null) return;
			int pos = m_GoodsInfos.IndexOf(newgoods);// m_GoodsInfos.FindIndex(0, o => o.m_SIdx == newgoodsidx);
			if (m_Dots.Count > m_Step) m_Dots[m_Step].GaugePause(true);
			if (m_GoodsInfos.Count > 0 && pos > -1) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Recommend, (result, obj) => {
					WEB.SEND_REQ_SHOP_INFO((res) => {
						USERINFO.SetDATA(res);
						SetData(false);
					});
				}, m_GoodsInfos, pos);
			}
		}
	}
}
