using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PDA_Research_Tree_Line : ObjMng
{
	public enum LineName
	{
		/// <summary> 왼쪽 선 (상위가 있을때) </summary>
		Left = 0,
		/// <summary> 가운데 연결 선 </summary>
		Center,
		/// <summary> 오른쪽 선 (상위가 있을때) </summary>
		Right,
		/// <summary> 가운데 (중앙에서 자신으로 연결) </summary>
		Center_Down,
		/// <summary> 가운데 (중앙에서 상위로 연결) </summary>
		Center_UP,
		/// <summary> 가운데 (중앙에서 상위가 없을때 더 상위로 연결) </summary>
		Center_UpItemNone,
		/// <summary> 왼쪽 선 (상위가 없을때) </summary>
		Left_In,
		/// <summary> 오른쪽 선 (상위가 없을때) </summary>
		Right_In,
		End
	}
	public enum LinePos
	{
		/// <summary> 왼쪽 </summary>
		Left = 0,
		/// <summary> 가운데 </summary>
		Center,
		/// <summary> 오른쪽 </summary>
		Right,
		End
	}

#pragma warning disable 0649
	[System.Serializable]
	struct SLine
	{
		// Line Image
		[ReName("Left", "Center", "Right", "Center_Down", "Center_UP", "Center_UP_Line", "Left_In", "Right_In")]
		public Image[] Line;
		[HideInInspector]
		public bool[] Active;
	}

	[System.Serializable]
	struct SUI
	{
		public GameObject LinePanel;
		[ReName("LEFT", "CENTER", "RIGHT")]
		public SLine[] Line;
		[ReName("LOCK", "UNLOCK")]
		public Color[] LineColor;
		[ReName("Left", "Center", "Right")]
		public Item_PDA_Research_Element[] Item;
	}
	[SerializeField] SUI m_SUI;
	Func<int, Item_PDA_Research_Tree_Line> m_FnGetLine;
#pragma warning restore 0649

	public void Init(Func<int, Item_PDA_Research_Tree_Line> GetLine)
	{
		m_FnGetLine = GetLine;
		// 모든 오브젝트 꺼준다.
		for (int i = m_SUI.LinePanel.transform.childCount - 1; i > -1; i--) m_SUI.LinePanel.transform.GetChild(i).gameObject.SetActive(false);
		//m_SUI.LinePanel.SetActive(false);
		for(int j = 0; j < m_SUI.Line.Length; j++)
		{
			if (m_SUI.Line[j].Active == null || m_SUI.Line[j].Active.Length < m_SUI.Line[j].Line.Length) m_SUI.Line[j].Active = new bool[m_SUI.Line[j].Line.Length];
			for (int i = 0; i < m_SUI.Line[j].Line.Length; i++)
			{
				m_SUI.Line[j].Line[i].gameObject.SetActive(false);
				m_SUI.Line[j].Active[i] = false;
			}
		}
		for (int i = 0; i < 3; i++) m_SUI.Item[i].gameObject.SetActive(false);
	}

	public void SetData(ResearchInfo Info, Action<Item_PDA_Research_Element, int> ClickCB)
	{
		int pos = Info.m_TData.m_Pos.m_Pos;
		m_SUI.Item[pos].gameObject.SetActive(true);
		m_SUI.Item[pos].SetData(Info, ClickCB);
		LineCheck(Info);
	}

	void LineCheck(ResearchInfo myinfo)
	{
		TResearchTable mydata = TDATA.GetResearchTable(myinfo.m_Type, myinfo.m_Idx, 0);
		TResearchTable.TreePos mypos = mydata.m_Pos;
		if (mypos.m_Line == 0) return;

		for (int i = 0; i < 3; i++)
		{
			// 0레벨 기준으로 셋팅
			TResearchTable.Preced preced = mydata.m_Preced[i];
			if (preced.m_Idx == 0) continue;
			TResearchTable prdata = TDATA.GetResearchTable(myinfo.m_Type, preced.m_Idx, 0);
			TResearchTable.TreePos prpos = prdata.m_Pos;
			if (mypos.m_Line == prpos.m_Line) continue;
			// 라인전체 비활성화상태라면 켜주기
			ResearchInfo info = USERINFO.GetResearchInfo(myinfo.m_Type, preced.m_Idx);
			SetLine(mypos, prpos, preced.m_LV <= info.m_GetLv, mypos, true);
		}
	}

	public void SetLine(TResearchTable.TreePos mypos, TResearchTable.TreePos trpos, bool IsActive, TResearchTable.TreePos beforpos, bool IsFirst = false)
	{
		if (mypos.m_Line == trpos.m_Line && mypos.m_Pos == trpos.m_Pos) return;
		//if (!m_SUI.LinePanel.activeSelf) m_SUI.LinePanel.SetActive(true);
		for (int i = m_SUI.LinePanel.transform.childCount - 1; i > -1; i--) m_SUI.LinePanel.transform.GetChild(i).gameObject.SetActive(true);

		// 라인이 맞춰질때까지 상위로 올려준다.
		LinePos pos = (LinePos)mypos.m_Pos;
		bool IsUpLine = Math.Abs(mypos.m_Line - trpos.m_Line) > 1;
		// 자신으로 들어오는건 무조건 활성화
		if (IsFirst) ActiveLine(pos, LineName.Center_Down, IsActive);
		TResearchTable.TreePos temppos = new TResearchTable.TreePos();

		if (beforpos.m_Line != mypos.m_Line) ActiveLine(pos, LineName.Center_Down, IsActive);
		if (mypos.m_Pos < trpos.m_Pos)
		{
			if(IsUpLine && (LinePos)trpos.m_Pos == LinePos.Center)
			{
				SetCenterLine((LinePos)mypos.m_Pos, IsActive, true);

				temppos.m_Line = mypos.m_Line - 1;
				temppos.m_Pos = mypos.m_Pos;
				Item_PDA_Research_Tree_Line nextline = m_FnGetLine(temppos.m_Line);
				nextline?.SetLine(temppos, trpos, IsActive, mypos);
			}
			else
			{
				if (!IsFirst && beforpos.m_Line == mypos.m_Line) SetLeftLine((LinePos)mypos.m_Pos, IsActive);
				SetRightLine((LinePos)mypos.m_Pos, IsActive);
				temppos.m_Line = mypos.m_Line;
				temppos.m_Pos = mypos.m_Pos + 1;
				SetLine(temppos, trpos, IsActive, mypos);
			}
		}
		else if (mypos.m_Pos > trpos.m_Pos)
		{
			if (IsUpLine && (LinePos)trpos.m_Pos == LinePos.Center)
			{
				SetCenterLine((LinePos)mypos.m_Pos, IsActive, true);
				temppos.m_Line = mypos.m_Line - 1;
				temppos.m_Pos = mypos.m_Pos;
				Item_PDA_Research_Tree_Line nextline = m_FnGetLine(temppos.m_Line);
				nextline?.SetLine(temppos, trpos, IsActive, mypos);
			}
			else
			{
				if (!IsFirst && beforpos.m_Line == mypos.m_Line) SetRightLine((LinePos)mypos.m_Pos, IsActive);
				SetLeftLine((LinePos)mypos.m_Pos, IsActive);
				temppos.m_Line = mypos.m_Line;
				temppos.m_Pos = mypos.m_Pos - 1;
				SetLine(temppos, trpos, IsActive, mypos);
			}
		}
		else
		{
			SetCenterLine((LinePos)mypos.m_Pos, IsActive, IsUpLine);
			// 위로 올라가는 라인 생성하기
			if (beforpos.m_Pos < mypos.m_Pos)
			{
				SetLeftLine((LinePos)mypos.m_Pos, IsActive);
			}
			else if (beforpos.m_Pos > mypos.m_Pos)
			{
				SetRightLine((LinePos)mypos.m_Pos, IsActive);
			}
			else ActiveLine(pos, LineName.Center, IsActive || IsLineActiveColor(pos, LineName.Center));

			temppos.m_Line = mypos.m_Line - 1;
			temppos.m_Pos = mypos.m_Pos;
			Item_PDA_Research_Tree_Line nextline = m_FnGetLine(temppos.m_Line);
			nextline?.SetLine(temppos, trpos, IsActive, mypos);
		}
		//// 자기 자신의 상태에 맞게 셋팅해준다.
		//if (mypos.m_Line - trpos.m_Line > 1)
		//{
		//	// 자신으로 들어오는건 무조건 활성화
		//	if (IsFirst) ActiveLine(pos, LineName.Center_Down, IsActive);
		//	TResearchTable.TreePos temppos = new TResearchTable.TreePos();
		//	if (mypos.m_Pos < trpos.m_Pos)
		//	{
		//		if (!IsFirst) SetLeftLine((LinePos)mypos.m_Pos, IsActive);
		//		SetRightLine((LinePos)mypos.m_Pos, IsActive);
		//		temppos.m_Line = mypos.m_Line;
		//		temppos.m_Pos = mypos.m_Pos + 1;
		//		SetLine(temppos, trpos, IsActive, mypos);
		//	}
		//	else if (mypos.m_Pos > trpos.m_Pos)
		//	{
		//		if (!IsFirst) SetRightLine((LinePos)mypos.m_Pos, IsActive);
		//		SetLeftLine((LinePos)mypos.m_Pos, IsActive);
		//		temppos.m_Line = mypos.m_Line;
		//		temppos.m_Pos = mypos.m_Pos - 1;
		//		SetLine(temppos, trpos, IsActive, mypos);
		//	}
		//	else
		//	{
		//		ActiveLine(pos, mypos.m_Line - trpos.m_Line > 1 ? LineName.Center_UpItemNone : LineName.Center_UP, IsActive);
		//		// 위로 올라가는 라인 생성하기
		//		if (beforpos.m_Pos < mypos.m_Pos)
		//		{
		//			SetLeftLine((LinePos)mypos.m_Pos, IsActive);
		//		}
		//		else if (beforpos.m_Pos > mypos.m_Pos)
		//		{
		//			SetRightLine((LinePos)mypos.m_Pos, IsActive);
		//		}
		//		else ActiveLine(pos, LineName.Center, IsActive || IsLineActiveColor(pos, LineName.Center));

		//		temppos.m_Line = mypos.m_Line - 1;
		//		temppos.m_Pos = mypos.m_Pos;
		//		Item_PDA_Research_Tree_Line nextline = m_FnGetLine(temppos.m_Line);
		//		nextline?.SetLine(temppos, trpos, IsActive, mypos);
		//	}
		//}
		//else if(mypos.m_Pos == trpos.m_Pos)
		//{
		//	// 트리구조에서 해당 상황은 나와서는 안된다.
		//	if (IsLineActive(pos, LineName.Center_UpItemNone)) return;

		//	// 왼쪽에서 들어오면서 꺾이는 라인
		//	if (IsLineActive(pos, LineName.Left_In))
		//	{
		//		m_SUI.Line[(int)pos].Line[(int)LineName.Left_In].gameObject.SetActive(false);
		//		bool temp = m_SUI.Line[(int)pos].Active[(int)LineName.Left_In];
		//		// 왼쪽에서 들어오는 라인으로 변경
		//		ActiveLine(pos, LineName.Left, temp);

		//		// 위로 가야되므로 중앙 켜주기
		//		ActiveLine(pos, LineName.Center, temp);
		//	}

		//	// 오른쪽에서 들어오면서 꺾이는 라인
		//	if (IsLineActive(pos, LineName.Right_In))
		//	{
		//		m_SUI.Line[(int)pos].Line[(int)LineName.Right_In].gameObject.SetActive(false);

		//		bool temp = m_SUI.Line[(int)pos].Active[(int)LineName.Right_In];
		//		// 왼쪽에서 들어오는 라인으로 변경
		//		ActiveLine(pos, LineName.Right, temp);
		//		// 위로 가야되므로 중앙 켜주기
		//		ActiveLine(pos, LineName.Center, temp);
		//	}

		//	if (beforpos.m_Pos != mypos.m_Pos)
		//	{
		//		// 마지막 줄의경우 좌우 줄 셋팅을 해주어야됨
		//		if (beforpos.m_Pos < mypos.m_Pos) ActiveLine(pos, LineName.Left, IsActive);
		//		else if (beforpos.m_Pos > mypos.m_Pos) ActiveLine(pos, LineName.Right, IsActive);
		//	}
		//	else ActiveLine(pos, LineName.Center_Down, IsActive);

		//	// 위로 가야되므로 중앙 켜주기
		//	ActiveLine(pos, LineName.Center, IsActive);
		//	// 위로 올라가는 라인 켜주기
		//	ActiveLine(pos, LineName.Center_UP, IsActive);
		//}
		//else if(mypos.m_Pos < trpos.m_Pos)
		//{

		//	if (beforpos.m_Pos != mypos.m_Pos)
		//	{
		//		if (IsLineActive(pos, LineName.Right_In))
		//		{
		//			// 꺽이는 구간으로 셋팅되어있을때
		//			m_SUI.Line[(int)pos].Line[(int)LineName.Right_In].gameObject.SetActive(false);
		//			bool temp = IsActive;
		//			if (m_SUI.Line[(int)pos].Active[(int)LineName.Right_In]) temp = true;

		//			// 왼쪽에서 들어오는 라인으로 변경
		//			ActiveLine(pos, LineName.Right, temp);
		//			// 위로 가야되므로 중앙 켜주기
		//			ActiveLine(pos, LineName.Center, temp);
		//		}
		//		else
		//		{
		//			// 왼쪽에서 들어오는 라인으로 변경
		//			ActiveLine(pos, LineName.Right, IsActive);
		//			// 위로 가야되므로 중앙 켜주기
		//			ActiveLine(pos, LineName.Center, IsActive);
		//		}
		//		// 이전 줄에서 이동했으므로 왼쪽도 켜준다.
		//		ActiveLine(pos, LineName.Left, IsActive);
		//	}
		//	else if (!IsLineActive(pos, LineName.Center) // 가운데가 꺼져있을때
		//		|| (!IsLineActiveColor(pos, LineName.Center) && IsActive))  // 가운데가 비활성 상태이고 오른쪽 활성 상태라면
		//	{
		//		// 자신으로 들어오는건 무조건 활성화
		//		ActiveLine(pos, LineName.Center_Down, IsActive);
		//		// 꺽이는 구간
		//		ActiveLine(pos, LineName.Right_In, IsActive);
		//	}
		//	else
		//	{
		//		// 자신으로 들어오는건 무조건 활성화
		//		ActiveLine(pos, LineName.Center_Down, IsActive);
		//		ActiveLine(pos, LineName.Center, IsActive);
		//		ActiveLine(pos, LineName.Right, IsActive);
		//		if(beforpos.m_Pos != mypos.m_Pos) ActiveLine(pos, LineName.Left, IsActive);
		//	}

		//	// 다음 라인 셋팅을 위해 호출
		//	TResearchTable.TreePos temppos = new TResearchTable.TreePos()
		//	{
		//		m_Line = mypos.m_Line,
		//		m_Pos = mypos.m_Pos + 1
		//	};

		//	SetLine(temppos, trpos, IsActive, mypos);
		//}
		//else
		//{
		//	if (beforpos.m_Pos != mypos.m_Pos)
		//	{
		//		if (IsLineActive(pos, LineName.Left_In))
		//		{
		//			// 꺽이는 구간으로 셋팅되어있을때
		//			m_SUI.Line[(int)pos].Line[(int)LineName.Left_In].gameObject.SetActive(false);
		//			bool temp = IsActive;
		//			if (m_SUI.Line[(int)pos].Active[(int)LineName.Left_In]) temp = true;

		//			// 왼쪽에서 들어오는 라인으로 변경
		//			ActiveLine(pos, LineName.Left, temp);
		//			// 위로 가야되므로 중앙 켜주기
		//			ActiveLine(pos, LineName.Center, temp);
		//		}
		//		else
		//		{
		//			// 왼쪽에서 들어오는 라인으로 변경
		//			ActiveLine(pos, LineName.Left, IsActive);
		//			// 위로 가야되므로 중앙 켜주기
		//			ActiveLine(pos, LineName.Center, IsActive);
		//		}
		//		// 이전 줄에서 이동했으므로 왼쪽도 켜준다.
		//		ActiveLine(pos, LineName.Right, IsActive);
		//	}
		//	else if (!IsLineActive(pos, LineName.Center) // 가운데가 꺼져있을때
		//		|| (!IsLineActiveColor(pos, LineName.Center) && IsActive))  // 가운데가 비활성 상태이고 오른쪽 활성 상태라면
		//	{
		//		// 자신으로 들어오는건 무조건 활성화
		//		ActiveLine(pos, LineName.Center_Down, IsActive);
		//		// 꺽이는 구간
		//		ActiveLine(pos, LineName.Left_In, IsActive);
		//	}
		//	else
		//	{
		//		// 자신으로 들어오는건 무조건 활성화
		//		ActiveLine(pos, LineName.Center_Down, IsActive);
		//		ActiveLine(pos, LineName.Center, IsActive);
		//		ActiveLine(pos, LineName.Left, IsActive);
		//		if (beforpos.m_Pos != mypos.m_Pos) ActiveLine(pos, LineName.Right, IsActive);
		//	}

		//	// 다음 라인 셋팅을 위해 호출
		//	TResearchTable.TreePos temppos = new TResearchTable.TreePos()
		//	{
		//		m_Line = mypos.m_Line,
		//		m_Pos = mypos.m_Pos - 1
		//	};

		//	SetLine(temppos, trpos, IsActive, mypos);
		//}
	}

	void SetRightLine(LinePos pos, bool IsActive)
	{
		bool temp = IsActive;
		// 왼쪽에서 들어오는게 이미 있다면
		if (IsLineActive(pos, LineName.Left_In))
		{
			m_SUI.Line[(int)pos].Line[(int)LineName.Left_In].gameObject.SetActive(false);
			if (IsLineActiveColor(pos, LineName.Left_In)) temp = true;
			ActiveLine(pos, LineName.Left, temp);
			ActiveLine(pos, LineName.Center, temp || IsLineActiveColor(pos, LineName.Center));
			ActiveLine(pos, LineName.Right, IsActive);
		}
		// 가운데 라인 활성상태라면
		else if(IsLineActive(pos, LineName.Center_UP) || IsLineActive(pos, LineName.Center_UpItemNone))
		{
			// 센터의 색상을 위해 다시 셋팅
			ActiveLine(pos, LineName.Center, IsActive || IsLineActiveColor(pos, LineName.Center));
			ActiveLine(pos, LineName.Right, IsActive);
		}
		else
		{
			ActiveLine(pos, LineName.Right_In, IsActive);
		}
	}

	void SetLeftLine(LinePos pos, bool IsActive)
	{
		bool temp = IsActive;
		// 왼쪽에서 들어오는게 이미 있다면
		if (IsLineActive(pos, LineName.Right_In))
		{
			m_SUI.Line[(int)pos].Line[(int)LineName.Right_In].gameObject.SetActive(false);
			if (IsLineActiveColor(pos, LineName.Right_In)) temp = true;
			ActiveLine(pos, LineName.Right, temp);
			ActiveLine(pos, LineName.Center, temp || IsLineActiveColor(pos, LineName.Center));
			ActiveLine(pos, LineName.Left, IsActive);
		}
		// 가운데 라인 활성상태라면
		else if (IsLineActive(pos, LineName.Center_UP) || IsLineActive(pos, LineName.Center_UpItemNone))
		{
			// 센터의 색상을 위해 다시 셋팅
			ActiveLine(pos, LineName.Center, IsActive || IsLineActiveColor(pos, LineName.Center));
			ActiveLine(pos, LineName.Left, IsActive);
		}
		else
		{
			ActiveLine(pos, LineName.Left_In, IsActive);
		}
	}
	void SetCenterLine(LinePos pos, bool IsActive, bool IsNone)
	{
		ActiveLine(pos, IsNone ? LineName.Center_UpItemNone : LineName.Center_UP, IsActive);
		ActiveLine(pos, LineName.Center, IsActive);
		bool temp = IsActive;
		// 왼쪽에서 들어오는게 이미 있다면
		if (IsLineActive(pos, LineName.Right_In))
		{
			m_SUI.Line[(int)pos].Line[(int)LineName.Right_In].gameObject.SetActive(false);
			if (IsLineActiveColor(pos, LineName.Right_In)) temp = true;
			ActiveLine(pos, LineName.Right, temp);
		}

		// 왼쪽에서 들어오는게 이미 있다면
		if (IsLineActive(pos, LineName.Left_In))
		{
			m_SUI.Line[(int)pos].Line[(int)LineName.Left_In].gameObject.SetActive(false);
			if (IsLineActiveColor(pos, LineName.Left_In)) temp = true;
			ActiveLine(pos, LineName.Left, temp);
		}
	}

	public void ActiveLine(LinePos pos, LineName line, bool Active)
	{
		// 이미 색상까지 활성화가 되었으면 셋팅해줄 필요없음
		if (m_SUI.Line[(int)pos].Active[(int)line]) return;
		Color col = m_SUI.LineColor[Active ? 1 : 0];
		// 이미 활성화 되었다면 무시
		if(!m_SUI.Line[(int)pos].Line[(int)line].gameObject.activeSelf) m_SUI.Line[(int)pos].Line[(int)line].gameObject.SetActive(true);
		m_SUI.Line[(int)pos].Line[(int)line].color = col;
		m_SUI.Line[(int)pos].Active[(int)line] = Active;
	}

	bool IsLineActive(LinePos pos, LineName line)
	{
		return m_SUI.Line[(int)pos].Line[(int)line].gameObject.activeSelf;
	}
	bool IsLineActiveColor(LinePos pos, LineName line)
	{
		return m_SUI.Line[(int)pos].Line[(int)line].gameObject.activeSelf && m_SUI.Line[(int)pos].Active[(int)line];
	}
}
