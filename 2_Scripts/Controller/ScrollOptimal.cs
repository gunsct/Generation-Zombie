using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ScrollOptimal : MonoBehaviour
{
	[Serializable]
	public struct SUI {
		public ScrollRect Scroll;
		public Transform MovePanel;
		public Transform Element;
		public LayoutGroup LayoutGroup;
		public ContentSizeFitter SizeFitter;
	}
	[SerializeField] SUI m_SUI;
	GridLayoutGroup m_Grid = null;
	VerticalLayoutGroup m_Vertical = null;
	HorizontalLayoutGroup m_Horizontal = null;
	Action<int> m_CB = null;
	Vector2 m_ContentSize = Vector2.zero;
	Vector2 m_Interval = Vector2.zero;
	int m_CheckCnt = 0;                         //스탭 체크 카운트
	int[] m_LineRow = new int[2];
	int[] m_Step = new int[2];                  //행이나 열 단계수, 0:현재, 1:최대
	int m_ListPos = 0;
	bool Is_Rf = false;
	bool Is_Init = false;

	public Transform GetViewPort { get { return m_SUI.Scroll.viewport; } }
	public Transform GetContent { get { return m_SUI.Scroll.content; } }
	public int GetCheckCnt { get { return m_CheckCnt; } }
	public int GetNowStep { get { return m_Step[0]; } }
	Vector3 GetMovePos { get { return m_SUI.MovePanel.localPosition; } }
	Vector3 GetContentPos { get { return m_SUI.Scroll.content.localPosition; } }
	public KeyValuePair<int, GameObject> GetActivePos {
		get {
			for (int i = 0; i < m_SUI.Scroll.content.childCount; i++) {
				GameObject obj = m_SUI.Scroll.content.GetChild(i).gameObject;
				if (!obj.activeSelf) return new KeyValuePair<int, GameObject>(i, obj);
			}
			return new KeyValuePair<int, GameObject>(0, null);
		}
	}

	private void Awake() {
		if (!Is_Init) Init();
	}
	public void Init() {
		if (Is_Init) return;
		Is_Init = true;
		if (m_SUI.Scroll == null) m_SUI.Scroll = GetComponent<ScrollRect>();
		if (m_SUI.LayoutGroup == null) m_SUI.LayoutGroup = m_SUI.Scroll.content.GetComponent<LayoutGroup>();
		if (m_SUI.SizeFitter == null) m_SUI.SizeFitter = m_SUI.Scroll.content.GetComponent<ContentSizeFitter>();
		if (m_SUI.LayoutGroup.GetType() == typeof(GridLayoutGroup)) {
			m_Grid = (GridLayoutGroup)m_SUI.LayoutGroup;
			m_Interval = m_Grid.cellSize + m_Grid.spacing;
		}
		else if (m_SUI.LayoutGroup.GetType() == typeof(VerticalLayoutGroup)) {
			m_Vertical = (VerticalLayoutGroup)m_SUI.LayoutGroup;
			m_Interval.y = ((RectTransform)m_SUI.Element.transform).sizeDelta.y + m_Vertical.spacing;
		}
		else if (m_SUI.LayoutGroup.GetType() == typeof(HorizontalLayoutGroup)) {
			m_Horizontal = (HorizontalLayoutGroup)m_SUI.LayoutGroup;
			m_Interval.x = ((RectTransform)m_SUI.Element.transform).sizeDelta.x + m_Horizontal.spacing;
		}

		RectOffset padding = m_SUI.LayoutGroup.padding;
		Vector2 contentsize = m_SUI.Scroll.viewport.rect.size;
		m_ContentSize = new Vector2(contentsize.x - padding.left - padding.right, contentsize.y - padding.top - padding.bottom);

		//스크롤 한페이지 + 2열, 행 만큼 생성
		m_LineRow[0] = m_Interval.y <= 0 ? 1 : Math.Max(1, Mathf.FloorToInt(m_ContentSize.y / m_Interval.y) + (m_SUI.Scroll.vertical ? 2 : 0));
		m_LineRow[1] = m_Interval.x <= 0 ? 1 : Math.Max(1, Mathf.FloorToInt(m_ContentSize.x / m_Interval.x) + (m_SUI.Scroll.horizontal ? 2 : 0));
		int cnt = m_LineRow[0] * m_LineRow[1];
		if (m_SUI.Scroll.content.childCount > 0) {
			for (int i = m_SUI.Scroll.content.childCount - 1; i >= 0; i--) {
				DestroyImmediate(m_SUI.Scroll.content.GetChild(i).gameObject);
			}
		}
		for (int i = 0; i < cnt; i++) {
			Instantiate(m_SUI.Element, m_SUI.Scroll.content);
		}
		m_SUI.SizeFitter.enabled = true;
	}

	/// <summary> scrollrect changeval 호출, content ypos로 이동량 체크 </summary>
	public void ChangeVal() {
		if (m_SUI.Scroll.content.childCount < 1) return;

		Vector3 contentpos = GetContentPos;
		int nowstep = 0;
		bool is_left = Math.Abs(contentpos.x) < m_Interval.x * m_Step[0];
		bool is_down = contentpos.y < m_Interval.y * m_Step[0];
		if (m_SUI.Scroll.horizontal && !Is_Rf && m_Interval.x > 0 && (Math.Abs(contentpos.x) >= m_Interval.x * (m_Step[0] + 1) || is_left)) {
			nowstep = Math.Clamp(m_Step[0] + (is_left ? -1 : 1), 0, Math.Max(0,m_Step[1] - Mathf.CeilToInt(m_ContentSize.x / m_Interval.x)));
			Is_Rf = m_Step[0] != nowstep;
			if (Is_Rf) {
				m_Step[0] = nowstep;
				m_ListPos = m_Step[0] * m_CheckCnt;
				m_CB?.Invoke(m_ListPos);
				m_SUI.MovePanel.localPosition += new Vector3(is_left ? -m_Interval.x : m_Interval.x, 0f, 0f);
				Is_Rf = false;
			}
		}
		else if (m_SUI.Scroll.vertical && !Is_Rf && m_Interval.y > 0 && (contentpos.y >= m_Interval.y * (m_Step[0] + 1) || is_down)) {
			nowstep = Math.Clamp(m_Step[0] + (is_down ? -1 : 1), 0, Math.Max(0,m_Step[1] - Mathf.CeilToInt(m_ContentSize.y / m_Interval.y)));
			Is_Rf = m_Step[0] != nowstep;
			if (Is_Rf) {
				m_Step[0] = nowstep;
				m_ListPos = m_Step[0] * m_CheckCnt;
				m_CB?.Invoke(m_ListPos);
				m_SUI.MovePanel.localPosition += new Vector3(0f, is_down ? m_Interval.y : -m_Interval.y, 0f);
				Is_Rf = false;
			}
		}
	}

	public void SetData(bool _init, int _cnt, int _spos = 0, Action<int> _cb = null) {
		m_CB = _cb;

		//전체 행,열 수 계산
		if (m_Grid != null) {
			m_CheckCnt = Mathf.FloorToInt(m_ContentSize.x / m_Interval.x);
		}
		else if (m_Vertical != null) {
			m_CheckCnt = 1;
		}
		else if (m_Horizontal != null) {
			m_CheckCnt = 1;
		}

		int premaxstep = m_Step[1];
		m_Step[1] = m_CheckCnt > 0 ? Mathf.CeilToInt(_cnt / m_CheckCnt) : 0;
		if (!_init && premaxstep > 0 && premaxstep != m_Step[1]) {
			m_Step[0] -= premaxstep - m_Step[1];
			m_Step[0] = Math.Max(m_Step[0], 0); 
			SetPosScroll(m_Step[0] * m_CheckCnt);
		}

		if (_spos >= 0) SetPosScroll(_spos);
	}
	public void InitScroll() {
		m_Step[0] = 0;
		m_SUI.MovePanel.localPosition = Vector3.zero;
		m_SUI.Scroll.content.localPosition = Vector2.zero;
	}
	/// <summary> 리스트의 특정 엘리먼트 위치를 받으면 직접 계산해서 그 위치로 스크롤링 </summary>
	public void SetPosScroll(int _pos, bool _scrolling = false, float _time = 0.5f) {
		if (!_scrolling) { 
			m_Step[0] = Mathf.FloorToInt(_pos / m_CheckCnt);
			m_ListPos = m_Step[0] * m_CheckCnt;
			m_CB?.Invoke(m_ListPos);
		}
		else m_CB?.Invoke(0);

		if (m_SUI.Scroll.horizontal) {
			if (_scrolling) {
				Scrolling(false, m_Interval.x * m_Step[0], _time);
			}
			else {
				m_SUI.MovePanel.localPosition = new Vector3(m_Interval.x * m_Step[0], GetMovePos.y, GetMovePos.z);
				m_SUI.Scroll.content.localPosition = new Vector3(-m_Interval.x * m_Step[0], GetContentPos.y, GetContentPos.z);
			}
		}
		else if (m_SUI.Scroll.vertical) {
			if (_scrolling) {
				Scrolling(true, -m_Interval.y * m_Step[0], _time);
			}
			else {
				m_SUI.MovePanel.localPosition = new Vector3(GetMovePos.x, -m_Interval.y * m_Step[0], GetMovePos.z);
				m_SUI.Scroll.content.localPosition = new Vector3(GetContentPos.x, m_Interval.y * m_Step[0], GetContentPos.z);
			}
		}
	}
	public void Scrolling(bool _vertical, float _val, float _time = 0.5f) {
		iTween.StopByName(gameObject, "MovePanel");
		iTween.StopByName(gameObject, "ScrollContent");
		iTween.ValueTo(gameObject, iTween.Hash("from", _vertical ? GetMovePos.y : GetMovePos.x, "to", _val, "onupdate", _vertical ? "TW_MovePanelY" : "TW_MovePanelX", "time", _time, "name", "MovePanel"));
		iTween.ValueTo(gameObject, iTween.Hash("from", _vertical ? GetContentPos.y : GetContentPos.x, "to", -_val, "onupdate", _vertical ? "TW_ScrollContentY" : "TW_ScrollContentX", "time", _time, "name", "ScrollContent"));
	}
	void TW_MovePanelX(float _amount) {
		m_SUI.MovePanel.localPosition = new Vector3(_amount, GetMovePos.y, GetMovePos.z);
		ChangeVal();
	}
	void TW_MovePanelY(float _amount) {
		m_SUI.MovePanel.localPosition = new Vector3(GetMovePos.x, _amount, GetMovePos.z);
		ChangeVal();
	}
	void TW_ScrollContentX(float _amount) {
		m_SUI.Scroll.content.localPosition = new Vector3(_amount, GetContentPos.y, GetContentPos.z);
		ChangeVal();
	}
	void TW_ScrollContentY(float _amount) {
		m_SUI.Scroll.content.localPosition = new Vector3(GetContentPos.x, _amount, GetContentPos.z);
		ChangeVal();
	}
}
