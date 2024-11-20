using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_SurvStat : ObjMng
{
	public Animator m_Anim;
	public TextMeshProUGUI m_Amount;
	public Item_StatUpDown m_ValUpDown;
	public GameObject m_ValUpDownFont;
	public TextMeshProUGUI[] m_ValUpDownFontTxt;
	public StatType m_StatType;
	public Image m_Gauge;
	public Image m_GaugeBG;
	public Image m_Glow;
	public Image m_Warning;
	public Color[] m_Colors = new Color[3];

	float m_Timer = 0;
	bool m_Hold;
	Main_Stage m_MainUI;
	float m_WarningRatio;

	private void OnEnable() {
		SetUI(m_StatType);
	}
	private void Update() {
		if (m_Hold) {
			m_Timer += Time.deltaTime;
			if(m_Timer > 0.5f) {
				m_Hold = false;
				m_Timer = 0;
				POPUP.GetMainUI().GetComponent<Main_Stage>()?.ShowSuvStatInfo(true, m_StatType);
			}
		}
	}
	public void SetUI(StatType _type) {
		m_StatType = _type;
		m_WarningRatio = TDATA.GetStatusDebuffFirst(STAGEINFO.m_PlayType == StagePlayType.Stage ? STAGEINFO.m_Idx : 0, m_StatType).m_StatVal;
		m_Gauge.sprite = UTILE.LoadImg(string.Format("UI/Gague/Gauge_SV_{0}_Fill", ((int)m_StatType) + 1), "png");
		m_GaugeBG.sprite = UTILE.LoadImg(string.Format("UI/Gague/Gauge_SV_{0}_BG", ((int)m_StatType) + 1), "png");
		m_Glow.sprite = UTILE.LoadImg(string.Format("UI/Gague/Gauge_SV_{0}_BG_Glow", ((int)m_StatType) + 1), "png");
		m_Warning.sprite = UTILE.LoadImg(string.Format("UI/Gague/Gauge_SV_{0}_BG_Warning", ((int)m_StatType) + 1), "png");
		m_Gauge.color = m_Glow.color = m_Colors[(int)m_StatType];

		m_Glow.gameObject.SetActive(false);
		m_Warning.gameObject.SetActive(false);
		m_Anim.SetTrigger("Normal");
		UpOff();
		DownOff();
	}
	/// <summary> 최초 현재값과 최대값 세팅 </summary>
	public void SetData(float _crntval, float _maxval) {
		m_Amount.text = Mathf.RoundToInt(_crntval).ToString();// + "/" + _maxval.ToString();
		m_Gauge.fillAmount = Mathf.Clamp(_crntval / _maxval, 0f, 1f);
		m_Warning.gameObject.SetActive(_crntval / _maxval < m_WarningRatio);
		m_Anim.SetTrigger(_crntval / _maxval < m_WarningRatio ? "Danger" : "Normal");
		Canvas.ForceUpdateCanvases();
	}

	/// <summary> 스탯 갱신, 이전값/현재값/최대값 </summary>
	public void RefreshData(float _crntval, float _preval, float _maxval) {
		if (_preval == _crntval) return;
		int val = Mathf.RoundToInt(_crntval - _preval);
		if (val == 0) return;

		UpOff();
		DownOff();
		m_Glow.gameObject.SetActive(true);
		if (val > 0) {
			m_ValUpDown.gameObject.SetActive(true);
			m_ValUpDown.SetData(val);
			Invoke("UpOff", 1f);
		}
		else {
			m_ValUpDownFont.SetActive(true);
			m_ValUpDownFontTxt[0].text = m_ValUpDownFontTxt[1].text = val.ToString();
			Invoke("DownOff", 1f);
		}
		iTween.StopByName(gameObject, "Count");
		iTween.StopByName(gameObject, "Gauge1");
		iTween.ValueTo(gameObject, iTween.Hash("from", _preval, "to", _crntval, "time", 1f, "easetype", "easeinOutQuad", "onupdate", "TW_Counting", "name", "Count"));
		iTween.ValueTo(gameObject, iTween.Hash("from", _preval / _maxval, "to", _crntval / _maxval, "time", 1f, "easetype", "easeinOutQuad", "onupdate", "TW_Gague", "name", "Gauge1"));

		//m_Amount.text = _crntval.ToString() + "/" + _maxval.ToString();
		//m_Gauge.fillAmount = Mathf.Clamp(_crntval / _maxval, 0f, 1f);
		m_Warning.gameObject.SetActive(_crntval / _maxval < m_WarningRatio);
		m_Anim.SetTrigger(_crntval / _maxval < m_WarningRatio ? "Danger" : "Normal");
		if(_preval / _maxval >= m_WarningRatio &&  _crntval / _maxval < m_WarningRatio) {
			switch (m_StatType) {
				case StatType.Men: PlayEffSound(SND_IDX.SFX_0494); break;
				case StatType.Hyg: PlayEffSound(SND_IDX.SFX_0493); break;
				case StatType.Sat: PlayEffSound(SND_IDX.SFX_0492); break;
			}
		}
	}
	public void ClickDown() {
		m_Hold = true;
		m_Timer = 0;
		POPUP.GetMainUI().GetComponent<Main_Stage>()?.ShowSuvStatInfo(false, m_StatType);
	}
	public void ClickUp() {
		m_Hold = false;
		m_Timer = 0;
		POPUP.GetMainUI().GetComponent<Main_Stage>()?.ShowSuvStatInfo(false, m_StatType);
	}
	/// <summary> RefreshData 트위너에서 호출, 게이지 변화/알림 </summary>
	void UpOff() {
		m_Glow.gameObject.SetActive(false);
		m_ValUpDown.gameObject.SetActive(false);
	}
	void DownOff() {
		m_Glow.gameObject.SetActive(false);
		m_ValUpDownFont.SetActive(false);
	}
	void TW_Gague(float _amount) {
		m_Gauge.fillAmount = _amount;
	}
	void TW_Counting(float _amount) {
		m_Amount.text = Mathf.RoundToInt(_amount).ToString();
	}
}

