using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_StoryElement : ObjMng
{
	public enum State
	{
		Open = 0,
		Viewed,
		Lock,
		END
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image CharImg;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Lock;
		public TextMeshProUGUI Reward;
	}

	[SerializeField] private SUI m_SUI;
	private CharInfo m_Info;
	State m_State = State.END;
	int m_Slot;
	Action<int> m_ClickCB;

	private void OnEnable()
	{
		// 현상태 애니로 표시
		if(m_State != State.END) m_SUI.Anim.SetTrigger(m_State.ToString());
	}

	public void SetData(CharInfo info, int Slot, State State, Action<int> CB)
	{
		m_Info = info;
		m_Slot = Slot;
		m_State = State;
		m_ClickCB = CB;
		m_SUI.CharImg.sprite = m_Info.m_TData.GetPortrait();
		m_SUI.Title.text = string.Format(TDATA.GetString(391), info.m_TData.GetCharName(), Slot+1);
		m_SUI.Lock.text = string.Format(TDATA.GetString(392), BaseValue.CHAR_OPEN_STORY_GRADE(Slot));
		m_SUI.Reward.text = BaseValue.CHAR_OPEN_STORY_REWARD_CNT(Slot).ToString();

		if (m_State != State.END && m_SUI.Anim.isActiveAndEnabled) m_SUI.Anim.SetTrigger(m_State.ToString());
	}

	public void OnBtnStoryClicked()
	{
		if (m_Info.m_TData.m_Story[m_Slot] < 1) return;
		if (m_State >= State.Lock) return;
		if (TDATA.GetDialogTable(m_Info.m_TData.m_Story[m_Slot]) == null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DL_Talk, (result, obj) => {
			m_ClickCB?.Invoke(m_Slot);
		}, m_Info.m_TData.m_Story[m_Slot], m_SUI.Title.text, m_SUI.CharImg.sprite, null, true, 0);
	}
}
