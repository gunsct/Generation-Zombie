using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PVP_Base_Upgrade_Info : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image Icon;
		public GameObject[] Group;
		public TextMeshProUGUI[] CntDesc;   //0:title, 1:val1, 2:val2
		public TextMeshProUGUI[] CntDesc2;  //0:title, 1:cntval1, 2:cntval2, 3:timeval1, 4:timeval2
		public GameObject[] Group2Obj;		//01:cnt, 23:time
	}
	[SerializeField] SUI m_SUI;
	CampBuildType m_Type;
	int m_Lv;
	int m_Pos;
	public void SetData(CampBuildType _type, int _lv, int _pos) {
		m_Type = _type;
		m_Lv = _lv;
		m_Pos = _pos;

		TItemTable itdata = TDATA.GetItemTable(BaseValue.CAMP_RES_IDX(m_Pos));
		m_SUI.Icon.sprite = itdata.GetItemImg();

		switch (m_Type) {
			case CampBuildType.Camp://약탈 비율, 최대
				m_SUI.Group2Obj[2].SetActive(false);
				m_SUI.Group2Obj[3].SetActive(false);
				TPVP_CampTable nowcdata = TDATA.GetTPVP_CampTable(m_Lv);
				TPVP_CampTable nextcdata = TDATA.GetTPVP_CampTable(m_Lv + 1);
				m_SUI.CntDesc[0].text = TDATA.GetString(6222);
				m_SUI.CntDesc[1].text = string.Format("{0:0.##}%", nowcdata.m_RatioCnt[m_Pos].Ratio * 100);
				m_SUI.CntDesc[2].text = string.Format("{0:0.##}%", nextcdata.m_RatioCnt[m_Pos].Ratio * 100);
				m_SUI.CntDesc2[0].text = TDATA.GetString(6223);
				m_SUI.CntDesc2[1].text = Utile_Class.CommaValue(nowcdata.m_RatioCnt[m_Pos].Cnt);
				m_SUI.CntDesc2[2].text = Utile_Class.CommaValue(nextcdata.m_RatioCnt[m_Pos].Cnt);
				break;
			case CampBuildType.Storage://보관 최대 수량, 약탈 최소 수량 10832
				m_SUI.Group2Obj[2].SetActive(false);
				m_SUI.Group2Obj[3].SetActive(false);
				m_SUI.Group[1].SetActive(false);
				TPVP_Camp_Storage nowsdata = TDATA.GetPVP_Camp_Storage(m_Lv);
				TPVP_Camp_Storage nextsdata = TDATA.GetPVP_Camp_Storage(m_Lv + 1);
				m_SUI.CntDesc[0].text = TDATA.GetString(10832);
				m_SUI.CntDesc[1].text = Utile_Class.CommaValue(nowsdata.m_SaveMat[m_Pos]);
				m_SUI.CntDesc[2].text = Utile_Class.CommaValue(nextsdata.m_SaveMat[m_Pos]);
				break;
			case CampBuildType.Resource:
				m_SUI.Group2Obj[0].SetActive(false);
				m_SUI.Group2Obj[1].SetActive(false);
				TPVP_Camp_Resource nowrdata = TDATA.GetPVP_Camp_Resource(m_Lv);
				TPVP_Camp_Resource nextrdata = TDATA.GetPVP_Camp_Resource(m_Lv + 1);
				m_SUI.CntDesc[0].text = TDATA.GetString(10834);
				m_SUI.CntDesc[1].text = Utile_Class.CommaValue(nowrdata.m_Mat[m_Pos].MakeCnt);
				m_SUI.CntDesc[2].text = Utile_Class.CommaValue(nextrdata.m_Mat[m_Pos].MakeCnt);
				m_SUI.CntDesc2[0].text = TDATA.GetString(10835);
				m_SUI.CntDesc2[3].text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.hh_mm_ss, nowrdata.m_Mat[m_Pos].NeedTime * 60);
				m_SUI.CntDesc2[4].text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.hh_mm_ss, nextrdata.m_Mat[m_Pos].NeedTime * 60);
				break;
		}
	}
}
