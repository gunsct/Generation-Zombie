using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 몬스터 스킬 잡기 클래스</summary>
public class Enemy_Skill_Move : Enemy_Skill
{
	public Enemy_Skill_Move(Enemy_Controller enemy) : base(enemy) { }

	protected override IEnumerator SignAction(Enemy_AtkInfo info, float delay = 0)
	{
		EnemySkillTable skill = BATTLEINFO.GetSkill(info.m_Atk);
		yield return new WaitForSeconds(delay);

		PlayEffSound(m_Enemy.GetSnd(info, true));
		GameObject Prefab = m_Enemy.GetAtkEff(info, true);
		Transform obj = Utile_Class.Instantiate(Prefab).transform;
		obj.position = info.m_EffPos;
		obj.localScale = Vector3.one * info.m_EffScale;


		IEnumerator Move = MoveAction(info, obj.gameObject);
		m_Enemy.StartCoroutine(Move);

		float time = skill.m_SignTime[0];
		Animator ani = obj.GetComponent<Animator>();
		if (ani != null) ani.speed = 1f * time;

		ParticleSystem[] allps = obj.GetComponentsInChildren<ParticleSystem>(true);
		float waittime = 0f;
		for (int i = allps.Length - 1; i > -1; i--)
		{
			ParticleSystem ps = allps[i];
			var psmain = ps.main;
			float tottime = psmain.startDelayMultiplier + psmain.startLifetimeMultiplier;
			float range = time / tottime;

			psmain.startDelayMultiplier *= range;
			psmain.startLifetimeMultiplier *= range;

			waittime = Mathf.Max(waittime, tottime * range);
		}
		yield return new WaitForSeconds(time);
		GameObject.Destroy(obj.gameObject);
		m_Enemy.StopCoroutine(Move);
		yield return new WaitForSeconds(skill.m_SignTime[1]);

	}

	IEnumerator MoveAction(Enemy_AtkInfo info, GameObject eff)
	{
		EnemySkillTable skill = BATTLEINFO.GetSkill(info.m_Atk);
		float limittime = skill.m_SignTime[0] * UTILE.Get_Random(skill.m_SkillValues[0], skill.m_SkillValues[1]);
		// 다음 이동지점
		while(limittime > 0)
		{
			float time = UTILE.Get_Random(0.1f, limittime);
			info.m_Pos = m_Enemy.GetRandomPos(info.m_Atk);
			info.m_EffPos = m_Enemy.GetRandomAtkPos(info.m_Pos);

			iTween.MoveTo(eff, iTween.Hash("position", info.m_EffPos, "time", time, "easetype", "easeOutQuad"));
			yield return new WaitForSeconds(time);
			limittime -= time;
		}
	}
}
