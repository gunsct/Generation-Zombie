using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gacha_Pickup : PopupBase
{
	public enum State
	{
		None,
		Hold,
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Transform Element;//Item_CharacterCard_GachaSelect//Item_PickUp_ListElement
		public Transform Bucket;
		public RectTransform ScrollTrans;
		public ScrollRect ScrollRect;
		public Item_PickUp_SlotElement[] Slots;
		public Item_SortingGroup SortingGroup;
		public GameObject[] TutoObj;//0:confirm
	}
	[SerializeField] SUI m_SUI;
	List<Item_PickUp_ListElement> m_AllChar = new List<Item_PickUp_ListElement>();
	public List<int> m_Chars;
	public List<int> m_PreChars;

	State m_State = State.None;
	[SerializeField] ETouchState m_TouchState;
	GraphicRaycaster m_GR;
	[SerializeField] float m_HoldTime = 0f;     //드래그드랍 조건 시간
	double m_PressTime;
	Vector3 m_TouchPoint;

	Item_PickUp_ListElement m_SelectChar;
	Item_PickUp_SlotElement m_SelectSoltChar;
	IEnumerator m_Action; //end ani check

	public GameObject GetFirstSlot() {
		return m_SUI.Slots[0].gameObject;
	}
	public GameObject GetList() {
		return m_SUI.ScrollRect.gameObject;
	}
	public GameObject GetConfirmBtn() {
		return m_SUI.TutoObj[0];
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_GR = POPUP.GetComponent<GraphicRaycaster>();
		m_Chars = USERINFO.GetGachaPickUp();
		m_PreChars.AddRange(m_PreChars);
		m_SUI.SortingGroup.SetData(SetSort, Item_SortingGroup.Mode.Normal);
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		SetChar();

		if (TUTO.IsTuto(TutoKind.PickupGacha, (int)TutoType_Pickup.Delay_TouchPickupBtn)) TUTO.Next();
		base.SetUI();
	}
	private void Update() {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
		if(Input.touchCount == 1)
#endif
		if (Input.GetMouseButton(0) && POPUP.GetPopup() == this) {
			if (m_State == State.None) {
				List<RaycastResult> results = new List<RaycastResult>();
				PointerEventData ped = new PointerEventData(null);
				ped.position = Input.mousePosition;
				m_GR.Raycast(ped, results);

				if (Mathf.Abs(m_SUI.ScrollRect.velocity.y) <= 0.1f) {
					for (int i = 0; i < results.Count; i++) {
						Item_PickUp_ListElement card = results[i].gameObject.GetComponentInParent<Item_PickUp_ListElement>();
						if (card != null && card.m_Idx != 0) {
							if (m_SelectChar == card) m_HoldTime += Time.deltaTime;
							else m_HoldTime = 0f;
							m_SelectChar = card;

							if (m_HoldTime > 0.5f) {//1초 이상 누르고 있으면 드래그 가능
								m_State = State.Hold;
								if (!TUTO.IsTutoPlay()) card.OpenDetail();
								AutoScrolling(m_SelectChar.transform);
								m_SelectChar = null;
								break;
							}
						}
						Item_PickUp_SlotElement slot = results[i].gameObject.GetComponentInParent<Item_PickUp_SlotElement>();
						if (slot != null && slot.m_Idx != 0) {
							if (m_SelectSoltChar == slot) m_HoldTime += Time.deltaTime;
							else m_HoldTime = 0f;
							m_SelectSoltChar = slot;

							if (m_HoldTime > 0.5f) {//1초 이상 누르고 있으면 드래그 가능
								m_State = State.Hold;
								if (!TUTO.IsTutoPlay()) slot.OpenDetail();
								break;
							}
						}
					}
				}
			}
		}
		else if (Input.GetMouseButtonUp(0)) {
			m_SUI.ScrollRect.enabled = true;
			m_HoldTime = 0f;
			m_State = State.None;
			m_SelectChar = null;
		}
	}
	void SetChar() {
		//선택슬롯
		for (int i = 0; i < 4; i++) {
			m_SUI.Slots[i].SetData(i, i < m_Chars.Count ? m_Chars[i] : 0, SetSlot);
		}
		//캐릭터 리스트
		List<TCharacterTable> tdatas = TDATA.GetAllCharacterInfos();
		UTILE.Load_Prefab_List(tdatas.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0;i< tdatas.Count; i++) {
			Item_PickUp_ListElement element = m_SUI.Bucket.GetChild(i).GetComponent<Item_PickUp_ListElement>();
			element.SetData(tdatas[i].m_Idx, m_Chars.Contains(tdatas[i].m_Idx), SetSlot);
			m_AllChar.Add(element);
		}
		SetSort();
	}
	void SetSort() {
		switch (m_SUI.SortingGroup.m_Condition) {
			//안잠겨있고 열리는 순서로
			case SortingType.Grade:
				int[] grade = new int[2];
				m_AllChar.Sort((Item_PickUp_ListElement befor, Item_PickUp_ListElement after) => {
					CharInfo info = USERINFO.m_Chars.Find(o => o.m_Idx == after.m_Idx);
					grade[0] = info == null ? after.m_TData.m_Grade : info.m_Grade; 
					info = USERINFO.m_Chars.Find(o => o.m_Idx == befor.m_Idx);
					grade[1] = info == null ? befor.m_TData.m_Grade : info.m_Grade;

					if (grade[0] != grade[1]) return grade[0].CompareTo(grade[1]);
					if (after.m_TData.m_SelectivePickupStage != befor.m_TData.m_SelectivePickupStage) {
						if(!m_SUI.SortingGroup.m_Ascending)
							return befor.m_TData.m_SelectivePickupStage.CompareTo(after.m_TData.m_SelectivePickupStage);
						else
							return after.m_TData.m_SelectivePickupStage.CompareTo(befor.m_TData.m_SelectivePickupStage);
					}
					return after.m_Idx.CompareTo(befor.m_Idx);
				});
				break;
			case SortingType.Job:
				m_AllChar.Sort((Item_PickUp_ListElement befor, Item_PickUp_ListElement after) => {
					if (after.m_TData.m_Job[0] != befor.m_TData.m_Job[0]) return after.m_TData.m_Job[0].CompareTo(befor.m_TData.m_Job[0]);
					if (after.m_TData.m_SelectivePickupStage != befor.m_TData.m_SelectivePickupStage) {
						if (!m_SUI.SortingGroup.m_Ascending)
							return befor.m_TData.m_SelectivePickupStage.CompareTo(after.m_TData.m_SelectivePickupStage);
						else
							return after.m_TData.m_SelectivePickupStage.CompareTo(befor.m_TData.m_SelectivePickupStage);
					}
						return after.m_Idx.CompareTo(befor.m_Idx);
				});
				break;
		}

		if (m_SUI.SortingGroup.m_Ascending) m_AllChar.Reverse();

		for (int i = 0; i < m_AllChar.Count; i++) {
			m_AllChar[i].transform.SetAsLastSibling();
		}
	}
	void SetSlot(Item_PickUp_ListElement _card) {
		if (TUTO.IsTutoPlay()) return;
		if (m_Chars.Contains(_card.m_Idx)) {
			for (int i = 0; i < 4; i++) {
				if (m_SUI.Slots[i].m_Idx == _card.m_Idx) {
					m_Chars.Remove(_card.m_Idx);
					m_SUI.Slots[i].SetData(0);
					_card.SwapSelect();
					break;
				}
			}
		}
		else {
			Item_PickUp_SlotElement empty = null;
			for (int i = 0; i < 4; i++) {
				if (m_SUI.Slots[i].m_Idx == 0 && !m_SUI.Slots[i].IS_Lock) {
					empty = m_SUI.Slots[i];
					break;
				}
			}
			if (empty != null) {
				if (_card.IS_Lock) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(7105));
				else {
					m_Chars.Add(_card.m_Idx);
					empty.SetData(_card.m_Idx);
					_card.SwapSelect();
				}
			}
			else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(7106));
		}
	}
	void SetSlot(Item_PickUp_SlotElement _card) {
		if (TUTO.IsTutoPlay()) return;
		if (_card.m_Idx != 0) {
			m_Chars.Remove(_card.m_Idx);
			m_AllChar.Find(o => o.m_Idx == _card.m_Idx).SwapSelect();
			_card.SetData(0);
		}
	}
	void AutoScrolling(Transform _trans) {
		float posy = 0f;

		float buckettop = m_SUI.ScrollTrans.position.y + m_SUI.ScrollTrans.rect.height / 2;
		float bucketbottom = m_SUI.ScrollTrans.position.y - m_SUI.ScrollTrans.rect.height / 2;

		float cardtop = _trans.position.y + _trans.GetComponent<RectTransform>().rect.height / 2 + 68;
		float cardbottom = _trans.position.y - _trans.GetComponent<RectTransform>().rect.height / 2 - 250;

		if (buckettop < cardtop) {//카드 위가 잘릴 경우
			posy = cardtop - buckettop;
			m_SUI.Bucket.localPosition -= new Vector3(0f, posy, 0f);
		}
		if (bucketbottom > cardbottom) {// 카드 아래가 잘릴 경우
			posy = bucketbottom - cardbottom;
			m_SUI.Bucket.localPosition += new Vector3(0f, posy, 0f);
		}
	}
	public void ClickConfirm() {
		if (TUTO.IsTutoPlay()) return;
		if (m_Chars.Count < BaseValue.GetSelectivePickupOpenCnt(USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx)) {
			POPUP.Set_MsgBox(PopupName.Msg_YN, string.Empty, TDATA.GetString(7104), (btn, obj) => {
				if ((EMsgBtn)btn == EMsgBtn.BTN_YES) {
					USERINFO.SetGachaPickUp(m_Chars);
					Close();
				}
			});
		}
		else {
			USERINFO.SetGachaPickUp(m_Chars);
			Close();
		}
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
