using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Battle_Note_Run : Item_Battle_Note_Base
{
#pragma warning disable 0649
	[SerializeField] CircleCollider2D m_Collider;
	[SerializeField] Transform m_MissPanel;
	Action<ENoteHitState, Item_Battle_Note_Run> m_EndCB;
#pragma warning restore 0649
	public override void SetData(EBodyType body, float endtime, params object[] valus)
	{
		float Add = 0f;
		float Per = 1f;// + USERINFO.GetSkillValue(SkillKind.Flexible);
		endtime = BaseValue.CalcValue(endtime, Add, Per);
		base.SetData(body, endtime, valus);
		m_Mode = ENoteType.Run;
	}

	protected override void SetTime(float time)
	{
		m_PlayTime = time;
		float Scale = GetScaleToTime();
		m_TimeGauge.localScale = new Vector3(Scale, Scale, 1f);
	}

	public void SetFootPos(int nPos, float Angle)
	{
		// 기본 으로 돌려준후 다시 셋팅해준다.
		transform.eulerAngles = Vector3.zero;

		m_SizeScale.eulerAngles = new Vector3(nPos == 1 ? 0f : 180f, 0f, 0f);

		transform.eulerAngles = new Vector3(0f, 0f, Angle);

		m_MissPanel.eulerAngles = Vector3.zero;
	}

	public override float GetRound() { return GetSize(); }
	//float size = tempNote.m_Collider.radius * tempNote.m_Collider.transform.localScale.x;
	public override float GetSize() {
		return m_Collider.radius * m_Collider.transform.lossyScale.x;
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
	}

	public void SetMiss()
	{
		if (m_State != NoteState.Play) return;
		if(m_Play != null)  StopCoroutine(m_Play);
		m_State = NoteState.End;
		SetTime(m_MaxTime);
		m_Ani.SetTrigger("Miss");
	}

	public override bool OnPressed(int id, Vector2 InputPos)
	{
		if (m_State != NoteState.Play) return false;
		Vector3 pos = Utile_Class.GetWorldPosition(InputPos);
		bool hit = m_Collider.OverlapPoint(pos);
		if(hit)
		{
			ENoteHitState state;
			GetHitRate(out state);
		}
		return hit;
	}

	protected override float GetHitRate(out ENoteHitState state)
	{
		state = ENoteHitState.MISS;
		float times = 0;
		float rate = GetScaleToTime();
		string trigger = "Hit";
		if(rate > 0.6f && rate <= 1.4f)
		{
			state = ENoteHitState.PERPECT;
		}
		else
		{
			state = ENoteHitState.MISS;
			trigger = "Miss";
		}

		m_State = NoteState.End;
		m_Ani.SetTrigger(trigger);
		StopCoroutine(m_Play);
		return times;// HitDamagePer();
	}
}
