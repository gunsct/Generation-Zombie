using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Item_Skill_Notice : ObjMng
{
	public enum AniName
	{
		Start = 0,
		Change,
		End
	}

#pragma warning disable 0649
	[SerializeField] Animator m_Ani;
	[SerializeField] TextMeshProUGUI m_Cnt;
	Transform m_WorldPos;
#pragma warning restore 0649

	void Update()
	{
		SetWorldPos(m_WorldPos);
	}

	public void SetCnt(int Cnt)
	{
		if(m_Cnt != null) m_Cnt.text = Cnt.ToString();
	}

	public void SetActive(bool Active, Action EndCb = null)
	{
		if (m_Ani != null)
		{
			if(Active) PlayAni(AniName.Start, EndCb);
			else EndCb?.Invoke();
		}
		else if(Active) StartCoroutine(AniEndCheck(EndCb));
		else EndCb?.Invoke();
	}

	public void PlayAni(AniName name, Action EndCb = null)
	{
		if (m_Ani != null)
		{
			m_Ani.SetTrigger(name.ToString());
			if(gameObject.activeSelf) StartCoroutine(AniEndCheck(EndCb));
		}
		else EndCb?.Invoke();
	}

	public void SetWorldPos(Transform pos)
	{
		if (pos == null) return;
		m_WorldPos = pos;
		transform.position = Utile_Class.GetCanvasPosition(m_WorldPos.position);
	}

	IEnumerator AniEndCheck(Action EndCb)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Ani));
		EndCb?.Invoke();
	}
}
