using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Serum : PopupBase
{
	[Serializable]
	public struct LGSUI
	{
		public GameObject Group;
		public Animator Anim;
		public Image Portrait;
		public TextMeshProUGUI NeedTxt;//577
	}
	[Serializable]
	public struct LFSUI
	{
		public GameObject Group;
		public Animator Anim;
		public TextMeshProUGUI Name;
		public Item_RewardList_Item Item;
		public TextMeshProUGUI ItemCnt;
	}
	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image[] BlockNum;
		public Transform CellBucket;
		public ScrollRect Scroll;
		public Transform BlockTrans;
		public Button CloseBtn;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] LGSUI m_LGSUI;
	[SerializeField] LFSUI m_LFSUI;
	TSerumBlockTable m_TSBData { get { return TDATA.GetSerumBlockTable(m_BlockPos); } }
	CharInfo m_CharInfo;
	List<Item_Serum_Cell> m_Cells = new List<Item_Serum_Cell>();
	[SerializeField] List<Item_Serum_Cell> m_NowCells = new List<Item_Serum_Cell>();
	[SerializeField] int m_BlockPos = 1;

	IEnumerator m_Action; //end ani check
	IEnumerator m_LoopSND;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		//if (TUTO.IsTuto(TutoKind.Serum, (int)TutoType_Serum.Select_SerumBtn)) TUTO.Next();
		base.SetData(pos, popup, cb, aobjValue);
		m_CharInfo = (CharInfo)aobjValue[0];
		m_SUI.Scroll.verticalNormalizedPosition = 0f;
		m_SUI.Scroll.horizontalNormalizedPosition = 0.5f;

		for (int i = 0; i < m_SUI.CellBucket.childCount; i++)
			m_Cells.Add(m_SUI.CellBucket.GetChild(i).GetComponent<Item_Serum_Cell>());

		m_BlockPos = m_CharInfo.GetSerumBlockCnt();
		SetBlockFont(m_BlockPos);
		m_SUI.Anim.SetInteger("Depth", m_BlockPos % 5 == 0 ? 5 : m_BlockPos % 5);
		SetCells();
		SetLockBlock();

		PlayEffSound(SND_IDX.SFX_1080);
		m_LoopSND = LoopSND();
		StartCoroutine(m_LoopSND);
	}
	IEnumerator LoopSND() {
		PlayEffSound(SND_IDX.SFX_1081);
		yield return new WaitForSeconds(2.5f);

		if (m_LoopSND != null) StopCoroutine(m_LoopSND);
		m_LoopSND = LoopSND();
		StartCoroutine(m_LoopSND);
	}
	void SetCells() {
		List<TSerumTable> tables = TDATA.GetSerumTableGroup(m_CharInfo.m_TData.m_SerumGroupIdx, m_BlockPos);
		bool preopen = true;
		m_NowCells.Clear();

		for (int i = 0; i < m_Cells.Count; i++) {
			bool crntopen = m_CharInfo.m_Serum.Contains(tables[i].m_Idx);
			bool now = true;
			if (tables[i].m_PrecedIdx.Count == 0 && m_CharInfo.m_Serum.Contains(tables[i].m_Idx)) now = false;
			for (int j = 0; j < tables[i].m_PrecedIdx.Count; j++) {//못얻고 이전거 되있을때
				if (!m_CharInfo.m_Serum.Contains(tables[i].m_PrecedIdx[j]) || m_CharInfo.m_Serum.Contains(tables[i].m_Idx)) {
					now = false;
					break;
				}
			}
			if (now) m_NowCells.Add(m_Cells[i]);
			preopen = crntopen;
			Item_Serum_Cell cell = m_Cells[i];
			cell.SetData(tables[i].m_Idx, crntopen, now, m_BlockPos, m_CharInfo,
				()=> {
					m_SUI.CloseBtn.enabled = false;
					m_SUI.Anim.SetTrigger("PopUp");
					StartCoroutine(MoveNowCell(cell));
				},
				() => {
					m_SUI.CloseBtn.enabled = true;
					SetCells();
					CheckChangeBlock();
					m_SUI.Anim.SetTrigger("PopUpEnd");
				});
		}

		StartCoroutine(MoveNowCell());
	}

	/// <summary> now 중 아래 있는 셀쪽으로 버킷 이동</summary>
	IEnumerator MoveNowCell(Item_Serum_Cell _now = null) {
		Item_Serum_Cell now = _now;
		if (now == null) {
			for (int i = m_NowCells.Count - 1; i > -1 ; i--) {
				now = now == null ? m_NowCells[i] : now.transform.position.y <= m_NowCells[i].transform.position.y ? m_NowCells[i] : now;
			}
		}
		if (now == null) yield break;

		m_SUI.Scroll.enabled = false;
		float vertical = Mathf.Clamp((float)m_Cells.IndexOf(now) / (float)m_Cells.Count, 0f, 1f);
		float horizontal = 0f;
		if (now.transform.localPosition.x == 0) horizontal = 0.5f;
		else if(now.transform.localPosition.x > 0) horizontal = 1f;
		else if (now.transform.localPosition.x < 0) horizontal = 0f;
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.verticalNormalizedPosition, "to", vertical, "onupdate", "TW_ScrollMoveVer", "time", 1f, "name", "MoveBucketVer"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.horizontalNormalizedPosition, "to", horizontal, "onupdate", "TW_ScrollMoveHor", "time", 1f, "name", "MoveBucketHor"));

		yield return new WaitForSeconds(1f);

		iTween.StopByName(m_SUI.BlockTrans.gameObject, "MoveBucketVer");
		iTween.StopByName(m_SUI.BlockTrans.gameObject, "MoveBucketHor");
		m_SUI.Scroll.enabled = true;

		//if (TUTO.IsTuto(TutoKind.Serum, (int)TutoType_Serum.ViewSerum))
		//{
		//	yield return new WaitForSeconds(1f);
		//	TUTO.Next();
		//}
	}
	void TW_ScrollMoveVer(float _amount) {
		m_SUI.Scroll.verticalNormalizedPosition = _amount;
	}
	void TW_ScrollMoveHor(float _amount) {
		m_SUI.Scroll.horizontalNormalizedPosition = _amount;
	}
	int CheckBlock() {//블럭 바뀌는것 체크
		bool allget = true;
		int block = m_BlockPos;
		while (allget) {
			List<TSerumTable> tables = TDATA.GetSerumTableGroup(m_CharInfo.m_TData.m_SerumGroupIdx, block);
			for(int i = 0; i < tables.Count; i++) {
				if (!m_CharInfo.m_Serum.Contains(tables[i].m_Idx)) {
					allget = false;
					return block;
				}
			}
			if (allget) {
				block++;
				block = Mathf.Clamp(block, 1, 10);
			}
			if (allget && block == 10) break;
		}

		return block;
	}
	void SetBlockFont(int _block) {
		int block10 = Mathf.Clamp(_block / 10, 0, 9);
		int block1 = Mathf.Clamp(_block % 10, 0, 9);
		m_SUI.BlockNum[0].sprite = UTILE.LoadImg(string.Format("UI/UI_Serum/Font_Number_{0}", block10), "png");
		m_SUI.BlockNum[1].sprite = UTILE.LoadImg(string.Format("UI/UI_Serum/Font_Number_{0}", block1), "png");
	}
	[ContextMenu("CheckChangeBlock")]
	void CheckChangeBlock() {
		int crntblock = m_CharInfo.GetSerumBlockCnt();
		SetBlockFont(crntblock);
		if (m_BlockPos != crntblock) {
			m_BlockPos = crntblock;
			SetLockBlock();
			StartCoroutine(IE_ChangeBlockAction());
		}
	}
	IEnumerator IE_ChangeBlockAction() {
		m_SUI.Anim.SetTrigger("Change");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 134f/ 375f));

		SetCells();
		m_SUI.Anim.SetInteger("Depth", m_BlockPos % 5  == 0 ? 5 : m_BlockPos % 5);

		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
	}
	/// <summary> 현재 블럭이 잠겼는지 체크 </summary>
	void SetLockBlock() {
		if(m_CharInfo.m_LV >= m_TSBData.m_NeedCharLv) {
			m_LGSUI.Group.SetActive(false);
			bool matlock = m_BlockPos == m_CharInfo.m_LockSerumBlockPos && m_TSBData.m_NeedItemCnt > 0;
			m_LFSUI.Group.SetActive(matlock);
			if (matlock) {
				RES_REWARD_ITEM ritem;
				TItemTable titem = TDATA.GetItemTable(m_CharInfo.m_TData.m_PieceIdx);
				ritem = new RES_REWARD_ITEM();
				ritem.Type = Res_RewardType.Item;
				ritem.UID = 0;
				ritem.Idx = m_CharInfo.m_TData.m_PieceIdx;
				ritem.Cnt = m_TSBData.m_NeedItemCnt;
				m_LFSUI.Item.SetData(ritem, null, false);
				int getcnt = USERINFO.GetItemCount(m_CharInfo.m_TData.m_PieceIdx);
				m_LFSUI.ItemCnt.text = string.Format("<color={0}>{1}</color> / {2}", getcnt >= m_TSBData.m_NeedItemCnt ? "#498E41" : "#EA5757", getcnt, m_TSBData.m_NeedItemCnt);
				m_LFSUI.Name.text = titem.GetName();
			}
		}
		else {
			m_LFSUI.Group.SetActive(false);
			m_LGSUI.Group.SetActive(true);
			m_LGSUI.Portrait.sprite = m_CharInfo.m_TData.GetPortrait();
			m_LGSUI.NeedTxt.text = string.Format(TDATA.GetString(577), m_CharInfo.m_TData.GetCharName(), m_TSBData.m_NeedCharLv);
		}
		
	}
	public void ClickUnlockBlock() {
		if (m_Action != null) return;
		if (USERINFO.GetItemCount(m_CharInfo.m_TData.m_PieceIdx) < m_TSBData.m_NeedItemCnt) {
			POPUP.StartLackPop(m_CharInfo.m_TData.m_PieceIdx, true);
			return;
		}
#if NOT_USE_NET
		USERINFO.DeleteItem(m_CharInfo.m_TData.m_PieceIdx, m_TSBData.m_NeedItemCnt);
		m_CharInfo.m_LockSerumBlockPos = m_BlockPos + 1;
		MAIN.Save_UserInfo();
		StartCoroutine(CloseFileLock());
#else
		WEB.SEND_REQ_CHAR_SERUM_PAGE_OPEN((res) => {
			if (!res.IsSuccess()) return;
			StartCoroutine(CloseFileLock());
		}, m_CharInfo);
#endif
	}
	public void Click_Guide() {
		if (TUTO.IsTutoPlay()) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.SerumGuide);
	}
	IEnumerator CloseFileLock() {
		m_LFSUI.Anim.SetTrigger("Unlock");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_LFSUI.Anim));
		m_LFSUI.Group.SetActive(false);
	}
	public void ClickStatistics() {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Serum_Statistics)) return;
		m_SUI.Anim.SetTrigger("PopUp");
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Serum_Statistic, (result, obj) => {

			m_SUI.Anim.SetTrigger("PopUpEnd");
		}, m_CharInfo, m_BlockPos);
	}
	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		//if (m_LFSUI.Group.activeSelf) m_LFSUI.Anim.SetTrigger("Unlock");
		//if (m_LGSUI.Group.activeSelf) m_LGSUI.Anim.SetTrigger("Unlock");
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		StopCoroutine(m_LoopSND);
		SND.StopEff();
		base.Close(_result);
	}

	[ContextMenu("DELETETEST")]
	void DELETETEST() {
		m_CharInfo.m_Serum.Remove(10521020);
	}
}
