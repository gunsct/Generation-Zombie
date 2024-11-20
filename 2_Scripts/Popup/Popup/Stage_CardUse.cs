using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stage_CardUse : PopupBase
{
	public enum AniName
	{
		In = 0,
		Out,
		LearningAbility
	}
	[System.Serializable]
	public struct SUI
	{
		public Animator Ani;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Info;
		public Image Icon;
		public Image IconBG;
		public Image Area;
		public GameObject SkillInfoGroup;
		public TextMeshProUGUI AP;
		public TextMeshProUGUI CoolTime;
		public TextMeshProUGUI Title;
		public GameObject[] Btns;	//0:ok,1:cancle

		public GameObject[] Panels;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator EndRoutine;
	Action<int> m_CancleCB;
	Action m_AniCB;
	bool Is_Select = false;
	bool Is_ActiveOk = false;
	public bool Is_OkBtnActive { get { return Is_ActiveOk; } } //m_SUI.Btns[0].activeSelf;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_SUI.Name.text = (string)aobjValue[0];
		m_SUI.Info.text = (string)aobjValue[1];
		Sprite icon = (Sprite)aobjValue[2];
		Sprite area = (Sprite)aobjValue[3];
		bool setskill = (bool)aobjValue[4];
		int ap = (int)aobjValue[5];
		int cooltime = (int)aobjValue[6];
		m_SUI.Name.gameObject.SetActive(ap > -1 && cooltime > -1);
		bool select = (bool)aobjValue[7];
		bool cancle = (bool)aobjValue[8];
		Is_ActiveOk = !TUTO.IsTutoPlay() && !select;
		//SetCancleBtn(0, false);
		SetCancleBtn(0, !TUTO.IsTutoPlay() && !select && cancle);//캔슬 여부(플레이어 스킬은 즉발아닌것)
		SetCancleBtn(1, !TUTO.IsTutoPlay() && cancle);//캔슬 여부(플레이어 스킬은 즉발아닌것)
		m_CancleCB = aobjValue.Length > 9 ? (Action<int>)aobjValue[9] : null;

		m_SUI.Icon.gameObject.SetActive(icon != null);
		m_SUI.Icon.sprite = icon;
		m_SUI.Icon.color = Utile_Class.GetCodeColor(setskill ? "#7B7B7B" : "#87753C");
		m_SUI.Area.gameObject.SetActive(area != null);
		m_SUI.Area.sprite = area;
		m_SUI.Icon.gameObject.SetActive(ap > -1 && cooltime > -1);
		m_SUI.IconBG.sprite = UTILE.LoadImg(string.Format("Card/Frame/BG_Skill{0}", setskill ? "_Enhanced" : string.Empty), "png");
		m_SUI.IconBG.gameObject.SetActive(ap > -1 && cooltime > -1);
		m_SUI.SkillInfoGroup.SetActive(ap > -1 && cooltime > -1);
		m_SUI.AP.text = ap.ToString();
		m_SUI.CoolTime.text = cooltime.ToString();
		m_SUI.Title.text = select ? TDATA.GetString(45) : (ap > -1 && cooltime > -1 ? TDATA.GetString(1089) : TDATA.GetString(1090));

		PlayAni(AniName.In, () => {
			if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.Delay_SkillUse)) TUTO.Next();
		});
		if (TUTO.IsTutoPlay()) 
			POPUP.StartTutoTimer(() => { Click_OkCancle(0); }, 3f);
	}

	public void Click_OkCancle(int _val) {
		if (Is_Select) return;
		Is_Select = true;
		SetCancleBtn(0, false);
		SetCancleBtn(1, false);
		m_CancleCB?.Invoke(_val);
	}
	public void SetCancleBtn(int _pos, bool _on) {
		m_SUI.Btns[_pos].SetActive(_on);
	}
	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (EndRoutine != null) return;
		EndRoutine = CloseCheck(Result);
		StartCoroutine(EndRoutine);
	}

	public void PlayAni(AniName name, Action EndCB = null)
	{
		m_SUI.Ani.SetTrigger(name.ToString());
		if (EndCB != null) StartCoroutine(AniEndCheck(EndCB));
	}

	IEnumerator AniEndCheck(Action EndCB)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Ani));
		EndCB?.Invoke();
	}

	IEnumerator CloseCheck(int Result)
	{
		PlayAni(AniName.Out);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Ani));
		if (TUTO.IsTuto(TutoKind.Stage_401, (int)TutoType_Stage_401.Delay_UseItem)) TUTO.Next();
		base.Close(Result);
	}

	public GameObject GetInfoPanel()
	{
		return m_SUI.Panels[0];
	}
}
