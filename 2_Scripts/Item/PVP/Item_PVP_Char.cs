using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static LS_Web;
using TMPro;
using UnityEngine.Rendering;

public class Item_PVP_Char : ObjMng
{
	public enum State
	{
		Live,
		Die
	}
	public enum TransActionType
	{
		Move,
		Atk,
		ScaleUpDown
	}
	public class Buff
	{
		public float Val;	//(디)버프값
		public int Turn;	// 지속턴
	}
	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		//카드는 아마 쉐이더로 하나에 묶겠지
		public SortingGroup Sorting;
		public SpriteRenderer Portrait;
		public SpriteRenderer GrageBG;
		public TextMeshPro Lv;
		public TextMeshPro HP;
		public SpriteRenderer HPGauge;
		public SpriteRenderer APGauge;
		public SpriteRenderer WeaponIcon;
		public SpriteRenderer ArmorIcon;
		public GameObject SkillAbleFX;
		public GameObject DieFX;
		public GameObject DebuffFX;
		public Transform BuffLabelPrefab;
		public Transform BuffLabelBucket;
		public Animator NowAtkFX;
	}
	[SerializeField] SUI m_SUI;
	public UserPos m_UserPos;
	public UserPos GetOtherPos { get { return (UserPos)(m_UserPos - 1 < 0 ? 1 : 0); } }
	public PVPPosType m_PosType;
	public int m_Pos;
	public State m_State;
	public RES_PVP_CHAR m_Info;
	public Item_PVP_Char m_Partner { get { return PVP.m_PlayUser[(int)m_UserPos].m_Chars[m_Pos < 5 ? m_Pos + 5 : m_Pos - 5]; } }

	float m_AP;
	float[,] m_Stat = new float[(int)StatType.Max, 2];
	public bool Is_Hit;
	/// <summary> 버프 타입별 버프종류와 값, 남은턴, 같은 계열이면 새거로 갈아끼움 </summary>
	public Dictionary<PVPTurnType, Dictionary<StatType, Buff>> m_Buffs = new Dictionary<PVPTurnType, Dictionary<StatType, Buff>>();
	public Dictionary<PVPTurnType, Dictionary<StatType, Item_PVP_BuffLabel>> m_BuffLabels = new Dictionary<PVPTurnType, Dictionary<StatType, Item_PVP_BuffLabel>>();

	IEnumerator m_Action;
	public bool IS_Action { get { return m_Action != null; } }
	public TCharacterTable m_TData { get { return TDATA.GetCharacterTable(m_Info.Idx); } }
	public TPVPSkillTable m_PSTData { get { return TDATA.GeTPVPSkillTable(m_TData.m_PVPSkillIdx); } }
	public iTween.EaseType m_MoveEase = iTween.EaseType.easeInCirc;
	public iTween.EaseType m_UpDownEase = iTween.EaseType.easeOutExpo;
	public void SetData(UserPos _pos, RES_PVP_CHAR _charinfo) {
		m_UserPos = _pos;
		m_Info = _charinfo;
		m_PosType = m_TData.m_PVPPosType;
		m_Pos = m_Info.Pos;
		m_State = State.Live;

		Init();
	}
	private void Init() {
		Is_Hit = false;
		//스탯
		m_Buffs.Clear();
		m_Buffs.Add(PVPTurnType.Team, new Dictionary<StatType, Buff>());
		m_Buffs.Add(PVPTurnType.Enemy, new Dictionary<StatType, Buff>());

		SetStat();
		//ap,hp
		m_AP = 0f;
		m_SUI.APGauge.material.SetFloat("Fill_Amount", 0f);
		SetAP(UTILE.Get_Random(0f, BaseValue.PVP_INIT_TURN_SEED), 0f);
		m_SUI.HPGauge.material.SetFloat("Fill_Amount", 0f);
		SetHP(0, 0f);

		//유아이
		m_SUI.Portrait.sprite = m_TData.GetPortrait();
		m_SUI.GrageBG.sprite = BaseValue.CharBG(m_Info.Grade);
		m_SUI.Lv.text = m_Info.LV.ToString();
		m_SUI.WeaponIcon.sprite = BaseValue.GetPVPEquipAtkIcon(m_PSTData.m_AtkType);
		m_SUI.ArmorIcon.sprite = BaseValue.GetPVPEquipDefIcon(m_TData.m_PVPArmorType);
		m_SUI.SkillAbleFX.SetActive(false);
		m_SUI.DieFX.SetActive(false);
		m_SUI.DebuffFX.SetActive(false);
		m_SUI.NowAtkFX.gameObject.SetActive(false);
	}
	public void Refresh() {
		m_Info.LV += Mathf.RoundToInt(PVP.GetBuff(m_UserPos, StageCardType.LevelUp));
		SetStat();
		SetAP(0);
		SetHP(0);
		m_SUI.Lv.text = m_Info.LV.ToString();
	}
	void SetStat() {
		for (int i = 0; i < m_Info.Stat.Count; i++) {
			var stat = m_Info.Stat.ElementAt(i);
			float val = stat.Value;
			float add = 0;
			float per = 1f;
			switch (stat.Key) {
				case StatType.Atk: per += PVP.GetBuff(m_UserPos, StageCardType.AtkUp); break;
				case StatType.Def: per += PVP.GetBuff(m_UserPos, StageCardType.DefUp); break;
				case StatType.HP: per += PVP.GetBuff(m_UserPos, StageCardType.HpUp); break;
				case StatType.Speed: per += PVP.GetBuff(m_UserPos, StageCardType.SpeedUp); break;
				case StatType.Critical: add += PVP.GetBuff(m_UserPos, StageCardType.CriticalUp); break;
				case StatType.CriticalDmg: add += PVP.GetBuff(m_UserPos, StageCardType.CriticalDmgUp); break;
				case StatType.Heal: per += PVP.GetBuff(m_UserPos, StageCardType.HealUp); break;
				case StatType.HeadShot: per += PVP.GetBuff(m_UserPos, StageCardType.HeadShotUp); break;
			}
			if (per > 0f) PVPMng.Log(string.Format("TeamChar SelectBuff - Pos : {0} // Idx : {1} // Stat : {2} // Val : {3}", m_Pos, m_Info.Idx, stat.Key.ToString(), per));
			val = val * per + add;
			m_Stat[(int)stat.Key, 0] = m_Stat[(int)stat.Key, 1] = stat.Key == StatType.HP ? Mathf.RoundToInt(val) : val;
		}
		m_Stat[(int)StatType.SuccessAttackPer, 0] = m_Stat[(int)StatType.SuccessAttackPer, 1] = BaseValue.SUCCESS_ATTACK_PER;
	}
	/// <summary> 버프와 파트너 계산된 스탯</summary>
	public float GetStat(StatType _type, int _pos = 0) {
		if (m_PosType == PVPPosType.Combat) {
			switch (_type) {
				case StatType.Critical:
				case StatType.CriticalDmg:
					return (m_Stat[(int)_type, _pos] + GetBuffStat(_type) + m_Partner.m_Stat[(int)_type, _pos] + m_Partner.GetBuffStat(_type)) / 2f;
				case StatType.Speed:
					return m_Stat[(int)_type, _pos] + GetBuffStat(_type);
				case StatType.Men:
				case StatType.Sat:
				case StatType.Hyg:
				case StatType.HP:
				case StatType.SuccessAttackPer:
					return m_Stat[(int)_type, _pos] * GetBuffStat(_type);
				default:
					return m_Stat[(int)_type, _pos] * GetBuffStat(_type) + m_Partner.m_Stat[(int)_type, _pos] * m_Partner.GetBuffStat(_type);
			}
		}
		else {
			switch (_type) {
				case StatType.Critical:
				case StatType.CriticalDmg:
				case StatType.Speed:
					return m_Stat[(int)_type, _pos] + GetBuffStat(_type);
				default:
					return m_Stat[(int)_type, _pos] * GetBuffStat(_type);
			}
		}
	}
	/// <summary> (디)버프, 즉각 적용 </summary>
	public void SetBuff(TPVPSkillTable _tdata, bool _my) {
		if (_tdata.m_TurnType == PVPTurnType.None) return;
		StatType stat = (StatType)Mathf.RoundToInt(_tdata.m_Vals[0]);
		var buffs = m_Buffs[_tdata.m_TurnType];
		if (!buffs.ContainsKey(stat)) {
			buffs.Add(stat, new Buff());
			SetBuffFX(_tdata.m_TurnType, stat, true, _tdata.m_Vals[2] > 0);
		}
		buffs[stat].Turn = _tdata.m_TurnCnt;
		buffs[stat].Val = _tdata.m_Vals[2];

		if (_my) {
			PVP.SetBuffAlarm(stat, Mathf.RoundToInt(_tdata.m_Vals[2]));
		}
		else {
			if (_tdata.m_Vals[2] < 0f) {
				List<SND_IDX> snds = new List<SND_IDX>() { SND_IDX.SFX_1330, SND_IDX.SFX_1331, SND_IDX.SFX_1332 };
				PlayEffSound(snds[UTILE.Get_Random(0, 3)]);
			}
		}
	}
	/// <summary> 현재 가진 버프 디버프로 해당 스텟 추가율 반환 </summary>
	public float GetBuffStat(StatType _type) {
		float val = 0f;
		switch (_type) {
			case StatType.Critical:
			case StatType.CriticalDmg:
			case StatType.Speed: 
				val = 0f;
				break;
			default:
				val = 1f;
				break;
		}
		for (PVPTurnType type = PVPTurnType.Team; type <= PVPTurnType.Enemy; type++) {
			var typebuffs = m_Buffs[type];
			if (typebuffs == null) continue;
			for (int i = 0; i < typebuffs.Count; i++) {
				var buffs = typebuffs.ElementAt(i);
				if (buffs.Key == _type) {
					val += buffs.Value.Val; 
					break;
				}
			}
		}
		return val;
	}
	public int GetCombatPower() {
		float cp = 0;
		for (StatType i = StatType.Men; i < StatType.Max; i++) {
			cp += Mathf.RoundToInt(GetStat(i, 1) * BaseValue.COMBAT_POWER_RATIO(i));
		}
		return (int)cp;
	}
	/// <summary> 해당 행동하는곳에서 체크
	/// TeamBuff	"아군에게 버프 효과 적용 시, 버프를 받은 아군(어태커 한정)이 1회 타격 할 때마다 버프 카운트가 1씩 차감(빗나감에도 1씩 차감)"
	/// EnemyBuff	"적군에게 디버프 효과 적용 시, 디버프를 받은 적군(어태커 한정)이 1회 피격 할 때마다 디버프 카운트가 1씩 차감(빗나감에도 1씩 차감)"
	/// UitlBuff	"스테이터스 관련 버프, 디버프로 즉시 적용"
	/// </summary>
	public void CheckBuff(bool _atk) {
		if (m_State == State.Die) return;

		for (PVPTurnType i = PVPTurnType.Team; i <= PVPTurnType.Enemy; i++) {
			var typebuffs = m_Buffs[i];
			for (int j = 0; j < typebuffs.Count; j++) {
				var buff = typebuffs.ElementAt(j);
				if (_atk && (buff.Key == StatType.Atk || buff.Key == StatType.SuccessAttackPer || buff.Key == StatType.Speed || buff.Key == StatType.Critical)) {
					buff.Value.Turn--;
					PVPMng.Log(string.Format("공격 // 공격진영 : {0} // pos : {1} // charidx : {2} // 턴타입 : {3} // 스탯 :{4} // 남은 턴수 : {5}",
					m_UserPos.ToString(), m_Pos, m_Info.Idx, i.ToString(), buff.Key, buff.Value));
				}
				else if (!_atk && buff.Key == StatType.Def) {
					buff.Value.Turn--;
					PVPMng.Log(string.Format("공격 // 공격진영 : {0} // pos : {1} // charidx : {2} // 턴타입 : {3} // 스탯 :{4} // 남은 턴수 : {5}",
					m_UserPos.ToString(), m_Pos, m_Info.Idx, i.ToString(), buff.Key, buff.Value));
				}
				if (buff.Value.Turn <= 0) {
					SetBuffFX(i, buff.Key, false);
					typebuffs.Remove(buff.Key);
				}
			}
		}
		//m_SUI.DebuffFX.SetActive(m_Buffs[PVPTurnType.Enemy].Count > 0);
	}
	/// <summary> 걸려있는 버프 목록 갱신, 버프 걸릴때 or 매턴 </summary>
	void SetBuffFX(PVPTurnType _type, StatType _stat, bool _add, bool _buff = true) {
		if (_add) {
			Item_PVP_BuffLabel label = Utile_Class.Instantiate(m_SUI.BuffLabelPrefab.gameObject, m_SUI.BuffLabelBucket).GetComponent<Item_PVP_BuffLabel>();
			label.SetData(_type, _stat, _buff, BuffLabelRefresh);
			label.transform.localPosition = new Vector3(0f, -0.58f * (m_SUI.BuffLabelBucket.childCount - 1), 0f);
			if (!m_BuffLabels.ContainsKey(_type)) m_BuffLabels.Add(_type, new Dictionary<StatType, Item_PVP_BuffLabel>());
			m_BuffLabels[_type].Add(_stat, label);
		}
		else m_BuffLabels[_type][_stat].End();
	}
	/// <summary> bufflabel end 호출 후 애니 끝난 후 오브젝트 및 리스트 삭제 summary>
	void BuffLabelRefresh(Item_PVP_BuffLabel _label) {
		for(int i = 0, pos = 0; i < m_SUI.BuffLabelBucket.childCount; i++) {
			Transform trans = m_SUI.BuffLabelBucket.GetChild(i);
			if (trans.GetComponent<Item_PVP_BuffLabel>() == _label) continue;
			trans.localPosition = new Vector3(0f, -0.58f * pos, 0f);
			pos++;
		}

		m_BuffLabels[_label.m_Type].Remove(_label.m_Stat);
		Destroy(_label.gameObject);
	}
	/// <summary> 명중률 계산해서 맞췄는지 체크 </summary>
	public bool IS_Hit() {
		if (m_PosType == PVPPosType.Combat) {
			float ratio = Mathf.Min(BaseValue.SUCCESS_ATTACK_PER_LIMIT, GetStat(StatType.SuccessAttackPer) * (1f + (float)m_PSTData.m_AtkCorrect / 100f));
			PVPMng.Log(string.Format("공격 // 공격진영 : {0} // pos : {1} // charidx : {2} // 명중률 : {3}",
				m_UserPos.ToString(), m_Pos, m_Info.Idx, ratio));
			return UTILE.Get_Random(0f, 1f) < ratio;
		}
		else return true;
	}
	/// <summary> 서포터가 파트너 어태커가 사망시 도망갈지 안갈지 체크 </summary>
	public bool IS_Run() {
		PVPMng.Log(string.Format("어태커 사망, 사망자진영 : {0} // 서포터위치 : {1} // charidx : {2} // 서포터 도망 수치 : {3}", m_UserPos.ToString(), m_Pos, m_Info.Idx, PVP.m_PlayUser[(int)m_UserPos].GetStat(StatType.Run, 0) + m_PSTData.m_RunPer));
		return UTILE.Get_Random(0f, 1f) < PVP.m_PlayUser[(int)m_UserPos].GetStat(StatType.Run, 0) + m_PSTData.m_RunPer;
	}
	/// <summary>
	/// 정신력 : 서포터 군이 스킬 사용 시 정신력을 소모한다.
	/// 포만도 : 한 번 행동(공격, 스킬) 사용 시 일정량의 포만도를 사용한다.
	/// 위생(청결도) : 적에게 피격 시 위생도가 감소한다.
	/// </summary>
	public KeyValuePair<StatType, int> GetConsumeStat(bool _atk) {
		if (_atk) {
			return new KeyValuePair<StatType, int>(m_PSTData.m_UseStatType, m_PSTData.m_UseStatVal);
		}
		else return new KeyValuePair<StatType, int>(StatType.Hyg, Mathf.RoundToInt(m_PSTData.m_AtkHygDmg * ( 1f + PVP.m_PlayUser[(int)m_UserPos].GetResearch(ResearchEff.PVPPerAtkHygUp))));
	}
	public class PVPDmgStr
	{
		public PVPDmgType Type = PVPDmgType.Normal;
		public Sprite Icon = null;
		public int Dmg = 0;
	}
	public PVPDmgStr GetDmg(Item_PVP_Char _target) {
		PVPDmgStr val = new PVPDmgStr();
		TPVPDefTable tdata = TDATA.GeTPVPDefTable(m_PSTData.m_AtkType);
		float wpratio = tdata != null ? tdata.GetRatio(_target.m_TData.m_PVPArmorType) : 1f;
		float ctratio = UTILE.Get_Random(0f, 1f) < GetStat(StatType.Critical) ? 1f + GetStat(StatType.CriticalDmg) : 1f;
		float defratio = BaseValue.GetPVPDmgDefRatio(m_Info.LV, _target.GetStat(StatType.Def));
		PVPMng.Log(string.Format("공격 // 공격진영 : {0} // pos : {1} // charidx : {2} // 치명률 : {3} // 치명배율 : {4}\n, 공격자 무기 : {5} // 타겟 방어구 : {6} // 무기방어구 상성 배율 : {7} // 데미지경감률 : {8}", 
			m_UserPos.ToString(), m_Pos, m_Info.Idx, GetStat(StatType.Critical), ctratio, m_PSTData.m_AtkType.ToString(), _target.m_TData.m_PVPArmorType.ToString(), wpratio, defratio));

		float dmg = GetStat(StatType.Atk) * m_PSTData.m_Vals[0] * ctratio * wpratio * (1f - defratio);
		PVPMng.Log(string.Format("공격력 : {0} // 타겟 방어력 : {1} // 스킬배율 : {2} // 기본데미지 : {3}", GetStat(StatType.Atk), _target.GetStat(StatType.Def), m_PSTData.m_Vals[0], dmg));
		dmg = dmg * UTILE.Get_Random(1f - BaseValue.PVP_DMG_VARIATION, 1f + BaseValue.PVP_DMG_VARIATION);
		PVPMng.Log(string.Format("최종데미지 : {0}", Mathf.RoundToInt(dmg)));

		val.Type = tdata != null ? (tdata.GetRatio(_target.m_TData.m_PVPArmorType) >= 1f ? PVPDmgType.Good : PVPDmgType.Bad) : PVPDmgType.Normal;
		val.Icon = val.Type == PVPDmgType.Normal ? null : (val.Type == PVPDmgType.Good ? BaseValue.GetPVPEquipAtkIcon(m_PSTData.m_AtkType) : BaseValue.GetPVPEquipDefIcon(_target.m_TData.m_PVPArmorType));
		val.Dmg = Mathf.RoundToInt(dmg);
		return val;
	}
	//타격자 어택이랑 피격자 히트 액션 끝나야 다음 턴으로 넘기게 하자
	public IEnumerator Atk(List<Item_PVP_Char> _target, List<Dictionary<StatType, int>> _dmgs, Action<int, int> _cb) {
		Vector3 atkerpos = transform.position;
		Vector3 atkfxpos = transform.position;
		Vector3 atkfxrot = m_UserPos == UserPos.My ? Vector3.zero : new Vector3(0f, 0f, 180f);
		string fxstr = string.Empty;
		SND_IDX sfx = SND_IDX.NONE;

		m_Action = null;

		//연출 및 데미지 표기
		if (m_PosType == PVPPosType.Combat) {
			//원거리 공격 타입 : 앞으로 살짝 나온 후 각자 무기 타입에 맞는 공격 액션 발생
			//근거리 공격 타입: 타겟에게 이동하여 공격 액션 발생
			switch (m_PSTData.m_Weapon) {
				case ItemType.Blunt:
					fxstr = string.Empty;
					sfx = SND_IDX.SFX_0441;
					break;
				case ItemType.Blade: 
					fxstr = string.Empty;
					sfx = SND_IDX.SFX_0420;
					break;
				case ItemType.Axe: 
					fxstr = string.Empty;
					sfx = SND_IDX.SFX_0441;
					break;
				case ItemType.Bow: 
					fxstr = string.Empty;
					sfx = SND_IDX.SFX_0431;
					break;
				case ItemType.Pistol: 
					fxstr = string.Empty;
					sfx = SND_IDX.SFX_0401;
					break;
				case ItemType.Shotgun: 
					fxstr = "Effect/PVP/Eff_PVP_ChAtk_ShotGun";
					sfx = SND_IDX.SFX_0500;
					break;
				case ItemType.Rifle: 
					fxstr = "Effect/PVP/Eff_PVP_ChAtk_Rifle";
					sfx = SND_IDX.SFX_0600;
					break;
			}
		}
		else {
			fxstr = m_PSTData.m_TurnType == PVPTurnType.Team ? "Effect/PVP/PVP_Trail_Green" : "Effect/PVP/PVP_Trail_Red";
			//이팩트를 통해 자신이 영향을 미치는 부분에 발사체 사출
		}

		if (m_PosType == PVPPosType.Combat) {
			if(_target.Count < 1) {
				PVP.CharSpeech(DialogueConditionType.UnSkill, this);
				yield return new WaitForSeconds(0.5f);
				yield break;
			}

			int pos = Mathf.FloorToInt(_target.Count / 2f);
			Vector3 targetpos = _target.Count > 0 ? _target[pos].transform.position : Vector3.zero;
			targetpos.y = 0f;
			List<IEnumerator> hitactions = new List<IEnumerator>();

			PlayEffSound(m_TData.GetVoice(TCharacterTable.VoiceType.Skill));
			//PlayVoiceSnd(new List<SND_IDX>() { m_TData.GetVoice(TCharacterTable.VoiceType.Skill) });

			//연출 전 소팅 변경
			m_SUI.Sorting.sortingOrder = 2;
			//타겟 확대
			for (int i = 0; i < _target.Count; i++) {
				//StartCoroutine(_target[i].TransAction(TransActionType.Move, _target[i].transform.position + new Vector3(0f, 0.18f, 0f), 0.3f, 0.4f, 0f, m_UpDownEase));
				_target[i].Call_TransAction(TransActionType.ScaleUpDown, Vector3.one * 1.08f, 0.3f, 0.35f, 0f, iTween.EaseType.easeOutCirc);
			}
			//확대
			if (gameObject.activeSelf) StartCoroutine(IE_AtkNowFxAction(true));
			yield return TransAction(TransActionType.Move, atkerpos + new Vector3(0f, 0.24f,0f), 0f, 0.35f, 0f, m_UpDownEase);
			yield return TransAction(TransActionType.ScaleUpDown, Vector3.one * 1.08f, 0f, 0.25f, 0.25f, iTween.EaseType.easeOutCirc);
			yield return new WaitForSeconds(0.2f);
			//타겟으로 이동 연출

			float movetime = 0.25f;
			switch (m_PSTData.m_Weapon) {
				case ItemType.Shotgun:
				case ItemType.Rifle:
				case ItemType.Blade:
					if(m_PSTData.m_Weapon == ItemType.Rifle)targetpos = Vector3.zero;
					targetpos += new Vector3(0f, m_UserPos == UserPos.My ? -1f : 1f, 0f);
					m_MoveEase = iTween.EaseType.easeInOutCubic;
					break;
				default:
					m_MoveEase = iTween.EaseType.easeInOutCirc;
					break;
			}

			if(IS_AtkMove()) yield return TransAction(TransActionType.Move, targetpos, 0f, movetime, movetime, m_MoveEase);
			//공격 모션
			
			yield return TransAction(TransActionType.Atk, Vector3.one, IS_Melee() ? 0f : 0.3f);

			PVP.CamAction(PVPMng.CamActionType.Shake_0, 0f, 0.25f, Vector3.one * 0.025f);
			//사운드
			PlayEffSound(sfx);
			//공격 이펙트
			if (!string.IsNullOrEmpty(fxstr)) {
				GameObject fx = PVP.StartEff(transform.position, fxstr);
				fx.transform.localEulerAngles = atkfxrot;
			}
			float atkedelay = 0f;
			if (IS_Melee()) atkedelay = 0f;
			else {
				switch (m_PSTData.m_Weapon) {
					case ItemType.Shotgun:
						atkedelay = 0.25f;
						break;
					case ItemType.Rifle:
						atkedelay = 1.4f;
						break;
					default:
						atkedelay = 0f;
						break;
				}
			}
			yield return new WaitForSeconds(atkedelay);

			for (int i = 0; i < _target.Count; i++) {
				//타겟에 도착 후 타격 판정 및 연출
				Item_PVP_Char target = _target[i];
				IEnumerator hitaction = target.Hit(this, _dmgs[i], (diffhp)=> {
					_cb?.Invoke(i, diffhp);
				},
				()=> {
					//타겟 피격 후 축소
					target.Call_TransAction(TransActionType.ScaleUpDown, Vector3.one, 0f, 0.25f, 0f, iTween.EaseType.easeOutCubic);
				});
				hitactions.Add(hitaction);
				StartCoroutine(hitaction);
			}

			//타겟 피격 연출 후 복귀 연출
			yield return new WaitForSeconds(0.3f);
			if (gameObject.activeSelf) StartCoroutine(IE_AtkNowFxAction(false));
			yield return TransAction(TransActionType.Move, atkerpos, 0f, 0.35f, 0.2f, m_MoveEase);
			//축소
			yield return TransAction(TransActionType.ScaleUpDown, Vector3.one, 0f, 0.25f, 0.25f, iTween.EaseType.easeOutCubic);
			//모든 연출 후 소팅 변경
			m_SUI.Sorting.sortingOrder = 1;

			yield return new WaitWhile(() => hitactions.FindAll(o=>o.Current != null).Count > 0);
		}
		else {
			if (_target.Count < 1) {
				List<Vector3> statpos = new List<Vector3>();
				UserPos targetpos = m_PSTData.m_TurnType == PVPTurnType.Team ? m_UserPos : GetOtherPos;

				switch (m_PSTData.m_Type) {
					case PVPSkillType.Status:
					case PVPSkillType.LowStatus:
					case PVPSkillType.TwoStat:
						//제자리 연출
						m_SUI.Sorting.sortingOrder = 2;
						//확대
						if (gameObject.activeSelf) StartCoroutine(IE_AtkNowFxAction(true));
						yield return TransAction(TransActionType.Move, atkerpos + new Vector3(0f, 0.24f, 0f), 0f, 0.35f, 0f, m_UpDownEase);
						yield return TransAction(TransActionType.ScaleUpDown, Vector3.one * 1.08f, 0f, 0.25f, 0.25f, iTween.EaseType.easeOutCirc);
						yield return new WaitForSeconds(0.2f);
						//공격 모션
						yield return TransAction(TransActionType.Atk, Vector3.one, IS_Melee() ? 0f : 0.3f);

						if(m_PSTData.m_Type == PVPSkillType.Status) {
							statpos.Add(PVP.GetStatPos(targetpos, (StatType)(int)m_PSTData.m_Vals[0]));
						}
						else if (m_PSTData.m_Type == PVPSkillType.LowStatus) {
							StatType lowstat = StatType.Men;
							float val = 0f;
							for (StatType i = StatType.Men; i < StatType.SurvEnd; i++) {
								float now = PVP.m_PlayUser[(int)targetpos].m_Stat[(int)i, 0];
								if (i == StatType.Men || val >= now) {
									lowstat = i;
									val = now;
								}
							}
							statpos.Add(PVP.GetStatPos(targetpos, lowstat));
						}
						else if (m_PSTData.m_Type == PVPSkillType.TwoStat) {
							statpos.Add(PVP.GetStatPos(targetpos, (StatType)(int)m_PSTData.m_Vals[0]));
							statpos.Add(PVP.GetStatPos(targetpos, (StatType)(int)m_PSTData.m_Vals[1]));
						}

						//사운드
						PlayEffSound(SND_IDX.SFX_0351);
						for (int i = 0; i < statpos.Count; i++) {
							//공격 이펙트
							GameObject fx = PVP.StartEff(atkfxpos, fxstr);
							fx.transform.localEulerAngles = atkfxrot;
							fx.GetComponent<AutoAciveOff>().SetTime(0.7f);

							float randx = Mathf.Abs(atkfxpos.x - statpos[i].x) * 0.2f;
							randx = UTILE.Get_Random(-randx, randx);
							float randy = Mathf.Abs(atkfxpos.y - statpos[i].y) * 0.2f;
							randy = UTILE.Get_Random(-randy, randy);
							float randz = UTILE.Get_Random(0f, 2f);
							iTween.MoveTo(fx, iTween.Hash("path", new Vector3[] { atkfxpos, (atkfxpos + statpos[i]) / 2f + new Vector3(randx, randy, randz), statpos[i] }, "time", 0.4f, "easetype", iTween.EaseType.easeOutCirc));

						}
						yield return new WaitForSeconds(0.4f);
						for (int i = 0; i < statpos.Count; i++) {
							fxstr = m_PSTData.m_TurnType == PVPTurnType.Team ? "Effect/PVP/PVP_Trail_Green_Hit" : "Effect/PVP/PVP_Trail_Red_Hit";
							GameObject fx = PVP.StartEff(statpos[i], fxstr);
						}
						yield return new WaitForSeconds(0.3f);

						//버프, 스탯 등 효과 적용
						_cb?.Invoke(-1, 0);

						//타겟 피격 연출 후 복귀 연출
						yield return new WaitForSeconds(0.2f);
						if (gameObject.activeSelf) StartCoroutine(IE_AtkNowFxAction(false));
						yield return TransAction(TransActionType.Move, atkerpos, 0f, 0.35f, 0f, m_UpDownEase);
						//축소
						yield return TransAction(TransActionType.ScaleUpDown, Vector3.one, 0f, 0.25f, 0.25f, iTween.EaseType.easeOutCubic);
						//모든 연출 후 소팅 변경
						m_SUI.Sorting.sortingOrder = 1;
						break;
					default:
						//아무것도 안할때
						yield return new WaitForSeconds(1f);
						break;
				}
			}
			else {
				int pos = Mathf.FloorToInt(_target.Count / 2f);
				Vector3 targetpos = _target.Count > 0 ? _target[pos].transform.position : Vector3.zero;

				//연출 전 소팅 변경
				m_SUI.Sorting.sortingOrder = 2; 
				//타겟 확대
				for (int i = 0; i < _target.Count; i++) {
					//StartCoroutine(_target[i].TransAction(TransActionType.Move, _target[i].transform.position + new Vector3(0f, 0.18f, 0f), 0.3f, 0.4f, 0f, m_UpDownEase));
					_target[i].Call_TransAction(TransActionType.ScaleUpDown, Vector3.one * 1.08f, 0.3f, 0.35f, 0f, iTween.EaseType.easeOutCirc);
				}
				//확대
				if(gameObject.activeSelf) StartCoroutine(IE_AtkNowFxAction(true));
				yield return TransAction(TransActionType.Move, atkerpos + new Vector3(0f, 0.24f, 0f), 0f, 0.35f, 0f, m_UpDownEase);
				yield return TransAction(TransActionType.ScaleUpDown, Vector3.one * 1.08f, 0f, 0.25f, 0.25f, iTween.EaseType.easeOutCirc);
				yield return new WaitForSeconds(0.2f);
				//타겟으로 이동
				//yield return TransAction(TransActionType.Move, targetpos, 0f, 0.25f, 0.25f, m_MoveEase);
				//공격 모션
				yield return TransAction(TransActionType.Atk, Vector3.one, IS_Melee() ? 0f : 0.3f);

				//사운드
				PlayEffSound(SND_IDX.SFX_0351);

				for (int i = 0; i < _target.Count; i++) {
					//공격 이펙트
					GameObject fx = PVP.StartEff(atkfxpos, fxstr);
					fx.transform.localEulerAngles = atkfxrot;
					fx.GetComponent<AutoAciveOff>().SetTime(0.7f);

					float randx = Mathf.Abs(atkfxpos.x - _target[i].transform.position.x) * 0.2f;
					randx = UTILE.Get_Random(-randx, randx);
					float randy = Mathf.Abs(atkfxpos.y - _target[i].transform.position.y) * 0.2f;
					randy = UTILE.Get_Random(-randy, randy);
					float randz = UTILE.Get_Random(0f, 2f);
					iTween.MoveTo(fx, iTween.Hash("path", new Vector3[] { atkfxpos, (atkfxpos + _target[i].transform.position) / 2f + new Vector3(randx, randy, randz), _target[i].transform.position }, "time", 0.4f, "easetype", iTween.EaseType.easeOutCirc));
				}
				yield return new WaitForSeconds(0.4f);
				for (int i = 0; i < _target.Count; i++) {
					fxstr = m_PSTData.m_TurnType == PVPTurnType.Team ? "Effect/PVP/PVP_Trail_Green_Hit" : "Effect/PVP/PVP_Trail_Red_Hit";
					GameObject fx = PVP.StartEff(_target[i].transform.position, fxstr);
				}
				yield return new WaitForSeconds(0.3f);

				//버프, 스탯 등 효과 적용
				for (int i = 0; i < _target.Count; i++) {
					_cb?.Invoke(i, 0);
					//StartCoroutine(_target[i].TransAction(TransActionType.Move, _target[i].transform.position + new Vector3(0f, -0.18f, 0f), 0.15f, 0.35f, 0f, m_UpDownEase));
					//타겟 피격 후 축소
					_target[i].Call_TransAction(TransActionType.ScaleUpDown, Vector3.one, 0.15f, 0.25f, 0f, iTween.EaseType.easeOutCubic);
				}
				if (m_UserPos == UserPos.My && m_PSTData.m_TurnType == PVPTurnType.Enemy && UTILE.Get_Random(0f, 1f) < 0.2f) {
					List<SND_IDX> snds = new List<SND_IDX>() { SND_IDX.SFX_1330, SND_IDX.SFX_1331, SND_IDX.SFX_1332 };
					PlayEffSound(snds[UTILE.Get_Random(0, 3)]);
				}

				//타겟 피격 연출 후 복귀 연출
				if (gameObject.activeSelf) StartCoroutine(IE_AtkNowFxAction(false));
				yield return TransAction(TransActionType.Move, atkerpos, 0f, 0.35f, 0.25f, m_MoveEase);
				//축소
				yield return TransAction(TransActionType.ScaleUpDown, Vector3.one, 0f, 0.25f, 0.25f, iTween.EaseType.easeOutCubic);
				//모든 연출 후 소팅 변경
				m_SUI.Sorting.sortingOrder = 1;
			}
		}
	}
	public IEnumerator Hit(Item_PVP_Char _atker, Dictionary<StatType, int> _dmgs, Action<int> _cb, Action _cb2) {
		//이펙트
		string fxstr = string.Empty;
		Vector3 rot = Vector3.zero;
		switch (_atker.m_PSTData.m_Weapon) {
			case ItemType.Blunt: fxstr = "Effect/PVP/Eff_PVP_ChHit_Blunt"; break;
			case ItemType.Blade: fxstr = "Effect/PVP/Eff_PVP_ChHit_Blade"; break;
			case ItemType.Axe: fxstr = "Effect/PVP/Eff_PVP_ChHit_Axe"; break;
			case ItemType.Bow: 
				fxstr = "Effect/PVP/Eff_PVP_ChHit_Bow";
				rot = new Vector3(0f, 0f, 180f); break;
			case ItemType.Pistol: fxstr = "Effect/PVP/Eff_PVP_ChHit_Pistol"; break;
			case ItemType.Shotgun: fxstr = "Effect/PVP/Eff_PVP_ChHit_ShotGun"; break;
			case ItemType.Rifle: fxstr = string.Empty; break;
		}
		if (!string.IsNullOrEmpty(fxstr)) {
			GameObject fx = PVP.StartEff(transform.position, fxstr);
			fx.transform.localEulerAngles = rot;
		}

		if (Is_Hit) {
			float hitsndratio = 0.3f;
			if (_dmgs.ContainsKey(StatType.HP)) {
				if (_dmgs[StatType.HP] / GetStat(StatType.HP, 1) > 0.3f) hitsndratio = 1f;
			   _cb?.Invoke(Mathf.RoundToInt(SetHP(-_dmgs[StatType.HP])));
			}
			//모션
			m_SUI.Anim.SetTrigger("Char_Damage");
		
			//사운드
			if (UTILE.Get_Random(0f, 1f) < hitsndratio) {
				SND_IDX hitvocidx = m_TData.GetHitVoice();
				PlayEffSound(hitvocidx);
			}
			yield return new WaitForSeconds(0.3f);
		}
		else {
			//m_SUI.Anim.SetTrigger("1_Route");
			PVP.SetDmgStr(transform.position, PVPDmgType.Miss, 0, false);
			iTween.ScaleTo(gameObject, iTween.Hash("scale", new Vector3(0.9f, 0.9f, 0.9f), "time", 0.4f, "easetype", iTween.EaseType.easeOutCirc, "islocal", true));
			iTween.RotateTo(gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, -5f), "time", 0.4f, "easetype", iTween.EaseType.easeOutCirc, "islocal", true));

			yield return new WaitForSeconds(1f);

			iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "time", 0.2f, "easetype", iTween.EaseType.easeOutCirc, "islocal", true));
			iTween.RotateTo(gameObject, iTween.Hash("rotation", Vector3.zero, "time", 0.2f, "easetype", iTween.EaseType.easeOutCirc, "islocal", true));

			yield return new WaitForSeconds(0.2f);
		}
		
		_cb2?.Invoke();
		Is_Hit = false;
		yield return null;
	}
	public void SetAP(float _add, float _time = 0.5f) {
		if (m_State == State.Die) return;
		m_AP += _add;
		if (m_AP < 0f) m_AP = 0f;
		m_SUI.SkillAbleFX.SetActive(m_AP >= 1f);
		if (m_AP >= 1f && m_AP - _add < 1f) {
			m_SUI.SkillAbleFX.GetComponent<Animator>().SetTrigger("Able");
		}
		if (_time > 0f) _time = _add > 0f ? 0.8f : 0.5f;

		iTween.StopByName(gameObject, "SetAP");
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.APGauge.material.GetFloat("Fill_Amount"), "to", Mathf.Clamp(m_AP, 0f, 1f), "onupdate", "TW_APGauge", "onstart", "TW_APStart", "oncomplete", "TW_APEnd", "time", _time, "easetype", iTween.EaseType.easeOutCirc, "name", "SetAP"));
	}
	void TW_APGauge(float _amount) {
		m_SUI.APGauge.material.SetFloat("Fill_Amount", _amount);
	}
	void TW_APStart() {
		if (m_PosType != PVPPosType.Combat) return;
		m_SUI.Anim.SetTrigger("AP_Highlight");
	}
	void TW_APEnd() {
		if (m_PosType != PVPPosType.Combat) return;
		m_SUI.Anim.SetTrigger("AP_Idle");
	}
	public float GetAP() {
		return m_AP;
	}
	public float pt = 0f;
	public float GetPivotTime() {
		float spd = GetStat(StatType.Speed);
		pt = spd <= 0f ? 1f : (1f - m_AP) / spd / (1f - m_PSTData.m_SpeedCorrection);
		return pt;
	}
	public float SetHP(float _add, float _time = 0.3f) {
		if (m_State == State.Die) return 0;
		float diff = 0f;
		if (_add >= 0) diff = Mathf.Min(m_Stat[(int)StatType.HP, 1] - m_Stat[(int)StatType.HP, 0], _add);
		else diff = Mathf.Max(-m_Stat[(int)StatType.HP, 0], _add);
		m_Stat[(int)StatType.HP, 0] = Mathf.RoundToInt(m_Stat[(int)StatType.HP, 0] + diff);
		m_SUI.HP.text = Mathf.RoundToInt(m_Stat[(int)StatType.HP, 0]).ToString();

		iTween.StopByName(gameObject, "SetHP");
		float ratio = m_Stat[(int)StatType.HP, 0] <= 0 ? 0f : m_Stat[(int)StatType.HP, 0] / m_Stat[(int)StatType.HP, 1];
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.HPGauge.material.GetFloat("Fill_Amount"), "to", ratio, "onupdate", "TW_HPGauge", "onstart", "TW_HPStart", "oncomplete", "TW_HPEnd", "time", _time, "easetype", iTween.EaseType.easeOutCirc, "name", "SetHP"));

		if (m_Stat[(int)StatType.HP, 0] <= 0f) {
			List<SND_IDX> snds = new List<SND_IDX>() { SND_IDX.SFX_1320, SND_IDX.SFX_1321, SND_IDX.SFX_1322, SND_IDX.SFX_1310, SND_IDX.SFX_1311, SND_IDX.SFX_1312 };
			PlayEffSound(snds[UTILE.Get_Random((int)m_UserPos * 3, (int)m_UserPos * 3 + 3)]);
			SetDie();
			m_Partner.SetRun(m_Partner.IS_Run());
		}
		return diff;
	}
	void TW_HPGauge(float _amount) {
		m_SUI.HPGauge.material.SetFloat("Fill_Amount", _amount);
	}
	void TW_HPStart() {
		if (m_PosType != PVPPosType.Combat) return;
		m_SUI.Anim.SetTrigger("HP_Highlight");
	}
	void TW_HPEnd() {
		if (m_PosType != PVPPosType.Combat) return;
		m_SUI.Anim.SetTrigger("HP_Idle");
	}
	public void SetDie() {
		m_State = State.Die;
		if (m_PosType == PVPPosType.Combat) m_SUI.Anim.SetTrigger("Die");
		m_SUI.DieFX.SetActive(true);
		m_SUI.SkillAbleFX.SetActive(false);
		m_SUI.DebuffFX.SetActive(false);
	}
	public void SetRun(bool _run) {
		if (_run) m_State = State.Die;
		StartCoroutine(IE_RunAction(_run));
	}
	IEnumerator IE_RunAction(bool _run) {
		m_SUI.Anim.SetTrigger("1_Route");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		m_SUI.Anim.SetTrigger(_run ? "2_Run" : "3_Left");
		if (!_run) {
			GameObject fx = PVP.StartEff(transform.position, "Effect/PVP/Item_PVP_Char_Sup_LeftFX");
			fx.transform.localScale = Vector3.one * 0.35f;
			PlayEffSound(SND_IDX.SFX_1345);
		}
		else {
			if (UTILE.Get_Random(0f, 1f) > 0.5f) {
				List<SND_IDX> snds = new List<SND_IDX>() { SND_IDX.SFX_1320, SND_IDX.SFX_1321, SND_IDX.SFX_1322, SND_IDX.SFX_1330, SND_IDX.SFX_1331, SND_IDX.SFX_1332 };
				PlayEffSound(snds[UTILE.Get_Random((int)m_UserPos * 3, (int)m_UserPos * 3 + 3)]);
			}

			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

			gameObject.SetActive(false);
		}
	}
	/// <summary> 코드로 오브젝트 포지션, 스케일 조절 </summary>
	public void Call_TransAction(TransActionType _type, Vector3 _val = default(Vector3), float _sdelay = 0f, float _time = 0f, float _edelay = 0f, iTween.EaseType _ease = iTween.EaseType.linear) {
		if (!gameObject.activeSelf) return;
		StartCoroutine(TransAction(_type, _val, _sdelay, _time, _edelay, _ease));
	}
	public IEnumerator TransAction(TransActionType _type, Vector3 _val = default(Vector3), float _sdelay = 0f, float _time = 0f, float _edelay = 0f, iTween.EaseType _ease = iTween.EaseType.linear) {
		switch (_type) {
			case TransActionType.Move:
				yield return new WaitForSeconds(_sdelay);
				iTween.MoveTo(gameObject, iTween.Hash("position", _val, "time", _time, "easetype", _ease));
				yield return new WaitForSeconds(_edelay);
				break;
			case TransActionType.ScaleUpDown:
				yield return new WaitForSeconds(_sdelay);
				iTween.ScaleTo(gameObject, iTween.Hash("scale", _val, "time", _time, "easetype", _ease));
				yield return new WaitForSeconds(_edelay);
				break;
			case TransActionType.Atk:
				yield return new WaitForSeconds(_sdelay);
				switch (m_PSTData.m_Weapon) {
					case ItemType.Bow:
						break;
					case ItemType.Shotgun:
						break;
					case ItemType.Pistol:
						break;
					case ItemType.Rifle:
						break;
				}
				yield return new WaitForSeconds(_edelay);
				break;
		}
	}
	/// <summary> 공격자 테두리 표시 </summary>
	IEnumerator IE_AtkNowFxAction(bool _on) {
		if (_on) {
			m_SUI.NowAtkFX.gameObject.SetActive(true);
		}
		else {
			m_SUI.NowAtkFX.SetTrigger("End");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.NowAtkFX));
			m_SUI.NowAtkFX.gameObject.SetActive(false);
		}
	}
	bool IS_Melee() {
		switch (m_PSTData.m_Weapon) {
			case ItemType.None:
			case ItemType.Blunt:
			case ItemType.Blade:
			case ItemType.Axe:
				return true;
			default: return false;
		}
	}
	bool IS_AtkMove() {
		switch (m_PSTData.m_Weapon) {
			case ItemType.Blunt:
			case ItemType.Blade:
			case ItemType.Axe:
			case ItemType.Shotgun:
			case ItemType.Rifle:
				return true;
			default: return false;
		}
	}
}
