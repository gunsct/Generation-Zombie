using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PVPMng : ObjMng
{
	int m_Turn = 0;//10턴마다 1라운드임
	public int GetTurn { get { return BaseValue.PVP_TURN_LIMIT - m_Turn; } }
	public void SetTurn(int _add) {
		m_Turn += _add;
		m_MainUI?.SetTurn(BaseValue.PVP_TURN_LIMIT - m_Turn);
	}
	/// <summary>
	/// 제한턴이 종료될 때까지 승패가 결정되지 않으면 방어측 승리
	/// 전투 종료 이전 클라이언트 종료 시, 패배
	/// 한팀의 어태커 5명의 HP가 모두 0 이하면 패배
	/// 한팀의 생존 스탯 - 포만, 청결, 정신 중 하나라도 0 이하면 패배
	/// 동일 턴에 양쪽 팀이 모두 패배조건을 만족한 경우, 방어측 승리
	/// </summary>
	void CheckResult() {
		m_Result = Result.None;

		if (m_Turn >= BaseValue.PVP_TURN_LIMIT) {
			string actionlog = "ResultCheck // Lose_TurnLimit";
			Log(actionlog);
			m_Result = Result.Turn;
			m_FailCause = PVPFailCause.Turn;
		}
		else {
			//혹시모를 개별 감소 체력 모아 전체 체력 감소시키는데 미세한 값 차이로 안끝날 경우 대비 예외처리
			for (int i = 0; i < 2; i++) {
				bool alldie = true;
				for (int j = 0; j < 5; j++) {
					if (m_PlayUser[i].m_Chars[j].m_State == Item_PVP_Char.State.Live) {
						alldie = false;
						break;
					}
				}
				if (alldie) {
					for (StatType k = StatType.Men; k <= StatType.SurvEnd; k++) {
						int mystat = Mathf.RoundToInt(m_PlayUser[(int)UserPos.My].GetStat(k));
						int targetstat = Mathf.RoundToInt(m_PlayUser[(int)UserPos.Target].GetStat(k));
						string actionlog = string.Format("ResultCheck // 스탯 : {0} // My : {1}, Target : {2}", k.ToString(), mystat, targetstat);
						Log(actionlog);
						if (mystat > 0 && targetstat > 0) continue;
						switch (k) {
							case StatType.Men: m_FailCause = PVPFailCause.Men; break;
							case StatType.Sat: m_FailCause = PVPFailCause.Sat; break;
							case StatType.Hyg: m_FailCause = PVPFailCause.Hyg; break;
							case StatType.HP: m_FailCause = PVPFailCause.HP; break;
						}
					}
				}
				if (alldie == true && i == 1) {
					m_Result = Result.WIN;
					string actionlog = "ResultCheck // WIN";
					Log(actionlog);
					return;
				}
				else if (alldie == true && i == 0) {
					m_Result = Result.SurvStat;
					List<SND_IDX> snds = new List<SND_IDX>() { SND_IDX.SFX_1320, SND_IDX.SFX_1321, SND_IDX.SFX_1322 };
					PlayEffSound(snds[UTILE.Get_Random(0, 3)]);
					string actionlog = "ResultCheck // Lose_SurvStat";
					Log(actionlog);
					return;
				}
			}

			for (StatType i = StatType.Men; i <= StatType.SurvEnd; i++) {
				if (m_Result != Result.None) break;
				int mystat = Mathf.RoundToInt(m_PlayUser[(int)UserPos.My].GetStat(i));
				int targetstat = Mathf.RoundToInt(m_PlayUser[(int)UserPos.Target].GetStat(i));
				string actionlog = string.Format("ResultCheck // 스탯 : {0} // My : {1}, Target : {2}", i.ToString(), mystat, targetstat);
				Log(actionlog);
				if (mystat > 0 && targetstat > 0) continue;
				switch (i) {
					case StatType.Men: m_FailCause = PVPFailCause.Men; break;
					case StatType.Sat: m_FailCause = PVPFailCause.Sat; break;
					case StatType.Hyg: m_FailCause = PVPFailCause.Hyg; break;
					case StatType.HP: m_FailCause = PVPFailCause.HP; break;
				}
				if (mystat > 0 && targetstat <= 0) {
					m_Result = Result.WIN;
					actionlog = "ResultCheck // WIN";
					Log(actionlog);
				}
				else {
					m_Result = Result.SurvStat;
					List<SND_IDX> snds = new List<SND_IDX>() { SND_IDX.SFX_1320, SND_IDX.SFX_1321, SND_IDX.SFX_1322 };
					PlayEffSound(snds[UTILE.Get_Random(0, 3)]);
					actionlog = "ResultCheck // Lose_SurvStat";
					Log(actionlog);
				}
			}
		}
	}
	/// <summary> 자연회복 - 행동력회복 - 공격 순으로 결과나오기 전까지 루프 </summary>
	IEnumerator TurnAtion() {
		bool consume = false;
		//자연회복
		AllAtkerRecvHP();
		//행동력 회복
		AllCharRecvAP();
		//계산해서 공격 차례 계산
		Item_PVP_Char atker = TurnAPCheck();
		if (atker.GetAP() >= 1f) {
			consume = true;
			float[] prehp = new float[2] { GetNowStat(UserPos.My, StatType.HP), GetNowStat(UserPos.Target, StatType.HP) };
			float[] presurvstat = new float[4];
			for (int i = 0; i < 4; i++) {
				presurvstat[i] = GetNowStat(atker.m_UserPos, (StatType)i);
			}
			float[] preap = new float[10];
			for(int i = 0; i < 10; i++) {
				preap[i] = m_PlayUser[(int)atker.m_UserPos].m_Chars[i].GetAP();
			}
			List<Item_PVP_Char> targets = GetTarget(atker);
			yield return AtkHit(atker, targets);

			//
			if(atker.m_PosType == PVPPosType.Supporter && atker.m_PSTData.m_TurnType == PVPTurnType.Team) {
				switch (atker.m_PSTData.m_Type) {
					case PVPSkillType.LowStatus:
					case PVPSkillType.Status:
					case PVPSkillType.TwoStat:
					case PVPSkillType.Ap:
						consume = false;
						for (int i = 0;i< presurvstat.Length; i++) {
							if (presurvstat[i] != GetNowStat(atker.m_UserPos, (StatType)i)) {
								consume = true;
								break;
							}
						}
						for (int i = 0; i < 10; i++) {
							if(i != atker.m_Pos && preap[i] != m_PlayUser[(int)atker.m_UserPos].m_Chars[i].GetAP()) {
								consume = true;
								break;
							}
						}
						break;
				}
			}
			if (atker.m_PosType == PVPPosType.Combat && targets.Count < 1) consume = false;

			//턴감소
			SetTurn(1);
			//턴 끝나고 HPTail
			m_MainUI.SetHPTail(UserPos.My, prehp[0], GetNowStat(UserPos.My, StatType.HP), GetMaxStat(UserPos.My, StatType.HP));
			m_MainUI.SetHPTail(UserPos.Target, prehp[1], GetNowStat(UserPos.Target, StatType.HP), GetMaxStat(UserPos.Target, StatType.HP));
		}

		CheckResult();
		if (m_Result == Result.None) {
			if (consume) {
				var consumestat = atker.GetConsumeStat(true);
				int val = AddStat(atker.m_UserPos, consumestat.Key, -consumestat.Value, true, false);
				//SetDmgStr(m_SUI.Stats[(int)consumestat.Key].transform.position, PVPDmgType.Normal, -consumestat.Value, false);
				Log(string.Format("공격 // 공격진영 : {0} // pos : {1} // charidx : {2} // 포지션 : {3} // 소모스탯 : {4} // 소모량 : {5} // 잔여량 : {6}",
					atker.m_UserPos.ToString(), atker.m_Pos, atker.m_Info.Idx, atker.m_PosType.ToString(), consumestat.Key.ToString(), val, GetNowStat(atker.m_UserPos, consumestat.Key)));
			}
			CheckResult();

			yield return new WaitForSeconds(0.4f);
			StartCoroutine(m_Action = TurnAtion());
		}
		else {
			ActionEnd();
			yield return new WaitForSeconds(2f);
			StateChange(State.Result, m_Result);
		}
	}
	/// <summary> ap 최고, 최하 캐릭터 체크 </summary>
	Item_PVP_Char TurnAPCheck() {
		List<Item_PVP_Char> allchars = new List<Item_PVP_Char>();
		for(int i = 0; i < 10; i++) {
			for(int j = 0; j < 2; j++) {
				if (m_PlayUser[j].m_Chars[i].m_State == Item_PVP_Char.State.Die) continue;
				allchars.Add(m_PlayUser[j].m_Chars[i]);
			}
		}
		allchars.Sort((Item_PVP_Char _before, Item_PVP_Char _after) => {
			return _after.GetAP().CompareTo(_before.GetAP());
		});

		return allchars.Count > 0 ? allchars[0] : null;
	}
	IEnumerator AtkHit(Item_PVP_Char _atker, List<Item_PVP_Char> _targets) {
		Dictionary<Item_PVP_Char, Item_PVP_Char.PVPDmgStr> chardmg = new Dictionary<Item_PVP_Char, Item_PVP_Char.PVPDmgStr>();
		List<Dictionary<StatType, int>> Dmgs = new List<Dictionary<StatType, int>>();
		Dictionary<StatType, float> dmgsum = new Dictionary<StatType, float>();

		//버프 체크
		_atker.CheckBuff(true);
		for (int i = 0; i < _targets.Count; i++) {
			_targets[i].CheckBuff(false);
		}


		//행동시 소모되는 스탯
		_atker.SetAP(-_atker.GetAP());

		if (_atker.m_PosType == PVPPosType.Combat) {
			//타격시 감소하는 스탯
			for (int i = 0; i < _targets.Count; i++) {
				if (_targets[i].m_State == Item_PVP_Char.State.Die) continue;
				bool hit = _atker.IS_Hit();

				_targets[i].Is_Hit = hit;
				chardmg.Add(_targets[i], hit ? _atker.GetDmg(_targets[i]) : new Item_PVP_Char.PVPDmgStr());

				int hpdmg = hit ? chardmg[_targets[i]].Dmg : 0;
				int hygdmg = hit ? _atker.GetConsumeStat(false).Value : 0;
				Log(string.Format("적용HP데미지 : {0} // 적용HYG데미지 : {1} // 회피하면 0, 수치가 적용시 타겟 잔여량을 맥스로함 ", hpdmg, hygdmg));
				Dmgs.Add(new Dictionary<StatType, int>());
				Dmgs[i].Add(StatType.HP, hpdmg);
				Dmgs[i].Add(StatType.Hyg, hygdmg);
			}
		}

		if (_atker.m_PosType == PVPPosType.Supporter) {
			m_MainUI.SupSkillFX(_atker.m_UserPos, _atker.m_Info);
			yield return new WaitForSeconds(1f);
		}

		yield return _atker.Atk(_targets, Dmgs, (pos, diffhp) => {
			if (_atker.m_PosType == PVPPosType.Combat) {
				if (_targets[pos].m_State == Item_PVP_Char.State.Die) m_PlayUser[(int)_atker.m_UserPos].m_KillCnt++;

				AddStat(_targets[pos].m_UserPos, StatType.HP, diffhp, false, true, false);
				SetDmgStr(_targets[pos].transform.position, chardmg[_targets[pos]].Type, diffhp, true, Vector3.zero, chardmg[_targets[pos]].Icon);
				if (!dmgsum.ContainsKey(StatType.HP)) dmgsum.Add(StatType.HP, 0);
				dmgsum[StatType.HP] += diffhp;

				int hygdmg = AddStat(_targets[pos].m_UserPos, StatType.Hyg, -Dmgs[pos][StatType.Hyg], false, true, false);
				SetDmgStr(m_SUI.Stats[(int)_targets[pos].m_UserPos * 3 + (int)StatType.Hyg].transform.position, PVPDmgType.Normal, hygdmg, false, new Vector3(0f, _targets[pos].m_UserPos == UserPos.My ? 0.5f : -0.5f));
				if (!dmgsum.ContainsKey(StatType.Hyg)) dmgsum.Add(StatType.Hyg, 0);
				dmgsum[StatType.Hyg] += hygdmg;

				Log(string.Format("피격 //  피격진영 : {0} // 감소HP : {1} // 잔여HP : {2} // 감소HYG : {3} // 잔여HYG : {4}",
					_targets[pos].m_UserPos.ToString(), diffhp, GetNowStat(_targets[pos].m_UserPos, StatType.HP), hygdmg, GetNowStat(_targets[pos].m_UserPos, StatType.Hyg)));
			}
			else {
				SupportSkill(_atker, pos == -1 ? null : _targets[pos]);
			}
		});
		if (_atker.m_UserPos == UserPos.Target) {
			for (int i = 0; i < dmgsum.Count; i++) {
				var dmg = dmgsum.ElementAt(i);
				SetBuffAlarm(dmg.Key, Mathf.RoundToInt(dmg.Value));
			}
		}
	}
	/// <summary> 스탯 변경 </summary>
	int AddStat(UserPos _pos, StatType _stat, int _val, bool _use = false, bool _snd = true, bool _alarm = true) {
		if (_val == 0) return 0;

		if (_val < 0f) {
			switch (_stat) {
				case StatType.Men:
					_val = Mathf.RoundToInt(_val * Mathf.Clamp(1f - m_PlayUser[(int)_pos].m_Stat[(int)StatType.MenDecreaseDef, 0] * 0.1f, 0f, 1f)); 
					break;
				case StatType.Sat:
					_val = Mathf.RoundToInt(_val * Mathf.Clamp(1f - m_PlayUser[(int)_pos].m_Stat[(int)StatType.SatDecreaseDef, 0] * 0.1f, 0f, 1f));
					break;
				case StatType.Hyg:
					_val = Mathf.RoundToInt(_val * Mathf.Clamp(1f - m_PlayUser[(int)_pos].m_Stat[(int)StatType.HygDecreaseDef, 0] * 0.1f, 0f, 1f));
					break;
			}
		}
		float preval = m_PlayUser[(int)_pos].m_Stat[(int)_stat, 0];
		m_PlayUser[(int)_pos].m_Stat[(int)_stat, 0] = Mathf.Clamp(m_PlayUser[(int)_pos].m_Stat[(int)_stat, 0] + _val ,0f, m_PlayUser[(int)_pos].m_Stat[(int)_stat, 1]);

		if (_stat <= StatType.SurvEnd)
			m_MainUI.SetSurvStat(_pos, _stat, preval, m_PlayUser[(int)_pos].m_Stat[(int)_stat, 0], m_PlayUser[(int)_pos].m_Stat[(int)_stat, 1], _use);

		if(_pos == UserPos.My && _alarm) PVP.SetBuffAlarm(_stat, _val);

		//여기서 전체 스텟 유아이도 수정
		//사운드
		if (_val > 0f) {
			switch (_stat) {
				case StatType.Men: PlayEffSound(SND_IDX.SFX_0451);break;
				case StatType.Hyg: PlayEffSound(SND_IDX.SFX_0453); break;
				case StatType.Sat: PlayEffSound(SND_IDX.SFX_0452); break;
				case StatType.HP: PlayEffSound(SND_IDX.SFX_0450); break;
			}
		}
		else if(_val < 0f) {
			if (_pos == UserPos.My) {
				if(_snd) PlayEffSound(SND_IDX.SFX_1340);
				if (preval / m_PlayUser[(int)_pos].m_Stat[(int)_stat, 1] >= 0.3f && m_PlayUser[(int)_pos].m_Stat[(int)_stat, 0] / m_PlayUser[(int)_pos].m_Stat[(int)_stat, 1] < 0.3f) {
					switch (_stat) {
						case StatType.Men: PlayEffSound(SND_IDX.SFX_0494); break;
						case StatType.Hyg: PlayEffSound(SND_IDX.SFX_0493); break;
						case StatType.Sat: PlayEffSound(SND_IDX.SFX_0492); break;
					}
				}
			}
		}
		if (_pos == UserPos.Target && m_PlayUser[(int)_pos].m_Stat[(int)_stat, 0] <= 0f) PlayEffSound(SND_IDX.SFX_1350);

		return _val;
	}
	/// <summary> 버프 추가 </summary>
	void SupportSkill(Item_PVP_Char _atker, Item_PVP_Char _target) {
		TPVPSkillTable tdata = _atker.m_PSTData;
		UserPos targetpos = tdata.m_TurnType == PVPTurnType.Team ? _atker.m_UserPos : _atker.GetOtherPos;
		float add = 0f;
		PVPDmgType dmgtype = tdata.m_TurnType == PVPTurnType.Team ? PVPDmgType.Heal: PVPDmgType.Normal;

		//대상은 이미 정해져있으니 
		int dmg = 0;
		switch (tdata.m_Type) {
			//생존스탯 즉각 적용
			case PVPSkillType.Status:
				PlayEffSound(SND_IDX.SFX_0450);
				if (_target != null) {
					if ((StatType)(int)tdata.m_Vals[0] == StatType.HP) {
						add = GetStatDmg((int)targetpos, (int)_atker.m_UserPos, StatType.HP, _target.GetStat(StatType.HP, 1), tdata.m_Vals[2]);
						add = _target.SetHP(add); 
						dmg = AddStat(targetpos, StatType.HP, Mathf.RoundToInt(add));
						SetDmgStr(_target.transform.position, dmgtype, dmg, true);
					}
					else {
						add = GetStatDmg((int)targetpos, (int)_atker.m_UserPos, (StatType)(int)tdata.m_Vals[0], _target.GetStat((StatType)(int)tdata.m_Vals[0], 1), tdata.m_Vals[2]);
						dmg = AddStat(targetpos, (StatType)(int)tdata.m_Vals[0], Mathf.RoundToInt(add));
						SetDmgStr(m_SUI.Stats[(int)targetpos * 3 + (int)tdata.m_Vals[0]].transform.position, dmgtype, dmg, false, new Vector3(0f, targetpos == UserPos.My ? 0.5f : -0.5f));
					}
				}
				else {
					add = GetStatDmg((int)targetpos, (int)_atker.m_UserPos, (StatType)(int)tdata.m_Vals[0], m_PlayUser[(int)targetpos].m_Stat[(int)tdata.m_Vals[0], 1], tdata.m_Vals[2]);
					dmg = AddStat(targetpos, (StatType)(int)tdata.m_Vals[0], Mathf.RoundToInt(add));
					SetDmgStr(m_SUI.Stats[(int)targetpos * 3 + (int)tdata.m_Vals[0]].transform.position, dmgtype, dmg, false, new Vector3(0f, targetpos == UserPos.My ? 0.5f : -0.5f));
				}
				Log(string.Format("서폿스킬 Status // 공격진영 : {0} // pos : {1} // charidx : {2} // 스탯 : {3} // 변화량 : {4} // 피격진영잔여량 : {5}", 
					_atker.m_UserPos.ToString(), _atker.m_Pos, _atker.m_Info.Idx, ((StatType)Mathf.RoundToInt(tdata.m_Vals[0])).ToString(), dmg, m_PlayUser[(int)targetpos].m_Stat[(int)tdata.m_Vals[0], 0]));
				break;
			case PVPSkillType.TwoStat:
				PlayEffSound(SND_IDX.SFX_0450);
				add = GetStatDmg((int)targetpos, (int)_atker.m_UserPos, (StatType)(int)tdata.m_Vals[0], m_PlayUser[(int)targetpos].m_Stat[(int)tdata.m_Vals[0], 1], tdata.m_Vals[2]);
				dmg = AddStat(targetpos, (StatType)(int)tdata.m_Vals[0], Mathf.RoundToInt(add));
				SetDmgStr(m_SUI.Stats[(int)targetpos * 3 + (int)tdata.m_Vals[0]].transform.position, dmgtype, dmg, false, new Vector3(0f, targetpos == UserPos.My ? 0.5f : -0.5f));
				Log(string.Format("서폿스킬 TwoStat // 공격진영 : {0} // pos : {1} // charidx : {2} // 스탯 : {3} // 변화량 : {4} // 피격진영잔여량 : {5}",
					_atker.m_UserPos.ToString(), _atker.m_Pos, _atker.m_Info.Idx, ((StatType)Mathf.RoundToInt(tdata.m_Vals[0])).ToString(), dmg, m_PlayUser[(int)targetpos].m_Stat[(int)tdata.m_Vals[0], 0]));

				add = GetStatDmg((int)targetpos, (int)_atker.m_UserPos, (StatType)(int)tdata.m_Vals[1], m_PlayUser[(int)targetpos].m_Stat[(int)tdata.m_Vals[1], 1], tdata.m_Vals[2]);
				dmg = AddStat(targetpos, (StatType)(int)tdata.m_Vals[1], Mathf.RoundToInt(add));
				SetDmgStr(m_SUI.Stats[(int)targetpos * 3 + (int)tdata.m_Vals[1]].transform.position, dmgtype, dmg, false, new Vector3(0f, targetpos == UserPos.My ? 0.5f : -0.5f));
				Log(string.Format("서폿스킬 TwoStat // 공격진영 : {0} // pos : {1} // charidx : {2} // 스탯 : {3} // 변화량 : {4} // 피격진영잔여량 : {5}",
					_atker.m_UserPos.ToString(), _atker.m_Pos, _atker.m_Info.Idx, ((StatType)Mathf.RoundToInt(tdata.m_Vals[0])).ToString(), dmg, m_PlayUser[(int)targetpos].m_Stat[(int)tdata.m_Vals[1], 0]));
				break;
			case PVPSkillType.LowStatus:
				PlayEffSound(SND_IDX.SFX_0450);
				float val = 0f;
				StatType lowstat = StatType.Men;
				for(StatType i = StatType.Men;i < StatType.SurvEnd; i++) {
					float now = m_PlayUser[(int)targetpos].m_Stat[(int)i, 0];
					if (i == StatType.Men || val >= now) {
						lowstat = i;
						val = now;
					}
				}
				add = GetStatDmg((int)targetpos, (int)_atker.m_UserPos, (StatType)(int)lowstat, m_PlayUser[(int)targetpos].m_Stat[(int)lowstat, 1], tdata.m_Vals[2]);
				dmg = AddStat(targetpos, lowstat, Mathf.RoundToInt(add));
				SetDmgStr(m_SUI.Stats[(int)targetpos * 3 + (int)lowstat].transform.position, dmgtype, dmg, false, new Vector3(0f, targetpos == UserPos.My ? 0.5f : -0.5f));
				Log(string.Format("서폿스킬 LowStatus // 공격진영 : {0} // pos : {1} // charidx : {2} // 스탯 : {3} // 변화량 : {4} // 피격진영잔여량 : {5}",
					_atker.m_UserPos.ToString(), _atker.m_Pos, _atker.m_Info.Idx, lowstat.ToString(), dmg, m_PlayUser[(int)targetpos].m_Stat[(int)lowstat, 0]));
				break;
			//캐릭터 개별 버프 적용
			case PVPSkillType.StatusBuff:
				PlayEffSound(SND_IDX.SFX_0470);
				_target.SetBuff(tdata, _target.m_UserPos == UserPos.My);//_atker.m_UserPos == UserPos.My
				Log(string.Format("서폿스킬 StatusBuff // 공격진영 : {0} // atkpos : {1} // atkcharidx : {2} // hitpos : {3} // hitcharidx : {4} // 스탯 : {5} // 변화량 : {6} // 누적버프량 : {7}",
					_atker.m_UserPos.ToString(), _atker.m_Pos, _atker.m_Info.Idx, _target.m_Pos, _target.m_Info.Idx, 
					((StatType)Mathf.RoundToInt(tdata.m_Vals[0])).ToString(), tdata.m_Vals[2], _target.GetBuffStat((StatType)Mathf.RoundToInt(tdata.m_Vals[0])))); 
				break;
			//캐릭터 개별 ap 적용
			case PVPSkillType.Ap:
				PlayEffSound(SND_IDX.SFX_0450);
				_target.SetAP(tdata.m_Vals[2]);
				break;
		}
	}
	float GetStatDmg(int _targetpos, int _ourpos, StatType _stat, float _val, float _ratio) {
		float def = 0f;
		float supup = 0f;
		switch (_stat) {
			case StatType.Men:
				def = m_PlayUser[_targetpos].GetStat(StatType.MenDecreaseDef);
				supup = m_PlayUser[_ourpos].GetResearch(ResearchEff.PVPPerSupMenUp);
				break;
			case StatType.Hyg:
				def = m_PlayUser[_targetpos].GetStat(StatType.HygDecreaseDef);
				supup = m_PlayUser[_ourpos].GetResearch(ResearchEff.PVPPerSupHygUp);
				break;
			case StatType.Sat:
				def = m_PlayUser[_targetpos].GetStat(StatType.SatDecreaseDef);
				supup = m_PlayUser[_ourpos].GetResearch(ResearchEff.PVPPerSupSatUp);
				break;
		}
		
		return _val * _ratio * Math.Clamp(1f - def, 0f, 1f) * (1f + supup);
	}
	/// <summary> 모든 캐릭터들 액션 포인트 회복 </summary>
	void AllCharRecvAP() {
		List<Item_PVP_Char> allchars = new List<Item_PVP_Char>();
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 2; j++) {
				if (m_PlayUser[j].m_Chars[i].m_State == Item_PVP_Char.State.Die) continue;
				allchars.Add(m_PlayUser[j].m_Chars[i]);
			}
		}
		allchars.Sort((Item_PVP_Char _before, Item_PVP_Char _after) => {
			return _before.GetPivotTime().CompareTo(_after.GetPivotTime());
		});
		for(int i = 0;i< allchars.Count; i++) {
			Log(string.Format("ap : {0} // 1 - ap : {1} // spd : {2} // pivittime : {3}", allchars[i].GetAP(), 1f - allchars[i].GetAP(), allchars[i].GetStat(StatType.Speed), allchars[i].GetPivotTime()));
		}
		if (allchars.Count < 1) return;

		float pivotaddtime = allchars[0].GetPivotTime();
		for (int i = 0; i < 2; i++) {
			for(int j = 0; j < m_PlayUser[i].m_Chars.Length; j++) {
				float add = m_PlayUser[i].m_Chars[j].GetStat(StatType.Speed) * pivotaddtime * (1f - m_PlayUser[i].m_Chars[j].m_PSTData.m_SpeedCorrection);
				m_PlayUser[i].m_Chars[j].SetAP(add);
				Log(string.Format("AP회복 // 회복진영 : {0} // pos : {1} // charidx : {2} //  기준회복시간 : {3} // 속도 : {4} // 회복량 : {5} // 현재잔여량 : {6}",
					m_PlayUser[i].m_Chars[j].m_UserPos.ToString(), m_PlayUser[i].m_Chars[j].m_Pos, m_PlayUser[i].m_Chars[j].m_Info.Idx, pivotaddtime, m_PlayUser[i].m_Chars[j].GetStat(StatType.Speed), add, m_PlayUser[i].m_Chars[j].GetAP()));
			}
		}
	}
	/// <summary> 턴 종료 후 자동 회복 체크 </summary>
	void AllAtkerRecvHP() {
		if (m_Turn > 0 && m_Turn % BaseValue.PVP_HEAL_TURN != 0) return;
		for (int i = 0; i < 2; i++) {
			float addhp = 0;
			for (int j = 0; j < 5; j++) {
				Item_PVP_Char atker = m_PlayUser[i].m_Chars[j];
				if (atker.m_State == Item_PVP_Char.State.Die) continue;
				//[어태커]현재 회복력 / ( Power 배율^([어태커]현재 레벨 / Power 갭차) * Power 기준 레벨 * 2 * PVP 회복상수 + [어태커]현재 회복력 )
				float per = BaseValue.GetPVPHeal(atker.GetStat(StatType.Heal), atker.m_Info.LV);
				float add = Mathf.Min(atker.GetStat(StatType.HP, 0), per * atker.GetStat(StatType.HP, 1) * BaseValue.PVP_HEAL_TURN);
				addhp += atker.SetHP(Mathf.RoundToInt(add));
				Log(string.Format("HP회복 // 회복진영 : {0} // pos : {1} // charidx : {2} // 레벨 : {3} // 등급 : {4} // 힐 : {5} // 최대체력 : {6} // 회복량 : {7}",
					atker.m_UserPos.ToString(), atker.m_Pos, atker.m_Info.Idx, atker.m_Info.LV, atker.m_Info.Grade, atker.GetStat(StatType.Heal), atker.GetStat(StatType.HP, 1), Mathf.RoundToInt(add)));
			}
			AddStat((UserPos)i, StatType.HP, Mathf.RoundToInt(addhp), false, true, true);
		}
	}
	/// <summary>
	/// skilltable에 weaponfx, atktargettype로 범위 지정
	/// 랜덤으로 N명의 적을 공격
	/// 전투력이 가장 강한 적 1명 or 가장 강한 적을 중심으로 범위 공격
	/// 모든 적을 공격
	/// 현재 HP가 가장 낮은 적 1명 or 가장 낮은 적을 중심으로 범위 공격
	/// 공격하려는 아군과 가장 가까운 위치에 있는 적 공격 or 가장 가까운 위치에 있는 적을 중심으로 범위 공격
	/// </summary>
	public List<Item_PVP_Char> GetTarget(Item_PVP_Char _atker) {
		List<Item_PVP_Char> m_Targets = new List<Item_PVP_Char>();
		//어태커는 상대방 어태커만 
		//서포터는 아군, 적군, 어태커, 서포터 전체
		Item_PVP_Char[] enemydeck = PVP.m_PlayUser[(int)_atker.GetOtherPos].m_Chars;
		List<Item_PVP_Char> enemy = enemydeck.ToList();
		Item_PVP_Char[] teamdeck = PVP.m_PlayUser[(int)_atker.m_UserPos].m_Chars;
		List<Item_PVP_Char> team = teamdeck.ToList();

		//val[0] 어태커는 데미지 배율 서포터는 스탯인덱스 val[1]이 타겟 수 val[2] 서포터는 비율
		int cnt = Mathf.RoundToInt(_atker.m_PSTData.m_Vals[1]);
		List<Item_PVP_Char> cantargets = new List<Item_PVP_Char>();

		//서폿 스킬에서 speed, ap는 어태커 서포터 전부 아니면 어태커만 
		bool is_spdap = false;
		switch (_atker.m_PSTData.m_Type) {
			case PVPSkillType.Ap: is_spdap = true; break;
			case PVPSkillType.StatusBuff: is_spdap = ((StatType)Mathf.RoundToInt(_atker.m_PSTData.m_Vals[0])) == StatType.Speed; break;
			default: is_spdap = false; break;
		}

		switch (_atker.m_PSTData.m_AtkTartgetType) {
			//살아있는 무작위 표적
			case PVPAtkTargetType.Random:
				if (_atker.m_PosType == PVPPosType.Combat) {
					cantargets = enemy.FindAll(o => o.m_Pos < 5 && o.m_State == Item_PVP_Char.State.Live);
					cnt = Mathf.Min(cnt, cantargets.Count);
				}
				else {
					switch (_atker.m_PSTData.m_TurnType) {
						case PVPTurnType.Team:
							cantargets = team.FindAll(o => (!is_spdap ? o.m_Pos < 5 : o.m_Pos < 10) && o.m_State == Item_PVP_Char.State.Live);
							cnt = Mathf.Min(cnt, cantargets.Count);
							break;
						case PVPTurnType.Enemy:
							cantargets = enemy.FindAll(o => (!is_spdap ? o.m_Pos < 5 : o.m_Pos < 10) && o.m_State == Item_PVP_Char.State.Live);
							cnt = Mathf.Min(cnt, cantargets.Count);
							break;
					}
				}
				for (int i = 0; i < cnt; i++) {
					Item_PVP_Char target = cantargets[UTILE.Get_Random(0, cantargets.Count)];
					m_Targets.Add(target);
					cantargets.Remove(target);
				}
				break;
			//살아있는 가장 전투력이 높은 적 1명을 공격합니다.
			case PVPAtkTargetType.Stronger:
				if (_atker.m_PosType == PVPPosType.Combat) {
					cantargets = enemy.FindAll(o => o.m_Pos < 5 && o.m_State == Item_PVP_Char.State.Live);
					cantargets.Sort((Item_PVP_Char _before, Item_PVP_Char _after) => {
						return _after.GetCombatPower().CompareTo(_before.GetCombatPower());
					});
					cnt = Mathf.Min(cnt, cantargets.Count);
				}
				else {
					switch (_atker.m_PSTData.m_TurnType) {
						case PVPTurnType.Team:
							cantargets = team.FindAll(o => (!is_spdap ? o.m_Pos < 5 : o.m_Pos < 10) && o.m_State == Item_PVP_Char.State.Live);
							cantargets.Sort((Item_PVP_Char _before, Item_PVP_Char _after) => {
								return _after.GetCombatPower().CompareTo(_before.GetCombatPower());
							});
							cnt = Mathf.Min(cnt, cantargets.Count);
							break;
						case PVPTurnType.Enemy:
							cantargets = enemy.FindAll(o => (!is_spdap ? o.m_Pos < 5 : o.m_Pos < 10) && o.m_State == Item_PVP_Char.State.Live);
							cantargets.Sort((Item_PVP_Char _before, Item_PVP_Char _after) => {
								return _after.GetCombatPower().CompareTo(_before.GetCombatPower());
							});
							cnt = Mathf.Min(cnt, cantargets.Count);
							break;
					}
				}
				for (int i = 0; i < cnt; i++) {
					Item_PVP_Char target = cantargets[0];
					m_Targets.Add(target);
					cantargets.Remove(target);
				}
				break;
			//전체
			case PVPAtkTargetType.All:
				if (_atker.m_PosType == PVPPosType.Combat) {
					cantargets = enemy.FindAll(o => o.m_Pos < 5 && o.m_State == Item_PVP_Char.State.Live);
				}
				else {
					switch (_atker.m_PSTData.m_TurnType) {
						case PVPTurnType.Team:
							cantargets = team.FindAll(o => (!is_spdap ? o.m_Pos < 5 : o.m_Pos < 10) && o.m_State == Item_PVP_Char.State.Live);
							break;
						case PVPTurnType.Enemy:
							cantargets = enemy.FindAll(o => (!is_spdap ? o.m_Pos < 5 : o.m_Pos < 10) && o.m_State == Item_PVP_Char.State.Live);
							break;
					}
				}
				m_Targets = cantargets;
				break;
			//현재 HP가 가장 적은 적 1명을 공격합니다.
			case PVPAtkTargetType.LowHP:
				if (_atker.m_PosType == PVPPosType.Combat) {
					cantargets = enemy.FindAll(o => o.m_Pos < 5 && o.m_State == Item_PVP_Char.State.Live);
					cantargets.Sort((Item_PVP_Char _before, Item_PVP_Char _after) => {
						return _before.GetStat(StatType.HP).CompareTo(_after.GetStat(StatType.HP));
					});
					cnt = Mathf.Min(cnt, cantargets.Count);
				}
				else {
					switch (_atker.m_PSTData.m_TurnType) {
						case PVPTurnType.Team:
							cantargets = team.FindAll(o => o.m_Pos < 5 && o.m_State == Item_PVP_Char.State.Live);
							cantargets.Sort((Item_PVP_Char _before, Item_PVP_Char _after) => {
								return _before.GetStat(StatType.HP).CompareTo(_after.GetStat(StatType.HP));
							});
							cnt = Mathf.Min(cnt, cantargets.Count);
							break;
						case PVPTurnType.Enemy:
							cantargets = enemy.FindAll(o => o.m_Pos < 5 && o.m_State == Item_PVP_Char.State.Live);
							cantargets.Sort((Item_PVP_Char _before, Item_PVP_Char _after) => {
								return _before.GetStat(StatType.HP).CompareTo(_after.GetStat(StatType.HP));
							});
							cnt = Mathf.Min(cnt, cantargets.Count);
							break;
					}
				}
				for (int i = 0; i < cnt; i++) {
					Item_PVP_Char target = cantargets[0];
					m_Targets.Add(target);
					cantargets.Remove(target);
				}
				break;
			//스킬 시전자의 정면 기준 좌우로 대상포함 N명을 공격합니다.(정면이랑 좌우)
			case PVPAtkTargetType.Near:
				int centerpos;
				int start;
				int end;
				if (_atker.m_PosType == PVPPosType.Combat) {
					centerpos = _atker.m_Pos;
					start = Math.Max(0, centerpos - cnt / 2);
					end = Math.Min(4, centerpos + cnt / 2);
					for (int i = start; i <= end; i++) {
						Item_PVP_Char target = enemydeck[i];
						if (target.m_State == Item_PVP_Char.State.Live) m_Targets.Add(target);
					}
				}
				else {
					bool sptline = UTILE.Get_Random(0f, 1f) > 0.5f;
					centerpos = sptline ? _atker.m_Pos : _atker.m_Pos - 5;
					start = Math.Max(sptline ? 4 : 0, centerpos - cnt / 2);
					end = Math.Min(sptline ? 4 : 9, centerpos + cnt / 2);
					switch (_atker.m_PSTData.m_TurnType) {
						case PVPTurnType.Team:
							for (int i = start; i <= end; i++) {
								Item_PVP_Char target = teamdeck[i];
								if ((!is_spdap ? target.m_Pos < 5 : target.m_Pos < 10) && target.m_State == Item_PVP_Char.State.Live) m_Targets.Add(target);
							}
							break;
						case PVPTurnType.Enemy:
							for (int i = start; i <= end; i++) {
								Item_PVP_Char target = enemydeck[i];
								if ((!is_spdap ? target.m_Pos < 5 : target.m_Pos < 10) && target.m_State == Item_PVP_Char.State.Live) m_Targets.Add(target);
							}
							break;
					}
				}
				break;
			case PVPAtkTargetType.RandomNear:
				cantargets = enemy.FindAll(o => o.m_Pos < 5 && o.m_State == Item_PVP_Char.State.Live);
				centerpos = cantargets[UTILE.Get_Random(0, cantargets.Count)].m_Pos;
				if (_atker.m_PosType == PVPPosType.Combat) {
					start = Math.Max(0, centerpos - cnt / 2);
					end = Math.Min(4, centerpos + cnt / 2);
					for (int i = start; i <= end; i++) {
						Item_PVP_Char target = enemydeck[i];
						if (target.m_State == Item_PVP_Char.State.Live) m_Targets.Add(target);
					}
				}
				else {
					bool sptline = UTILE.Get_Random(0f, 1f) > 0.5f;
					centerpos = sptline ? centerpos : centerpos - 5;
					start = Math.Max(sptline ? 4 : 0, centerpos - cnt / 2);
					end = Math.Min(sptline ? 4 : 9, centerpos + cnt / 2);
					switch (_atker.m_PSTData.m_TurnType) {
						case PVPTurnType.Team:
							for (int i = start; i <= end; i++) {
								Item_PVP_Char target = teamdeck[i];
								if ((!is_spdap ? target.m_Pos < 5 : target.m_Pos < 10) && target.m_State == Item_PVP_Char.State.Live) m_Targets.Add(target);
							}
							break;
						case PVPTurnType.Enemy:
							for (int i = start; i <= end; i++) {
								Item_PVP_Char target = enemydeck[i];
								if ((!is_spdap ? target.m_Pos < 5 : target.m_Pos < 10) && target.m_State == Item_PVP_Char.State.Live) m_Targets.Add(target);
							}
							break;
					}
				}
				break;
		}
		return m_Targets;
	}
}