using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Challenge_Start_Week : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Ani;
		public Image Blur;
		public TextMeshProUGUI ETime;
	}

	[SerializeField] SUI m_SUI;
	ChallengeInfo m_Info;

	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (ChallengeInfo)aobjValue[0];
		m_SUI.Blur.sprite = (Sprite)aobjValue[1];
		base.SetData(pos, popup, cb, aobjValue);
		PlayEffSound(SND_IDX.SFX_0101);
	}

	public override void SetUI() {
		base.SetUI();
	}

	public void Update() {
		if(m_Info != null) m_SUI.ETime.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, m_Info.GetRemainTime()));
	}

	public override void Close(int Result = 0) {
		if (IS_TouchLock) return;
		if (m_Action != null) return;
		StartCoroutine(m_Action = IE_Close(Result));
	}

	public IEnumerator IE_Close(int Result) {
		m_SUI.Ani.SetTrigger("End");
		yield return Utile_Class.CheckAniPlay(m_SUI.Ani);
		base.Close(Result);
	}
}
