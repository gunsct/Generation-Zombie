using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public enum LightMode
{
	LightStick = 0,
	StreetLight,
	FlashLight,
	Line,
	StarShell
}

public class LightInfo
{
	public LightMode m_Type;
	public int m_TurnCnt = 0;
	int _Line, _Pos;
	/// <summary> 밝히는 범위,라인형 제외 </summary>
	int m_Area = 1;
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

	public LightInfo(LightMode type, int _area = 1)
	{
		m_Type = type;
		m_Area = _area;
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
		case LightMode.LightStick:
		//case LightMode.StreetLight:
		case LightMode.FlashLight:
		case LightMode.Line:
		case LightMode.StarShell:
			return true;
		}
		return false;
	}
	public bool ISLineMoveType()
	{
		switch (m_Type)
		{
		case LightMode.LightStick:
		//case LightMode.StreetLight:
		case LightMode.Line:
		case LightMode.StarShell:
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
			float x = ((3 + m_Line * 2 - 1) * interverX * -0.5f) + interverX * m_Pos + MoveX;
			Vector3 pos = new Vector3(x, y, 0);
			switch (m_Type)//플래시라이트는 이펙트 고정
			{
			case LightMode.StreetLight:
				return;
			case LightMode.FlashLight:
				break;
			default:
				pos.y += MoveY;
				break;
			}
			m_Eff.transform.localPosition = pos;
		}
		else m_Eff.transform.position = m_Target.transform.position;
	}

	public bool TurnCheck(int selectpos)
	{
		m_TurnCnt--;
		if (ISLineMoveType()) m_Line--;
		if (ISPosMoveType())
		{
			switch (m_Type)
			{
			case LightMode.LightStick://두번째 줄부터는 내려올때 왼쪽부터 0으로 채워지니 가운데면 -1 오른쪽이면 -2
			case LightMode.StarShell:
			case LightMode.StreetLight:
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
			case LightMode.FlashLight://첫번째 줄에 있는건 좌우로 이동만 같이
			case LightMode.Line:
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
			}
		}
		return m_TurnCnt < 0;
	}

	public bool IsLightCard(Item_Stage card) {
		bool islight = IsLightCard(card.m_Line, card.m_Pos);
		//가로등만으로 밝혀지는지 체크
		if(!card.IS_NotOnlyStreetLight && islight) card.IS_NotOnlyStreetLight = m_Type != LightMode.StreetLight && m_Type != LightMode.LightStick;
		return islight;
	}
	/// <summary> 특정 카드의 라인, 포지션으로 밝힘 상태인지 체크 </summary>
	public bool IsLightCard(int line, int pos) {
		switch (m_Type)
		{
			case LightMode.LightStick:
			case LightMode.StreetLight:
			case LightMode.StarShell:
				if (Math.Abs(line - m_Line) > m_Area) return false;
				if (Math.Abs((pos - (line - m_Line)) - m_Pos) > m_Area) return false;//한줄 넘길 때 마다 0번 인덱스가 왼쪽으로 1칸씩 가니까 라인 차이만큼 아래면 더해주고 위면 빼주고
				return true;
		case LightMode.FlashLight:
			// 한 열 전체
			return m_Pos + (line - m_Line) == pos;
		case LightMode.Line:
			return m_Line == line;
		}
		return false;
	}

	public bool Equals(LightInfo info)
	{
		return m_Type == info.m_Type && m_Line == info.m_Line && m_Pos == info.m_Pos;
	}
}
public partial class StageMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Light
	List<LightInfo> m_Lights = new List<LightInfo>();

	public void RemoveLightAll()
	{
		for (int i = m_Lights.Count - 1; i > -1; i--) RemoveLight(m_Lights[i]);
	}

	public void RemoveTargetLight(Item_Stage card)
	{
		if (card == null) return;
		for (int i = m_Lights.Count - 1; i > -1; i--)
		{
			LightInfo info = m_Lights[i];
			if (info.m_Target != null && info.m_Target.Equals(card)) RemoveLight(info);
		}
	}
	public void RemoveLight(LightInfo light)
	{
		m_Lights.Remove(light);
		if (light.m_Eff) Destroy(light.m_Eff);
	}

	/// <summary> 라이트스틱, 플래시라이트, 조명탄 등 이펙트 등록 </summary>
	public void AddLight(LightInfo light)
	{
		for (int i = m_Lights.Count - 1; i > -1; i--)
		{
			LightInfo info = m_Lights[i];
			if (info.Equals(light)) RemoveLight(info);

		}
		m_Lights.Add(light);
	}

	/// <summary> 해당 카드가 밝혀져 있는지 체크 </summary>
	public bool IS_LightInfoCard(Item_Stage card, bool ISGuard)
	{
		if (card.m_Line < (ISGuard ? 2 : 0) + Mathf.Clamp(STAGEINFO.m_TStage.GetDarkLv - m_User.m_TurnCnt, 1, STAGEINFO.m_TStage.GetDarkLv)) return true;//기본으로 밝혀지는 라인이면 밝혀진것

		bool islight = false;//IsLightCard 함수에서 라이트 종류별로 가로등만 밝혀졌는지 체크
		for (int i = m_Lights.Count - 1; i > -1; i--)//생성되있는 라이트들에 해당 카드가 밝힘 범위에 있는지 체크
		{
			if (m_Lights[i].IsLightCard(card) && !islight) islight = true;
		}
		return islight;
	}

	/// <summary> 스테이지 카드 맨 하단 3개중 택한거 끝나고 0,1,2(좌중우)에 대해 턴과 이동 체크</summary>
	public IEnumerator CheckLightTurn(int selectpos)
	{
		for (int i = m_Lights.Count - 1; i > -1; i--)
		{
			LightInfo info = m_Lights[i];
			if (info.TurnCheck(selectpos)) RemoveLight(info);//턴 다되서 제거
			else if(info.ISPosMoveType())
			{
				if (info.m_Line <= 0 || info.m_Pos < 0 || info.m_Pos >= m_ViewCard[info.m_Line].Length) RemoveLight(info);//턴은 남았지만 카드가 맨아래열에서 내려와 제거
			}
		}
		yield return Check_LightOnOff();
	}

	/// <summary> 스테이지 카드들과 라이트계열 체크 </summary>
	public IEnumerator Check_LightOnOff() {
		//시너지
		bool ISGuard = m_User.GetSynergeValue(JobType.Guard, 0) != null;
		List<Item_Stage> actioncard = new List<Item_Stage>();

		for (int j = 0, jMax = AI_MAXLINE; j <= jMax; j++) {
			for (int i = 0, iMax = m_ViewCard[j].Length; i < iMax; i++) {
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				item.IS_NotOnlyStreetLight = false;
				if (item.m_IsLight) {
					// 이미 라이트로 등록된 상태일때
					// 라이트 리스트중 해당 좌표에 라이트가 아니라면 해제
					if (IS_LightInfoCard(item, ISGuard)) {
						StreetLightShadowCheck(item, ISGuard);
						continue;//밝힘 상태인 카드를 체크해서 어둠이어야 하면 액션과 상태 변경
					}
					actioncard.Add(item);
					item.Action(EItem_Stage_Card_Action.DarkCardOn, 0, (obj) => {
						actioncard.Remove(obj);
						StreetLightShadowCheck(item, ISGuard);
					});
				}
				else {
					if (!IS_LightInfoCard(item, ISGuard)) {
						StreetLightShadowCheck(item, ISGuard);
						continue;//어둠 상태인 카드를 체크해서 밝힘이어야 하면 액션과 상태 변경
					}
					actioncard.Add(item);
					item.Action(EItem_Stage_Card_Action.DarkCardOff, 0, (obj) => {
						actioncard.Remove(obj);
					});
					StreetLightShadowCheck(item, ISGuard);
				}
			}
		}
		yield return new WaitWhile(() => actioncard.Count > 0);
	}

	/// <summary> 어둠 카드가 가로등으로만 밝혀졌을때 마스킹된 그림자 생성하는지 체크 
	/// (어둠레벨 + 시너지 + 턴 수)자동밝힘 스타트 라인으로 밝힌게 아니고
	/// 어둠 카드 이고
	/// 밝혀진 카드이고
	/// 가로등만으로 밝혀진 경우
	/// </summary>
	void StreetLightShadowCheck(Item_Stage _item, bool _isguard) {
		
		bool autolight = _item.m_Line < (_isguard ? 2 : 0) + Mathf.Clamp(STAGEINFO.m_TStage.GetDarkLv - m_User.m_TurnCnt, 1, STAGEINFO.m_TStage.GetDarkLv);
		_item.SetStreetLightShadow(!autolight && _item.m_Info.IsDarkCard && _item.m_Info.IsLight && !_item.IS_NotOnlyStreetLight);
	}
}
