using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Battle_Note_Normal : Item_Battle_Note_Base
{
#pragma warning disable 0649
	[SerializeField] CircleCollider2D m_Collider;
#pragma warning restore 0649
	public override void SetData(EBodyType body, float endtime, params object[] valus)
	{
		base.SetData(body, endtime, valus);
		m_Mode = ENoteType.Normal;
	}

	protected override void SetTime(float time)
	{
		m_PlayTime = time;
		float Scale = GetScaleToTime();
		m_TimeGauge.localScale = new Vector3(Scale, Scale, 1f);
		float Angle = (Scale - 1f) * (30f / (m_TimeStartScale - 1f));
		m_TimeGauge.eulerAngles = new Vector3(0f, 0f, Angle);
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
		while (m_PlayTime < m_MaxTime)
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
		//if (TUTO.IsTuto(TutoKind.Start, (int)TutoType_Start.DLG22_End))
		Vector3 pos = Utile_Class.GetWorldPosition(InputPos);
		bool hit = m_Collider.OverlapPoint(pos);
		if(hit)
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
				Damage = Mathf.RoundToInt(Damage * BaseValue.NOTE_DMG_RATIO(ENoteType.Normal) * cridmg);
				Debug.Log(string.Format("HitState:{0} // Dmg:{1} // CriDmg:{2}, ", state.ToString(), Damage, cridmg));
				enemy.OnHit(state, m_Mode, m_Size, Damage, rate, transform.position);
			}
		}
		return hit;
	}

	ENoteHitState GetHitState()
	{
		ENoteHitState state;
		float rate = GetScaleToTime();
		if (rate <= 0.9f) state = ENoteHitState.GOOD;
		else if (rate <= 1f) state = ENoteHitState.PERPECT;
		else if (rate <= 1.9f) state = ENoteHitState.GOOD;
		else state = ENoteHitState.MISS;
		return state;
	}

	protected override float GetHitRate(out ENoteHitState state)
	{
		state = ENoteHitState.MISS;
		if (m_State != NoteState.Play) return 0f;
		float times = 0;
		float rate = GetScaleToTime();
		string trigger = "Hit";
		if (rate <= 0.9f)
		{
			state = ENoteHitState.GOOD;
			times = 0.1f + rate;
		}
		else if(rate <= 1.05f)
		{
			state = ENoteHitState.PERPECT;
			times = rate * 1.5f;
		}
		else if(rate <= 1.9f)
		{
			state = ENoteHitState.GOOD;
			times = 2f - rate;
		}
		else
		{
			state = ENoteHitState.MISS;
			trigger = "Miss";
		}
		if(times < 0.1f)
		{
			state = ENoteHitState.MISS;
			trigger = "Miss";
			times = 0;
		}
		m_State = NoteState.End;
		m_Ani.SetTrigger(trigger);
		StopCoroutine(m_Play);
		return times;// HitDamagePer();
	}
}
