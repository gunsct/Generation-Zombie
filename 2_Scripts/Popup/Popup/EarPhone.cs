using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarPhone : PopupBase
{
	[SerializeField] Animator m_Ani;

	/// <summary> 연출에 의한 클릭 막기 </summary>
	protected bool m_ISAniPlay { get { return Utile_Class.IsAniPlay(m_Ani); } }
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		StartCoroutine(Check());
	}

	IEnumerator Check()
	{
		yield return Utile_Class.CheckAniPlay(m_Ani);
		yield return new WaitWhile(() => !WEB.IS_SendNet());
		Close();
	}

	public void Click_Skip() {
		if (!MAIN.Is_LoadServerConfig) return;
		if (Utile_Class.IsAniPlay(m_Ani, 80f / 432f))
			Utile_Class.AniSkip(m_Ani, 81f);
		else if (Utile_Class.IsAniPlay(m_Ani, 300f / 432f))
			Utile_Class.AniSkip(m_Ani, 301f);
		else if (Utile_Class.IsAniPlay(m_Ani, 430f / 432f))
			Utile_Class.AniSkip(m_Ani, 431f);
	}
}
