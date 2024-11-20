using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_103
{

	DL_301 = 1,
	Focus_StartBtn,
	Decksetting_Delay,
	DL_302,
	Focus_CharListOnBtn,
	Select_FirstChar1052,
	Focus_CharListOffBtn,
	Focus_GoStageBtn,
	StageStart_Loading,
	Focus_Merge,
	DL_305,
	Focus_Line_0_0_1,
	Delay_Merge,
	Focus_Sniping_Merge,
	Sniping_Action_Start,
	SelectSnipingTarget,
	Sniping_Action_End,
	Focus_Line_0_All,
	DL_310,
	Select_2_1052Char,
	Char2_1052_SkillEnd,

	//Active_HPAP,
	//Select_1_1021Char,
	//Char1_1021_SkillEnd,
	//Focus_APGauge,
	//Focus_1031_SkillInfo,
	//Focus_MissionGuide,
	////Decksetting_Delay = 1,
	//DL_1121 = 1,
	//StageStart_Loading,
	//DL_1131,
	//Focus_Merge,
	//DL_1141,
	//GetMat_Delay,
	//Focus_Merge2,
	//DL_1151,
	//DL_1152,
	//FREE_Play_1,
	//DL_1153,
	////Delay_Clear,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_103()
	{
		TutoType_Stage_103 type = GetTutoState<TutoType_Stage_103>();
		//switch (type) {
		//	case TutoType_Stage_103.FREE_Play_1:
		//	case TutoType_Stage_103.Char1_1021_SkillEnd:
		//	return true;
		//}
		return false;
	}
	void PlayTuto_Stage_103(int no, object[] args)
	{
		TutoType_Stage_103 type = (TutoType_Stage_103)no;
		TutoUI ui;
		RectTransform rtf;
		Item_Stage card;
		Vector3 pos, temp;
		Item_Stage_Char charcard;
		DeckSetting decksetting;
		Main_Stage stageui;
		List<GameObject> fxs = new List<GameObject>();

		switch (type) {
			case TutoType_Stage_103.DL_301:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(301, () => { Next(); });
				break;
			case TutoType_Stage_103.Focus_StartBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				Main_Play playui = POPUP.GetMainUI().GetComponent<Main_Play>();
				rtf = (RectTransform)playui.GetStageStartBtn().transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_103.Decksetting_Delay:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_103.DL_302:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(302, () => { Next(); });
				break;
			case TutoType_Stage_103.Focus_CharListOnBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetCharListOnBtn.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_103.Select_FirstChar1052:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetChar1052.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_103.Focus_CharListOffBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetCharListOffBtn.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_103.Focus_GoStageBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetStartBtn().transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_103.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_103.Focus_Merge:
				ui = POPUP.ShowTutoUI();
				ui.SetTutoFrame(false);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetMaking.TutoMergeTabAction(() => { ui.StartDlg(304, () => { Next(); }); });
				rtf = (RectTransform)stageui.GetMaking.transform;
				ui.SetFocus(2, rtf.position + new Vector3(0f, rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
				break;
			case TutoType_Stage_103.DL_305:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(305, () => { Next(); });
				break;
			case TutoType_Stage_103.Focus_Line_0_0_1:
				ui = POPUP.ShowTutoUI();
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, false);
				fxs = ui.SetFX(new List<Transform>() { STAGE.m_ViewCard[0][1].transform, STAGE.m_ViewCard[1][1].transform, STAGE.m_ViewCard[1][2].transform, STAGE.m_ViewCard[1][3].transform }, "Item/Tuto/Item_FX_Card_Tuto", Vector3.one * 4.45f);
				for (int i = 0; i < fxs.Count; i++) fxs[i].GetComponent<Animator>().SetTrigger(i == 0 ? "Able" : "Loop");
				POPUP.StartTutoTimer(() => {
					ui.RemoveFX();
				}, 2f);
				break;
			case TutoType_Stage_103.Delay_Merge:
				POPUP.ShowTutoUI().RemoveFX();
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_103.Focus_Sniping_Merge:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetMaking.GetFirstMakeCard.transform;
				ui.SetFocus(2, rtf.position, rtf.rect.width, rtf.rect.height, true);
				ui.StartDlg(307);
				break;
			case TutoType_Stage_103.Sniping_Action_Start:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_103.SelectSnipingTarget:
				ui = POPUP.ShowTutoUI();
				card = STAGE.m_ViewCard[1][2];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
				ui.StartDlg(308);
				break;
			case TutoType_Stage_103.Sniping_Action_End:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_103.Focus_Line_0_All:
				ui = POPUP.ShowTutoUI();
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 3f, temp.y - pos.y, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_Stage_103.DL_310:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(310, () => { Next(); });
				break;
			case TutoType_Stage_103.Select_2_1052Char:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				charcard = STAGE.m_Chars[2];
				pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(1f);
				Start_ClickDelay();
				break;
			case TutoType_Stage_103.Char2_1052_SkillEnd:
				POPUP.RemoveTutoUI();
				charcard = STAGE.m_Chars[1];
				charcard.SkillColoTimeInit();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(1f);
				break;

			//case TutoType_Stage_103.Active_HPAP:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(-1);
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	rtf = (RectTransform)stageui.GetAPGauge.transform;
			//	ui.SetFocus(2, rtf.position + new Vector3(0f, -rtf.rect.height * 0.5f, 0f), rtf.rect.width, rtf.rect.height * 2f, false);
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	stageui.GetAPGauge.gameObject.SetActive(true);
			//	DLGTINFO?.f_RfAPUI?.Invoke(STAGE_USERINFO.m_AP[0], STAGE_USERINFO.m_AP[0], STAGE_USERINFO.m_AP[1]);
			//	stageui.GetAPGauge.transform.GetComponent<Animator>().SetTrigger("FirstIn");
			//	stageui.GetHpObj.SetActive(true);
			//	DLGTINFO?.f_RfHPUI?.Invoke(STAGE_USERINFO.GetStat(StatType.HP), STAGE_USERINFO.GetStat(StatType.HP), STAGE_USERINFO.GetMaxStat(StatType.HP));
			//	stageui.GetHpObj.transform.GetComponent<Animator>().SetTrigger("FirstIn");
			//	POPUP.StartTutoTimer(() => {
			//		stageui.GetAPGauge.TuToApGaugeFullAction(0.5f);
			//	}, 0.5f); 
			//	POPUP.StartTutoTimer(() => {
			//		Next();
			//	}, 2f);
			//	break;
			//case TutoType_Stage_103.Select_1_1021Char:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	charcard = STAGE.m_Chars[2];
			//	pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
			//	Start_ClickDelay();
			//	break;
			//case TutoType_Stage_103.Char1_1021_SkillEnd:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_103.Focus_APGauge:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	rtf = (RectTransform)stageui.GetAPGauge.transform;
			//	ui.SetFocus(2, rtf.position, rtf.rect.width, rtf.rect.height, false);
			//	ui.StartDlg(308, () => { Next(); }, 0f, 1);
			//	break;
			//case TutoType_Stage_103.Focus_1031_SkillInfo:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	charcard = STAGE.m_Chars[1];
			//	GameObject infobtn = charcard.GetInfoBtnBG;
			//	pos = Utile_Class.GetCanvasPosition(infobtn.transform.position - charcard.SkillInfoSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(infobtn.transform.position + charcard.SkillInfoSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(infobtn.transform.position), temp.x - pos.x, temp.y - pos.y, false);
			//	ui.StartDlg(312, () => { Next(); }, 0f, 1);
			//	break;
			//case TutoType_Stage_103.Focus_MissionGuide:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	rtf = (RectTransform)stageui.GetMissionGuideObj.transform;
			//	ui.SetFocus(2, rtf.position + new Vector3(-rtf.rect.width * 0.5f * rtf.lossyScale.x, -rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
			//	ui.StartDlg(314, () => { Next(); }, 0f, 1);
			//	break;
			//case TutoType_Stage_103.DL_1121:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1121, () => { Next(); });
			//	break;
			//case TutoType_Stage_103.StageStart_Loading:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_103.DL_1131:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1131, () => { Next(); });
			//	break;
			//case TutoType_Stage_103.Focus_Merge:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	ui.SetTutoFrame(false);
			//	Main_Stage stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	stageui.GetMaking.TutoMergeTabAction(() => { Next(); });
			//	rtf = (RectTransform)stageui.GetMaking.transform;
			//	ui.SetFocus(2, rtf.position + new Vector3(0f, rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
			//	break;
			//case TutoType_Stage_103.DL_1141:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1141, () => { Next(); });
			//	break;
			//case TutoType_Stage_103.GetMat_Delay:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_103.Focus_Merge2:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	ui.SetTutoFrame(false);
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	rtf = (RectTransform)stageui.GetMaking.transform;
			//	ui.SetFocus(2, rtf.position + new Vector3(0f, rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
			//	POPUP.StartTutoTimer(() => { Next(); }, 2f);
			//	break;
			//case TutoType_Stage_103.DL_1151:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	rtf = (RectTransform)stageui.GetMaking.GetFirstMakeCard.transform;
			//	ui.SetFocus(2, rtf.position, rtf.rect.width, rtf.rect.height, false);
			//	ui.StartDlg(1151, () => { Next(); });
			//	break;
			//case TutoType_Stage_103.DL_1152:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1152, () => { Next(); });
			//	break;
			//case TutoType_Stage_103.FREE_Play_1:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_103.DL_1153:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetDrag(true);
			//	List<GameObject> fxs = null;
			//	POPUP.StartTutoTimer(() => {
			//		fxs = ui.SetFX(new List<Transform>() { STAGE.m_ViewCard[0][0].transform, STAGE.m_ViewCard[1][0].transform, STAGE.m_ViewCard[1][1].transform, STAGE.m_ViewCard[1][2].transform }, "Item/Tuto/Item_FX_Card_Tuto", Vector3.one * 4.45f);
			//		for (int i = 0; i < fxs.Count; i++) if (fxs[i] != null) fxs[i].GetComponent<Animator>().SetTrigger(i == 0 ? "Tuto_1" : "Tuto_2");
			//	}, 0.5f); 
			//	POPUP.StartTutoTimer(() => {
			//		ui.RemoveFX();
			//		fxs = ui.SetFX(new List<Transform>() { STAGE.m_ViewCard[0][1].transform, STAGE.m_ViewCard[1][1].transform, STAGE.m_ViewCard[1][2].transform, STAGE.m_ViewCard[1][3].transform }, "Item/Tuto/Item_FX_Card_Tuto", Vector3.one * 4.45f);
			//		for (int i = 0; i < fxs.Count; i++) if (fxs[i] != null) fxs[i].GetComponent<Animator>().SetTrigger(i == 0 ? "Tuto_1" : "Tuto_2");
			//	}, 1.5f); 
			//	POPUP.StartTutoTimer(() => {
			//		ui.RemoveFX();
			//		fxs = ui.SetFX(new List<Transform>() { STAGE.m_ViewCard[0][2].transform, STAGE.m_ViewCard[1][2].transform, STAGE.m_ViewCard[1][3].transform, STAGE.m_ViewCard[1][4].transform }, "Item/Tuto/Item_FX_Card_Tuto", Vector3.one * 4.45f);
			//		for (int i = 0; i < fxs.Count; i++) if(fxs[i] != null) fxs[i].GetComponent<Animator>().SetTrigger(i == 0 ? "Tuto_1" : "Tuto_2");
			//	}, 2.5f);
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(1153, () => { 
			//		ui.RemoveFX(); 
			//		Next(); 
			//	});
			//	break;
			//case TutoType_Stage_103.Delay_Clear:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			case TutoType_Stage_103.End:
				SetTutoEnd();
				break;
		}
	}

	public bool TouchCheckLock_Stage_103(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_103 type = GetTutoState<TutoType_Stage_103>();
		Item_CharManageCard card;
		Item_Stage_Char charcard;
		Item_Stage stagecard;
		switch (type) {
			case TutoType_Stage_103.Focus_StartBtn:
				if (checktype != TutoTouchCheckType.Play_Btn) return true;
				return (int)args[0] != 1;

			case TutoType_Stage_103.Focus_CharListOnBtn:
				if (checktype != TutoTouchCheckType.DeckSetting_ListPage) return true;
				return (int)args[0] != 0;
			case TutoType_Stage_103.Select_FirstChar1052:
				if (checktype != TutoTouchCheckType.Item_CharManageCard_Select) return true;
				if ((Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Click
					&& (Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Hold) return true;
				card = (Item_CharManageCard)args[1];
				return card?.m_Info?.m_Idx != 1052;
			case TutoType_Stage_103.Focus_CharListOffBtn:
				if (checktype != TutoTouchCheckType.DeckSetting_ListPage) return true;
				return (int)args[0] != 1;
			case TutoType_Stage_103.Focus_GoStageBtn:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return (int)args[0] == 0;
			case TutoType_Stage_103.Decksetting_Delay:
			case TutoType_Stage_103.StageStart_Loading:
			//case TutoType_Stage_103.Delay_Clear:
			//case TutoType_Stage_103.GetMat_Delay:
				return false;
			case TutoType_Stage_103.Focus_Line_0_0_1:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 0 || stagecard.m_Pos != 1;
			case TutoType_Stage_103.Focus_Sniping_Merge:
				if (checktype != TutoTouchCheckType.StageMaking) return true;
				return ((Item_Stage_MakeCard)args[0]).m_MatType != StageMaterialType.Sniping;
			case TutoType_Stage_103.SelectSnipingTarget:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 1 || stagecard.m_Pos != 2;
			case TutoType_Stage_103.Sniping_Action_End:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return false;
			case TutoType_Stage_103.Select_2_1052Char:
				if (checktype != TutoTouchCheckType.StageCard_Char) return true;
				charcard = (Item_Stage_Char)args[0];
				return charcard.m_Info != null && charcard.m_Info.m_Idx != 1052;
			case TutoType_Stage_103.Char2_1052_SkillEnd:
				if (checktype != TutoTouchCheckType.StageCard && checktype != TutoTouchCheckType.PopupOnClose) return true;
				if (checktype == TutoTouchCheckType.StageCard) return ((Item_Stage)args[0]).m_Info.m_TData.m_Type != StageCardType.Material;
				return false;
		}
		return true;
	}
}
