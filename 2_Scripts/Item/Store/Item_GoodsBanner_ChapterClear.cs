using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;

public class Item_GoodsBanner_ChapterClear : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public GameObject Element;   //Item_GoodsBanner_ChapterClear_Element
		public SimpleScrollSnap Scroll;
	}
	[SerializeField] SUI m_SUI;
	List<RecommendGoodsInfo> m_Infos = new List<RecommendGoodsInfo>();
	Action<RecommendGoodsInfo> m_CB;
	public int GetGoodsCnt { get { return m_Infos.Count; } }
	public void SetData(List<RecommendGoodsInfo> _infos, Action<RecommendGoodsInfo> _cb) {
		m_CB = _cb;
		m_Infos.Clear();
		for(int i = m_SUI.Scroll.Content.childCount - 1; i > -1; i--) {
			m_SUI.Scroll.Remove(i);
		}
		if (_infos.Count > 0) {
			m_Infos.AddRange(_infos);
			m_Infos.Sort((RecommendGoodsInfo _before, RecommendGoodsInfo _after) => {
				return _before.m_STData.m_Idx.CompareTo(_after.m_STData.m_Idx);
			});
			for (int i = 0; i < m_Infos.Count; i++) {
				m_SUI.Scroll.Add(m_SUI.Element, i);
				Item_GoodsBanner_ChapterClear_Element element = m_SUI.Scroll.Content.GetChild(i).GetComponent<Item_GoodsBanner_ChapterClear_Element>();
				element.SetData(m_Infos[i], m_CB);
			}
			m_SUI.Scroll.GoToPanel(m_Infos.Count - 1);
		}
	}
}
