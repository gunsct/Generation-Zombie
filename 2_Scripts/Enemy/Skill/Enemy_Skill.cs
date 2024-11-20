using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 몬스터 스킬 기본 인터페이스 클래스</summary>
public class Enemy_Skill : ClassMng
{
	protected Enemy_Controller m_Enemy;
	protected List<Enemy_AtkInfo> m_Atks = new List<Enemy_AtkInfo>();
	public Enemy_Skill(Enemy_Controller enemy)
	{
		m_Enemy = enemy;
	}

	public virtual IEnumerator Action(Enemy_AtkInfo info, float delay, Action<Enemy_AtkInfo> HitCheckCB)
	{
		m_Atks.Add(info);
		yield return SignAction(info, delay);

		HitCheckCB?.Invoke(info);
		yield return AtkAction(info);
		m_Atks.Remove(info);
	}



	protected virtual IEnumerator SignAction(Enemy_AtkInfo info, float delay = 0)
	{
		EnemySkillTable skill = BATTLEINFO.GetSkill(info.m_Atk);
		yield return new WaitForSeconds(delay);
		PlayEffSound(m_Enemy.GetSnd(info, true));
		GameObject Prefab = m_Enemy.GetAtkEff(info, true);
		Transform obj = Utile_Class.Instantiate(Prefab).transform;
		obj.position = info.m_EffPos;
		obj.localScale = Vector3.one * info.m_EffScale;

		float time = skill.m_SignTime[0];
		Animator ani = obj.GetComponent<Animator>();
		if (ani != null) ani.speed = 1f * time;

		ParticleSystem[] allps = obj.GetComponentsInChildren<ParticleSystem>(true);
		float waittime = time;
		for (int i = allps.Length - 1; i > -1; i--)
		{
			ParticleSystem ps = allps[i];
			var psmain = ps.main;
			float tottime = psmain.startDelayMultiplier + psmain.startLifetimeMultiplier;
			float range = 1f / time;
			psmain.simulationSpeed = range;

			waittime = Mathf.Max(waittime, tottime / range);
		}

		yield return new WaitForSeconds(time);
		GameObject.Destroy(obj.gameObject);
		yield return new WaitForSeconds(skill.m_SignTime[1]);
	}

	protected virtual IEnumerator AtkAction(Enemy_AtkInfo info)
	{
		PlayEffSound(m_Enemy.GetSnd(info, false));
		EnemySkillTable skill = BATTLEINFO.GetSkill(info.m_Atk);
		GameObject Prefab = m_Enemy.GetAtkEff(info, false);
		Transform obj = Utile_Class.Instantiate(Prefab).transform;
		obj.position = info.m_EffPos;
		obj.localScale = Vector3.one * info.m_EffScale;

		Animator ani = obj.GetComponent<Animator>();
		double checktime = UTILE.Get_Time();
		float waittime = 0f;
		ParticleSystem[] allps = obj.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = allps.Length - 1; i > -1; i--)
		{
			ParticleSystem ps = allps[i];
			var psmain = ps.main;
			float tottime = psmain.startDelayMultiplier + psmain.startLifetimeMultiplier;
			waittime = Mathf.Max(waittime, tottime);
		}
		if (ani != null)
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(ani));
		}
		yield return new WaitForSeconds(waittime);
		GameObject.Destroy(obj.gameObject);
	}


	public virtual bool IS_AtkPlay()
	{
		return m_Atks.Count > 0;
	}

}
