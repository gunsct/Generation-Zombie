using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_PVP_BuffLabel : ObjMng
{
   [Serializable]
   public struct SUI
	{
		public Animator Anim;
		public SpriteRenderer Icon;
	}
	[SerializeField] SUI m_SUI;
	public PVPTurnType m_Type;
	public StatType m_Stat;
	bool IS_Buff;
	Action<Item_PVP_BuffLabel> m_CB;
	public void SetData(PVPTurnType _type, StatType _stat, bool _buff, Action<Item_PVP_BuffLabel> _cb) {
		m_Type = _type;
		m_Stat = _stat;
		IS_Buff = _buff;
		m_CB = _cb;
		m_SUI.Icon.sprite = UTILE.LoadImg(string.Format("UI/Icon/Icon_Char_Stat_{0}", (int)_stat), "png");
		m_SUI.Anim.SetTrigger(IS_Buff ? "Buff_Start" : "Debuff_Start");
	}
	public void End() {
		StartCoroutine(IE_EndAction());
	}
	IEnumerator IE_EndAction() {
		m_SUI.Anim.SetTrigger(IS_Buff ? "Buff_End" : "Debuff_End");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		m_CB?.Invoke(this);
	}
}
