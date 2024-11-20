using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class DL_Talk : PopupBase
{
#pragma warning disable 0649
	public enum State
	{
		Talk,
		Etc,
		Select,
		Result,
	}
	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image[] ChapNum;
		public TextMeshProUGUI ChapName;
		public Image BgBig;
		public Image BgSmall;
		public ParticleSystem BGSmall2;
		public ScrollRect m_Scroll;
		public TextMeshProUGUI m_AreaName;
		public Transform m_TalkBucket;
		public GameObject m_TalkIconLeft, m_TalkIconRight, m_TalkTalkLeft, m_TalkTalkRight, m_TalkNarration, m_TalkCardImg, m_TalkUnitImg;
		public GameObject m_TouchBox;
		public GameObject m_SkipBtn;
	}
	[Serializable]
	public class TalkSelect
	{
		public Item_Talk_Select[] m_Selects;
	}
	[Serializable]
	public struct SSUI
	{
		public GameObject[] m_SelectPanels;
		public Transform[] m_SelectBucket;
		public Animator[] m_SelectPanelAnims;
		public TalkSelect[] m_TalkSelects;
		public GameObject m_SmallCardPrefab;
		public Transform m_SmallCardBucket;
		public Image[] m_Timer;
		public Image[] m_SelectBGs;
		public RectTransform m_Scroll;
	}
	[Serializable]
	public struct SRUI
	{
		public Animator m_Anim;
		public GameObject m_SelectReward;
		public Image m_Img;
		public TextMeshProUGUI m_Name;
		public TextMeshProUGUI m_Desc;
	}
#pragma warning restore 0649
	[SerializeField] SUI m_SUI;
	[SerializeField] SSUI m_SSUI;
	[SerializeField] SRUI m_SRUI;                                           //유아이

	Sprite[] Bgs = new Sprite[2];
	bool Is_NormalType;

	TDialogTable m_PreTable = null, m_CrntTable = null;                     //이전,현재 다이얼로그
	Item_Talk_Char m_PreChar = null;                                        //이전 대화자
	Item_Talk_Talk m_PreTalk = null;                                        //이전 대사
	Item_Talk_Narration m_PreNarr = null;									//이전 나레이션
	List<Item_Talk_Select> m_Selects = new List<Item_Talk_Select>();
	Item_Talk_Select m_CrntSelect = null;
	[SerializeField] int m_NextDLIdx = 0;                                   //다음 다이얼로그 인덱스, 0이면 대화 끝남
	[SerializeField] int m_SkipLastDLIdx = 0;			//스킵 가능한 마지막 인덱스(선택지 전, 선택지 없으면 마지막 전)
	Coroutine m_AutoNextCor;                                                //자동개행 코루틴
	bool m_NextDL = false;                                                  //자동개행 판단
	bool m_AutoScroll = true;                                               //스크롤중에는 자동으로 맨아래 안끌려가게
	[SerializeField] bool m_Skipping = false;								//스킵중에 다음으로 못넘기게

	int m_SelectPanelPos;                                                   //선택지 패널 종류
	TCaseSelectTable m_HideSelectTable = null;                              //숨겨진 선택지
	float[] m_ScrollPosY = new float[2];
	float[] m_ScrollHeight = new float[2];

	bool m_FatedTime;
	IEnumerator m_CorFatedSND;
	SND_IDX m_NowBG;

	int m_CallPos;
	State m_State;

	/// <summary> aobjValue 0:에피소드번호 </summary>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		//TODO:장소-디테일장소 구조 나오고 바뀌면 작업
		//배경이랑 이름 변경
		m_NextDLIdx = (int)aobjValue[0];
		m_SUI.m_AreaName.text = (string)aobjValue[1];
		Bgs[0] = (Sprite)aobjValue[2];
		Bgs[1] = (Sprite)aobjValue[3];//챕터 마지막 다이얼로그에서만 쓰이니 나머지는 null
		Is_NormalType = (bool)aobjValue[4];
		m_CallPos = (int)aobjValue[5];
		int chapnum = 0;
		if (aobjValue.Length > 6)
			chapnum = (int)aobjValue[6] / 100;
		else
			chapnum = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx / 100;

		m_SUI.BgBig.sprite = Bgs[0];
		m_SUI.BgSmall.sprite = Bgs[1];
		var shape = m_SUI.BGSmall2.shape;
		shape.texture = UTILE.GetTextureFromSprite(Bgs[1]);
		m_SUI.ChapNum[0].sprite = UTILE.LoadImg(string.Format("Font/StgMain_ImgFont/ImageFont_Chp_{0}", chapnum / 10), "png");
		m_SUI.ChapNum[1].sprite = UTILE.LoadImg(string.Format("Font/StgMain_ImgFont/ImageFont_Chp_{0}", chapnum % 10), "png");
		m_SUI.ChapName.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 20239), chapnum);
		PlayEffSound(SND_IDX.SFX_0344);
		m_NowBG = SND.GetNowBG;
		PlayBGSound(SND_IDX.BGM_0053);

		StartCoroutine(StartAction());
	}
	IEnumerator StartAction() {
		m_SUI.Anim.SetTrigger(Is_NormalType ? "Start_Normal" : "Start_Main");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, Is_NormalType ? 90f / 415f : 220f / 305f));

		SetNextDLIdx(m_NextDLIdx);
		StartCoroutine(DLScript());

		CheckSkipDL();
	}
	void CheckSkipDL() {
		int start = m_NextDLIdx;
		while (start != 0) {
			TDialogTable nexttable = TDATA.GetDialogTable(start).GetNextDialog();
			if (nexttable == null) {
				m_SkipLastDLIdx = start;
				break;
			}
			if (nexttable.m_SelectGID != 0 )  
				m_SkipLastDLIdx = start;
			if(nexttable.m_NextDLIdx == 0)
				m_SkipLastDLIdx = nexttable.m_Idx;
			start = nexttable.m_NextDLIdx;
		}
	}
	void SetNextDLIdx(int Idx)
	{
		m_NextDLIdx = Idx;
	}
	private void Awake() {
		for (int i = 0; i < m_SSUI.m_SelectPanels.Length; i++) {
			m_SSUI.m_SelectPanels[i].SetActive(false);
			for (int j = 0; j < m_SSUI.m_TalkSelects[m_SelectPanelPos].m_Selects.Length; j++) m_SSUI.m_TalkSelects[i].m_Selects[j].gameObject.SetActive(false);
		}
		m_SRUI.m_SelectReward.SetActive(false);
	}
	private void OnDisable() {
		StopAllCoroutines();
	}
	private void OnEnable() {//다른 이벤트 팝업 꺼지면 이어서 진행
		if(m_NextDLIdx != 0)
			StartCoroutine(DLScript());
	}
	/// <summary> 진행할 다이얼로그 체크해서 진행 </summary>
	//그룹 아이디로 뭉탱이 호출
	IEnumerator DLScript() {
		yield return new WaitForSeconds(0.5f);

		while (m_NextDLIdx != 0) {
			m_SUI.m_TouchBox.SetActive(true);
			m_SUI.m_SkipBtn.SetActive(true);
			m_CrntTable = TDATA.GetDialogTable(m_NextDLIdx);
			TDialogTable nexttable = m_CrntTable.GetNextDialog();
			if (m_SkipLastDLIdx > 0 && m_NextDLIdx == m_SkipLastDLIdx && m_CrntTable.m_SelectGID == 0) {
				m_Skipping = false;
			}
			if (nexttable != null)
				SetNextDLIdx(nexttable.m_Idx);
			else
				SetNextDLIdx(0);
			switch (m_CrntTable.m_Dir)
			{
				case DialogTalkDir.None:
					if (!m_CrntTable.m_CardImg.Equals("") || !m_CrntTable.m_UnitImg.Equals("")) {//컷신 이미지
						m_State = State.Etc;
						SetImg(m_CrntTable);
					}
					else if(m_CrntTable.m_SelectGID == 0) {//상황지문
						m_State = State.Etc;
						SetNarration(m_CrntTable);
					}
					else {//선택지
						yield return UTILE.GetCaptureSprite(m_SSUI.m_SelectBGs);
						m_State = State.Select;
						if (m_Skipping) SetSkipSelect(m_CrntTable);
						else SetSelect(m_CrntTable);
					}
					break;
				case DialogTalkDir.Left:
				case DialogTalkDir.Right:
					m_State = State.Talk;
					SetTalk(m_CrntTable);
					break;
			}
			if (!m_CrntTable.m_Sound.Equals("")) {
				PlayEffSound((SND_IDX)Enum.Parse(typeof(SND_IDX), m_CrntTable.m_Sound), m_CrntTable.m_SndVolume);
			}

			m_PreTable = m_CrntTable;

			m_NextDL = m_Skipping;
			if (m_AutoNextCor != null)//다음줄 개행
				StopCoroutine(m_AutoNextCor);

			//추가 아이템 화면넘어가면 스크롤 맨 아래로 이동
			Canvas.ForceUpdateCanvases();
			if(m_AutoScroll) m_SUI.m_Scroll.verticalNormalizedPosition = 0;

			if (m_NextDLIdx == 0 && m_State != State.Select) m_SUI.m_SkipBtn.SetActive(false);
			yield return new WaitWhile(() => m_NextDL != true);
		}
		Close();
	}
	/// <summary> 다음줄 개행, 자동개행 종료 </summary>
	public void ClickNext() {
		if (m_Skipping) return;
		if (m_AutoNextCor != null)
			StopCoroutine(m_AutoNextCor);
		m_NextDL = true;
	}
	[ContextMenu("SkipTest")]
	public void ClickSkip() {
	if (m_Skipping) return;
		TDialogTable crnttable = TDATA.GetDialogTable(m_NextDLIdx);
		if (crnttable == null) return;
		TDialogTable nexttable = crnttable.GetNextDialog();
		if (nexttable == null && crnttable.m_SelectGID == 0) {
			ClickNext();
			return;
		}

		var idx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx;
#if !NOT_USE_NET
		WEB.SEND_REQ_ANALYTICS(AnalyticsType.story_skip_button, m_CallPos, idx, USERINFO.GetDifficulty());
#endif
		m_Skipping = true;
		m_NextDL = true;
	}
	public void AutoScroll(bool _auto) {
		m_AutoScroll = _auto;
	}
	/// <summary> 자동 개행 </summary>
	IEnumerator AutoScriptNext() {
		yield return new WaitForSeconds(3.5f);
		m_NextDL = true;
	}
	/// <summary> 대화 출력, 좌우로 나뉘고 대화자 바뀌지 않으면 아래로 이어 붙임 </summary>
	void SetTalk(TDialogTable _table) {
		GameObject icon = null, talk = null;
		if (_table.m_Dir == DialogTalkDir.Left) {
			icon = m_SUI.m_TalkIconLeft;
			talk = m_SUI.m_TalkTalkLeft;
		}
		else if (_table.m_Dir == DialogTalkDir.Right) {
			icon = m_SUI.m_TalkIconRight;
			talk = m_SUI.m_TalkTalkRight;
		}
		//이전 대화자 없거나 다른경우 새로 추가
		if (m_PreTable?.GetTalker()?.GetName() != _table.GetTalker()?.GetName()) {
			Item_Talk_Char charicon = Instantiate(icon, m_SUI.m_TalkBucket).GetComponent<Item_Talk_Char>();
			m_PreChar = charicon;
			charicon.SetData(_table.GetTalker());
		}
	   
		//추가되는 대사, 현재 대화자에 붙임
		Item_Talk_Talk chartalk = Instantiate(talk, m_PreChar.transform).GetComponent<Item_Talk_Talk>();
		chartalk.SetData(_table.m_TalkEmotion, _table.GetDesc(), _table.m_TimeAni);
		PlayEffSound(SND_IDX.SFX_0345);
		if (m_PreNarr != null) {
			m_PreNarr.Stop();
		}
		//이전 대사 있으면 애니 정지
		if (m_PreTalk != null) {
			m_PreTalk.Stop();
			//대화자 같으면 말꼬리 끄기
			if (m_PreTable?.GetTalker()?.GetName() == _table.GetTalker()?.GetName())
				if (chartalk.CrntTail != null)
					chartalk.CrntTail.SetActive(false);
		}
		m_PreTalk = chartalk;
	}
	/// <summary> 나레이션 출력 </summary>
	void SetNarration(TDialogTable _table) {
		m_PreNarr = Instantiate(m_SUI.m_TalkNarration, m_SUI.m_TalkBucket).GetComponent<Item_Talk_Narration>();
		m_PreNarr.SetData(_table.GetDesc(), _table.m_TimeAni);
	}
	void SetImg(TDialogTable _table) {
		bool card = !_table.m_CardImg.Equals("");
		Item_Talk_Img img = Instantiate(card ? m_SUI.m_TalkCardImg : m_SUI.m_TalkUnitImg, m_SUI.m_TalkBucket).GetComponent<Item_Talk_Img>();
		img.SetData(card ? _table.GetCardImg() : _table.GetUnitImg(), _table.m_Ani, card);
	}
	/// <summary> 선택지 세팅 </summary>
	void SetSelect(TDialogTable _table) {
		PlayEffSound(SND_IDX.SFX_0340);
		switch (_table.m_SelectGroupType) {
			case SelectGroupType.Normal:
			case SelectGroupType.NormalTime:
				//일반 패널
				m_SelectPanelPos = 0;
				break;
			case SelectGroupType.Fated:
			case SelectGroupType.FatedTime:
				//운명 패널
				PlayEffSound(SND_IDX.SFX_9401);
				m_SelectPanelPos = 1;
				break;
		}
		m_SSUI.m_SelectPanels[m_SelectPanelPos].SetActive(true);

		m_SUI.m_TouchBox.SetActive(false);
		m_SUI.m_SkipBtn.SetActive(false);
		List<TCaseSelectTable> selecttables = TDATA.GetCaseSelectGroupTable(_table.m_SelectGID);
		m_Selects.Clear();
		//List<int> pos = new List<int>() { 0, 1, 2, 3 };
		for (int i = 0; i < selecttables.Count; i++) {
			if (selecttables[i].m_Hide) {
				m_HideSelectTable = selecttables[i];
				continue;
			}
			//int objpos = pos[UTILE.Get_Random(0, pos.Count)];
			//pos.Remove(objpos);

			Item_Talk_Select select = m_SSUI.m_TalkSelects[m_SelectPanelPos].m_Selects[i];
			select.SetData(CB_Select, CB_SelectBlock, selecttables[i], (Item_Talk_Select.SelectType)m_SelectPanelPos);
			select.gameObject.SetActive(true);
			select.transform.SetAsLastSibling();
			m_Selects.Add(select);
			if ((Item_Talk_Select.SelectType)m_SelectPanelPos == Item_Talk_Select.SelectType.Special 
				&& (selecttables[i].m_StrType == SelectStringType.TimeBlock || selecttables[i].m_StrType == SelectStringType.TimeChange)) m_FatedTime = true;

			float delay = 0f;
			switch (i) {
				case 0 : delay = m_SelectPanelPos == 0 ? 50f / 165f : 125f / 215f; break;
				case 1: delay = m_SelectPanelPos == 0 ? 60f / 165f : 140f / 215f; break;
				case 2: delay = m_SelectPanelPos == 0 ? 70f / 165f : 155f / 215f; break;
				case 3: delay = m_SelectPanelPos == 0 ? 80f / 165f : 170f / 215f; break;
			}
			StartCoroutine(SelectElementStart(select, delay));
		}
		//선택지 나오고 기본 패널이면 스크롤이 올라가게
		if(m_SelectPanelPos == 0) StartCoroutine(SetSelectScrollMove(true));

		//선택지 타이머
		switch (_table.m_SelectGroupType) {
			case SelectGroupType.NormalTime:
			case SelectGroupType.FatedTime:
				StartCoroutine(SetTimer(_table.m_SelectGroupValue));
				break;
		}
	}
	void SetSkipSelect(TDialogTable _table) {
		List<TCaseSelectTable> selecttables = TDATA.GetCaseSelectGroupTable(_table.m_SelectGID);
		m_NextDLIdx = selecttables[0].m_NextDLIdx != 0 ? selecttables[0].m_NextDLIdx : m_NextDLIdx;
		CheckSkipDL();
		
	}

	IEnumerator SetSelectScrollMove(bool _up) {
		yield return new WaitForEndOfFrame();
		if(_up)iTween.ValueTo(gameObject, iTween.Hash("from", m_SSUI.m_Scroll.offsetMin.y, "to", m_SSUI.m_SelectPanels[m_SelectPanelPos].GetComponent<RectTransform>().rect.height, "onupdate", "TW_ScrollMove", "time", 0.5f));
		else iTween.ValueTo(gameObject, iTween.Hash("from", m_SSUI.m_Scroll.offsetMin.y, "to", 0f, "onupdate", "TW_ScrollMove", "time", 0.5f));

		yield return new WaitForSeconds(0.5f);
	}
	IEnumerator SetTimer(float _time) {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SSUI.m_SelectPanelAnims[m_SelectPanelPos]));

		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "onupdate", "TW_Timer", "oncomplete", "TW_TimerEnd", "time", _time, "name", "Timer"));
	}
	void TW_ScrollMove(float _amount) {
		m_SSUI.m_Scroll.offsetMin = new Vector2(0f, _amount);
		m_SUI.m_Scroll.verticalNormalizedPosition = 0f;
	}
	void TW_Timer(float _amount) {
		m_SSUI.m_Timer[m_SelectPanelPos].fillAmount = _amount;
	}
	void TW_TimerEnd() {
		m_SSUI.m_SelectPanelAnims[m_SelectPanelPos].SetTrigger("End");
		if (m_SelectPanelPos == 1) m_SSUI.m_SelectPanelAnims[m_SelectPanelPos].SetTrigger("Background_End");

		if (m_HideSelectTable != null) {
			m_Selects.Find(t=>t.m_TData.m_Idx == m_HideSelectTable.m_Idx).ClickSelect();
		}
		else {
			m_Selects[UTILE.Get_Random(0, m_Selects.Count)].ClickSelect();
		}
	}
	IEnumerator SelectElementStart(Item_Talk_Select _select, float _delay) {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SSUI.m_SelectPanelAnims[m_SelectPanelPos], _delay));
		_select.SetAnim("Start");
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(_select.Anim));
		_select.SetPlay();
		if (m_FatedTime) {
			m_FatedTime = false;
			StartCoroutine(m_CorFatedSND = FatedTimeSND());
		}
	}
	IEnumerator SelectElementsEnd() {
		yield return new WaitForSeconds(0.5f);
		Item_TalkRewardCard smallcard = null;
		int idx = m_CrntSelect.m_TData.m_Rewards[0].m_Value;
		if (idx > 0) {
			smallcard = Utile_Class.Instantiate(m_SSUI.m_SmallCardPrefab, m_SSUI.m_SmallCardBucket).GetComponent<Item_TalkRewardCard>();
			smallcard.SetData(idx);
			smallcard.transform.position = m_CrntSelect.GetTxtWPos();
			m_CrntSelect.Anim.SetTrigger((Item_Talk_Select.SelectType)m_SelectPanelPos == Item_Talk_Select.SelectType.Normal ? "End" : "Break");
		}
		for (int i = 0; i < m_SSUI.m_SelectBucket[m_SelectPanelPos].childCount; i++) {
			Item_Talk_Select select = m_SSUI.m_SelectBucket[m_SelectPanelPos].GetChild(i).GetComponent<Item_Talk_Select>();
			if (select.m_TData == null || select == m_CrntSelect) continue;
			yield return new WaitForSeconds(0.3f);
			idx = select.m_TData.m_Rewards[0].m_Value;
			if (idx > 0) {
				smallcard = Utile_Class.Instantiate(m_SSUI.m_SmallCardPrefab, m_SSUI.m_SmallCardBucket).GetComponent<Item_TalkRewardCard>();
				smallcard.SetData(idx);
				smallcard.transform.position = select.GetTxtWPos();
				if(select.m_TData.m_StrType == SelectStringType.TimeBlock && select.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
					if ((Item_Talk_Select.SelectType)m_SelectPanelPos == Item_Talk_Select.SelectType.Normal) select.SetAnim("End");
				}
				else
					select.SetAnim((Item_Talk_Select.SelectType)m_SelectPanelPos == Item_Talk_Select.SelectType.Normal ? "End" : "Break");
			}
			//작은 카드 생성
		}

		yield return new WaitForEndOfFrame();

		for (int i = 0; i < m_SSUI.m_SelectBucket[m_SelectPanelPos].childCount; i++) {
			Item_Talk_Select select = m_SSUI.m_SelectBucket[m_SelectPanelPos].GetChild(i).GetComponent<Item_Talk_Select>();
			if (!m_Selects.Contains(select)) continue;
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(select.Anim));
		}

		yield return new WaitForSeconds(1f);
	}
	/// <summary> 선택지들이 블록될때 선택지들에서 빼고 선택지가 0개면 히든 선택지 처리 </summary>
	void CB_SelectBlock(Item_Talk_Select _select) {
		if (m_Selects.Contains(_select)) m_Selects.Remove(_select);
		if(m_Selects.Count < 1 && m_HideSelectTable != null) {
			CB_Select(m_HideSelectTable, false);
		}
	}
	/// <summary> 선택지 고르면 보상 지급 </summary>
	void CB_Select(TCaseSelectTable _table, bool _timeend) {
		if (m_State != State.Select) return;
		m_State = State.Result;
		PlayEffSound(m_SelectPanelPos == 0 ? SND_IDX.SFX_0341 : SND_IDX.SFX_0321);
		FatedTimeEnd();
		//타이머 정지
		iTween.StopByName(gameObject, "Timer");

		//선택지 비활성화
		for (int i = 0; i < m_Selects.Count; i++) {
			if (m_Selects[i].m_TData.m_Idx == _table.m_Idx) {
				m_CrntSelect = m_Selects[i];
				continue;
			}
			m_Selects[i].ButtonInteractOff(_timeend);
			//m_Selects[i].SetAnim("NotSelect");
		}
		//선택지 보상 지급 및 다음 선택지 지정(없으면 그대로)
		m_NextDLIdx = _table.m_NextDLIdx != 0 ? _table.m_NextDLIdx : m_NextDLIdx;
		CheckSkipDL();

#if NOT_USE_NET
		List<LS_Web.RES_REWARD_ITEM> rewards = new List<LS_Web.RES_REWARD_ITEM>();
		for (int i = 0; i < _table.m_Rewards.Count; i++) {
			rewards.Add(new LS_Web.RES_REWARD_ITEM() { 
				Idx = _table.m_Rewards[i].m_Value, 
				Cnt = _table.m_Rewards[i].m_Count 
			});

			Utile_Class.DebugLog(rewards[i].Idx + " 받음");
		}
		USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Selects.Clear();
		USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Selects.Add(new UserInfo.StageSelect() {
			Idx = _table.m_Idx,
			Dlg = m_CrntTable.m_Idx,
			Rewards = rewards
		});
		if (rewards.Count < 1) {
			StartCoroutine(CloseSelect());
			return;
		}
		StartCoroutine(ViewSelectReward(rewards[0].Idx));

#else
		List<LS_Web.RES_REWARD_BASE> rewards = new List<LS_Web.RES_REWARD_BASE>();
		SendSelectReward((res) => {
			USERINFO.SetDATA(res.Stage);
			rewards.Clear();
			rewards.AddRange(res.GetRewards());
			if (rewards.Count < 1) {
			StartCoroutine(CloseSelect());
				return;
			}
			StartCoroutine(ViewSelectReward(rewards[0].GetIdx()));
		}, m_CrntTable.m_Idx, _table.m_Idx);
#endif
	}

	void SendSelectReward(Action<LS_Web.RES_STAGE_TALKSELECT> cb, int _dl, int _idx) {
		UserInfo.StageIdx stageidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
		UserInfo.Stage stage = USERINFO.m_Stage[StageContentType.Stage];
		WEB.SEND_REQ_STAGE_TALKSELECT((res) => {
			if (!res.IsSuccess()) {
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					// 클리어 다시 시도
					// 타이틀부터 다시 시작함 (플레이상태에서 시드 선택정보가 있으면 다시 시도함)
					MAIN.ReStart();
				});
				return;
			}
			cb?.Invoke(res);
		}, stage.UID, stageidx.Week, stageidx.Pos, _dl, _idx);
	}
	/// <summary> 선택지 보상 보여주는 연출</summary>
	IEnumerator ViewSelectReward(int _idx) {
		//스테이지 테이블의 로드가 필요함
		TDATA.LoadStageData();
		TW_SmallCardBucketAlpha(1f);
		yield return SelectElementsEnd();
		m_SRUI.m_SelectReward.SetActive(true);
		bool good = true;
		m_SRUI.m_Anim.SetTrigger(good ? "Reward_Good" : "Reward_Bad");

		TStageCardTable table = TDATA.GetStageCardTable(_idx);
		m_SRUI.m_Img.sprite = table.GetImg();
		if(table.m_Type == StageCardType.Material) m_SRUI.m_Name.text = string.Format("{0} x{1}", table.GetName(), table.m_Value2);
		else m_SRUI.m_Name.text = table.GetName();
		m_SRUI.m_Desc.text = TDATA.GetString(323);

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SRUI.m_Anim, 62f / 571f));

		PlayEffSound(SND_IDX.SFX_0321);
	}
	IEnumerator FatedTimeSND() {
		AudioSource source =  PlayEffSound(SND_IDX.SFX_9403);
		yield return new WaitWhile(() => source != null);
		StartCoroutine(m_CorFatedSND = FatedTimeSND());
	}
	void FatedTimeEnd() {
		if (m_CorFatedSND != null) {
			StopCoroutine(m_CorFatedSND);
			SND.StopEffSound(SND_IDX.SFX_9403);
		}
	}

	public void ClickCloseSelectReward() {
		StartCoroutine(CloseSelectReward());
	}

	IEnumerator CloseSelectReward() {
		FatedTimeEnd();
		//선택지 패널 닫기
		m_SSUI.m_SelectPanelAnims[m_SelectPanelPos].SetTrigger("End");
		if (m_SelectPanelPos == 1) m_SSUI.m_SelectPanelAnims[m_SelectPanelPos].SetTrigger("Background_End");
		else StartCoroutine(SetSelectScrollMove(false));
		//보상팝업 닫기
		m_SRUI.m_Anim.SetTrigger("End");
		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "onupdate", "TW_SmallCardBucketAlpha", "time", 0.5f));

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SRUI.m_Anim));

		m_SRUI.m_SelectReward.SetActive(false);
		m_SSUI.m_SelectPanels[m_SelectPanelPos].SetActive(false);

		m_SUI.m_TouchBox.SetActive(true);
		m_NextDL = true;
	}
	IEnumerator CloseSelect() {
		FatedTimeEnd();
		
		//선택지 패널 닫기
		m_SSUI.m_SelectPanelAnims[m_SelectPanelPos].SetTrigger("End");
		if (m_SelectPanelPos == 1) m_SSUI.m_SelectPanelAnims[m_SelectPanelPos].SetTrigger("Background_End");
		else StartCoroutine(SetSelectScrollMove(false));

		yield return new WaitForSeconds(0.2f);

		for (int i = 0; i < m_Selects.Count; i++) {
			m_Selects[i].SetAnim("End");
		}
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SSUI.m_SelectPanelAnims[m_SelectPanelPos]));

		m_SSUI.m_SelectPanels[m_SelectPanelPos].SetActive(false);
		m_SUI.m_TouchBox.SetActive(true);
		m_NextDL = true;
	}
	void TW_SmallCardBucketAlpha(float _amount) {
		m_SSUI.m_SmallCardBucket.GetComponent<CanvasGroup>().alpha = _amount;
	}
	public bool IS_NextDL() {
		return m_NextDLIdx != 0 ? true : false;
	}
	public override void Close(int Result = 0) {
		PlayBGSound(m_NowBG);

		base.Close(Result);
	}
}
