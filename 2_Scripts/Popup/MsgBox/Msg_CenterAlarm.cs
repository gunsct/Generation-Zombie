using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Msg_CenterAlarm : MsgBoxBase
{
	[System.Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Msg;
		public CanvasGroup CG;
		public GameObject BG;
	}
	[SerializeField]
	SUI m_SUI;
	//float m_Timer = 0f, m_MaxTime = 0.3f;
	public GameObject GetBG { get { return m_SUI.BG; } }
	private void Awake() {
		TW_Fade(0f);
	}
	IEnumerator Start() {
		if (m_DestroyTime < 0.3f) m_DestroyTime = 0.3f;
		m_DestroyTime -= 0.3f;
		iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "onupdate", "TW_Fade", "time", 0.3f, "ignoretimescale", true));

		if (m_DestroyTime > 0) yield return new WaitForSecondsRealtime(m_DestroyTime);

		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "onupdate", "TW_Fade", "oncomplete", "AutoClose", "time", 0.3f, "ignoretimescale", true));
	}
	public override void SetMsg(string Title, string Msg) {
		m_SUI.Msg.text = Msg;
	}
	[SerializeField] private float m_DestroyTime;

	void AutoClose() {
		Close();
	}
	void TW_Fade(float _amount) {
		m_SUI.CG.alpha = _amount;
	}
}
