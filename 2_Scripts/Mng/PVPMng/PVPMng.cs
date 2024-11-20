using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static LS_Web;

public partial class PVPMng : ObjMng
{
	public enum State
	{
		Loading = 0,
		SelectReward,
		Play,
		Battle,
		Result,
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Instance
	private static PVPMng m_Instance = null;
	public static PVPMng Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public static bool IsValid()
	{
		return m_Instance != null;
	}
	Main_PVP m_MainUI;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Value
#pragma warning disable 0649
	[Serializable]
	public struct SUI
	{
		public Transform[] FXBucket;	//0:배경이펙트,1:스킬이펙트
		public Item_PVP_Char[] Decks;
		public Item_SurvStat_PVP[] Stats;
		public GameObject DmgStr;
		public Animator ScalePanelAnim;
	}
	[SerializeField] SUI m_SUI;
	/// <summary> 이펙트 풀 </summary>
	public class FX
	{
		public GameObject Obj;
		public Vector3 Scale;
		public Vector3 Rot;
	}
	Dictionary<string, List<FX>> m_FXPool = new Dictionary<string, List<FX>>();
	EF_BuffCenterAlarm m_AddStatAlarm;
#if PVP_TEST
	[SerializeField, ReName("자신", "적")] long[] TestPower = new long[2] { 1000, 1000 };
#endif
	State m_State;
	public bool IS_Play { get { return m_State == State.Play; } }
	Result m_Result;
	PVPFailCause m_FailCause;
	IEnumerator m_Action;
	GameObject m_SelectPanel;

	public GameObject GetStatObj(int _pos, StatType _type) {
		return m_SUI.Stats[_pos * 3 + (int)_type].gameObject;
	}
#pragma warning restore 0649
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Process
	// Start is called before the first frame update
	void Awake()
	{
#if PVP_TEST
		if (MAIN.IS_BackState(MainState.START))
		{
			TDATA.LoadDefaultTables(1);
			// 가상 데이터 셋팅
			PVPINFO.SetUser(PVPInfo.UserPos.My, 0, TestPower[0], 1);
			PVPINFO.SetUser(PVPInfo.UserPos.Target, 1, TestPower[1], 2);
		}
#endif
		// TUDO : PVP 관련 툴 데이터 로드
		TDATA.LoadPVPData();
		// 기본 카드 테이블 로드
		TDATA.LoadStageCardTable();

		m_Instance = this;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void Start() {
		Init();
	}

	private void Update()
	{
		//CameraMoveUpdate();
		if (m_State != State.Play) return;
		if (m_Action != null) return;
	}
	public void Init()
	{
		InitCamAnim();

		m_Turn = 0;
		// 플레이 기본 데이터 셋팅
		SetCharSpeedRevision();
		SetPlayData();
		m_MainUI = POPUP.Set_Popup(PopupPos.MAINUI, PopupName.PVP).GetComponent<Main_PVP>();
		m_Action = IE_StartAction();
		StartCoroutine(m_Action);
	}

	[System.Diagnostics.Conditional("USE_PVP_DEBUG")]
	static public void Log(string s)
	{
		Debug.Log("[PVP] " + s);
	}

	void ActionEnd()
	{
		m_Action = null;
	}
	/// <summary> 기권 </summary>
	public void SetSurrender() {
		StopAllCoroutines();
		for(int i = 0; i < m_SUI.Decks.Length; i++) {
			m_SUI.Decks[i].StopAllCoroutines();
		}
		ActionEnd();
		StateChange(State.Result, m_Result = Result.Surrender);
	}
	public void StateChange(State state, params object[] args)
	{
		m_State = state;
		switch(state)
		{
			case State.Loading:
				m_SUI.ScalePanelAnim.SetTrigger("1_Ready");
				break;
			case State.SelectReward:
				m_MainUI.SetAnim(State.SelectReward);
				m_SelectPanel = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_StartReward, (result, obj) => {
					// 플레이 기본 데이터 셋팅
					SetRefreshChar();
					PlayEffSound(SND_IDX.SFX_1303);
					m_MainUI.SetAnim(State.Play);
					m_SUI.ScalePanelAnim.SetTrigger("3_Fight");
				}, 401).gameObject;
				break;
			case State.Play:
				Time.timeScale = PlayerPrefs.GetFloat(string.Format("PVP_ACC_{0}", USERINFO.m_UID), 1f);
				m_Action = TurnAtion();
				StartCoroutine(m_Action);
				break;
			case State.Result:
				Time.timeScale = 1f;
				StartCoroutine(IE_EndAction((Result)args[0] == Result.WIN));
				break;
			}
	}
	IEnumerator IE_EndAction(bool _win) {
		PopupName name = PopupName.NONE;
		if (m_Result == Result.Surrender) {
			SetEnd(_win);
		}
		else {
			if (m_FailCause == PVPFailCause.HP) {
				m_MainUI.GetAnim.SetTrigger("HPEnd");
				yield return new WaitForSeconds(1.5f);
				m_MainUI.GetAnim.SetTrigger("4_End");
				PlayEffSound(_win ? SND_IDX.SFX_1360 : SND_IDX.SFX_1361);
				yield return new WaitForSeconds(1f);
				SetEnd(_win);
			}
			else if (m_FailCause == PVPFailCause.Turn) name = PopupName.PVP_FailCause_CV;
			else name = PopupName.PVP_FailCause_SR;

			if (name != PopupName.NONE) {
				POPUP.Set_Popup(PopupPos.POPUPUI, name, null, m_FailCause, _win ? 1 : 0);
				yield return new WaitForSeconds(2.3f);
				m_MainUI.GetAnim.SetTrigger("4_End");
				PlayEffSound(_win ? SND_IDX.SFX_1360 : SND_IDX.SFX_1361);
				yield return new WaitForSeconds(1f);
				SetEnd(_win);
			}
		}
	}
	/// <summary> 종료 체크 </summary>
	void SetEnd(bool _win) {
		//종료 호출하고 끝나면 플레이로 돌아가기
		RES_PVP_USER_BASE info = PVPINFO.Users[0].m_Info;
		RES_PVP_USER_DETAIL preinfo = new RES_PVP_USER_DETAIL() {
			UserNo = info.UserNo,
			Name = info.Name,
			Profile = info.Profile,
			Nation = info.Nation,
			LV = info.LV,
			Rank = info.Rank,
			Power = info.Power,
			Point = info.Point
		};

		WEB.SEND_REQ_PVP_END((res) => {//RES_PVP_END
			if (res.IsSuccess()) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Result_Point, (result, obj) => {
					TPvPRankTable ranktdata = TDATA.GeTPvPRankTable(PVPINFO.m_Group.Rankidx);
					if (ranktdata.m_Idx == 103) {
						if (_win) SetRaid(res.GetRewards(), () => { MAIN.StateChange(MainState.PLAY); });
						else MAIN.StateChange(MainState.PLAY);
					}
					else {
						POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Result_League, (result, obj) => {
							if (_win) SetRaid(res.GetRewards(), () => { MAIN.StateChange(MainState.PLAY); });
							else MAIN.StateChange(MainState.PLAY);
						}, PVPINFO.m_Group.Rankidx, PVPINFO.m_Group.Users, res.Users);
					}
				}, preinfo, res, _win);
			}
			else if (res.result_code == EResultCode.ERROR_PVP_STATE) {
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10051));
				MAIN.StateChange(MainState.PLAY);
			}
		}, PVPINFO.Users[(int)UserPos.Target].m_Info.UserNo, _win, m_PlayUser[(int)UserPos.My].m_KillCnt, PVPINFO.m_CounterIdx);
	}
	void SetRaid(List<RES_REWARD_BASE> _rewards, Action _cb) {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Result_Raid, (res, obj) => { 
			_cb?.Invoke(); 
		}, _rewards);
	}
	public Item_PVP_Char GetChar(UserPos _userpos, int _pos) {
		return m_SUI.Decks[(_userpos == UserPos.My ? 0 : 10) + _pos];
	}
	public Item_SurvStat_PVP GetStat(UserPos _userpos, StatType _stat) {
		return m_SUI.Stats[3 * (int)_userpos + (int)_stat];
	}
	public void SetDmgStr(Vector3 _pos, PVPDmgType _type, int _dmg, bool _randpos, Vector3 _addpos = default(Vector3), Sprite _icon = null) {
		if (_type != PVPDmgType.Miss && _dmg == 0) return;
		Item_PVP_DMG eff = Utile_Class.Instantiate(m_SUI.DmgStr, m_SUI.FXBucket[1]).GetComponent<Item_PVP_DMG>();
		eff.SetData(_type, _dmg, _icon);
		eff.transform.position = new Vector3(_pos.x, _pos.y, 0f) + _addpos + (_randpos ? new Vector3(UTILE.Get_Random(-0.2f, 0.2f), UTILE.Get_Random(-0.2f, 0.2f), 0f) : Vector3.zero);
		//eff.transform.localScale = Vector3.one * 3f;
	}
	public GameObject StartEff(Vector3 position, string effPath) {
		FX eff = null;
		if (m_FXPool.ContainsKey(effPath)) {
			m_FXPool[effPath].RemoveAll(o => o.Obj == null);
			eff = m_FXPool[effPath].Find(o => !o.Obj.activeSelf);
		}
		if (eff == null) {
			GameObject obj = UTILE.LoadPrefab(effPath, true, m_SUI.FXBucket[1]);
			if (!m_FXPool.ContainsKey(effPath)) m_FXPool.Add(effPath, new List<FX>());
			eff = new FX() { Obj = obj, Scale = obj.transform.localScale, Rot = obj.transform.localEulerAngles };
			m_FXPool[effPath].Add(eff);
		}
		eff.Obj.transform.position = GetEffPos(position);
		eff.Obj.transform.localEulerAngles = eff.Rot;
		eff.Obj.transform.localScale = eff.Scale / m_SUI.FXBucket[1].localScale.x;
		eff.Obj.SetActive(true);
		return eff.Obj;
	}
	public Vector3 GetEffPos(Vector3 v3TargetPos) {
		// z의 -5 지점이기때문에 맞춰서 이펙트들이 커짐
		// 0지점의 위치를 찾아준다.

		// 스크린의 좌표을 알아낸다.
		Vector2 screenpos = Utile_Class.GetCanvasPosition(v3TargetPos);

		// 스크린좌표로부터 Ray 구해준다.
		Ray ray = m_MyCam.ScreenPointToRay(screenpos);

		// 0 지점 알아내기
		return ray.origin + ray.direction / ray.direction.z * Mathf.Abs(ray.origin.z);
	}
	public Vector3 GetStatPos(UserPos _userpos, StatType _type) {
		return m_SUI.Stats[(int)_userpos * 3 + (int)_type].transform.position;
	}
	/// <summary> 버프 적용시 우측 알람 </summary>
	public void SetBuffAlarm(StatType _stat, int _val) {
		if (m_AddStatAlarm == null) {
			EF_BuffCenterAlarm alarm = UTILE.LoadPrefab("Effect/EF_BuffCenterAlarm", true, POPUP.GetWorldUIPanel()).GetComponent<EF_BuffCenterAlarm>();
			m_AddStatAlarm = alarm;
		}
		m_AddStatAlarm.SetData(null, _stat, _val);
	}
	/// <summary> 배속 버튼 </summary>
	public void SetAccSwap() {
		float acc = PlayerPrefs.GetFloat(string.Format("PVP_ACC_{0}", USERINFO.m_UID), 1f);
		acc = acc == 1f ? 2f : 1f;
		Time.timeScale = acc;
		PlayerPrefs.SetFloat(string.Format("PVP_ACC_{0}", USERINFO.m_UID), acc);
		PlayerPrefs.Save();
	}
	public void CharSpeech(DialogueConditionType _type, Item_PVP_Char _user) {
		TConditionDialogueGroupTable table = _user.m_TData.GetSpeechTable(_type);
		if (table != null && table.IS_CanSpeech()) 
			m_MainUI.GetSpeech.SetData(Utile_Class.GetCanvasPosition(_user.transform.position + new Vector3(0f, 1.2f, 0f)), table, 1.5f);
	}
}
