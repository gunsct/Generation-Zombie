using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Challenge_RewardList : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI[] Title;
		public ScrollRect Scroll;
		public RectTransform Prefab;

		public Animator Ani;
	}

	[SerializeField] SUI m_SUI;
	ChallengeMode m_Mode;
	List<ChallengeReward> m_Reward;
	long m_STime;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_Mode = (ChallengeMode)aobjValue[0];
		m_Reward = (List<ChallengeReward>)aobjValue[1];
		base.SetData(pos, popup, cb, aobjValue);

	}
	public override void SetUI()
	{
		base.SetUI();
		var title = TDATA.GetString(m_Mode == ChallengeMode.Week ? 599 : 598);
		for (int i = 0; i < m_SUI.Title.Length; i++) m_SUI.Title[i].text = title;

		int Max = m_Reward.Count;
		UTILE.Load_Prefab_List(Max, m_SUI.Scroll.content, m_SUI.Prefab);

		for (int i = 0; i < Max; i++)
		{
			Item_Challenge_RewardList item = m_SUI.Scroll.content.GetChild(i).GetComponent<Item_Challenge_RewardList>();
			item.SetData(m_Reward[i]);
		}
		
	}

	public override void Close(int Result = 0)
	{
		StartCoroutine(IE_Close(Result));
	}

	public IEnumerator IE_Close(int Result)
	{
		m_SUI.Ani.SetTrigger("Close");
		yield return Utile_Class.CheckAniPlay(m_SUI.Ani);
		base.Close(Result);
	}
}
