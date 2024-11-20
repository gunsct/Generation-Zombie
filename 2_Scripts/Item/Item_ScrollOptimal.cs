using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_ScrollOptimal : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Transform Content;
		public GameObject Block;
	}
	[SerializeField]
	SUI m_SUI;

	/// <summary> 보여줄 아이템의 총 수량 </summary>
	[SerializeField] int m_ListCnt;
	/// <summary> 현재 행 </summary>
	[SerializeField] int m_Step = 0;
	/// <summary> 전체 행 </summary>
	[SerializeField] int m_TotalStep = 0;
	/// <summary> 행간격 </summary>
	[SerializeField] float m_StepInterval = 300f;
	/// <summary> 보여주는 열 수 </summary>
	[SerializeField] int m_LineCnt = 4;
	/// <summary> 보여주는 행 수 </summary>
	[SerializeField] int m_RowCnt = 6;
	Vector3 m_PrePos = Vector3.zero;
	[SerializeField] float m_MoveY = 0f;
	/// <summary> 스크롤중 아이템 터치 불가 </summary>
	[SerializeField] float m_BlockVal = 5f;
	[SerializeField] int m_ViewLastLine = 3;
	/// <summary> 스크롤 가능 여부 체크 </summary>
	Func<bool> m_Check;
	/// <summary> 스크롤 행 갱신될 때 호출 </summary>
	Action m_RFCB;
	public float m_SpacingY { get { return m_SUI.Content.GetComponent<GridLayoutGroup>().spacing.y; } }

	public void SetData(int _cnt,int _line, int _row, float _interval, Func<bool> _check = null, Action _rfcb = null) {
		ScrollInit();
		m_ListCnt = _cnt;
		m_LineCnt = _line;
		m_RowCnt = _row;
		m_StepInterval = _interval;
		m_Check = _check;
		m_RFCB = _rfcb;
	}
	public void Refresh(int _cnt, bool _movePosition = true) {
		//수량이 달라질때만 맨위로 초기화
		if(m_ListCnt != _cnt && _movePosition) m_SUI.Content.localPosition -= new Vector3(0f, m_SUI.Content.localPosition.y, 0f);
		m_ListCnt = _cnt;
	}

	private void LateUpdate() {
		if (this == null) return;
		if (m_Check != null && !m_Check.Invoke()) return;
		if (m_ListCnt == 0) return;
		if (((RectTransform)m_SUI.Content).rect.height < ((RectTransform)m_SUI.Content.parent).rect.height) {
			m_SUI.Block.SetActive(false);
			m_MoveY = 0f;
			return;
		}
		if (m_LineCnt * (m_RowCnt - m_ViewLastLine) > m_ListCnt) {
			ScrollInit();
			m_RFCB.Invoke();
			m_SUI.Block.SetActive(false);
			return;
		}

		if (Input.GetMouseButtonDown(0)) {
			m_MoveY = 0f;
			m_PrePos = Input.mousePosition;
		}
		else if (Input.GetMouseButton(0)) {
			m_MoveY = Input.mousePosition.y - m_PrePos.y;
			m_PrePos = Input.mousePosition;
		}
		else {
			m_MoveY = Mathf.Lerp(m_MoveY, 0f, 0.05f);
		}
		m_TotalStep = m_ListCnt / m_LineCnt;
		m_TotalStep += m_ListCnt % m_LineCnt > 0 ? 1 : 0;

		if (m_SUI.Content.localPosition.y > m_StepInterval && Mathf.Max(0, m_TotalStep - m_RowCnt) > m_Step - Mathf.Max(0, m_ViewLastLine - 2)) {
			m_SUI.Content.localPosition -= new Vector3(0f, m_StepInterval, 0f);
			m_Step++;
			m_RFCB.Invoke();
		}
		else if (m_SUI.Content.localPosition.y <= 0f && m_Step > 0) {
			m_SUI.Content.localPosition += new Vector3(0f, m_StepInterval, 0f);
			m_Step--;
			m_RFCB.Invoke();
		}

		int childidx = 0;
		for(int i = m_SUI.Content.childCount - 1; i > -1 ; i--) {
			if (m_SUI.Content.GetChild(i).gameObject.activeSelf) {
				childidx = i;
				break;
			}
		}
		Rect rect = transform.GetComponent<RectTransform>().rect;
		GridLayoutGroup grid = m_SUI.Content.GetComponent<GridLayoutGroup>();
		float spacingy = grid.spacing.y;// + grid.cellSize.y;
		float padding = Mathf.Max(spacingy, rect.position.y + rect.yMax - m_SUI.Content.GetChild(childidx).position.y);// + m_StepInterval;
		float y = Mathf.Clamp(m_SUI.Content.localPosition.y + m_MoveY, 0f, (m_Step < m_TotalStep - m_RowCnt ? m_RowCnt : m_ViewLastLine + 1) * m_StepInterval + padding);//2=>m_ViewLastLine
		m_SUI.Content.localPosition = new Vector3(m_SUI.Content.localPosition.x, y, m_SUI.Content.localPosition.z);// 
		m_SUI.Block.SetActive(Mathf.Abs(m_MoveY) > m_BlockVal);
	}
	public void ScrollInit() {
		m_Step = 0;
		m_SUI.Content.localPosition -= new Vector3(0f, m_SUI.Content.localPosition.y, 0f);
	}
	public void SetClearElement() {
		for(int i = m_SUI.Content.childCount - 1; i > -1; i--) {
			Destroy(m_SUI.Content.GetChild(i).gameObject);
		}
	}
	public Transform GetContent() {
		return m_SUI.Content;
	}
	public int GetNowStep() {
		return m_Step;
	}
}
