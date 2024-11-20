using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_CharDeckCard : Item_CharacterCard
{
	public enum Anim
	{
		Normal,
		In,
		Out,
		NormalOut,
		None
	}
	public enum State
	{
		Set,
		Empty
	}
	[Serializable]
	public struct SSUI
	{
		public Animator Anim;
		public TextMeshProUGUI EmptyTxt;
		public GameObject[] NeedGroup;
		public Image NeedChar;
		public Image NeedJob;
		public GameObject NeedFrame;
	}
	[SerializeField]
	SSUI m_SSUI;
	public State m_State = State.Empty;
	Anim m_AnimState = Anim.None;
	public int m_Pos;
	public int m_NeedIdx;
	public bool Is_Need;
	private void Awake() {
		for(int i = 0; i < m_SSUI.NeedGroup.Length; i++) {
			if (m_SSUI.NeedGroup[i] != null) m_SSUI.NeedGroup[i].SetActive(false);
		}
		m_SSUI.NeedFrame.SetActive(false);
	}
	public override void SetData(CharInfo _char = null) {
		m_SSUI.NeedFrame.SetActive(false);
		if (_char == null) {
			m_Info = null;
			SetAnim(Anim.NormalOut);
			m_State = State.Empty;
		}
		else {
			base.SetData(_char);
			m_State = State.Set;
			SetAnim(Anim.In);
		}
	}
	public void SetData(CharInfo _char, int _needchar, int _pos) {
		SetData(_char);

		Is_Need = _needchar > 0;
		m_NeedIdx = _needchar;

		m_SSUI.EmptyTxt.text = TDATA.GetString(_needchar > 0 ? 871 : 870);
		for (int i = 0; i < m_SSUI.NeedGroup.Length; i++) {
			if (m_SSUI.NeedGroup[i] != null) m_SSUI.NeedGroup[i].SetActive(_needchar > 0);
		}
		if(_char == null) m_SSUI.NeedFrame.SetActive(_needchar > 0);
		else m_SSUI.NeedFrame.SetActive(_char.m_Idx == _needchar && _needchar > 0);

		if (_needchar > 0) {
			TCharacterTable tdata = TDATA.GetCharacterTable(_needchar);
			m_SSUI.NeedChar.sprite = tdata.GetPortrait();
			m_SSUI.NeedJob.sprite = tdata.GetJobIcon()[0];
			m_SSUI.NeedFrame.GetComponent<Animator>().SetTrigger(_pos == 0 ? "Color_Force" : "Color_Need");
		}
	}
	public void Refresh(CharInfo _char) {
		base.SetData(_char);
	}
	public void OutSlot() {
		m_State = State.Empty;
		SetAnim(Anim.Out);
	}
	public void SetAnim(Anim _trig) {
		m_AnimState = _trig;
		m_SSUI.Anim.SetTrigger(_trig.ToString());
	}
	//상세 정보
	public void OpenDetail() {
		List<CharInfo> charinfos = new List<CharInfo>();
		for(int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			CharInfo info = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]);
			charinfos.Add(info);
		}
		USERINFO.ShowCharInfo(m_Idx, charinfos, (result, obj) => {
			DLGTINFO.f_RFDeckCharInfoCard?.Invoke();
		});
	}
}