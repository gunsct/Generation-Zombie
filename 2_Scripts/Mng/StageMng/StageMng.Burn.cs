﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum BurnMode
{
	AREA1 = 0,
	AREA3,
	Line
}

public class BurnInfo
{
	public BurnMode m_Type;
	public int m_TurnCnt = 0;
	int _Line, _Pos;
	public int m_Line
	{
		set { _Line = value; }
		get { return m_Target ? m_Target.m_Line : _Line; }
	}
	public int m_Pos
	{
		set { _Pos = value; }
		get { return m_Target ? m_Target.m_Pos : _Pos; }
	}
	// 따라다닐 대상
	public Item_Stage m_Target;
	public GameObject m_Eff;

	public BurnInfo(BurnMode type)
	{
		m_Type = type;
	}

	public void SetTarget(Item_Stage target)
	{
		m_Target = target;
	}
	public void SetTarget(int line, int pos)
	{
		m_Line = line;
		m_Pos = pos;
	}

	public void SetTurn(int cnt)
	{
		m_TurnCnt = cnt;
	}
	public bool ISPosMoveType()
	{
		switch (m_Type)
		{
		case BurnMode.AREA1:
		case BurnMode.AREA3:
		case BurnMode.Line:
			return true;
		}
		return false;
	}
	public bool ISLineMoveType()
	{
		switch (m_Type)
		{
		case BurnMode.AREA1:
		case BurnMode.AREA3:
		case BurnMode.Line:
			return true;
		}
		return false;
	}

	public void SetEff(GameObject obj)
	{
		m_Eff = obj;
		m_Eff.transform.localEulerAngles = Vector3.zero;
	}

	public void Update(float MoveX, float MoveY)
	{
		if (m_Eff == null) return;
		if (!m_Target)
		{
			// 자신의 위치의 좌표로 셋팅

			// 새로운 라인 추가
			float interverX = BaseValue.STAGE_INTERVER.x;
			float interverY = BaseValue.STAGE_INTERVER.y;
			float y = interverY * m_Line;
			float x = ((3 + m_Line * 2 - 1) * interverX * -0.5f) + interverX * m_Pos;
			Vector3 pos = new Vector3(x, y, 0);
			switch (m_Type)
			{
			default:
				pos.x += MoveX;
				pos.y += MoveY;
				break;
			}
			m_Eff.transform.localPosition = pos;
		}
		else m_Eff.transform.position = m_Target.transform.GetChild(0).position;//전투나 이동같은것 때문에 아래 패널을 따라다니게 함
	}

	public bool TurnCheck(int selectpos)
	{
		m_TurnCnt--;
		if (m_Target)
		{
			m_Line = m_Target.m_Line;
			m_Pos = m_Target.m_Pos;
		}
		else
		{
			if (ISLineMoveType()) m_Line--;
			if (ISPosMoveType())
			{
				switch (m_Type)
				{
				case BurnMode.Line:
					switch (selectpos)
					{
					case 0:
						m_Pos++;
						break;
					case 2:
						m_Pos--;
						break;
					}
					break;
				default:
					switch (selectpos)
					{
					case 1:
						m_Pos--;
						break;
					case 2:
						m_Pos -= 2;
						break;
					}
					break;
				}
			}
		}
		return m_TurnCnt < 0;
	}

	public bool IsAreaCard(Item_Stage card)
	{
		return IsAreaCard(card.m_Line, card.m_Pos);
	}

	public bool IsAreaCard(int line, int pos)
	{
		if(m_Target)
		{
			m_Line = m_Target.m_Line;
			m_Pos = m_Target.m_Pos;
		}
		switch (m_Type)
		{
		case BurnMode.AREA1:
			return line == m_Line && pos == m_Pos;
		case BurnMode.AREA3:
			// 타겟 기준 3x3
			if (Math.Abs(line - m_Line) > 1) return false;
			if (Math.Abs((pos - (line - m_Line)) - m_Pos) > 1) return false;
			return true;
		case BurnMode.Line:
			return line == m_Line;
		}
		return false;
	}

	public bool Equals(BurnInfo info)
	{
		return m_Type == info.m_Type && m_Line == info.m_Line && m_Pos == info.m_Pos;
	}
}
public partial class StageMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Light
	public List<BurnInfo> m_BurnInfos = new List<BurnInfo>();
	public void RemoveBurnAll()
	{
		for (int i = m_BurnInfos.Count - 1; i > -1; i--) RemoveBurnInfo(m_BurnInfos[i]);

	}
	public void RemoveBurnInfo(BurnInfo info)
	{
		m_BurnInfos.Remove(info);
		if (info.m_Eff) Destroy(info.m_Eff);
	}

	public void AddBurnInfo(BurnInfo info)
	{
		for (int i = m_BurnInfos.Count - 1; i > -1; i--)
		{
			BurnInfo temp = m_BurnInfos[i];
			if (temp.Equals(info)) RemoveBurnInfo(temp);

		}
		m_BurnInfos.Add(info);
	}
	public void PlusBurnInfo(Item_Stage _card, int _add) {
		for (int i = m_BurnInfos.Count - 1; i > -1; i--) {
			if (m_BurnInfos[i].m_Target == _card) m_BurnInfos[i].m_TurnCnt += _add;
		}
	}
	public bool IS_BurnInfoCard(Item_Stage card)
	{
		for (int i = m_BurnInfos.Count - 1; i > -1; i--)
		{
			if (m_BurnInfos[i].IsAreaCard(card)) return true;
		}
		return false;
	}

	public void CheckBurnInfoTurn(int selectpos)
	{
		for (int i = m_BurnInfos.Count - 1; i > -1; i--)
		{
			BurnInfo info = m_BurnInfos[i];
			if (info.TurnCheck(selectpos)) RemoveBurnInfo(info);
			else if (info.m_Line <= 0) RemoveBurnInfo(info);
			else if (info.m_Pos < 0 || info.m_Pos >= m_ViewCard[info.m_Line].Length) RemoveBurnInfo(info);
			else if (info.m_Target.IS_Die()) RemoveBurnInfo(info);
		}
	}
}