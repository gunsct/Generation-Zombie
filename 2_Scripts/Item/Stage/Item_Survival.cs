using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Survival : ObjMng
{

	[System.Serializable]
	public struct StatPosRot
	{
		public Vector3[] Pos;
		public Vector3[] Rot;
		public Vector3[] TxtRot;
	}
	[System.Serializable]
	public struct SUI
	{
		public Item_SurvStat[] SurvStat; //men, hyg, sat
		public StatPosRot[] StatPosRot; //사용 스탯 수에 따른 UI 위치와 회전
	}
	[SerializeField]
	SUI m_SUI;
	List<Transform> m_UseStat = new List<Transform>();
	public GameObject StatObj(StatType _type) { return m_SUI.SurvStat[(int)_type].gameObject; }
	private void Awake() {
		for(int i = 0; i < m_SUI.SurvStat.Length; i++) {
			m_SUI.SurvStat[i].SetUI((StatType)i);
			m_UseStat.Add(m_SUI.SurvStat[i].transform);
		}
		if (MainMng.IsValid()) {
			DLGTINFO.f_RfStatUI += SetSuvStateEach;
		}
	}
	private void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RfStatUI -= SetSuvStateEach;
		}
	}
	/// <summary> 위치 회전 세팅 </summary>
	public void SetTrans() {
		for (int i = 0; i < m_UseStat.Count; i++) {
			m_UseStat[i].localPosition = m_SUI.StatPosRot[m_UseStat.Count - 1].Pos[i];
			m_UseStat[i].localEulerAngles = m_SUI.StatPosRot[m_UseStat.Count - 1].Rot[i];
			m_UseStat[i].GetComponent<Item_SurvStat>().m_Amount.transform.localEulerAngles = m_SUI.StatPosRot[m_UseStat.Count - 1].TxtRot[i];
			m_UseStat[i].GetComponent<Item_SurvStat>().m_ValUpDown.transform.localEulerAngles = -m_SUI.StatPosRot[m_UseStat.Count - 1].Rot[i];
		}
	}
	/// <summary> 미사용 스탯 꺼둠 </summary>
	public void StatOff(int _pos) {
		m_SUI.SurvStat[_pos].gameObject.SetActive(false);
		m_UseStat.Remove(m_SUI.SurvStat[_pos].transform);
	}
	/// <summary> 특정 스탯 갱신, 타입/이전값/현재값/최대값 </summary>
	public void SetSuvStateEach(StatType _type, float _crntval, float _preval, float _maxval) {
		if (gameObject.activeInHierarchy && _preval != _crntval)
			m_SUI.SurvStat[(int)_type].RefreshData(_crntval, _preval, _maxval);
		else
			m_SUI.SurvStat[(int)_type].SetData(_crntval, _maxval);
	}
}
