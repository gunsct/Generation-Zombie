using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Button : ObjMng
{
   [System.Serializable]
   public struct SUI
	{
		public Image BG;
		public Button Btn;
		public ScrollRectEventTrigger Event;
		public GameObject UpMark;
	}
	[SerializeField] SUI m_SUI;

	public void SetActive(bool Active, bool ViewArrow = false, UIMng.BtnBG bg = UIMng.BtnBG.Normal)
	{
		SetBG(bg);
		if (m_SUI.Btn != null) m_SUI.Btn.enabled = Active;
		if (m_SUI.Event != null) m_SUI.Event.enabled = Active;
		if (m_SUI.UpMark != null) m_SUI.UpMark.SetActive(ViewArrow);
	}

	public void SetBG(UIMng.BtnBG bg = UIMng.BtnBG.Normal)
	{
		m_SUI.BG.sprite = POPUP.GetBtnBG(bg);
	}
}
