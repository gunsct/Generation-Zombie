using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ScrollOptimalAuto : MonoBehaviour
{
	[Serializable]
	public struct SUI {
		public ScrollRect Scroll;
		public Transform Element;
		public LayoutGroup LayoutGroup;
	}
	[SerializeField] SUI m_SUI;
	GridLayoutGroup m_Grid = null;
	VerticalLayoutGroup m_Vertical = null;
	HorizontalLayoutGroup m_Horizontal = null;
	Action m_CB = null;
	Vector2 m_ContentSize = Vector2.zero;
	Vector2 m_Interval = Vector2.zero;
	int m_CheckCnt = 0;                         //스탭 체크 카운트
	int[] m_LineRow = new int[2];
	int[] m_Step = new int[2];                  //행이나 열 단계수, 0:현재, 1:최대
	bool Is_Init = false;
	List<Vector3> m_EementsInitPos = new List<Vector3>();

	RectTransform GetViewPort() { return (RectTransform)m_SUI.Scroll.viewport; } 
	public Transform GetContent { get { return m_SUI.Scroll.content; } }
	public int GetCheckCnt { get { return m_CheckCnt; } }
	public int GetNowStep { get { return m_Step[0]; } }
	Vector3 GetContentPos { get { return GetContent.localPosition; } }
	List<Transform> GetFirstLine() {
		List<Transform> elements = new List<Transform>();
		for(int i = 0; i < m_CheckCnt; i++) {
			elements.Add(GetContent.GetChild(i));
		}

		return elements;
	}
	RectTransform GetFirst() {
		return (RectTransform)GetFirstLine()[0];
	}
	List<Transform> GetLastLine() {
		List<Transform> elements = new List<Transform>();
		for (int i = GetContent.childCount - 1; i > GetContent.childCount - 1 - m_CheckCnt; i--) {
			elements.Add(GetContent.GetChild(i));
		}

		return elements;
	}
	RectTransform GetLast() {
		return (RectTransform)GetLastLine()[0];
	}
	public void Init() {
		if (Is_Init) return;
		if (m_SUI.Scroll == null) m_SUI.Scroll = GetComponent<ScrollRect>();
		if (m_SUI.LayoutGroup == null) m_SUI.LayoutGroup = GetContent.GetComponent<LayoutGroup>();
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
		StartCoroutine(IE_Generate(cnt));
	}
	IEnumerator IE_Generate(int cnt) {
		if (GetContent.childCount > 0) {
			for (int i = GetContent.childCount - 1; i >= 0; i--) {
				DestroyImmediate(GetContent.GetChild(i).gameObject);
			}
		}
		for (int i = 0; i < cnt; i++) {
			Instantiate(m_SUI.Element, GetContent);
		}

		yield return new WaitForEndOfFrame();
		//처음 세팅만 하고 끄기
		for(int i = 0; i < cnt; i++) {
			m_EementsInitPos.Add(GetContent.GetChild(i).localPosition);
		}
		m_SUI.LayoutGroup.enabled = false;
		InitScroll();
		Is_Init = true;
	}
	/// <summary> scrollrect changeval 호출, content ypos로 이동량 체크 </summary>
	public void ChangeVal() {
		if (!Is_Init) return;
		if (GetContent.childCount < 1) return;

		List<Transform> first = GetFirstLine();
		List<Transform> last = GetLastLine();
		//첫줄 끝줄 계산
		if (m_SUI.Scroll.horizontal && m_Interval.x > 0) {
			if (GetFirst().position.x + GetFirst().rect.xMax < GetViewPort().position.x + GetViewPort().rect.xMin && m_Step[0] < m_Step[1]) {//왼쪽으로 끌면
				for (int i = first.Count - 1; i >= 0; i--) {
					first[i].SetAsLastSibling();
				}
				for (int i = 0; i < first.Count; i++) {
					first[i].localPosition = last[i].localPosition + new Vector3(m_Interval.x, 0f, 0f);
				}
				m_Step[0] = Math.Clamp(m_Step[0] + 1, 0, m_Step[1]);
				m_CB?.Invoke();
			}
			else if (GetLast().position.x + GetLast().rect.xMin > GetViewPort().position.x + GetViewPort().rect.xMax && m_Step[0] > 0) {//오른쪽으로 끌면
				for (int i = last.Count - 1; i >= 0; i--) {
					last[i].SetAsFirstSibling();
				}
				for (int i = 0; i < last.Count; i++) {
					last[i].localPosition = first[i].localPosition - new Vector3(m_Interval.x, 0f, 0f);
				}
				m_Step[0] = Math.Clamp(m_Step[0] - 1, 0, m_Step[1]);
				m_CB?.Invoke();
			}
		}
		else if (m_SUI.Scroll.vertical && m_Interval.y > 0) {
			if (GetFirst().position.y + GetFirst().rect.yMin > GetViewPort().position.y + GetViewPort().rect.yMax && m_Step[0] < m_Step[1]) {//끌어 올리면
				for (int i = first.Count - 1; i >= 0; i--) {
					first[i].SetAsLastSibling();
				}
				for (int i = 0;i< first.Count; i++) {
					first[i].localPosition = last[i].localPosition - new Vector3(0f, m_Interval.y, 0f);
				}
				m_Step[0] = Math.Clamp(m_Step[0] + 1, 0, m_Step[1]);
				m_CB?.Invoke();
			}
			else if(GetLast().position.y + GetLast().rect.yMax < GetViewPort().position.y + GetViewPort().rect.yMin && m_Step[0] > 0) {//끌어 내리면
				for (int i = last.Count - 1; i >= 0; i--) {
					last[i].SetAsFirstSibling();
				}
				for (int i = 0; i < last.Count; i++) {
					last[i].localPosition = first[i].localPosition + new Vector3(0f, m_Interval.y, 0f);
				}
				m_Step[0] = Math.Clamp(m_Step[0] - 1, 0, m_Step[1]);
				m_CB?.Invoke();
			}
		}
	}

	public void SetData(int _cnt, Action _cb = null) {
		m_CB = _cb;
		RectOffset padding = m_SUI.LayoutGroup.padding;
		RectTransform trans = m_SUI.Scroll.content;
		Vector2 space = Vector2.zero;
		//전체 행,열 수 계산
		if (m_Grid != null) {
			m_CheckCnt = Mathf.FloorToInt(m_ContentSize.x / m_Interval.x);
			space = m_Grid.spacing;
		}
		else if (m_Vertical != null) {
			m_CheckCnt = 1;
			space.x = m_Vertical.spacing;
		}
		else if (m_Horizontal != null) {
			m_CheckCnt = 1;
			space.y = m_Horizontal.spacing;
		}
		m_Step[1] = Mathf.CeilToInt(_cnt / m_CheckCnt);

		//컨텐츠 사이즈 계산 패딩, 스페이스, 엘리먼트
		if (m_SUI.Scroll.vertical) {
			trans.sizeDelta = new Vector2(trans.sizeDelta.x, padding.top + padding.bottom + m_Interval.y * _cnt / m_CheckCnt - space.y);
		}
		if (m_SUI.Scroll.horizontal) {
			trans.sizeDelta = new Vector2(padding.left + padding.right + m_Interval.x * _cnt / m_CheckCnt - space.x, trans.sizeDelta.y);
		}
	}
	public void InitScroll() {
		m_Step[0] = 0;
		m_SUI.Scroll.normalizedPosition = new Vector2(0f, 1f);
		if (GetContent.childCount == m_EementsInitPos.Count) {
			for (int i = 0; i < m_EementsInitPos.Count; i++) {
				GetContent.GetChild(i).localPosition = m_EementsInitPos[i];
			}
		}
	}
	/// <summary> 리스트의 특정 엘리먼트 위치를 받으면 직접 계산해서 그 위치로 스크롤링 </summary>
	public void SetPosScroll(int _pos, bool _scrolling = false, float _time = 0.5f) {
		if (!_scrolling) { 
			m_Step[0] = Mathf.FloorToInt(_pos / m_CheckCnt);
			m_CB?.Invoke();
		}
		else m_CB?.Invoke();

		if (m_SUI.Scroll.horizontal) {
			Scrolling(false, m_Interval.x * m_Step[0], _time);
		}
		else if (m_SUI.Scroll.vertical) {
			Scrolling(true, -m_Interval.y * m_Step[0], _time);
		}
	}
	public void Scrolling(bool _vertical, float _val, float _time = 0.5f) {
		iTween.StopByName(gameObject, "ScrollContent");
		iTween.ValueTo(gameObject, iTween.Hash("from", _vertical ? GetContentPos.y : GetContentPos.x, "to", -_val, "onupdate", _vertical ? "TW_ScrollContentY" : "TW_ScrollContentX", "time", _time, "name", "ScrollContent"));
	}
}
