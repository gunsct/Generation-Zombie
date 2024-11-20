using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISprite : ObjMng
{
	Image m_Img;
	SpriteRenderer m_SpriteRenderer;
	[SerializeField] string m_Path;
	[SerializeField] bool is_UseAppLang = true;
	[SerializeField] LanguageCode m_LangCode = LanguageCode.KO;
	string m_NationPath;
	Action m_CB;

	private void Awake() {
		m_NationPath = string.Format("{0}_{1}", m_Path, is_UseAppLang ? APPINFO.m_Language.ToString() : m_LangCode.ToString());
		m_Img = GetComponent<Image>();
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		if (m_Img != null) m_CB = SetImg;
		if (m_SpriteRenderer != null) m_CB = SetSpriteRenderer;
	}

	private void OnEnable() {
		m_CB?.Invoke();
	}
	void SetImg() {
		m_Img.sprite = GetSprite();
	}
	void SetSpriteRenderer() {
		m_SpriteRenderer.sprite = GetSprite();
	}
	Sprite GetSprite() {
		Sprite img = UTILE.LoadImg(m_NationPath, "png");
		if( img == null) img = UTILE.LoadImg(string.Format("{0}_KO", m_Path), "png");
		if (img == null) img = UTILE.LoadImg(Utile_Class.AtlasName.UI, Path.GetFileName(m_NationPath));
		if (img == null) img = UTILE.LoadImg(Utile_Class.AtlasName.UI, Path.GetFileName(string.Format("{0}_KO", m_Path)));
		return img;
	}
}
