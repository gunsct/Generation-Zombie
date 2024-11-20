using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Battle_Note_Charge : Item_Battle_Note_Base
{
#pragma warning disable 0649
	[SerializeField] SpriteRenderer[] m_Gauge;
	[SerializeField] CircleCollider2D m_Collider;
	[SerializeField]
	Color[] m_GradeColors = new Color[4];
	[SerializeField]
	float[] m_GradeTimes = new float[3];
	[SerializeField]
	float[] m_GradeDMG = new float[3];
#pragma warning restore 0649
	float m_ChargeTime;
	int m_GuageGrade;
	int m_TouchID;
	public override void SetData(EBodyType body, float endtime, params object[] valus)
	{
		base.SetData(body, endtime, valus);
		m_TouchID = -1;
		m_Mode = ENoteType.Charge;
		m_TimeGauge.gameObject.SetActive(true);
		m_Gauge[0].color = new Color(0f, 0f, 0f, 0f);
		m_Gauge[1].color = new Color(0f, 0f, 0f, 0f);
		m_GuageGrade = -1;
		m_ChargeTime = 0;
		StopCoroutine("Shake");
		transform.localScale = Vector3.one;
		m_SizeScale.position = transform.position;
	}

	protected override void SetTime(float time)
	{
		m_PlayTime = time;
		float Scale = GetScaleToTime();
		m_TimeGauge.localScale = new Vector3(Scale, Scale, 1f);
		float Angle = (Scale - 1f) * (30f / (m_TimeStartScale - 1f));
		m_TimeGauge.eulerAngles = new Vector3(0f, 0f, Angle);
	}

	void AddChargeTime(float time)
	{
		if (m_GuageGrade < m_GradeTimes.Length)
		{
			if (m_GuageGrade < 0)
			{
				m_GuageGrade = 0;
				m_ChargeTime = 0;
				SetChargTime();
			}
			m_ChargeTime += time;
			if (m_ChargeTime >= m_GradeTimes[m_GuageGrade])
			{
				m_ChargeTime -= m_GradeTimes[m_GuageGrade];
				m_GuageGrade++;
				SetChargTime();
				PlayEffSound(SND_IDX.SFX_0935);
			}
		}
		if (m_GuageGrade < m_GradeTimes.Length)
		{
			m_Gauge[1].material.SetFloat("_FillAmount", m_ChargeTime / m_GradeTimes[m_GuageGrade]);
		}
		else
		{
			m_Gauge[1].material.SetFloat("_FillAmount", 1f);
		}
	}

	void SetChargTime()
	{
		if (!m_Gauge[0].gameObject.activeSelf) m_Gauge[0].gameObject.SetActive(true);
		if (!m_Gauge[1].gameObject.activeSelf) m_Gauge[1].gameObject.SetActive(true);
		switch(m_GuageGrade)
		{
		case 0:
			m_Gauge[0].color = new Color(0f, 0f, 0f, 0f);
			m_Gauge[1].color = m_GradeColors[0];
			break;
		default:
			m_Gauge[0].color = m_Gauge[1].color;
			m_Gauge[1].color = m_GradeColors[m_GuageGrade];
			StartCoroutine("Shake");
			break;
		}
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
		PlayEffSound(SND_IDX.SFX_0990);
	}

	protected IEnumerator Charge()
	{
		m_TimeGauge.gameObject.SetActive(false);
		m_ChargeTime = 0;
		AddChargeTime(0);
		while (true)
		{
			yield return null;
			AddChargeTime(Time.deltaTime);
			// 최대차지는 2초까지만 유지
			if (m_GuageGrade == m_GradeTimes.Length)
			{
				yield return new WaitForSeconds(1f);
				OnHit();
				break;
			}
		}
	}

	IEnumerator Shake()
	{
		if(m_GuageGrade < m_GradeTimes.Length)
		{
			transform.localScale = Vector3.one * 1.2f;
			iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "time", 0.2f, "easetype", "easeInOutSine"));
			yield return new WaitForSeconds(0.2f);
			transform.localScale = Vector3.one;
		}
		else
		{
			transform.localScale = Vector3.one * 1.2f;
			while (true)
			{
				yield return new WaitForEndOfFrame();
				m_SizeScale.localPosition = new Vector3(UTILE.Get_Random(-0.05f, 0.05f), UTILE.Get_Random(-0.05f, 0.05f), 0f);
			}
		}
	}

	public override bool OnPressed(int id, Vector2 InputPos)
	{
		if (m_State != NoteState.Play) return false;
		if (POPUP.IS_PopupUI()) return false;

		Vector3 pos = Utile_Class.GetWorldPosition(InputPos);
		bool hit = m_Collider.OverlapPoint(pos);
		if(hit)
		{
			m_TouchID = id;
			if (m_Play != null)
			{
				StopCoroutine(m_Play);
				m_Play = null;
			}
			m_Play = Charge();
			StartCoroutine(m_Play);
		}
		return hit;
	}

	public override bool OnMoved(int id, Vector2 InputPos)
	{
		if (m_TouchID != id) return false;
		Vector3 pos = Utile_Class.GetWorldPosition(InputPos);
		bool hit = m_Collider.OverlapPoint(pos);
		if (!hit) OnHit();
		return true;
	}

	public override bool OnReleased(int id, Vector2 InputPos)
	{
		if (m_TouchID != id) return false;
		OnHit();
		m_TouchID = -1;
		return true;
	}

	private void OnHit()
	{
		m_TouchID = -1;

		StopCoroutine(m_Play);
		StopCoroutine("Shake");
		transform.localScale = Vector3.one;
		m_SizeScale.position = transform.position;

		m_State = NoteState.End;
		if (m_GuageGrade < 1)
		{
			m_Ani.SetTrigger("Miss");
		}
		else
		{
			m_Ani.SetTrigger("Hit");
			float rate = m_GradeDMG[m_GuageGrade - 1];
			ENoteHitState state = m_GuageGrade < 3 ? ENoteHitState.GOOD : ENoteHitState.PERPECT;
			PlayEffSound(m_GuageGrade < 3 ? SND_IDX.SFX_0930 : SND_IDX.SFX_0931);
			Enemy_Controller enemy = BATTLE.GetEnemy();
			int Damage = BATTLEINFO.GetNoteDamage(BATTLEINFO.GetAtk(m_Mode)
				, STAGE_USERINFO.m_CharLV
				, BATTLEINFO.GetEnemyDef()
				, rate * BATTLEINFO.m_EnemyTData.m_Body[(int)m_Body].Ratio
				, state);
			float cridmg = 1f + (state == ENoteHitState.PERPECT ? (0.5f + 0.5f * STAGE_USERINFO.GetStat(StatType.CriticalDmg)) : 0f);
			Damage = Mathf.RoundToInt(Damage * BaseValue.NOTE_DMG_RATIO(ENoteType.Charge) * cridmg);
			enemy.OnHit(state, m_Mode, m_Size, Damage, rate, transform.position);
		}

	}
}
