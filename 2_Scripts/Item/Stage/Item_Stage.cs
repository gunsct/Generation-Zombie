using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public enum EItem_Stage_Card_Action
{
	None = 0,
	Move,
	Scale,
	TargetOn,
	TargetOff,
	FadeIn,
	FadeOut,
	Atk,
	Damage,
	Change,
	ChangeEnemyMat,
	ChangeVal,
	RefreshImgName,
	MoveChange,
	Die,
	MoveTarget,
	DarkCardOn,
	DarkCardOff,
	Get,
	Lock,
	UnLock,
	DissolveGet
}
public enum EItem_Stage_Card_FrontAction
{
	None = 0,
	/// <summary> 디졸브로 현재이미지 생성 </summary>
	Dissolve_In,
	/// <summary> 디졸브로 전이미지 제거하기 </summary>
	Dissolve_Out,
	/// <summary> 디졸브로 아이템 생성 </summary>
	Item_In,
}

public enum EDIR
{
	UP = 0,
	LEFT,
	DOWN,
	RIGHT,
	UP_LEFT,
	DOWN_LEFT,
	DOWN_RIGHT,
	UP_RIGHT,
	NONE
}

public class Item_Stage : ObjMng
{
	public enum MakingState
	{
		/// <summary> 체크하지 않고 넘어가기 </summary>
		None = 0,
		/// <summary> 이동이든 변환이든 된상태 체크 대상 </summary>
		Check,
		/// <summary> 합쳐질 대상 재료 </summary>
		Target,
		End
	}
	[System.Serializable]
	public struct SMoveDirUI
	{
		public GameObject Active;
		public Animator Ani;
		public Transform Arrow;
		public string Lastani;
	}

	[System.Serializable]
	public class SStageUI
	{
		public GameObject Active;
		public SpriteRenderer GradeMark;
		public RenderAlpha_Controller[] GradeAlpha;
		public SortingGroup Group;
		public StageCardShaderController Icon;
		public TextMeshPro Name;
		public Sprite StatDownImg;
	}
	[Serializable]
	public struct SEnemyTypeUI
	{
		public GameObject Deadly;
		public SpriteRenderer DeadlyImg;
		public GameObject Range;
		public GameObject Multi;
		public GameObject Stat;
		public SpriteRenderer StatImg;
	}
	[System.Serializable]
	public struct SLampDarkUI
	{
		public GameObject[] NotLamps;
		public GameObject Lamp;
	}
	[SerializeField] Transform m_MovePanel;
	[SerializeField] BoxCollider m_Collider;
	[SerializeField] SStageUI[] m_Stage;
	[SerializeField] SMoveDirUI m_MoveMark;
	[SerializeField] Sprite[] m_HPBImg;
	[SerializeField] Sprite[] m_HPNImg;
	[SerializeField] Sprite[] m_HPFrameImg;
	[SerializeField] GameObject m_LimitTurnGroup;
	[SerializeField] TextMeshPro[] m_LimitTurn;
	[SerializeField] Animator m_CardAnim;
	[SerializeField] Sprite m_DarkDefaultImg;
	[SerializeField, ColorUsage(true, true)] Color[] m_DissolveColor;
	[SerializeField] RenderAlpha_Controller m_Alpha;
	[SerializeField] GameObject m_ItemImg;
	[SerializeField] GameObject m_StreetLampDark;
	[SerializeField] SLampDarkUI m_SLDUI;
	[SerializeField] Sprite[] m_FrameImg;
	[SerializeField] Sprite[] m_MaskImg;
	[SerializeField] TextMeshProEffect[] m_TMPFX;
	[SerializeField] GameObject m_ReduceStatEnemyFX;
	[SerializeField] GameObject m_DarkPatrolMark;
	[SerializeField] GameObject m_SaveUtileFX;
	[SerializeField] GameObject m_TouchGuideFX;
	[SerializeField] GameObject m_CardUseFX;
	[SerializeField] SEnemyTypeUI m_SETUI;
	public SortingGroup m_Sort;
	public StageCardInfo m_Info;

	public SStageUI NowCard { get { return m_Stage[m_ViewCardPos]; } }
	/// <summary> 밝혀진 상태인지 </summary>
	public bool m_IsLight {
		set { m_Info.IsLight = value; }
		get { return m_Info.IsLight; }
	}

	IEnumerator m_Action = null;
	IEnumerator m_AccentBumpAction = null;
	/// <summary> 액션이 끝났는지 </summary>
	public bool IS_NoAction { get { return m_Action == null; } }

	int m_StageChangeIdx = 0;			//어둠에서 원래카드로 바뀔때 등
	int m_EnemyChangeIdx = 0;			//카드가 에너미로 바뀔때
	int m_EnemyMatChagneIdx = 0;        //적이 재료카드로 바뀔때

	int m_ViewCardPos = 0;				//카드 오브제 앞뒤,0,1
	int m_InitLine, m_InitPos;			//초기화 위치
	public int m_MoveLine, m_MovePos;   //가상으로 이동한 자리 이동체크가 끝난후에는 다시 돌려줌

	Vector3[] m_PanelTransInit = new Vector3[3];
	public int m_Line {
		set { m_InitLine = m_MoveLine = value; }
		get { return m_MoveLine; }
	}
	public int m_Pos {
		set { m_InitPos = m_MovePos = value; }
		get { return m_MovePos; }
	}
	public bool IS_FadeIn = true;

	public MakingState m_MakingState;

	public Vector3 ImgSize { get { return m_Stage[m_ViewCardPos].Icon.transform.lossyScale; } }

	//아래 두개는 3,4 매치 등 쓰지는 않고 남겨둔 코드용
	public bool IS_MakingTarget { get { return m_MakingState == MakingState.Target; } }
	public bool IS_MakingMaterial(StageMaterialType type) { return !IS_MakingTarget && (StageMaterialType)Mathf.RoundToInt(m_Info.m_TData.m_Value1) == type; }

	/// <summary> 공격할 대상 (추적용) </summary>
	public Item_Stage m_Target;

	/// <summary> 체인 카드 죽을때 이펙트 시작 위치 </summary>
	public float? m_ChainDieEffPosX;
	/// <summary> 공격턴에 공격을 했는지 유무 </summary>
	public bool ISAtk;
	/// <summary> 이동을 했는지 유무 </summary>
	public bool ISMove;
	/// <summary> 캐릭터 일대일 전투로 죽었는지 </summary>
	public bool IS_KilledDuel;
	/// <summary> 락모드로 잠겨있는가 </summary>
	public bool IS_Lock;
	/// <summary> 락 이펙트 </summary>
	public GameObject m_LockFX;
	/// <summary> 가로등으로만 밝혀지지 않은건지 </summary>
	public bool IS_NotOnlyStreetLight;

	public bool IS_Awake = false;
	List<SND_IDX> m_HitSND = new List<SND_IDX>();
	/// <summary> 적타입 활성 트리거, 애니메이터에서 이름으로 체크가 안됌 </summary>
	string m_EnemyTypeTrig = string.Empty;
	public bool IS_CanTouch;
	/// <summary> 활성화 된 카드인지 </summary>
	public bool ISActiveCard() {
		return IS_CanTouch;
		//return m_Collider.enabled;
	}
	/// <summary> 죽은 에너미인지 </summary>
	public bool IS_Die() {
		return m_Info.IS_EnemyCard && m_Info.GetStat(EEnemyStat.HP) < 1;
	}
	/// <summary> 변환 예정인 카드인지 </summary>
	public bool IS_ChangeCard() {
		return m_EnemyChangeIdx > 0 || m_StageChangeIdx > 0 || m_EnemyMatChagneIdx > 0;
	}
	/// <summary> 죽어라 재료로 바뀔 적인지 </summary>
	public bool IS_ChangeMatDieEnemyCard() {
		return m_EnemyMatChagneIdx > 0;
	}
	/// <summary> AI패턴이 가능 상태인 적인지 </summary>
	public bool IS_AIEnemy()
	{
		StageCardInfo info = m_Info;
		if (!info.IS_AIEnemy()) return false;
		if (info.IsDarkCard) return false;
		if (IS_Die()) return false;
		return true;
	}
	/// <summary> 가로등 효과 온오프 </summary>
	public void SetStreetLightShadow(bool _on) {
		if (_on && m_StreetLampDark.activeSelf) return;
		if (!_on && !m_StreetLampDark.activeSelf) return;

		m_StreetLampDark.SetActive(_on);

		for (int i = 0;i< m_SLDUI.NotLamps.Length; i++) {
			Utile_Class.ChangeLayer(m_SLDUI.NotLamps[i].transform, m_StreetLampDark.activeSelf ? 7 : 0);
		}

		Utile_Class.ChangeLayer(m_SLDUI.Lamp.transform, m_StreetLampDark.activeSelf ? 0 : 7);
	}
	/// <summary> 어둠 상태일때 스킬등으로 위치 표시 </summary>
	public void SetDarkPatrolMark(bool _on) {
		m_DarkPatrolMark.SetActive(_on);
	}
	private void Awake() {
		IS_Awake = true;
		SetData();

		for (int i = 0; i < 2;i++) m_Stage[i].Icon.SetColor("_DarkColor", new Color(0f, 0f, 0f, 0.5f));
		IS_CanTouch = true;
		//m_Collider.enabled = true;
		ActiveDark(true);
		for (int i = 0; i < m_TMPFX.Length; i++) m_TMPFX[i].enabled = false;
		m_ReduceStatEnemyFX.SetActive(false);
	}

	private void OnEnable() {
		m_Sort.enabled = true;
		for (int i = 0; i < 2; i++) m_Stage[i].Group.enabled = true;
	}

	private void OnDisable() {
		m_Sort.enabled = false;
		for (int i = 0; i < 2; i++) m_Stage[i].Group.enabled = false;
	}
	private void Init() {
		m_MakingState = MakingState.Check;
		m_ChainDieEffPosX = null;
		m_IsLight = false;
		m_LimitTurnGroup.SetActive(false); 
		InitCardAnim();
		ActiveMoveMark(false);

		iTween.Stop(gameObject);
		for (int i = m_Stage.Length - 1; i > -1; i--) {
			SStageUI ui = m_Stage[i];
			ui.Icon.Init();
			ui.Icon.SetColor("_DissolveColor", m_DissolveColor[0]);
			ui.Icon.SetColor("_Color", Color.white);
		}

		m_MovePanel.localPosition = Vector3.zero;

		m_ViewCardPos = 0;
		m_Stage[m_ViewCardPos].Active.SetActive(true);
		m_Stage[1 - m_ViewCardPos].Active.SetActive(false);
		for(int i = 0; i < 2; i++) {
			m_Stage[i].Icon.Active_HP(false);
			m_Stage[i].Icon.SetStatActive(false);
		}
		ActiveDark(true);
		IS_FadeIn = true;
		SetFadeAlpha(1f);
		IS_KilledDuel = false;
		IS_Lock = false;
		IS_NotOnlyStreetLight = false;
		if (m_LockFX) Destroy(m_LockFX);
		m_ItemImg.SetActive(false);
		SetStreetLightShadow(false);
		for (int i = 0; i < m_TMPFX.Length; i++) m_TMPFX[i].enabled = false;
		TW_ScaleBumping(false);
		SetAccent(false);
		DarkEnemyShakeCheck(false);
		FlashingDark(false);
		SetDarkPatrolMark(false);
		SetDeadlyMark(false);
		ActiveSaveUtileFX(false);
		ActiveTouchGuide(false);
		ActiveCardUse(false);
		m_HitSND.Clear();
		m_EnemyTypeTrig = string.Empty;
	}

	public void SetData(StageCardInfo info) {
		m_Info = info;
		m_Info.InitVal();
		SetData();
	}

	void SetData()
	{
		if (!IS_Awake) return;
		Init();
		StopAllCoroutines();
		for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetColor("_DarkColor", new Color(0f, 0f, 0f, 0.5f));
		SetFrontImg(m_Info.GetImg(), m_Info.GetName());
		DarkEnemyShakeCheck(true);
		SetReduceStatFX(true);
		SetTurn();
		SetStatMark();
		ActiveDeadlyMark(true);
		ActiveSaveUtileFX(true);
		SetEnemyType();
		SetBG();
	}

	public void SetCardAnim(string _name) {
		m_CardAnim.SetTrigger(_name);
	}
	void InitCardAnim() {
		m_CardAnim.SetTrigger("Normal");
		m_CardAnim.SetTrigger("Pos_Normal");
	}
	void ActiveStatMark(bool Active) {
		if (Active) Active = m_Info.ISRefugee;// && m_IsLight;
		if (Active && m_Info.IsDark) Active = false;
		for(int i = 0;i<2;i++) m_Stage[i].Icon.SetStatActive(Active);
	}
	/// <summary> 피난민의 생존스탯 표기 </summary>
	void SetStatMark() {
		ActiveStatMark(true);
		if (!m_Info.ISRefugee) return;
		TEnemyTable enemydata = m_Info.m_TEnemyData;
		Sprite sprite = BaseValue.GetStatMark(enemydata.m_Type, enemydata.m_RewardGID);
		for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetStat(true, sprite, enemydata.IS_BadRefugee() ? m_Stage[i].StatDownImg : null);
	}
	/// <summary> 즉사형 에너미 표기 </summary>
	void ActiveDeadlyMark(bool Active) {
		if (Active) Active = m_Info.IS_EnemyCard;// && m_IsLight;
		if (m_Info.IsDark) Active = false;
		SetDeadlyMark(m_Info.m_TEnemyData == null ? false : (m_Info.m_TEnemyData.m_Deadly && Active));
		if (m_SETUI.Deadly.activeSelf) {
			TEnemyStageSkillTable atkdata = TDATA.GetEnemyStageSkillTableGroup(m_Info.m_TEnemyData.m_Skill.m_GroupID);
			m_SETUI.DeadlyImg.sprite = UTILE.LoadImg(string.Format("UI/Icon/{0}", atkdata.m_Range < 1 ? "Mark_Death" : "Mark_Death_3"), "png");
		}
	}
	void SetDeadlyMark(bool _active) {
		m_SETUI.Deadly.SetActive(_active);
	}
	/// <summary> 머지탭으로 저장되는 유틸카드 표기 </summary>
	void ActiveSaveUtileFX(bool _active = true) {
		if (_active) m_SaveUtileFX.SetActive(!m_Info.IsDark && m_Info.m_TData.m_Type == StageCardType.SaveCard);
		else m_SaveUtileFX.SetActive(_active);
	}
	public void ActiveTouchGuide(bool _active = true) {
		m_TouchGuideFX.SetActive(_active);
	}
	public void ActiveCardUse(bool _active = true) {
		m_CardUseFX.SetActive(_active);
	}
	/// <summary> 에너미 성향 표시 </summary>
	public void SetEnemyType() {
		m_SETUI.Multi.SetActive(false);
		m_SETUI.Range.SetActive(false);
		m_SETUI.Stat.SetActive(false);

		if (m_Info.IS_EnemyCard && !m_Info.IsDark && !IS_Die()) {
			List<GameObject> active = new List<GameObject>();
			TEnemyTable tdata = m_Info.m_TEnemyData;
			TEnemyStageSkillTable atkdata = TDATA.GetEnemyStageSkillTableGroup(tdata.m_Skill.m_GroupID);
			if (atkdata == null) return;
			string trig = m_Line <= atkdata.m_Range ? "On" : "Off";

			if (m_Info.m_TEnemyData.m_Deadly) {
				if (!m_EnemyTypeTrig.Equals(trig)) {
					m_SETUI.Deadly.transform.GetComponent<Animator>().SetTrigger(trig);
					m_EnemyTypeTrig = trig;
				}
				}
			else {
				if (atkdata.m_Range > 0) {
					m_SETUI.Range.SetActive(true);
					if (!m_EnemyTypeTrig.Equals(trig) && m_SETUI.Range.gameObject.activeInHierarchy) {
						m_SETUI.Range.transform.GetComponent<Animator>().SetTrigger(trig);
						m_EnemyTypeTrig = trig;
					}
						active.Add(m_SETUI.Range);
				}
				switch (atkdata.m_ATKType) {
					//다중 공격
					case ESkillATKType.MultiHit:
					case ESkillATKType.MultiBite:
					case ESkillATKType.MultiAttack:
					case ESkillATKType.MultiSlash:
					case ESkillATKType.MultiScratch:
					case ESkillATKType.ZombieMultiBite:
					case ESkillATKType.ZombieMultiSpit:
						m_SETUI.Multi.SetActive(true);
						if (!m_EnemyTypeTrig.Equals(trig) && m_SETUI.Multi.gameObject.activeInHierarchy) {
							m_SETUI.Multi.transform.GetComponent<Animator>().SetTrigger(trig);
							m_EnemyTypeTrig = trig;
						}
							active.Add(m_SETUI.Multi);
						break;
				}
				//생존 스탯 공격
				for (StatType i = StatType.Men; i < StatType.SurvEnd; i++) {
					if (!STAGE_USERINFO.Is_UseStat(i)) continue;
					float val = atkdata.Get_SrvDmg(i);
					if (val > 0f) {
						switch (i) {
							case StatType.Men:
								m_SETUI.StatImg.sprite = UTILE.LoadImg("UI/Icon/Stage_EnemyType_3", "png");
								break;
							case StatType.Hyg:
								m_SETUI.StatImg.sprite = UTILE.LoadImg("UI/Icon/Stage_EnemyType_4", "png");
								break;
							case StatType.Sat:
								m_SETUI.StatImg.sprite = UTILE.LoadImg("UI/Icon/Stage_EnemyType_5", "png");
								break;
						}
						m_SETUI.Stat.SetActive(true);
						if (!m_EnemyTypeTrig.Equals(trig) && m_SETUI.Stat.gameObject.activeInHierarchy) {
							m_SETUI.Stat.transform.GetComponent<Animator>().SetTrigger(trig);
							m_EnemyTypeTrig = trig;
						}
						active.Add(m_SETUI.Stat);
						break;
					}
				}

				if (active.Count == 1) {
					active[0].transform.localPosition = new Vector3(0f, 0.056f, 0f);
				}
				else if (active.Count == 2) {
					active[0].transform.localPosition = new Vector3(-0.6f, 0.056f, 0f);
					active[1].transform.localPosition = new Vector3(0.6f, 0.056f, 0f);
				}
				else if (active.Count == 3) {
					active[0].transform.localPosition = new Vector3(-1.2f, 0.056f, 0f);
					active[1].transform.localPosition = new Vector3(0f, 0.056f, 0f);
					active[2].transform.localPosition = new Vector3(1.2f, 0.056f, 0f);
				}
			}
		}
	}
	/// <summary> 시한폭탄 등 시간 표기 </summary>
	public void SetTurn(bool _chagne = false) {
		bool Active = !m_Info.IsDark && (m_Info.m_TData.m_Type == StageCardType.TimeBomb || m_Info.m_Turn > 0);

		m_LimitTurnGroup.SetActive(Active);
		if (m_LimitTurnGroup.activeSelf) {
			for (int i = 0; i < m_LimitTurn.Length; i++) m_LimitTurn[i].text = m_Info.m_Turn.ToString();

			string trig = "Normal";
			if (m_Info.m_Turn < 2) trig = "Danger";
			else if (m_Info.m_Turn < 3) trig = "Alert";

			SetCardAnim(trig);
			if(_chagne) SetCardAnim("Change");
		}
	}
	/// <summary> 체력바 세팅 </summary>
	public void SetHPBar() {
		if (m_Info.IsDark) {
			SetHP(false, true);
			return;
		}
		SetHP(true, false);
	}
	public void RefreshHP() {
		SetHP(true, true);
	}
	/// <summary> 체력 설정 </summary>
	public void SetHP(bool _use, bool Init = false) {
		int MaxHP = m_Info.GetMaxStat(EEnemyStat.HP);
		int HP = m_Info.GetStat(EEnemyStat.HP);
		if (!m_Info.IS_EnemyCard || HP == MaxHP || m_Info.IsDark) {
			for (int i = 0; i < 2; i++) m_Stage[i].Icon.Active_HP(false);
			return;
		}
		iTween.StopByName(gameObject, "HPGauge");

		for (int i = 0; i < 2; i++) m_Stage[i].Icon.Active_HP(true);
		float amount = (float)HP / (float)MaxHP;
		int pos = m_Info.ISRefugee ? 0 : 1;

		for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetHPImg(m_HPFrameImg[pos], m_HPBImg[pos], m_HPNImg[pos]);
		for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetHPAmount(amount);

		float Now = NowCard.Icon.GetFloat("_HPBackAmount");
		if (Init) {
			for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetHPBackAmount(amount);
		}
		else {
			if (Now > amount) iTween.ValueTo(gameObject, iTween.Hash("from", Now, "to", amount, "time", 1f, "easetype", "easeInCubic", "onupdate", "TW_HPBackAmount", "name", "HPGauge"));
			else
				for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetHPBackAmount(amount);
		}
	}
	void TW_HPBackAmount(float _amount) {
		for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetHPBackAmount(_amount);
	}
	void SetBG() {
		for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetFloat("_BGMask", m_Info.IS_RoadBlock ? 0 : 1);
	}
	/// <summary> 첫줄 카드 등 강조 효과 </summary>
	public void SetAccent(bool _on) {
		if (m_AccentBumpAction != null) StopCoroutine(m_AccentBumpAction);
		AccentPower(1f);
		if (_on) {
			m_AccentBumpAction = IE_AccentBump(true, 0.92f, 1.7f);
			StartCoroutine(m_AccentBumpAction);
		}
	}
	IEnumerator IE_AccentBump(bool _on, float _min, float _max) {
		bool on = _on;
		float power = m_Stage[0].Icon.GetFloat("_Power");
		if (on) {
			power = Mathf.Clamp(power + Time.deltaTime * 0.75f, _min, _max);
			if (power >= _max) on = false;
		}
		else {
			power = Mathf.Clamp(power - Time.deltaTime * 0.75f, _min, _max);
			if (power <= _min) on = true;
		}
		AccentPower(power);

		yield return new WaitForEndOfFrame();

		m_AccentBumpAction = IE_AccentBump(on, _min, _max);
		StartCoroutine(m_AccentBumpAction);
	}
	void AccentPower(float _amount) {
		for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetFloat("_Power", _amount);
	}
	/// <summary> 카드 유형별 프레임 </summary>
	Sprite SetFrame() {
		if (!m_Info.IsDark && m_Info.m_NowTData.IS_NotFrameCard()) {
			return m_FrameImg[3];
		}
		else if (!m_Info.IsDark && m_Info.IS_AutoGet()) {
			return m_FrameImg[1];
		}
		else if(!m_Info.IsDark && m_Info.m_TData.m_Type == StageCardType.SaveCard) {
			return m_FrameImg[2];
		}
		else {
			return m_FrameImg[0];
		}
	}
	public void SetFrame(int _idx) {
		m_Stage[m_ViewCardPos].Icon.SetFrame(m_FrameImg[_idx]);
		if(_idx != 3) m_Stage[m_ViewCardPos].Icon.SetFrameMask(m_MaskImg[_idx]);
	}
	/// <summary> 카드 유형별 마스크 </summary>
	Sprite SetMask() {
		if (!m_Info.IsDark && m_Info.IS_AutoGet()) {
			return m_MaskImg[1];
		}
		else if (!m_Info.IsDark && m_Info.m_TData.m_Type == StageCardType.SaveCard) {
			return m_MaskImg[2];
		}
		else return m_MaskImg[0];
	}
	/// <summary> 카드간 거리 측정 </summary>
	public int GetDis(Item_Stage target) {
		return GetTempDis(target, m_Line, m_Pos);
	}

	public int GetTempDis(Item_Stage target, int MyLine, int MyPos) {
		int linegap = target.m_Line - MyLine;
		int linepos = MyPos + linegap;
		return Math.Abs(linegap) + Math.Abs(target.m_Pos - linepos);
	}

	public bool CheckTargetAtkDis(Item_Stage target) {
		return CheckPosAtkDis(target, m_Line, m_Pos);
	}
	public bool CheckPosAtkDis(Item_Stage target, int MyLine, int MyPos) {
		if (target == null) return false;
		if (GetTempDis(target, MyLine, MyPos) > m_Info.m_TEnemyData.m_AtkAI.m_Values[0]) return false;
		return IS_CrosLineCard(target, MyLine, MyPos);
	}

	public bool IS_CrosLineCard(Item_Stage target) {
		return IS_CrosLineCard(target, m_Line, m_Pos);
	}

	public bool IS_CrosLineCard(Item_Stage target, int MyLine, int MyPos) {
		if (target == null) return false;
		// 4방향만 가능
		// 같은 라인
		// 가로 라인
		if (MyLine == target.m_Line) return true;
		// 세로 라인
		if (MyPos + (target.m_Line - MyLine) == target.m_Pos) return true;
		return false;
	}

	public bool IS_CrosLineCard(int targetline, int targetpos) {
		// 4방향만 가능
		// 같은 라인
		// 가로 라인
		if (m_Line == targetline) return true;
		// 세로 라인
		if (m_Pos + (targetline - m_Line) == targetpos) return true;
		return false;
	}

	public EDIR GetTargetDir(Item_Stage target) {
		EDIR Dir = EDIR.NONE;
		if (IS_CrosLineCard(target, m_Line, m_Pos)) {
			if (m_Line == target.m_Line) {
				Dir = target.m_Pos - m_Pos > 0 ? EDIR.RIGHT : EDIR.LEFT;
			}
			else {
				Dir = target.m_Line - m_Line > 0 ? EDIR.UP : EDIR.DOWN;
			}
		}

		return Dir;
	}
	public EDIR GetTargetDir(int line, int pos) {
		EDIR Dir = EDIR.NONE;
		if (IS_CrosLineCard(line, pos)) {
			if (m_Line == line) {
				Dir = pos - m_Pos > 0 ? EDIR.RIGHT : EDIR.LEFT;
			}
			else {
				Dir = line - m_Line > 0 ? EDIR.UP : EDIR.DOWN;
			}
		}

		return Dir;
	}

	public EDIR Get_FlipDir(EDIR dir) {
		//if (dir == EDIR.NONE) return EDIR.NONE;
		//int tamp = (int)dir + 2 % 5;
		//return (EDIR)tamp;
		switch (dir) {
			case EDIR.LEFT: return EDIR.RIGHT;
			case EDIR.RIGHT: return EDIR.LEFT;
			case EDIR.UP: return EDIR.DOWN;
			case EDIR.DOWN: return EDIR.UP;
		}
		return EDIR.NONE;
	}

	/// <summary> 앞면 설정하기 </summary>
	/// <param name="Img"> 앞면이될 이미지 </param>
	/// <param name="Name"> 앞면이될 이름 </param>
	/// <param name="FrontAction"> EItem_Stage_Card_FrontAction </param>
	public void SetFrontImg(Sprite Img, string Name, EItem_Stage_Card_FrontAction FrontAction = EItem_Stage_Card_FrontAction.None, float time = 0f, int colorPos = 0) {
		int pos = 1 - m_ViewCardPos;
		if (m_Info.IS_EnemyCard) {
			m_Stage[pos].Name.text = Name;
		}
		m_Stage[pos].Icon.SetCard(Img);
		SetDarkImg(m_Info.m_NowTData.m_Type);
		m_Stage[pos].Icon.SetFrame(SetFrame());
		m_Stage[pos].Icon.SetFrameMask(SetMask());
		m_Stage[pos].Name.text = Name;
		m_Stage[pos].Name.GetComponent<RenderAlpha_Controller>().Alpha = 1f;
		m_Stage[pos].Name.gameObject.SetActive(m_Info.IsDark || !m_Info.m_RealTData.IS_NotFrameCard());
		m_Stage[pos].Icon.SetColor("_DissolveColor", m_DissolveColor[colorPos]);
		m_Stage[pos].Icon.SetFloat("_DissolveValue", 0f);
		for (int i = 0; i < m_Stage[pos].GradeAlpha.Length; i++) {
			m_Stage[pos].GradeAlpha[i].SetAlpha(1);
			m_Stage[pos].GradeAlpha[i].gameObject.SetActive(m_Info.IsGradeMark);
		}

		switch (FrontAction) {
			case EItem_Stage_Card_FrontAction.None:
				m_Stage[pos].Active.SetActive(true);
				m_Stage[pos].Group.sortingOrder = m_Line == 0 ? 4 : 2;
				m_Stage[m_ViewCardPos].Active.SetActive(false);
				m_ViewCardPos = pos;
				SetViewCardDissolveValue(0);
				break;
			case EItem_Stage_Card_FrontAction.Dissolve_Out:
				m_Stage[pos].Active.SetActive(true);
				m_Stage[pos].Group.sortingOrder = m_Line == 0 ? 4 : 1;
				m_Stage[m_ViewCardPos].Active.SetActive(true);
				m_Stage[m_ViewCardPos].Group.sortingOrder = m_Line == 0 ? 4 : 2;

				if (IS_ScreenCard()) {
					SetViewCardDissolveValue(0f);
					iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", time, "easetype", "easeOutCubic", "onupdate", "SetViewCardDissolveValue", "name", "SetViewCardDissolveValue"));
				}
				else SetViewCardDissolveValue(1f);
				break;
		}
		SetHPBar();
	}
	public void MovePositionInit() {
		m_Line = m_InitLine;
		m_Pos = m_InitPos;
		ISMove = false;
	}

	public void SetPos(int line, int pos) {
		m_Sort.sortingOrder = line == 0 ? 3 : line * -1;
		m_Line = line;
		m_Pos = pos;
#if UNITY_EDITOR
		gameObject.name = string.Format("{0}_{1}_Stage", m_Line, m_Pos);
#endif
	}
	public void SetVirtualPos(int line, int pos) {
		m_MoveLine = line;
		m_MovePos = pos;
	}

	public void SetTarget(Item_Stage target) {
		m_Target = target;
	}
	/// <summary> AI 이동방향 표기 </summary>
	void ActiveMoveMark(bool Active) {
		if (!m_Info.IS_EnemyCard) Active = false;
		if (m_Info.IsDarkCard) Active = false;
		if (m_Target == null) Active = false;
		if (Active) {
			//Active = m_Info.m_RepeatCnt[1] > 0;
			m_MoveMark.Active.SetActive(Active);
			if (Active) {
				m_MoveMark.Ani.SetTrigger(m_MoveMark.Lastani);
			}
		}
		else m_MoveMark.Active.SetActive(Active);
	}

	public void ViewTargetPos() {
		List<StageMng.Navipos> navis = STAGE.Navigation(this, m_Target);
		bool Active = navis.Count > 1;
		EDIR dir = EDIR.NONE;
		float gap = 0f;
		if (Active) {
			StageMng.Navipos NextPos = navis[1];
			dir = GetTargetDir(NextPos.L, NextPos.P);
			// 이동 경로가 겹치는 경우는 붙어있는 경우뿐이다 (서로 공격일때만 가능)
			List<StageMng.Navipos> targetnavis = STAGE.Navigation(m_Target, m_Target.m_Target);
			if (targetnavis.Count > 1 && targetnavis[1].L == m_Line && targetnavis[1].P == m_Pos) gap = 0.6f;
		}
		Active = Active && dir != EDIR.NONE;

		if (Active) {
			string Trigger = m_Info.m_TEnemyData.ISAtkMoveType() ? "Enemy" : "Ally";
			m_MoveMark.Arrow.localPosition = new Vector3(gap, 0f, 0f);
			switch (dir) {
				case EDIR.UP:
					Trigger = string.Format("{0}_T", Trigger);
					break;
				case EDIR.LEFT:
					Trigger = string.Format("{0}_L", Trigger);
					break;
				case EDIR.DOWN:
					Trigger = string.Format("{0}_B", Trigger);
					break;
				default:
					Trigger = string.Format("{0}_R", Trigger);
					break;
			}
			m_MoveMark.Lastani = Trigger;
		}
		ActiveMoveMark(Active);
	}
	/// <summary> 어둠 활성 </summary>
	public void ActiveDark(bool Active, bool isTarget = false) {
		DarkInit();
		iTween.StopByName(gameObject, "Flashing");
		IS_CanTouch = isTarget;
		//m_Collider.enabled = isTarget;
		for (int i = 0; i < 2; i++) m_Stage[i].Icon.ActiveShadow(Active);
	}
	/// <summary> 선택가능한 카드의 딤을 점멸시킴 </summary>
	public void FlashingDark(bool _flashing) {
		if (_flashing) {
			for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetColor("_DarkColor", new Color(0f, 0f, 0f, 0f));
			iTween.ColorTo(gameObject, iTween.Hash("a", 1f, "time", 2f, "looptype", "pingpong", "name", "Flashing"));
		}
		else DarkInit();
	}
	void DarkInit() {
		iTween.StopByName(gameObject, "Flashing");
		for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetColor("_DarkColor", new Color(0f, 0f, 0f, 0.5f));
	}
	/// <summary> 카드 유형별 어둠 이미지 </summary>
	void SetDarkImg(StageCardType _type) {
		if (m_Info.IsDark) {
			for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetShadowImg(m_DarkDefaultImg);
			return;
		}
		switch (_type) {
			case StageCardType.Roadblock:
			case StageCardType.AllRoadblock:
				for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetShadowImg(m_Info.GetRealImg());
				break;
			default:
				for (int i = 0; i < 2; i++) m_Stage[i].Icon.SetShadowImg(m_DarkDefaultImg);
				break;
		}
	}
	void SetFadeAlpha(float _amount) {
		m_Alpha.Alpha = _amount;
		if (_amount < 1f) m_LimitTurnGroup.SetActive(false);
		else SetTurn();
	}

	void SetViewCardDissolveValue(float _amount) {
		float alpha = 1f - _amount;
		if (alpha < 1f) m_LimitTurnGroup.SetActive(false);
		else SetTurn();
		m_Stage[m_ViewCardPos].Name.GetComponent<RenderAlpha_Controller>().Alpha = alpha;
		m_Stage[m_ViewCardPos].Icon.SetFloat("_DissolveValue", _amount);
		if (m_Info.IsGradeMark) {
			for (int i = 0; i < m_Stage[m_ViewCardPos].GradeAlpha.Length; i++) {
				m_Stage[m_ViewCardPos].GradeAlpha[i].Alpha = 1 - _amount;
			}
		}
	}

	/// <summary> 해당 카드가 스크린 좌표안에 있는지 확인하기 </summary>
	public bool IS_ScreenCard() {
		Vector3 size = m_Collider.size * transform.lossyScale.x * 0.5f;
		Vector3 LEFT = transform.right * -size.x;
		Vector3 UP = transform.up * size.y;
		Vector3 RIGHT = transform.right * size.x;
		Vector3 DOWN = transform.up * -size.y;

		Vector2 LUPOS = Utile_Class.GetCanvasPosition(transform.position + LEFT + UP);
		Vector2 RDPOS = Utile_Class.GetCanvasPosition(transform.position + RIGHT + DOWN);
		// 모서리의 한점이라도 스크린 좌표로 들오면 됨
		return IS_PosintScreen(LUPOS.x, LUPOS.y) || IS_PosintScreen(LUPOS.x, RDPOS.y) || IS_PosintScreen(RDPOS.x, LUPOS.y) || IS_PosintScreen(RDPOS.x, RDPOS.y);
	}

	bool IS_PosintScreen(float x, float y) {
		return x >= 0 && x <= Canvas_Controller.BASE_SCREEN_WIDTH && y >= 0 && y <= Canvas_Controller.BASE_SCREEN_HEIGHT;
	}

	/// <summary> 어둠상태인 에너미 카드가 잠입 모드일때 사이즈 쉐이킹</summary>
	void DarkEnemyShakeCheck(bool _on = false) {
		if (!_on) {
			if (IsInvoking("DarkEnemyShake")) {
				CancelInvoke("DarkEnemyShake");
				for (int i = 1; i < 6; i++) iTween.StopByName(m_MovePanel.gameObject, string.Format("DarkEnemyShake{0}", i));
				TW_DarkEnemyInit();
			}
		}
		else {
			if (STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.TurmoilCount && STAGEINFO.m_TStage.m_Fail.m_Value == 1 && m_Info.m_RealTData.m_Type == StageCardType.Enemy && m_Info.IsDark) {
				m_PanelTransInit[0] = m_MovePanel.localScale;
				m_PanelTransInit[1] = m_MovePanel.localPosition;
				m_PanelTransInit[2] = m_MovePanel.localEulerAngles;

				InvokeRepeating("DarkEnemyShake", 0f, UTILE.Get_Random(5f, 7f));
			}
			else {
				if (IsInvoking("DarkEnemyShake")) {
					CancelInvoke("DarkEnemyShake");
					for (int i = 1; i < 6; i++) iTween.StopByName(m_MovePanel.gameObject, string.Format("DarkEnemyShake{0}", i));
					TW_DarkEnemyInit();
				}
			}
		}
	}
	/// <summary> 어둠 상태인 에너미 카드 흔드는 연출 </summary>
	public void DarkEnemyShake() {
		TW_DarkEnemyInit();
		Vector3 scale = m_MovePanel.localScale;
		Vector3 pos = m_MovePanel.localPosition;

		iTween.ScaleTo(m_MovePanel.gameObject, iTween.Hash("scale", new Vector3(scale.x * 1.05f, scale.y * 1.05f, 1f), "time", 0.2f, "easetype", "easeinoutsine", "islocal", true, "name", "DarkEnemyShake1"));
		iTween.ScaleTo(m_MovePanel.gameObject, iTween.Hash("scale", scale, "time", 0.8f,"delay", 0.2f, "easetype", "linear", "islocal", true, "name", "DarkEnemyShake2"));
		iTween.MoveTo(m_MovePanel.gameObject, iTween.Hash("position", new Vector3(pos.x, pos.y + 0.1f, pos.z - 0.1f), "time", 0.2f, "islocal", true, "name", "DarkEnemyShake3"));
		iTween.MoveTo(m_MovePanel.gameObject, iTween.Hash("position", pos, "time", 0.8f, "delay", 0.2f, "islocal", true, "name", "DarkEnemyShake4"));
		iTween.ShakeRotation(m_MovePanel.gameObject, iTween.Hash("z", 1f, "time", 0.4f, "delay", 0.2f, "islocal", true, "name", "DarkEnemyShake5"));
	}
	void TW_DarkEnemyInit() {
		m_MovePanel.localScale = m_PanelTransInit[0];
		m_MovePanel.localPosition = m_PanelTransInit[1];
		m_MovePanel.localEulerAngles = m_PanelTransInit[2];
	}
	/// <summary> 카드 액션 </summary>
	public void Action(EItem_Stage_Card_Action act, float WaitTime = 0f, Action<Item_Stage> EndCB = null, params object[] args) {DarkInit();
		if (m_Action != null) {
			StopCoroutine(m_Action);
			m_Action = null;
		}
		switch (act) {
			case EItem_Stage_Card_Action.Move:
				if (args.Length > 1) m_Action = Action_Move(WaitTime, EndCB, (int)args[0], (int)args[1]);
				else m_Action = Action_Move(WaitTime, EndCB);
				break;
			case EItem_Stage_Card_Action.Scale:
				m_Action = Action_Scale(WaitTime, EndCB);
				break;
			case EItem_Stage_Card_Action.TargetOn:
				m_Action = Action_TargetOn(WaitTime, EndCB);
				break;
			case EItem_Stage_Card_Action.TargetOff:
				m_Action = Action_TargetOff(WaitTime, EndCB);
				break;
			case EItem_Stage_Card_Action.FadeIn:
				m_Action = Action_Fade_In((args == null || args.Length < 1 ? 0.3f : (float)args[0]), EndCB);//0.4->0.3
				break;
			case EItem_Stage_Card_Action.FadeOut:
				m_Action = Action_Fade_Out((args == null || args.Length < 1 ? 0.3f : (float)args[0]), EndCB);//0.4->0.3
				break;
			case EItem_Stage_Card_Action.Atk:
				m_Action = Action_Atk((Item_Stage)args[0], (EnemySkillTable)args[1], EndCB);
				break;
			case EItem_Stage_Card_Action.Damage:
				m_Action = Action_Damage(EndCB);
				break;
			case EItem_Stage_Card_Action.Change:
				m_Action = Action_Change(WaitTime, EndCB, true, args.Length > 0 ? Mathf.Max(0f, (float)args[0]) : 0f);
				break;
			case EItem_Stage_Card_Action.ChangeEnemyMat:
				m_Action = Action_ChangeEnemyMat(WaitTime, EndCB, true);
				break;
			case EItem_Stage_Card_Action.ChangeVal:
				m_Action = Action_ChangeVal(WaitTime, EndCB);
				break;
			case EItem_Stage_Card_Action.RefreshImgName:
				m_Action = Action_RefreshImgName(WaitTime, EndCB);
				break;
			case EItem_Stage_Card_Action.MoveChange:
				m_Action = Action_MoveChange((Item_Stage)args[0], (Vector3)args[1], (Vector3)args[2], (EFF_MoveCardChange)args[3], EndCB);
				break;
			case EItem_Stage_Card_Action.Die:
				m_Action = Action_Die(EndCB);
				break;
			case EItem_Stage_Card_Action.MoveTarget:
				m_Action = Action_MoveTarget((Item_Stage)args[0], EndCB);
				break;
			case EItem_Stage_Card_Action.Get:
				m_Action = Action_Get((Vector3)args[0], WaitTime, EndCB);
				break;
			case EItem_Stage_Card_Action.DissolveGet:
				m_Action = Action_DissolveGet((Vector3)args[0], WaitTime, EndCB);
				break;
			case EItem_Stage_Card_Action.DarkCardOn:
				m_Action = Action_DarkCardOn(EndCB);
				break;
			case EItem_Stage_Card_Action.DarkCardOff:
				m_Action = Action_DarkCardOff(EndCB);
				break;
			case EItem_Stage_Card_Action.Lock:
				m_Action = Action_Lock(EndCB);
				break;
			case EItem_Stage_Card_Action.UnLock:
				m_Action = Action_UnLock(EndCB);
				break;
		}
		if (m_Action != null && gameObject.activeInHierarchy) StartCoroutine(m_Action);
	}

	IEnumerator Action_Move(float WaitTime, Action<Item_Stage> EndCB, int gapy = -1,int gapx = -1) {
		m_MakingState = MakingState.Check;
		yield return new WaitForSeconds(WaitTime);

		float Scale = BaseValue.STAGE_SELECT_LINE_SCALE.x;
		float interverX = BaseValue.STAGE_INTERVER.x;
		float x = ((3 + m_Line * 2 - 1) * interverX * -0.5f) + interverX * m_Pos;
		Vector3 v3SPos = new Vector3(x, BaseValue.STAGE_INTERVER.y * m_Line, 0f);

		float gapmul = 1f;
		if (gapx > 0 && gapy > 0) gapmul = gapx / gapy ;

		//iTween.MoveTo(gameObject, iTween.Hash("position", v3SPos, "time", BaseValue.STAGE_MOVE_TIME, "easetype", "linear", "islocal", true));
		iTween.ValueTo(gameObject, iTween.Hash("from", transform.localPosition.x, "to", v3SPos.x,"onupdate", "TW_MoveX", "time", gapmul * BaseValue.STAGE_MOVE_TIME, "easetype", "linear"));
		iTween.ValueTo(gameObject, iTween.Hash("from", transform.localPosition.y, "to", v3SPos.y, "onupdate", "TW_MoveY", "time", BaseValue.STAGE_MOVE_TIME, "easetype", "linear"));

		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

		if (m_Line == 0) {
			yield return Action_Scale(WaitTime, EndCB);
		}
		else {
			EndCB?.Invoke(this);
			Action(EItem_Stage_Card_Action.None);
		}
		SetEnemyType();
	}
	void TW_MoveX(float _amount) {
		transform.localPosition = new Vector3(_amount, transform.localPosition.y, transform.localPosition.z);
	}
	void TW_MoveY(float _amount) {
		transform.localPosition = new Vector3(transform.localPosition.x, _amount, transform.localPosition.z);
	}
	IEnumerator Action_TargetOn(float WaitTime, Action<Item_Stage> EndCB) {
		DarkEnemyShakeCheck(false);
		TW_ScaleBumping(false);
		SetAccent(true);
		ActiveDark(false, true);

		yield return new WaitForSeconds(WaitTime);
		m_PanelTransInit[1] = new Vector3(0f, 0f, -0.5f);
		iTween.StopByName(m_MovePanel.gameObject, "MoveZ");
		iTween.MoveTo(m_MovePanel.gameObject, iTween.Hash("z", -0.5f, "time", 0.2f, "easetype", "linear", "islocal", true, "name", "MoveZ"));//time 0.3->0.2

		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_MovePanel.gameObject, "MoveZ"));

		EndCB?.Invoke(this);
		Action(EItem_Stage_Card_Action.None);
	}
	IEnumerator Action_TargetOff(float WaitTime, Action<Item_Stage> EndCB, bool endInit = true) {
		ActiveDark(true);
		SetAccent(false);
		yield return new WaitForSeconds(WaitTime);

		m_PanelTransInit[1] = Vector3.zero;
		iTween.StopByName(m_MovePanel.gameObject, "MoveZ");
		iTween.MoveTo(m_MovePanel.gameObject, iTween.Hash("z", 0, "time", 0.2f, "easetype", "linear", "islocal", true, "name", "MoveZ"));//time 0.3->0.2

		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_MovePanel.gameObject, "MoveZ"));

		DarkEnemyShakeCheck(true);
		EndCB?.Invoke(this);
		if (endInit) Action(EItem_Stage_Card_Action.None);
	}

	IEnumerator Action_Scale(float WaitTime, Action<Item_Stage> EndCB = null) {
		DarkEnemyShakeCheck(false);

		float Scale = BaseValue.STAGE_SELECT_LINE_SCALE.x;
		float interverX = BaseValue.STAGE_INTERVER.x;
		float x = ((3 + m_Line * 2 - 1) * interverX * -0.5f) + interverX * m_Pos;
		//Vector3 v3SPos = new Vector3(x, BaseValue.STAGE_INTERVER.y * m_Line, 0f);
		Vector3 v3EPos = new Vector3(BaseValue.STAGE_SELECT_LINE_EPOSX * (m_Pos - 1), BaseValue.STAGE_SELECT_LINE_EPOSY, 0f);//new Vector3(v3SPos.x * Scale, v3SPos.y - (BaseValue.STAGE_INTERVER.y * (Scale - 1) * 0.6f), 0f);
		//Vector3 v3GPos = v3EPos - v3SPos;
		//Vector3 v3SScale = transform.localScale;
		//Vector3 v3GScale = BaseValue.STAGE_SELECT_LINE_SCALE - v3SScale;

		if (IS_ScreenCard()) {
			for (int i = 0; i < m_TMPFX.Length; i++) m_TMPFX[i].enabled = true;
			ActiveDark(false);
			ActiveMoveMark(false);
			yield return new WaitForSeconds(WaitTime);

			iTween.ScaleTo(gameObject, iTween.Hash("scale", BaseValue.STAGE_SELECT_LINE_SCALE, "time", BaseValue.STAGE_MOVE_TIME, "easetype", "easeOutQuad"));
			iTween.MoveTo(gameObject, iTween.Hash("position", v3EPos, "time", BaseValue.STAGE_MOVE_TIME, "easetype", "easeOutQuad", "islocal", true));
			SetAccent(true);

			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		}
		else {
			transform.localScale = BaseValue.STAGE_SELECT_LINE_SCALE;
			transform.localPosition = v3EPos;
		}

		DarkEnemyShakeCheck(true);
		ActiveDark(m_Line != 0, m_Line == 0);
		if (m_Line == 0) {
			SetStreetLightShadow(false);
		}

		EndCB?.Invoke(this);
		Action(EItem_Stage_Card_Action.None);
	}
	public void TW_ScaleBumping(bool _on) {
		iTween.StopByName(m_MovePanel.gameObject, "Bumping");
		m_MovePanel.transform.localScale = Vector3.one;
		if (_on) iTween.ScaleTo(m_MovePanel.gameObject, iTween.Hash("scale", new Vector3(1.03f, 1.03f, 1f), "time", 2f, "easetype", "easeinoutsine", "islocal", true, "looptype", "pingpong", "name", "Bumping"));
	}

	IEnumerator Action_Fade_In(float MaxTime, Action<Item_Stage> EndCB) {
		IS_FadeIn = true;

		if (IS_Lock && m_LockFX) m_LockFX.GetComponent<Eff_Card_Chain>().SetAnim(Eff_Card_Chain.AnimState.Start);

		if (IS_ScreenCard()) {
			iTween.ValueTo(gameObject, iTween.Hash("from", m_Alpha.Alpha, "to", 1f, "time", MaxTime, "easetype", "easeInQuart", "onupdate", "SetFadeAlpha"));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		}
		else SetFadeAlpha(1f);
		SetHPBar();

		EndCB?.Invoke(this);
		Action(EItem_Stage_Card_Action.None);
	}

	IEnumerator Action_Fade_Out(float MaxTime, Action<Item_Stage> EndCB, bool endInit = true) {
		IS_FadeIn = false;
		ActiveDark(false);

		if (IS_Lock && m_LockFX) m_LockFX.GetComponent<Eff_Card_Chain>().SetAnim(Eff_Card_Chain.AnimState.End);

		if (IS_ScreenCard()) {
			iTween.ValueTo(gameObject, iTween.Hash("from", m_Alpha.Alpha, "to", 0f, "time", MaxTime, "easetype", "easeInQuart", "onupdate", "SetFadeAlpha"));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		}
		else SetFadeAlpha(0f);

		EndCB?.Invoke(this);
		if (endInit) Action(EItem_Stage_Card_Action.None);
	}
	IEnumerator Action_Atk(Item_Stage target, EnemySkillTable skill, Action<Item_Stage> EndCB) {
		DarkEnemyShakeCheck(false);
		TW_ScaleBumping(false);
		SetAccent(false);
		// 데미지 액션이 있으면 중지
		if (IS_ScreenCard() || target.IS_ScreenCard()) {
			ActiveDark(false);
			m_Sort.sortingOrder = 4;

			float x = 0, y = 0;
			if (target.m_Line == m_Line) {
				x = BaseValue.STAGE_INTERVER.x * 0.3f;
				if (target.m_Pos < m_Pos) x *= -1f;
			}
			else {
				y = BaseValue.STAGE_INTERVER.y * 0.3f;
				if (target.m_Line < m_Line) y *= -1f;
			}
			float delay = 0.1f;
			iTween.MoveTo(m_MovePanel.gameObject, iTween.Hash("x", x, "y", y, "islocal", true, "time", delay, "easetype", "easeOutQuad", "name", "MoveTarget"));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_MovePanel.gameObject, "MoveTarget"));

			delay = 0.2f;//0.3->0.2
			iTween.MoveTo(m_MovePanel.gameObject, iTween.Hash("position", Vector3.zero, "islocal", true, "time", delay, "easetype", "easeOutQuad", "name", "MoveTarget"));


			if (target.m_Info.IS_EnemyCard)
				target.SetDamage(this, skill.GetCnt());
			else {
				Item_Stage card = target;
				target.Action(EItem_Stage_Card_Action.Die, 0, (obj)=> { card = null; });
				yield return new WaitWhile(() => card != null);
			}
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_MovePanel.gameObject, "MoveTarget"));

			m_Sort.sortingOrder = m_Line == 0 ? 3 : m_Line * -1;
			ActiveDark(m_Line != 0);
		}
		else {
			if (target.m_Info.IS_EnemyCard)
				target.SetDamage(this, skill.GetCnt());
			else {
				Item_Stage card = target;
				target.Action(EItem_Stage_Card_Action.Die, 0, (obj) => { card = null; });
				yield return new WaitWhile(() => card != null);
			}
		}

		DarkEnemyShakeCheck(true);
		EndCB?.Invoke(this);
		Action(EItem_Stage_Card_Action.None);
	}

	IEnumerator Action_Damage(Action<Item_Stage> EndCB) {
		DarkEnemyShakeCheck(false);
		TW_ScaleBumping(false);
		//SetAccent(false);
		if (IS_ScreenCard()) {
			ActiveDark(false);
			Vector3 position = m_MovePanel.position;
			float actiontime = 0.1f;
			float delaytime = 0f;
			// 흔들기
			for (int i = 0; i < 4; i++) {
				iTween.MoveTo(m_MovePanel.gameObject, iTween.Hash("position", new Vector3(Utile_Class.Get_RandomStatic(-0.1f, 0.1f), Utile_Class.Get_RandomStatic(-0.1f, 0.1f), 0f)
					, "isLocal", true, "time", actiontime, "delay", delaytime, "easetype", "easeOutQuad", "name", "Damage"));
				delaytime += actiontime;
			}

			iTween.MoveTo(m_MovePanel.gameObject, iTween.Hash("position", Vector3.zero, "isLocal", true, "time", actiontime, "delay", delaytime, "easetype", "easeOutQuad", "name", "Damage"));
			delaytime += actiontime;

			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_MovePanel.gameObject));
			ActiveDark(m_Line != 0, m_Line == 0);
		}

		DarkEnemyShakeCheck(true);
		EndCB?.Invoke(this);
		Action(EItem_Stage_Card_Action.None);
	}

	/// <summary> NPC간 AI 전투용 </summary>
	public void SetDamage(Item_Stage AtkCard, int Cnt) {
		StageCardInfo AtkInfo = AtkCard.m_Info;
		TEnemyTable atk = AtkInfo.m_TEnemyData;
		TEnemyTable Def = m_Info.m_TEnemyData;

		
		int Damage = BaseValue.GetDamage(AtkCard.m_Info.GetStat(EEnemyStat.ATK), AtkInfo.m_LV, m_Info.GetStat(EEnemyStat.DEF), 1f, ENoteHitState.End);
		Damage = Mathf.RoundToInt(Damage * TDATA.GetEnemyAtkRatioRelation(AtkCard.m_Info.m_TEnemyData.m_Type, m_Info.m_TEnemyData.m_Type));
		int changenemy = atk.m_AtkAI.m_Type == EEnemyAIAtkType.Infection ? atk.m_AtkAI.m_Values[1] : 0;

		SetDamage(false, Damage * BaseValue.NpctoNpcDmg, Cnt, changenemy);
	}
	/// <summary> 피격시 데미지 적용, 자동전투에서만 duel이 true </summary>
	public void SetDamage(bool _duel, int damage, int Cnt = 1, int changenemy = 0, bool damageaction = true)//자동전투에서만 _duel이 true임
	{
		m_Info.m_HP = Math.Max(0, m_Info.GetStat(EEnemyStat.HP) - damage);

		// 데미지 폰트
		for (int i = Cnt - 1; i > -1; --i) STAGE.SetDamage(this, damage);
		SetHPBar();

		if (!ISAtk && damageaction) Action(EItem_Stage_Card_Action.Damage);
		if (IS_Die()) {
			if (IS_ScreenCard()) PlayHitSND();
			int changecard = 0;
			if(m_Info.m_TData.m_Type == StageCardType.Hive) changecard = Mathf.RoundToInt(m_Info.m_TData.m_Value2);//하이브 제거시 에너미로 변환
			if (changenemy > 0) SetEnemyChange(changenemy);     // 변경할 몬스터가 있을때
			else if (changecard > 0) {
				SetCardChange(changecard); // 시체 변경
			}
			else if (!_duel) {//일대일 싸움 제외
#if !STAGE_TEST
				if (!TUTO.IsTuto(TutoKind.Stage_105, (int)(int)TutoType_Stage_105.Machingun_Action_End) && m_Info.m_TData.m_Idx != 509)//231은 105에서 튜토용으로만 쓰임
#endif
				{
					if (m_Info.m_TEnemyData.m_Grade == EEnemyGrade.Normal) {
						if (m_Info.m_TEnemyData.m_DropCardGid > 0) {
							SetEnemyMatChange(TDATA.GetRandEnemyDropTable(m_Info.m_TEnemyData.m_DropCardGid).m_CardIdx);
						}
						else {
							List<TStageCardTable> datas = STAGEINFO.GetMaterialCardIdxs();
							if (datas.Count > 0) SetEnemyMatChange(datas[UTILE.Get_Random(0, datas.Count)].m_Idx);    // 재료로 변경
						}

						//List<int> gids = new List<int>();
						//int idx = 0;
						//for (int i = 0; i < 3; i++) {
						//	int gid = gids.Count < 2 ? BaseValue.BATTLE_REWARD_COMMON_GID : m_Info.m_TEnemyData.m_RewardGID;

						//	TIngameRewardTable table = TDATA.GetPickIngameReward(gid, m_Info.m_TEnemyData.m_RewardLV, STAGE.CheckBattleReward, true);
						//	gids.Add(table.m_Val);
						//}
						//idx = gids[UTILE.Get_Random(0, 3)];
						//SetCardChange(idx);
					}
					else {
						SetCardChange(m_Info.m_TEnemyData.m_RewardCancle ? BaseValue.ITEM_REWARDBOX_CANCLE_IDX : BaseValue.ITEM_REWARDBOX_NOTCANCLE_IDX);
						m_Info.m_IngameRewardIdx = m_Info.m_TEnemyData.m_RewardGID;
						m_Info.m_DropRewardIdx = m_Info.m_TEnemyData.m_DropCardGid;
						m_Info.Is_RewardCancle = m_Info.m_TEnemyData.m_RewardCancle;
					}
				}
			}
			else IS_KilledDuel = true;
		}
		else StartCoroutine(HitSFXDelay(0.2f));
	}
	IEnumerator HitSFXDelay(float _delay) {
		yield return new WaitForSeconds(_delay);
		if (IS_ScreenCard() && UTILE.Get_Random(0, 100) < 30) PlayHitSND();
	}
	public void PlayHitSND() {
		if (!m_Info.IS_EnemyCard) return;
		if (m_HitSND.Count < 1) m_HitSND.AddRange(m_Info.m_TEnemyData.m_HitVoice);
		if (m_HitSND.Count < 1) return;
		SND_IDX idx = m_HitSND[UTILE.Get_Random(0, m_HitSND.Count)];
		m_HitSND.Remove(idx);
		PlayEffSound(idx);
	}
	/// <summary> 바뀔 에너미 인덱스 세팅 </summary>
	public void SetEnemyChange(int idx) {
		m_EnemyChangeIdx = idx;
	}
	/// <summary> 바뀔 스테이지 카드 인덱스 세팅 </summary>
	public void SetCardChange(int idx) {
		m_StageChangeIdx = idx;
	}
	/// <summary> 죽은 적이 바뀔 재료카드 인덱스 </summary>
	public void SetEnemyMatChange(int _idx) {
		m_EnemyMatChagneIdx = _idx;
	}
	IEnumerator Action_Die(Action<Item_Stage> EndCB) {
		// AI 정지 대상으로 셋팅되어있는지 확인
		for (int i = STAGE.m_AIStopInfos.Count - 1; i > -1; i--) {
			if (STAGE.m_AIStopInfos[i].m_Target == this) STAGE.RemoveAIStopInfo(STAGE.m_AIStopInfos[i]);
		}
		// AI 원거리 블록 대상으로 셋팅되어있는지 확인
		for (int i = STAGE.m_AiBlockRangeAtkInfos.Count - 1; i > -1; i--) {
			if (STAGE.m_AiBlockRangeAtkInfos[i].m_Target == this) STAGE.RemoveAiBlockRangeAtkInfo(STAGE.m_AiBlockRangeAtkInfos[i]);
		}
		// 화상 대상으로 셋팅되어있는지 확인
		for (int j = STAGE.m_BurnInfos.Count - 1; j > -1; j--) {
			if (STAGE.m_BurnInfos[j].m_Target == this) STAGE.RemoveBurnInfo(STAGE.m_BurnInfos[j]);
		}

		IS_FadeIn = false;
		m_Stage[m_ViewCardPos].Icon.SetColor("_DissolveColor", m_DissolveColor[1]);


		DarkEnemyShakeCheck(false);
		ActiveStatMark(false);
		ActiveDeadlyMark(false);
		ActiveMoveMark(false);
		ActiveDark(false);
		SetDarkPatrolMark(false);
		ActiveSaveUtileFX(false); 
		SetEnemyType();
		if (m_LockFX) Destroy(m_LockFX);

		if (IS_ScreenCard()) {
			SetViewCardDissolveValue(0f);
	
			iTween.Stop(m_MovePanel.gameObject);
			m_MovePanel.localPosition = Vector3.zero;

			float dissolvetime = 0.6f;//1->0.6f
			iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", dissolvetime, "easetype", "easeInQuart", "onupdate", "SetViewCardDissolveValue"));
			DieFX(m_Info.m_NowTData.m_Type);
			switch (m_Info.m_NowTData.m_Type) {
				case StageCardType.Roadblock:
				case StageCardType.AllRoadblock:
					break;
				default:
					PlayEffSound(SND_IDX.SFX_0221);
					break;
			}
			//if (m_Info.ISRefugee) {
			//	yield return new WaitForSeconds(dissolvetime * 0.5f);
			//	PlayEffSound(SND_IDX.SFX_1601);
			//}
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		}
		else {
			SetViewCardDissolveValue(1f);
			DieFX(m_Info.m_NowTData.m_Type);
		}

		EndCB?.Invoke(this);
		Action(EItem_Stage_Card_Action.None);
	}
	IEnumerator Action_Change(float actiontime, Action<Item_Stage> EndCB, bool endInit = true, float _delay = 0f) {
		TW_ScaleBumping(false);
		SetAccent(false);

		int matPos = 0;
		// AI 정지 대상으로 셋팅되어있는지 확인
		for (int i = STAGE.m_AIStopInfos.Count - 1; i > -1; i--) {
			if (STAGE.m_AIStopInfos[i].m_Target == this) STAGE.RemoveAIStopInfo(STAGE.m_AIStopInfos[i]);
		}
		// AI 원거리 블록 대상으로 셋팅되어있는지 확인
		for (int i = STAGE.m_AiBlockRangeAtkInfos.Count - 1; i > -1; i--) {
			if (STAGE.m_AiBlockRangeAtkInfos[i].m_Target == this) STAGE.RemoveAiBlockRangeAtkInfo(STAGE.m_AiBlockRangeAtkInfos[i]);
		}
		// 화상 대상으로 셋팅되어있는지 확인
		for (int j = STAGE.m_BurnInfos.Count - 1; j > -1; j--) {
			if (STAGE.m_BurnInfos[j].m_Target == this) STAGE.RemoveBurnInfo(STAGE.m_BurnInfos[j]);
		}
		if (m_Info.m_TData.m_Type == StageCardType.Ash) {
			DieFX(m_Info.m_TData.m_Type);
			matPos = 1;
		}
		if (actiontime < 0.3f) actiontime = 0.3f;//0.5->0.3
		if (m_EnemyChangeIdx > 0) {
			m_Info.SetData(m_EnemyChangeIdx);
			m_EnemyChangeIdx = 0;
		}
		else {//!IsLight && IsDarkCard 어둠카드인데 밝혀지지 않은 경우
			if (!m_Info.IsDarkCard) m_Info.SetIdx(m_StageChangeIdx);//다크카인데 밝혀지거나 다크카드 아닌 경우
			else {
				m_Info.SetRealIdx(m_StageChangeIdx);
			}
			m_StageChangeIdx = 0;
			m_Info.SetData();
		}
		m_MakingState = MakingState.Check;

		yield return new WaitForSeconds(_delay);

		ActiveStatMark(false);
		ActiveDeadlyMark(false);
		ActiveMoveMark(false);
		SetDarkPatrolMark(false);
		ActiveSaveUtileFX(false);
		SetEnemyType();
		SetFrontImg(m_Info.GetImg(), m_Info.GetName(), EItem_Stage_Card_FrontAction.Dissolve_Out, actiontime, matPos);

		if (IS_ScreenCard()) {
			ActiveDark(false);
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
			//첫줄 사살해서 재료카드로 바뀌면 터치 가능하게
			ActiveDark(m_Line != 0, m_Line == 0);
		}

		SetTurn();
		SetStatMark();
		ActiveMoveMark(true);
		ActiveDeadlyMark(true);
		ActiveSaveUtileFX(true);
		SetBG(); 

		 m_Stage[m_ViewCardPos].Active.SetActive(false);

		m_ViewCardPos = 1 - m_ViewCardPos;
		m_Stage[m_ViewCardPos].Group.sortingOrder = m_Line == 0 ? 4 : 2;

		m_Target = null;
		EndCB?.Invoke(this);

		if (endInit) Action(EItem_Stage_Card_Action.None);

	}
	IEnumerator Action_ChangeEnemyMat(float actiontime, Action<Item_Stage> EndCB, bool endInit = true) {
		TW_ScaleBumping(false);
		SetAccent(false);

		int matPos = 2;

		// AI 정지 대상으로 셋팅되어있는지 확인
		for (int i = STAGE.m_AIStopInfos.Count - 1; i > -1; i--) {
			if (STAGE.m_AIStopInfos[i].m_Target == this) STAGE.RemoveAIStopInfo(STAGE.m_AIStopInfos[i]);
		}
		// AI 원거리 블록 대상으로 셋팅되어있는지 확인
		for (int i = STAGE.m_AiBlockRangeAtkInfos.Count - 1; i > -1; i--) {
			if (STAGE.m_AiBlockRangeAtkInfos[i].m_Target == this) STAGE.RemoveAiBlockRangeAtkInfo(STAGE.m_AiBlockRangeAtkInfos[i]);
		}
		// 화상 대상으로 셋팅되어있는지 확인
		for (int j = STAGE.m_BurnInfos.Count - 1; j > -1; j--) {
			if (STAGE.m_BurnInfos[j].m_Target == this) STAGE.RemoveBurnInfo(STAGE.m_BurnInfos[j]);
		}
		if (actiontime < 0.3f) actiontime = 0.3f;//0.5->0.3

		if (!m_Info.IsDarkCard)
			m_Info.SetIdx(m_EnemyMatChagneIdx);
		else
			m_Info.SetRealIdx(m_EnemyMatChagneIdx);
		m_EnemyMatChagneIdx = 0;
		m_Info.SetData();

		m_MakingState = MakingState.Check;

		ActiveStatMark(false);
		ActiveDeadlyMark(false);
		ActiveMoveMark(false);
		SetDarkPatrolMark(false);
		ActiveSaveUtileFX(false);
		SetEnemyType();
		if (IS_ScreenCard()) {
			ActiveDark(false);

			Vector3 cardscale = m_Stage[1].Active.transform.localScale;
			TW_SetItemScale(0f);
			TW_SetPanel_0_Scale(0f);

			iTween.ValueTo(gameObject, iTween.Hash("from", cardscale.x, "to", 7.741244f, "time", actiontime, "onupdate", "TW_Panel_1SizeX", "easetype", "easeInExpo"));
			iTween.ValueTo(gameObject, iTween.Hash("from", cardscale.y, "to", 10.14483f, "time", actiontime, "onupdate", "TW_Panel_1SizeY", "easetype", "easeInExpo"));
			SetFrontImg(m_Info.GetImg(), m_Info.GetName(), EItem_Stage_Card_FrontAction.Dissolve_Out, actiontime, matPos);

			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
			m_Stage[1].Active.transform.localScale = cardscale;//카드 크기 원상복귀

			m_ItemImg.SetActive(true);
			iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", actiontime, "onupdate", "TW_SetItemAlpha", "easetype", "linear"));
			iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 2f, "time", actiontime, "onupdate", "TW_SetItemScale", "easetype", "linear"));
			iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", cardscale.x, "time", actiontime, "onupdate", "TW_SetPanel_0_Scale", "easetype", "linear"));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

			m_ItemImg.SetActive(false);
			//첫줄 사살해서 재료카드로 바뀌면 터치 가능하게
			ActiveDark(m_Line != 0, m_Line == 0);
		}
		else {
			SetFrontImg(m_Info.GetImg(), m_Info.GetName(), EItem_Stage_Card_FrontAction.Dissolve_Out, actiontime, matPos);
		}

		SetTurn();
		SetStatMark();
		ActiveDeadlyMark(true);
		ActiveSaveUtileFX(true);
		SetBG();

		m_Stage[m_ViewCardPos].Active.SetActive(false);

		m_ViewCardPos = 1 - m_ViewCardPos;
		m_Stage[m_ViewCardPos].Group.sortingOrder = m_Line == 0 ? 4 : 2;

		m_Target = null;
		EndCB?.Invoke(this);

		if (endInit) Action(EItem_Stage_Card_Action.None);

	}
	IEnumerator Action_RefreshImgName(float actiontime, Action<Item_Stage> EndCB, bool endInit = true) {
		SetFrontImg(m_Info.GetImg(), m_Info.GetName(), EItem_Stage_Card_FrontAction.Dissolve_Out, actiontime, 0);

		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

		EndCB?.Invoke(this);

		if (endInit) Action(EItem_Stage_Card_Action.None);
	}
	/// <summary> 호출전 바뀐 레벨이나 갯수등 반영된걸 바꾸는것, UI만 갱신 </summary>
	IEnumerator Action_ChangeVal(float actiontime, Action<Item_Stage> EndCB, bool endInit = true) {
		int matPos = 0;
		if (m_Info.m_TData.m_Type == StageCardType.Ash) {
			DieFX(m_Info.m_TData.m_Type);
			matPos = 1;
		}
		ActiveStatMark(false);
		ActiveDeadlyMark(false);
		ActiveMoveMark(false);
		SetDarkPatrolMark(false);
		ActiveSaveUtileFX(false);
		SetEnemyType();
		if (IS_ScreenCard()) {
			ActiveDark(false);

			SetFrontImg(m_Info.GetImg(), m_Info.GetName(), EItem_Stage_Card_FrontAction.Dissolve_Out, actiontime, matPos);

			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
			//첫줄 사살해서 재료카드로 바뀌면 터치 가능하게
			ActiveDark(m_Line != 0, m_Line == 0);
		}
		else {
			SetFrontImg(m_Info.GetImg(), m_Info.GetName(), EItem_Stage_Card_FrontAction.Dissolve_Out, actiontime, matPos);
		}

		SetTurn();
		SetStatMark();
		ActiveDeadlyMark(true);
		ActiveSaveUtileFX(true);
		SetBG();

		m_Stage[m_ViewCardPos].Active.SetActive(false);

		m_ViewCardPos = 1 - m_ViewCardPos;
		m_Stage[m_ViewCardPos].Group.sortingOrder = m_Line == 0 ? 4 : 2;
		EndCB?.Invoke(this);

		if (endInit) Action(EItem_Stage_Card_Action.None);
	}
	IEnumerator Action_MoveChange(Item_Stage atk, Vector3 SPos, Vector3 EPos, EFF_MoveCardChange Eff, Action<Item_Stage> EndCB) {
		ActiveStatMark(false);
		ActiveDeadlyMark(false);
		ActiveMoveMark(false);
		SetDarkPatrolMark(false);
		ActiveSaveUtileFX(false);
		SetEnemyType();

		if (IS_ScreenCard()) {
			atk.ActiveDark(false);
			ActiveDark(false);
			if (Eff != null) {
				bool EffAction = true;
				Eff.SetData(SPos, EPos, (obj) => {
					EffAction = false;
				});
				yield return new WaitWhile(() => EffAction);
			}
		}

		yield return Action_Change(1f, (obj) => {
			atk.ActiveDark(atk.m_Line != 0);
			ActiveDark(m_Line != 0);
			EndCB?.Invoke(obj);
		});

	}
	IEnumerator Action_MoveTarget(Item_Stage target, Action<Item_Stage> EndCB) {
		int line = m_Line;
		int pos = m_Pos;

		SetPos(target.m_Line, target.m_Pos);
		target.SetPos(line, pos);
		m_MakingState = MakingState.Check;
		target.m_MakingState = MakingState.Check;

		Vector3 mymovePos = target.transform.localPosition;
		Vector3 targetmovePos = transform.localPosition;
		Vector3 scale = transform.localScale;
		Vector3 targetscale = target.transform.localScale;

		if (IS_ScreenCard() || target.IS_ScreenCard()) {
			ActiveDark(false);
			target.ActiveDark(false);

			float movetime = 0.3f;//0.5f->0.3f
			iTween.MoveTo(gameObject, iTween.Hash("position", mymovePos, "time", movetime, "isLocal", true, "easetype", "easeOutQuad", "name", "MoveTarget"));
			iTween.MoveTo(target.gameObject, iTween.Hash("position", targetmovePos, "time", movetime, "isLocal", true, "easetype", "easeOutQuad", "name", "MoveTarget"));


			iTween.ScaleTo(gameObject, iTween.Hash("scale", targetscale, "time", movetime, "easetype", "easeOutQuad", "name", "MoveTarget"));
			iTween.ScaleTo(target.gameObject, iTween.Hash("scale", scale, "time", movetime, "easetype", "easeOutQuad", "name", "MoveTarget"));

			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(target.gameObject));
		}
		else {
			gameObject.transform.localPosition = mymovePos;
			target.gameObject.transform.localPosition = targetmovePos;
			gameObject.transform.localScale = targetscale;
			target.gameObject.transform.localScale = scale;
		}

		ActiveDark(m_Line != 0);
		target.ActiveDark(target.m_Line != 0);
		SetEnemyType();

		EndCB?.Invoke(this);
		Action(EItem_Stage_Card_Action.None);
	}
	IEnumerator Action_Get(Vector3 target, float delaytime, Action<Item_Stage> EndCB) {
		ActiveDark(false);
		yield return new WaitForSeconds(delaytime);
		// 갑자기 이동하면 이상하므로 확대시켜줌
		float actiontime = 0.15f;//0.3->0.2
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * 1.5f, "isLocal", true, "time", actiontime, "easetype", "easeOutQuad"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

		actiontime = 0.2f;//0.5->0.3
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * 0.1f, "isLocal", true, "time", actiontime, "easetype", "easeOutQuad"));
		iTween.MoveTo(gameObject, iTween.Hash("position", target, "time", actiontime, "easetype", "easeOutQuad"));

		yield return Action_Fade_Out(actiontime, null, false);
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

		switch (m_Info.m_TData.m_Type) {
			case StageCardType.BigSupplyBox:
				if (IS_ScreenCard()) PlayEffSound(SND_IDX.SFX_0003);
				break;
			case StageCardType.TornBody:
			case StageCardType.Garbage:
			case StageCardType.Pit:
				if (IS_ScreenCard()) PlayEffSound(SND_IDX.SFX_0471);
				break;
			default:
				if (IS_ScreenCard()) PlayEffSound(SND_IDX.SFX_0206);
				break;
		}
		EndCB?.Invoke(this);

		Action(EItem_Stage_Card_Action.None);
	}
	IEnumerator Action_DissolveGet(Vector3 target, float delaytime, Action<Item_Stage> EndCB) {
		ActiveDark(false);
		yield return new WaitForSeconds(delaytime);

		IS_FadeIn = false;
		m_Stage[m_ViewCardPos].Icon.SetColor("_DissolveColor", m_DissolveColor[1]);

		DarkEnemyShakeCheck(false);
		ActiveStatMark(false);
		ActiveDeadlyMark(false);
		ActiveMoveMark(false);
		ActiveDark(false);
		SetDarkPatrolMark(false);
		ActiveSaveUtileFX(false);
		SetEnemyType();
		if (m_LockFX) Destroy(m_LockFX);

		if (IS_ScreenCard()) {
			SetViewCardDissolveValue(0f);

			iTween.Stop(m_MovePanel.gameObject);
			m_MovePanel.localPosition = Vector3.zero;

			float dissolvetime = 0.25f;//1->0.6f
			iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", dissolvetime, "easetype", "easeInQuart", "onupdate", "SetViewCardDissolveValue"));
			switch (m_Info.m_NowTData.m_Type) {
				case StageCardType.Roadblock:
				case StageCardType.AllRoadblock:
					break;
				default:
					PlayEffSound(SND_IDX.SFX_0221);
					break;
			}
		}
		else {
			SetViewCardDissolveValue(1f);
		}


		// 갑자기 이동하면 이상하므로 확대시켜줌
		float actiontime = 0.15f;//0.3->0.2
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * 1.5f, "isLocal", true, "time", actiontime, "easetype", "easeOutQuad"));
		yield return new WaitForSeconds(actiontime);
		//yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

		actiontime = 0.2f;//0.5->0.3
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * 0.5f, "isLocal", true, "time", actiontime, "easetype", "easeOutQuad"));
		iTween.MoveTo(gameObject, iTween.Hash("position", target, "time", actiontime, "easetype", "easeOutQuad"));

		yield return new WaitForSeconds(actiontime * 0.5f);

		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

		DieFX(m_Info.m_NowTData.m_Type);

		EndCB?.Invoke(this);

		Action(EItem_Stage_Card_Action.None);
	}
	IEnumerator Action_DarkCardOn(Action<Item_Stage> EndCB) {
		m_IsLight = false;
		if (!m_Info.IsDark) {
			SetHPBar();
			EndCB?.Invoke(this);
			yield break;
		}

		SetDarkPatrolMark(STAGE.m_DarkPatrolCnt > 0 && m_Info.IS_DmgTarget());
		DarkEnemyShakeCheck(true);
		SetReduceStatFX(false);
		SetHPBar();
		SetTurn();
		ActiveStatMark(false);
		ActiveMoveMark(false);
		ActiveDeadlyMark(false);
		ActiveSaveUtileFX(false);
		SetEnemyType();

		SetFrontImg(m_Info.GetImg(), m_Info.GetName(), EItem_Stage_Card_FrontAction.Dissolve_Out, 0.6f);//1->0.6

		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		m_Stage[m_ViewCardPos].Active.SetActive(false);

		m_ViewCardPos = 1 - m_ViewCardPos;
		m_Stage[m_ViewCardPos].Group.sortingOrder = m_Line == 0 ? 4 : 2;
		EndCB?.Invoke(this);
	}
	IEnumerator Action_DarkCardOff(Action<Item_Stage> EndCB) {
		if (!m_Info.IsDark) {
			m_IsLight = true;
			SetHPBar();
			EndCB?.Invoke(this);
			yield break;
		}
		m_IsLight = true;
		SetFrontImg(m_Info.GetImg(), m_Info.GetName(), EItem_Stage_Card_FrontAction.Dissolve_Out, 0.6f);//1->0.6
		m_MakingState = MakingState.Check;

		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

		SetDarkPatrolMark(false);
		DarkEnemyShakeCheck(false);
		SetReduceStatFX(true);
		SetHPBar();
		SetTurn();
		SetStatMark();
		ActiveMoveMark(true);
		ActiveDeadlyMark(true);
		ActiveSaveUtileFX(true);
		SetEnemyType();
		SetBG();

		m_Stage[m_ViewCardPos].Active.SetActive(false);

		m_ViewCardPos = 1 - m_ViewCardPos;
		m_Stage[m_ViewCardPos].Group.sortingOrder = m_Line == 0 ? 4 :2;

		ActiveDark(m_Line != 0, m_Line == 0);
		EndCB?.Invoke(this);
	}

	IEnumerator Action_Lock(Action<Item_Stage> EndCB) {
		//카드 잠금 연출
		IS_Lock = true;
		if (IS_ScreenCard()) PlayEffSound(SND_IDX.SFX_0622);
		m_LockFX = STAGE.StartEff(transform, "Effect/Stage/Eff_Card_Chain");
		m_LockFX.GetComponent<Eff_Card_Chain>().SetAnim(Eff_Card_Chain.AnimState.Start);
		m_LockFX.transform.SetParent(transform);
		m_LockFX.transform.localPosition = Vector3.zero;

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_LockFX.GetComponent<Animator>()));

		EndCB?.Invoke(this);
	}

	IEnumerator Action_UnLock(Action<Item_Stage> EndCB) {
		//카드 잠금 연출
		IS_Lock = false;

		if (IS_ScreenCard()) PlayEffSound(SND_IDX.SFX_0622);
		m_LockFX.GetComponent<Eff_Card_Chain>().SetAnim(Eff_Card_Chain.AnimState.End);
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_LockFX.GetComponent<Animator>()));
		if (m_LockFX) Destroy(m_LockFX);

		EndCB?.Invoke(this);
	}
	/// <summary>Render Alpha controller 알파 조절 외부 호출용 </summary>
	public void SetLockOnOff(bool _on) {
		if(IS_Lock && m_LockFX) m_LockFX.GetComponent<Eff_Card_Chain>().SetAnim(_on ? Eff_Card_Chain.AnimState.Start : Eff_Card_Chain.AnimState.End);
	}
	/// <summary> 잠긴 카드 터치 효과 </summary>
	public void LockTouch() {
		PlayEffSound(SND_IDX.SFX_0623);
		m_LockFX.GetComponent<Eff_Card_Chain>().SetAnim(Eff_Card_Chain.AnimState.Touch);
	}
	/// <summary> 적 -> 재료 변환 이미지 알파 </summary>
	void TW_SetItemAlpha(float _amount) {
		m_ItemImg.GetComponent<SpriteRenderer>().color = new Color(m_ItemImg.GetComponent<SpriteRenderer>().color.r, m_ItemImg.GetComponent<SpriteRenderer>().color.g, m_ItemImg.GetComponent<SpriteRenderer>().color.b, _amount);
	}
	void TW_SetItemScale(float _amount) {
		m_ItemImg.transform.localScale = new Vector3(_amount, m_ItemImg.transform.localScale.y, m_ItemImg.transform.localScale.z);
	}
	void TW_SetPanel_0_Scale(float _amount) {
		m_Stage[0].Active.transform.localScale = new Vector3(_amount, m_Stage[0].Active.transform.localScale.y, m_Stage[0].Active.transform.localScale.z);
	}
	void TW_Panel_1SizeX(float _amount) {
		m_Stage[1].Active.transform.localScale = new Vector3(_amount, m_Stage[1].Active.transform.localScale.y, m_Stage[1].Active.transform.localScale.z);
	}
	void TW_Panel_1SizeY(float _amount) {
		m_Stage[1].Active.transform.localScale = new Vector3(m_Stage[1].Active.transform.localScale.x, _amount, m_Stage[1].Active.transform.localScale.z);
	}
	/// <summary> 스캣 감소시키는 에너미 표기 </summary>
	void SetReduceStatFX(bool _on) {
		if (!m_Info.IsDark && m_Info.IS_EnemyCard) {
			for (int i = (int)StatType.Men; i < (int)StatType.SurvEnd; i++) {
				if (!STAGE_USERINFO.Is_UseStat((StatType)i)) continue;
				if (m_Info.m_TEnemyData.GetBTReduceStat((StatType)i, m_Info.m_LV) != 0 || !_on) {
					m_ReduceStatEnemyFX.SetActive(_on);
					break;
				}
			}
		}
	}
	/// <summary> 사망시 효과 </summary>
	void DieFX(StageCardType _type) {
		switch (_type) {
			case StageCardType.Roadblock:
			case StageCardType.AllRoadblock:
				break;
			default:
				STAGE.StartEff(transform, "Effect/Stage/Eff_Card_Die");
				break;
		}
	}
}
