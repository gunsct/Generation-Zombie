using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_PVP_DMG : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMesh[] Txt;
		public SpriteRenderer[] Icons;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(PVPDmgType _type, int _dmg, Sprite _icon = null) {
		m_SUI.Anim.SetTrigger(_type.ToString());
		if (_type != PVPDmgType.Miss) {
			for (int i = 0; i < m_SUI.Txt.Length; i++) {
				m_SUI.Txt[i].text = Mathf.Abs(_dmg).ToString();
				m_SUI.Txt[i].anchor = (_type == PVPDmgType.Heal || _type == PVPDmgType.Normal) ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft;
			}
			for (int i = 0; i < m_SUI.Icons.Length; i++) m_SUI.Icons[i].sprite = _icon;
		}
	}
}
