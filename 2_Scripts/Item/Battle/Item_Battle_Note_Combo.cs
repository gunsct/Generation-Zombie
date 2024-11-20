using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Battle_Note_Combo : Item_Battle_Note_Base
{
#pragma warning disable 0649
	[SerializeField] GameObject m_HitEff;
	[SerializeField] Transform m_Sides;
	[SerializeField] CircleCollider2D m_Collider;
	int m_MaxCnt = 0;
	int m_Value = 0;
#pragma warning restore 0649

	public override void SetData(EBodyType body, float endtime, params object[] valus)
	{
		base.SetData(body, endtime, valus);
		m_Mode = ENoteType.Combo;
		m_MaxCnt = m_Value = (int)valus[0];
		for (int i = m_Sides.childCount - 1; i > -1; i--)
		{
			m_Sides.GetChild(i).gameObject.SetActive(i < m_Value);
		}
	}
	public override void SetSize(ENoteSize size)
	{
		m_Size = size;
		float scale = BaseValue.NOTE_SCALE[(int)size] / 0.7f;
		m_SizeScale.localScale = new Vector3(scale, scale, 1f);
	}

	void SetHit()
	{
		if (m_Value < 0) return;
		--m_Value;
		Transform side = m_Sides.GetChild(m_Value);
		side.gameObject.SetActive(false);
		GameObject eff = Utile_Class.Instantiate(m_HitEff);
		eff.transform.localScale = side.lossyScale;
		eff.transform.position = side.position;

		if (m_Value < 1)
		{
			ENoteHitState state;
			float rate = GetHitRate(out state);
			Enemy_Controller enemy = BATTLE.GetEnemy();
			int Damage = BATTLEINFO.GetNoteDamage(BATTLEINFO.GetAtk(m_Mode)
				, STAGE_USERINFO.m_CharLV
				, BATTLEINFO.GetEnemyDef()
				, rate * BATTLEINFO.m_EnemyTData.m_Body[(int)m_Body].Ratio
				, state);
			Damage = Mathf.RoundToInt(Damage * BaseValue.NOTE_DMG_RATIO(ENoteType.Combo));
			if (state == ENoteHitState.GOOD || state == ENoteHitState.PERPECT) {
				float cridmg = 1f + (state == ENoteHitState.PERPECT ? (0.5f + 0.5f * STAGE_USERINFO.GetStat(StatType.CriticalDmg)) : 0f);
				Damage += Mathf.RoundToInt(Damage * USERINFO.GetSkillValue(SkillKind.ComboAddDmg) * cridmg);
				Debug.Log(string.Format("HitState:{0} // Dmg:{1} // CriDmg:{2}, ", state.ToString(), Damage, cridmg));
			}
			enemy.OnHit(state, m_Mode, m_Size, Damage, rate, transform.position);
		}
		else
			PlayEffSound(SND_IDX.SFX_0920);
	}

	protected override void SetTime(float time)
	{
		m_PlayTime = time;
		SpriteRenderer img = m_TimeGauge.GetComponent<SpriteRenderer>();
		img.material.SetFloat("_FillAmount", GetTimeRate());
	}

	public override float GetRound() { return GetSize(); }
	//float size = tempNote.m_Collider.radius * tempNote.m_Collider.transform.localScale.x;
	public override float GetSize()
	{
		return m_Collider.radius * m_Collider.transform.lossyScale.x;
	}

	protected override IEnumerator Play()
	{
		m_State = NoteState.Play;
		SetTime(0f);
		m_Ani.SetTrigger("Start");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Ani));

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
		Vector3 pos = Utile_Class.GetWorldPosition(InputPos);
		bool hit = m_Collider.OverlapPoint(pos);
		if(hit)
		{
			SetHit();
		}
		return hit;
	}

	protected override float GetHitRate(out ENoteHitState state)
	{
		state = ENoteHitState.GOOD;
		if (m_State != NoteState.Play) return 0f;
		float times = 1;//0->1
		float rate = GetTimeRate();
		string trigger = "Hit";
		//if (rate <= 0.3f)
		//{
		//	state = ENoteHitState.GOOD;
		//	times = (m_MaxCnt - m_Value) * 0.5f;
		//}
		//else
		//{
		//	state = ENoteHitState.PERPECT;
		//	times = rate * m_MaxCnt;
		//}
		m_State = NoteState.End;
		m_Ani.SetTrigger(trigger);
		StopCoroutine(m_Play);
		return times;// HitDamagePer();
	}
}
