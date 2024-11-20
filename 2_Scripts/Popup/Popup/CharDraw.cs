using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using System.Linq;
using System.Text;

public class CharDraw : PopupBase
{
	enum State
	{
		Intro,
		Result,
		GetInfo
	}
	[Serializable]
	public struct SUI
	{
		public GameObject[] Panels;
		public TextMeshProUGUI ReDrawTxt;
	}
	/// <summary> 인트로 유아이 </summary>
	[Serializable]
	public struct SITUI
	{
		public Animator Anim;
	}
	/// <summary> 결과 유아이 </summary>
	[Serializable]
	public struct SRTUI
	{
		public Animator Anim;
		public Transform[] ListBucket;
		public ScrollRect Scroll;
		public Transform RewardPrefab;
		public Vector3[] RewardPos;
		public GameObject ReBuyBtn;
		public Item_Store_Buy_Button BuyBtn;
		public GameObject SkipBtn;
		public GameObject BusLight;
	}
	/// <summary> 캐릭 정보 유아이 </summary>
	[Serializable]
	public struct SIFUI
	{
		public Animator Anim;
		public Image[] Portrait;
		public Item_Item_Card PieceCard;
		public Item_GradeGroup Grade;
		public Image[] JobIcons;
		public TextMeshProUGUI[] Name;
		public TextMeshProUGUI[] Count;
		public Item_Skill_Card SkillCard;
		public TextMeshProUGUI SkillName;
		public TextMeshProUGUI SkillDesc;
		public TextMeshProUGUI[] CharSpeech;
		public TextMeshProUGUI Bonus;
	}

	[SerializeField] SUI m_SUI;
	[SerializeField] SITUI m_SITUI;
	[SerializeField] SRTUI m_SRTUI;
	[SerializeField] SIFUI m_SIFUI;
	TShopTable m_TDataShop;
	public TShopTable GetTable { get { return m_TDataShop; } }
	const int SpGrade = 3;
	List<OpenItem> m_Items = new List<OpenItem>();
	bool m_Skip;
	bool m_Next;
	bool m_OnlyResult = false;
	Action<int, TShopTable> m_RCB;
	IEnumerator m_Action; //end ani check
	SND_IDX m_PreBG;

	private void Awake() {
		for(int i = 0; i < 3; i++) {
			m_SUI.Panels[i].SetActive(false);
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		
		base.SetData(pos, popup, cb, aobjValue);

		m_PreBG = SND.GetNowBG;
		SND.StopBG();


		m_Items = (List<OpenItem>)aobjValue[0];
		if (aobjValue.Length > 2) {
			m_OnlyResult = false;
			m_TDataShop = (TShopTable)aobjValue[1];
			m_RCB = (Action<int, TShopTable>)aobjValue[2];
			//m_SRTUI.ReBuyBtn.SetActive(m_TDataShop.GetPrice() > 0);
			if (m_TDataShop.m_PriceType == PriceType.AD_InitTime && !USERINFO.IS_CanBuy(m_TDataShop)) 
				m_TDataShop = TDATA.GetGroupShopTable(ShopGroup.Gacha).Find(o => o.m_PriceType == PriceType.Item && o.m_Rewards[0].m_ItemIdx == m_TDataShop.m_Rewards[0].m_ItemIdx && o.m_Rewards[0].m_ItemCnt == m_TDataShop.m_Rewards[0].m_ItemCnt);
			else if (m_TDataShop.m_PriceType == PriceType.AD_AddTime) {
				if (m_TDataShop.GetPrice() > 0)
					m_TDataShop = TDATA.GetGroupShopTable(ShopGroup.Gacha).Find(o => o.m_PriceType == PriceType.Item && o.m_Rewards[0].m_ItemIdx == m_TDataShop.m_Rewards[0].m_ItemIdx && o.m_Rewards[0].m_ItemCnt == m_TDataShop.m_Rewards[0].m_ItemCnt);
				else m_SRTUI.ReBuyBtn.SetActive(false);
			}
			//if (m_TDataShop.m_PriceType == PriceType.Item && !USERINFO.IS_CanBuy(m_TDataShop))
			//	m_TDataShop = TDATA.GetGroupShopTable(ShopGroup.Gacha).Find(o => o.m_PriceType == PriceType.Cash && o.m_Rewards[0].m_ItemIdx == m_TDataShop.m_Rewards[0].m_ItemIdx && o.m_Rewards[0].m_ItemCnt == m_TDataShop.m_Rewards[0].m_ItemCnt);
			//m_SRTUI.BuyBtn.SetData(m_TDataShop.m_Idx);

			int getcnt = USERINFO.GetItemCount(m_TDataShop.m_PriceIdx);
			int needcnt = m_TDataShop.GetPrice();
			TShopTable ticketdata = TDATA.GetShopTable(BaseValue.SHOP_IDX_CHARGACHA_TICKET);
			m_SRTUI.BuyBtn.SetData(new PriceType[2] { m_TDataShop.m_PriceType, ticketdata.m_PriceType }, new int[2] { getcnt > 0 ? Math.Min(needcnt, getcnt) : 0, ticketdata.GetPrice(Math.Max(0, needcnt - getcnt)) }, new int[2] { m_TDataShop.m_PriceIdx, ticketdata.m_PriceIdx });
			

			//m_SUI.ReDrawTxt.text = m_TDataShop.m_Price.ToString("N0");
		}
		else {
			m_OnlyResult = true;
			m_SRTUI.ReBuyBtn.SetActive(false);
		}

		m_SRTUI.SkipBtn.SetActive(m_Items.Count > 1);

		//bool special = m_Items.Find((t) => t.m_Type == OpenItemType.Character && t.m_CharInfo.m_Grade >= 5) != null ||
		//m_Items.Find((t) => t.m_Type == OpenItemType.Item && TDATA.GetAllCharacterInfos().Find((t2) => t2.m_PieceIdx == t.m_Idx && t.m_Grade[0] >= 5) != null) != null;
		//StartCoroutine(DrawAction(special));

		List<OpenItem> chars = m_Items.FindAll(o => o.m_Type == OpenItemType.Character && o.m_CharInfo != null);
		List<OpenItem> pieces = m_Items.FindAll(o => o.m_Type == OpenItemType.Item && TDATA.GetAllCharacterInfos().Find(o2 => o2.m_PieceIdx == o.m_Idx) != null);
		int chartopgrade = chars.Count > 0 ? chars.Max(o => o.m_Grade[0]) : 0;
		int piecetopgrade = pieces.Count > 0 ? pieces.Max(o => o.m_Grade[0]) : 0;
		StartCoroutine(DrawAction(Math.Max(chartopgrade, piecetopgrade)));
	}
	//인트로 - 결과창 시작 - 캐릭터 정보 - 
	IEnumerator DrawAction(int _maxgrade) {
		if (!m_OnlyResult) {
			//인트로
			PlayEffSound(SND_IDX.SFX_1000);
			PlayEffSound(_maxgrade >= SpGrade ? SND_IDX.SFX_1006 : SND_IDX.SFX_1007);
			m_SUI.Panels[(int)State.Intro].SetActive(true);
			m_SITUI.Anim.SetTrigger(_maxgrade >= SpGrade - 1 ? (UTILE.Get_Random(0f,1f) > 0.2f ? "G_Special" : "G_Change") : "G_Normal");
			m_SITUI.Anim.SetTrigger(_maxgrade >= SpGrade ? "Start_5Star" : "Start");
			yield return new WaitForEndOfFrame();

			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SITUI.Anim) && !m_Skip);
			if(m_Skip) SND.StopEff();
			PlayEffSound(SND_IDX.SFX_1001);
			SND.PlayBgSound(SND_IDX.BGM_1001);

			m_Skip = false;
			m_SUI.Panels[(int)State.Intro].SetActive(false);

			m_SRTUI.BusLight.SetActive(_maxgrade >= SpGrade);
			m_SUI.Panels[(int)State.Result].SetActive(true);
			m_SRTUI.Anim.SetTrigger(_maxgrade >= SpGrade - 1 ? "G_Special" : "G_Normal");

			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SRTUI.Anim));
			m_SUI.Panels[(int)State.Result].SetActive(false);
		}
		else {
			m_SRTUI.BusLight.SetActive(_maxgrade >= SpGrade);
		}
		//캐릭 정보
		for (int i = 0; i < m_Items.Count; i++) {
			if (m_Skip) break;
			TCharacterTable chartable = null;
			CharInfo charinfo = null;
			if (m_Items[i].m_Type == OpenItemType.Character) {
				charinfo = USERINFO.m_Chars.Find(t => t.m_Idx == m_Items[i].m_Idx);
				chartable = TDATA.GetCharacterTable(m_Items[i].m_Idx);
				m_SIFUI.Portrait[0].sprite = m_SIFUI.Portrait[1].sprite = chartable.GetPortrait();
				for (int j = 0; j < m_SIFUI.JobIcons.Length; j++) {
					m_SIFUI.JobIcons[j].sprite = chartable.GetJobIcon()[0];
				}
				m_SIFUI.Name[0].text = m_SIFUI.Name[1].text = chartable.GetCharName();
				int grade = charinfo == null ? chartable.m_Grade : m_Items[i].m_Grade[1];
				m_SIFUI.Grade.SetData(grade);
				TSkillTable skill = TDATA.GetSkill(chartable.m_SkillIdx[(int)SkillType.Active]);
				m_SIFUI.SkillCard.SetData(skill.m_Idx, 1);
				m_SIFUI.SkillName.text = skill.GetName();
				m_SIFUI.SkillDesc.text = skill.GetInfo();

				PlayEffSound(grade >= SpGrade ? SND_IDX.SFX_1003 : SND_IDX.SFX_1002);
				SND.StopAllVoice();
				PlayVoiceSnd(new List<SND_IDX>() { chartable.GetVoice(TCharacterTable.VoiceType.CharDraw) });
			}
			else {
				chartable = TDATA.GetAllCharacterInfos().Find(t=>t.m_PieceIdx == m_Items[i].m_Idx);
				charinfo = USERINFO.m_Chars.Find(t => t.m_Idx == chartable.m_Idx);
				m_SIFUI.Portrait[0].sprite = m_SIFUI.Portrait[1].sprite = chartable.GetPortrait();
				for (int j = 0; j < m_SIFUI.JobIcons.Length; j++) {
					m_SIFUI.JobIcons[j].sprite = chartable.GetJobIcon()[0];
				}
				m_SIFUI.Name[0].text = m_SIFUI.Name[1].text = chartable.GetCharName();
				m_SIFUI.Grade.SetData(m_Items[i].m_Grade[0]);
				TSkillTable skill = TDATA.GetSkill(chartable.m_SkillIdx[(int)SkillType.Active]);
				m_SIFUI.SkillCard.SetData(skill.m_Idx, 1);
				m_SIFUI.SkillName.text = skill.GetName();
				m_SIFUI.SkillDesc.text = skill.GetInfo();

				TItemTable itemtable = TDATA.GetItemTable(m_Items[i].m_Idx);
				m_SIFUI.PieceCard.SetData(itemtable.m_Idx, 0, itemtable.m_Grade);
				m_SIFUI.Portrait[2].sprite = itemtable.GetItemImg();
				m_SIFUI.Name[2].text = m_SIFUI.Name[3].text = itemtable.GetName();
				m_SIFUI.Count[0].text = m_SIFUI.Count[1].text = string.Format("x{0}", m_Items[i].m_Cnt);

				PlayEffSound(SND_IDX.SFX_1004);
				SND.StopAllVoice();
				PlayVoiceSnd(new List<SND_IDX>() { chartable.GetVoice(TCharacterTable.VoiceType.CharDraw) });
			}
			m_SUI.Panels[(int)State.GetInfo].SetActive(true);
			if(m_Items[i].m_Type == OpenItemType.Character)
				m_SIFUI.Anim.SetTrigger(chartable.m_Grade >= SpGrade ? "SpecialFirst" : "NormalFirst");//USERINFO.m_Chars.Find(o=>o.m_Idx == chartable.m_Idx) != null ? (chartable.m_Grade >= SpGrade ? "Special" : "Normal") :
			else if(m_Items[i].m_Type == OpenItemType.Item)
				m_SIFUI.Anim.SetTrigger(chartable.m_Grade >= SpGrade ? "SpecialPiece" : "Piece");
			m_SIFUI.Anim.SetTrigger(string.Format("Grade_{0}", m_Items[i].m_Grade[0]));

			//캐릭터별 가챠 대사, 음성
			string charspeech = TDATA.GetString(ToolData.StringTalbe.Dialog, chartable.GetSpeechTable(DialogueConditionType.Gacha).m_StringIdx);
			for (int j = 0; j < m_SIFUI.CharSpeech.Length; j++) {
				m_SIFUI.CharSpeech[j].text = charspeech;
			}
			
			StringBuilder msg = new StringBuilder();
			for (int j = 1; j < 3; j++) {
				TSkillTable tdata = TDATA.GetSkill(chartable.m_SkillIdx[j]);
				if (tdata == null) continue;
				if (tdata.GetStatType() == StatType.None)
					msg.Append(string.Format("[{0}]", tdata.GetName()));
				else if (tdata.GetStatType() != StatType.None)
					msg.Append(string.Format("[{0} +{1}]", TDATA.GetStatString(tdata.GetStatType()), Mathf.RoundToInt(tdata.GetValue() * 100f)));
				msg.Append("\n");
			}
			m_SIFUI.Bonus.text = msg.ToString();

			yield return new WaitForEndOfFrame();
			//스킵 x 40~80프레임 사이면 애니스킵
			yield return new WaitWhile(() => !m_Next && !m_Skip);
			//피스 45 + 100 캐릭 90 + 100
			float[] frame = new float[2];
			if (m_Items[i].m_Type == OpenItemType.Character) {
				if (chartable.m_Grade >= SpGrade) {
					frame[0] = 190f;
					frame[1] = 220f;
				}
				else {
					frame[0] = 90f;
					frame[1] = 125f;
				}
			}
			else if (m_Items[i].m_Type == OpenItemType.Item) {
				if (chartable.m_Grade >= SpGrade) {
					frame[0] = 190f;
					frame[1] = 198;
				}
				else {
					frame[0] = 90f;
					frame[1] = 104f;
				}
			}
			if (!m_Skip && Utile_Class.IsAniPlay(m_SIFUI.Anim, frame[0] / frame[1])) {
				m_Next = false;
				Utile_Class.AniSkip(m_SIFUI.Anim, frame[0]);
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitWhile(() => !m_Next && !m_Skip);
			m_Next = false;
			m_SUI.Panels[(int)State.GetInfo].SetActive(false);
		}
		m_Skip = false;
		//결과
		PlayEffSound(SND_IDX.SFX_1005);

		bool down10 = m_Items.Count <= 10;
		bool isPost = false;
		Transform bucket = down10 ? m_SRTUI.ListBucket[1] : m_SRTUI.ListBucket[0];
		UTILE.Load_Prefab_List(m_Items.Count, bucket, m_SRTUI.RewardPrefab);

		for (int i = 0; i < m_Items.Count; i++) {
			Item_CharDraw_Element reward = bucket.GetChild(i).GetComponent<Item_CharDraw_Element>();
			reward.gameObject.SetActive(false);
			if (m_Items[i].result_code == EResultCode.SUCCESS_POST) isPost = true;
			if (m_Items[i].m_Type == OpenItemType.Character) {
				reward.SetData(m_Items[i].m_CharInfo, m_Items[i].m_Grade[1]);
			}
			else {
				reward.SetData(m_Items[i].m_Idx, m_Items[i].m_Cnt, m_Items[i].m_Grade[0]);
			}
			if (down10) reward.transform.localPosition = m_SRTUI.RewardPos[i];
		}
		m_SUI.Panels[(int)State.Result].SetActive(true);
		m_SRTUI.Anim.SetTrigger("Result");
		StartCoroutine(RewardFade(down10, bucket));

		if (isPost) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(535));
	}

	IEnumerator RewardFade(bool _down10, Transform _bucket) {
		yield return new WaitForSeconds(9f / 60f);
		for (int i = 0;i < _bucket.childCount; i++) {
			Item_CharDraw_Element reward = _bucket.GetChild(i).GetComponent<Item_CharDraw_Element>();
			reward.gameObject.SetActive(true);
			yield return new WaitForEndOfFrame();
			m_SRTUI.Scroll.verticalNormalizedPosition = 0f;
			reward.SetAnim();
			if ((_down10 && (i == 2 || i == 6)) || (!_down10 && (i + 1) % 4 == 0)) {
				yield return new WaitForSeconds(15f / 60f);
			}
		}
	}

	public void SkipBtn() {
		m_Skip = true;
	}
	public void NextBtn() {
		m_Next = true;
	}
	public void ClickReDraw() {
		if (m_Action != null) return;
		//string msg = Utile_Class.StringFormat(TDATA.GetString(790), BaseValue.GetPriceTypeName(m_TDataShop.m_PriceType, m_TDataShop.m_PriceIdx));
		//POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, msg, (result, obj) => {
		//	if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
		//		Close(1);
		//	}
		//	else {
		//		POPUP.StartLackPop(m_TDataShop.GetPriceIdx(), false, (result) => {
		//			Close(2);
		//		});
		//	}
		//}, m_TDataShop.m_PriceType, m_TDataShop.m_PriceIdx, m_TDataShop.GetPrice(), false);
		int getcnt = USERINFO.GetItemCount(m_TDataShop.m_PriceIdx);
		int needcnt = m_TDataShop.GetPrice();
		bool canbuy = getcnt >= needcnt;
		string msg = string.Empty;
		if (canbuy) {
			msg = Utile_Class.StringFormat(TDATA.GetString(790), BaseValue.GetPriceTypeName(m_TDataShop.m_PriceType, m_TDataShop.m_PriceIdx));//_tdata.m_PriceType == PriceType.Item ? TDATA.GetItemTable(_tdata.m_PriceIdx).GetName() : TDATA.GetString(122);
			POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, msg, (result, obj) => {
				if (result == 1) {
					if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
						///아이템 획득 연출
						m_RCB?.Invoke(1, m_TDataShop);
						Close(1);
					}
					else {
						POPUP.StartLackPop(m_TDataShop.GetPriceIdx(), false, (result) => {
							Close(2);
						});
					}
				}
			}, m_TDataShop.m_PriceType, m_TDataShop.m_PriceIdx, m_TDataShop.GetPrice(), false);
		}
		else {
			TShopTable ticketdata = TDATA.GetShopTable(BaseValue.SHOP_IDX_CHARGACHA_TICKET);
			msg = Utile_Class.StringFormat(TDATA.GetString(1044), BaseValue.GetPriceTypeName(m_TDataShop.m_PriceType, m_TDataShop.m_PriceIdx));
			POPUP.Set_MsgBox(PopupName.Msg_Store_Ticket_Buy, string.Empty, msg, (result, obj) => {
				if (result == 1) {
					if (obj.GetComponent<Msg_Store_Ticket_Buy>().IS_CanBuy) {
						///아이템 획득 연출
						USERINFO.ITEM_BUY(ticketdata.m_Idx, needcnt - getcnt, (res) => {
							if (res != null && res.IsSuccess()) {
								m_RCB?.Invoke(1, m_TDataShop);
								Close(1);
							}
						});
					}
					else {
						POPUP.StartLackPop(ticketdata.GetPriceIdx(), false, (result) => {
							Close(2);
						});
					}
				}
			}, ticketdata.m_PriceType, ticketdata.m_PriceIdx, ticketdata.GetPrice(needcnt - getcnt), m_TDataShop.m_PriceIdx, needcnt, false);
		}
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SRTUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SRTUI.Anim));
		SND.PlayBgSound(m_PreBG);
		base.Close(_result);
	}
}
