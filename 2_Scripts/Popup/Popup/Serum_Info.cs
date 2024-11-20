using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Serum_Info : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_Serum_Cell Cell;
		public TextMeshProUGUI[] BlockNum;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI StatTxt;
		public Image TargetCharIcon;
		public TextMeshProUGUI TartgetCharName;
		public GameObject[] TargetGroup;//0 self 1 all

		public Item_RewardList_Item Mat;
		public GameObject MatLack;
		public TextMeshProUGUI MatCardCnt;
		public TextMeshProUGUI MatName;
		public TextMeshProUGUI MatCnt;
		public TextMeshProUGUI PriceName;
		public TextMeshProUGUI[] DollarCnt;//0 보유1 필요

		public GameObject[] CheckBoxs;//0재료1달러2둘다있을때

		public GameObject NotPreSerumAlarm;
		public GameObject MatGroup;
		public GameObject EndInjectAlarm;
		public Animator EndInjectAnim;
		public Button InjectBtn;
		public GameObject CanInjectBtnGlow;
		public TextMeshProUGUI InjectBtnTxt;
	}
	[SerializeField] SUI m_SUI;
	int m_BlockNum;
	bool m_IsOpened;
	bool m_IsNow;
	TSerumTable m_TData;
	CharInfo m_CharInfo;
	IEnumerator m_Action = null;
	private void Awake() {
		m_SUI.NotPreSerumAlarm.SetActive(false);
		m_SUI.EndInjectAlarm.SetActive(false);
		m_SUI.CanInjectBtnGlow.SetActive(false);
		m_SUI.TargetGroup[0].SetActive(false);
		m_SUI.TargetGroup[1].SetActive(false);
		for (int i = 0; i < 3; i++) m_SUI.CheckBoxs[i].SetActive(false);
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_TData = (TSerumTable)aobjValue[0];
		m_IsOpened = (bool)aobjValue[1];
		m_IsNow = (bool)aobjValue[2];
		m_BlockNum = (int)aobjValue[3];
		m_CharInfo = (CharInfo)aobjValue[4];

		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		base.SetUI();
		m_SUI.Cell.SetData(m_TData.m_Idx, m_IsOpened, m_IsNow, m_BlockNum, m_CharInfo, null, null);
		m_SUI.BlockNum[0].text = string.Format("{0} BLOCK", m_BlockNum);
		m_SUI.BlockNum[1].text = string.Format("<size=40 %>NO.</size>{0}", TDATA.GetSerumTableGroup(m_TData.m_GId, m_BlockNum).IndexOf(m_TData) + 1);
		m_SUI.Title.text = string.Format("{0} {1}", TDATA.GetStatString(m_TData.m_Type), TDATA.GetString(106));
		if(m_TData.m_ValType == StatValType.Ratio)
			m_SUI.StatTxt.text = string.Format("{0} +{1:0.##}% {2}", TDATA.GetStatString(m_TData.m_Type), m_TData.m_Val * 100f, TDATA.GetString(448));
		else
			m_SUI.StatTxt.text = string.Format("{0} +{1} {2}", TDATA.GetStatString(m_TData.m_Type), m_TData.m_Val, TDATA.GetString(448));

		m_SUI.TargetGroup[(int)m_TData.m_TargetType].SetActive(true);
		TCharacterTable chartable = m_CharInfo.m_TData;
		m_SUI.TargetCharIcon.sprite = chartable.GetPortrait();
		m_SUI.TartgetCharName.text = chartable.GetCharName();
		RES_REWARD_ITEM item = new RES_REWARD_ITEM() { 
			Idx = m_TData.m_Material, 
			Cnt = m_TData.m_MatCnt, 
			Type = Res_RewardType.Item
		};
		m_SUI.Mat.SetData(item);
		bool mat = USERINFO.GetItemCount(item.Idx) >= m_TData.m_MatCnt;
		m_SUI.MatCardCnt.text = string.Format("<color={0}>{1}</color> / {2}", mat ? "#498E41" : "#EA5757", USERINFO.GetItemCount(item.Idx), item.Cnt);
		m_SUI.MatLack.SetActive(!mat);
		m_SUI.MatName.text = TDATA.GetItemTable(item.Idx).GetName();
		m_SUI.MatCnt.text = string.Format("{0}:{1}", TDATA.GetString(449), item.Cnt);
		m_SUI.PriceName.text = TDATA.GetItemTable(BaseValue.DOLLAR_IDX).GetName();
		m_SUI.DollarCnt[0].text = Utile_Class.CommaValue(USERINFO.m_Money);
		m_SUI.DollarCnt[1].text = Utile_Class.CommaValue(m_TData.m_DollarCnt);
		m_SUI.DollarCnt[0].color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, m_TData.m_DollarCnt, "#A2321E", "#0C6516");
		IS_CanInject();

		if (m_IsOpened) {//주입 완료
			m_SUI.MatGroup.SetActive(false);
			m_SUI.EndInjectAlarm.SetActive(true);
			m_SUI.EndInjectAnim.SetTrigger("Normal");
			m_SUI.CheckBoxs[2].SetActive(true);
		}
		else if (!m_IsOpened && !m_IsNow) {//선행 혈청 미완
			m_SUI.NotPreSerumAlarm.SetActive(true);
		}
		else if(!m_IsOpened && !IS_CanInject()) {//재료 부족
			//m_SUI.InjectBtn.interactable = false;
			m_SUI.InjectBtnTxt.text = TDATA.GetString(442);
			m_SUI.CanInjectBtnGlow.SetActive(false);
		}
		else if(!m_IsOpened && m_IsNow && IS_CanInject()) {//주입 가능
			//m_SUI.InjectBtn.interactable = true;
			m_SUI.InjectBtnTxt.text = TDATA.GetString(345);
			m_SUI.CanInjectBtnGlow.SetActive(true);
		}
	}
	bool IS_CanInject() {
		bool mat = USERINFO.GetItemCount(m_TData.m_Material) >= m_TData.m_MatCnt;
		bool money = USERINFO.m_Money >= m_TData.m_DollarCnt;

		m_SUI.CheckBoxs[0].SetActive(mat);
		m_SUI.CheckBoxs[1].SetActive(money);

		return mat && money;
	}
	public void ClickInject() {
		if (m_Action != null) return;
		if (!m_IsOpened && !m_IsNow) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(446));
			return;
		}
		if (!m_IsOpened && USERINFO.GetItemCount(m_TData.m_Material) < m_TData.m_MatCnt) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(374));
			return;
		}
		if(!m_IsOpened && USERINFO.m_Money < m_TData.m_DollarCnt) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
			return;
		}
#if NOT_USE_NET
		m_IsOpened = true;
		USERINFO.ChangeMoney(-m_TData.m_DollarCnt);
		USERINFO.DeleteItem(m_TData.m_Material, m_TData.m_MatCnt);
		m_CharInfo.InsertSerum(m_TData.m_Idx);
		USERINFO.Check_Mission(MissionType.Serum, 0, 0, 1);
		MAIN.Save_UserInfo();
		m_Action = IE_InjectAction();
		StartCoroutine(m_Action);
#else
		m_Action = IE_InjectAction();
		WEB.SEND_REQ_CHAR_SERUM((res) => {
			if (!res.IsSuccess()) {
				m_Action = null;
				switch (res.result_code)
				{
				case EResultCode.ERROR_CHAR_LEARNED_SERUM:
						// 캐릭터 정보 다시 받아서 UI 갱신해준다.
						WEB.SEND_REQ_CHARINFO((res2) => {
							SetUI();
						}, USERINFO.m_UID, m_CharInfo.m_UID);
						return;
				}
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			USERINFO.Check_Mission(MissionType.Serum, 0, 0, 1);
			StartCoroutine(m_Action);
		}, m_CharInfo, m_TData.m_Idx);
#endif

	}
	IEnumerator IE_InjectAction() {
		PlayEffSound(SND_IDX.SFX_1082);

		m_SUI.MatGroup.SetActive(false);
		m_SUI.EndInjectAlarm.SetActive(true);
		m_SUI.EndInjectAnim.SetTrigger("Start");
		m_SUI.CheckBoxs[2].SetActive(true);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.EndInjectAnim));

		m_Action = IE_CloseAction(1);
		StartCoroutine(m_Action);
	}
	/// <summary> 재료 수급처 팝업 </summary>
	public void ClickMatGuide() {
		if (m_Action != null) return;
		POPUP.ViewItemInfo((result, obj)=> { SetUI(); }, new object[] { new ItemInfo(m_TData.m_Material), PopupName.NONE, null, m_TData.m_MatCnt });
	}
	public void ClickClose() {
		if (m_Action != null) return;
		m_Action = IE_CloseAction(0);
		StartCoroutine(m_Action);
	}
	IEnumerator IE_CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		Close(_result);
	}
}
