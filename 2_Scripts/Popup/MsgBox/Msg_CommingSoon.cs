using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Msg_CommingSoon : MsgBoxBase
{
	[System.Serializable]
	public struct SUI
	{
		public Image Portrait;
		public TextMeshProUGUI Name;
		public Item_Talk_Talk Talk;
		public CanvasGroup CG;
	}
	[SerializeField]
	SUI m_SUI;
	float m_Timer = 0f, m_MaxTime = 0.3f;

	private void Awake() {
		TW_Fade(0f);
	}
	private void Start() {
		if (m_DestroyTime > 0.1f)
			iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "onupdate", "TW_Fade", "oncomplete", "AutoClose", "time", 0.3f, "delay", m_DestroyTime));
		iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "onupdate", "TW_Fade", "time", 0.3f));
	}
	private void Update() {
		if (m_Timer < m_MaxTime) {
			m_Timer += Time.deltaTime;
			m_SUI.CG.alpha = Mathf.Min(1f, m_Timer / m_MaxTime);
		}
	}
	public override void SetMsg(string Title, string Msg) {
		TCharacterTable table = TDATA.GetAllCharacterTable().ElementAt(UTILE.Get_Random(0, TDATA.GetAllCharacterTable().Count)).Value;
		m_SUI.Portrait.sprite = table.GetPortrait();
		m_SUI.Name.text = table.GetCharName();
		m_SUI.Talk.SetData(DialogTalkType.Normal, Msg);
	}
	[SerializeField] private float m_DestroyTime;

   
	void AutoClose() {
		Close();
	}
	void TW_Fade(float _amount) {
		m_SUI.CG.alpha = _amount;
	}
}
