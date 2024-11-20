using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_TalkRewardCard : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Img;
		public TextMeshProUGUI Name;
	}
    [SerializeField] SUI m_SUI;

	public void SetData(int _idx) {
		m_SUI.Anim.SetTrigger("Open");
		TStageCardTable table = TDATA.GetStageCardTable(_idx);
		m_SUI.Img.sprite = table.GetImg();
		if (table.m_Type == StageCardType.Material) m_SUI.Name.text = string.Format("{0} x{1}", table.GetName(), table.m_Value2);
		else m_SUI.Name.text = table.GetName();
	}
}
