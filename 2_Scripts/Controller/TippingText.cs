using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TippingText : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI m_Txt;
	[SerializeField] TextMeshProEffect m_FX;
	float m_Interval = 0.06f;
	public bool m_End = true;
	Coroutine m_Cor;

	void Init() {
		if(m_Txt == null)
			m_Txt = GetComponent<TextMeshProUGUI>();
		if (m_Txt != null && m_FX == null) {
			m_FX = gameObject.AddComponent<TextMeshProEffect>();
			m_FX.AutoPlay = false;
			m_FX.Repeat = false;
			m_FX.Type = TextMeshProEffect.EffectType.Unfold;
		}
	}
	public void SetData(string _str) {
		Init();
		m_End = false;
		m_Txt.text = _str;
		m_FX.enabled = true;
		m_FX.DurationInSeconds = m_Interval * _str.Length;
		m_FX.Play();
		if (m_Cor != null)
			StopCoroutine(m_Cor);
		m_Cor = StartCoroutine(IE_Skip());
	}
	IEnumerator IE_Skip() {
		yield return new WaitForSeconds(m_FX.DurationInSeconds);
		Skip();
	}
	public void Skip() {
		if (!m_End) {
			if (m_Cor != null)
				StopCoroutine(m_Cor);
			if (m_FX == null) Init();
			m_FX.Finish();
			m_End = true;
		}
	}
}
