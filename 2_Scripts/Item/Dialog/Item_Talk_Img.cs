using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_Talk_Img : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image Img;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(Sprite _sprite, string _trig, bool _iscard = false) {
		if (m_SUI.Anim != null) m_SUI.Anim.SetTrigger(_trig);
		m_SUI.Img.sprite = _sprite;
		if (!_iscard) {
			m_SUI.Img.SetNativeSize();
			Canvas.ForceUpdateCanvases();
		}
	}
}
