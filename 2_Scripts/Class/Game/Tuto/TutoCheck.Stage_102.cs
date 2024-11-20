using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_102
{
	DL_201 = 1,
	Focus_StartBtn,
	DL_202,
	Focus_GoStageBtn,
	StageStart_Loading,
	Focus_HPAP,
	FREE_Play,
	DL_208,
	Select_1_1031Char,
	Char1_1031_SkillEnd,
	Focus_HPAP2,
	Focus_1031_SkillInfo,

	//Focus_MissionGuide,
	//DL_103,
	//Focus_GoStageBtn,
	//StageStart_Loading = 1,
	//Focus_Line1,
	//FREE_Play_1,
	//Focus_APGauge,
	//DL_1091,
	//DL_1101,
	//Select_3Char,
	//Char3_SkillEnd,
	//DL_1111,
	//FREE_Play_2,
	//Focus_SkillInfoBtn,
	//DL_1114,
	//FREE_Play_3,
	//DL_1116,
	//Select_2Char,
	//Char2_SkillEnd,
	//DL_1118,
	//FREE_Play_4,
	//DL_1093,
	//Select_1Char,
	//Char1_SkillEnd,
	//DL_1095,
	//Delay_Clear,
	End
	//5->2->1

	//Focus_Accel,
	//Focus_Chars,
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_102()
	{
		TutoType_Stage_102 type = GetTutoState<TutoType_Stage_102>();
		switch (type) {
			case TutoType_Stage_102.FREE_Play:
			//case TutoType_Stage_102.Char3_SkillEnd:
			//case TutoType_Stage_102.Char1_SkillEnd:
			//case TutoType_Stage_102.FREE_Play_1:
			//case TutoType_Stage_102.FREE_Play_2:
			//case TutoType_Stage_102.FREE_Play_3:
			//case TutoType_Stage_102.FREE_Play_4:
			//case TutoType_Stage_102.Delay_Clear:
				return true;
		}
		return false;
	}
	void PlayTuto_Stage_102(int no, object[] args)
	{
		TutoType_Stage_102 type = (TutoType_Stage_102)no;
		TutoUI ui;
		RectTransform rtf;
		Item_Stage_Char charcard;
		//Item_Stage card;
		Vector3 pos, temp;
		Main_Stage stageui;
		DeckSetting decksetting;
		switch (type)
		{
			case TutoType_Stage_102.DL_201:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(201, () => { Next(); });
				break;
			case TutoType_Stage_102.Focus_StartBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				Main_Play playui = POPUP.GetMainUI().GetComponent<Main_Play>();
				rtf = (RectTransform)playui.GetStageStartBtn().transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_102.DL_202:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(202, () => { Next(); });
				break;
			case TutoType_Stage_102.Focus_GoStageBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetStartBtn().transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_102.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_102.Focus_HPAP:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.StartDlg(204, ()=> { Next(); });
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetAPGauge.transform;
				ui.SetFocus(2, rtf.position + new Vector3(0f, -rtf.rect.height * 0.5f, 0f), rtf.rect.width, rtf.rect.height * 2f, false);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.gameObject.SetActive(true);
				DLGTINFO?.f_RfAPUI?.Invoke(STAGE_USERINFO.m_AP[0], STAGE_USERINFO.m_AP[0], STAGE_USERINFO.m_AP[1]);
				stageui.GetAPGauge.transform.GetComponent<Animator>().SetTrigger("FirstIn");
				stageui.GetHpObj.SetActive(true);
				DLGTINFO?.f_RfHPUI?.Invoke(STAGE_USERINFO.GetStat(StatType.HP), STAGE_USERINFO.GetStat(StatType.HP), STAGE_USERINFO.GetMaxStat(StatType.HP));
				stageui.GetHpObj.transform.GetComponent<Animator>().SetTrigger("FirstIn");
				break;
			case TutoType_Stage_102.FREE_Play:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_102.DL_208:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(1f);
				ui.StartDlg(208, () => { Next(); });
				break;
			case TutoType_Stage_102.Select_1_1031Char:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				charcard = STAGE.m_Chars[1];
				pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
				Start_ClickDelay();
				break;
			case TutoType_Stage_102.Char1_1031_SkillEnd:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_102.Focus_HPAP2:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.StartDlg(210, () => { Next(); }, 0f, 1);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetAPGauge.transform;
				ui.SetFocus(2, rtf.position + new Vector3(0f, -rtf.rect.height * 0.5f, 0f), rtf.rect.width, rtf.rect.height * 2f, false);

				break;
			case TutoType_Stage_102.Focus_1031_SkillInfo:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				charcard = STAGE.m_Chars[1];
				GameObject infobtn = charcard.GetInfoBtnBG;
				pos = Utile_Class.GetCanvasPosition(infobtn.transform.position - charcard.SkillInfoSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(infobtn.transform.position + charcard.SkillInfoSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(infobtn.transform.position), temp.x - pos.x, temp.y - pos.y, false);
				ui.StartDlg(211, () => { 
					stageui.GetAPGauge.TuToApGaugeFullAction(0.5f); 
					Next(); 
				}, 0f, 1);
				break;
			//case TutoType_Stage_102.Focus_MissionGuide:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	rtf = (RectTransform)stageui.GetMissionGuideObj.transform;
			//	ui.SetFocus(2, rtf.position + new Vector3(-rtf.rect.width * 0.5f * rtf.lossyScale.x, -rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
			//	ui.StartDlg(204, () => { Next(); }, 0f, 1);
			//	break;
			//case TutoType_Stage_102.Focus_Line1:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	card = STAGE.m_ViewCard[0][1];
			//	pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 3, temp.y - pos.y, true);
			//	POPUP.StartTutoTimer(() => { Next(); }, 2f);
			//	break;
			//case TutoType_Stage_102.FREE_Play_1:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	stageui.GetAPGauge.TuToApGaugeFullAction(0.25f);
			//	break;
			//case TutoType_Stage_102.Focus_APGauge:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	stageui.GetAPGauge.TutoAPGaugeAction(50f / 234f, ()=> { Next(); });
			//	rtf = (RectTransform)stageui.GetAPGauge.transform;
			//	ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
			//	break;
			//case TutoType_Stage_102.DL_1091:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1091, () => { Next(); });
			//	break;
			//case TutoType_Stage_102.DL_1101:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1101, () => { Next(); });
			//	break;
			//case TutoType_Stage_102.Select_3Char:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	charcard = STAGE.m_Chars[2];
			//	pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
			//	Start_ClickDelay();
			//	break;
			//case TutoType_Stage_102.Char3_SkillEnd:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_102.DL_1111:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1111, () => { Next(); });
			//	break;
			//case TutoType_Stage_102.FREE_Play_2:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_102.Focus_SkillInfoBtn:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	charcard = STAGE.m_Chars[1];
			//	GameObject infobtn = charcard.GetInfoBtnBG;
			//	pos = Utile_Class.GetCanvasPosition(infobtn.transform.position - charcard.SkillInfoSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(infobtn.transform.position + charcard.SkillInfoSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(infobtn.transform.position), temp.x - pos.x, temp.y - pos.y, false);
			//	POPUP.StartTutoTimer(() => { Next(); }, 2f);
			//	break;
			//case TutoType_Stage_102.DL_1114:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1114, () => { Next(); });
			//	break;
			//case TutoType_Stage_102.FREE_Play_3:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_102.DL_1116:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1116, () => { Next(); });
			//	break;
			//case TutoType_Stage_102.Select_2Char:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	charcard = STAGE.m_Chars[1];
			//	pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
			//	Start_ClickDelay();
			//	break;
			//case TutoType_Stage_102.Char2_SkillEnd:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_102.DL_1118:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1118, () => { Next(); });
			//	break;
			//case TutoType_Stage_102.FREE_Play_4:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_102.DL_1093:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1093, () => { Next(); });
			//	break;
			//case TutoType_Stage_102.Select_1Char:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	charcard = STAGE.m_Chars[0];
			//	pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
			//	Start_ClickDelay();
			//	break;
			//case TutoType_Stage_102.Char1_SkillEnd:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_102.DL_1095:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1095, () => { Next(); });
			//	break;
			//case TutoType_Stage_102.Delay_Clear:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			case TutoType_Stage_102.End:
				SetTutoEnd();
				break;
		}
	}

	public bool TouchCheckLock_Stage_102(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_102 type = GetTutoState<TutoType_Stage_102>();
		Item_Stage_Char charcard;
		//Item_Stage stagecard;

		switch (type) {
			case TutoType_Stage_102.Focus_StartBtn:
				if (checktype != TutoTouchCheckType.Play_Btn) return true;
				return (int)args[0] != 1;
			case TutoType_Stage_102.Focus_GoStageBtn:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return (int)args[0] == 0;
			case TutoType_Stage_102.StageStart_Loading:
			case TutoType_Stage_102.FREE_Play:
				return false;
			case TutoType_Stage_102.Select_1_1031Char:
				if (checktype != TutoTouchCheckType.StageCard_Char) return true;
				charcard = (Item_Stage_Char)args[0];
				return charcard.m_Info != null && charcard.m_Info.m_Idx != 1031;
			case TutoType_Stage_102.Char1_1031_SkillEnd:
				if (checktype != TutoTouchCheckType.StageCard && checktype != TutoTouchCheckType.PopupOnClose) return true;
				if (checktype == TutoTouchCheckType.StageCard) return ((Item_Stage)args[0]).m_Info.m_TEnemyData == null;
				return false;
				//case TutoType_Stage_102.FREE_Play_1:
				//case TutoType_Stage_102.FREE_Play_2:
				//case TutoType_Stage_102.FREE_Play_3:
				//case TutoType_Stage_102.FREE_Play_4:
				////case TutoType_Stage_102.Delay_Clear:
				//	return false;
				//case TutoType_Stage_102.Select_2Char:
				//	if (checktype != TutoTouchCheckType.StageCard_Char) return true;
				//	charcard = (Item_Stage_Char)args[0];
				//	return charcard.m_Info != null && charcard.m_Info.m_Idx != 1003;
				//case TutoType_Stage_102.Select_3Char:
				//	if (checktype != TutoTouchCheckType.StageCard_Char) return true;
				//	charcard = (Item_Stage_Char)args[0];
				//	return charcard.m_Info != null && charcard.m_Info.m_Idx != 1056;
				//case TutoType_Stage_102.Select_1Char:
				//	if (checktype != TutoTouchCheckType.StageCard_Char) return true;
				//	charcard = (Item_Stage_Char)args[0];
				//	return charcard.m_Info != null && charcard.m_Info.m_Idx != 1001;
				//case TutoType_Stage_102.Char3_SkillEnd:
				//case TutoType_Stage_102.Char1_SkillEnd:
				//	if (checktype != TutoTouchCheckType.StageCard && checktype != TutoTouchCheckType.PopupOnClose) return true;
				//	if (checktype == TutoTouchCheckType.StageCard) return ((Item_Stage)args[0]).m_Info.m_TEnemyData == null;
				//	return false;
				//case TutoType_Stage_102.Char2_SkillEnd:
				//	if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				//	return false;

				//case TutoType_Stage_102.Focus_Accel:
				//	if (checktype != TutoTouchCheckType.Play_Btn) return true;
				//	return (int)args[0] != 0;
		}
		return true;
	}
}
