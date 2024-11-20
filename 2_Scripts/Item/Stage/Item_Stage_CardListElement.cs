using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Stage_CardListElement : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image[] Img;             //0:카드,1:모드
		public GameObject[] ImgGroup;	//0:카드,1:모드
		public TextMeshProUGUI Name;
		public TextMeshProUGUI[] Desc;
		public GameObject Alarm;
		public LayoutElement Layout;
	}
	[SerializeField] SUI m_SUI;
	TStageCardTable m_TData;
	public void SetData(int _idx, bool _new) {
		m_TData = TDATA.GetStageCardTable(_idx);
		m_SUI.Img[0].sprite = GetImg();
		m_SUI.ImgGroup[1].gameObject.SetActive(false);
		m_SUI.Layout.minHeight = 290f;
		m_SUI.Name.text = GetName();
		List<string> desc = GetDesc();
		for (int i = 0; i < m_SUI.Desc.Length; i++) {
			if (i < desc.Count)
				m_SUI.Desc[i].text = GetDesc()[i];
			else
				m_SUI.Desc[i].gameObject.SetActive(false);
		}
		m_SUI.Alarm.SetActive(_new);
	}
	public void SetData(Sprite _img, string _name, List<string> _desc, bool _new) {
		m_SUI.Img[1].sprite = _img;
		m_SUI.ImgGroup[0].gameObject.SetActive(false);
		m_SUI.Layout.minHeight = 230f;
		m_SUI.Name.text = _name;
		for (int i = 0; i < m_SUI.Desc.Length; i++) {
			if (i < _desc.Count)
				m_SUI.Desc[i].text = _desc[i];
			else
				m_SUI.Desc[i].gameObject.SetActive(false);
		}
		m_SUI.Alarm.SetActive(_new);
	}

	public Sprite GetImg() {
		Sprite re = null;
		string imgname = m_TData.m_Img;
		switch (m_TData.m_Type) {
			case StageCardType.Material:
				bool behind = STAGE_USERINFO.ISBuff(StageCardType.RandomMaterial);
				return behind ? UTILE.LoadImg("Card/Stage/Stage_0", "png") : TDATA.GetStageMaterialTable((StageMaterialType)Mathf.RoundToInt(m_TData.m_Value1)).GetStateCardImg();
		}
		if (string.IsNullOrWhiteSpace(imgname) && m_TData.m_Type == StageCardType.Enemy) imgname = TDATA.GetEnemyTable((int)m_TData.m_Value1).m_PrefabName;
		if (!string.IsNullOrWhiteSpace(imgname)) re = UTILE.LoadImg(imgname, "png");
		if (re == null) re = UTILE.LoadImg("Card/Stage/CardBack", "png");
		return re;
	}

	public string GetName() {
		switch (m_TData.m_Type) {
			case StageCardType.Material:
				int matcountdown = Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MaterialCountDown));
				int matcnt = Mathf.Max(Mathf.RoundToInt(m_TData.m_Value2) - matcountdown, 1);
				return string.Format("{0} X {1}", m_TData.GetName(), matcnt);
			case StageCardType.Drill:
				return m_TData.GetName(Mathf.RoundToInt(m_TData.m_Value1 == 0f ? 1 : m_TData.m_Value1 * 100f), Mathf.RoundToInt(m_TData.m_Value1 == 0f ? 1 : m_TData.m_Value1));
		}
		if (m_TData.m_Type == StageCardType.Enemy) return TDATA.GetEnemyTable((int)m_TData.m_Value1).GetName();
		return m_TData.GetName(Mathf.RoundToInt(m_TData.m_Value1 * 100f), Mathf.RoundToInt(m_TData.m_Value1));
	}
	public List<string> GetDesc() {
		List<string> desc = new List<string>();
		if (m_TData.m_Type == StageCardType.Enemy) {
			string info = m_TData.GetOnlyInfo();
			if (!string.IsNullOrEmpty(info)) desc.Add(info);
			desc.AddRange(TDATA.GetEnemyTable((int)m_TData.m_Value1).GetDescs(m_TData));
		}
		else desc.Add(m_TData.GetInfo());
		return desc;
	}
}
