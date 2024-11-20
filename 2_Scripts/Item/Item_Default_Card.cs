using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Default_Card : ObjMng {
	[System.Serializable]
	public struct SUI
	{
		public Image Img;
		public TextMeshProUGUI Name;
	}
	[SerializeField]
	SUI m_SUI;

	public virtual void SetData(Sprite _img, string _name) {
		m_SUI.Img.sprite = _img;
		m_SUI.Name.text = _name;
	}
}
