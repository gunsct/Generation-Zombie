using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;

public class PersonnelFileChange : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Title;
		public Item_Item_Card[] Piece;
		public GameObject[] NoName;
		public TextMeshProUGUI[] HaveCnt;
		public TextMeshProUGUI[] GetCnt;
		public Slider Slider;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;
	int m_MatIdx;
	int m_ChangeIdx;
	int m_Grade;
	int m_UseCount;
	int[] m_ChangeCount = new int[2];//생성 개수 현재, 최대
	int m_HoldCounting = 0;
	[SerializeField] bool m_IsCountHold;
	bool m_IsPlus;
	float[] m_HoldTimer = new float[2];
	LS_Web.REQ_CHAGE_CHAR_PIECE.ChangeMode m_Mode;
	List<RES_REWARD_BASE> m_Reward = new List<RES_REWARD_BASE>();
	int m_Mul;
	bool IS_Counting { get { return m_ChangeCount[0] >= 0 && m_ChangeCount[0] <= m_ChangeCount[1]; } }//BaseValue.PERSONNELFILE_CHANGE_RATIO, BaseValue.COMMON_PERSONNELFILE_IDX
	int ChangeCnt { get { return m_MatIdx != BaseValue.COMMON_PERSONNELFILE_IDX ? m_ChangeCount[1] * (TDATA.GetItemTable(m_MatIdx).m_Value / BaseValue.PERSONNELFILE_CHANGE_RATIO) : m_ChangeCount[1]; } }
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_MatIdx = (int)aobjValue[0];
		m_ChangeIdx = (int)aobjValue[1];
		m_Grade = (int)aobjValue[2];
		m_Mode = m_MatIdx == BaseValue.COMMON_PERSONNELFILE_IDX ? LS_Web.REQ_CHAGE_CHAR_PIECE.ChangeMode.CharPiece : LS_Web.REQ_CHAGE_CHAR_PIECE.ChangeMode.NormalPiece;

		int matcnt = USERINFO.GetItemCount(m_MatIdx);
		int changecnt = USERINFO.GetItemCount(m_ChangeIdx);

		if (m_Mode == LS_Web.REQ_CHAGE_CHAR_PIECE.ChangeMode.CharPiece) {//무기명 -> 목표
			m_ChangeCount[1] = Mathf.CeilToInt(matcnt / TDATA.GetItemTable(m_ChangeIdx).m_Value);
			m_SUI.Title.text = TDATA.GetString(610);
			m_SUI.Piece[0].SetData(m_MatIdx, 0, 1);
			m_SUI.Piece[1].SetData(m_ChangeIdx, 0, m_Grade);
			m_SUI.NoName[0].SetActive(true);
			m_SUI.NoName[1].SetActive(false);
			m_Mul = 1;
		}
		else {//목표 -> 무기명
			m_ChangeCount[1] = matcnt;//Mathf.CeilToInt(matcnt * TDATA.GetItemTable(m_MatIdx).m_Value * BaseValue.PERSONNELFILE_CHANGE_RATIO / 100);
			m_SUI.Title.text = TDATA.GetString(609);
			m_SUI.Piece[0].SetData(m_MatIdx, 0, m_Grade);
			m_SUI.Piece[1].SetData(m_ChangeIdx, 0, 1);
			m_SUI.NoName[0].SetActive(false);
			m_SUI.NoName[1].SetActive(true);
			m_Mul = Mathf.Max(1, Mathf.CeilToInt(TDATA.GetItemTable(m_MatIdx).m_Value * BaseValue.PERSONNELFILE_CHANGE_RATIO / 100));
		}
		m_SUI.Slider.minValue = 0;
		m_SUI.Slider.maxValue = Math.Min(m_ChangeCount[1], 999);

		m_SUI.HaveCnt[0].text = matcnt.ToString();
		m_SUI.HaveCnt[1].text = changecnt.ToString();
		Click_Count(0);
	}
	private void Update() {
		if (m_IsCountHold) {
			m_HoldTimer[0] += Time.deltaTime;
			if (m_HoldTimer[0] > m_HoldTimer[1]) {
				m_HoldTimer[0] = 0;
				m_HoldTimer[1] = Mathf.Max(0.1f, m_HoldTimer[1] - 0.1f);
				m_HoldCounting++;
				Click_Count(m_HoldCounting * (m_IsPlus ? 1 : -1));
				if (!IS_Counting) {
					HoldEnd_Count();
				}
			}
		}
	}
	public void HoldStart_Count(bool _plus) {
		m_IsCountHold = true;
		m_IsPlus = _plus;
		m_HoldCounting = 0;
		m_HoldTimer[0] = 0f;
		m_HoldTimer[1] = 0.5f;
		if (!IS_Counting) {
			HoldEnd_Count();
		}
	}

	public void HoldEnd_Count() {
		if (!m_IsCountHold) return;
		m_IsCountHold = false;
	}
	public void Click_Count(int _count) {
		if (m_Action != null) return;
		if(m_ChangeCount[1] < m_ChangeCount[0] + _count && m_Mode == REQ_CHAGE_CHAR_PIECE.ChangeMode.CharPiece) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(1021), TDATA.GetItemTable(m_MatIdx).GetName()));
			return;
		}
		m_ChangeCount[0] += _count;
		m_ChangeCount[0] = Mathf.Clamp(m_ChangeCount[0], 0, m_ChangeCount[1]);
		if(m_Mode == LS_Web.REQ_CHAGE_CHAR_PIECE.ChangeMode.CharPiece) {//무기명 -> 목표
			m_UseCount = m_ChangeCount[0] * TDATA.GetItemTable(m_ChangeIdx).m_Value;
		}
		else {//목표 -> 무기명
			m_UseCount = m_ChangeCount[0];//Mathf.CeilToInt((float)m_ChangeCount[0] / (float)(TDATA.GetItemTable(m_MatIdx).m_Value * BaseValue.PERSONNELFILE_CHANGE_RATIO / 100));// m_ChangeCount[0] * TDATA.GetItemTable(m_MatIdx).m_Value * BaseValue.PERSONNELFILE_CHANGE_RATIO / 100;
		}
		m_SUI.GetCnt[0].text = string.Format("(-{0})", m_UseCount);
		m_SUI.GetCnt[1].text = string.Format("(+{0})", m_ChangeCount[0] * m_Mul);
		m_SUI.Slider.value = m_ChangeCount[0];
	}
	public void Click_Max() {
		if (m_Action != null) return;
		Click_Count(m_ChangeCount[1] - m_ChangeCount[0]);
	}
	public void Slider_Count() {
		int diff = (int)m_SUI.Slider.value - m_ChangeCount[0];
		Click_Count(diff);
	}
	public void Click_Change() {
		if (m_Action != null) return;
		if (m_UseCount < 1 || m_ChangeCount[0] < 1) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(m_Mode == LS_Web.REQ_CHAGE_CHAR_PIECE.ChangeMode.CharPiece ? 896 : 897));
			return;
		}

#if NOT_USE_NET
		USERINFO.DeleteItem(m_MatIdx, m_UseCount);
		ItemInfo reward = USERINFO.InsertItem(m_ChangeIdx, m_ChangeCount[0] * m_Mul);
		MAIN.Save_UserInfo();
		m_Reward.Add(new RES_REWARD_ITEM() {
			Type = Res_RewardType.Item,
			UID = reward.m_Uid,
			Idx = reward.m_Idx,
			Cnt = m_ChangeCount[0] * m_Mul
		});
		MAIN.SetRewardList(new object[] { m_Reward }, () => { Close(1); });
#else
		WEB.SEND_REQ_CHAGE_CHAR_PIECE((res) => {
			if (!res.IsSuccess()) {
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			m_Reward.AddRange(res.GetRewards());
			MAIN.SetRewardList(new object[] { m_Reward }, () => { Close(1); });
		}, m_Mode, m_Mode == REQ_CHAGE_CHAR_PIECE.ChangeMode.NormalPiece ? m_MatIdx : m_ChangeIdx, m_Mode == REQ_CHAGE_CHAR_PIECE.ChangeMode.NormalPiece ? m_UseCount : m_ChangeCount[0]);
#endif
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int Result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(Result);
	}
}
