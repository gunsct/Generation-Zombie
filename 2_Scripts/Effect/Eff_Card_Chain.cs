using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Eff_Card_Chain : ObjMng
{
	public enum AnimState
	{
		Start,
		End,
		Touch
	}
	public Animator m_Anim;

	public void SetAnim(AnimState _state) {
		m_Anim.SetTrigger(_state.ToString());
	}
}
