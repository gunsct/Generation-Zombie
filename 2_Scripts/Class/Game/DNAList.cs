using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class DNAList : PopupBase
{
	[Serializable]
	public class Tab
	{
		public TextMeshProUGUI[] Name;
		public GameObject[] OnOff;
		public void SetBtn(bool _on) {
			OnOff[0].SetActive(_on);
			OnOff[1].SetActive(!_on);
		}
	}
    [Serializable]
    public struct SUI
    {
		public Animator Anim;
		public Tab[] TypeTabs;
		public Animator[] GradeTabs;
		public Transform Bucket;
		public Transform Element; // Item_Info_Char_DNAStat_Main
		public GameObject SubTitle;
	}
    [SerializeField] SUI m_SUI;
	DNABGType m_BGType = DNABGType.Red;
	int m_Grade = 1;
	PopupName m_PrePopup;
	List<TMakingTable> m_TDatas;
	GachaGroup m_GachaGroup;

	IEnumerator m_Action;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_PrePopup = (PopupName)aobjValue[0];
		m_SUI.SubTitle.SetActive(m_PrePopup == PopupName.DNAMaking);
		for (int i = 0; i < m_SUI.TypeTabs.Length; i++) {
			m_SUI.TypeTabs[i].Name[0].text = m_SUI.TypeTabs[i].Name[1].text = string.Format("{0} DNA", BaseValue.GetDNAColorName((DNABGType)(i + 1)));
		}
		m_TDatas = TDATA.GetGroupMakingTable(MakingGroup.DNA);
		m_TDatas.Sort((before, after) => {
			return before.m_ItemIdx.CompareTo(after.m_ItemIdx);
		});
		base.SetData(pos, popup, cb, aobjValue);
		ClickColorTab((int)DNABGType.Red);
		ClickGradeTab(1);
	}
	public override void SetUI() {
		base.SetUI();
		List<TDnaTable> tdatas = TDATA.GetAllDnaTable().FindAll(o => o.m_BGType == m_BGType && o.m_Grade == m_Grade);
		UTILE.Load_Prefab_List(tdatas.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0;i< tdatas.Count; i++) {
			Item_DNAList_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_DNAList_Element>();
			float makeper = 0f;
			if (m_PrePopup == PopupName.DNAMaking) {
				TMakingTable tdata = m_TDatas[(int)m_BGType - 1];
				m_GachaGroup = TDATA.GetGachaGroup(tdata.m_ItemIdx);
				TGachaGroupTable gachadata = m_GachaGroup.m_List.Find(o => o.m_RewardIdx == tdatas[i].m_Idx);
				makeper = (float)gachadata.m_Prob / (float)m_GachaGroup.m_TotalProb;
			}
			element.SetData(tdatas[i].m_Idx, makeper);
		}
	}
	/// <summary> 1~4의 색상 탭 </summary>
	public void ClickColorTab(int _type) {
		m_BGType = (DNABGType)_type;
		for(int i = 0; i < m_SUI.TypeTabs.Length; i++) m_SUI.TypeTabs[i].SetBtn(i == _type - 1);
		SetUI();
	}
	/// <summary> 1~5의 등급 탭</summary>
	public void ClickGradeTab(int _grade) {
		m_Grade = _grade;
		for (int i = 0; i < m_SUI.GradeTabs.Length; i++) m_SUI.GradeTabs[i].SetTrigger(i == _grade - 1 ? "Select" : "NotSelect");
		SetUI();
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
