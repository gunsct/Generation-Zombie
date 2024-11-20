using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_PB_Chain : ObjMng
{
	public enum AniName
	{
		Normal = 0,
		Pressed,
		End
	}
#pragma warning disable 0649
	[SerializeField] Animator m_Ani;
#pragma warning restore 0649
	int m_Cnt;
	System.Action<Item_PB_Chain> m_CB;
	System.Action m_PressCB;
	public void SetData(int cnt = 0, System.Action<Item_PB_Chain> cb = null)
	{
		m_Cnt = cnt;
		m_CB = cb;
	}

	public void PressCheckCB(System.Action cb)
	{
		m_PressCB = cb;
	}

	public void OnPress()
	{
		if (m_Cnt < 1) return;
		m_PressCB?.Invoke();
		if (--m_Cnt < 1) {
			SND.PlayEffSound(SND_IDX.SFX_0952);
			AniAction(AniName.End);
			m_CB?.Invoke(this);
		}
		else {
			SND.PlayEffSound(SND_IDX.SFX_0951);
			AniAction(AniName.Pressed);
		}
	}

	public void OnRelease()
	{
		if (m_Cnt < 1) return;
		AniAction(AniName.Normal);
	}

	public void AniEnd()
	{
		GameObject.Destroy(gameObject);
	}

	public void AniAction(AniName ani)
	{
		m_Ani.SetTrigger(ani.ToString());
	}
}
