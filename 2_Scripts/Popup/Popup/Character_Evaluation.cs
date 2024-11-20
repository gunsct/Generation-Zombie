using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using System.Linq;
using UnityEngine.UI;

public class Character_Evaluation : PopupBase
{
	public enum State
	{
		View,
		EvaCategory,
		EvaPoint,
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image Portrait;
		public TextMeshProUGUI CharName;
		public GameObject[] Panels;
		public TextMeshProUGUI EvaPoint;    //평점
		public GameObject TagPrefab;        //Item_Eva_Tag
		public Transform TagBucket;
		public Slider PointSlider;
	}
	[SerializeField] SUI m_SUI;
	List<Item_Eva_Tag> m_Tags = new List<Item_Eva_Tag>();

	public int m_CharIdx;							//현재 캐릭터 인덱스
	CharInfo m_Info;								//현재 캐릭터
	List<CharInfo> m_CharList;						//볼 수 있는 캐릭터 목록
	RES_CHAR_GET_EVA m_CharEva;						//캐릭터 평가 데이터
	int m_Point;									//평점, 1~10 1당 0.5점
	List<EvaData> m_EvaDatas = new List<EvaData>(); //평가 데이터들

	State m_State;
	CharHRType m_CrntCategory;
	bool m_GetChar;									//가진 캐릭터인지 여부
	bool m_BeingEva;								//기존 평가 여부
	IEnumerator m_Action;

	TCharacterTable m_TCData { get { return TDATA.GetCharacterTable(m_CharIdx); } }

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_CharEva = (RES_CHAR_GET_EVA)aobjValue[0];
		m_GetChar = aobjValue.Length > 2;
		if (m_GetChar) {
			m_Info = (CharInfo)aobjValue[1];
			m_CharIdx = m_Info.m_Idx;
			m_CharList = (List<CharInfo>)aobjValue[2];
		}
		else {
			m_CharIdx = (int)aobjValue[1];
		}
		m_SUI.Portrait.sprite = m_TCData.GetPortrait();
		m_SUI.CharName.text = string.Format("[{0}] 인사 평가", m_TCData.GetCharName());

		m_BeingEva = m_CharEva.MyData != null;

		SetState((int)State.View);

		base.SetData(pos, popup, cb, aobjValue);
	}
	/// <summary> 상태 변환 </summary>
	void SetState(int _state) {
		m_State = (State)_state;
		for(int i = 0; i < m_SUI.Panels.Length; i++) {
			m_SUI.Panels[i].SetActive(i == _state);
		}
		switch (m_State) {
			case State.View:
				SetView();
				break;
			case State.EvaCategory:
				SetCategory(CharHRType.Utility);
				break;
			case State.EvaPoint:
				SetPoint();
				break;
		}
	}
	/// <summary> 캐릭터의 평가 데이터들 시각화 </summary>
	void SetView() {
		float point = m_CharEva.Data.Point;
		m_SUI.EvaPoint.text = string.Format("0:0.0", point);

		List<EvaCnt> infos = m_CharEva.Data.Info;
		int tagtotal = infos.Sum(o => o.Cnt);
		//TODO:태그 총 수량에 비례한 뭔가 시각적인 효과 보이기

	}
	/// <summary> 태그들 세팅 </summary>
	void SetCategory(CharHRType _type) {
		m_CrntCategory = _type;
		for (int i = m_Tags.Count - 1; i > 0; i--) {
			Destroy(m_Tags[i].gameObject);
		}
		m_Tags.Clear();

		List<TChar_HRTable> datas = TDATA.GetChar_HRGroupTable(_type);
		for(int i = 0; i < datas.Count; i++) {
			Item_Eva_Tag tag = Utile_Class.Instantiate(m_SUI.TagPrefab, m_SUI.TagBucket).GetComponent<Item_Eva_Tag>();
			tag.SetData(datas[i].m_Idx, CB_SetEvaData);
			m_Tags.Add(tag);
		}
	}
	/// <summary> 카테고리별 태그들 데이터 콜백</summary>
	public void CB_SetEvaData(int _idx) {
		if(m_EvaDatas.Find(o=>o.Pos == _idx) == null) {
			m_EvaDatas.Add(new EvaData() { Pos = _idx, Value = 1 });
			m_CrntCategory++;
			if (m_CrntCategory < CharHRType.Max) {
				SetCategory(m_CrntCategory);
			}
			else SetState((int)State.EvaPoint);
		}
	}
	/// <summary> 점수 평가 </summary>
	void SetPoint() {
		m_SUI.PointSlider.normalizedValue = 0f;
	}
	/// <summary> 슬라이드 값 변화시 호출 </summary>
	public void ChangePoint(int _amount) {
		m_Point = _amount;
	}
	/// <summary> 평가 끝내고 서버로 데이터 전송 </summary>
	public void Click_EvaluationEnd() {
		WEB.SEND_REQ_CHAR_SET_EVA((res) => {
			if (!res.IsSuccess()) return;
			Close(0);
		}, m_CharIdx, m_Point, m_EvaDatas);
	}
	/// <summary> 평가중 취소하면 진행중인 데이터 날리고 초기화면으로 돌림 </summary>
	public void CancleEva() {
		m_Point = 0;
		m_EvaDatas.Clear();
		SetState((int)State.View);
	}
	/// <summary> 캐릭터 변경 </summary>
	public void Click_ChangeChar(bool _right) {
		if (m_Action != null) return;
		int pos = 0;

		for (int i = 0; i < m_CharList.Count; i++) {
			if (m_CharList[i] == m_Info) {
				pos = i;
				break;
			}
		}

		if (_right) pos = pos < m_CharList.Count - 1 ? pos + 1 : 0;
		else pos = pos > 0 ? pos - 1 : m_CharList.Count - 1;

		m_Info = m_CharList[pos];

		WEB.SEND_REQ_CHAR_GET_EVA((res) => {
			if (!res.IsSuccess()) {
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					WEB.SEND_REQ_CHARINFO((res2) => {
						SetUI();
					}, USERINFO.m_UID, m_Info.m_UID);
				});
				return;
			}

			SetData(PopupPos.POPUPUI, PopupName.Character_Evaluation, m_EndCB, new object[] { m_Info, m_CharList });
		}, m_CharIdx);
	}

	public override void Close(int Result = 0) {
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
}
