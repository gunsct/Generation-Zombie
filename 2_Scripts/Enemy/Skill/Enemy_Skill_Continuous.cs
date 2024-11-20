using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 몬스터 스킬 잡기 클래스</summary>
public class Enemy_Skill_Continuous : Enemy_Skill
{
	public Enemy_Skill_Continuous(Enemy_Controller enemy) : base(enemy) { }
	long GuardCheckID = 0;
	public override IEnumerator Action(Enemy_AtkInfo info, float delay, Action<Enemy_AtkInfo> HitCheckCB)
	{
		GuardCheckID = -1;
		EnemySkillTable tdata = BATTLEINFO.GetSkill(info.m_Atk);
		// 같은 부위에 랜덤 생성을 해준다.
		int nCnt = (int)tdata.m_SkillValues[0];
		float time = tdata.m_SkillValues[1] / tdata.m_SkillValues[0];
		for (int i = 0; i < nCnt; i++)
		{
			Enemy_AtkInfo atkinfo = null;
			while (atkinfo == null)
			{
				atkinfo = m_Enemy.Create_AtkInfo(info.m_Atk, info.m_Pos);
				// 일단 무한루프는 막기위해 그냥 그자리에 뿌리게함
				if (info.m_Pos == EBattleDir.All) break;
				if (!CheckPos(atkinfo.m_EffPos)) atkinfo = null;
			}
			atkinfo.m_EffScale = UTILE.Get_Random(0.8f, 1.2f);
			m_Enemy.StartCoroutine(ContinuousAction(atkinfo, delay, HitCheckCB));
			yield return new WaitForSeconds(time);
		}
	}

	bool CheckPos(Vector3 pos)
	{
		for (int i = m_Atks.Count - 1; i > -1; i--)
		{
			if (Vector3.Distance(m_Atks[i].m_EffPos, pos) < 0.5f) return false;
		}
		return true;
	}

	IEnumerator ContinuousAction(Enemy_AtkInfo info, float delay, Action<Enemy_AtkInfo> HitCheckCB)
	{
		yield return base.Action(info, delay, (atkinfo) => {
			if(BATTLE.m_DefState == EPlayerState.Def && GuardCheckID != BATTLE.m_GuardCheckID)
			{
				atkinfo.IsGuardCntCheck = true;
				GuardCheckID = BATTLE.m_GuardCheckID;
			}
			else
			{
				atkinfo.IsGuardCntCheck = false;
			}
			// 첫번째 공격만 가드 카운트 해준다.
			HitCheckCB?.Invoke(atkinfo);
		});
	}

}
