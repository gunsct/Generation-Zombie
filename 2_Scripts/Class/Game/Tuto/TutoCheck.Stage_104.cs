using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_104
{
	StageStart_Loading = 1,
	//Focus_MissionGuide,
	//Focus_Line_0_2_3_3B3,
	DL_403,
	Select_3_1029Char,
	Delay_SkillUse,
	Focus_Line_0_2_3,
	Char3_1029_SkillEnd,
	Focus_MissionGuide2,


	//Focus_Merge,
	//Focus_Line_0,
	//Delay_Merge,
	//Focus_Sniping_Merge,
	//Sniping_Action_Start,
	//SelectSnipingTarget,
	//Sniping_Action_End,
	//DL_406,
	//Delay_Clear,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_104()
	{
		TutoType_Stage_104 type = GetTutoState<TutoType_Stage_104>();
		//switch (type) {
		//	case TutoType_Stage_104.Delay_Clear:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_Stage_104(int no, object[] args)
	{
		TutoType_Stage_104 type = (TutoType_Stage_104)no;
		TutoUI ui;
		RectTransform rtf;
		Item_Stage card;
		Item_Stage_Char charcard;
		Vector3 pos, temp;
		Main_Stage stageui;
		List<GameObject> fxs = new List<GameObject>();

		switch (type)
		{
			case TutoType_Stage_104.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			//case TutoType_Stage_104.Focus_MissionGuide:
			//	ui = POPUP.ShowTutoUI();
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	rtf = (RectTransform)stageui.GetMissionGuideObj.transform;
			//	ui.SetFocus(2, rtf.position + new Vector3(-rtf.rect.width * 0.5f * rtf.lossyScale.x, -rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
			//	ui.StartDlg(401, () => { Next(); }, 0f, 1);
			//	break;
			//case TutoType_Stage_104.Focus_Line_0_2_3_3B3:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	card = STAGE.m_ViewCard[2][3];
			//	pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 3f, (temp.y - pos.y) * 3f, false);
			//	POPUP.StartTutoTimer(() => { Next(); }, 2f);
			//	break;
			case TutoType_Stage_104.DL_403:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(403, () => { Next(); });
				break;
			case TutoType_Stage_104.Select_3_1029Char:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				charcard = STAGE.m_Chars[3];
				pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(1f);
				Start_ClickDelay();
				break;
			case TutoType_Stage_104.Delay_SkillUse:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_104.Focus_Line_0_2_3:
				ui = POPUP.ShowTutoUI();
				card = STAGE.m_ViewCard[2][3];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
				break;
			case TutoType_Stage_104.Char3_1029_SkillEnd:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_104.Focus_MissionGuide2:
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(1f);
				charcard = STAGE.m_Chars[3];
				charcard.SkillColoTimeInit();
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);;
				rtf = (RectTransform)stageui.GetMissionGuideObj.transform;
				ui.SetFocus(2, rtf.position + new Vector3(-rtf.rect.width * 0.5f * rtf.lossyScale.x, -rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
				ui.StartDlg(405, () => { Next(); }, 0f, 1);
				break;




			//case TutoType_Stage_104.Focus_Merge:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	ui.SetTutoFrame(false);
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	stageui.GetMaking.TutoMergeTabAction(null);
			//	rtf = (RectTransform)stageui.GetMaking.transform;
			//	ui.SetFocus(2, rtf.position + new Vector3(0f, rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
			//	ui.StartDlg(401, () => { Next(); });
			//	Start_ClickDelay();
			//	break;
			//case TutoType_Stage_104.Focus_Line_0:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	card = STAGE.m_ViewCard[0][1];
			//	pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
			//	fxs = ui.SetFX(new List<Transform>() { STAGE.m_ViewCard[0][1].transform, STAGE.m_ViewCard[1][1].transform, STAGE.m_ViewCard[1][2].transform, STAGE.m_ViewCard[1][3].transform }, "Item/Tuto/Item_FX_Card_Tuto", Vector3.one * 4.45f);
			//	for (int i = 0; i < fxs.Count; i++) fxs[i].GetComponent<Animator>().SetTrigger(i == 0 ? "Able" : "Loop");
			//	ui.StartDlg(403, () => { 
			//		ui.RemoveFX();
			//	});
			//	break;
			//case TutoType_Stage_104.Delay_Merge:
			//	POPUP.ShowTutoUI().RemoveFX();
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_104.Focus_Sniping_Merge:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	rtf = (RectTransform)stageui.GetMaking.GetFirstMakeCard.transform;
			//	ui.SetFocus(2, rtf.position, rtf.rect.width, rtf.rect.height, true);
			//	ui.StartDlg(404);
			//	break;
			//case TutoType_Stage_104.Sniping_Action_Start:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_104.SelectSnipingTarget:
			//	ui = POPUP.ShowTutoUI();
			//	card = STAGE.m_ViewCard[1][2];
			//	pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
			//	ui.StartDlg(405);
			//	break;
			//case TutoType_Stage_104.Sniping_Action_End:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_104.DL_406:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(406, () => { Next(); });
			//	break;
			case TutoType_Stage_104.End:
				SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Stage_104(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_104 type = GetTutoState<TutoType_Stage_104>();
		Item_Stage stagecard;
		Item_Stage_Char charcard;
		switch (type) {
			case TutoType_Stage_104.StageStart_Loading:
				return false;
			case TutoType_Stage_104.Select_3_1029Char:
				if (checktype != TutoTouchCheckType.StageCard_Char) return true;
				charcard = (Item_Stage_Char)args[0];
				return charcard.m_Info != null && charcard.m_Info.m_Idx != 1029;
			case TutoType_Stage_104.Focus_Line_0_2_3:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 2 || stagecard.m_Pos != 3;
			case TutoType_Stage_104.Char3_1029_SkillEnd:
				if (checktype != TutoTouchCheckType.StageCard && checktype != TutoTouchCheckType.PopupOnClose) return true;
				if (checktype == TutoTouchCheckType.StageCard) return ((Item_Stage)args[0]).m_Info.m_TData.m_Type != StageCardType.Material;
				return false;


			//case TutoType_Stage_104.Focus_Line_0:
			//	if (checktype != TutoTouchCheckType.StageCard) return true;
			//	stagecard = (Item_Stage)args[0];
			//	return stagecard.m_Line != 0 || stagecard.m_Pos != 1;
			//case TutoType_Stage_104.Focus_Sniping_Merge:
			//	if (checktype != TutoTouchCheckType.StageMaking) return true;
			//	return ((Item_Stage_MakeCard)args[0]).m_MatType != StageMaterialType.Sniping;
			//case TutoType_Stage_104.SelectSnipingTarget:
			//	if (checktype != TutoTouchCheckType.StageCard) return true;
			//	stagecard = (Item_Stage)args[0];
			//	return stagecard.m_Line != 1 || stagecard.m_Pos != 2;
			//case TutoType_Stage_104.Sniping_Action_End:
			//	if (checktype != TutoTouchCheckType.PopupOnClose) return true;
			//	return false;
		}
		return true;
	}
}
