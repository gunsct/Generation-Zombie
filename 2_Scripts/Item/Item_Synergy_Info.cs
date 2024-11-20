using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Synergy_Info : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Desc;
		public Item_CharSmall_Card[] Chars;
		public TextMeshProUGUI Need;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(JobType _type) {
		TSynergyTable table = TDATA.GetSynergyTable(_type);
		m_SUI.Icon.sprite = table.GetIcon();
		m_SUI.Name.text = table.GetName();
		m_SUI.Desc.text = table.GetDesc();
		m_SUI.Need.text = table.m_NeedCount.ToString();

		List<TCharacterTable> chartables = TDATA.GetGroupCharacterTable(table.m_SynergyType);
		if (chartables.Count > 0) {
			for (int i = 0; i < m_SUI.Chars.Length; i++) {
				if (i > chartables.Count - 1)
					m_SUI.Chars[i].gameObject.SetActive(false);
				else {
					m_SUI.Chars[i].GetComponent<Item_CharSmall_Card>().SetData(chartables[i].m_Idx);
					m_SUI.Chars[i].gameObject.SetActive(true);
				}
			}
		}
	}
}
