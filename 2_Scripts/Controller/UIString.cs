using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIString : ObjMng
{
	[SerializeField] ToolData.StringTalbe m_Table = ToolData.StringTalbe.UI;
	[SerializeField] int m_UIStringIndex;
	Text m_Text;
	TextMesh m_TextMesh;
	TextMeshPro m_TextMeshPro;
	TextMeshProUGUI m_TextMeshProUGUI;
	Action m_CB;
	private void Awake() {
		m_Text = GetComponent<Text>();
		m_TextMesh = GetComponent<TextMesh>();
		m_TextMeshPro = GetComponent<TextMeshPro>();
		m_TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
		if (m_Text) m_CB = Set_Text;
		if (m_TextMesh) m_CB = Set_TextMesh;
		if (m_TextMeshPro) m_CB = Set_TextMeshPro;
		if (m_TextMeshProUGUI) m_CB = Set_TextMeshProUGUI;
		m_CB?.Invoke();
	}

	private void OnEnable()
	{
		m_CB?.Invoke();
	}

	void Set_Text()
	{
		m_Text.text = TDATA.GetString(m_Table, m_UIStringIndex);
	}

	void Set_TextMesh()
	{
		m_TextMesh.text = TDATA.GetString(m_Table, m_UIStringIndex);
	}
	void Set_TextMeshPro()
	{
		m_TextMeshPro.SetText(TDATA.GetString(m_Table, m_UIStringIndex));
	}
	void Set_TextMeshProUGUI()
	{
		m_TextMeshProUGUI.SetText(TDATA.GetString(m_Table, m_UIStringIndex));
	}
}
