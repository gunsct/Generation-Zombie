using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_PDA_Research_Element : ObjMng
{
	public enum State
	{
		/// <summary> 0레벨 잠김 상태 </summary>
		Lock = 0,
		/// <summary> 스테이지 제한 </summary>
		Lock_Stg,
		/// <summary> 레벨업 가능 </summary>
		Active,
		/// <summary> 선행 조건 충족 안됨 </summary>
		Deactive,
		/// <summary> 연구 중 </summary>
		Timer,
		/// <summary> 연구 완료 </summary>
		Complete,
		/// <summary>  </summary>
		End
	}
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Animator Ani;
		public Image Icon;
		public TextMeshProUGUI Deco;
		public Image Time;
		public TextMeshProUGUI TimeValue;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI StgLock;
	}

	[SerializeField] SUI m_SUI;
	State m_State;
#pragma warning restore 0649
	public ResearchInfo m_Info;
	Action<Item_PDA_Research_Element, int> m_ClickCB;
	bool m_IsLock;
	bool m_IsFirstOpen;
	public void SetData(ResearchInfo info, Action<Item_PDA_Research_Element, int> ClickCB)
	{
		m_Info = info;
		m_ClickCB = ClickCB;
		// 선행 조건을 만족했는지 확인
		m_IsFirstOpen = m_Info.Is_Open();
		m_IsLock = m_Info.Is_Open(false);
		//스테이지 락 표기는 m_Info.Is_StgUnLock();로 한다
		SetInfo();
		SetStateAni();
	}

	public void SetStateAni()
	{
		m_State = State.Active;
		if (m_Info.m_GetLv < m_Info.m_MaxLV) {
			if (!m_IsFirstOpen) m_State = m_Info.Is_StgUnLock() ? State.Lock : State.Lock_Stg;
			else if (!m_IsLock) m_State = m_Info.Is_StgUnLock(false) ? State.Deactive : State.Lock_Stg;
			else if (m_Info.m_State == TimeContentState.Play) m_State = m_Info.IS_Complete() ? State.Complete : State.Timer;
		}
		m_SUI.Ani.SetTrigger(m_State.ToString());
	}

	void SetInfo()
	{
		TResearchTable data = m_Info.m_TData;
		int MaxLV = m_Info.m_MaxLV;
		bool IsMaxLV = m_Info.m_GetLv == MaxLV;

		m_SUI.Icon.sprite = data.GetIcon();
		m_SUI.Deco.text = UTILE.Get_MemoryUnit(UTILE.Get_Random(100, 10000000));
		m_SUI.Name.text = data.GetName();
		if (IsMaxLV)
		{
			m_SUI.LV.text = "MAX";
		}
		else
		{
			m_SUI.LV.text = string.Format("Lv. {0} / {1}", m_Info.m_GetLv, MaxLV);
			if(m_IsFirstOpen) SetTimer();
		}
		m_SUI.StgLock.text = m_Info.Is_StgUnLock(!m_IsFirstOpen) ? string.Empty : string.Format("{0}-{1}", data.m_UnLockVal / 100, data.m_UnLockVal % 100);
	}

	void SetTimer()
	{
		StartCoroutine(timecheck());
	}

	IEnumerator timecheck()
	{
		// 타이머 UI 셋팅
		TimeContentState state = m_Info.m_State;
		switch(state)
		{
		case TimeContentState.Play: // 연구 중
			if(m_Info.IS_Complete())
			{
				SetStateAni();
				yield break;
			}
			double remain = m_Info.GetRemainTime();
			m_SUI.TimeValue.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, remain);
			m_SUI.Time.fillAmount = 1f - (float)(remain / (m_Info.GetMaxTime() * 0.001d));
			break;
		default:
			yield break;
		}
		yield return new WaitForEndOfFrame();
		SetTimer();
	}

	public void OnClick() {
		if (m_State == State.Lock || m_State == State.Lock_Stg) return;
		m_ClickCB?.Invoke(this, 0);
	}
}
