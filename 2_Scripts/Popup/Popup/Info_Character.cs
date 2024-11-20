using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static LS_Web;

[System.Serializable] public class DIC_DNAColor : SerializableDictionary<DNABGType, Color> { }
public class Info_Character : PopupBase
{
	[System.Serializable]
	public struct SAcriveDNAUI
	{
		public GameObject Active;
		public Image Grade;
		public DIC_DNAColor GradeColor;
		public Image BG;
		public DIC_DNAColor BGColor;
		public Image Icon;
		public TextMeshProUGUI Name;
		public DIC_DNAColor NameColor;
	}
	[System.Serializable]
	public struct SEqUI {
		public Button Btn;
		public Item_Item_Card Item;
		public TextMeshProUGUI LockStr;
		public GameObject Eff;
		public GameObject CPGroups;
		public TextMeshProUGUI CPTxt;
	}

	[Serializable]
	public struct SDNAUI {
		public Button Btn;
		public Item_RewardDNA_Card Item;
		public Animator EffAnim;
		public GameObject Lock;
		public GameObject[] LockDeActives;
	}

	[Serializable]
	public struct SDUI {
		public Image[] DecoNums;
		public TextMeshProUGUI CharName;
		public Transform MainBucket;
		public Transform MainElement;//Item_Info_Char_DNAStat_Main
		public Item_Info_Char_DNAStat_None MainNone;
		public Transform SubBucket;
		public Transform SubElement; //Item_Info_Char_DNAStat_Sub
		public Item_Info_Char_DNAStat_None SubNone;
		public Transform SetBucket;
		public Transform SetElement; // Item_Info_Char_DNAStat_Set
		public GameObject DNABtnAlarm;
	}

	[Serializable]
	public struct SGradeUpButtonGroup {
		public GameObject Group;
		public Button GradeUpBtn;
	}

	[Serializable]
	public struct SButtonGroup {
		public GameObject Group;
		public Animator Anim;
	}

	[Serializable]
	public struct SJobGroup {
		public Image Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Desc;
		public TextMeshProUGUI Need;
	}

	[Serializable]
	public struct SSkillGroup {
		public Item_Skill_Card[] Skill;
		public GameObject[] SkillUpgradeAlarm;
		public TextMeshProUGUI[] Names;
		public TextMeshProUGUI[] Descs;
		public GameObject[] Stars;
		public GameObject[] SetGrow;
		public Color[] NameCol;
		public TextMeshProUGUI[] ActiveSkillAP;
		public TextMeshProUGUI[] ActiveSkillCool;
		public GameObject SetFX;
	}
	[Serializable]
	public struct SPVPGroup
	{
		public Image PosIcon;
		public Image WeaponIcon;
		public Image ArmorIcon;
		public GameObject[] SptOffObjs;
		public GameObject[] SptOnObjs;
		public TextMeshProUGUI PosName;
		public TextMeshProUGUI WeaponName;
		public TextMeshProUGUI ArmorName;
		public TextMeshProUGUI SkillName;
		public TextMeshProUGUI SkillDesc;
	}
	[Serializable]
	public struct SStatGroup
	{
		public TextMeshProUGUI ValueTxt;
		[FormerlySerializedAs("Group")] public GameObject LevelUpGroup;
		public Animator Anim;
		public TextMeshProUGUI LevelUpTxt;
	}

	[Serializable]
	public struct SStoryGroup
	{
		public TextMeshProUGUI StoryCntTxt;
		public TextMeshProUGUI CharNameTxt;
		public TextMeshProUGUI SpeachTxt;
		public TextMeshProUGUI DescTxt;
		public Item_StoryElement[] StoryElements;
	}

	[System.Serializable]
	public struct SUI
	{
		public GameObject[] SkillTabObj;
		public GameObject PVPTabObj;
		public Item_Tab[] Tabs;

		public ScrollRect[] ScrollRect;

		public TextMeshProUGUI CharName;
		public TextMeshProUGUI[] Lv;
		public TextMeshProUGUI[] Exp;
		public TextMeshProUGUI[] Money;
		public Image[] CharImgs;
		public SJobGroup JobGroup;
		public Item_GradeGroup GradeGroup;
		public Animator CPAnim;

		public SSkillGroup SkillGroup;
		public SPVPGroup PVPGroup;

		public SButtonGroup LvUpBtnGroup;

		//[ReName("CP", "HP", "Men", "Hyg", "Sat", "Atk", "Def", "Heal", "Critical", "Speed")]
		public SStatGroup[] StatGroups;
		public GameObject StatBonusElement;  //x:385, y:-366.6
		public TextMeshProUGUI GetBonus;
		//[ReName("Weapon", "Helmet", "Costume", "Shoes", "Accessory")]
		public SEqUI[] Equips;
		public RectTransform[] LVRanks;
		public SAcriveDNAUI ActiveDnaInfo;
		//[ReName("DNA1", "DNA2", "DNA3", "DNA4", "DNA5")]
		public SDNAUI[] DNAs;

		public SStoryGroup StoryGroup;
		
		public GameObject AutoEquipObject;
		public GameObject AutoUnequipObject;
		public GameObject PrioritySetEquipObject;
		public GameObject[] PrioritySetEquipCheck;

		public SGradeUpButtonGroup GradeUpBtnGroup;

		public GameObject ButtonStoryAlarm;
		public GameObject ButtonUpgradeAlarm;
		public GameObject ButtonSerumAlarm;
		public GameObject SerumLock;
		public TextMeshProUGUI SerumLockTxt;

		public TextMeshProUGUI RankUpMineTxt;
		public TextMeshProUGUI RankUpNeedTxt;
		
		public Animator Ani;
		public Animator NormalModeAnim;

		public GameObject[] ChangeBtn;
		public GameObject LvUpHoldAlarm;
		[Header("튜토리얼 전용")]
		public GameObject[] Panels;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SDUI m_SDUI;
	IEnumerator m_Action;
	private IEnumerator m_MoveTopAction;
	int m_Idx;
	int m_PreCP;
	public CharInfo m_Info;   //획득 캐릭터
	bool m_LvUpHold;
	int m_BeforLV;
	[SerializeField]
	float[] m_LvUpHoldTimer = new float[2] { 0f, 0.5f };
	int m_HoldCnt;
	int m_LvUpHoldAdd = 1;
	float[] m_LvUpHoldAlarmTimer = new float[4] { 0f, 3f, 0f, 4f };//0,1 알람 떠있는 시간, 2,3 터치 후 알람 등장 안했을때 횟수 초기화 시간
	int[] m_LvUpHoldCnt = new int[2] { 0, 3 };
	bool m_PrioritySetEquip;

	private Color enhancedColor;
	private Color normalColor;

	private StatType[] statTypes;
	private int[] curStats;
	private int[] befStats;
	private int[] diffStats;
	private int befLevel;

	private bool isInit = true;
	bool IS_LvUpAction;
	List<CharInfo> m_CharList = new List<CharInfo>();
	int m_StartInfoPagePos;
	enum State
	{
		Normal,
		DNA
	}
	State m_State;
	int GetCP { get { return m_StartInfoPagePos == 0 ? m_Info.m_CP : m_Info.m_PVPCP; } }

	private void Update() {
		if (m_LvUpHold) {
			m_LvUpHoldTimer[0] += Time.deltaTime;
			if (m_LvUpHoldTimer[0] > m_LvUpHoldTimer[1]) {
				if (m_SUI.LvUpHoldAlarm.activeSelf)
					SetLvUpHoldAlarm(false);
				m_LvUpHoldTimer[0] = 0;
				if(m_LvUpHoldTimer[1]  == 0.1f && m_HoldCnt == 5) {
					if (m_LvUpHoldAdd < 5) m_LvUpHoldAdd = 5;
					else if (m_LvUpHoldAdd < 10) m_LvUpHoldAdd = 10;
					m_HoldCnt = 0;
				}
				else
					m_LvUpHoldTimer[1] = Mathf.Max(0.1f, m_LvUpHoldTimer[1] - 0.1f);
				if (!AddLV(true)) {
					HoldEndLvUp();
				}
				else m_HoldCnt++;
			}
		}
		if(m_LvUpHoldCnt[0] == m_LvUpHoldCnt[1]) {
			SetLvUpHoldAlarm(true);
		}
		else if(m_LvUpHoldCnt[0] > 0) {
			m_LvUpHoldAlarmTimer[2] += Time.deltaTime;
			if(m_LvUpHoldAlarmTimer[2] > m_LvUpHoldAlarmTimer[3])
				SetLvUpHoldAlarm(false);
		}
		if (m_SUI.LvUpHoldAlarm.activeSelf) {
			m_LvUpHoldAlarmTimer[0] += Time.deltaTime;
			if (m_LvUpHoldAlarmTimer[0] > m_LvUpHoldAlarmTimer[1])
				SetLvUpHoldAlarm(false);
		}
	}
	void SetLvUpHoldAlarm(bool _active) {
		m_SUI.LvUpHoldAlarm.SetActive(_active);
		m_LvUpHoldCnt[0] = 0;
		m_LvUpHoldAlarmTimer[0] = m_LvUpHoldAlarmTimer[2] = 0f;
	}

	// 4.0 작업
	// TUDO : 스토리 작업
	// 스킬 배경 BG_Skill, 세트 착용시 : BG_Skill_Enhanced
	// 스킬 테두리 CardFrame2_0, 세트 착용시 : Frame_Skill_Enhanced 
	// 스킬 프레임 최적화

	/// <summary> aobjValue 0 : 캐릭터 인덱스 </summary>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_EndCB = cb;

		m_Idx = (int)aobjValue[0];
		m_CharList = (List<CharInfo>)aobjValue[1];
		m_StartInfoPagePos = (int)aobjValue[2];
		if (m_CharList.Count < 1) {
			m_SUI.ChangeBtn[0].SetActive(false);
			m_SUI.ChangeBtn[1].SetActive(false);
		}
		m_PreCP = 0;

		m_Info = USERINFO.GetChar(m_Idx);
		m_Info.m_GetAlarm = false;

		statTypes = new StatType[10];
		statTypes[1] = StatType.HP;
		statTypes[2] = StatType.Men;
		statTypes[3] = StatType.Hyg;
		statTypes[4] = StatType.Sat;
		statTypes[5] = StatType.Atk;
		statTypes[6] = StatType.Def;
		statTypes[7] = StatType.Heal;
		statTypes[8] = StatType.Critical;
		statTypes[9] = StatType.Speed;

		curStats = new int[10];
		befStats = new int[10];
		diffStats = new int[10];

		curStats[0] = befStats[0] = GetCP;
		for (int i = 1; i < curStats.Length; i++)
		{
			befStats[i] = curStats[i] = (int) m_Info.GetStat(statTypes[i], m_Info.m_LV, m_Info.m_Grade);
		}

		befLevel = m_Info.m_LV;

		enhancedColor = m_SUI.SkillGroup.Names[0].color;
		normalColor = m_SUI.SkillGroup.Names[1].color;

		m_SUI.NormalModeAnim.SetInteger("Grade", m_Info.m_Grade);

		StartCoroutine(IE_DNASlotAnimDelay());

		SND.StopAllVoice();
		SND_IDX vocidx = m_Info.m_TData.GetVoice(TCharacterTable.VoiceType.CharInfo);
		PlayVoiceSnd(new List<SND_IDX>() { vocidx });

		SetPrioritySetEquip(true);
		m_SUI.SkillGroup.SetFX.SetActive(false);

		SetLvUpHoldAlarm(false);

		base.SetData(pos, popup, cb, aobjValue);

		StartCoroutine(m_Action = CheckStartAction());
	}
	public void ChangeCharacter(bool _right) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 10)) return;
		if (m_Action != null) return;
		if (m_CharList.Count < 1) return;
		int pos = m_CharList.IndexOf(m_Info);

		if (_right) pos = (pos + 1) % m_CharList.Count;// pos < m_CharList.Count - 1 ? pos + 1 : 0;
		else pos = (pos - 1 + m_CharList.Count) % m_CharList.Count;// pos > 0 ? pos - 1 : m_CharList.Count - 1;

		int nextidx = m_CharList[pos].m_Idx;

		StartCoroutine(m_Action = CoMoveTop(() => {
			isInit = true;
			m_SUI.Ani.SetTrigger("Next");
			SetData(PopupPos.POPUPUI, PopupName.Info_Char, m_EndCB, new object[] { nextidx, m_CharList, m_StartInfoPagePos });
			m_Action = null;
		}));
	}
	IEnumerator CheckStartAction() {
		m_Action = null;
		yield return Utile_Class.CheckAniPlay(m_SUI.Ani);
		if (TUTO.IsTuto(TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.ViewCharInfo)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.CharGradeUP, (int)TutoType_CharGradeUP.ViewCharInfo)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Serum, (int)TutoType_Serum.ViewCharInfo)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.DNA, (int)TutoType_DNA.ViewCharInfo)) TUTO.Next();
	}

	public override void SetUI() {
		SetAllUI();
		base.SetUI();
	}
	void SetAllUI(bool _ignoreLvUp = false) {
		if (isInit) {
			var sprPortrait = TDATA.GetCharacterTable(m_Idx).GetPortrait();
			for (int i = 0; i < m_SUI.CharImgs.Length; i++) {
				m_SUI.CharImgs[i].sprite = sprPortrait;
			}

			var charTable = TDATA.GetCharacterTable(m_Idx);
			m_SUI.CharName.text = charTable.GetCharName();

			// 시너지 표시
			var synergyTable = TDATA.GetSynergyTable(charTable.m_Job[0]);
			m_SUI.JobGroup.Icon.sprite = synergyTable.GetIcon();
			m_SUI.JobGroup.Name.text = synergyTable.GetName();
			m_SUI.JobGroup.Desc.text = synergyTable.GetDesc();
			m_SUI.JobGroup.Need.text = synergyTable.m_NeedCount.ToString();

			m_SUI.Tabs[0].SetData(0, TDATA.GetString(921), ClickTab);
			m_SUI.Tabs[1].SetData(1, TDATA.GetString(922), ClickTab);
			m_SUI.Tabs[m_StartInfoPagePos].OnClick();
		}
		bool scrolllock = false;
		if (TUTO.IsTuto(TutoKind.CharGradeUP)) scrolllock = true;
		else if (TUTO.IsTuto(TutoKind.EquipCharLVUP)) scrolllock = true;
		//else if (TUTO.IsTuto(TutoKind.Serum)) scrolllock = true;
		else if (TUTO.IsTuto(TutoKind.DNA)) scrolllock = true;
		SetScrollLock(scrolllock);

		Utile_Class.DebugLog("CHAR UID " + m_Info.m_UID);

		SetGrade(m_Info.m_Grade);

		if(!_ignoreLvUp) SetLvUpUI();

		SetEquipSlot();

		SetSkillUI(true);

		SetPVPUI();

		SetActiveDANUI();

		SetDNASlot();

		SetDNAInfo();

		SetDNABtnAlarm();

		SetStoryUI();

		TCharacterGradeTable ranktable = TDATA.GetCharGradeTable(m_Info.m_Idx, m_Info.m_Grade);
		m_SUI.ButtonUpgradeAlarm.SetActive(m_Info.IS_CanRankUP());
		m_SUI.RankUpMineTxt.text = $"{USERINFO.GetItemCount(ranktable.m_MatIdx)}";
		m_SUI.RankUpNeedTxt.text = $"/{ranktable.m_MatCount}";

		TSerumBlockTable tdata = TDATA.GetSerumBlockTable(1);
		m_SUI.ButtonSerumAlarm.SetActive(USERINFO.CheckContentUnLock(ContentType.Serum) && m_Info.IS_CanSerum() && m_Info.m_LV >= tdata.m_NeedCharLv);
		//m_SUI.ButtonSerumAlarm.SetActive(TUTO.IsEndTuto(TutoKind.Serum) ? m_Info.IS_CanSerum() : false);

		m_SUI.SerumLock.SetActive(m_Info.m_LV < tdata.m_NeedCharLv);
		m_SUI.SerumLockTxt.text = string.Format(TDATA.GetString(1072), tdata.m_NeedCharLv);
		//int openserumidx = BaseValue.CONTENT_OPEN_IDX(ContentType.Serum);
		//m_SUI.SerumLock.SetActive(USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < openserumidx);
		//m_SUI.SerumLockTxt.text = string.Format("{0}-{1}", openserumidx / 100, openserumidx % 100);

		RefreshAutoEquipButton();
		var dnas = m_SUI.DNAs;
		for (int i = 0; i < dnas.Length; i++) {
			bool slotlock = !m_Info.m_DNASlot[i];
			//dnas[i].Btn.interactable = !slotlock;
			if(dnas[i].Lock != null) dnas[i].Lock.SetActive(slotlock);
			for(int j = 0; j < dnas[i].LockDeActives.Length; j++) {
				if (dnas[i].LockDeActives[j] != null) dnas[i].LockDeActives[j].SetActive(!slotlock);
			}
		}
		if (isInit) SetStatUI();

		SetGetBonus();

		isInit = false;
	}

	void SetGetBonus() {
		StringBuilder msg = new StringBuilder();
		TCharacterTable cdata = TDATA.GetCharacterTable(m_Idx);
		for (int i = 1; i < 3; i++) {
			TSkillTable stdata = TDATA.GetSkill(cdata.m_SkillIdx[i]);
			if (stdata == null) continue;
			if (stdata.GetStatType() == StatType.None)
				msg.Append(string.Format("[{0}]", stdata.GetInfo()));
			else if (stdata.GetStatType() != StatType.None)
				msg.Append(string.Format("[{0} +{1}]", TDATA.GetStatString(stdata.GetStatType()), cdata.GetPassiveStatValue(stdata.GetStatType(), m_Info == null ? 1 : m_Info.m_LV)));
			msg.Append("\n");
		}
		m_SUI.GetBonus.text = msg.ToString();
	}
	void SetActiveDANUI()
	{
		if(m_Info.m_EqDNAUID[0] != 0)
		{
			m_SUI.ActiveDnaInfo.Active.SetActive(true);

			DNAInfo info = USERINFO.GetDNA(m_Info.m_EqDNAUID[0]);
			TDnaTable tdata = info.m_TData;
			m_SUI.ActiveDnaInfo.BG.color = m_SUI.ActiveDnaInfo.BGColor[tdata.m_BGType];
			m_SUI.ActiveDnaInfo.Grade.color = m_SUI.ActiveDnaInfo.GradeColor[tdata.m_BGType];
			m_SUI.ActiveDnaInfo.Name.color = m_SUI.ActiveDnaInfo.NameColor[tdata.m_BGType];
			m_SUI.ActiveDnaInfo.Name.text = tdata.GetName();
			m_SUI.ActiveDnaInfo.Icon.sprite = tdata.GetIcon();
		}
		else m_SUI.ActiveDnaInfo.Active.SetActive(false);
	}

	public void SetScrollLock(bool _lock) {
		for (int i = 0; i < m_SUI.ScrollRect.Length; i++) {
			m_SUI.ScrollRect[i].enabled = !_lock;
			m_SUI.ScrollRect[i].GetComponent<EventTrigger>().enabled = !_lock;
		}
	}
	/// <summary> 
	/// 여유장비 있을때 장착된게 하나도 없거나 한부위라도 더 좋은 장비가 존재하면 자동장착 활성화 모두탈착 비활성화
	/// 여유 장비가 없으면 자동장착 비활성화 모두탈착은 모든 칸이 비어있을때 비활성화
	/// </summary>
	public void RefreshAutoEquipButton()
	{
		var Time = UTILE.Get_Time_Milli();
		// 버튼 표기법
		// 전투력만 높은게있으면 장착 표시
		bool IsEQChange = false;
		bool eqone = false;
		for (EquipType i = EquipType.Weapon; i < EquipType.Max; i++)
		{
			var eq = m_Info.GetEquip(i);// USERINFO.GetItem(m_Info.m_EquipUID[(int)i]);
			var teq = eq != null ? eq.m_TData : null;
			var cp = eq != null ? eq.m_CP : 0;
			ItemInfo temp = null;
			if (USERINFO.m_ItemDic.ContainsKey(i)) temp = USERINFO.m_ItemDic[i].Find(o =>
			{
				if (USERINFO.IsUseEquipItem(o.m_Uid)) return false;
				if (o.m_Uid == m_Info.m_EquipUID[(int)i]) return false;
				var tdata = o.m_TData;
				if (tdata.m_Value != 0 && tdata.m_Value != m_Idx) return false;
				// 전용장비 우선일경우 체크
				if (m_PrioritySetEquip)
				{
					if (teq != null)
					{
						// 장착장비가 전용 장비일때
						if (teq.m_Value != 0)
						{
							if (tdata.m_Value == 0) return false;
							// 체크 장비가 전용 장비일때
							return o.m_CP > cp;
						}
						// 전용 장비 착용 중이 아니라면 체크장비가 전용장비면 변경대상
						if (tdata.m_Value != 0) return true;
					}
				}
				return o.m_CP > cp;
			});
			if (temp != null)
			{
				IsEQChange = true;
				break;
			}
			if (m_Info.GetEquip(i) != null) eqone = true;
		}

		m_SUI.PrioritySetEquipObject.SetActive(IsEQChange);
		m_SUI.AutoEquipObject.SetActive(IsEQChange);
		m_SUI.AutoUnequipObject.SetActive(!IsEQChange && eqone);


		// 625.568115234375
		// 575.52294921875
		// 565.563720703125
		//bool is_uppercp = false;
		//bool is_unequip = true;

		//Dictionary<EquipType, List<ItemInfo>> equipinfos = USERINFO.GetUnEquipCPSort(m_Info, m_PrioritySetEquip);

		//bool is_nohaveequip = true;
		//for (EquipType i = EquipType.Weapon; i < EquipType.Max; i++)
		//{
		//	if (m_Info.m_EquipUID[(int)i] != 0) is_unequip = false;
		//	if (equipinfos[i].Count > 0)
		//	{
		//		is_nohaveequip = false;
		//		break;
		//	}
		//}

		//if (is_nohaveequip)
		//{
		//	m_SUI.PrioritySetEquipObject.SetActive(false);
		//	m_SUI.AutoEquipObject.SetActive(false);
		//	m_SUI.AutoUnequipObject.SetActive(!is_unequip);
		//}
		//else
		//{
		//	is_unequip = true;
		//	for (EquipType i = EquipType.Weapon; i < EquipType.Max; i++)
		//	{
		//		if (m_Info.m_EquipUID[(int)i] != 0)
		//		{
		//			is_unequip = false;
		//			ItemInfo item = USERINFO.GetItem(m_Info.m_EquipUID[(int)i]);
		//			if (equipinfos[i].Count > 0)
		//			{
		//				if (m_PrioritySetEquip)
		//				{
		//					if (equipinfos[i][0].m_TData.m_Value == m_Idx)
		//					{
		//						if (equipinfos[i][0].m_TData.m_Value == item.m_TData.m_Value && equipinfos[i][0].m_CP > item.m_CP) is_uppercp = true;
		//						else if (equipinfos[i][0].m_TData.m_Value != item.m_TData.m_Value) is_uppercp = true;
		//					}
		//					else
		//					{
		//						if (item.m_TData.m_Value == m_Idx) continue;
		//						else if (equipinfos[i][0].m_CP > item.m_CP) is_uppercp = true;
		//					}
		//				}
		//				else if (equipinfos[i][0].m_CP > item.m_CP) is_uppercp = true;
		//			}

		//			if (equipinfos[i].Count > 0 && equipinfos[i][0].m_CP > item.m_CP) {
		//				if (m_PrioritySetEquip && equipinfos[i][0].m_TData.m_Value != m_Idx) continue;
		//				is_uppercp = true;
		//			}
		//		}
		//		else
		//		{
		//			if (equipinfos[i].Count > 0)
		//			{
		//				if (m_PrioritySetEquip && equipinfos[i].Count > 0 && equipinfos[i][0].m_TData.m_Value != m_Idx) continue;
		//				is_uppercp = true;
		//			}
		//			else
		//			{
		//				is_unequip = false;
		//			}
		//		}
		//	}

		//	m_SUI.PrioritySetEquipObject.SetActive(is_unequip || is_uppercp);
		//	m_SUI.AutoEquipObject.SetActive(is_unequip || is_uppercp);
		//	m_SUI.AutoUnequipObject.SetActive(!is_unequip && !is_uppercp);
		//}

		Debug.Log($"Time Check {UTILE.Get_Time_Milli() - Time}");
	}

	public void RefreshSkillCard()
	{
		var isSetEquip = m_Info.IS_SetEquip();
		SkillType type = SkillType.Active;
		if (isSetEquip) type = SkillType.SetActive;
		int idx = TDATA.GetCharacterTable(m_Idx).m_SkillIdx[(int)SkillType.Active];
		int LV = m_Info.GetSkillLV(SkillType.Active);

		bool setactive = type == SkillType.SetActive;

		m_SUI.SkillGroup.Skill[0].SetData(idx, LV, false, setactive, m_Info, RefreshSkillCard);
		m_SUI.SkillGroup.SetGrow[0].SetActive(setactive);
		m_SUI.SkillGroup.Names[0].color = m_SUI.SkillGroup.NameCol[setactive ? 0 : 1];
		m_SUI.SkillGroup.Stars[0].SetActive(setactive);

		m_SUI.SkillGroup.SkillUpgradeAlarm[0].SetActive(m_Info.m_Skill[0].CheckLvUp());
	}
	void SetEquipSkillMsg() {
		if (m_SUI.SkillGroup.SetFX.activeSelf) return;
		int idx = 0;
		int LV = 0;
		TSkillTable skillTable = null;
		for (int i = 1; i < 3; i++) {
			if(i == 1) {
				idx = TDATA.GetCharacterTable(m_Idx).m_SkillIdx[(int)SkillType.Active];
				LV = m_Info.GetSkillLV(SkillType.Active);
				skillTable = TDATA.GetSkill(idx);
			}
			else {
				idx = TDATA.GetCharacterTable(m_Idx).m_SkillIdx[(int)SkillType.SetActive];
				LV = m_Info.GetSkillLV(SkillType.SetActive);
				skillTable = TDATA.GetSkill(idx);
			}
			m_SUI.SkillGroup.Skill[i].SetData(idx, LV, false, i == 2, m_Info, RefreshSkillCard);
			m_SUI.SkillGroup.SetGrow[i].SetActive(true);
			m_SUI.SkillGroup.Names[i].color = m_SUI.SkillGroup.NameCol[i == 2 ? 0 : 1];
			m_SUI.SkillGroup.Names[i].text = skillTable.GetName();
			m_SUI.SkillGroup.Descs[i].text = skillTable.GetInfo(LV, 1);
			m_SUI.SkillGroup.Stars[i].SetActive(i == 2);
			m_SUI.SkillGroup.ActiveSkillAP[i].text = m_Info != null ? m_Info.GetNeedAP(false).ToString() : skillTable.m_BaseAP.ToString();
			m_SUI.SkillGroup.ActiveSkillCool[i].text = skillTable.m_Cool.ToString();
		}
		StartCoroutine(SetEquipSkillMsgAction());
	}
	IEnumerator SetEquipSkillMsgAction() {
		m_SUI.SkillGroup.SetFX.SetActive(true);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.SkillGroup.SetFX.GetComponent<Animator>()));
		m_SUI.SkillGroup.SetFX.SetActive(false);
	}
	public void ClickEquipSkillMsgSkip() {
		Animator anim = m_SUI.SkillGroup.SetFX.GetComponent<Animator>();
		if (Utile_Class.IsAniPlay(anim) && !Utile_Class.IsAniPlay(anim, 85f / 272f) && Utile_Class.IsAniPlay(anim, 235f / 272f)) {
			anim.PlayInFixedTime("Start", 0, 235f / 60f);
		}
	}
	void SetGrade(int _limit) {
		m_SUI.GradeGroup.SetData(_limit);
		m_SUI.NormalModeAnim.SetInteger("Grade", _limit);
		m_SUI.GradeUpBtnGroup.Group.SetActive(m_Info.IS_EnoughRank());
	}
	void SetStatUI() {
		float power = 0;
		Dictionary<StatType, float> nowbonus = USERINFO.GetCharLvStatBonus();

		m_SUI.StatGroups[0].ValueTxt.text = GetCP.ToString();

		for (int i = 1; i < statTypes.Length; i++)
		{
			float val = m_Info.GetStat(statTypes[i]);
			if (nowbonus.ContainsKey(statTypes[i])) {//<color=#CA7100><voffset=0.1em><size=70%>(+{1}%)</size></voffset></color>
				m_SUI.StatGroups[i].ValueTxt.text = statTypes[i] == StatType.Critical ? string.Format("{0:0.0}%<color=#CA7100><voffset=0.1em><size=70%>(+{1}%)</size></voffset></color>", val * 100f, nowbonus[statTypes[i]] * 100f) : string.Format("{0}<color=#CA7100><voffset=0.1em><size=70%>(+{1})</size></voffset></color>", Mathf.RoundToInt(val), Mathf.RoundToInt(nowbonus[statTypes[i]]));
			}
			else m_SUI.StatGroups[i].ValueTxt.text = statTypes[i] == StatType.Critical ? string.Format("{0:0.0}%", val * 100f) : Mathf.RoundToInt(val).ToString();
		}

		for (int i = 0; i < statTypes.Length; i++)
		{
			bool isDiff = diffStats[i] != 0;
			if (!isInit)
			{
				if (isDiff)
				{
					bool isUp = diffStats[i] > 0;
					string str = diffStats[i].ToString("+#;-#;0");
					m_SUI.StatGroups[i].LevelUpTxt.text = str;
					m_SUI.StatGroups[i].LevelUpGroup.SetActive(true);
					m_SUI.StatGroups[i].Anim.SetTrigger(isUp ? "Up" : "Down");

					for (int j = 0; j < diffStats.Length; j++)
					{
						diffStats[i] = 0;
					}
				}
				else
				{
					m_SUI.StatGroups[i].LevelUpGroup.SetActive(false);
				}
			}
		}
		
		if (!isInit)
		{
			if (m_PreCP != 0 && m_PreCP < (int)power)
				m_SUI.CPAnim.SetTrigger("Up");
		}
		m_PreCP = (int)power;
	}
	void SetSkillUI(bool _init)
	{
		var isSetEquip = m_Info.IS_SetEquip();
		if (isSetEquip && !_init) {//세트장비 장착시 알림
			SetEquipSkillMsg();
		}
		
		SkillType type = SkillType.Active;
		if (isSetEquip) type = SkillType.SetActive;

		int idx = TDATA.GetCharacterTable(m_Idx).m_SkillIdx[(int)type];
		int LV = m_Info.GetSkillLV(SkillType.Active);

		TSkillTable skillTable = TDATA.GetSkill(idx);
		bool setactive = type == SkillType.SetActive;

		m_SUI.SkillGroup.Skill[0].SetData(idx, LV, false, setactive, m_Info, RefreshSkillCard);

		m_SUI.SkillGroup.SkillUpgradeAlarm[0].SetActive(m_Info.m_Skill[0].CheckLvUp());
		m_SUI.SkillGroup.Names[0].text = skillTable.GetName();
		m_SUI.SkillGroup.Descs[0].text = skillTable.GetInfo(LV, 1);

		m_SUI.SkillGroup.Names[0].color = m_SUI.SkillGroup.NameCol[setactive ? 0 : 1];
		m_SUI.SkillGroup.SetGrow[0].SetActive(setactive);
		m_SUI.SkillGroup.Stars[0].SetActive(setactive);
		m_SUI.SkillGroup.ActiveSkillAP[0].text = m_Info != null  ? m_Info.GetNeedAP(false).ToString() : skillTable.m_BaseAP.ToString();
		m_SUI.SkillGroup.ActiveSkillCool[0].text = skillTable.m_Cool.ToString();
	}
	void SetPVPUI() {
		TCharacterTable tdata = TDATA.GetCharacterTable(m_Idx);
		TPVPSkillTable stdata = TDATA.GeTPVPSkillTable(tdata.m_PVPSkillIdx);

		for (int i = 0; i < m_SUI.PVPGroup.SptOffObjs.Length; i++) {
			m_SUI.PVPGroup.SptOffObjs[i].SetActive(tdata.m_PVPPosType == PVPPosType.Combat);
		}
		for (int i = 0; i < m_SUI.PVPGroup.SptOnObjs.Length; i++) {
			m_SUI.PVPGroup.SptOnObjs[i].SetActive(tdata.m_PVPPosType != PVPPosType.Combat);
		}

		m_SUI.PVPGroup.PosIcon.sprite = BaseValue.GetPVPPosIcon(tdata.m_PVPPosType);
		m_SUI.PVPGroup.WeaponIcon.sprite = BaseValue.GetPVPEquipAtkIcon(stdata.m_AtkType);
		m_SUI.PVPGroup.ArmorIcon.sprite = BaseValue.GetPVPEquipDefIcon(tdata.m_PVPArmorType);
		m_SUI.PVPGroup.PosName.text = BaseValue.GetPVPPosName(tdata.m_PVPPosType);
		m_SUI.PVPGroup.WeaponName.text = BaseValue.GetPVPEquipAtkName(stdata.m_AtkType);
		m_SUI.PVPGroup.ArmorName.text = BaseValue.GetPVPEquipDefName(tdata.m_PVPArmorType);
		m_SUI.PVPGroup.SkillName.text = stdata.GetName();
		m_SUI.PVPGroup.SkillDesc.text = stdata.GetDesc();
	}
	void SetStoryUI()
	{
		var opensloct = BaseValue.CHAR_OPEN_STORY_SLOT(m_Info.m_Grade);
		m_SUI.StoryGroup.StoryCntTxt.text = (opensloct + 1).ToString();
		m_SUI.StoryGroup.CharNameTxt.text = m_Info.m_TData.GetCharName();
		m_SUI.StoryGroup.SpeachTxt.text = m_Info.m_TData.GetSpeech();
		m_SUI.StoryGroup.DescTxt.text = m_Info.m_TData.GetCharDesc();


		for (int i = 0; i < m_SUI.StoryGroup.StoryElements.Length; i++)
		{
			if (m_Info.m_TData.m_Story[i] < 1)
			{
				m_SUI.StoryGroup.StoryElements[i].gameObject.SetActive(false);
			}
			else
			{
				m_SUI.StoryGroup.StoryElements[i].SetData(m_Info, i, i > opensloct ? Item_StoryElement.State.Lock : m_Info.Story[i] ? Item_StoryElement.State.Viewed : Item_StoryElement.State.Open, StoryEndCB);
			}
		}
	}

	void StoryEndCB(int Slot)
	{
		if (m_Info.Story[Slot]) return;
#if NOT_USE_NET
		m_Info.Story[Slot] = true;

		SetStoryUI();
		var addCash = BaseValue.CHAR_OPEN_STORY_REWARD_CNT(Slot);
		USERINFO.GetCash(addCash);
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
		rewards.Add(new RES_REWARD_MONEY()
		{
			Type = Res_RewardType.Cash,
			Befor = USERINFO.m_Cash - addCash,
			Now = USERINFO.m_Cash,
			Add = addCash
		});
		MAIN.SetRewardList(new object[] { rewards }, null);
		//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardList, (result, obj) => { }, rewards);

#else
		WEB.SEND_REQ_CHAR_STORY((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					WEB.SEND_REQ_CHARINFO((res2) => {
						SetStoryUI();
					}, USERINFO.m_UID, m_Info.m_UID);
				});
				return;
			}
			SetStoryUI();
			if (res.Rewards == null) return;
			List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
			rewards.AddRange(res.GetRewards());
			if (rewards.Count > 0)
			{
				MAIN.SetRewardList(new object[] { rewards }, null);
				//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardList, (result, obj) => { }, rewards);
			}
		}, m_Info, Slot);
#endif
	}

	void SetLvUpUI() {
		m_SUI.Lv[0].text = m_Info.m_LV.ToString();
		m_SUI.Lv[1].text = $"/ {m_Info.m_LvMax}";

		m_SUI.ButtonUpgradeAlarm.SetActive(m_Info.IS_CanRankUP());

		m_SUI.LvUpBtnGroup.Anim.SetTrigger(m_Info.CheckLvUp(1).Key > 0 ? "Normal" : "Disabled");

		m_SUI.LvUpBtnGroup.Group.SetActive(true);

		m_SUI.Exp[0].text = USERINFO.m_Exp[(int)EXPType.Ingame].ToString();
		m_SUI.Exp[1].text = m_Info.m_ExpMax.ToString();
		m_SUI.Money[0].text = USERINFO.m_Money.ToString();
		m_SUI.Money[1].text = TDATA.GetExpTable(m_Info.m_LV).m_Money.ToString();
		m_SUI.Exp[0].color = BaseValue.GetUpDownStrColor((int)m_Info.USERINFO.m_Exp[(int)EXPType.Ingame], (int)m_Info.m_ExpMax);
		m_SUI.Money[0].color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, TDATA.GetExpTable(m_Info.m_LV).m_Money);

		if (!isInit)
		{
			if (befLevel != m_Info.m_LV)
			{
				m_SUI.Ani.SetTrigger("LvUp");
				SetBonusStatAlarm(m_Info.m_LV - befLevel);
				befLevel = m_Info.m_LV;
			}
		}
	}
	void SetEquipSlot() {//EquipLock 도 추가해라
		for (int i = 0; i < m_SUI.Equips.Length; i++) SetEquipSlot(i);
		SetEquipLVRank();
	}
	public int EquopTypeLVRankSortNo(EquipType type)
	{
		switch (type)
		{
		case EquipType.Helmet:		return 4;
		case EquipType.Costume:		return 1;
		case EquipType.Shoes:		return 3;
		case EquipType.Accessory:	return 2;
		}
		return 0;
	}

	void SetEquipLVRank()
	{
		// 유저가 보유한 장비 레벨업에 필요한 경험치 총량
		var totalexp = USERINFO.GetBagItems(3).Sum(o => o.GetExp() * o.m_Stack);
		// 레벨업 우선순위 1,2위
		// 레벨업이 가능한 아이템 체크
		var list = m_Info.GetEquipItems().FindAll(o => o.IS_LvUP() && o.GetNeedExp(o.m_Lv+1) <= totalexp);
		if (list.Count > 0)
		{
			list.Sort((befor, after) => {
				var bneedexp = befor.GetNeedExp(befor.m_Lv + 1);
				var aneedexp = after.GetNeedExp(after.m_Lv + 1);
				if (bneedexp != aneedexp) return bneedexp.CompareTo(aneedexp);
				var btypesortno = EquopTypeLVRankSortNo(befor.m_TData.GetEquipType());
				var atypesortno = EquopTypeLVRankSortNo(after.m_TData.GetEquipType());
				return btypesortno.CompareTo(atypesortno);
			});
		}
		for(int i = 0; i < 2; i++)
		{
			if (list.Count > i)
			{
				m_SUI.LVRanks[i].gameObject.SetActive(true);
				var rtf = (RectTransform)m_SUI.Equips[(int)list[i].m_TData.GetEquipType()].Btn.transform;
				m_SUI.LVRanks[i].position = rtf.position + new Vector3(94 * rtf.lossyScale.x, 94 * rtf.lossyScale.y, 0);
			}
			else m_SUI.LVRanks[i].gameObject.SetActive(false);
		}



	}

	void SetEquipSlot(int pos)
	{//EquipLock 도 추가해라
		bool IsOpen = m_Info != null;
		m_SUI.Equips[pos].Eff.SetActive(false);
		if (m_Info == null || m_Info.m_EquipUID[pos] == 0) {
			m_SUI.Equips[pos].Item.gameObject.SetActive(false);
			m_SUI.Equips[pos].CPGroups.SetActive(false);
		}
		else {
			ItemInfo item = USERINFO.GetItem(m_Info.m_EquipUID[pos]);
			if (item != null) {
				m_SUI.Equips[pos].Item.gameObject.SetActive(true);
				m_SUI.Equips[pos].Item.SetData(item);
				m_SUI.Equips[pos].CPTxt.text = item.m_CP.ToString();
				m_SUI.Equips[pos].CPGroups.SetActive(true);
			}
			else {
				m_SUI.Equips[pos].Item.gameObject.SetActive(false);
				m_SUI.Equips[pos].CPGroups.SetActive(false);
			}
		}

		m_SUI.Equips[pos].Btn.interactable = IsOpen;
	}
	
	void SetDNASlot() 
	{
		for (int i = 0; i < m_SUI.DNAs.Length; i++) SetDNASlot(i);
	}

	void SetDNASlot(int pos)
	{
		//m_SUI.DNAs[pos].EffAnim.gameObject.SetActive(false);
		DNAInfo dnainfo = USERINFO.GetDNA(m_Info.m_EqDNAUID[pos]);
		if (m_Info == null || dnainfo == null) {
			m_SUI.DNAs[pos].Item.gameObject.SetActive(false); 
			if(m_SUI.DNAs[pos].EffAnim.gameObject.activeInHierarchy) m_SUI.DNAs[pos].EffAnim.SetTrigger("NotEquip");
			//m_SUI.DNAs[pos].EffAnim.SetTrigger(DNABGType.None.ToString());
		}
		else {
			int idx = dnainfo.m_Idx;
			int grade = dnainfo.m_Grade;
			int lv = dnainfo.m_Lv;
			if (idx != 0) {
				m_SUI.DNAs[pos].Item.gameObject.SetActive(true);
				m_SUI.DNAs[pos].Item.SetData(idx, pos, lv, - 1, CallbackDnaSlot);
			}
			else m_SUI.DNAs[pos].Item.gameObject.SetActive(false);
			//m_SUI.DNAs[pos].EffAnim.SetTrigger("Equiped");
			TDnaTable data = TDATA.GetDnaTable(idx);
			if (m_SUI.DNAs[pos].EffAnim.gameObject.activeInHierarchy) m_SUI.DNAs[pos].EffAnim.SetTrigger(data.m_BGType.ToString());
		}
	}

	private void SetDNAInfo()
	{
		int deconum = m_Idx;
		int[] nums = new int[4];
		nums[0] = m_Idx / 1000;
		nums[1] = m_Idx % 1000 / 100;
		nums[2] = m_Idx % 100 / 10;
		nums[3] = m_Idx % 10;

		nums[0] = (nums[2] * nums[3]) % 10;
		nums[1] = (nums[2] + nums[3]) % 10;
		nums[2] = (nums[0] * nums[3]) % 10;
		nums[3] = (nums[1] + nums[2]) % 10;
		for (int i = 0; i < 4; i++) {
			m_SDUI.DecoNums[i].sprite = UTILE.LoadImg(string.Format("UI/UI_Serum/Font_Number_{0}", nums[i]), "png");
		}

		m_SDUI.CharName.text = m_Info.m_TData.GetCharName();

		//기본 옵션
		if (m_Info.m_EqDNAUID[0] > 0)
		{
			m_SDUI.MainBucket.gameObject.SetActive(true);
			m_SDUI.MainNone.gameObject.SetActive(false);
			Item_Info_Char_DNAStat_Main main = (m_SDUI.MainBucket.childCount > 0 ? m_SDUI.MainBucket.GetChild(0).gameObject : Utile_Class.Instantiate(m_SDUI.MainElement.gameObject, m_SDUI.MainBucket)).GetComponent<Item_Info_Char_DNAStat_Main>();
			main.SetData(USERINFO.GetDNA(m_Info.m_EqDNAUID[0]).m_Idx);
		}
		else
		{
			m_SDUI.MainBucket.gameObject.SetActive(false);
			m_SDUI.MainNone.gameObject.SetActive(true);
			m_SDUI.MainNone.SetData(false, TDATA.GetString(940));
		}

		//부가 옵션
		Dictionary<StatType, float> subvals = new Dictionary<StatType, float>();
		for (StatType i = StatType.Men; i< StatType.Max; i++) {
			float val = m_Info.GetDNAVal(i);
			if(val > 0f) subvals.Add(i, val);
		}
		UTILE.Load_Prefab_List(subvals.Count, m_SDUI.SubBucket, m_SDUI.SubElement);
		for (int i = 0; i < subvals.Count; i++) {
			Item_Info_Char_DNAStat_Sub sub = m_SDUI.SubBucket.GetChild(i).GetComponent<Item_Info_Char_DNAStat_Sub>();
			var stat = subvals.ElementAt(i);
			sub.SetData(stat.Key, stat.Value);
		}
		m_SDUI.SubBucket.gameObject.SetActive(subvals.Count > 0);
		m_SDUI.SubNone.gameObject.SetActive(subvals.Count < 1);
		m_SDUI.SubNone.SetData(false, TDATA.GetString(938));
		//세트옵션
		m_Info.CheckDNASetFX();
		Dictionary<DNABGType, int> setvals = m_Info.m_EquipDNASetCnt;
		for (int i = setvals.Count - 1; i > -1; i--) {
			TDNASetEffectTable tdata = TDATA.GetDNASetFXTable(setvals.ElementAt(i).Key, setvals.ElementAt(i).Value);
			if (tdata == null) setvals.Remove(setvals.ElementAt(i).Key);
		}
		UTILE.Load_Prefab_List(Mathf.Max(1, setvals.Count), m_SDUI.SetBucket, m_SDUI.SetElement);
		if (setvals.Count > 0) {
			for (int i = 0; i < setvals.Count; i++) {
				Item_Info_Char_DNAStat_Set set = m_SDUI.SetBucket.GetChild(i).GetComponent<Item_Info_Char_DNAStat_Set>();
				var stat = setvals.ElementAt(i);
				set.SetData(stat.Key, stat.Value);
			}
		}
		else {
			Item_Info_Char_DNAStat_Set set = m_SDUI.SetBucket.GetChild(0).GetComponent<Item_Info_Char_DNAStat_Set>();
			set.SetData(DNABGType.None);
		}
	}
	void SetDNABtnAlarm() {
		bool canset = false;
		for(int i = 0;i< m_Info.m_DNASlot.Length; i++) {
			if(m_Info.m_DNASlot[i] && m_Info.m_EqDNAUID[i] == 0) {
				canset = true;
				break;
			}
		}
		
		m_SDUI.DNABtnAlarm.SetActive(canset && USERINFO.m_DNAs.Count - USERINFO.m_EqDNAs.Count > 0 && USERINFO.CheckContentUnLock(ContentType.CharDNA, false));
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 레벨 업

	bool AddLV(bool ViewPopup = false) {
		var check = m_Info.CheckLvUp(m_LvUpHoldAdd);
		int add = check.Key;
		if(add == 0) {
			while (add == 0 && m_LvUpHoldAdd > 0) {
				add = m_Info.CheckLvUp(--m_LvUpHoldAdd).Key;
			}
		}
		m_LvUpHoldAdd = add;
		if (m_LvUpHoldAdd == 0)
		{
			if(ViewPopup)
			{
				if (check.Value == 0)//레벨 맥스
				{
					if (m_Info.IS_EnoughMaxLv())
					{
						POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, "", TDATA.GetString(94));
					}
					else
					{
						POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, "", TDATA.GetString(368));
					}
				}
				else if(check.Value == 1) {//경험치 미달
					POPUP.StartLackPop(BaseValue.EXP_IDX);
				}
				else if(check.Value == 2) {//달러 미달
					POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
				}
				//else if (USERINFO.m_Exp[(int)EXPType.Ingame] < TDATA.GetExpTable(m_Info.m_LV).m_Exp)
				//{
				//	//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, "", TDATA.GetString(367));
				//}
				//else if (USERINFO.m_Money < TDATA.GetExpTable(m_Info.m_LV).m_Money) {
				//	POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
				//	//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, "", TDATA.GetString(366));
				//}
			}
			PlayCommVoiceSnd(VoiceType.Fail);
			return false;
		}

		PlayEffSound(SND_IDX.SFX_0111);

		befLevel = m_Info.m_LV;

		m_Info.SetLvUp(m_LvUpHoldAdd);
		SetGrade(m_Info.m_Grade);

		UpdateStats();

		SetStatUI();
		SetLvUpUI();
		SetGetBonus();

		return true;
	}

	void SendCharLVUP(int LV) {
		if (LV < 1) return;
		IS_LvUpAction = true;
		PlayEffSound(SND_IDX.SFX_0111);
		if(LV == 1 && !m_SUI.LvUpHoldAlarm.activeSelf)
			m_LvUpHoldCnt[0]++;
#if NOT_USE_NET
		USERINFO.Check_Mission(MissionType.CharLevelUp, 0, 0, LV);
		USERINFO.m_Achieve.Check_Achieve(AchieveType.Character_LevelUp_Count, 0, LV);
		USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.Character_Level_Count, m_BeforLV, m_Info.m_LV);
		UpdateStats();
		SetStatUI();
		SetUI();
#else
		WEB.SEND_REQ_CHAR_LVUP((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					WEB.SEND_REQ_CHARINFO((res2) => {
						befLevel = m_Info.m_LV;
						SetAllUI(true);
					}, USERINFO.m_UID, m_Info.m_UID);
				});
				return;
			}

			befLevel = m_Info.m_LV;
			UpdateStats();
			SetStatUI();
			SetAllUI(true);
			IS_LvUpAction = false; 
			DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);

			if (m_Info.m_LV >= TDATA.GetSerumBlockTable(1).m_NeedCharLv && PlayerPrefs.GetInt($"SERUM_GUIDE_{USERINFO.m_UID}", 0) == 0) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.SerumGuide, (res, obj) => { PlayerPrefs.SetInt($"SERUM_GUIDE_{USERINFO.m_UID}", 1); });
			}
		}, m_Info.m_UID, m_Info, m_BeforLV);
#endif
	}

	public void HoldLvUp() {
		if (m_Info == null) return;
		if (m_LvUpHold) return;
		if (IS_LvUpAction) return;
		//if (!m_Info.CheckLvUp()) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 2)) return;
		m_LvUpHold = true;
		m_BeforLV = m_Info.m_LV;
		m_LvUpHoldTimer[0] = 0f;
		m_LvUpHoldTimer[1] = 0.5f;
		m_LvUpHoldAdd = 1;
		m_HoldCnt = 0;
		if (!AddLV(true)) {
			HoldEndLvUp();
		}
	}

	public void HoldEndLvUp() {
		if (!m_LvUpHold) return;
		if (IS_LvUpAction) return;
		int AddLV = m_Info.m_LV - m_BeforLV;
		m_LvUpHold = false;
		SendCharLVUP(AddLV);
	}

	public void ClickLvUp()
	{
		if (m_Action != null) return;
		if (m_LvUpHold) return;
		if (IS_LvUpAction) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 2)) return;
		m_BeforLV = m_Info.m_LV;
		if (AddLV(true))
		{
			SendCharLVUP(1);
			m_SUI.LvUpBtnGroup.Anim.SetTrigger("Pressed");
		}

	}

	public void ClickRankUP()
	{
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 3)) return;
		if (TUTO.IsTuto(TutoKind.CharGradeUP, (int)TutoType_CharGradeUP.Focus_GradeUPMenu)) TUTO.Next();

		int prerank = m_Info.m_Grade;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.CharUpgrade, (result, obj) => {
			if (prerank != m_Info.m_Grade) {
				m_SUI.Lv[0].text = m_Info.m_LV.ToString();
				m_SUI.Lv[1].text = string.Format("/{0}", USERINFO.m_LV);
				SetGrade(m_Info.m_Grade);
				UpdateStats();
				SetStatUI();
				SetUI();
			}
		}, m_Info);
	}
	public void ClickEquipSlot(int _idx)
	{
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 4, (EquipType)_idx)) return;
		var isSetEquip = m_Info.IS_SetEquip();

		if (m_Info.m_EquipUID[_idx] != 0) {
			long idx = m_Info.m_EquipUID[_idx];
			//장착 되어 있는 경우
			Action<Info_Item.InfoChange, object[]> changeInfo = (state, args) => {
				UpdateStats();
				if (state == Info_Item.InfoChange.Equip || state == Info_Item.InfoChange.LVUP) {
					// 장착 연출
					m_Action = EqAction(new List<EquipType>() { (EquipType)_idx }, idx != m_Info.m_EquipUID[_idx]);
					StartCoroutine(m_Action);
				}
				else {
					SetStatUI();
					SetSkillUI(isSetEquip);
					m_Action = null;
				}
			};
			POPUP.ViewItemInfo((result, obj) => {
				if (TUTO.IsTuto(TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.Focus_EquipUpgradeExit)) TUTO.Next();
			}, new object[]{ USERINFO.GetItem(m_Info.m_EquipUID[_idx]), m_Popup, changeInfo });
		}
		else {
			//슬롯 비어있는 경우
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.EquipChange, (result, obj) =>
			{
				if (result == 1)
				{
					UpdateStats();
					m_Action = EqAction(new List<EquipType>() { (EquipType)_idx }, !isSetEquip);
					StartCoroutine(m_Action);
				}
			}, (EquipType)_idx, null, m_Info);
		}
	}

#pragma warning disable 0162
	public void ClickStory()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 9)) return;
		POPUP.Set_MsgBox(PopupName.Msg_CommingSoon, string.Empty, TDATA.GetString(185));
		return;
		StartCoroutine(CoMoveTop(()=> { m_SUI.Ani.SetTrigger("NormalToStory"); }));
	}
#pragma warning restore 0162

	public void OnBtnCloseStoryClicked()
	{
		m_SUI.Ani.SetTrigger("StoryToNormal");
	}

	public void CallbackDnaSlot(object sender, int pos)
	{
		ClickDnaSlot(pos);
	}
	
	public void ClickDnaSlot(int pos)
	{
		if (m_Action != null) return;
		if (m_SUI.DNAs[pos].Lock != null && m_SUI.DNAs[pos].Lock.activeSelf) {
			int shopidx = 0;
			switch (pos) {
				case 2: shopidx = BaseValue.SHOP_IDX_DNA_SLOT_OPEN_3; break;
				case 3: shopidx = BaseValue.SHOP_IDX_DNA_SLOT_OPEN_4; break;
				case 4: shopidx = BaseValue.SHOP_IDX_DNA_SLOT_OPEN_5; break;
			}
			TShopTable shopdata = TDATA.GetShopTable(shopidx);
			string msg = Utile_Class.StringFormat(TDATA.GetString(790), BaseValue.GetPriceTypeName(shopdata.m_PriceType, shopdata.m_PriceIdx));
			POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, msg, (result, obj) => {
				if (result == 1) {
					if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
						WEB.SEND_REQ_CHAR_DNA_SLOTOPEN((res) => {
							if (res.IsSuccess()) {
								m_SUI.DNAs[pos].Lock.SetActive(false);
								for (int i = 0; i < m_SUI.DNAs[pos].LockDeActives.Length; i++) {
									if (m_SUI.DNAs[pos].LockDeActives[i] != null) m_SUI.DNAs[pos].LockDeActives[i].SetActive(true);
								}
							}
						}, m_Info, pos);
					}
					else {
						POPUP.StartLackPop(shopdata.GetPriceIdx());
					}
				}
			}, shopdata.m_PriceType, shopdata.m_PriceIdx, shopdata.GetPrice(), false);
			//슬롯 확장 프로토콜
		}
		else {
			DNAInfo dinfo = USERINFO.GetDNA(m_Info.m_EqDNAUID[pos]);
			if (dinfo == null) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNASelect, (result, obj) => {
					if (result == 1) {
						m_SUI.DNAs[pos].EffAnim.SetTrigger("Equip");
					}
					if (m_Info.m_EqDNAUID[pos] != 0) {
						UpdateStats();
						SetStatUI();
						SetActiveDANUI();
					}
					SetDNABtnAlarm();
				}, pos, m_Info);
			}
			else {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_DNA, (result, obj) => {
					switch(result)
					{
					case 1: m_SUI.DNAs[pos].EffAnim.SetTrigger("Equip"); break;
					case 2: SetDNASlot(pos); break;
					}
					if (m_Info.m_EqDNAUID[pos] != dinfo.m_UID) {
						UpdateStats();
						SetStatUI();
						SetActiveDANUI();
					}
					SetDNABtnAlarm();
				}, dinfo, m_Info, pos);
			}
		}
	}

	IEnumerator EqAction(List<EquipType> items, bool _sfx = true)
	{
		SetEquipLVRank();
		for (int i = 0; i < items.Count; i++)
		{
			StartCoroutine(CheckEqChangAni((int)items[i]));
			if(_sfx) PlayEffSound(SND_IDX.SFX_0004);
			yield return new WaitForSeconds(0.2f);
		}

		// 전투력만 변경이 되므로 SetUI대신 호출함
		// SetUI를 호출할경우 이펙트들이 자동으로 꺼지게됨
		SetStatUI();
		SetSkillUI(!_sfx);
		m_Action = null;


		if (TUTO.IsTuto(TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.Focus_AutoEquip)) TUTO.Next();
	}

	IEnumerator CheckEqChangAni(int pos)
	{
		SetEquipSlot(pos);
		Animator ani = m_SUI.Equips[pos].Item.GetComponent<Animator>();
		// 장착 변경
		ani.SetTrigger("Equip");
		yield return new WaitForSeconds(22f / 60f);
		m_SUI.Equips[pos].Eff.SetActive(true);
		yield return new WaitForSeconds(3f);
		m_SUI.Equips[pos].Eff.SetActive(false);
	}
	public void SwapPrioritySetEquip() {
		m_PrioritySetEquip = !m_PrioritySetEquip;
		POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, m_PrioritySetEquip ? TDATA.GetString(614) : TDATA.GetString(615));
		SetPrioritySetEquip(m_PrioritySetEquip);
	}
	void SetPrioritySetEquip(bool _priority) {
		m_PrioritySetEquip = _priority;
		for(int i = 0;i< m_SUI.PrioritySetEquipCheck.Length;i++) m_SUI.PrioritySetEquipCheck[i].SetActive(m_PrioritySetEquip);
	}
	public void OnAutoEqSet()
	{
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 5)) return;
		var isSetEquip = m_Info.IS_SetEquip();
		Dictionary<EquipType, List<ItemInfo>> equipinfos = USERINFO.GetUnEquipCPSort(m_Info, m_PrioritySetEquip);
		List<EquipType> change = new List<EquipType>();
		Dictionary<EquipType, long> changeuid = new Dictionary<EquipType, long>(){
			{ EquipType.Weapon, 0 },
			{ EquipType.Helmet, 0 },
			{ EquipType.Costume, 0 },
			{ EquipType.Shoes, 0 },
			{ EquipType.Accessory, 0 }
		};
		for (EquipType i = EquipType.Weapon; i < EquipType.Max; i++)
		{
			var eq = m_Info.GetEquip(i); //USERINFO.GetItem(m_Info.m_EquipUID[(int)i]);
			var teq = eq != null ? eq.m_TData : null;
			var cp = eq != null ? eq.m_CP : 0;
			ItemInfo temp = null;
			if (USERINFO.m_ItemDic.ContainsKey(i))
			{
				var list = USERINFO.m_ItemDic[i].FindAll(o =>
				{
					if (USERINFO.IsUseEquipItem(o.m_Uid)) return false;
					if (o.m_Uid == m_Info.m_EquipUID[(int)i]) return false;
					var tdata = o.m_TData;
					if (tdata.m_Value != 0 && tdata.m_Value != m_Idx) return false;
					// 전용장비 우선일경우 체크
					if (m_PrioritySetEquip)
					{
						if(teq != null)
						{
							// 장착장비가 전용 장비일때
							if (teq.m_Value != 0)
							{
								if (tdata.m_Value == 0) return false;
								// 체크 장비가 전용 장비일때
								return o.m_CP > cp;
							}
							// 전용 장비 착용 중이 아니라면 체크장비가 전용장비면 변경대상
							if (tdata.m_Value != 0) return true;
						}
					}
					return o.m_CP > cp;
				});
				//for (int j = 0; j < list.Count; j++) list[j].GetCombatPower();
				list.Sort((before, after) =>
				{
					if (m_PrioritySetEquip)
					{
						bool beset = before.m_TData.m_Value == m_Idx;
						bool afset = after.m_TData.m_Value == m_Idx;
						if (beset != afset) return afset.CompareTo(beset);
					}
					int bpower = before.m_CP;
					int apower = after.m_CP;
					if (before.m_CP != after.m_CP) return after.m_CP.CompareTo(before.m_CP);
					return before.m_Idx.CompareTo(after.m_Idx);

				});
				temp = list.Count > 0 ? list[0] : null;
			}
			if (temp != null)
			{
				changeuid[i] = temp.m_Uid;
				if (eq != null) USERINFO.RemoveEquipUID(eq.m_Uid);
				m_Info.m_EquipUID[(int)temp.m_TData.GetEquipType()] = temp.m_Uid;
				USERINFO.AddEquipUID(temp.m_Uid);
				change.Add(i);
			}
		}

		//for (EquipType i = EquipType.Weapon; i < EquipType.End; i++) {
		//	ItemInfo eqitem = m_Info.GetEquip(i);
		//	int power = eqitem != null ? eqitem.m_CP : 0;
		//	//본인 전용 장비 or 공용 장비
		//	List<ItemInfo> canequips = equipinfos[i].FindAll(o => o.m_TData.m_Value == 0 || o.m_TData.m_Value == m_Info.m_Idx);
		//	if (canequips.Count > 0) {
		//		bool is_uppercp = false;
		//		ItemInfo changeitem = changeitem = canequips[0];
		//		if (eqitem != null) {
		//			if (canequips.Count > 0) {
		//				if (m_PrioritySetEquip) {
		//					if (changeitem.m_TData.m_Value == m_Idx) {
		//						if (changeitem.m_TData.m_Value == eqitem.m_TData.m_Value && changeitem.m_CP > eqitem.m_CP) is_uppercp = true;
		//						else if (changeitem.m_TData.m_Value != eqitem.m_TData.m_Value) is_uppercp = true;
		//					}
		//					else {
		//						if (eqitem.m_TData.m_Value == m_Idx) continue;
		//						else if (changeitem.m_CP > eqitem.m_CP) is_uppercp = true;
		//					}
		//				}
		//				else if (changeitem.m_CP > eqitem.m_CP) is_uppercp = true;
		//			}
		//		}
		//		else {
		//			if (equipinfos[i].Count > 0) {
		//				//if (m_PrioritySetEquip && equipinfos[i].Count > 0 && equipinfos[i][0].m_TData.m_Value != m_Idx) continue;
		//				is_uppercp = true;
		//			}
		//		}
		//		if (is_uppercp) {
		//			changeuid[i] = changeitem.m_Uid;
		//			if (eqitem != null) USERINFO.RemoveEquipUID(eqitem.m_Uid);
		//			m_Info.m_EquipUID[(int)changeitem.m_TData.GetEquipType()] = changeitem.m_Uid;
		//			USERINFO.AddEquipUID(changeitem.m_Uid);
		//			change.Add(i);
		//		}
		//	}
		//}
#if NOT_USE_NET
		for (EquipType i = EquipType.Weapon; i < EquipType.End; i++) {
			if (changeuid[i] != 0) {
				ItemInfo eqitem = m_Info.GetEquip(i);
				if (eqitem != null) USERINFO.RemoveEquipUID(eqitem.m_Uid);
				m_Info.m_EquipUID[(int)i] = changeuid[i];
				USERINFO.AddEquipUID(changeuid[i]);
			}
		}

		if (change.Count < 1) return;
		MAIN.Save_UserInfo();
		UpdateStats();
		SetSkillUI(true);
		m_Action = EqAction(change);
		StartCoroutine(m_Action);
		RefreshAutoEquipButton();
#else
		List<long> changeuids = new List<long>(changeuid.Values);
		changeuids = changeuids.FindAll(o => o > 0);
		WEB.SEND_REQ_CHAR_EQUIP((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					WEB.SEND_REQ_CHARINFO((res2) => {
						SetUI();
					}, USERINFO.m_UID, m_Info.m_UID);
				});
				return;
			}
			m_Action = EqAction(res.EquipPos, !isSetEquip);
			StartCoroutine(m_Action);
			RefreshAutoEquipButton();
		}, m_Info.m_UID, changeuids);
#endif
	}
	
	public void OnAutoUnEqSet() {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 5)) return;
		// 전체 장비중인지 확인
		List<ItemInfo> equipItems = new List<ItemInfo>();
		for (EquipType i = EquipType.Weapon; i < EquipType.End; i++)
		{
			ItemInfo eqitem = m_Info.GetEquip(i);
			if (eqitem != null)
			{
				equipItems.Add(eqitem);
			}
		}

		if (!(USERINFO.m_InvenUseSize + equipItems.Count < USERINFO.m_InvenSize))
		{
			WEB.StartErrorMsg(EResultCode.ERROR_INVEN_SIZE);
			return;
		}

#if NOT_USE_NET
		for (int i = 0; i < equipItems.Count; i++)
		{
			USERINFO.RemoveEquipUID(equipItems[i].m_Uid);
		}
		
		UpdateStats();
		SetEquipSlot();
		SetStatUI();
		SetSkillUI(false);
		RefreshAutoEquipButton();

		PlayEffSound(SND_IDX.SFX_0005);

		MAIN.Save_UserInfo();
#else
		WEB.SEND_REQ_CHAR_UNEQUIPALL((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					WEB.SEND_REQ_CHARINFO((res2) => {
						SetUI();
					}, USERINFO.m_UID, m_Info.m_UID);
				});
				return;
			}
			UpdateStats();
			SetEquipSlot();
			SetStatUI();
			SetSkillUI(true);
			RefreshAutoEquipButton();

			PlayEffSound(SND_IDX.SFX_0005);
		}, m_Info.m_UID);
#endif
	}

	public void OnBtnSwitchDNAClicked()
	{
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 6)) return;
		int openidx = BaseValue.CONTENT_OPEN_IDX(ContentType.CharDNA);
		if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < openidx) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(273), openidx / 100, openidx % 100, TDATA.GetString(325)));
			PlayCommVoiceSnd(VoiceType.Fail);
			return;
		}

		StartCoroutine(m_Action = CoMoveTop(()=> {
			m_State = State.DNA;
			m_SUI.Ani.SetTrigger("ToDNA");
			PlayEffSound(SND_IDX.SFX_1070);
			StartCoroutine(IE_DNASlotAnimDelay());
			m_Action = null;
			if (TUTO.IsTuto(TutoKind.DNA, (int)TutoType_DNA.Select_DNABtn)) TUTO.Next();
		}));
	}
	IEnumerator IE_DNASlotAnimDelay() {
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < m_SUI.DNAs.Length; i++) {
			yield return new WaitWhile(() => m_SUI.DNAs[i].EffAnim.gameObject.activeInHierarchy == false);
			//m_SUI.DNAs[i].EffAnim.SetTrigger(DNABGType.None.ToString());
			DNAInfo dnainfo = USERINFO.GetDNA(m_Info.m_EqDNAUID[i]);
			if (m_Info == null || dnainfo == null)
				m_SUI.DNAs[i].EffAnim.SetTrigger("NotEquip");
			else
				m_SUI.DNAs[i].EffAnim.SetTrigger("Equiped");
		}
	}
	public void OnBtnSwitchEquipClicked()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 7)) return;

		if (m_Action != null) return;
		StartCoroutine(m_Action = CoMoveTop(() => {
			m_State = State.Normal; 
			m_SUI.Ani.SetTrigger("ToNormal");
			m_Action = null;
		}, ()=> {
			SetGrade(m_Info.m_Grade);
			SetLvUpUI();
			SetStatUI();
		}));
	}

	/// <summary> 상세 정보창 </summary>
	public void ClickDetailInfo()
	{
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 0)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_Char_Detail, null, m_Info);
	}

	public void OnBtnClickSynergyAll()
	{
		ClickSynergyInfo(m_Info.m_TData.m_Job[0]);
	}
	
	public void ClickSynergyInfo(JobType _type)
	{
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 1)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Synergy_All, null, _type);
	}
	public override void Close(int Result = 0)
	{
		if (m_Action != null) return;
		SND.StopAllVoice();
		//if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		base.Close(1);
	}

	public void OnBtnTopClicked()
	{
		if (m_MoveTopAction != null)
		{
			return;
		}
		m_MoveTopAction = CoMoveTop();
		StartCoroutine(m_MoveTopAction);
	}

	private IEnumerator CoMoveTop(Action _cb = null, Action _anicb = null)
	{
		var content = m_SUI.ScrollRect[(int)m_State].content;
		m_SUI.ScrollRect[(int)m_State].StopMovement();
		m_SUI.ScrollRect[(int)m_State].velocity = Vector2.zero;

		if (m_SUI.ScrollRect[(int)m_State].verticalNormalizedPosition != 1f) {
			iTween.ValueTo(gameObject, iTween.Hash(
				"from", m_SUI.ScrollRect[(int)m_State].verticalNormalizedPosition,
				"to", 1f,
				"time", 0.5f,
				"easetype", "easeInOutBack",
				"onupdate", "MoveTop"));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		}
		_cb?.Invoke();
		m_MoveTopAction = null;
		yield return new WaitForEndOfFrame();
		_anicb?.Invoke();
	}
	
	private void MoveTop(float yPos)
	{
		m_SUI.ScrollRect[(int)m_State].StopMovement();
		m_SUI.ScrollRect[(int)m_State].verticalNormalizedPosition = yPos;
	}

	public void UpdateStats()
	{
		for (int i = 0; i < befStats.Length; i++)
		{
			befStats[i] = curStats[i];
		}
		
		curStats[0] = GetCP;
		for (int i = 1; i < statTypes.Length; i++)
		{
			curStats[i] = Mathf.RoundToInt(m_Info.GetStat(statTypes[i]));
		}
		
		for (int i = 0; i < statTypes.Length; i++)
		{
			diffStats[i] = (curStats[i] - befStats[i]);
		}
	}
	void SetBonusStatAlarm(int _diff) {
		if (_diff < 1) return;
		int sumlv = USERINFO.m_Chars.Sum(o => o.m_LV);
		Dictionary<StatType, float> prebonus = USERINFO.GetCharLvStatBonus(sumlv - _diff);
		Dictionary<StatType, float> nowbonus = USERINFO.GetCharLvStatBonus(sumlv);
		Dictionary<StatType, float> diffbonus = new Dictionary<StatType, float>();
		for (int i = 0; i < nowbonus.Count; i++) {
			StatType type = nowbonus.ElementAt(i).Key;
			if (!prebonus.ContainsKey(type)) diffbonus.Add(type, nowbonus[type]);
			else if (prebonus[type] < nowbonus[type]) diffbonus.Add(type, nowbonus[type] - prebonus[type]);
		}
		if (diffbonus.Count > 0) {
			Item_LvBonus_Alarm bonusalarm = Utile_Class.Instantiate(m_SUI.StatBonusElement, transform).GetComponent<Item_LvBonus_Alarm>();
			bonusalarm.SetData(diffbonus, sumlv);
		}
	}
	bool ClickTab(Item_Tab _tab) {
		m_SUI.Tabs[0].SetActive(_tab.m_Pos == 0);
		m_SUI.Tabs[1].SetActive(_tab.m_Pos == 1);
		m_SUI.SkillTabObj[0].SetActive(_tab.m_Pos == 0);
		m_SUI.SkillTabObj[1].SetActive(_tab.m_Pos == 0);
		m_SUI.PVPTabObj.SetActive(_tab.m_Pos == 1);

		return true;
	}
	//TODO: 미획득 캐릭터는 혈청 버튼 x
	public void ClickSerum()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 8)) return;
		TSerumBlockTable tdata = TDATA.GetSerumBlockTable(1);
		if(m_Info.m_LV < tdata.m_NeedCharLv) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(686), tdata.m_NeedCharLv));
			PlayCommVoiceSnd(VoiceType.Fail);
			return;
		}
		//int openidx = BaseValue.CONTENT_OPEN_IDX(ContentType.Serum);
		//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < openidx) {
		//	POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(273), openidx / 100, openidx % 100, TDATA.GetString(325))); 
		//	PlayCommVoiceSnd(VoiceType.Fail);
		//	return;
		//}
		int precnt = m_Info.m_Serum.Count;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Serum, (result, obj) => {
			if (precnt != m_Info.m_Serum.Count) {
				UpdateStats();
				SetStatUI();
				SetUI();
			}
		}, m_Info);
	}
	/// <summary> 캐릭터 평가 </summary>
	public void ClickRating() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 12)) return;
#if NOT_USE_NET
		return;
#else
		//web에 캐릭터 평가관련 데이터 받음
		WEB.SEND_REQ_CHAR_GET_EVA((res) => {
			if (!res.IsSuccess()) {
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					WEB.SEND_REQ_CHARINFO((res2) => {
						SetUI();
					}, USERINFO.m_UID, m_Info.m_UID);
				});
				return;
			}

			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Character_Evaluation, (result, obj) => {
				SetData(PopupPos.POPUPUI, PopupName.Info_Char, m_EndCB, new object[] { obj.GetComponent<Character_Evaluation>().m_CharIdx, m_CharList });
			}, (RES_CHAR_GET_EVA)res, m_Info, m_CharList);
		}, m_Info.m_Idx);
#endif
	}
	public void Click_InfoTab(int _pos) {
		if(_pos == 1)
			POPUP.Set_MsgBox(PopupName.Msg_CommingSoon, string.Empty, TDATA.GetString(185));
	}
	/// <summary> 튜토리얼용 강조 영역 표시용 </summary>
	/// <param name="Pos">0:장비 장착, 1:혈청 버튼, 2:DNA 버튼, 3:DNA 장착, 4:자동 장착, 5:무기칸, 6:캐릭터 레벨그룹, 7:캐릭터 레벨업 버튼 그룹, 8:캐릭터 승급 버튼 </param>
	/// <returns></returns>
	public GameObject GetTutoPanel(int Pos)
	{
		return m_SUI.Panels[Pos];
	}
}
