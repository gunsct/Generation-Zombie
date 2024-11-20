using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using UnityEngine.UI;
using TMPro;

public class Main_PVP : PopupBase
{
	[Serializable]
	public class PlayerInfo
	{
		public Image Portrait;
		public Image NationIcon;
		public Text Name;
		public Slider[] HpGauge;
		public Item_PVP_SupSkill SupSkill;
	}
    [Serializable]
    public struct SUI
	{
		public Animator m_Anim;
		public PlayerInfo[] UserInfos;
		public Image[] Turn;
		public Animator TurnAnim;
		public GameObject TurnGroup;
		public GameObject[] HPGroup;
		public Animator AccBtnAnim;
		public Item_SpeechBubble SpeechBubble;
		public GameObject[] TutoObjs;
	}
	[SerializeField] SUI m_SUI;
	public Animator GetAnim { get { return m_SUI.m_Anim; } }
	public GameObject GetHPBar(int _pos) { 
		return m_SUI.HPGroup[_pos]; 
	}
	public GameObject GetTurnGroup { get { return m_SUI.TurnGroup; } }
	public Item_SpeechBubble GetSpeech { get { return m_SUI.SpeechBubble; } }
	/// <summary> 0:상단 정보 바, 1:상대 스탯, 2:플레이어 스탯 </summary> 
	public GameObject GetTutoObj(int _pos) {
		return m_SUI.TutoObjs[_pos];
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		PlayBGSound(SND_IDX.BGM_0661);
		PlayEffSound(SND_IDX.SFX_1302);
		Init();
	}
	public void Init() {
		m_SUI.SpeechBubble.gameObject.SetActive(false);

		for (int i = 0; i < 2; i++) {
			var userinfo = PVPINFO.Users[i];
			m_SUI.UserInfos[i].Portrait.sprite = TDATA.GetUserProfileImage(userinfo.m_Info.Profile);
			m_SUI.UserInfos[i].NationIcon.sprite = BaseValue.GetNationIcon(userinfo.m_Info.Nation);
			m_SUI.UserInfos[i].Name.text = userinfo.m_Info.Name;
			for (StatType j = StatType.Men; j < StatType.SurvEnd; j++)
				PVP.GetStat((UserPos)i, j).SetData(PVP.m_PlayUser[i].GetStat(j, 0), PVP.m_PlayUser[i].GetStat(j, 0), PVP.m_PlayUser[i].GetStat(j, 1), false);
			m_SUI.UserInfos[i].SupSkill.gameObject.SetActive(false);
		}
		TW_TeamHPGauge(PVP.GetNowStat(UserPos.My, StatType.HP) / PVP.GetMaxStat(UserPos.My, StatType.HP));
		TW_TeamHPGaugeTail(PVP.GetNowStat(UserPos.My, StatType.HP) / PVP.GetMaxStat(UserPos.My, StatType.HP));
		TW_EnemyHPGauge(PVP.GetNowStat(UserPos.Target, StatType.HP) / PVP.GetMaxStat(UserPos.Target, StatType.HP));
		TW_EnemyHPGaugeTail(PVP.GetNowStat(UserPos.Target, StatType.HP) / PVP.GetMaxStat(UserPos.Target, StatType.HP));

		SetTurn(PVP.GetTurn, true);
		SetAccBtn();
	}
	public void SetAnim(PVPMng.State _state) {
		switch (_state) {
			case PVPMng.State.Play: m_SUI.m_Anim.SetTrigger("3_Fight"); break;
		}
	}
	public void SetSurvStat(UserPos _userpos, StatType _stat, float _preval, float _crntval, float _maxval, bool _use = false) {
		if(_stat == StatType.HP) {
			string func = _userpos == UserPos.My ? "TW_TeamHPGauge" : "TW_EnemyHPGauge";
			float[] ratio = new float[2];
			ratio[0] = _preval <= 0 ? 0f : _preval / _maxval;
			ratio[1] = _crntval <= 0 ? 0f : _crntval / _maxval;
			iTween.StopByName(gameObject, "HPGauge");
			iTween.ValueTo(gameObject, iTween.Hash("from", ratio[0], "to", ratio[1], "onupdate", func, "time", 1f, "name", "HPGauge"));
		}
		else
			PVP.GetStat(_userpos, _stat).SetData(_preval, _crntval, _maxval, _crntval < _preval, _use);
	}
	public void SetHPTail(UserPos _userpos, float _preval, float _crntval, float _maxval) {
		if (_preval == _crntval) return;
		string func = _userpos == UserPos.My ? "TW_TeamHPGaugeTail" : "TW_EnemyHPGaugeTail";
		float[] ratio = new float[2];
		ratio[0] = _preval <= 0 ? 0f : _preval / _maxval;
		ratio[1] = _crntval <= 0 ? 0f : _crntval / _maxval;
		iTween.StopByName(gameObject, "HPGaugeTail");
		iTween.ValueTo(gameObject, iTween.Hash("from", ratio[0], "to", ratio[1], "onupdate", func, "time", 1f, "name", "HPGaugeTail"));
	}
	public void SupSkillFX(UserPos _userpos, RES_PVP_CHAR _info) {
		m_SUI.UserInfos[(int)_userpos].SupSkill.SetData(_userpos, _info);
	}
	void TW_TeamHPGauge(float _amount) {
		m_SUI.UserInfos[(int)UserPos.My].HpGauge[0].value = _amount;
	}
	void TW_TeamHPGaugeTail(float _amount) {
		m_SUI.UserInfos[(int)UserPos.My].HpGauge[1].value = _amount;
	}
	void TW_EnemyHPGauge(float _amount) {
		m_SUI.UserInfos[(int)UserPos.Target].HpGauge[0].value = _amount;
	}
	void TW_EnemyHPGaugeTail(float _amount) {
		m_SUI.UserInfos[(int)UserPos.Target].HpGauge[1].value = _amount;
	}
	public void SetTurn(int _turn, bool _init = false) {
		m_SUI.Turn[0].sprite = UTILE.LoadImg(string.Format("UI/UI_Store/Store_05_NumberFont_{0}{1}", Mathf.FloorToInt(_turn / 100), _turn < 100 ? "_Off" : string.Empty), "png");
		m_SUI.Turn[1].sprite = UTILE.LoadImg(string.Format("UI/UI_Store/Store_05_NumberFont_{0}{1}", Mathf.FloorToInt(_turn % 100 / 10), _turn < 10 ? "_Off" : string.Empty), "png");
		m_SUI.Turn[2].sprite = UTILE.LoadImg(string.Format("UI/UI_Store/Store_05_NumberFont_{0}{1}", Mathf.FloorToInt(_turn % 10), _turn < 1 ? "_Off" : string.Empty), "png");

		m_SUI.TurnAnim.SetTrigger(_init ? "Idle" : "Change");
		m_SUI.TurnAnim.SetTrigger(_turn >= 20 ? "Normal" : "Warning");
	}
	void SetAccBtn() {
		float acc = PlayerPrefs.GetFloat(string.Format("PVP_ACC_{0}", USERINFO.m_UID), 1f);
		m_SUI.AccBtnAnim.SetTrigger(acc == 2f ? "x2" : "x1");
	}
	public void ClickPause() {
		if (!PVP.IS_Play) return;

		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Pause, (res, obj)=> { 
			if(res == 1) {//항복
				PVP.SetSurrender();
			}
		});
	}
	/// <summary> 배속 버튼</summary>
	public void ClickAccSwap() {
		if (!PVP.IS_Play) return;
		PVP.SetAccSwap();
		SetAccBtn();
	}
}
