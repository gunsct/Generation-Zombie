using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EF_DuelFontFX : MonoBehaviour
{
	public Text[] m_Font;
	public Transform m_BG;
	
	public void SetData(int _dmg) {
		m_Font[0].text = m_Font[1].text = _dmg.ToString();
		Canvas.ForceUpdateCanvases();
		m_BG.localScale = Vector3.one * m_Font[0].rectTransform.rect.width / 50f;
	}
}
