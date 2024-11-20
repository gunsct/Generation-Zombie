using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using Coffee.UIEffects;

public class Challenge_EndReward_Week : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Ani;

		public Image Blur;
		public ScrollRect Scroll;
		public RectTransform Prefab;
	}

	[SerializeField] SUI m_SUI;

	List<ChallengeInfo> m_Datas;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_Datas = (List<ChallengeInfo>)aobjValue[0];
		m_SUI.Blur.sprite = (Sprite)aobjValue[1];

		PlayEffSound(SND_IDX.SFX_0191);
		// 시작 연출동안 스크롤 막기
		m_SUI.Scroll.enabled = false;
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI()
	{
		base.SetUI();

		int Max = Math.Min(100, m_Datas.Count);
		UTILE.Load_Prefab_List(Max, m_SUI.Scroll.content, m_SUI.Prefab);

		var group = m_SUI.Scroll.content.GetComponent<HorizontalLayoutGroup>();
		var w = m_SUI.Prefab.sizeDelta.x;
		m_SUI.Scroll.content.sizeDelta = new Vector2(group.padding.left + group.padding.right + Max * w + (Max - 1) * group.spacing, m_SUI.Scroll.content.sizeDelta.y);

		for (int i = 0; i < Max; i++)
		{
			var item = m_SUI.Scroll.content.GetChild(i).GetComponent<Item_Challenge_EndReward_Week>();
			item.SetData(m_Datas[i]);
			item.gameObject.SetActive(false);
		}
	}
	public void StartViewListAction()
	{
		StartCoroutine("ViewAction");
	}

	IEnumerator ViewAction()
	{
		var group = m_SUI.Scroll.content.GetComponent<HorizontalLayoutGroup>();
		float x = group.padding.left;
		var PrefabW = m_SUI.Prefab.sizeDelta.x;
		float left = PrefabW + group.spacing;
		float righ = PrefabW + group.padding.right;
		float ViewW = m_SUI.Scroll.viewport.rect.width;
		for (int i = 0, iMax = m_SUI.Scroll.content.childCount; i < iMax; i++)
		{
			var rtf = (RectTransform)m_SUI.Scroll.content.GetChild(i);
			rtf.gameObject.SetActive(true);
			x += left;
			float NextW = x + righ;
			if (i < iMax - 1 && NextW > ViewW)
			{
				iTween.MoveTo(m_SUI.Scroll.content.gameObject, iTween.Hash("position", new Vector3(-(NextW - ViewW), 0, 0), "delay", 0.35f, "islocal", true, "time", 0.31f, "easetype", "easeOutQuad"));
			}
			yield return new WaitForSeconds(0.5f);
		}

		// 스크롤 풀어주기
		m_SUI.Scroll.enabled = true;

		m_SUI.Ani.SetTrigger("ViewBtn");
	}

	public override void Close(int Result = 0)
	{
		if (!m_SUI.Scroll.enabled) return;
		StartCoroutine(IE_Close(Result));
	}

	public IEnumerator IE_Close(int Result)
	{
		m_SUI.Ani.SetTrigger("End");
		yield return Utile_Class.CheckAniPlay(m_SUI.Ani);
		base.Close(Result);
	}
}
