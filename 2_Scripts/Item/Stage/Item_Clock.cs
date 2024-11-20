using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Clock : ObjMng
{
	enum State
	{
		Normal,
		Warning
	}
	[SerializeField]
	Animator m_Anim;
	[SerializeField]
	int m_PreClockTime, m_PreClockTurn;
	float m_ClockNumAngle = 210f;
	[SerializeField]
	Transform m_ClockNumberBack, m_ClockNumberFront;
	[SerializeField]
	Image m_ClockTurn10Current, m_ClockTurn10Next, m_ClockTurn1Current, m_ClockTurn1Next, m_ClockWeatherTemp10, m_ClockWeatherTemp1, m_ClockWeatherTempDot1;
	float m_Temporature = 0f;
	int m_PreTemp10, m_PreTemp1, m_PreTempDot1, m_PreTurn100, m_PreTurn10, m_PreTurn1;
	[SerializeField]
	Image m_ClockWeatherCurrent, m_ClockWeatherNext;
	[SerializeField]
	GameObject[] m_HideObj;
	bool m_IsHide;
	[SerializeField]
	Sprite[] m_NumFonts = new Sprite[13];
	bool m_UseColor;
	State m_State = State.Normal;
	bool LowTurn(int _turn) {
		return _turn <= 5;
	}
	IEnumerator m_CorWarning;
	const float CHANGETIME = 0.6f;
	const float CHANGEDELAY = 0f;

	private void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFClockUI += SetData;
			DLGTINFO.f_RFClockChangeUI += RefreshTurnTime;
			DLGTINFO.f_HDClockUI += SetHide;
		}

		m_HideObj[0].SetActive(false);
	}
	private void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFClockUI -= SetData;
			DLGTINFO.f_RFClockChangeUI -= RefreshTurnTime;
			DLGTINFO.f_HDClockUI -= SetHide;
		}
	}
	/// <summary> 시계 세팅 </summary>
	public void SetData(int _turn, int _time, float _delay = 0f) {
		if (m_IsHide) return;
		if (gameObject.activeInHierarchy) {
			m_UseColor = false;
			StartCoroutine(SetTurnCounting(_turn, CHANGETIME, _delay > 0f ? _delay : CHANGEDELAY));
			if (LowTurn(_turn) && STAGE_USERINFO.IS_TurnUse()) {
				SetLowTurnFX(_turn, _time);
			}
			else {
				SetLowTurnFX(-1);
				SetTime(_time == 0 ? 24 : _time, CHANGETIME, _delay > 0f ? _delay : CHANGEDELAY);
			}
		}
	}
	public void RefreshTurnTime(int _turn, int _time) {
		if (m_IsHide) return;
		m_UseColor = true;
		SetTurn(_turn, CHANGETIME, CHANGEDELAY);//_fast ? 0.3f :1.36f, _fast ? 0f : 0.3f
		if (LowTurn(_turn) && STAGE_USERINFO.IS_TurnUse()) {
			SetLowTurnFX(_turn, _time);
		}
		else {
			SetLowTurnFX(-1);
			SetTime(_time == 0 ? 24 : _time, CHANGETIME, CHANGEDELAY);
		}
	}
	/// <summary> 날씨 교체 </summary>
	public void SetWeather(WeatherType _weather, float _timer = 2f, float _delay = 0f) {
		float pretemp = m_Temporature;
		switch (_weather) {
			case WeatherType.Sunny:
				m_ClockWeatherNext.sprite = UTILE.LoadImg("UI/Icon/WD_Icon_00", "png");
				m_Temporature = Temporature(22, 28); 
				break;
			case WeatherType.Cloud:
				m_ClockWeatherNext.sprite = UTILE.LoadImg("UI/Icon/WD_Icon_01", "png");
				m_Temporature = Temporature(16, 22);
				break;
			case WeatherType.Rain:
				m_ClockWeatherNext.sprite = UTILE.LoadImg("UI/Icon/WD_Icon_02", "png");
				m_Temporature = Temporature(10, 16);
				break;
			case WeatherType.Snow:
				m_ClockWeatherNext.sprite = UTILE.LoadImg("UI/Icon/WD_Icon_03", "png");
				m_Temporature = Temporature(0, 10);
				break;
		}
		iTween.ValueTo(gameObject, iTween.Hash("from", pretemp, "to", m_Temporature, "time", _timer, "delay", _delay, "easetype", "easeOutCubic", "onupdate", "ClockWeatherTempChange"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", _timer, "delay", _delay, "easetype", "easeOutCubic", "onupdate", "ClockWeatherNextAlpha"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", _timer, "delay", _delay, "easetype", "easeOutCubic", "onupdate", "ClockWeatherCurrentAlpha"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", -46f, "time", _timer, "delay", _delay, "easetype", "easeOutCubic", "onupdate", "ClockWeatherChange", "oncomplete", "ClockWeatherChangeEnd"));
	}
	
	/// <summary> SetWeather 트위너 호출, 날씨 변경 </summary>
	void ClockWeatherChange(float _amount) {
		m_ClockWeatherCurrent.transform.localPosition = new Vector3(m_ClockWeatherCurrent.transform.localPosition.x, _amount, m_ClockWeatherCurrent.transform.localPosition.z);
		m_ClockWeatherNext.transform.localPosition = new Vector3(m_ClockWeatherNext.transform.localPosition.x, 46f + _amount, m_ClockWeatherNext.transform.localPosition.z);
	}
	/// <summary> SetWeather 트위너 호출, 알파 연출 </summary>
	void ClockWeatherCurrentAlpha(float _amount) {
		m_ClockWeatherCurrent.color = new Color(m_ClockWeatherCurrent.color.r, m_ClockWeatherCurrent.color.g, m_ClockWeatherCurrent.color.b, _amount);
	}
	/// <summary> SetWeather 트위너 호출, 알파 연출 </summary>
	void ClockWeatherNextAlpha(float _amount) {
		m_ClockWeatherNext.color = new Color(m_ClockWeatherNext.color.r, m_ClockWeatherNext.color.g, m_ClockWeatherNext.color.b, _amount);
	}
	/// <summary> SetWeather 트위너 호출, 날씨 연출 종료 </summary>
	void ClockWeatherChangeEnd() {//원래 자리로 옮기면서 현재날씨를 바뀐 날씨로 교체
		m_ClockWeatherCurrent.sprite = m_ClockWeatherNext.sprite;
		ClockWeatherChange(0f);
		ClockWeatherCurrentAlpha(1f);
	}
	/// <summary> 온도 랜덤 </summary>
	float Temporature(int min, int max) {
		return UTILE.Get_Random(min, max) + UTILE.Get_Random(1, 10) * 0.1f;
	}
	/// <summary> SetWeather 트위너 호출, 온도 연출 </summary>
	void ClockWeatherTempChange(float _amount) {
		if ((int)(_amount / 10f) != m_PreTemp10) {
			m_ClockWeatherTemp10.sprite = m_NumFonts[(int)(_amount / 10f)];
			m_PreTemp10 = (int)(_amount / 10f);
		}
		if ((int)(_amount % 10f) != m_PreTemp1) {
			m_ClockWeatherTemp1.sprite = m_NumFonts[(int)(_amount % 10f)];
			m_PreTemp1 = (int)(_amount % 10f);
		}
		if ((int)((_amount % 1f) * 10f) != m_PreTempDot1) {
			m_ClockWeatherTempDot1.sprite = m_NumFonts[(int)((_amount % 1f) * 10f)];
			m_PreTempDot1 = (int)((_amount % 1f) * 10f);
		}
	}
	/// <summary> 시간 갱신 </summary>
	public void SetTime(int _time, float _timer = 2f, float _delay = 0f) {
		if(Utile_Class.IsPlayiTween(gameObject, "NumRot")) iTween.StopByName(gameObject, "NumRot");

		int movetime = 0;
		if (m_PreClockTime > _time)
			movetime = 24 - m_PreClockTime + _time;
		else if (m_PreClockTime < _time) {
			movetime = _time - m_PreClockTime;
		}
		else return;
			//movetime = _time;
		if (movetime > 1) {
			iTween.ValueTo(gameObject, iTween.Hash("from", m_ClockNumAngle, "to", m_ClockNumAngle - (movetime * 30f + 5f), "time", _timer * 0.9f, "delay", _delay, "easetype", "easeOutCubic", "onupdate", "ClockNumberRot", "name", "NumRot"));
			iTween.ValueTo(gameObject, iTween.Hash("from", m_ClockNumAngle - (movetime * 30f + 5f), "to", m_ClockNumAngle - (movetime * 30f), "time", _timer * 0.1f, "delay", _timer * 0.95f + _delay, "easetype", "easeOutCubic", "onupdate", "ClockNumberRot", "name", "NumRot"));
			//SoundPlayManager.Instance.AddEffectSound(SoundList.SFX_8011.ToString(), _delay + 0.2f);
		}
		else {
			iTween.ValueTo(gameObject, iTween.Hash("from", m_ClockNumAngle, "to", m_ClockNumAngle - movetime * 30f, "time", _timer, "delay", _delay, "easetype", "easeOutCubic", "onupdate", "ClockNumberRot", "name", "NumRot"));
			//SoundPlayManager.Instance.AddEffectSound(SoundList.SFX_8010.ToString(), _delay + 0.2f);
		}

		m_ClockNumAngle -= movetime * 30f;
		m_PreClockTime = _time;
	}
	/// <summary> SetTime 트위너 호출, 시계회전 종료 </summary>
	void ClockNumberRot(float _amount) {
		m_ClockNumberBack.localEulerAngles = new Vector3(0f, 0f, _amount);
		//m_ClockNumberFront.localEulerAngles = new Vector3(0f, 0f, _amount);
	}
	/// <summary> 날짜 변경 연출 </summary>
	public void SetTurn(int _turn, float _timer = 1f, float _delay = 0f) {
		if (_turn < 0) return;
		if (m_PreClockTurn == _turn) return;

		SetTurnColor(m_PreClockTurn, false);
		SetTurnColor(_turn, true);
		if (Utile_Class.IsPlayiTween(gameObject)) {
			ClockTurn1ChangeEnd();
			ClockTurn10ChangeEnd();
		}
		int cnt = Mathf.Abs(m_PreClockTurn - _turn);
		bool up = m_PreClockTurn < _turn;
		if ((int)(_turn % 10) != m_PreTurn1) {//1자리 바뀜
											  //m_ClockTurn1Next.sprite = m_NumFonts[(int)(_turn > 0 ? _turn % 10 : 0)];
			int imgidx = m_PreTurn1;
			for (int i = 0;i< cnt; i++) {
				imgidx += up ? 1 : -1;
				if (imgidx > 9) imgidx = 0;
				else if(imgidx < 0) imgidx = 9;
				iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", -68f, "time", _timer / cnt, "delay", _delay + _timer / cnt * i, "easetype", "easeOutCubic", "onupdate", "ClockTurn1Change", "onstart", "TW_SetTurn1NextImg", "onstartparams", imgidx, "oncomplete", "ClockTurn1ChangeEnd"));
			}
			//iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", -68f, "time", _timer, "delay", _delay, "easetype", "easeOutCubic", "onupdate", "ClockTurn1Change", "oncomplete", "ClockTurn1ChangeEnd"));
			m_PreTurn1 = (int)(_turn % 10);
		}
		if ((int)((_turn % 100) / 10f) != m_PreTurn10) {//10자리 바뀜
			m_ClockTurn10Next.sprite = m_NumFonts[_turn >= 0 ? (int)((_turn % 100) / 10f) : 0];
			iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", -68f, "time", _timer, "delay", _delay, "easetype", "easeOutCubic", "onupdate", "ClockTurn10Change", "oncomplete", "ClockTurn10ChangeEnd"));
			m_PreTurn10 = (int)((_turn % 100) / 10f);
		}
		m_PreClockTurn = _turn;
	}
	void TW_SetTurn1NextImg(int _num) {
		m_ClockTurn1Next.sprite = m_NumFonts[_num];
	}
	IEnumerator SetTurnCounting(int _turn, float _timer = 1f, float _delay = 0f) {
		int count = 0;
		float interval = _timer / (float)_turn;
		while (count <= _turn) {
			SetTurn(count, interval, _delay);
			count++;
			yield return new WaitForSeconds(interval);
		}
	}
	void SetTurnColor(int _turn, bool _next) {
		if (m_UseColor) {
			if (LowTurn(_turn)) {
				if (_next) {
					m_ClockTurn1Next.color = Utile_Class.GetCodeColor("#FF5853");
					m_ClockTurn10Next.color = Utile_Class.GetCodeColor("#9A3028");
				}
				else {
					m_ClockTurn1Current.color = Utile_Class.GetCodeColor("#FF5853");
					m_ClockTurn10Current.color = Utile_Class.GetCodeColor("#9A3028");
				}
			}
			else if (_turn <= 9) {
				if (_next) {
					m_ClockTurn1Next.color = Utile_Class.GetCodeColor("#ffffff");
					m_ClockTurn10Next.color = Utile_Class.GetCodeColor("#898881");
				}
				else {
					m_ClockTurn1Current.color = Utile_Class.GetCodeColor("#ffffff");
					m_ClockTurn10Current.color = Utile_Class.GetCodeColor("#898881");
				}
			}
			else {
				if (_next) {
					m_ClockTurn1Next.color = Utile_Class.GetCodeColor("#ffffff");
					m_ClockTurn10Next.color = Utile_Class.GetCodeColor("#ffffff");
				}
				else {
					m_ClockTurn1Current.color = Utile_Class.GetCodeColor("#ffffff");
					m_ClockTurn10Current.color = Utile_Class.GetCodeColor("#ffffff");
				}
			}
		}
	}
	/// <summary> SetTurn 트위너 호출, 날짜 변경 </summary>
	void ClockTurn10Change(float _amount) {
		m_ClockTurn10Current.transform.localPosition = new Vector3(m_ClockTurn10Current.transform.localPosition.x, _amount, m_ClockTurn10Current.transform.localPosition.z);
		m_ClockTurn10Next.transform.localPosition = new Vector3(m_ClockTurn10Next.transform.localPosition.x, 68f + _amount, m_ClockTurn10Next.transform.localPosition.z);
	}
	/// <summary> SetTurn 트위너 호출, 날짜 변경 </summary>
	void ClockTurn1Change(float _amount) {
		m_ClockTurn1Current.transform.localPosition = new Vector3(m_ClockTurn1Current.transform.localPosition.x, _amount, m_ClockTurn1Current.transform.localPosition.z);
		m_ClockTurn1Next.transform.localPosition = new Vector3(m_ClockTurn1Next.transform.localPosition.x, 68f + _amount, m_ClockTurn1Next.transform.localPosition.z);
	}
	/// <summary> SetTurn 트위너 호출, 날짜 변경 종료 </summary>
	void ClockTurn10ChangeEnd() {
		SetTurnColor(m_PreClockTurn, true);
		SetTurnColor(m_PreClockTurn, false);
		m_ClockTurn10Current.sprite = m_ClockTurn10Next.sprite;
		ClockTurn10Change(0f);
	}
	/// <summary> SetTurn 트위너 호출, 날짜 변경 종료 </summary>
	void ClockTurn1ChangeEnd() {
		SetTurnColor(m_PreClockTurn, true);
		SetTurnColor(m_PreClockTurn, false);
		m_ClockTurn1Current.sprite = m_ClockTurn1Next.sprite;
		ClockTurn1Change(0f);
	}

	public void SetHide(bool _hide) {
		m_IsHide = _hide;
		m_HideObj[0].SetActive(_hide);
	}
	void SetLowTurnFX(int _turn, int _time = 0) {
		if(_turn < 0) {//off
			if(m_State != State.Normal) {
				m_Anim.SetTrigger("Normal");
				if (m_CorWarning != null) {
					StopCoroutine(m_CorWarning);
					m_CorWarning = null;
				}
			}
			m_State = State.Normal;
		}
		else {
			if (m_State != State.Warning) {
				m_Anim.SetTrigger("Warning");
				m_CorWarning = WarningAction(_time);
				StartCoroutine(m_CorWarning);
			}
			m_State = State.Warning;
		}
	}
	IEnumerator WarningAction(int _time) {
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject, "NumRot"));//이전에 하던건 기다리기

		SetTime(_time + 1, 1f, 0f);

		yield return new WaitForSeconds(1f);

		if (STAGEINFO.m_Result != StageResult.None) {
			if (Utile_Class.IsPlayiTween(gameObject, "NumRot")) iTween.StopByName(gameObject, "NumRot");
			yield break;
		}

		m_CorWarning = WarningAction(_time + 1);
		StartCoroutine(m_CorWarning);
	}
	[ContextMenu("TEST")]
	void TimeUP() {
		SetTurn(m_PreClockTurn+ 7, CHANGETIME, CHANGEDELAY);
	}
	[ContextMenu("TEST2")]
	void TimeDown() {
		SetTurn(m_PreClockTurn - 5, CHANGETIME, CHANGEDELAY);
	}
}
