using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEF_StageCard_Chain : ObjMng
{
#pragma warning disable 0649
	[SerializeField] Item_Clock m_Clock;

	bool m_Presscheck;
	[SerializeField] Vector2[] m_ChainPoss;
	[SerializeField] Vector3[] m_ChainRots;
	[SerializeField] GameObject m_ChainPrefab;
	[SerializeField] RectTransform m_ChainPanel;
#pragma warning restore 0649
	public void SetData(int StartTurn, int StartTime, Action<GameObject> EndCB)
	{
		StartCoroutine(CreateChain(UTILE.Get_Random(3, 6), StartTurn, StartTime, EndCB));
	}

	IEnumerator CreateChain(int Cnt, int StartTurn, int StartTime, Action<GameObject> EndCB)
	{
		SetClockAlpha(0f);
		m_Clock.SetTime(StartTime, 0);
		m_Clock.SetTurn(10, 0);

		float delayTime = 0.8f / (float)Cnt;
		float AddNext = 0;
		Dictionary<int, List<int>> CreateCheck = new Dictionary<int, List<int>>();
		List<Item_PB_Chain> Chains = new List<Item_PB_Chain>();
		for (int i = Cnt; i > 0; i--)
		{
			float delay = UTILE.Get_Random(AddNext, delayTime + AddNext);
			Item_PB_Chain item = Utile_Class.Instantiate(m_ChainPrefab, m_ChainPanel).GetComponent<Item_PB_Chain>();
			Chains.Add(item);
			item.SetData();
			RectTransform rtf = (RectTransform)item.transform;
			int Pos = 0;
			int Rot = 0;
			while(true)
			{
				Pos = UTILE.Get_Random(0, m_ChainPoss.Length);
				if (!CreateCheck.ContainsKey(Pos)) CreateCheck.Add(Pos, new List<int>());
				Rot = UTILE.Get_Random(0, m_ChainPoss.Length);
				if (CreateCheck[Pos].Find((t) => t == Rot + 1) < 1) break;
			}
			CreateCheck[Pos].Add(Rot + 1);
			rtf.anchoredPosition = m_ChainPoss[Pos];
			rtf.eulerAngles = m_ChainRots[Rot];
			yield return new WaitForSeconds(delay);
			AddNext = delayTime + AddNext - delay;
		}


		iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", 0.5f, "easetype", "easeOutQuad", "onupdate", "SetClockAlpha"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

		// 턴 이동해준다.
		m_Clock.SetTime((STAGE_USERINFO.m_Time + StartTurn) % 24, 1.36f);
		m_Clock.SetTurn(0, 1.36f);
		yield return new WaitForSeconds(1.36f);

		float time = 0.12f;
		AddNext = 0;
		delayTime = time / (float)Chains.Count;
		for (int i = Chains.Count - 1; i > -1; i--)
		{
			float delay = UTILE.Get_Random(AddNext, delayTime + AddNext);
			Item_PB_Chain item = Chains[i];
			item.AniAction(Item_PB_Chain.AniName.End);
			yield return new WaitForSeconds(delay);
			AddNext = delayTime + AddNext - delay;
		}

		// 체인 제거 및 시계 알파
		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", 1f, "easetype", "easeOutQuad", "onupdate", "SetAlpha"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		yield return new WaitForSeconds(0.5f);
		EndCB?.Invoke(gameObject);

		Destroy(gameObject);
	}

	void SetAlpha(float _Alpha)
	{
		GetComponent<CanvasGroup>().alpha = _Alpha;
	}
	void SetClockAlpha(float _Alpha)
	{
		m_Clock.GetComponent<CanvasGroup>().alpha = _Alpha;
	}
}
