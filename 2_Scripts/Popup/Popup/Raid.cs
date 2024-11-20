using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Raid : PopupBase
{
	[System.Serializable]
	public struct SUI
	{
		public Image EnemyImg;
		public Animator Anim;
	}
	[SerializeField]
	SUI m_SUI;
	bool m_AniPlay;

	/// <summary> aobjValue:0 습격 대상 스프라이트</summary>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		Item_Stage stagecard = (Item_Stage)aobjValue[0];
		if(stagecard != null)
			m_SUI.EnemyImg.sprite = stagecard.m_Info.GetEnemyAlphaImg();
		StartCoroutine(AniCheck());
	}

	public bool ISAniPlay()
	{
		return m_AniPlay;
	}

	/// <summary> 습격 시작, 애니 종료 후 전투 </summary>
	IEnumerator AniCheck()
	{
		m_AniPlay = true;
		m_SUI.Anim.SetTrigger("Start");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		m_AniPlay = false;
	}

	/// <summary> 습격 시작, 애니 종료 후 전투 </summary>
	IEnumerator End()
	{
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		Close();
	}
	/// <summary> 습격 종료 </summary>
	public void OutRaid() {
		StartCoroutine(End());
	}
}
