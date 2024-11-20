using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutoUI : PopupBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct STalk
	{
		public GameObject Active;
		public GameObject Dlg;
		public Transform DlgPanel;
		public Image Icon;
		public TextMeshProUGUI Name;
		public Item_Talk_Talk Talk;
		public Button Btn;
	}
	public enum FocusAnim
	{
		Tab,
		Press
	}
	[System.Serializable]
	struct SFocus
	{
		public Image Focus;
		public CanvasGroup FXGroup;
		public Image[] OutImg;
		public GameObject Touch;
		public Animator Anim;
		public GameObject OnClick;
	}

	[System.Serializable]
	struct SDrag
	{
		public GameObject Active;
	}
	[Serializable]
	struct SCM
	{
		public GameObject Active;
		public TextMeshProUGUI Msg;
		public RectTransform Rect;
	}
	[System.Serializable]
	struct SUI
	{
		public SFocus Focus;
		public STalk Talk;
		public SDrag Drag;
		public SCM CM;
		public GameObject Frame;
		public float[] TalkPosY;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] Sprite[] m_FocusImg;
	[SerializeField] RectTransform m_CloneObjPanel;
	[SerializeField] Transform m_FocusTarget;
	Vector3 m_FocusGap;
	GameObject m_CloneObj;
	RectTransform m_CloneParent;
	int m_CloneSibling;
	int m_DlgIdx;
	int m_NextDlgIdx;
	Action m_DlgEndCB;
	Action m_CmCB;
	IEnumerator m_DlgTimer;
	IEnumerator m_TouchDelay;
	List<GameObject> m_FXs = new List<GameObject>();
	public RectTransform GetCMRect { get { return m_SUI.CM.Rect; } }
	public bool IS_Focus { get { return m_SUI.Focus.FXGroup.alpha == 1f ? true : false; } }
#pragma warning restore 0649

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		StartDlg(-1);
		SetTutoFrame();
		SetDrag();
		SetCM();
		SetStageSkillInfoOff();
	}
	private void Update()
	{
		if (m_FocusTarget != null)
		{
			m_SUI.Focus.Focus.rectTransform.position = m_FocusTarget.position + m_FocusGap;
			SetOutUI();
		}
	}
	void SetStageSkillInfoOff() {
		if (POPUP.GetMainUI().m_Popup == PopupName.Stage) POPUP.GetMainUI().GetComponent<Main_Stage>().ShowSkillInfo(false);
	}

	public void SetFocus(int Mode, Vector3 ScreenPos, float W, float H, bool SportAction = false, FocusAnim _anim = FocusAnim.Tab)
	{
		if(ScreenPos == Vector3.zero && W == 0f && H == 0f) { }//초기화
		else {//생성
			PlayEffSound(SportAction ? SND_IDX.SFX_9531 : SND_IDX.SFX_9530);
		}
		m_FocusTarget = null;
		if (Mode == 0) {
			m_SUI.Focus.Focus.sprite = null;
			m_SUI.Focus.Focus.color = new Color(1f, 1f, 1f, 0f);
			m_SUI.Focus.FXGroup.alpha = 0f;
		}
		else {
			m_SUI.Focus.Focus.sprite = m_FocusImg[Mode - 1];
			m_SUI.Focus.Focus.color = new Color(1f, 1f, 1f, 1f);
			m_SUI.Focus.FXGroup.alpha = 1f;
		}

		if (Mode == 2)
		{
			if (W >= 1) W += 68 * 2;
			if (H >= 1) H += 68 * 2;
		}

		RectTransform rtf = m_SUI.Focus.Focus.rectTransform;
		rtf.sizeDelta = new Vector2(W, H);
		rtf.position = ScreenPos;

		m_SUI.Focus.Touch.SetActive(SportAction);
		if (SportAction) m_SUI.Focus.Anim.SetTrigger(_anim.ToString());
		SetOutUI();
	}

	public void SetFocus(int Mode, Transform target, Vector3 gap, float W, float H, bool SportAction = false, FocusAnim _anim = FocusAnim.Tab)
	{
		if(typeof(RectTransform) == target.GetType()) {
			RectTransform rtf = (RectTransform)target;
			gap += new Vector3(W * (0.5f - rtf.pivot.x) * POPUP.GetCC.GetScaleW, H * (rtf.pivot.y - 0.5f) * POPUP.GetCC.GetScaleH, 0f);
		}
		SetFocus(Mode, target.position, W, H, SportAction, _anim);
		m_FocusTarget = target;
		m_FocusGap = gap;
		if (m_FocusTarget != null)
		{
			m_SUI.Focus.Focus.rectTransform.position = m_FocusTarget.position + m_FocusGap;
			SetOutUI();
		}
	}

	void SetOutUI()
	{
		RectTransform focus = m_SUI.Focus.Focus.rectTransform;
		RectTransform left = m_SUI.Focus.OutImg[0].rectTransform;
		RectTransform right = m_SUI.Focus.OutImg[1].rectTransform;
		RectTransform bottom = m_SUI.Focus.OutImg[2].rectTransform;
		RectTransform up = m_SUI.Focus.OutImg[3].rectTransform;

		Rect rect = focus.rect;
		float scrw = Screen.width / Canvas_Controller.SCALE;
		float scrh = Screen.height / Canvas_Controller.SCALE;
		float scrhw = scrw * 0.5f;
		float scrhh = scrh * 0.5f;
		left.sizeDelta = new Vector2(scrhw + focus.localPosition.x + rect.x, 0);
		right.sizeDelta = new Vector2(scrhw - focus.localPosition.x + rect.x, 0);

		bottom.localPosition = new Vector3(-focus.localPosition.x, bottom.localPosition.y, 0);
		bottom.sizeDelta = new Vector2(scrw - rect.width, scrhh + focus.localPosition.y + rect.y);
		up.localPosition = new Vector3(-focus.localPosition.x, up.localPosition.y, 0);
		up.sizeDelta = new Vector2(scrw - rect.width, scrhh - focus.localPosition.y + rect.y);

		m_SUI.Focus.Touch.transform.position = m_SUI.Focus.Focus.transform.position;
	}

	public void StartDlg(int idx, Action EndCB = null, float clickdelay = 0f, int _ypos = 0)
	{
		if (m_TouchDelay != null) StartCoroutine(m_TouchDelay);
		if(idx < 0)
		{
			m_SUI.Talk.Active.SetActive(false);
			return;
		}
		m_DlgIdx = idx;
		m_DlgEndCB = EndCB;


		m_SUI.Talk.Active.SetActive(true);

		if (idx > 0)
		{
			m_SUI.Talk.Dlg.SetActive(true);
			m_SUI.Talk.DlgPanel.localPosition = new Vector3(0f, MAIN.m_UIMng.GetCanvasH() * m_SUI.TalkPosY[_ypos], 0f);
			TDialogTable dlg = TDATA.GetDialogTable(m_DlgIdx);
			m_NextDlgIdx = dlg.m_NextDLIdx;
			ShowDlg();
		}
		else
		{
			m_NextDlgIdx = 0;
			m_SUI.Talk.Dlg.SetActive(false);
		}
		if (clickdelay >= 1)
		{
			m_TouchDelay = TouchDelay(clickdelay);
			StartCoroutine(m_TouchDelay);
		}
		else ActiveTalkTauch(EndCB != null);
	}
	public bool IS_TalkAction() {
		return m_SUI.Talk.Talk.IsAction();
	}
	void ActiveTalkTauch(bool Active)
	{
		m_SUI.Talk.Btn.image.raycastTarget = Active;
		m_SUI.Talk.Btn.interactable = Active;
	}
	public void SetDrag(bool _on = false) {
		m_SUI.Drag.Active.SetActive(_on);
	}
	public void SetCM(bool _on = false, string _msg = null, Action _cb = null) {
		m_SUI.CM.Active.SetActive(_on);
		m_SUI.CM.Msg.text = _msg;
		m_CmCB = _cb;
	}
	public void CopyParentInfo(RectTransform parent)
	{
		m_CloneParent = parent;
		m_CloneObjPanel.anchorMin = parent.anchorMin;
		m_CloneObjPanel.anchorMax = parent.anchorMax;
		m_CloneObjPanel.rotation = parent.rotation;
		m_CloneObjPanel.sizeDelta = parent.sizeDelta;
		m_CloneObjPanel.localScale = parent.localScale;
		m_CloneObjPanel.position = parent.position;
	}
	public void SetTutoFrame(bool _on = true) {
		m_SUI.Frame.SetActive(_on);
	}
	public void UIClone()
	{
		RemoveClone();
		if (TUTO.IsTuto(TutoKind.EquipCharLVUP))
		{
			TutoType_EquipCharLVUP state = TUTO.GetTutoState<TutoType_EquipCharLVUP>();
			switch (state)
			{
			case TutoType_EquipCharLVUP.Select_CharInfo_Menu:
				m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Character);
				break;
			}
		}
		else if (TUTO.IsTuto(TutoKind.Making))
		{
			TutoType_Making state = TUTO.GetTutoState<TutoType_Making>();
			switch (state)
			{
			case TutoType_Making.Select_PDA:
				m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.PDA);
				break;
			}
		}
		//else if (TUTO.IsTuto(TutoKind.Factory))
		//{
		//	TutoType_Factory state = TUTO.GetTutoState<TutoType_Factory>();
		//	switch (state)
		//	{
		//	case TutoType_Factory.Select_Dungeon_Menu:
		//		m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Dungeon);
		//		break;
		//	}
		//}
		//else if (TUTO.IsTuto(TutoKind.Academy))
		//{
		//	TutoType_Academy state = TUTO.GetTutoState<TutoType_Academy>();
		//	switch (state)
		//	{
		//	case TutoType_Academy.Select_Dungeon_Menu:
		//		m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Dungeon);
		//		break;
		//	}
		//}
		//else if (TUTO.IsTuto(TutoKind.Subway))
		//{
		//	TutoType_Subway state = TUTO.GetTutoState<TutoType_Subway>();
		//	switch (state)
		//	{
		//	case TutoType_Subway.DLG_5040:
		//		m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Dungeon);
		//		break;
		//	}
		//}
		else if (TUTO.IsTuto(TutoKind.PVP_Main))
		{
			TutoType_PVP_Main state = TUTO.GetTutoState<TutoType_PVP_Main>();
			switch (state)
			{
			case TutoType_PVP_Main.DLG_5050:
				m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Dungeon);
				break;
			}
		}
		//else if (TUTO.IsTuto(TutoKind.Bank))
		//{
		//	TutoType_Bank state = TUTO.GetTutoState<TutoType_Bank>();
		//	switch (state)
		//	{
		//	case TutoType_Bank.Select_Dungeon_Menu:
		//		m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Dungeon);
		//		break;
		//	}
		//}
		else if (TUTO.IsTuto(TutoKind.Research))
		{
			TutoType_Research state = TUTO.GetTutoState<TutoType_Research>();
			switch (state)
			{
			case TutoType_Research.Select_PDA:
				m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.PDA);
				break;
			}
		}
		//else if (TUTO.IsTuto(TutoKind.Serum))
		//{
		//	TutoType_Serum state = TUTO.GetTutoState<TutoType_Serum>();
		//	switch (state)
		//	{
		//	case TutoType_Serum.Select_CharInfo_Menu:
		//		m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Character);
		//		break;
		//	}
		//}
		//else if (TUTO.IsTuto(TutoKind.Tower))
		//{
		//	TutoType_Tower state = TUTO.GetTutoState<TutoType_Tower>();
		//	switch (state)
		//	{
		//	case TutoType_Tower.Select_Dungeon_Menu:
		//		m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Dungeon);
		//		break;
		//	}
		//}
		else if (TUTO.IsTuto(TutoKind.DNA))
		{
			TutoType_DNA state = TUTO.GetTutoState<TutoType_DNA>();
			switch (state)
			{
			case TutoType_DNA.Select_CharInfo_Menu:
				m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Character);
				break;
			}
		}
		//else if (TUTO.IsTuto(TutoKind.Cemetery))
		//{
		//	TutoType_Cemetery state = TUTO.GetTutoState<TutoType_Cemetery>();
		//	switch (state)
		//	{
		//	case TutoType_Cemetery.Select_Dungeon_Menu:
		//		m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Dungeon);
		//		break;
		//	}
		//}
		else if (TUTO.IsTuto(TutoKind.Zombie))
		{
			TutoType_Zombie state = TUTO.GetTutoState<TutoType_Zombie>();
			switch (state)
			{
			case TutoType_Zombie.Select_PDA:
				m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.PDA);
				break;
			}
		}
		else if (TUTO.IsTuto(TutoKind.DNA_Make))
		{
			TutoType_DNA_Make state = TUTO.GetTutoState<TutoType_DNA_Make>();
			switch (state)
			{
			case TutoType_DNA_Make.DLG_5030:
				m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.PDA);
				break;
			}
		}
		else if (TUTO.IsTuto(TutoKind.Adventure))
		{
			TutoType_Adventure state = TUTO.GetTutoState<TutoType_Adventure>();
			switch (state)
			{
			case TutoType_Adventure.Select_PDA:
				m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.PDA);
				break;
			}
		}
		//else if (TUTO.IsTuto(TutoKind.University))
		//{
		//	TutoType_University state = TUTO.GetTutoState<TutoType_University>();
		//	switch (state)
		//	{
		//	case TutoType_University.Select_Dungeon_Menu:
		//		m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetMenuBtn(MainMenuType.Dungeon);
		//		break;
		//	}
		//}
		//else if (TUTO.IsTuto(TutoKind.Hard_Stage))
		//{
		//	TutoType_Hard state = TUTO.GetTutoState<TutoType_Hard>();
		//	switch (state)
		//	{
		//	case TutoType_Hard.Select_Hard:
		//		m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetStageDiffBtn(StageDifficultyType.Hard);
		//		break;
		//	}
		//}
		//else if (TUTO.IsTuto(TutoKind.Nightmare_Stage))
		//{
		//	TutoType_Nightmare state = TUTO.GetTutoState<TutoType_Nightmare>();
		//	switch (state)
		//	{
		//	case TutoType_Nightmare.Select_Nightmare:
		//		m_CloneObj = ((Main_Play)POPUP.GetMainUI()).GetStageDiffBtn(StageDifficultyType.Nightmare);
		//		break;
		//	}
		//}


		if (m_CloneObj != null)
		{
			m_CloneSibling = m_CloneObj.transform.GetSiblingIndex();
			CopyParentInfo((RectTransform)m_CloneObj.transform.parent);
			Vector3 pos = m_CloneObj.transform.position;
			m_CloneObj.transform.SetParent(m_CloneObjPanel);
			m_CloneObj.transform.position = pos;
		}
	}

	IEnumerator CloneAniCheck()
	{
		while(true)
		{
			yield return new WaitForEndOfFrame();
			CopyParentInfo(m_CloneParent);
		}
	}

	public void RemoveClone()
	{
		if (m_CloneObj == null) return;

		Vector3 pos = m_CloneObj.transform.position;
		m_CloneObj.transform.SetParent(m_CloneParent);
		m_CloneObj.transform.position = pos;
		m_CloneObj.transform.SetSiblingIndex(m_CloneSibling);
		m_CloneObj = null;
		StopCoroutine("CloneAniCheck");
	}

	void ShowDlg()
	{
		TDialogTable dlg = TDATA.GetDialogTable(m_DlgIdx);
		TTalkerTable talker = dlg.GetTalker();
		m_SUI.Talk.Icon.sprite = talker.GetSprPortrait();
		m_SUI.Talk.Name.text = talker.GetName();
		m_SUI.Talk.Talk.SetData(dlg.m_TalkEmotion, string.Format(dlg.GetDesc(), USERINFO.m_Name), true, 1.5f);
		PlayEffSound(SND_IDX.SFX_9510);
		// 토크 종료 체크 후 0.5초정도 터치 막기
		m_Check = TalkActionCheck();
		StartCoroutine(m_Check);
	}

	IEnumerator m_Check = null;
	void Next()
	{
		if (m_Check != null) return;
		if (m_NextDlgIdx == 0)
		{
			ActiveTalkTauch(false);
			m_DlgEndCB?.Invoke();
			return;
		}
		m_DlgIdx = m_NextDlgIdx;
		TDialogTable dlg = TDATA.GetDialogTable(m_DlgIdx);
		m_NextDlgIdx = dlg.m_NextDLIdx;
		ShowDlg();
	}

	IEnumerator TouchDelay(float delay)
	{
		m_SUI.Talk.Btn.image.raycastTarget = true;
		m_SUI.Talk.Btn.interactable = false;
		yield return new WaitForSeconds(delay);
		ActiveTalkTauch(true);
	}

	public void OnTouchDlg()
	{
		if (m_SUI.Talk.Talk.IsAction())
		{
			m_SUI.Talk.Talk.Set_Action_Finishied();
			return;
		}
		Next();
	}

	IEnumerator TalkActionCheck()
	{
		// 토크 종료 체크
		yield return new WaitWhile(() => m_SUI.Talk.Talk.IsAction());
		// 0.5초 다음 토크 막기
		yield return new WaitForSeconds(0.5f);
		m_Check = null;
	}

	public void OnTouchCM() {
		m_CmCB?.Invoke();
	}
	public List<GameObject> SetFX(List<Transform> _parents, string _name, Vector3 _scale = default(Vector3)) {
		for(int i = 0; i < _parents.Count; i++) {
			GameObject fx = UTILE.LoadPrefab(_name, true, _parents[i]);
			fx.transform.localScale = _scale;
			m_FXs.Add(fx);
		}
		return m_FXs;
	}
	public void RemoveFX() {
		for (int i = m_FXs.Count - 1; i > -1;i--) {
			Destroy(m_FXs[i]);
		}
		m_FXs.Clear();
	}
}
