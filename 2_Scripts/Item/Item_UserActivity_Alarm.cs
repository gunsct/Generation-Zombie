using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_UserActivity_Alarm : ObjMng
{
    [System.Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Text Txt;
	}
	[SerializeField] SUI m_SUI;
	UserActivityType m_Type;
	RectTransform m_TxtRect;
	Action m_CB;
	public void SetData(Action _cb) {
		PlayEffSound(UTILE.Get_Random(0,2) > 0 ? SND_IDX.SFX_0160 : SND_IDX.SFX_0161);
		m_CB = _cb;
		m_SUI.Txt.text = string.Empty;

		// 큐방식으로 빼내기
		var msg = MAIN.GetSysMsg();
		if (msg == null)
			SetDummy();
		else {
			SetReal(msg);
			USERINFO.InsertSystemMsgInfo(msg);
		}
//#if NOT_USE_NET
//		SetDummy();
//#else
//		// 통신은 res를 받기전까지의 딜레이가 있기때문에 시스템 메세지같이 스테이지로 넘어가는 순간에도 호출되는 놈은 해주면 안됨
//		// 해당 UI는 로딩중에 제거가됨
//		// 문제가되는 프로세스
//		// 팝업 -> 스테이지 시작 -> 팝업 닫힘 (해당상황때문에 여기로 들어옴) -> 통신 시작 -> 스테이지 진입함 -> res 데이터 받음 -> 메세지를 띄우려하니 해당 UI는 제거됨 에러 -> 스테이지 완료시 Exception이 발생한 상태이때문에 작동 안함
//		// 추천 프로세스
//		// MainMng : 시스템 메세지를 큐방식으로 담아둔다.
//		// MainMng 또는 PlayMng 에서 특정 시간마다 SEND_REQ_SYSTEM_MSG를 호출해서 받아온다.
//		// 유저활동 알림 데이터가 너무 많으면 마지막 10개만 기억해도됨(단 시스템 메세지는 보여주어야함)
//		// PlayMng : 특정 시간마다 큐에 내용이 있을때 보여줌
//		// 내용이 없다면 더미를 사용함
//		WEB.SEND_REQ_SYSTEM_MSG((res) => {
//			if (!res.IsSuccess()) {
//				TW_TxtMoveEnd();
//				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
//				return;
//			}
//			if (res.Msgs.Count > 0) {
//				res.Msgs.Sort((RES_SYSTEM_MSG _before, RES_SYSTEM_MSG _after) => { return _before.Data.CompareTo(_after.Data); });
//				SetReal(res.Msgs[0]);
//				USERINFO.InsertSystemMsgInfo(res.Msgs[0]);
//			}
//			else {
//				SetDummy();
//			}
//		}, USERINFO.m_SysMsgInfo.UID);
//#endif
	}
	void SetReal(RES_SYSTEM_MSG _msg) {
		SystemMsg_Data_Item msg = JsonConvert.DeserializeObject<SystemMsg_Data_Item>(_msg.Data);
		switch (_msg.Type) {
			case SystemMsgType.Char:
				TCharacterTable chartable = TDATA.GetCharacterTable(msg.Idx);
				m_SUI.Txt.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 1100001001), msg.m_Name, msg.Grade, chartable.GetCharName());
				break;
			case SystemMsgType.MakingEQ:
				TItemTable itemtable = TDATA.GetItemTable(msg.Idx);
				m_SUI.Txt.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 1100001002), msg.m_Name, msg.Grade, itemtable.GetName());
				break;
			case SystemMsgType.Zombie:
				TZombieTable zombietable = TDATA.GetZombieTable(msg.Idx);
				m_SUI.Txt.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 1100001003), msg.m_Name, msg.Grade, zombietable.GetName());
				break;
		}
		StartCoroutine(Move());
	}
	void SetDummy() {
		////더미로 해야 할 경우
		List<int> probs = new List<int>();
		int probsum = 0;
		probsum += BaseValue.USERACTIVITY_CHAR_PROB;
		probs.Add(probsum);
		probsum += BaseValue.USERACTIVITY_SETEQUIP_PROB;
		probs.Add(probsum);
		probsum += BaseValue.USERACTIVITY_ZOMBIE_PROB;
		probs.Add(probsum);

		int rand = UTILE.Get_Random(0, probsum);
		for (int i = 0; i < probs.Count; i++) {
			if (probs[i] <= rand) {
				m_Type = (UserActivityType)i;
				break;
			}
		}

		string dummyname = TDATA.GetString(ToolData.StringTalbe.Etc, USERINFO.GetActivityDummyName());
		switch (m_Type) {
			case UserActivityType.GetChar:
				List<TCharacterTable> chartables = TDATA.GetAllCharacterInfos().FindAll(t => t.m_Grade >= 3);
				TCharacterTable chartable = chartables[UTILE.Get_Random(0, chartables.Count)];
				m_SUI.Txt.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 1100001001), dummyname, chartable.m_Grade, chartable.GetCharName());
				break;
			case UserActivityType.MakeSetEquip:
				List<TItemTable> itemtables = TDATA.GetAllItemIdxs().FindAll(t => t.m_Grade >= 5 && t.GetEquipType() != EquipType.End && t.m_Value != 0);
				TItemTable itemtable = itemtables[UTILE.Get_Random(0, itemtables.Count)];
				m_SUI.Txt.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 1100001002), dummyname, itemtable.m_Grade, itemtable.GetName());
				break;
			case UserActivityType.ZombieGradeUp:
				List<TZombieTable> zombietables = TDATA.GetAllZombieTable().FindAll(t => t.m_Grade >= 5);
				TZombieTable zombietable = zombietables[UTILE.Get_Random(0, zombietables.Count)];
				m_SUI.Txt.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 1100001003), dummyname, zombietable.m_Grade, zombietable.GetName());
				break;
		}

		StartCoroutine(Move());
	}
	IEnumerator Move() {
		yield return new WaitForEndOfFrame();
		m_TxtRect = m_SUI.Txt.GetComponent<RectTransform>();
		RectTransform Rect = GetComponent<RectTransform>();
		iTween.ValueTo(gameObject, iTween.Hash("from", m_TxtRect.rect.width, "to", -m_TxtRect.rect.width, "time", 6f, "onupdate", "TW_TxtMove", "oncomplete", "TW_TxtMoveEnd"));
	}
	void TW_TxtMove(float _amount) {
		m_TxtRect.localPosition = new Vector3(_amount, 0f, 0f);
	}
	void TW_TxtMoveEnd() {
		m_CB?.Invoke();
		gameObject.SetActive(false);
	}
}
