using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Battle_Note_Base : ObjMng
{
	protected enum NoteState
	{
		Play = 0,
		End
	}
#pragma warning disable 0649
	[SerializeField]
	protected float m_TimeStartScale = 2.5f;
	/// <summary> 시간동안 움직일 게이지용 이미지 </summary>
	[SerializeField]
	protected Transform m_TimeGauge;
	[SerializeField]
	protected Transform m_SizeScale;
	[SerializeField]
	protected Animator m_Ani;

	bool m_IsMove;
	[HideInInspector] public bool IsAction { get { return m_IsMove; } }
	[HideInInspector] public EBodyType m_Body;
	[HideInInspector] public ENoteType m_Mode;
	[HideInInspector] public int m_CreateNo;

	protected IEnumerator m_Play;
	protected NoteState m_State;
	protected ENoteSize m_Size;
#pragma warning restore 0649
	/// <summary> 플레이된 시간 </summary>
	protected float m_PlayTime;
	protected float m_MaxTime;

	private void OnEnable()
	{
		m_Play = Play();
		StartCoroutine(m_Play);
	}

	public void SetNoteNo(int No)
	{
		m_CreateNo = No;
	}

	public virtual void SetSize(ENoteSize size)
	{
		m_Size = size;
		float scale = BaseValue.NOTE_SCALE[(int)size];
		m_SizeScale.localScale = new Vector3(scale, scale, 1f);
	}

	public virtual void SetData(EBodyType body, float endtime, params object[] valus)
	{
		m_Body = body;
		m_MaxTime = endtime;
	}

	protected virtual void SetTime(float time) { }

	public virtual float GetRound() { return 0f; }
	//float size = tempNote.m_Collider.radius * tempNote.m_Collider.transform.localScale.x;

	public virtual float GetSize() { return 0f; }

	protected virtual float GetScaleToTime()
	{
		return m_TimeStartScale * GetTimeRate();
	}

	protected virtual float GetTimeRate()
	{
		return (m_MaxTime - m_PlayTime) / m_MaxTime;
	}

	protected virtual IEnumerator Play() { yield return new WaitForEndOfFrame(); }

	public void NoteEnd()
	{
		m_Play = null;
		BATTLE.RemoveNote(this);
	}

	public virtual void OnClear()
	{
		if(m_State != NoteState.End)
		{
			if (m_Play != null) StopCoroutine(m_Play);
			m_State = NoteState.End;
			m_Ani.SetTrigger("Hit");
		}
	}

	public virtual bool OnPressed(int id, Vector2 InputPos) { return false; }

	public virtual bool OnMoved(int id, Vector2 InputPos) { return false; }
	public virtual bool OnReleased(int id, Vector2 InputPos) { return false; }
	public virtual bool OnSliced(Vector2 start, Vector2 end) { return false; }

	protected virtual float GetHitRate(out ENoteHitState state) {
		state = ENoteHitState.MISS;
		return 0f;

	}
	public bool IS_Clear() {
		return m_State == NoteState.End;
	}
}
