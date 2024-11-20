using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;

public class Item_EquipGachaList_Item : ObjMng
{
	[Serializable]
	public struct SUI {
		public Item_RewardList_Item Item;
		public GameObject[] GoodGroup;
		public CanvasGroup CanvasGroup;
	}
	[SerializeField] SUI m_SUI;
	RES_REWARD_BASE m_Reward;
	int m_Grade;
	public int GetGrade{ get {return m_Grade;}}
	public void SetData(int maxgrade, RES_REWARD_BASE data, Action<GameObject> selectcb = null, bool IsStartEff = true) {
		m_Reward = data;
		m_SUI.Item.SetData(m_Reward, selectcb, IsStartEff);

		m_Grade = TDATA.GetItemTable(m_Reward.GetIdx()).m_Grade;
		for(int i = 0;i< m_SUI.GoodGroup.Length;i++)
			m_SUI.GoodGroup[i].SetActive(m_Grade == maxgrade);
	}
	public void AlphaOnOff(bool _on) {
		m_SUI.CanvasGroup.alpha = _on ? 1f : 0f;
		m_SUI.Item.StartAnim();
	}
	public void ClickViewInfo() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_ToolTip, m_Reward.GetIdx())) return;
		POPUP.ViewItemToolTip(m_Reward, (RectTransform)transform);
	}
}
