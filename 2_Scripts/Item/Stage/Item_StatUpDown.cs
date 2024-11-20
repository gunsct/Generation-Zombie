using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_StatUpDown : ObjMng
{
	[SerializeField]
	Color m_StatUp = new Color32(0x74, 0xde, 0x5d, 0xff), m_StatDown = new Color32(0xf7, 0x5b, 0x3a, 0xff);
	public Animator m_Ani;
	[SerializeField]
	Image m_Arrow;
	[SerializeField]
	TextMeshProUGUI m_Amount;

	public void SetData(float val) {
		if (val == 0) return;
		m_Amount.text = val.ToString();
		if (val > 0) {
			m_Amount.color = m_StatUp;
			m_Arrow.sprite = UTILE.LoadImg("UI/Icon/Icon_SV_Up", "png");
			m_Ani.SetTrigger("Up");
		}
		else {
			m_Amount.color = m_StatDown;
			m_Arrow.sprite = UTILE.LoadImg("UI/Icon/Icon_SV_Down", "png");
			m_Ani.SetTrigger("Down");
		}
	}
}
