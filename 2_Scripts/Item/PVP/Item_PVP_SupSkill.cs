using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PVP_SupSkill : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image[] Portrait;
		public TextMeshProUGUI[] CharName;
		public TextMeshProUGUI[] SkillName;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;
	public void SetData(UserPos _userpos, RES_PVP_CHAR _info) {
		TCharacterTable ctada = TDATA.GetCharacterTable(_info.Idx);
		m_SUI.Portrait[0].sprite = m_SUI.Portrait[1].sprite = ctada.GetPortrait();
		m_SUI.CharName[0].text = m_SUI.CharName[1].text = string.Format("{0} : {1}", TDATA.GetString(994), ctada.GetCharName());
		m_SUI.SkillName[0].text = m_SUI.SkillName[1].text = TDATA.GeTPVPSkillTable(ctada.m_PVPSkillIdx).GetName();

		gameObject.SetActive(true);

		PlayEffSound(ctada.GetVoice(TCharacterTable.VoiceType.Skill));
		//PlayVoiceSnd(new List<SND_IDX>() { ctada.GetVoice(TCharacterTable.VoiceType.Skill) });

		m_SUI.Anim.SetTrigger(_userpos == UserPos.My ? "Player" : "Enemy");

		if (m_Action != null) StopCoroutine(m_Action);
		m_Action = IE_Action();
		StartCoroutine(m_Action);
	}
	IEnumerator IE_Action() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		gameObject.SetActive(false);
	}
}
