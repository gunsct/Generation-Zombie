using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_SurvStat_PVP : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public SpriteRenderer[] Numbers;	//0:100,1:10,2:0
		public SpriteRenderer[] Icon;       //0:Gauge_SV_{0}_BG, 1:Gauge_SV_{0}_BG_Warning, 2:Gauge_SV_{0}_Fill, 3:Gauge_SV_{0}_BG_Glow
		public Color[] Colors;              //0:³ë¶û¸àÅ»,1:ÆÄ¶ûÀ§»ý,2:ÁÖÈ²Çã±â,3:È¸»ö
		public Sprite[] NumImgs;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] StatType m_Type;
	Material m_GaugeMat;

	private void Awake() {
		m_SUI.Icon[0].sprite = UTILE.LoadImg(string.Format("UI/Gague/Gauge_SV_{0}_BG", (int)m_Type + 1), "png");
		m_SUI.Icon[1].sprite = UTILE.LoadImg(string.Format("UI/Gague/Gauge_SV_{0}_BG_Warning", (int)m_Type + 1), "png");
		m_SUI.Icon[2].sprite = UTILE.LoadImg(string.Format("UI/Gague/Gauge_SV_{0}_Fill", (int)m_Type + 1), "png");
		m_SUI.Icon[2].color = m_SUI.Colors[(int)m_Type];
		m_GaugeMat = m_SUI.Icon[2].material;
		m_SUI.Icon[3].sprite = UTILE.LoadImg(string.Format("UI/Gague/Gauge_SV_{0}_BG_Glow", (int)m_Type + 1), "png");
	}
	public void SetData(float _pre, float _now, float _max, bool _hit = false, bool _use = false) {
		m_SUI.Anim.SetTrigger(_now / _max <= 0.3f ? "Danger" : "Normal");
		m_SUI.Anim.SetTrigger(_use ? "Use" : _hit ? "Hit" : "Idle");
		if (_pre == _now) {
			TW_Counting(_now);
			TW_Gauge(_now / _max);
		}
		else {
			iTween.StopByName(gameObject, "Counting");
			iTween.StopByName(gameObject, "Gauge");
			iTween.ValueTo(gameObject, iTween.Hash("from", _pre, "to", _now, "onupdate", "TW_Counting", "time", 1.08f, "name", "Counting"));
			iTween.ValueTo(gameObject, iTween.Hash("from", _pre / _max, "to", _now / _max, "onupdate", "TW_Gauge", "time", 1.08f, "name", "Gauge"));
		}
	}
	void TW_Counting(float _amount) {
		int h = Mathf.FloorToInt(_amount / 100);
		int t = Mathf.FloorToInt(_amount % 100 / 10);
		int o = Mathf.FloorToInt(_amount % 10);

		m_SUI.Numbers[0].sprite = m_SUI.NumImgs[h];
		m_SUI.Numbers[0].color = _amount >= 100 ? m_SUI.Colors[(int)m_Type] : m_SUI.Colors[3];
		m_SUI.Numbers[1].sprite = m_SUI.NumImgs[t];
		m_SUI.Numbers[1].color = _amount >= 10 ? m_SUI.Colors[(int)m_Type] : m_SUI.Colors[3];
		m_SUI.Numbers[2].sprite = m_SUI.NumImgs[o];
		m_SUI.Numbers[2].color = _amount >= 1 ? m_SUI.Colors[(int)m_Type] : m_SUI.Colors[3];
	}
	void TW_Gauge(float _amount) {
		m_GaugeMat.SetFloat("_FillAmount", _amount);
	}
}
