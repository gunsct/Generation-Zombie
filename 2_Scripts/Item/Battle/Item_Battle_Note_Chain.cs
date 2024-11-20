using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Battle_Note_Chain : Item_Battle_Note_Base
{
#pragma warning disable 0649
	[SerializeField] CircleCollider2D m_Collider;
	[SerializeField] TextMesh m_Number;
	[SerializeField] Material m_FontMat;
	public float m_NextCreateDelay = 0.2f;
	Item_Battle_Note_Chain m_Next;
	Item_Battle_Note_Chain m_Befor;
	int m_No;
	int m_Max;
	ENoteHitState m_Hitstate = ENoteHitState.End;
	public bool m_ISBeforNode { get { return m_Befor != null; } }
	public bool m_ISMissNode { get { return m_Hitstate == ENoteHitState.MISS; } }

#pragma warning restore 0649
	public override void SetData(EBodyType body, float endtime, params object[] valus) {
		//미리 박혀있던 폰트의 메테리얼 알파를 바꿔서 전체가 다같이 바뀌느라 깜빡이길래 인스턴스로 바꿈
		m_Number.gameObject.GetComponent<MeshRenderer>().material = new Material(m_FontMat);

		base.SetData(body, endtime, valus);
		m_Befor = m_Next = null;
		m_Mode = ENoteType.Chain;
		m_Hitstate = ENoteHitState.End;
		m_No = (int)valus[0];
		m_Max = (int)valus[1];
		m_Number.text = (m_No + 1).ToString();
	}

	public void SetBeforChain(Item_Battle_Note_Chain next_chain) {
		m_Befor = next_chain;
	}

	public void SetNextChain(Item_Battle_Note_Chain next_chain) {
		m_Next = next_chain;
	}

	protected override void SetTime(float time) {
		m_PlayTime = time;
		SpriteRenderer img = m_TimeGauge.GetComponent<SpriteRenderer>();
		img.material.SetFloat("_FillAmount", GetTimeRate());
	}

	public override float GetRound() { return GetSize(); }
	//float size = tempNote.m_Collider.radius * tempNote.m_Collider.transform.localScale.x;
	public override float GetSize() {
		return m_Collider.radius * m_Collider.transform.lossyScale.x;
	}

	protected override IEnumerator Play() {
		m_State = NoteState.Play;
		SetTime(0f);
		m_Ani.SetTrigger("Start");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Ani));

		// 노드가 플레이됨
		while (m_PlayTime < m_MaxTime) {
			yield return new WaitForFixedUpdate();
			SetTime(m_PlayTime + Time.fixedDeltaTime);
			if (m_No > 0 && m_Befor?.m_Hitstate == ENoteHitState.MISS) break;
		}
		if (m_State != NoteState.End) SetFaile();
	}

	public void SetFaile() {
		if (m_Next == this || m_Befor == this) {
			m_Next = null;
			m_Befor = null;
			m_Ani.SetTrigger("Miss");
			m_State = NoteState.End;
			m_Hitstate = ENoteHitState.MISS;
			if (m_Play != null) StopCoroutine(m_Play);
			return;
		} 
		m_Ani.SetTrigger("Miss");
		PlayEffSound(SND_IDX.SFX_0990);
		m_State = NoteState.End;
		m_Hitstate = ENoteHitState.MISS;
		if (m_Play != null) StopCoroutine(m_Play);
		if (m_Next)
		{
			m_Next.SetBeforChain(null);
			m_Next.SetFaile();
			m_Next = null;
		}

		if(m_Befor)
		{
			m_Befor.SetNextChain(null);
			m_Befor.SetFaile();
			m_Befor = null;
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
			if(m_Befor != null)
			{
				SetFaile();
			}
			else
			{
				m_Ani.SetTrigger("Hit");
				float rate = 1f / (float)m_Max;
				m_Hitstate = ENoteHitState.GOOD;
				StopCoroutine(m_Play);
				m_State = NoteState.End;
				if (m_Next)
				{
					m_Next.SetBeforChain(null);
				}
				else
				{
					rate = 1f;
					m_Hitstate = ENoteHitState.PERPECT;
				}
				Enemy_Controller enemy = BATTLE.GetEnemy();
				int Damage = BATTLEINFO.GetNoteDamage(BATTLEINFO.GetAtk(m_Mode)
					, STAGE_USERINFO.m_CharLV
					, BATTLEINFO.GetEnemyDef()
					, rate
					, m_Hitstate);
				float cridmg = 1f + (m_Hitstate == ENoteHitState.PERPECT ? (0.5f + 0.5f * STAGE_USERINFO.GetStat(StatType.CriticalDmg)) : 0f);
				Damage = Mathf.RoundToInt((Damage + Damage * m_No * 0.25f) * BaseValue.NOTE_DMG_RATIO(ENoteType.Chain) * cridmg);
				Debug.Log(string.Format("HitState:{0} // Dmg:{1} // CriDmg:{2}, ", m_Hitstate.ToString(), Damage, cridmg));
				enemy.OnHit(m_Hitstate, m_Mode, m_Size, Damage, rate, transform.position);
			}
		}
		return hit;
	}
}
