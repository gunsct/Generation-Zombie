using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_CharEquipProgress : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image Portrait;
		public TextMeshProUGUI Name;
		public GameObject ElementPrefab;
		public Transform Bucket;
		public GameObject[] Group;//0:진행중, 1:진행완료
		public TextMeshProUGUI MakeTimer;
		public Image TimerGauge;
	}
	[SerializeField] SUI m_SUI;
	List<Item_Mk_CharEquip> m_CharEqRewards = new List<Item_Mk_CharEquip>();
	MakingInfo m_Info;
	Action<Item_Mk_Element_Parent> m_CB;

	private void Update() {
		if (gameObject.activeSelf) {
			CharEqRewardScrolling();
			if (m_SUI.Group[0].activeSelf) {
				if (m_Info != null) m_SUI.TimerGauge.fillAmount = 1f - (float)m_Info.GetRemainTime() / (float)(m_Info.GetMaxTime() * 0.001d);
			}
		}
	}
	public void SetData(MakingInfo _info, Action<Item_Mk_Element_Parent> _cb) {
		m_Info = _info;
		m_CB = _cb;
		//charequipprogress 세팅
		TItemTable itemtable = TDATA.GetItemTable(_info.m_Idx);
		m_SUI.Portrait.sprite = itemtable.GetItemImg();
		m_SUI.Name.text = itemtable.GetName();

		m_CharEqRewards.Clear();
		List<RES_REWARD_BASE> resreward = TDATA.GetGachaItemList(TDATA.GetItemTable(_info.m_Idx));

		UTILE.Load_Prefab_List(resreward.Count, m_SUI.Bucket, m_SUI.ElementPrefab.transform);
		float startx = -((RectTransform)m_SUI.Bucket.transform).rect.xMax;
		float intervalx = ((RectTransform)m_SUI.Bucket.GetChild(0).transform).rect.xMax + 40f;
		for (int i = 0; i < m_SUI.Bucket.childCount; i++) {
			Item_Mk_CharEquip eq = m_SUI.Bucket.GetChild(i).GetComponent<Item_Mk_CharEquip>();
			eq.SetData(resreward[i].GetIdx());
			eq.transform.localPosition = new Vector3(startx + intervalx * i, 0f, 0f);
			m_CharEqRewards.Add(eq);
		}
		m_SUI.Group[0].SetActive(_info.GetRemainTime() > 0);
		m_SUI.Group[1].SetActive(_info.GetRemainTime() <= 0);
	}

	public void RefreshTimer() {
		if (!gameObject.activeSelf) return;
		if (!m_SUI.Group[0].activeSelf) return;
		if (m_Info == null) return;
		if (m_Info.m_State != TimeContentState.Play) return;

		m_SUI.MakeTimer.text = UTILE.GetSecToTimeStr(m_Info.GetRemainTime());
		float per = 1f - (float)m_Info.GetRemainTime() / (float)(m_Info.GetMaxTime() * 0.001d);
	}
	/// <summary> 전용장비 생산 화면 보상 자동 스크롤링 </summary>
	void CharEqRewardScrolling() {
		if (m_SUI.Bucket.childCount < 1) return;
		float startx = -((RectTransform)m_SUI.Bucket.transform).rect.xMax;
		float intervalx = ((RectTransform)m_SUI.Bucket.GetChild(0).transform).rect.xMax + 40f;
		for (int i = m_CharEqRewards.Count - 1; i >= 0; i--) {
			m_CharEqRewards[i].transform.position -= new Vector3(100f * Time.deltaTime, 0f, 0f);
			if (m_CharEqRewards[i].transform.localPosition.x < -((RectTransform)m_SUI.Bucket.transform).rect.xMax - ((RectTransform)m_CharEqRewards[i].transform).rect.xMax) {
				m_CharEqRewards[i].transform.localPosition += new Vector3(intervalx * (m_CharEqRewards.Count - 1), 0f, 0f);
			}
		}
	}
	/// <summary> 카드 선택시 제작 위해 인덱스 반환 </summary>
	public void ClickBtn() {
		m_CB?.Invoke(new Item_Mk_Element_Parent() { m_Mk_TData = m_Info.m_TData });
	}
}
