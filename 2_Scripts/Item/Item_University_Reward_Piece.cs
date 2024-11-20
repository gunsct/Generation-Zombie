using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_University_Reward_Piece : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image Portrait;
		public Image BG;
		public Sprite CommPiece;
	}
	[SerializeField] SUI m_SUI;
	int m_Idx;
	public void SetData(TCharacterTable _tdata) {

		m_SUI.Portrait.sprite = _tdata == null ? m_SUI.CommPiece : _tdata.GetPortrait();
		m_SUI.BG.sprite = BaseValue.CharBG(_tdata == null ? 0 : _tdata.m_Grade);
	}
}
