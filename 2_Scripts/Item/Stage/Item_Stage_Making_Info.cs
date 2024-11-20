using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Stage_Making_Info : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	public struct SMaterial
	{
		public GameObject Active;
		public Image Icon;
		public TextMeshProUGUI Cnt;
	}

	[System.Serializable]
	public struct SUI
	{
		public Item_Stage_Making_Card Card;
		public Image Item;
		public SMaterial[] Mats;
	}

	[SerializeField] SUI m_SUI;
#pragma warning restore 0649

	public void SetData(TStageMakingTable data)
	{
		TStageCardTable table = TDATA.GetStageCardTable(data.m_CardIdx);
		if (m_SUI.Item != null) m_SUI.Item.sprite = table.GetImg();
		if (m_SUI.Card != null) m_SUI.Card.SetData(data);

		// 재료
		for (int i = 0; i < 2; i++)
		{
			if (i < data.m_Materal.Count)
			{
				TStageMakingTable.MakeMaterial mat = data.m_Materal[i];
				m_SUI.Mats[i].Active.SetActive(true);
				m_SUI.Mats[i].Active.GetComponentInChildren<Item_RewardItem_Card>().GetLvGroup.SetActive(false);
				if ((int)mat.m_Type <= (int)StageMaterialType.DefaultMat) {
					TStageMaterialTable mattable = TDATA.GetStageMaterialTable(mat.m_Type);
					m_SUI.Mats[i].Icon.sprite = mattable.GetStateCardImg();
				}
				else {
					int idx = TDATA.GetStageMakingData(mat.m_Type).m_CardIdx;
					m_SUI.Mats[i].Icon.sprite = TDATA.GetStageCardTable(idx).GetImg();
				}
				m_SUI.Mats[i].Cnt.text = string.Format("{0}/{1}",STAGEINFO.m_Materials[(int)mat.m_Type], Utile_Class.CommaValue(mat.m_Cnt));

			}
			else m_SUI.Mats[i].Active.SetActive(false);
		}
	}
}
