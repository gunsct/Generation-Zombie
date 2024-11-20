using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_ZombieReward_Card : ObjMng
{
   [Serializable]
   public struct SUI
	{
		public Image Icon;
		public Image Frame;
		public Image GradeBG;
		public TextMeshProUGUI[] Grade;
	}

	[SerializeField]
	SUI m_SUI;
	int m_Idx;
	public int m_Grade;
	TZombieTable m_TData { get { return TDATA.GetZombieTable(m_Idx); } }

	public void SetData(int _idx, int _grade = 1) {
		m_Idx = _idx;
		m_Grade = _grade == 0 ? m_TData.m_Grade : _grade;
		m_SUI.Icon.sprite = m_TData.GetItemSmallImg();
		if (m_SUI.Frame != null) m_SUI.Frame.sprite = BaseValue.GradeFrame(ItemType.Zombie, m_Grade);
		if (m_SUI.GradeBG != null) m_SUI.GradeBG.sprite = BaseValue.ItemGradeBG(ItemType.Zombie, m_Grade);
		m_SUI.Grade[0].text = string.Format("<color=#aaaaaa>Z</color>{0}", m_Grade);
		m_SUI.Grade[1].text = string.Format(" Z</size>{0}", m_Grade);
	}

	public virtual void ClickCard() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_ToolTip, m_Idx)) return;
		if (m_TData != null) POPUP.ViewItemToolTip(GetRewardInfo(), (RectTransform)transform);
	}

	public RES_REWARD_BASE GetRewardInfo()
	{
		var res = new RES_REWARD_ZOMBIE();
		res.Idx = m_Idx;
		res.Grade = m_Grade;
		return res;
	}
}
