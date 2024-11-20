using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using System.Linq;

public class Item_ToolTip : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Transform m_Panel;
		public TextMeshProUGUI m_GroupName;
		public TextMeshProUGUI m_Name;
		public TextMeshProUGUI m_Desc;
		public TextMeshProUGUI m_HadCnt;
		public GameObject CountGroup;
	}
	[SerializeField]
	SUI m_SUI;
	int m_Idx;
	RES_REWARD_BASE m_Data;
	RectTransform m_Target;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Data = (RES_REWARD_BASE)aobjValue[0];
		m_Target = (RectTransform)aobjValue[1];

		m_SUI.CountGroup.SetActive(m_Data.Type == Res_RewardType.Item && TDATA.GetItemTable(m_Data.GetIdx()).m_Type == ItemType.Etc);
		base.SetData(pos, popup, cb, aobjValue);
	}

	void SetPosition()
	{
		if (m_Target == null) return;
		Canvas.ForceUpdateCanvases();
		RectTransform rtfMask = (RectTransform)transform;
		RectTransform rtfPanel = (RectTransform)m_SUI.m_Panel.transform;
		m_SUI.m_Panel.transform.position = m_Target.position;
		Vector2 v2Pos = rtfPanel.anchoredPosition;
		// 아이콘의 높이만큼 이동 고정 300 + 0.5 으로 셋팅 (UI 셋팅중 사이즈가 사라질수도있음)
		float h = rtfPanel.sizeDelta.y * 0.5f;
		float w = rtfPanel.sizeDelta.x * 0.5f;
		v2Pos.y += 150f;
		v2Pos.y += h;

		float t = rtfMask.rect.height * 0.5f;
		float r = rtfMask.rect.width * 0.5f;

		// 위치 조정
		if (v2Pos.y + h > t)
		{
			v2Pos = rtfPanel.anchoredPosition;
			v2Pos.y -= 150f;
			v2Pos.y -= h;
		}

		if (v2Pos.x + w > r) v2Pos.x = r - w;
		if (v2Pos.x - w < -r) v2Pos.x = -r + w;


		rtfPanel.anchoredPosition = v2Pos;
	}

	private void Update()
	{
		// 스크롤등 움직임 때문에 타겟 위치로 재셋팅
		SetPosition();
	}

	public override void SetUI() {
		int stack = 0;
		int grade = 1;
		switch (m_Data.Type) {
			case Res_RewardType.Item:
				var item = (RES_REWARD_ITEM)m_Data;
				var titem = TDATA.GetItemTable(item.Idx);
				switch (titem.m_Type)
				{
				case ItemType.Dollar: stack = (int)USERINFO.m_Money; break;
				case ItemType.Cash: stack = (int)USERINFO.m_Cash; break;
				case ItemType.Exp: stack = (int)USERINFO.m_Exp[1]; break;
				case ItemType.Energy: stack = (int)USERINFO.m_Energy.Cnt; break;
				default: stack = USERINFO.m_Items.FindAll(o => o.m_Idx == item.Idx).Sum(o => o.m_Stack); break;
				}

				// TODO FIX GRADE
				grade = item.Grade == 0 ? titem.m_Grade : item.Grade;
				m_SUI.m_GroupName.text = titem.GetGradeGroupName(grade);
				m_SUI.m_GroupName.color = BaseValue.GradeColor(grade);
				m_SUI.m_Name.text = titem.GetName();
				m_SUI.m_Desc.text = titem.GetInfo();
				m_SUI.m_HadCnt.text = string.Format("{0} {1}", TDATA.GetString(243), stack);
				break;
			case Res_RewardType.Char:
				var cinfo = (RES_REWARD_CHAR)m_Data;
				var tcinfo = TDATA.GetCharacterTable(cinfo.Idx);
				m_SUI.m_GroupName.text = string.Empty;
				m_SUI.m_Name.text = tcinfo.GetCharName();
				m_SUI.m_Desc.text = tcinfo.GetCharDesc();
				m_SUI.m_HadCnt.text = string.Format("{0} {1}", TDATA.GetString(243), USERINFO.m_Chars.Find((t) => t.m_Idx == m_Idx) == null ? 0 : 1);
				break;
			case Res_RewardType.DNA:
				var dna = (RES_REWARD_DNA)m_Data;
				var tdna = TDATA.GetDnaTable(dna.Idx);
				grade = tdna.m_Grade;
				m_SUI.m_GroupName.text = string.Empty;
				m_SUI.m_Name.text = tdna.GetName();
				m_SUI.m_Desc.text = tdna.GetDesc();
				m_SUI.m_HadCnt.text = string.Format("{0} {1}", TDATA.GetString(243), USERINFO.m_DNAs.FindAll((t) => t.m_Idx == dna.Idx && t.m_Grade == grade).Count);
				break;
			case Res_RewardType.Zombie:
				var zombie = (RES_REWARD_ZOMBIE)m_Data;
				var tzombie = TDATA.GetZombieTable(zombie.Idx);
				grade = zombie.Grade == 0 ? tzombie.m_Grade : zombie.Grade;
				m_SUI.m_GroupName.text = string.Empty;
				m_SUI.m_Name.text = tzombie.GetName();
				m_SUI.m_Desc.text = tzombie.GetDesc();// string.Empty;
				m_SUI.m_HadCnt.text = string.Format("{0} {1}", TDATA.GetString(243), USERINFO.m_Zombies.FindAll((t) => t.m_Idx == zombie.Idx && t.m_Grade == grade).Count);
				break;
		}

		SetPosition();
	}
}
