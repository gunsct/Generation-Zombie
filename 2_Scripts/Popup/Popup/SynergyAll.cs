using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynergyAll : PopupBase
{
	//모든 시너지 효과, 클릭시 보유 캐릭터와 정보가 나올것
	[System.Serializable]
	public struct SUI
	{
		public Item_Synergy_Info Info;
		public Item_CharacterCard[] BigChars;
		public Transform SynergyCardParent;
		public GameObject SynergyCard;
		public Transform CrntClick;
	}
	[SerializeField]
	SUI m_SUI;
	JobType m_CrntSynergy;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		for(int i = 1;i < (int)JobType.All; i++) {
			TSynergyTable table = TDATA.GetSynergyTable((JobType)i);
			Utile_Class.Instantiate(m_SUI.SynergyCard, m_SUI.SynergyCardParent).GetComponent<Item_Synergy_Card>().SetData((JobType)i, ClickSynergyCard);
		}
		m_SUI.CrntClick.SetAsLastSibling();
		Canvas.ForceUpdateCanvases();
		m_CrntSynergy = (JobType)aobjValue[0];
		ClickSynergyCard(m_CrntSynergy);
	}

	void ClickSynergyCard(JobType _type) {
		m_SUI.CrntClick.position = m_SUI.SynergyCardParent.GetChild((int)_type - 1).position;
		m_SUI.Info.SetData(_type);

		//시너지 해당 캐릭터
		List<TCharacterTable> chartables = TDATA.GetGroupCharacterTable(_type);
		if (chartables.Count > 0) {
			for (int i = 0; i < m_SUI.BigChars.Length; i++) {
				if (i >= chartables.Count)
					m_SUI.BigChars[i].gameObject.SetActive(false);
				else {
					CharInfo charinfo = USERINFO.GetChar(chartables[i].m_Idx);
					if (charinfo != null) m_SUI.BigChars[i].GetComponent<Item_CharacterCard>().SetData(charinfo);
					else m_SUI.BigChars[i].GetComponent<Item_CharacterCard>().SetData(chartables[i].m_Idx);
					m_SUI.BigChars[i].gameObject.SetActive(true);
				}
			}
		}
	}
}
