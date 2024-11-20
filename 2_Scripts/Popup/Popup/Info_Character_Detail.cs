using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Info_Character_Detail : PopupBase
{
	[System.Serializable]
	public struct SUI
	{
		public Item_InfoStat_Slide[] StatSlide;
		public Item_InfoStat[] StatNoSlide;
		public TextMeshProUGUI Desc;
		public ScrollRect Scroll;
		public RectTransform Content;
	}
	[SerializeField]
	SUI m_SUI;
	CharInfo m_Info;
	Coroutine m_Cor;
	char[] m_Desc;
	StringBuilder m_Temp = new StringBuilder();
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_Info = (CharInfo)aobjValue[0];
		m_SUI.StatSlide[0].SetData(StatType.Men, Mathf.RoundToInt(m_Info.GetStat(StatType.Men)), m_Info.m_TData.GetStatImport(StatType.Men));
		m_SUI.StatSlide[1].SetData(StatType.Hyg, Mathf.RoundToInt(m_Info.GetStat(StatType.Hyg)), m_Info.m_TData.GetStatImport(StatType.Hyg));
		m_SUI.StatSlide[2].SetData(StatType.Sat, Mathf.RoundToInt(m_Info.GetStat(StatType.Sat)), m_Info.m_TData.GetStatImport(StatType.Sat));
		m_SUI.StatSlide[3].SetData(StatType.Atk, Mathf.RoundToInt(m_Info.GetStat(StatType.Atk)), m_Info.m_TData.GetStatImport(StatType.Atk));
		m_SUI.StatSlide[4].SetData(StatType.Def, Mathf.RoundToInt(m_Info.GetStat(StatType.Def)), m_Info.m_TData.GetStatImport(StatType.Def));
		m_SUI.StatSlide[5].SetData(StatType.Heal, Mathf.RoundToInt(m_Info.GetStat(StatType.Heal)), m_Info.m_TData.GetStatImport(StatType.Heal));
		m_SUI.StatNoSlide[0].SetData(StatType.HP, Mathf.RoundToInt(m_Info.GetStat(StatType.HP)));
		m_SUI.StatNoSlide[1].SetData(StatType.Sta, Mathf.RoundToInt(m_Info.GetStat(StatType.Sta)));
		m_SUI.StatNoSlide[2].SetData(StatType.Guard, Mathf.RoundToInt(m_Info.GetStat(StatType.Guard)));
		m_Desc = m_Info.m_TData.GetCharDesc().ToCharArray();

		m_Cor = StartCoroutine(DescAction());
	}
	/// <summary> 설명 타이핑되면서 내려가고 쭉 올라오기</summary>
	IEnumerator DescAction() {
		int pos = 0;
		m_SUI.Scroll.vertical = false;
		while (pos < m_Desc.Length) {
			m_Temp.Append(m_Desc[pos]);
			m_SUI.Desc.text = m_Temp.ToString();
			m_SUI.Scroll.verticalNormalizedPosition = 0;
			pos++;
			yield return new WaitForSeconds(0.06f);
		}
		yield return new WaitForSeconds(0.5f);
		//천천히 올라가기
		if (m_SUI.Content.rect.height > 500f) {
			iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.verticalNormalizedPosition, "to", 1f, "time", m_SUI.Content.rect.height / 500f, "onupdate", "TW_VerticalVal", "easetype", "linear"));
			yield return new WaitUntil(() => !Utile_Class.IsPlayiTween(gameObject));
		}
		m_SUI.Scroll.vertical = true;
	}
	void TW_VerticalVal(float _amount) {
		m_SUI.Scroll.verticalNormalizedPosition = _amount;
	}
	public void ClickDescSkip() {
		if (m_Cor != null) {
			StopCoroutine(m_Cor);
			m_Cor = null;
			iTween.Stop(gameObject);

			m_SUI.Scroll.verticalNormalizedPosition = 1f;
			m_SUI.Desc.text = m_Info.m_TData.GetCharDesc();
			m_SUI.Scroll.vertical = true;
		}
	}
}
