using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartAlpha_Controller : ObjMng
{
	[SerializeField] List<Image> m_Imgs;
	[SerializeField] List<TextMeshProUGUI> m_TextMeshs;
	[SerializeField] List<Text> m_Texts;
	[SerializeField, Range(0,1)] float m_Alpha;

	void Awake()
	{
		for (int i = m_Imgs.Count - 1; i > -1; i--)
		{
			var color = m_Imgs[i].color;
			color.a = m_Alpha;
			m_Imgs[i].color = color;
		}

		for (int i = m_TextMeshs.Count - 1; i > -1; i--)
		{
			var color = m_TextMeshs[i].color;
			color.a = m_Alpha;
			m_TextMeshs[i].color = color;
		}

		for (int i = m_Texts.Count - 1; i > -1; i--)
		{
			var color = m_Texts[i].color;
			color.a = m_Alpha;
			m_Texts[i].color = color;
		}
	}
}
