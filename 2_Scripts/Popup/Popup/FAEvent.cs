using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;

public class FAEvent : PopupBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public RectTransform InfoPanel;
		public ScrollRect TabScroll;
		public RectTransform TabPrefab;

		public GameObject[] Btns;
	}

	[SerializeField] SUI m_SUI;
	bool IsAutoPlay = false;
	IEnumerator m_ItemAction = null, m_PlayAction = null;
	Item_FAEvent m_LoadEventUI;
	List<MyFAEvent> m_NowEvents = new List<MyFAEvent>();
#pragma warning restore 0649
	void Start()
	{
		if (IsAutoPlay)
		{
			m_PlayAction = AutoPlay();
			StartCoroutine(m_PlayAction);
		}
		else SelectTab(0);
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		IsAutoPlay = (bool)aobjValue[0];
		m_NowEvents = USERINFO.m_Event.GetViewAttEvent();
		m_NowEvents.Sort((befor, after) => befor.UID.CompareTo(after.Times[1]));
		LoadBtn();
		m_SUI.Btns[0].SetActive(false);
		m_SUI.Btns[1].SetActive(false);
		base.SetData(pos, popup, cb, aobjValue);
	}
	void LoadBtn()
	{
		// 리스트 시작순으로 소팅해줌
		UTILE.Load_Prefab_List(m_NowEvents.Count, m_SUI.TabScroll.content, m_SUI.TabPrefab);

		for(int i = 0, iMax = m_NowEvents.Count; i < iMax; i++)
		{
			Item_Tab_Atd tab = m_SUI.TabScroll.content.GetChild(i).GetComponent<Item_Tab_Atd>();
			MyFAEvent data = m_NowEvents[i];
			tab.SetData(data.GetTitle(), i, SelectTab);
			tab.SetActive(false, data.IsReward());
		}
	}

	void SelectTab(int Pos)
	{
		if (m_ItemAction != null || m_PlayAction != null) return;
		SetTabUI(Pos);

		m_ItemAction = LoadItem(Pos);
		StartCoroutine(m_ItemAction);
	}

	void SetTabUI(int pos)
	{
		for (int i = 0, iMax = m_NowEvents.Count; i < iMax; i++)
		{
			Item_Tab_Atd tab = m_SUI.TabScroll.content.GetChild(i).GetComponent<Item_Tab_Atd>();
			tab.SetActive((i == pos), m_NowEvents[i].IsReward());
		}
	}

	IEnumerator LoadItem(int Pos)
	{
		if (m_LoadEventUI != null)
		{
			yield return m_LoadEventUI.PlayAni(Item_FAEvent.State.Out);
			m_LoadEventUI = null;
		}
		MyFAEvent myevent = m_NowEvents[Pos];
		GameObject obj = UTILE.LoadPrefab(string.Format("Item/FAEvent/{0}", myevent.Prefab), true, (Transform)m_SUI.InfoPanel);
		if (obj != null)
		{
			m_LoadEventUI = obj.GetComponent<Item_FAEvent>();
			m_LoadEventUI.SetData(myevent);
			yield return m_LoadEventUI.PlayAni(Item_FAEvent.State.In);
		}
		if(!IsAutoPlay) m_SUI.Btns[1].SetActive(true);
		m_ItemAction = null;
	}

	IEnumerator AutoPlay()
	{
		for (int i = 0, iMax = m_NowEvents.Count; i < iMax; i++)
		{
			MyFAEvent myevent = m_NowEvents[i];
			if (!myevent.IsReward()) continue;
			// 자동 보상
			SetTabUI(i);
			// 로드 및 연출 끝날때까지 대기
			yield return LoadItem(i);
			// 보상 받기
			yield return m_LoadEventUI.RewardAction(null);
			m_SUI.Btns[0].SetActive(true);
			yield return new WaitWhile(() => m_SUI.Btns[0].activeSelf);
		}
		m_PlayAction = null;
		m_SUI.Btns[0].SetActive(true);
	}

	public void OnTouchPress()
	{
		if (m_PlayAction != null)
		{
			m_SUI.Btns[0].SetActive(false);
			return;
		}
		Close(0);
	}

	public override void Close(int Result = 0)
	{
		if (m_PlayAction != null) return;
		base.Close(Result);
	}
}
