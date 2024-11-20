using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_GachaReward : ObjMng
{
	public enum Anim
	{
		Char,
		Piece
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_CharacterCard CharCard;
		public Item_RewardItem_Card PieceCard;
	}
	[SerializeField] SUI m_SUI;
	Anim m_Anim;
	CharInfo m_Info;

	private void OnEnable() {
		m_SUI.Anim.SetTrigger(m_Anim.ToString());
	}
	public void SetData(CharInfo _char, int _grade = 0) {
		m_Info = _char;
		m_SUI.PieceCard.gameObject.SetActive(false);
		m_SUI.CharCard.SetData(_char);
		if (_grade > 0) m_SUI.CharCard.SetGrade(_grade);
		m_Anim = Anim.Char;
		m_SUI.Anim.SetTrigger(m_Anim.ToString());
	}
	public void SetData(int _piece, int _cnt, int _chargrade = 1) {
		if (_chargrade < 1) _chargrade = 1;
		m_SUI.PieceCard.SetData(_piece, _cnt);
		int charidx = TDATA.GetCharacterTableToPiece(_piece).m_Idx;
		CharInfo copychar = new CharInfo(charidx, 0, _chargrade);
		m_SUI.CharCard.SetData(copychar);
		m_Anim = Anim.Piece;
		m_SUI.Anim.SetTrigger(m_Anim.ToString());
	}
	public void Click_NotGetView() {
		if (m_Info == null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_Char_NotGet, null, m_Info.m_Idx, Info_Character_NotGet.State.Normal);
	}
}
