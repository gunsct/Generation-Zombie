using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
public enum EItem_Stage_Char_Action
{
	None = 0,
	FadeIn,
	FadeOut
}
public class Item_Stage_Char : ObjMng
{
	[System.Serializable]
	public class SUI
	{
		public int m_ProfilePos;
		public ItemCardRenderer[] Profile;
		public SpriteRenderer APIcon;
		public TextMeshPro AP;
		public TextMeshPro Lv;
		public GameObject[] Panel;
		public GameObject InfoBtn;
		public GameObject SkillInfoBtnBG;
		public GameObject TouchGuide;
	}
	[System.Serializable]
	public class SNotice
	{
		public GameObject Profab;
		[HideInInspector]
		public Item_Skill_Notice Load;
		public Transform Pos;
	}

	[System.Serializable]
	public class SSkill
	{
		public GameObject Active;
		public ItemCardRenderer Icon;
		public GameObject CoolTimeGroup;
		public Animator CoolTimeAnim;
		public TextMeshPro CoolTime;
		public SNotice Notice;
		public GameObject SelectFX;
		public Animator BtnAnim;
		public GameObject[] InfoPanels;
	}
	[Serializable]
	public struct SDUI
	{
		public GameObject Prefab;  //IE_AutoDestory
		public Transform Bucket;
	}

	[SerializeField] SUI m_SUI;
	[SerializeField] SDUI m_SDUI;
	[SerializeField] SSkill m_Skill;
	[SerializeField] RenderAlpha_Controller m_Alpha;
	public SortingGroup m_Sort;
	// 0 : 쿨타임, 1 : 스킬 활성 알림
	[SerializeField] Transform[] m_NoticePanel = new Transform[2];
	public Vector3 ImgSize { get { return m_SUI.Profile[m_SUI.m_ProfilePos].transform.lossyScale; } }
	public Vector3 SkillInfoSize { get { return m_SUI.SkillInfoBtnBG.transform.lossyScale; } }
	public int m_TransverseAtkMode = 0;

	public int m_Pos;
	public CharInfo m_Info;
	public int m_SkillCoolTime;
	public bool m_SkillLock = false;
	IEnumerator m_Action = null;

	int m_TUTOAddLV { get { return TUTO.CheckUseCloneDeck() ? 99 : 0; } }
	public GameObject GetInfoBtn { get { return m_SUI.InfoBtn; } }
	public GameObject GetInfoBtnBG { get { return m_SUI.SkillInfoBtnBG; } }

	private void OnDestroy()
	{
		if (m_Skill.Notice.Load != null) Destroy(m_Skill.Notice.Load.gameObject);
	}
	public void SetData(int pos, CharInfo Info, int StartTransMode = -1) {
		m_SUI.Panel[0].SetActive(Info != null);
		m_SUI.Panel[1].SetActive(Info == null);
		if (Info == null)
		{
			//*없으면 잠금
			//gameObject.SetActive(false);
			return;
		}
		GetComponent<SortingGroup>().sortingOrder = 4;
		m_Pos = pos;
		m_Info = Info;
		m_SUI.m_ProfilePos = 0;
		m_TransverseAtkMode = StartTransMode;
		m_SUI.Profile[0].Init();
		m_SUI.Profile[1].Init();
		m_SUI.Profile[m_SUI.m_ProfilePos].gameObject.SetActive(false);
		SetProfileImg();
		m_Info.IS_SetEquip();
		m_Skill.Icon.Init();
		m_Skill.Icon.SetMainTexture(m_Info.m_Skill[0].m_TData.GetImg());
		m_Skill.BtnAnim.SetTrigger("Normal");

		m_SUI.AP.text = m_Info.GetNeedAP().ToString();
		m_SUI.Lv.text = (Info.m_LV + m_TUTOAddLV).ToString();

		// 스킬 쿨티임 초기셋팅
		m_SkillLock = false;
		m_Skill.Active.SetActive(false);

		if (STAGE == null) return;
		int cooltime = TDATA.GetSkill(Info.m_Skill[0].m_Idx).m_Cool;
		//시너지
		float? synergy2 = STAGE.m_User.GetSynergeValue(JobType.Researcher, 1);
		if (synergy2 != null) {
			cooltime = Mathf.Max(0, cooltime - Mathf.RoundToInt((float)synergy2));
			STAGE_USERINFO.ActivateSynergy(JobType.Researcher);
			Utile_Class.DebugLog_Value("Researcher 시너지 발동 " + "변화 전 -> 후 : 전 :" + (TDATA.GetSkill(Info.m_Skill[0].m_Idx).m_Cool).ToString() + " 후 : " + cooltime.ToString());
			//STAGE.m_User.m_SynergyUseCnt[JobType.Researcher]++;
		}
		m_SkillCoolTime = 0;
		for (int i = 0; i < 2; i++)
		{
			if (m_Skill.Notice.Load == null)
			{
				m_Skill.Notice.Load = Utile_Class.Instantiate(m_Skill.Notice.Profab, POPUP.GetWorldUIPanel()).GetComponent<Item_Skill_Notice>();
				m_Skill.Notice.Load.GetComponent<CanvasGroup>().alpha = 1f;
				m_Skill.Notice.Load.SetWorldPos(m_Skill.Notice.Pos);
			}

			m_Skill.Notice.Pos.gameObject.SetActive(false);
			m_Skill.Notice.Load.gameObject.SetActive(false);
		}
		m_Skill.CoolTimeGroup.SetActive(false); 
		SetSelectFX(false);
		ActiveTouchGuide(false);
		SetSkillInfoActive(true, true);
	}
	public void RefreshLvText() {
		if (m_Info == null) return;
		m_SUI.Lv.text = Mathf.Clamp(m_Info.m_LV + m_TUTOAddLV + STAGE_USERINFO.GetBuffValue(StageCardType.LevelUp), 1, m_Info.m_StgLvLimit).ToString();
	}
	public void RefreshAPText() {
		if (m_Info == null) return;
		m_SUI.AP.text = m_Info.GetNeedAP().ToString();
			//Mathf.Clamp(0, Mathf.RoundToInt(m_Info.m_Skill[0].GetSkillAP() 
			//* (1f - Math.Min(1,STAGE_USERINFO.GetBuffValue(StageCardType.APConsumDown))) 
			//+ Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ApPlus)))
			//, STAGE_USERINFO.m_AP[1]) .ToString();
	}
	public void SetProfileImg()
	{

		m_SUI.m_ProfilePos = 1 - m_SUI.m_ProfilePos;
		ItemCardRenderer profile = m_SUI.Profile[m_SUI.m_ProfilePos];
		if (!profile.gameObject.activeSelf) profile.gameObject.SetActive(true);
		profile.GetComponent<SortingGroup>().sortingOrder = 1;
		TSkillTable skill = TDATA.GetSkill(m_Info.m_TData.m_SkillIdx[(int)SkillType.Active]);
		switch(skill.m_Kind)
		{
		case SkillKind.TransverseAtk:
			if(m_TransverseAtkMode < 0) profile.SetMainTexture(m_Info.m_TData.GetPortrait());
			else profile.SetMainTexture(m_Info.m_TData.GetPortrait(m_TransverseAtkMode == 0 ? "M" : "W"));
			break;
		default:
			profile.SetMainTexture(m_Info.m_TData.GetPortrait());
			break;
		}
		profile.SetTexture("_BackGround", 1, BaseValue.CharBG(m_Info.m_Grade), null);
		profile.SetFloat("_DissolveValue", 0f);
	}

	public void SetAlpha(float alpha)
	{
		m_Alpha.Alpha = alpha;
		if (m_Skill.Notice.Load != null) m_Skill.Notice.Load.GetComponent<CanvasGroup>().alpha = alpha;
	}
	/// <summary> 사용 가능 이라는 문구 나오는거만 알파 따로 조절 </summary>
	public void SetSkillAlarmAlpha(float _val) {
		if (m_Skill.Notice.Load != null) m_Skill.Notice.Load.GetComponent<CanvasGroup>().alpha = _val;
	}
	public void SetDissolve(float _Amount)
	{
		m_SUI.Profile[m_SUI.m_ProfilePos].SetFloat("_DissolveValue", _Amount);
	}
	public void SetBackDissolve(float _Amount)
	{
		m_SUI.Profile[1 - m_SUI.m_ProfilePos].SetFloat("_DissolveValue", _Amount);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Skill
	public void SetSkillInfoActive(bool _active, bool _init = false) {
		for (int i = 0; i < m_Skill.InfoPanels.Length; i++) {
			m_Skill.InfoPanels[i].SetActive(_active);
			if (i == 1 && !_init) {
				m_Skill.BtnAnim.SetTrigger(IS_UseActiveSkill() ? "Active" : "Normal");
			}
		}
	}
	public void CheckSkillCoolTime(int AddTime)
	{
		m_SkillCoolTime -= AddTime;
		if (m_SkillCoolTime < 0) {
			m_SkillCoolTime = 0;
			return;
		}
		SetSkillCoolTimeUI();
		//for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
		//	STAGE.m_Chars[i].SetSkillCoolTimeUI();
		//}
	}
	public void SkillColoTimeInit()
	{
		m_SkillCoolTime = 0;
		SetSkillCoolTimeUI();
	}


	public void SetSkillCoolTime(int AddTime = 0)
	{
		int cooltime = m_Info.m_Skill[0].m_TData.m_Cool;
		//시너지
		float? synergy2 = STAGE.m_User.GetSynergeValue(JobType.Researcher, 1);
		if (synergy2 != null) {
			cooltime = Mathf.Max(0, cooltime - Mathf.RoundToInt((float)synergy2));
			STAGE_USERINFO.ActivateSynergy(JobType.Researcher);
			Utile_Class.DebugLog_Value("Researcher 시너지 발동 " + "변화 전 -> 후 : 전 :" + cooltime.ToString() + " 후 : " + cooltime.ToString());
			//STAGE.m_User.m_SynergyUseCnt[JobType.Researcher]++;
		}
		m_SkillCoolTime = STAGEINFO.m_TStage.GetMode(PlayType.NoCool) != null ? 0 : Math.Max(0, cooltime - AddTime);
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++)
			STAGE.m_Chars[i].SetSkillCoolTimeUI();
	}
	//public void SetCoolTimeUIOnOff(float _time, bool _on) {
	//	iTween.ValueTo(gameObject, iTween.Hash("from", _on ? 0f : 1f, "to", _on ? 1f : 0f, "time", _time, "onupdate", "TW_CoolTImeAlpha"));
	//}
	//void TW_CoolTImeAlpha(float _amount) {
	//	m_Skill.Notice[0].Load.GetComponent<CanvasGroup>().alpha = _amount;
	//}
	public void SetSkillCoolTimeUI()
	{
		// 처음엔 모두 꺼진상태로 시작하므로 여기서 막으며 안나옴
		if (m_SkillLock) return;
		//if (m_Info == null) return;
		if (m_Info.GetSkillLV(SkillType.Active) < 1) return;
		if(m_SkillCoolTime > 0)
		{
			//m_Skill.Notice[0].Load.SetCnt(m_SkillCoolTime);
			if (!m_Skill.CoolTimeGroup.activeSelf) {
				m_Skill.CoolTimeGroup.SetActive(true);
				StartCoroutine(IE_SkillCoolTimeAnim("Start"));
			}
			else StartCoroutine(IE_SkillCoolTimeAnim("Change"));

			m_Skill.CoolTime.text = m_SkillCoolTime.ToString();
			if (m_Skill.Notice.Pos.gameObject.activeSelf)
			{
				m_Skill.Notice.Load.SetActive(false, () => {
					m_Skill.Notice.Pos.gameObject.SetActive(false);
					m_Skill.Notice.Load.gameObject.SetActive(false);
					m_Skill.Notice.Load.GetComponent<CanvasGroup>().alpha = 0f;
					SkillActiveAction();
				});
			}
			else SkillActiveAction();
		}
		else {
			if (m_Skill.CoolTimeGroup.activeSelf) {
				StartCoroutine(IE_SkillCoolTimeAnim("End", () => {
					m_Skill.CoolTimeGroup.SetActive(false);
				}));
			}
			SkillActiveAction();
		}
		if (!IS_UseActiveSkill()) {
			m_Skill.Notice.Pos.gameObject.SetActive(false);
			m_Skill.Notice.Load.gameObject.SetActive(false);
			m_Skill.Notice.Load.GetComponent<CanvasGroup>().alpha = 0f;
			m_Skill.BtnAnim.SetTrigger("Normal");
		}
	}
	IEnumerator IE_SkillCoolTimeAnim(string _trig, Action _cb = null) {
		yield return new WaitWhile(() => !m_Skill.CoolTimeAnim.gameObject.activeInHierarchy);
		m_Skill.CoolTimeAnim.SetTrigger(_trig);

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Skill.CoolTimeAnim));

		_cb?.Invoke();
	}
	public void SkillActiveAction()
	{
		if(m_Alpha.Alpha == 1f)
			m_Skill.Notice.Load.GetComponent<CanvasGroup>().alpha = IS_UseActiveSkill() ? 1f : 0f;
		if (!m_Skill.Notice.Pos.gameObject.activeSelf)
		{
			m_Skill.Active.SetActive(IS_UseActiveSkill());
			m_Skill.Notice.Pos.gameObject.SetActive(true);
			//팝업 켜져있거나 알파 1 아니면 안나오게함
			m_Skill.Notice.Load.gameObject.SetActive(m_Skill.Notice.Load.GetComponent<CanvasGroup>().alpha == 1f && !POPUP.IS_PopupUI());
			if(m_Skill.Notice.Load.gameObject.activeSelf) m_Skill.Notice.Load.SetActive(true, null);
			m_Skill.BtnAnim.ResetTrigger(IS_UseActiveSkill() ? "Normal" : "Active");
			m_Skill.BtnAnim.SetTrigger(IS_UseActiveSkill() ? "Active" : "Normal");
			if(IS_UseActiveSkill())
				PlayEffSound(SND_IDX.SFX_0496);
		}
		else m_Skill.Notice.Load.PlayAni(Item_Skill_Notice.AniName.Change);

	}

	public bool IS_UseActiveSkill()
	{
		if (m_SkillLock) return false;
		if (m_Info == null) return false;
		if (STAGEINFO.m_TStage.m_AP < 0) return false;
		if (m_Info.GetSkillLV(SkillType.Active) < 1) return false;
		if (STAGE_USERINFO.m_AP[0] < m_Info.GetNeedAP())  return false;//행동력 부족시 사용 불가
		// 스킬 사용가능 여부
		return m_SkillCoolTime < 1;
	}
	public void SetAPUI(int _crntap) {
		if (m_Info == null) return;
		if (m_SkillLock)
			m_SUI.AP.color = m_SUI.APIcon.color = Utile_Class.GetCodeColor("#FF000C");
		else {
			int needap = m_Info.GetNeedAP(); //m_Info.m_Skill[0].GetSkillAP();
			//needap = Mathf.RoundToInt(needap * (1f - Math.Min(1, STAGE_USERINFO.GetBuffValue(StageCardType.APConsumDown))) + Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ApPlus)));
			m_SUI.AP.color = m_SUI.APIcon.color = needap <= _crntap ? Utile_Class.GetCodeColor("#FAE63F") : Utile_Class.GetCodeColor("#FF000C");
		}
		m_Skill.Active.SetActive(IS_UseActiveSkill());
		if (IS_UseActiveSkill()) SetSkillCoolTimeUI();
		else m_Skill.BtnAnim.SetTrigger("Normal");
	}

	public void BanChar(bool _ban) {
		m_SkillLock = _ban;
		m_SUI.Panel[1].SetActive(true);
	}
	public void ActiveTouchGuide(bool _active = true) {
		m_SUI.TouchGuide.SetActive(_active);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Action
	public void LearningAbility(TSkillTable skill) {
		m_Skill.Icon.Init();
		m_Skill.Icon.SetMainTexture(skill == null ? m_Info.m_Skill[0].m_TData.GetImg() : skill.GetImg());
	}
	public bool IS_LearningSkill(TSkillTable skill) {
		return m_Info.m_TData.m_SkillIdx[m_Info.IS_SetEquip() ? (int)SkillType.SetActive : (int)SkillType.Active] != skill.m_Idx;
	}

	public void Change_TransverseAtk(Action<Item_Stage_Char> EndCB = null)
	{
		StartCoroutine(Action_TransverseAtk(EndCB));
	}

	IEnumerator Action_TransverseAtk(Action<Item_Stage_Char> EndCB)
	{
		m_TransverseAtkMode = 1 - m_TransverseAtkMode;
		m_SUI.Profile[m_SUI.m_ProfilePos].GetComponent<SortingGroup>().sortingOrder = 2;
		SetProfileImg();
		iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", 0.5f, "easetype", "easeInQuart", "onupdate", "SetBackDissolve"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		m_SUI.Profile[1 - m_SUI.m_ProfilePos].gameObject.SetActive(false);
		EndCB?.Invoke(this);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Action
	public void Action(EItem_Stage_Char_Action act, float WaitTime = 0f, Action<Item_Stage_Char> EndCB = null, params object[] args)
	{
		if (m_Action != null)
		{
			StopCoroutine(m_Action);
			m_Action = null;
		}
		switch(act)
		{
		case EItem_Stage_Char_Action.FadeIn:
			m_Action = Action_FadeIn(WaitTime, (float)args[0], EndCB);
			break;
		case EItem_Stage_Char_Action.FadeOut:
			m_Action = Action_FadeOut(WaitTime, (float)args[0], EndCB);
			break;
		}

		if (m_Action != null) StartCoroutine(m_Action);
	}

	IEnumerator Action_FadeIn(float WaitTime, float MaxTime, Action<Item_Stage_Char> EndCB)
	{
		yield return new WaitForSeconds(WaitTime);
		iTween.ValueTo(gameObject, iTween.Hash("from", m_Alpha.Alpha, "to", 1f, "time", MaxTime, "easetype", "easeInQuart", "onupdate", "SetAlpha", "name", "FadeInOut"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		Action(EItem_Stage_Char_Action.None);
		EndCB?.Invoke(this);
	}

	IEnumerator Action_FadeOut(float WaitTime, float MaxTime, Action<Item_Stage_Char> EndCB)
	{
		yield return new WaitForSeconds(WaitTime);
		iTween.ValueTo(gameObject, iTween.Hash("from", m_Alpha.Alpha, "to", 0f, "time", MaxTime, "easetype", "easeInQuart", "onupdate", "SetAlpha", "name", "FadeInOut"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		Action(EItem_Stage_Char_Action.None);
		EndCB?.Invoke(this);
	}

	public bool GetSkillFrameActive() {
		return m_Skill.Active.activeSelf;
	}
	public void SetSkillFrameActive(bool _ative) {
		m_Skill.Active.SetActive(_ative);
	}
	public void DNAAlarm(OptionType _type) {
		Item_Stage_Char_DNA alarm = Utile_Class.Instantiate(m_SDUI.Prefab, m_SDUI.Bucket).GetComponent<Item_Stage_Char_DNA>();
		alarm.SetData(TDATA.GetDnaTable(_type));
	}
	public void SetSelectFX(bool _on) {
		m_Skill.SelectFX.SetActive(_on);
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 튜토리얼용
	public RectTransform GetNotice(int pos)
	{
		return (RectTransform)m_Skill.Notice.Load.transform;
	}

}
