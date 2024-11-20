using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Battle_Note_Slice : Item_Battle_Note_Base
{
#pragma warning disable 0649
	public Transform m_RotatePanel;
	[SerializeField] CircleCollider2D m_Area;
	[SerializeField] CircleCollider2D m_Check;
	public float m_SuccessAngle = 90f;
#pragma warning restore 0649
	public override void SetData(EBodyType body, float endtime, params object[] valus)
	{
		base.SetData(body, endtime, valus);
		m_Mode = ENoteType.Slash;
		m_RotatePanel.eulerAngles = new Vector3(0, 0, UTILE.Get_Random(0f, 360f));
	}

	protected override void SetTime(float time)
	{
		m_PlayTime = time;
		SpriteRenderer img = m_TimeGauge.GetComponent<SpriteRenderer>();
		img.material.SetFloat("_FillAmount", GetTimeRate());
	}

	public override float GetRound() { return GetCheckSize(); }
	public override float GetSize() {
		return m_Area.radius * m_Area.transform.lossyScale.x;
	}
	public float GetCheckSize()
	{
		return m_Check.radius * m_Check.transform.lossyScale.x;
	}

	protected override IEnumerator Play()
	{
		m_State = NoteState.Play;
		SetTime(0f);
		m_Ani.SetTrigger("Start");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile( () => Utile_Class.IsAniPlay(m_Ani));

		// 노드가 플레이됨
		while(m_PlayTime < m_MaxTime)
		{
			yield return new WaitForFixedUpdate();
			SetTime(m_PlayTime + Time.fixedDeltaTime);
		}

		m_State = NoteState.End;
		SetTime(m_MaxTime);
		m_Ani.SetTrigger("Miss");
		PlayEffSound(SND_IDX.SFX_0990);
	}

	public override bool OnPressed(int id, Vector2 InputPos)
	{
		if (m_State != NoteState.Play) return false;
		if (POPUP.IS_PopupUI()) return false;

		Vector3 pos = Utile_Class.GetWorldPosition(InputPos);
		return m_Check.OverlapPoint(pos);
	}

	public override bool OnSliced(Vector2 start, Vector2 end)
	{
		if (m_State != NoteState.Play) return false;
		if (POPUP.IS_PopupUI()) return false;
		Vector3 SPos = Utile_Class.GetWorldPosition(start);
		Vector3 EPos = Utile_Class.GetWorldPosition(end);
		bool hit = ISIntersection(SPos, EPos);
		if (hit)
		{
			if (IsSlice(SPos, EPos))
			{
				ENoteHitState state;
				float rate = GetHitRate(out state);
				if (state != ENoteHitState.MISS)
				{
					Enemy_Controller enemy = BATTLE.GetEnemy();
					int Damage = BATTLEINFO.GetNoteDamage(BATTLEINFO.GetAtk(m_Mode)
						, STAGE_USERINFO.m_CharLV
						, BATTLEINFO.GetEnemyDef()
						, rate * BATTLEINFO.m_EnemyTData.m_Body[(int)m_Body].Ratio
						, state);
					float cridmg = 1f + (state == ENoteHitState.PERPECT ? (0.5f + 0.5f * STAGE_USERINFO.GetStat(StatType.CriticalDmg)) : 0f);
					Damage = Mathf.RoundToInt(Damage * BaseValue.NOTE_DMG_RATIO(ENoteType.Slash) * cridmg);
					enemy.OnHit(state, m_Mode, m_Size, Damage, rate, transform.position);
				}
			}
			else
			{
				m_State = NoteState.End;
				m_Ani.SetTrigger("Miss");
				StopCoroutine(m_Play);
			}
		}
		return hit;
	}

	bool ISIntersection(Vector3 start, Vector3 end)
	{
		Vector2 pos1, pos2;
		float radius = GetCheckSize();
		int d = UTILE.FindLineCircleIntersection(transform.position, radius, start, end, out pos1, out pos2);

		if (d < 1) return false;

		bool hit = false;
		float dis = Vector3.Distance(start, end);
		float minX = Mathf.Min(start.x, end.x);
		float maxX = Mathf.Max(start.x, end.x);
		float minY = Mathf.Min(start.y, end.y);
		float maxY = Mathf.Max(start.y, end.y);
		// 교점이 라인선상에 있는지 확인
		if (Vector3.Distance(transform.position, start) < radius || Vector3.Distance(transform.position, end) < radius)
		{
			// 시작점이나 끝점이 원 안에 있을때
			// 교점이 선안에 있는지만 확인 하면 원안에서 시작과 끝이 있으면 결과가 나오지 않으므로 원안에 있는지 확인
			hit = true;
		}
		else if ((minX <= pos1.x && maxX >= pos1.x && minY <= pos1.y && maxY >= pos1.y)
			|| (minX <= pos2.x && maxX >= pos2.x && minY <= pos2.y && maxY >= pos2.y))
		{
			// pos1, pos2 는 항상 교점을 찍어주므로 영역안에있으면 라인선상에 있는것이다.
			// 정확한 연산을하면 float이라 오차가 있을수있음
			hit = true;
		}

		return hit;
	}

	bool IsSlice(Vector3 start, Vector3 end)
	{
		float noteangle = m_RotatePanel.eulerAngles.z % 360f;
		float minangle = noteangle - m_SuccessAngle * 0.5f;
		float maxangle = noteangle + m_SuccessAngle * 0.5f;
		float angle = Utile_Class.GetAngle(start, end);
		if (minangle <= 0 && angle >= 270f)
		{
			maxangle = 360f;
			minangle = 360f + minangle;
			return angle >= minangle && angle <= maxangle;
		}
		return angle >= minangle && angle <= maxangle;
	}

	protected override float GetHitRate(out ENoteHitState state)
	{
		state = ENoteHitState.MISS;
		if (m_State != NoteState.Play) return 0f;
		float times = 0;
		float rate = GetScaleToTime();
		string trigger = "Hit";
		if (m_PlayTime < m_MaxTime * 2f / 3f) {//제한시간 2/3 미만 성공시 퍼펙트
			state = ENoteHitState.PERPECT;
			times = 1.5f;
		}
		else {
			state = ENoteHitState.GOOD;
			times = 1f;
		}
		m_State = NoteState.End;
		m_Ani.SetTrigger(trigger);
		StopCoroutine(m_Play);
		return times;// HitDamagePer();
	}
}
