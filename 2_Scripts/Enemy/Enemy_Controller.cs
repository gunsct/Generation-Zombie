using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy_AtkInfo
{
	public int m_Atk;
	public EBattleDir m_Pos;
	public EBattleDir m_EffDir;
	public Vector3 m_EffPos;
	public float m_EffScale = 1f;
	public bool IsGuardCntCheck;
}

public enum EEnemyAni
{
	Default = 0,
	Start,
	Start_Skip,
	Hit,
	Hit5,
	Dead,
	Skill,
	Init,
	End
}

public enum EEnemyAniName
{
	Default = 0,
	Start,
	Start_Human,
	Hit1,
	Hit2,
	Hit3,
	Hit4,
	Hit5,
	Dead,
	Start_Skip,
	Start_Skip_Human,
	Init,
	End
}

public enum EEnemyAction
{
	Normal = 0,
	Atk,
	Skill1,
	Skill1_Return,
	Skill1_Suc,
	End
}

public class Enemy_Controller : ObjMng
{
	[System.Serializable]
	public struct Body
	{
		public EBodyType Type;
		public PolygonCollider2D Collider;
	}

	[System.Serializable]
	public class ActionEff
	{
		[SerializeField, ReName("전조", "공격")]
		public SND_IDX[] Sounds = new SND_IDX[2];
		[SerializeField, ReName("왼쪽", "중앙", "오른쪽", "전체")]
		public GameObject[] m_Sign = new GameObject[(int)EBattleDir.End];
		[SerializeField, ReName("왼쪽", "중앙", "오른쪽", "전체")]
		public GameObject[] m_Action = new GameObject[(int)EBattleDir.End];
	}

	Animator m_ActionAni;
	Animator m_StateAni;
	Animator m_LoopAni;
	public ParticleSystem m_DeadFX;
	public Transform m_AtkMovePanel;

	public List<Body> m_Body = new List<Body>();
	[Header("몬스터 공격 위치")]
	[ReName("왼쪽", "중앙", "오른쪽")]
	public BoxCollider2D[] m_AtkPos = new BoxCollider2D[3];
	[SerializeField, ReName("일반 공격", "스킬 공격")]
	ActionEff[] m_AtkEff = new ActionEff[1];
	Enemy_Skill[] m_SkillActions;
	Enemy_Skill m_PlaySkillAction;

	int m_Idx;
	int m_Lv;
	List<SND_IDX> m_HitSND = new List<SND_IDX>();
	TEnemyTable m_TData { get { return TDATA.GetEnemyTable(m_Idx); } }
	public void SetData(int _idx)
	{
		m_Idx = _idx;

		transform.localScale = Vector3.one;
		if (m_AtkMovePanel == null) m_AtkMovePanel = transform.GetChild(0);
		m_StateAni = m_AtkMovePanel.GetComponent<Animator>();
		m_StateAni.applyRootMotion = false;
		m_ActionAni = GetComponent<Animator>();
		m_LoopAni = m_StateAni.transform.GetChild(0).GetComponent<Animator>();

		List <EnemySkillTable> group = BATTLEINFO.m_EnemyTData.m_Skill.List;
		int nCtn = group.Count;
		m_SkillActions = new Enemy_Skill[nCtn];
		for (int i = 0, Offset = 0; i < nCtn; i++)
		{
			switch(group[i].m_Type)
			{
			case EAtkType.Hit:          m_SkillActions[Offset++] = new Enemy_Skill_Hit(this); break;
			case EAtkType.Catch:        m_SkillActions[Offset++] = new Enemy_Skill_Catch(this); break;
			case EAtkType.Continuous:   m_SkillActions[Offset++] = new Enemy_Skill_Continuous(this); break;
			case EAtkType.Move:         m_SkillActions[Offset++] = new Enemy_Skill_Move(this); break;
			default:                    m_SkillActions[Offset++] = new Enemy_Skill(this); break;
			}
		}
		m_HitSND.Clear();
	}

	public Vector3 GetRandomPos(EBodyType type)
	{
		PolygonCollider2D collider = m_Body.Find(data => data.Type == type).Collider;
		Vector2 pos = collider.transform.position;

		Vector2[] v2Points = collider.GetPath(Utile_Class.Get_RandomStatic(0, collider.pathCount));
		Vector2 v2min = v2Points[0], v2max = v2Points[0];
		Vector3 v3Pos = Vector3.zero;
		// 각 범위의 최소 최대 크기
		for (int i = 1; i < v2Points.Length; ++i)
		{
			if (v2Points[i].x > v2max.x) v2max.x = v2Points[i].x;
			if (v2Points[i].y > v2max.y) v2max.y = v2Points[i].y;

			if (v2Points[i].x < v2min.x) v2min.x = v2Points[i].x;
			if (v2Points[i].y < v2min.y) v2min.y = v2Points[i].y;
		}
		v2max += pos;
		v2min += pos;

		// 무한루프 해결방법
		// 방법 1.
		// 콜라이더 범위 늘리기 여맥 적게
		// 방법 2.
		// 콜라이더 상관없이 그냥 빨간범위 에서 나오게 하기
		// 방법 3.
		// y좌표의 랜덤값을 뽑은후
		// 해당 위치가 포함된 라인을 찾아서
		// 좌우의 크로스 라인 찾아 랜덤으로 뽑아서 넣기

		// 1번일때
		//while (true)
		//{
		//	Vector2 v2Temp;
		//	v2Temp.x = Utile_Class.Get_RandomStatic(v2min.x, v2max.x); 
		//	v2Temp.y = Utile_Class.Get_RandomStatic(v2min.y, v2max.y);
		//	if (collider.OverlapPoint(transform.TransformPoint(v2Temp)))
		//	{
		//		// 영역 충돌이 있으면
		//		v3Pos = new Vector3(v2Temp.x, v2Temp.y, 0f);
		//		break;
		//	}
		//}
		// 2번일때
		v3Pos = new Vector3(Utile_Class.Get_RandomStatic(v2min.x, v2max.x), Utile_Class.Get_RandomStatic(v2min.y, v2max.y), 0f);
		return v3Pos;
	}

	public Vector3 GetRandomAtkPos(EBattleDir pos)
	{
		Vector3 v3Pos = Vector3.zero;
		if (BattleMng.IsValid()) v3Pos = BaseValue.GetBattleDirPos(pos, BaseValue.BATTLE_EVA_DIS, 0);
		if ((int)pos >= m_AtkPos.Length) return v3Pos;
		BoxCollider2D collider = m_AtkPos[(int)pos];
		Vector2 v2min = collider.size * -0.5f;
		Vector2 v2max = collider.size * 0.5f;
		v3Pos = collider.transform.position;
		float PosX, PosY;
		PosX = v3Pos.x + collider.offset.x + Utile_Class.Get_RandomStatic(v2min.x, v2max.x);
		//화면 밖으로 안나가게 보정
		PosX = Mathf.Clamp(PosX, BATTLE.CAM_SIZE[0].x * 0.9f, BATTLE.CAM_SIZE[1].x * 0.9f);
		PosY = v3Pos.y + collider.offset.y + Utile_Class.Get_RandomStatic(v2min.y, v2max.y);
		return new Vector3(PosX, PosY, 0f);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Attack

	public int GetAtkType()
	{
		return BATTLEINFO.m_EnemyTData.GetAtkGroupPos();
	}

	public Enemy_AtkInfo Create_AtkInfo(int atktype)
	{
		return Create_AtkInfo(atktype, GetRandomPos(atktype));
	}

	public EBattleDir GetRandomPos(int atktype)
	{
		EBattleDir Pos = EBattleDir.All;
		// 전체 공격이 없다면
		if (m_AtkEff[atktype].m_Sign.Length <= (int)EBattleDir.All || m_AtkEff[atktype].m_Sign[(int)EBattleDir.All] == null)
			Pos = UTILE.Get_Random<EBattleDir>(EBattleDir.Left, EBattleDir.All);
		else Pos = UTILE.Get_Random<EBattleDir>(EBattleDir.Left, EBattleDir.End);
		return Pos;
	}

	public Enemy_AtkInfo Create_AtkInfo(int atktype, EBattleDir Pos)
	{
		Enemy_AtkInfo re = new Enemy_AtkInfo();
		re.m_Atk = atktype;
		re.m_EffScale = 1f;
		// 공격 데이터 셋팅
		re.m_Pos = Pos;
		// 이펙트의 방향 셋팅 (중앙의경우 없는 경우가 있어 왼쪽 오른쪽에서 랜덤으로 빼야됨)
		re.m_EffDir = GetEffDir(re.m_Pos, m_AtkEff[re.m_Atk]);
		re.m_EffPos = GetRandomAtkPos(re.m_Pos);
		return re;
	}

	public SND_IDX GetSnd(Enemy_AtkInfo info, bool _wait)
	{
		return m_AtkEff[info.m_Atk].Sounds[_wait ? 0 : 1];
	}
	/// <summary> 0:피격, 1:사망 사운드 </summary>
	public SND_IDX GetSnd() {
		if (m_HitSND.Count < 1) m_HitSND.AddRange(m_TData.m_HitVoice);
		if (m_HitSND.Count < 1) return SND_IDX.NONE;
		SND_IDX idx = m_HitSND[UTILE.Get_Random(0, m_HitSND.Count)];
		m_HitSND.Remove(idx);
		return idx;
	}
	public GameObject GetAtkEff(Enemy_AtkInfo info, bool isWait)
	{
		if(isWait) return m_AtkEff[info.m_Atk].m_Sign[(int)info.m_EffDir];
		return m_AtkEff[info.m_Atk].m_Action[(int)info.m_EffDir];
	}

	EBattleDir GetEffDir(EBattleDir dir, ActionEff eff)
	{
		EBattleDir re = dir;
		int pos = (int)dir;
		GameObject Prefab = eff.m_Sign[pos];
		if (Prefab == null)
		{
			List<EBattleDir> objlist = new List<EBattleDir>();

			if (eff.m_Sign[(int)EBattleDir.Left] != null) objlist.Add(EBattleDir.Left);
			if (eff.m_Sign[(int)EBattleDir.Right] != null) objlist.Add(EBattleDir.Right);
			if (objlist.Count > 0) re = objlist[UTILE.Get_Random(0, objlist.Count)];
		}

		return re;
	}

	public void SetAtk(Enemy_AtkInfo info, Action<Enemy_AtkInfo> HitCheckCB = null, float delay = 0f)
	{
		m_PlaySkillAction = m_SkillActions[info.m_Atk];
		StartCoroutine(m_PlaySkillAction.Action(info, delay, HitCheckCB));
	}

	public bool IS_AtkPlay()
	{
		return m_PlaySkillAction != null && m_PlaySkillAction.IS_AtkPlay();
	}

	public void OnHit(ENoteHitState hitstate, ENoteType mode, ENoteSize size, int Damage, float rate, Vector3 pos)
	{
		PlayVoice(2);
		if (hitstate == ENoteHitState.GOOD) {
			switch (mode) {
				case ENoteType.Normal: PlayEffSound(SND_IDX.SFX_0910); break;
				case ENoteType.Combo: PlayEffSound(SND_IDX.SFX_0921); break;
				case ENoteType.Chain: PlayEffSound(SND_IDX.SFX_0940); break;
				case ENoteType.Slash: PlayEffSound(SND_IDX.SFX_0900); break;
			}
		}
		else if (hitstate == ENoteHitState.PERPECT) {
			switch (mode) {
				case ENoteType.Normal: PlayEffSound(SND_IDX.SFX_0911); break;
				case ENoteType.Combo: PlayEffSound(SND_IDX.SFX_0921); break;
				case ENoteType.Chain: PlayEffSound(SND_IDX.SFX_0941); break;
				case ENoteType.Slash: PlayEffSound(SND_IDX.SFX_0900); break;
			}
			Damage += Mathf.RoundToInt(Damage * USERINFO.GetSkillValue(SkillKind.CriUp));
		}

		BATTLE.SetDamage(hitstate, mode, size, Damage, rate, pos);
		BATTLE.EnemyDamage(Damage);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Ani
	IEnumerator m_IsAniTransition;
	IEnumerator m_IsAniSound;
	bool m_ISAniStop;
	public bool ISAniPlay()
	{
		return m_IsAction != null || m_IsAniTransition != null || Utile_Class.IsAniPlay(m_StateAni);
	}

	public void StartAni(EEnemyAni ani, float _spd = 1f) {
		StartCoroutine(StartAniAction(ani, _spd));
	}
	IEnumerator StartAniAction(EEnemyAni ani, float _spd = 1f) {
		if (m_IsAniSound != null) {
			StopCoroutine(m_IsAniSound);
			m_IsAniSound = null;
		}
		if (Utile_Class.IsAniPlay(m_StateAni)) m_StateAni.enabled = false;
		EEnemyAniName name = GetAniName(ani);
		m_IsAniTransition = AniTransitionCheck();
		if (!m_StateAni.enabled) m_StateAni.enabled = true;
		m_StateAni.SetTrigger(name.ToString());

		yield return new WaitForEndOfFrame();

		m_StateAni.speed *= _spd;
		StartCoroutine(m_IsAniTransition);
		switch (name) {
			case EEnemyAniName.Start:
				m_IsAniSound = StartAniSound(248f / 265f);
				break;
			case EEnemyAniName.Start_Human:
				m_IsAniSound = StartAniSound(248f / 265f);
				break;
			case EEnemyAniName.Start_Skip:
				m_IsAniSound = StartAniSound(4f / 24f);
				break;
			case EEnemyAniName.Start_Skip_Human:
				m_IsAniSound = StartAniSound(11f / 32f);
				break;
		}
		if (m_IsAniSound != null) yield return m_IsAniSound;
		m_ISAniStop = false;
		m_StateAni.speed /= _spd;
	}

	IEnumerator StartAniSound(float _nomal) {
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_StateAni, _nomal));
		PlayVoice(0);
	}

	/// <summary> 0 : 등장, 1 : 공격, 2 : Hit </summary>
	public void PlayVoice(int Mode)
	{
		SND_IDX snd = SND_IDX.NONE;
		if (BATTLEINFO.m_EnemyTData.m_Type == EEnemyType.Zombie || BATTLEINFO.m_EnemyTData.m_Type == EEnemyType.Mutant)
		{
			switch(Mode)
			{
			case 0: snd = SND_IDX.VOC_1510; break;
			}
		}

		PlayEffSound(snd);
	}

	public void StopAni()
	{
		m_StateAni.speed = 0f;
		m_ISAniStop = true;
	}

	public void RestartAni()
	{
		if (!m_ISAniStop) return;
		m_StateAni.speed = 1f;
		m_ISAniStop = false;
	}

	EEnemyAniName GetAniName(EEnemyAni ani)
	{
		switch(ani)
		{
		case EEnemyAni.Start:
			return BATTLEINFO.m_EnemyTData.m_Tribe == EEnemyTribe.Human ? EEnemyAniName.Start_Human : EEnemyAniName.Start;
		case EEnemyAni.Start_Skip:
			return BATTLEINFO.m_EnemyTData.m_Tribe == EEnemyTribe.Human ? EEnemyAniName.Start_Skip_Human : EEnemyAniName.Start_Skip;
		case EEnemyAni.Hit:
			return UTILE.Get_Random<EEnemyAniName>(EEnemyAniName.Hit1, EEnemyAniName.Hit4 + 1);
		case EEnemyAni.Hit5:
			return EEnemyAniName.Hit5;
		case EEnemyAni.Dead:
			return EEnemyAniName.Dead;
		case EEnemyAni.Init:
			return EEnemyAniName.Init;
		}
		return EEnemyAniName.Default;
	}


	IEnumerator AniTransitionCheck()
	{
		yield return new WaitForEndOfFrame();
		m_IsAniTransition = null;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Action
	IEnumerator m_IsAction;
	public void AtkActionStart(Enemy_AtkInfo atkinfo, int mode = 0, float delay = 0f)
	{
		if (m_IsAction != null) StopCoroutine(m_IsAction);
		m_IsAction = null;
		if (atkinfo == null) AtkActionStart(EEnemyAction.Normal, delay);
		else
		{
			switch (atkinfo.m_Atk)
			{
			case 0: AtkActionStart(EEnemyAction.Atk, delay); break;
			case 1: AtkActionStart(EEnemyAction.Skill1 + mode, delay); break;
			}
		}
		if (m_IsAction != null) StartCoroutine(m_IsAction);
	}

	public void AtkActionStart(EEnemyAction action, float delay = 0f)
	{

		if (m_IsAction != null) StopCoroutine(m_IsAction);
		m_IsAction = null;
		switch (action)
		{
		case EEnemyAction.Normal:
			m_IsAction = NormalAction(delay);
			break;
		case EEnemyAction.Atk:
			m_IsAction = AtkAction(delay);
			break;
		case EEnemyAction.Skill1:
			m_IsAction = Skill1Action(delay);
			break;
		case EEnemyAction.Skill1_Return:
			m_IsAction = Skill1_ReturnAction(delay);
			break;
		case EEnemyAction.Skill1_Suc:
			m_IsAction = Skill1_SucAction(delay);
			break;
		}
		if (m_IsAction != null) StartCoroutine(m_IsAction);
	}

	protected IEnumerator NormalAction(float delay)
	{
		yield return new WaitForSeconds(delay);
		m_IsAction = null;
	}

	protected virtual IEnumerator AtkAction(float delay)
	{
		yield return ScaleAAtkAction(Vector3.one, delay);
		AtkActionStart(null);
	}

	IEnumerator ScaleAAtkAction(Vector3 nowScale, float delay)
	{
		iTween.StopByName(gameObject, "ScaleAtkAction");
		yield return new WaitForSeconds(delay);
		Vector3 v3Scale = nowScale * 1.07f;
		transform.localScale = v3Scale;

		yield return new WaitForSeconds(0.2f);
		float maxtime = 0.5f;
		iTween.ScaleTo(gameObject, iTween.Hash("scale", nowScale, "time", maxtime, "easetype", "easeOutQuad", "name", "ScaleAtkAction"));
		yield return new WaitForSeconds(maxtime);
	}

	protected virtual IEnumerator Skill1Action(float delay)
	{
		yield return new WaitForSeconds(delay);
		if(m_ActionAni == null)
		{
			Vector3 nowScale = transform.localScale;
			Vector3 v3Scale = nowScale * 1.07f;
			transform.localScale = v3Scale;
		}
		else
		{
			m_ActionAni.SetTrigger("SKILL1");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(()=> Utile_Class.IsAniPlay(m_ActionAni));
		}
		AtkActionStart(null);
	}

	protected virtual IEnumerator Skill1_SucAction(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (m_ActionAni == null)
		{
			Vector3 nowScale = transform.localScale;
			Vector3 v3Scale = nowScale * 1.07f;
			transform.localScale = v3Scale;
		}
		else
		{
			m_LoopAni.enabled = false;
			m_LoopAni.transform.localScale = m_LoopAni.transform.parent.localScale;
			m_ActionAni.SetTrigger("SKILL1_Suc");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_ActionAni));
			m_LoopAni.enabled = true;
			m_LoopAni.SetTrigger("Start");
		}
		AtkActionStart(null);
	}

	protected virtual IEnumerator Skill1_ReturnAction(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (m_ActionAni == null)
		{
			float maxtime = 0.5f;
			iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "time", maxtime, "easetype", "easeOutQuad"));
			iTween.MoveTo(gameObject, iTween.Hash("position", Vector3.zero, "time", maxtime, "easetype", "easeOutQuad"));
			yield return new WaitForSeconds(maxtime);
		}
		else
		{
			m_ActionAni.SetTrigger("SKILL1_Return");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_ActionAni));
		}
		AtkActionStart(null);
	}
}
