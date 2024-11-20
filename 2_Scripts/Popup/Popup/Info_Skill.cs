using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Info_Skill : PopupBase
{
	[System.Serializable]
	public struct SUI
	{
		public Item_Skill_Card SkillCard;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI TypeName;
		public TextMeshProUGUI Desc;
		public GameObject UpgradeGroup;
		public Transform InfoGroup;
		public GameObject LvUpFX;
		public Item_MatItem_Card[] NeedMatCard;
		public TextMeshProUGUI[] MoneyCount;
		public GameObject UpgradeBtn;
	}
	[SerializeField]
	SUI m_SUI;

	TSkillTable m_TData { get { return TDATA.GetSkill(m_Idx); } }

	TSkillGrowthTable m_TDataGrowth { get { return TDATA.GetSkillGrowthTable(m_TData.m_Grade, m_Lv); } }
	int m_Idx;
	int m_Lv;
	bool m_Lock;
	CharInfo m_Char;
	   
	/// <summary> aobjValue 0:스킬종류, 1:스킬레벨, 2:잠김 여부 </summary>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Idx = (int)aobjValue[0];
		m_Lv = (int)aobjValue[1];
		m_Lock = (bool)aobjValue[2];
		m_Char = (CharInfo)aobjValue[3];

		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI()
	{
		SkillType type = m_Char.GetSkillType(m_Idx);
		if (type == SkillType.Active && m_Char.IS_SetEquip()) type = SkillType.SetActive;
		m_SUI.SkillCard.SetData(m_Idx, m_Lv, false, type == SkillType.SetActive);
		m_SUI.Name.text = TDATA.GetSkill(m_Idx).GetName();
		m_SUI.TypeName.text = TDATA.GetSkill(m_Idx).GetTypeName();
		m_SUI.Desc.text = TDATA.GetSkill(m_Idx).GetInfo(m_Lv);

		m_SUI.UpgradeGroup.SetActive(!m_Lock);
		if (!m_Lock)
		{
			SetNeedMat();
			m_SUI.UpgradeBtn.SetActive(CheckUpgrade());
		}
	}

	/// <summary> 필요 재료, 돈 갱신 </summary>
	void SetNeedMat() {
		if (m_TDataGrowth != null) {
			for (int i = 0; i < m_SUI.NeedMatCard.Length; i++) {
				if (i < m_TDataGrowth.m_NeedMaterial.Count) {
					m_SUI.NeedMatCard[i].SetData(m_TDataGrowth.m_NeedMaterial[i].m_Idx, m_TDataGrowth.m_NeedMaterial[i].m_Cnt);
					m_SUI.NeedMatCard[i].gameObject.SetActive(true);
				}
				else m_SUI.NeedMatCard[i].gameObject.SetActive(false);
			}
			m_SUI.MoneyCount[0].text = USERINFO.m_Money.ToString();
			m_SUI.MoneyCount[1].text = m_TDataGrowth.m_NeedMoney.ToString();
			m_SUI.MoneyCount[0].color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, m_TDataGrowth.m_NeedMoney);
		}
	}
	/// <summary> 업그레이드에 재료, 돈 충분한지 체크</summary>
	bool CheckUpgrade() {
		if (m_TDataGrowth == null) return false;
		for (int i = 0; i < m_TDataGrowth.m_NeedMaterial.Count; i++) {
			if (USERINFO.GetItemCount(m_TDataGrowth.m_NeedMaterial[i].m_Idx) < m_TDataGrowth.m_NeedMaterial[i].m_Cnt) return false;
		}
		if (USERINFO.m_Money < m_TDataGrowth.m_NeedMoney) return false;
		return true;
	}
	/// <summary> 업그레이드 </summary>
	public void Upgrade()
	{
		SkillType type = m_Char.GetSkillType(m_Idx);
#if NOT_USE_NET
		PlayEffSound(SND_IDX.SFX_0111);
		GameObject fx = Utile_Class.Instantiate(m_SUI.LvUpFX, m_SUI.InfoGroup);
		fx.transform.localPosition = new Vector3(-425.75f, 146.3902f, 0f);

		//재료랑 돈 소모하고 갱신
		for (int i = 0; i < m_TDataGrowth.m_NeedMaterial.Count; i++) {
			USERINFO.DeleteItem(m_TDataGrowth.m_NeedMaterial[i].m_Idx, m_TDataGrowth.m_NeedMaterial[i].m_Cnt);
		}
		USERINFO.ChangeMoney(-m_TDataGrowth.m_NeedMoney);
		//레벨업
		m_Lv++;
		m_Char.SkillLVUP(type);
		//레벨 제한아니면 UI 갱신
		if (m_Lv >= m_Char.GetSkillMaxLV(type)) m_Lock = true;
		SetUI();
#else
		WEB.SEND_REQ_CHAR_SKILL_LVUP((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					WEB.SEND_REQ_CHARINFO((res2) => {
						SetUI();
					}, USERINFO.m_UID, m_Char.m_UID);
				});
				return;
			}
			m_Lv = m_Char.m_Skill[(int)type].m_LV;
			//레벨 제한아니면 UI 갱신
			if (m_Lv >= m_Char.GetSkillMaxLV(type)) m_Lock = true;
			SetUI();
		}, m_Char.m_UID, type, 1);
#endif
	}
}
