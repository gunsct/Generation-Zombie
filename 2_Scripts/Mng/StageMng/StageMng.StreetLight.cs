using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StreetLightInfo
{
	public int m_TurnCnt = 0;
	int _Line, _Pos;
	bool Is_Move;
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

	public StreetLightInfo(){}

	public void SetTarget(Item_Stage target)
	{
		m_Target = target;
	}
	public void SetTarget(int line, int pos, bool _move = false)
	{
		m_Line = line;
		m_Pos = pos;
		Is_Move = _move;
	}

	public void SetTurn(int cnt)
	{
		m_TurnCnt = cnt;
	}

	public void SetEff(GameObject obj)
	{
		m_Eff = obj;
		m_Eff.transform.localEulerAngles = Vector3.zero;
	}

	public void Update(float MoveX, float MoveY)
	{
		if (m_Eff == null || !Is_Move) return;
		if (!m_Target)
		{
			// 자신의 위치의 좌표로 셋팅

			// 새로운 라인 추가
			float interverX = BaseValue.STAGE_INTERVER.x;
			float interverY = BaseValue.STAGE_INTERVER.y;
			float y = interverY * m_Line;
			float x = ((3 + m_Line * 2 - 1) * interverX * -0.5f) + interverX * m_Pos;
			Vector3 pos = new Vector3(x, y, 0);
			pos.x += MoveX;
			pos.y += MoveY;
			m_Eff.transform.localPosition = pos;
		}
		else m_Eff.transform.position = m_Target.transform.position;
	}

	public bool TurnCheck(int selectpos)
	{
		m_TurnCnt--;
		if (Is_Move) {
			if (m_Target) {
				m_Line = m_Target.m_Line;
				m_Pos = m_Target.m_Pos;
			}
			else {
				m_Line--;
				switch (selectpos) {
					case 1:
						m_Pos--;
						break;
					case 2:
						m_Pos -= 2;
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
		// 타겟 기준 3x3
		if (Math.Abs(line - m_Line) > 1) return false;
		if (Math.Abs((pos - (line - m_Line)) - m_Pos) > 1) return false;
		return true;
	}

	public bool Equals(StreetLightInfo info)
	{
		return m_Line == info.m_Line && m_Pos == info.m_Pos;
	}
}
public partial class StageMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Light
	List<StreetLightInfo> m_StreetLightInfos = new List<StreetLightInfo>();
	public void RemoveStreetLightAll()
	{
		for (int i = m_StreetLightInfos.Count - 1; i > -1; i--) RemoveStreetLightInfo(m_StreetLightInfos[i]);

	}
	public void RemoveStreetLightInfo(StreetLightInfo info)
	{
		m_StreetLightInfos.Remove(info);
		if (info.m_Eff) Destroy(info.m_Eff);
	}

	public void AddStreetLightInfo(StreetLightInfo info)
	{
		for (int i = m_StreetLightInfos.Count - 1; i > -1; i--)
		{
			StreetLightInfo temp = m_StreetLightInfos[i];
			if (temp.Equals(info)) RemoveStreetLightInfo(temp);

		}
		m_StreetLightInfos.Add(info);
	}

	public bool IS_StreetLightInfoCard(Item_Stage card)
	{
		for (int i = m_StreetLightInfos.Count - 1; i > -1; i--)
		{
			if (m_StreetLightInfos[i].IsAreaCard(card)) return true;
		}
		return false;
	}

	public void CheckStreetLightInfoTurn(int selectpos)
	{
		for (int i = m_StreetLightInfos.Count - 1; i > -1; i--)
		{
			StreetLightInfo info = m_StreetLightInfos[i];
			if (info.TurnCheck(selectpos)) RemoveStreetLightInfo(info);
			else if (info.m_Line <= 0) RemoveStreetLightInfo(info);
			else if(info.m_Pos < 0 || info.m_Pos >= m_ViewCard[info.m_Line].Length) RemoveStreetLightInfo(info);
		}
	}
}
