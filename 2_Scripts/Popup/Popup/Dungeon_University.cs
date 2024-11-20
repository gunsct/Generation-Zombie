using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using static LS_Web;
using System.Text;
using UnityEngine.UI;

public class Dungeon_University : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public ScrollRect Scroll;
		public Item_DG_University_Menu[] m_Elements;
	}
	[SerializeField] SUI m_SUI;
	DayOfWeek m_Day;
	private IEnumerator m_Action;
	public bool IS_End { get { return m_Action != null; } }

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_Day = UTILE.GetServerDayofWeek();
		for (int i = 0; i < m_SUI.m_Elements.Length; i++) {
			m_SUI.m_Elements[i].SetData((DayOfWeek)(1 + (int)i/2), i % 2, new Action[] { Click_Detail, Click_EndDetail });
		}
		if (PlayerPrefs.GetInt($"ContentUnlock_{USERINFO.m_UID}") == 1) {
			m_SUI.Scroll.horizontalNormalizedPosition = 0;
			for (int i = 0; i < m_SUI.m_Elements.Length; i++) {
				m_SUI.m_Elements[i].SetAnim(Item_DG_University_Menu.State.Act_Start);
			}
		}
		else {
			switch (m_Day) {
				case DayOfWeek.Saturday:
					m_SUI.Scroll.horizontalNormalizedPosition = 0.4f;
					break;
				case DayOfWeek.Sunday:
					m_SUI.Scroll.horizontalNormalizedPosition = 1f;
					break;
				default:
					m_SUI.Scroll.horizontalNormalizedPosition = 1f - 1f / (float)m_Day;
					break;
			}
			StartCoroutine(SetElementAnim(m_Day));
		}
		MainMenuType premenu = STAGEINFO.GetPreMenu();
		if (premenu != MainMenuType.Dungeon) SND.PlayEffSound(SND_IDX.SFX_0183);
	}
	IEnumerator SetElementAnim(DayOfWeek _day) {
		for (int i = 0; i < m_SUI.m_Elements.Length; i++) {
			switch (m_Day) {
				case DayOfWeek.Saturday:
					m_SUI.m_Elements[i].SetAnim((i / 2 >= 0 && i / 2 <= 2) ? Item_DG_University_Menu.State.Act_Start : Item_DG_University_Menu.State.Deact_Start);
					//m_SUI.Scroll.horizontalNormalizedPosition = 0f;
					break;
				case DayOfWeek.Sunday:
					//m_SUI.Scroll.horizontalNormalizedPosition = 0.6f;
					m_SUI.m_Elements[i].SetAnim((i / 2 >= 3 && i / 2 <= 4) ? Item_DG_University_Menu.State.Act_Start : Item_DG_University_Menu.State.Deact_Start);
					break;
				default:
					//m_SUI.Scroll.horizontalNormalizedPosition = 1f / (float)m_Day;
					m_SUI.m_Elements[i].SetAnim((i/2 == (int)m_Day-1) ? Item_DG_University_Menu.State.Act_Start : Item_DG_University_Menu.State.Deact_Start);
					break;
			}
			if (i % 2 == 1) yield return new WaitForSeconds(0.2f);
		}
	}
	void Click_Detail() {
		m_SUI.Anim.SetTrigger("Out");
	}
	void Click_EndDetail() {
		m_SUI.Anim.SetTrigger("In");
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int Result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(Result);
	}

	public void ScrollLock(bool _lock) {
		m_SUI.Scroll.enabled = !_lock;
	}
}
