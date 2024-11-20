using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SceneChange : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Image FadeBG;
		public CanvasGroup CanvasGroup;
	}
	[SerializeField]
	SUI m_SUI;
	bool m_FadeIn;
	float m_Timer;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_FadeIn = (bool)aobjValue[0];
		if (m_FadeIn) TW_Alpha(1f);
		else {
			iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", 0.3f, "easetype", "linear", "onupdate", "TW_Alpha"));
		}
	}
	private void Update() {
		if (!m_FadeIn && m_Timer < 0.3f) {
			m_Timer += Time.deltaTime;
			if(m_Timer > 0.3f)  Close();
		}
	}
	void TW_Alpha(float _amount) {
		m_SUI.CanvasGroup.alpha = _amount;
	}
}
