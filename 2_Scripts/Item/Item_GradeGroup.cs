using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_GradeGroup : ObjMng
{
   [System.Serializable]
   public struct SUI
	{
		public Image[] Star;
		public SpriteRenderer[] Star_SR;
		public SImgInfoUI[] Img;
	}
	[System.Serializable]
	public struct SImgInfoUI
	{
		public Sprite Img;
		public Vector2 Size;
	}

	[SerializeField] SUI m_SUI;

	public void SetData(int grade, bool _spriterenderer = false)
	{
		if (_spriterenderer) {
			Vector2 basesize = m_SUI.Star_SR[0].GetComponent<RectTransform>().sizeDelta;
			Vector2[] scale = { new Vector2(m_SUI.Img[0].Size.x / basesize.x, m_SUI.Img[0].Size.y / basesize.y)
				, new Vector2(m_SUI.Img[1].Size.x / basesize.x, m_SUI.Img[1].Size.y / basesize.y) };
			int yellow = grade - 5;
			for (int i = 0; i < 5; i++) {
				bool Active = i < grade;
				if (m_SUI.Star_SR[i].gameObject.activeSelf != Active) m_SUI.Star_SR[i].gameObject.SetActive(Active);
				if (Active) {
					int Pos = i < yellow ? 1 : 0;
					m_SUI.Star_SR[i].sprite = m_SUI.Img[Pos].Img;
					m_SUI.Star_SR[i].GetComponent<RectTransform>().localScale = scale[Pos];
				}
			}
		}
		else {
			Vector2 basesize = m_SUI.Star[0].rectTransform.sizeDelta;
			Vector2[] scale = { new Vector2(m_SUI.Img[0].Size.x / basesize.x, m_SUI.Img[0].Size.y / basesize.y)
				, new Vector2(m_SUI.Img[1].Size.x / basesize.x, m_SUI.Img[1].Size.y / basesize.y) };
			int yellow = grade - 5;
			for (int i = 0; i < 5; i++) {
				bool Active = i < grade;
				if (m_SUI.Star[i].gameObject.activeSelf != Active) m_SUI.Star[i].gameObject.SetActive(Active);
				if (Active) {
					int Pos = i < yellow ? 1 : 0;
					m_SUI.Star[i].sprite = m_SUI.Img[Pos].Img;
				}
			}
		}
	}
}
