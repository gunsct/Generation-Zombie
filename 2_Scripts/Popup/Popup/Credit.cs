using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Credit : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Text UserName;
		public GameObject SkipBtn;
	}
	[SerializeField] SUI m_SUI;
	SND_IDX m_NowBG;
	[SerializeField] float m_Spd = 3f;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_SUI.UserName.text = USERINFO.IS_SetName ? USERINFO.m_Name : "You";
		m_SUI.SkipBtn.SetActive(false); 
		TW_SkipAlpha(0f);
		SND.FXMute(true);
		m_NowBG = SND.GetNowBG;
		PlayBGSound(SND_IDX.BGM_9999);
		StartCoroutine(AutoEnd());
	}
	public void OnAcc(bool _acc) {
		m_SUI.Anim.speed = _acc ? m_Spd : 1f;
		if (!m_SUI.SkipBtn.activeSelf) {
			m_SUI.SkipBtn.SetActive(true);
			iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", 1f, "onupdate", "TW_SkipAlpha"));
		}
	}
	void TW_SkipAlpha(float _amount) {
		m_SUI.SkipBtn.GetComponent<CanvasGroup>().alpha = _amount;
	}
	IEnumerator AutoEnd() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		Close();
	}
	public override void Close(int Result = 0) {
		SND.FXMute(false);
		PlayBGSound(m_NowBG);
		base.Close(Result);
	}
}
