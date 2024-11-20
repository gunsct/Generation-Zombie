using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Serum_Cell : ObjMng { 
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image Icon;
		public Image TargetIcon;
		public Sprite[] TargetSprites;
		public TextMeshProUGUI Val;
		public GameObject CenterLine;
		public GameObject[] SideLine;
		public GameObject Btn;
		public Transform Panel;
	}
	[SerializeField] SUI m_SUI;

	int m_Idx;
	bool m_IsOpened;
	bool m_IsNow;
	int m_BlockNum;
	CharInfo m_CharInfo;
	Action m_OpenCB;
	Action m_CloseCB;
	TSerumTable m_TData { get { return TDATA.GetSerumTable(m_Idx); } }
	public void SetData(int _idx, bool _opened, bool _now, int _block, CharInfo _charinfo, Action _opencb, Action _closecb) {//열려
		m_Idx = _idx;
		m_IsOpened = _opened;
		m_IsNow = _now;
		m_OpenCB = _opencb;
		m_CharInfo = _charinfo;
		m_CloseCB = _closecb;
		m_BlockNum = _block;

		Sprite icon = UTILE.LoadImg(string.Format("UI/Icon/Icon_Char_Stat_{0}", (int)m_TData.m_Type), "png");
		m_SUI.Icon.sprite = icon;
		m_SUI.TargetIcon.sprite = m_SUI.TargetSprites[(int)m_TData.m_TargetType];
		if(m_TData.m_ValType == StatValType.Ratio)
			m_SUI.Val.text = string.Format("+ {0:0.##}%", m_TData.m_Val * 100f);
		else
			m_SUI.Val.text = string.Format("+ {0}", m_TData.m_Val);

		m_SUI.Anim.SetTrigger(string.Format("{0}{1}", m_TData.m_Color.ToString(), _opened ? string.Empty : "_De"));
		m_SUI.Anim.SetTrigger(m_TData.m_TargetType.ToString());
		m_SUI.Anim.SetTrigger(_opened ? "Actived" : "Deact");
		bool mat = USERINFO.GetItemCount(m_TData.m_Material) >= m_TData.m_MatCnt;
		bool money = USERINFO.m_Money >= m_TData.m_DollarCnt;
		m_SUI.Anim.SetTrigger(_now && mat && money? "Now" : "NotNow");

		Color color = _opened ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 0f, 0f, 60f / 255f);
		if (m_SUI.CenterLine != null) m_SUI.CenterLine.GetComponent<Image>().color = color;
		for(int i = 0; i < m_SUI.SideLine.Length; i++) {
			m_SUI.SideLine[i].GetComponent<Image>().color = color;
		}

		m_SUI.Panel.localEulerAngles = new Vector3(0f, 0f, UTILE.Get_Random(0f, 360f));
		//m_SUI.Btn.SetActive(!_opened);
	}
	public void ClickOpen()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Serum_Cell, m_Idx)) return;
		//오픈 팝업 연결, 재료 개수 
		if (m_OpenCB == null) return;
		m_OpenCB?.Invoke();
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Serum_Info, (result, obj) => {
			if (result > 0) {
				m_IsOpened = true;
				m_IsNow = false;
				SetData(m_Idx, m_IsOpened, m_IsNow, m_BlockNum, m_CharInfo, m_OpenCB, m_CloseCB);
			}
			m_CloseCB?.Invoke();
		}, m_TData, m_IsOpened, m_IsNow, m_BlockNum, m_CharInfo);
	}
}
