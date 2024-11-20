using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stage_Making_New : PopupBase
{
#pragma warning disable 0649
	[SerializeField] Animator m_Ani;
	[SerializeField] Item_Stage_Making_Info m_Info;
	IEnumerator ClosePlay;
	bool m_StartAniPlay;
#pragma warning restore 0649

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		PlayEffSound(SND_IDX.SFX_0100);
		ClosePlay = null;
		m_Info.SetData((TStageMakingTable)aobjValue[0]);
		StartCoroutine(StartAniEndCheck());
	}

	IEnumerator StartAniEndCheck()
	{
		m_StartAniPlay = true;
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Ani));
		m_StartAniPlay = false;
	}

	public void AutoCloase()
	{
		ClosePlay = AutoCloase(0);
		StartCoroutine(ClosePlay);
	}
	IEnumerator AutoCloase(int btn)
	{
		yield return new WaitWhile(() => m_StartAniPlay);
		yield return CloseAni(btn);
		m_Ani.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Ani));
		base.Close((int)btn);
	}

	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_StartAniPlay) return;
		if (ClosePlay != null) return;
		ClosePlay = CloseAni(Result);
		StartCoroutine(ClosePlay);
	}


	public IEnumerator CloseAni(int btn)
	{
		m_Ani.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Ani));

		base.Close(btn);
	}
}
