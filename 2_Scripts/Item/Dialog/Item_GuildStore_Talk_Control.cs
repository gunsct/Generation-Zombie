using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_GuildStore_Talk_Control : ObjMng
{
	public enum CloseMode
	{
		None = 0,
		Auto
	}

	public enum State
	{
		Start = 0,
		Talking,
		Close
	}
	[System.Serializable]
	struct SUI
	{
		public Animator Anim;
		public Item_Talk_Talk Talk;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] CloseMode m_CloseMode;

	//[HideInInspector]
	public State m_State;

	IEnumerator m_ActionCheck;
	int m_Idx;
	DialogTalkType m_Type;
	void OnEnable()
	{
		if (m_Idx == 0) return;
		if (m_ActionCheck != null) StopCoroutine(m_ActionCheck);
		m_ActionCheck = PlayAction(m_Idx, m_Type);
		StartCoroutine(m_ActionCheck);
	}

	public void StartTalk(int DialogIdx, DialogTalkType type = DialogTalkType.Normal)
	{
		m_Idx = DialogIdx;
		m_Type = type;
		if (gameObject.activeSelf)
		{
			if (m_ActionCheck != null) StopCoroutine(m_ActionCheck);
			m_ActionCheck = PlayAction(m_Idx, type);
			StartCoroutine(m_ActionCheck);
		}
	}

	IEnumerator PlayAction(int DialogIdx, DialogTalkType type)
	{
		m_Idx = 0;
		m_SUI.Anim.SetTrigger("Start");
		m_SUI.Talk.SetData(type, "", true);
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim);
		m_SUI.Talk.SetData(type, TDATA.GetString(ToolData.StringTalbe.Dialog, DialogIdx), true);
		yield return new WaitWhile(() => m_SUI.Talk.IsAction());
		yield return new WaitForSeconds(1.3f);
		if(m_CloseMode == CloseMode.Auto) m_SUI.Anim.SetTrigger("End");
		m_ActionCheck = null;
	}

}
