using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Union_Option_MarkEdit : PopupBase
{
	public enum Mode
	{
		/// <summary> 길드 생성 </summary>
		Create = 0,
		/// <summary> 길드 설정 </summary>
		Option
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Mark;
		public ScrollRect scroll;
		public Transform Element;//Item_Union_MarkElement
	}
	[SerializeField] SUI m_SUI;

	IEnumerator m_Action;
	public int m_MarkIdx = 1;
	List<TItemTable> m_Marks;
	List<Item_Union_MarkElement> m_Items = new List<Item_Union_MarkElement>();
	Mode m_Mode;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_Marks = TDATA.GetItemTypeGroupTable(ItemType.Guild_Mark);
		m_MarkIdx = (int)aobjValue[0];
		m_Mode = (Mode)aobjValue[1];
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();

		int Max = m_Marks.Count;
		UTILE.Load_Prefab_List(Max, m_SUI.scroll.content, m_SUI.Element);
		m_Items.Clear();
		GridLayoutGroup group = m_SUI.scroll.content.GetComponent<GridLayoutGroup>();
		int selectpos = 0;
		for (int i = 0;i< Max; i++) {
			int Idx = m_Marks[i].m_Idx;
			Item_Union_MarkElement item = m_SUI.scroll.content.GetChild(i).GetComponent<Item_Union_MarkElement>();
			m_Items.Add(item);
			item.SetData(Idx, SetMark);
			if (Idx == m_MarkIdx) selectpos = i;
		}
		SetMark(m_MarkIdx);
		StartCoroutine(MovePos(selectpos));
	}

	IEnumerator MovePos(int Pos)
	{
		m_SUI.scroll.velocity = Vector2.zero;
		yield return new WaitForEndOfFrame();
		RectTransform view = m_SUI.scroll.viewport;
		RectTransform cotent = m_SUI.scroll.content;
		RectTransform Item = (RectTransform)cotent.GetChild(Pos).transform;
		float y = (Item.anchoredPosition.y + (Item.rect.height * Item.pivot.y)) / (cotent.rect.height - Item.rect.height);
		// y값이 음수기때문에 1에서 빼줘야 자신의 위치
		float pos = 1f + y;

		// 마지막 연구쪽 라인을 찾아서 넣어주어야함
		m_SUI.scroll.verticalNormalizedPosition = pos;
	}

	void SetMark(int _idx) {
		if (_idx < 1) return;
		m_MarkIdx = _idx;
		m_SUI.Mark.sprite = TDATA.GetGuideMark(_idx);

		for (int i = 0; i < m_Items.Count; i++) m_Items[i].SetAnim(m_Items[i].m_Idx == m_MarkIdx);
	}

	public void Click_Info()
	{
		POPUP.Set_MsgBox(PopupName.Msg_Guide, string.Empty, TDATA.GetString(6033));
	}

	public void Click_Comfirm()
	{
		if(m_Mode == Mode.Option)
		{
			WEB.SEND_REQ_GUILD_INFO_CHANGE((res) =>
			{
				if (!res.IsSuccess())
				{
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					if (USERINFO.m_Guild.MyGrade() != GuildGrade.Master) Close((int)Union_JoinList.CloseResult.Grade);
					return;
				}
				Close(m_MarkIdx);
			}, LS_Web.GUILD_INFO_CHANGE_MODE.Icon, m_MarkIdx.ToString());
		}
		Close(m_MarkIdx);
	}

	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
