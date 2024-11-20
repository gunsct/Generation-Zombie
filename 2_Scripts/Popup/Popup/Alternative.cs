using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;

public class Alternative : PopupBase
{
	public enum State
	{
		Start = 0,
		Turo,
		Select,
		Desc,
		Result,
		PrologueEnd,
		AlternativeEnd
	}
	public enum DrageType
	{
		None = 0,
		Vertical,
		Horizontal
	}
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI[] QuestionTxt;
		public ScrollRect DescScroll;
		public Transform Card;
		public Transform CardBucket;
		public Image[] CardImg;//0센터1좌2우
		public Image CardGrad;
		public TextMeshProUGUI SelectYes;
		public TextMeshProUGUI SelectNo;
		public Sprite[] SelectTFImg;
		public Animator MainAnim;
		public Animator DescOnlyAnim;
		public Animator FingerIconAnim;
		public Animator SelectAnim;
		public Animator SwipeAnimS;
		public Animator SwipeAnimD;
		public Animator DescViewAnim;
		public Animator ResultAnim;
		public Image ResultImg;
		public Image ResultBG;
		public TextMeshProUGUI ResultTxt;
	}

	readonly float RATIO_MUL = 10f;
	readonly float SELECT_ANGLE = 2.5f;

	[SerializeField]
	SUI m_SUI;
	[SerializeField] State m_State;
	[SerializeField] DrageType m_DrageType;
	AlternativeMode m_Mode;
	List<int> m_Idxs = new List<int>();
	int m_QstPage = 0;
	int m_QstCnt = 1;
	TAlternativeTable m_Table;

	Vector3 m_CardInitPos;
	Coroutine m_AnimCor;
	Coroutine m_DescAnimCor;
	Vector2 m_TouchDownPos, m_TouchDragPos;
	float m_SelectAngle;
	bool m_SelectYN;
	bool m_DragDesc = false;
#if NOT_USE_NET
	List<int> m_GetCharIdx = new List<int>();
#endif
	public List<LS_Web.REQ_ALTERNATIVE_INFO> SelectInfo = new List<LS_Web.REQ_ALTERNATIVE_INFO>();

	private void Update() {
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
#else
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
		{
			if (m_State == State.Select || m_State == State.Desc)
				m_TouchDownPos = Input.mousePosition;
		}
#if UNITY_EDITOR
		else if (Input.GetMouseButton(0))
#else
		else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
#endif
		{
			//가로 움직일때는 때기 전까지 가로모드고 세로도 마찬가지
			m_TouchDragPos = Input.mousePosition;
			Vector2 drag = m_TouchDragPos - m_TouchDownPos;
			if (Math.Abs(drag.normalized.x) > Math.Abs(drag.normalized.y) && m_DrageType == DrageType.None) {//가로 드래그, 좌-우+
				if (Math.Abs(drag.x / Screen.width) > 0.1f)
					m_DrageType = DrageType.Horizontal;
			}
			else if (Math.Abs(drag.normalized.x) < Math.Abs(drag.normalized.y) && m_DrageType == DrageType.None) {//세로 드래그
				if (Math.Abs(drag.y / Screen.height) > 0.1f)
					m_DrageType = DrageType.Vertical;
			}
			if (m_DrageType == DrageType.None || m_DrageType == DrageType.Horizontal) {
				if (m_State == State.Select) {
					float ratiox = Mathf.Clamp(Math.Abs(drag.x), 0f, Screen.width) / Screen.width;
					if (drag.normalized.x > 0f) {//우
						m_SelectAngle = Mathf.Clamp(ratiox * RATIO_MUL * -SELECT_ANGLE, -10, 0f);
						CardRotZ(m_SelectAngle);
						SelectNoColor(ratiox * RATIO_MUL);
						SelectYesColor(0f);
						CardCImgAlpha(1f - 3f * ratiox);
						CardLImgAlpha(0f);
						CardRImgAlpha(3f * ratiox);
					}
					else if (drag.normalized.x < 0f) {//좌
						m_SelectAngle = Mathf.Clamp(ratiox * RATIO_MUL * SELECT_ANGLE, 0f, 10);
						CardRotZ(m_SelectAngle);
						SelectYesColor(ratiox * RATIO_MUL);
						SelectNoColor(0f);
						CardCImgAlpha(1f - 3f * ratiox);
						CardRImgAlpha(0f);
						CardLImgAlpha(3f * ratiox);
					}
					SelectGrad(3f * ratiox);
					CardBucketScale(Mathf.Clamp(1.1f - ratiox * 0.2f, 1f, 1.1f));
				}
			}

			if (m_DrageType == DrageType.Vertical) {
				if ((m_State == State.Select || m_State == State.Desc) && Math.Abs(drag.y / Screen.height) > 0.1f && !m_DragDesc) {
					if (drag.normalized.y > 0f) {//위
						ClickDescOnOff(false);
					}
					else if (drag.normalized.y < 0f) {//아래
						ClickDescOnOff(true);
					}
					CardInit();
				}
			}
		}
#if UNITY_EDITOR
		else if(Input.GetMouseButtonUp(0))
#else
		else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
#endif
		{
			switch (m_State) {
				case State.Start:
					if (m_SUI.DescOnlyAnim.gameObject.activeSelf) {//desc 스킵
						if (m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).IsName("1_DescOnly_Start") && m_SUI.QuestionTxt[0].GetComponent<TippingText>().m_End == false) {
							m_SUI.QuestionTxt[0].GetComponent<TippingText>().Skip();
							m_SUI.DescOnlyAnim.CrossFade("1_DescOnly_Start", 0f, 0, 1f, 0f);
						}
						else if (m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).IsName("1_DescOnly_Start") && m_SUI.QuestionTxt[0].GetComponent<TippingText>().m_End == true) {
							m_SUI.DescOnlyAnim.SetTrigger("End");
						}
						else if (m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).IsName("1_DescOnly_Start_Hand") && m_SUI.QuestionTxt[0].GetComponent<TippingText>().m_End == false) {
							m_SUI.QuestionTxt[0].GetComponent<TippingText>().Skip();
							m_SUI.DescOnlyAnim.CrossFade("1_DescOnly_Start_Hand", 0f, 0, 1f, 0f);
						}
						else if (m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).IsName("1_DescOnly_Start_Hand") && m_SUI.QuestionTxt[0].GetComponent<TippingText>().m_End == true && m_QstPage == 0) {
							m_QstPage++;
						}
						else if (m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).IsName("1_DescOnly_Start_Hand") && m_SUI.QuestionTxt[0].GetComponent<TippingText>().m_End == false && m_QstPage == 1) {
							m_SUI.QuestionTxt[0].GetComponent<TippingText>().Skip();
						}
						else if(m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).IsName("1_DescOnly_Start_Hand") && m_SUI.QuestionTxt[0].GetComponent<TippingText>().m_End == true) {
							m_SUI.DescOnlyAnim.SetTrigger("End");
							m_SUI.FingerIconAnim.SetTrigger("End");
						}
					}
					break;
				case State.Select:
					if (m_SelectAngle >= SELECT_ANGLE || m_SelectAngle <= -SELECT_ANGLE) {
						SetState(State.Result);
						if (m_AnimCor != null)
							StopCoroutine(m_AnimCor);
						m_AnimCor = StartCoroutine(SelectAnim(m_SelectAngle > 0 ? true : false));
						m_SelectAngle = 0f;
					}
					else
						CardInit();
					break;
				case State.Result:
					if (m_SUI.ResultAnim.GetCurrentAnimatorStateInfo(0).IsName("4_Result_Start") && m_SUI.ResultTxt.GetComponent<TippingText>().m_End == false) {
						m_SUI.ResultTxt.GetComponent<TippingText>().Skip();
						m_SUI.ResultAnim.CrossFade("4_Result_Start", 0f, 0, 1f, 0.1f);
					}
					else if (m_SUI.ResultAnim.GetCurrentAnimatorStateInfo(0).IsName("4_Result_Start") && m_SUI.ResultTxt.GetComponent<TippingText>().m_End == true) {
						m_SUI.ResultAnim.SetTrigger("End");
					}
					break;
				case State.PrologueEnd:
					if (m_SUI.DescViewAnim.GetCurrentAnimatorStateInfo(0).IsName("3_DescView_Prologue") && m_SUI.QuestionTxt[1].GetComponent<TippingText>().m_End == false) {
						m_SUI.QuestionTxt[1].GetComponent<TippingText>().Skip();
						m_SUI.DescViewAnim.CrossFade("3_DescView_Prologue", 0f, 0, 1f, 0.1f);
					}
					else if (m_SUI.DescViewAnim.GetCurrentAnimatorStateInfo(0).IsName("3_DescView_Prologue") && m_SUI.QuestionTxt[1].GetComponent<TippingText>().m_End == true) {
						m_AnimCor = StartCoroutine(AlternativeEnd());
					}
					break;
			}
			m_DrageType = DrageType.None;
			m_TouchDownPos = Vector2.zero;
		}
	}

	public void DescDrag(bool _drag) {
		m_DragDesc = _drag;
	}
	void SetState(State _state) {
		m_State = _state;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="popup"></param>
	/// <param name="cb"></param>
	/// <param name="aobjValue">0:AlternativeMode(모드), 0:List<int> 테이블 인덱스들</int></param>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_Mode = (AlternativeMode)aobjValue[0];
		m_Idxs = (List<int>)aobjValue[1];
		m_CardInitPos = m_SUI.Card.position;
		SetQuestion();
	}
	void SetQuestion() {
		SetState(State.Start);
		//리셋
		if (m_AnimCor != null)
			StopCoroutine(m_AnimCor);
		if (m_DescAnimCor != null)
			StopCoroutine(m_DescAnimCor);

		m_SUI.DescOnlyAnim.gameObject.SetActive(true);
		m_SUI.SelectAnim.gameObject.SetActive(false);
		m_SUI.DescViewAnim.gameObject.SetActive(false);
		m_SUI.ResultAnim.gameObject.SetActive(false);

		m_SUI.Card.position = m_CardInitPos;
		CardRotZ(0f);
		SelectNoColor(0f);
		SelectYesColor(0f);
		CardBucketScale(1.1f);
		SelectGrad(0f);
		CardCImgAlpha(1f);
		CardLImgAlpha(0f);
		CardRImgAlpha(0f);

		//뉴 데이터셋
		m_Table = TDATA.GetAlternativeTable(m_Idxs[m_QstCnt - 1]);
		m_SUI.SelectYes.text = m_Table.GetSelectTF(true);
		m_SUI.SelectNo.text = m_Table.GetSelectTF(false);
		m_SUI.CardImg[0].sprite = m_Table.GetQuestionImg();
		m_SUI.SelectTFImg[0] = m_Table.GetSelectTFImg(true);
		m_SUI.CardImg[1].sprite = m_Table.GetSelectTFImg(true);
		m_SUI.SelectTFImg[1] = m_Table.GetSelectTFImg(false);
		m_SUI.CardImg[2].sprite = m_Table.GetSelectTFImg(false);

		StartCoroutine(StartAnim());
	}
	/// <summary> 시작 애니 </summary>
	IEnumerator StartAnim() {
		m_QstPage = 0;
		m_SUI.MainAnim.SetTrigger("Start");
		m_SUI.DescOnlyAnim.gameObject.SetActive(true);
		yield return new WaitForEndOfFrame();
		if (m_Mode == AlternativeMode.Prologue && m_QstCnt == 1) {//151,152,301
			int txtidx = 0;
			for (int i = 0; i < 3; i++) {
				if (i == 0) txtidx = 151;
				else if (i == 1) txtidx = 152;
				else txtidx = 301;
				m_SUI.QuestionTxt[0].GetComponent<TippingText>().SetData(TDATA.GetString(ToolData.StringTalbe.Etc, txtidx));
				m_SUI.DescOnlyAnim.SetTrigger("Start");
				yield return new WaitForEndOfFrame();
				yield return new WaitUntil(() => m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
				yield return new WaitUntil(() => m_SUI.QuestionTxt[0].GetComponent<TippingText>().m_End == true);
				yield return new WaitUntil(() => m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).IsName("1_DescOnly_End") && m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
			}
		}
		if(m_Table.GetQuestion(m_QstPage) != null) m_SUI.QuestionTxt[0].GetComponent<TippingText>().SetData(m_Table.GetQuestion(m_QstPage));
		m_SUI.DescOnlyAnim.SetTrigger("Hand");
		yield return new WaitForEndOfFrame();
		m_SUI.FingerIconAnim.SetTrigger(m_QstCnt.ToString());
		yield return new WaitForEndOfFrame();

		if (m_Table.GetQuestion(m_QstPage + 1) != null) {
			yield return new WaitUntil(() => m_SUI.QuestionTxt[0].GetComponent<TippingText>().m_End == true && m_QstPage == 1);
			m_SUI.QuestionTxt[0].GetComponent<TippingText>().SetData(m_Table.GetQuestion(m_QstPage));
		}
		yield return new WaitUntil(() => m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).IsName("1_DescOnly_End") && m_SUI.DescOnlyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
		m_SUI.DescOnlyAnim.gameObject.SetActive(false);
		StartCoroutine(ReadyAnim());
	}
	/// <summary> 튜토리얼이나 바로 선택지 애니 </summary>
	IEnumerator ReadyAnim() {
		m_SUI.SelectAnim.gameObject.SetActive(true);
		if (PlayerPrefs.GetInt($"Alternative_Tuto_{USERINFO.m_UID}", 0) == 0) {
			SetState(State.Turo);
			m_SUI.QuestionTxt[1].text = string.Format("{0}{1}", m_Table.GetQuestion(0), m_Table.GetQuestion(1));
			m_SUI.SelectAnim.SetTrigger("Tuto");
			//yield return new WaitForEndOfFrame();
			//yield return new WaitWhile(() => m_SUI.SelectAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.78f);
			//m_SUI.SwipeAnimS.SetTrigger("TtB");
			//yield return new WaitForEndOfFrame();
			//yield return new WaitUntil(() => m_SUI.SelectAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
			//m_SUI.SelectAnim.SetTrigger("Out");
			//m_SUI.DescViewAnim.gameObject.SetActive(true);
			//m_SUI.DescScroll.verticalNormalizedPosition = 1;
			//m_SUI.DescViewAnim.SetTrigger("Tuto");
			//yield return new WaitForEndOfFrame();
			//yield return new WaitUntil(() => m_SUI.DescViewAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35f);
			//m_SUI.SwipeAnimD.SetTrigger("BtT");
			//yield return new WaitForEndOfFrame();
			//yield return new WaitUntil(() => m_SUI.DescViewAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
			//m_SUI.SelectAnim.SetTrigger("In");
			//m_SUI.DescViewAnim.SetTrigger("Out");
			//yield return new WaitForEndOfFrame();
			//yield return new WaitUntil(() => m_SUI.DescViewAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
			//m_SUI.DescViewAnim.gameObject.SetActive(false);

			PlayerPrefs.SetInt($"Alternative_Tuto_{USERINFO.m_UID}", 1);
			PlayerPrefs.Save();
		}
		else {
			m_SUI.SelectAnim.SetTrigger("Start");
		}
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.SelectAnim));
		m_SUI.SelectAnim.enabled = false;
		m_SUI.QuestionTxt[1].text = string.Format("{0}{1}", m_Table.GetQuestion(0), m_Table.GetQuestion(1));
		SetState(State.Select);
	}
	/// <summary> 질문 내려보기 올리기 </summary>
	IEnumerator DescViewOnOff(bool _on) {
		PlayEffSound(SND_IDX.SFX_0007);
		if (_on) {
			SetState(State.Desc);
			m_SUI.DescScroll.verticalNormalizedPosition = 1;
			m_SUI.DescViewAnim.gameObject.SetActive(true);
			m_SUI.SelectAnim.enabled = true;
			m_SUI.SelectAnim.SetTrigger("Out");
			m_SUI.DescViewAnim.SetTrigger("In");
			yield return new WaitForEndOfFrame();
			yield return new WaitUntil(() => m_SUI.DescViewAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
		}
		else {
			m_SUI.SelectAnim.SetTrigger("In");
			m_SUI.DescViewAnim.SetTrigger("Out");
			yield return new WaitForEndOfFrame();
			yield return new WaitUntil(() => m_SUI.DescViewAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
			m_SUI.DescViewAnim.gameObject.SetActive(false);
			yield return new WaitUntil(() => m_SUI.SelectAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
			m_SUI.SelectAnim.enabled = false;
			SetState(State.Select);
		}
	}
	public void ClickDescOnOff(bool _on) {
		if (m_State != State.Select && m_State != State.Desc) return;
		if (!_on && m_SUI.DescViewAnim.gameObject.activeInHierarchy == true && m_SUI.DescViewAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
			if (m_DescAnimCor != null)
				StopCoroutine(m_DescAnimCor);
			m_DescAnimCor = StartCoroutine(DescViewOnOff(false));
		}
		else if (_on && !m_SUI.DescViewAnim.gameObject.activeSelf) {
			if (m_DescAnimCor != null)
				StopCoroutine(m_DescAnimCor);
			m_DescAnimCor = StartCoroutine(DescViewOnOff(true));
		}
		m_DragDesc = false;
	}
	/// <summary> YesNo 선택시 호출, SeedQuestion에서 콜백함 </summary>
	IEnumerator SelectAnim(bool _yn) {
		m_SelectYN = _yn;
		m_SUI.ResultImg.sprite = m_Table.GetResultTFImg(m_SelectYN);
		m_SUI.ResultBG.sprite = m_SUI.SelectTFImg[m_SelectYN == true ? 0 : 1];

		yield return new WaitForEndOfFrame();
		if (m_SelectYN) {
			iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Card.position.x, "to", m_SUI.Card.position.x - 40f, "time", 1f, "onupdate", "CardPosX"));
			iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Card.position.y, "to", m_SUI.Card.position.y - 200f, "time", 1f, "onupdate", "CardPosY", "easetype", "easeInCubic"));
		}
		else {
			iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Card.position.x, "to", m_SUI.Card.position.x + 40f, "time", 1f, "onupdate", "CardPosX"));
			iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Card.position.y, "to", m_SUI.Card.position.y - 200f, "time", 1f, "onupdate", "CardPosY", "easetype", "easeInCubic"));
		}
		yield return new WaitForSeconds(1f);
		m_SUI.SelectAnim.enabled = true;
		//아래 애니메이터 켜지면 꺼질때 텍스쳐 알파값 0으로 고정되서 주석함
		//yield return new WaitForEndOfFrame();
		//m_SUI.SelectAnim.SetTrigger("End");
		//yield return new WaitForEndOfFrame();
		//yield return new WaitUntil(() => m_SUI.SelectAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
		m_SUI.SelectAnim.gameObject.SetActive(false);
		m_SUI.ResultAnim.gameObject.SetActive(true);
		yield return new WaitForEndOfFrame();
		m_SUI.ResultTxt.GetComponent<TippingText>().SetData(m_Table.GetResultTF(m_SelectYN));
		yield return new WaitUntil(() => m_SUI.ResultAnim.GetCurrentAnimatorStateInfo(0).IsName("4_Result_End") && m_SUI.ResultAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
		m_SUI.ResultAnim.gameObject.SetActive(false);
		ResultEnd();
	}
	void ResultEnd() {
#if NOT_USE_NET
		int rewardidx = m_Table.GetRewardValTF(m_SelectYN);
		switch (m_Table.GetRewardTypeTF(m_SelectYN)) {
			case RewardKind.None:
				break;
			case RewardKind.Character:
				if (!m_GetCharIdx.Contains(m_Table.GetRewardValTF(m_SelectYN))) m_GetCharIdx.Add(m_Table.GetRewardValTF(m_SelectYN));
				CharInfo info = USERINFO.m_Chars.Find(t => t.m_Idx == rewardidx);
				if (info != null) {
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(186));
					USERINFO.InsertItem(info.m_TData.m_PieceIdx, BaseValue.STAR_OVERLAP(info.m_TData.m_Grade));
				}
				else {
					info = USERINFO.InsertChar(rewardidx);
					for (int i = 0; i < BaseValue.GetDeckSlotCnt(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx); i++)
						if (USERINFO.m_Deck[0].m_Char[i] == 0)
							USERINFO.m_Deck[0].SetChar(i, info.m_UID);
				}
				break;
			case RewardKind.Item:
				TItemTable tdata = TDATA.GetItemTable(rewardidx);
				if (tdata.m_Type == ItemType.RandomBox || tdata.m_Type == ItemType.AllBox) {//박스는 바로 까서 주기
					TDATA.GetGachaItem(tdata);
				}
				else {
					ItemInfo iteminfo = USERINFO.InsertItem(rewardidx, 1);
					break;
				}
				break;
			case RewardKind.Zombie:
				ZombieInfo zombieInfo = USERINFO.InsertZombie(rewardidx);
				break;
			case RewardKind.DNA:
				TDnaTable dnaTable = TDATA.GetDnaTable(rewardidx);
				DNAInfo dnaInfo = new DNAInfo(dnaTable.m_Idx);
				USERINFO.m_DNAs.Add(dnaInfo);
				break;
		}
		MAIN.Save_UserInfo();
#else
		SelectInfo.Add(new LS_Web.REQ_ALTERNATIVE_INFO() { Idx = m_Table.m_Idx, Select = m_SelectYN });
#endif

		if (m_Idxs.Count > m_QstCnt) {
			m_QstCnt++;
			SetQuestion();
		}
		else {
			switch (m_Mode) {
				case AlternativeMode.Prologue:
					if (m_AnimCor != null)
						StopCoroutine(m_AnimCor);
					m_AnimCor = StartCoroutine(PrologueEnd());
					break;
				case AlternativeMode.StageEnd:
					StartCoroutine(AlternativeEnd());
					break;
			}
		}
	}
	IEnumerator PrologueEnd() {
		SetState(State.PrologueEnd);
		m_SUI.DescScroll.verticalNormalizedPosition = 1;
		m_SUI.QuestionTxt[1].text = string.Empty;
		yield return new WaitForEndOfFrame();
		m_SUI.DescViewAnim.gameObject.SetActive(true);
		yield return new WaitForEndOfFrame();
		m_SUI.DescViewAnim.SetTrigger("Prologue");
		m_SUI.QuestionTxt[1].GetComponent<TippingText>().SetData(TDATA.GetString(ToolData.StringTalbe.Etc, 302));
	}
	IEnumerator AlternativeEnd() {
		List<CharInfo> chars = new List<CharInfo>();
		bool reward = false;
#if NOT_USE_NET
#else
		reward = true;
		switch (m_Mode)
		{
		case AlternativeMode.StageEnd:
			SendStageAltReward((res) => {
				USERINFO.SetDATA(res.Stage);
				reward = false;
			});
			break;
		}
#endif

		SetState(State.AlternativeEnd);
		m_SUI.MainAnim.SetTrigger("End");

		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => m_SUI.MainAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
		yield return new WaitWhile(() => reward);

		Close(m_Mode == AlternativeMode.StageEnd ? 0 : 1);
	}

	void SendStageAltReward(Action<LS_Web.RES_STAGE_ALTREWARD> cb) {
		UserInfo.StageIdx stageidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
		UserInfo.Stage stage = USERINFO.m_Stage[StageContentType.Stage];
		WEB.SEND_REQ_STAGE_ALTREWARD((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					// 클리어 다시 시도
					//SendStageAltReward(cb);
					// 타이틀부터 다시 시작함 (플레이상태에서 시드 선택정보가 있으면 다시 시도함)
					MAIN.ReStart();
				});
				return;
			}
			cb?.Invoke(res);
		}, stage.UID, stageidx.Week, stageidx.Pos, SelectInfo[0]);
	}

	/// <summary> 조작 놨을때 카드 초기화 </summary>
	void CardInit() {
		iTween.Stop(gameObject);
		iTween.RotateTo(m_SUI.Card.gameObject, iTween.Hash("z", 0f, "time", 0.3f));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.SelectNo.color.a, "to", 0f, "time", 0.3f, "onupdate", "SelectNoColor"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.SelectYes.color.a, "to", 0f, "time", 0.3f, "onupdate", "SelectYesColor"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.CardBucket.localScale.x, "to", 1.1f, "time", 0.3f, "onupdate", "CardBucketScale"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.CardGrad.color.a, "to", 0f, "time", 0.3f, "onupdate", "SelectGrad"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.CardImg[0].color.a, "to", 1f, "time", 0.3f, "onupdate", "CardCImgAlpha"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.CardImg[1].color.a, "to", 0f, "time", 0.3f, "onupdate", "CardLImgAlpha"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.CardImg[2].color.a, "to", 0f, "time", 0.3f, "onupdate", "CardRImgAlpha"));
	}
	//선택지 조작 관련
	void CardPosX(float _amount) {
		m_SUI.Card.position = new Vector3(_amount, m_SUI.Card.position.y, m_SUI.Card.position.z);
	}
	void CardPosY(float _amount) {
		m_SUI.Card.position = new Vector3(m_SUI.Card.position.x, _amount, m_SUI.Card.position.z);
	}
	void CardRotZ(float _amount) {//트위너
		m_SUI.Card.localEulerAngles = new Vector3(0f, 0f, _amount);
	}
	void CardBucketScale(float _amount) {
		m_SUI.CardBucket.localScale = Vector3.one * _amount;
	}
	void CardCImgAlpha(float _amount) {
		m_SUI.CardImg[0].color = new Color(1, 1, 1, _amount);
	}
	void CardLImgAlpha(float _amount) {
		m_SUI.CardImg[1].color = new Color(1, 1, 1, _amount);
	}
	void CardRImgAlpha(float _amount) {
		m_SUI.CardImg[2].color = new Color(1, 1, 1, _amount);
	}
	void SelectGrad(float _amount) {
		m_SUI.CardGrad.color = new Color(0f, 0f, 0f, _amount);
	}
	void SelectNoColor(float _amount) {//트위너
		m_SUI.SelectNo.color = new Color(m_SUI.SelectYes.color.r, m_SUI.SelectYes.color.g, m_SUI.SelectYes.color.b, _amount);
	}
	void SelectYesColor(float _amount) {//트위너
		m_SUI.SelectYes.color = new Color(m_SUI.SelectYes.color.r, m_SUI.SelectYes.color.g, m_SUI.SelectYes.color.b, _amount);
	}
}
