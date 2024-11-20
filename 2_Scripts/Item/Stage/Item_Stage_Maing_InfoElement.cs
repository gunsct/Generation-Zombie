using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_Stage_Maing_InfoElement : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Item_Stage_MakeCard UtilCard;
		public GameObject[] MatGroup;
		public GameObject PlusIcon;
		public TextMeshProUGUI[] GroupNames;
		public Item_Stage_MakeCard[] Mat1Cards;
		public Item_Stage_MakeCard[] Mat2Cards;
		public TextMeshProUGUI Desc;
	}
	[SerializeField]
	SUI m_SUI;
	TStageMakingTable m_TData;
	private void Awake() {
		for(int i = 0; i < 2; i++) m_SUI.MatGroup[i].SetActive(false);
		for(int i = 0; i < 4; i++) {
			m_SUI.Mat1Cards[i].gameObject.SetActive(false);
			m_SUI.Mat2Cards[i].gameObject.SetActive(false);
		}
	}
	public void SetData(TStageMakingTable _table, bool _debuff) {
		m_TData = _table;
		//완성시
		m_SUI.UtilCard.SetData(_table.m_MatType, _table);

		m_SUI.GroupNames[0].text = TDATA.GetStageCardTable(_table.m_CardIdx).GetName();
		m_SUI.Desc.text = TDATA.GetStageCardTable(_table.m_CardIdx).GetInfo();
		//재료
		for (int i = 0;i< _table.m_Materal.Count; i++) {
			Item_Stage_MakeCard[] cards = i == 0 ? m_SUI.Mat1Cards : m_SUI.Mat2Cards;
			m_SUI.MatGroup[i].SetActive(true);
			for (int j = 0;j< _table.m_Materal[i].m_Cnt; j++)
			{
				cards[j].gameObject.SetActive(true);
				if ((int)_table.m_Materal[i].m_Type <= (int)StageMaterialType.DefaultMat) {
					cards[j].SetData(_table.m_Materal[i].m_Type);
					m_SUI.GroupNames[i + 1].text = TDATA.GetStageMaterialTable(_table.m_Materal[i].m_Type).GetName();
				}
				else {
					cards[j].SetData(_table.m_Materal[i].m_Type, TDATA.GetStageMakingData(TDATA.GetStageMakingData(_table.m_Materal[i].m_Type).m_CardIdx));
					m_SUI.GroupNames[i + 1].text = TDATA.GetStageCardTable(TDATA.GetStageMakingData(_table.m_Materal[i].m_Type).m_CardIdx).GetName();
				}
			}
			if (_debuff) {
				cards[cards.Length - 1].gameObject.SetActive(true);
				if ((int)_table.m_Materal[i].m_Type <= (int)StageMaterialType.DefaultMat)
					cards[cards.Length - 1].SetData(_table.m_Materal[i].m_Type);
				else 
					cards[cards.Length - 1].SetData(_table.m_Materal[i].m_Type, TDATA.GetStageMakingData(TDATA.GetStageMakingData(_table.m_Materal[i].m_Type).m_CardIdx));
			}
		}
		m_SUI.PlusIcon.SetActive(_table.m_Materal.Count > 1);
	}
}
