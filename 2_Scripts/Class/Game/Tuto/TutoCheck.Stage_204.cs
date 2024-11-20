using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_204
{
	Decksetting_Delay = 1,
	DL_2601,
	Focus_GoStageBtn,
	StageStart_Loading,
	FREE_Play_1,
	DL_122,
	Select_Char_1012,
	Char_1012_SkillEnd,
	Delay_Merge,
	Focus_Merge_Medicine,
	Delay_Mdeicine,
	DL_126,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_204()
	{
		TutoType_Stage_204 type = GetTutoState<TutoType_Stage_204>();
		//switch (type) {
		//	case TutoType_Stage_205.Delay_Clear:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_Stage_204(int no, object[] args)
	{
		TutoType_Stage_204 type = (TutoType_Stage_204)no;
		TutoUI ui;
		RectTransform rtf;
		Vector3 pos, temp;
		Main_Stage stageui;
		DeckSetting decksetting;
		Item_Stage_Char charcard;

		switch (type) {
			case TutoType_Stage_204.Decksetting_Delay:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_204.DL_2601:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2601, () => { Next(); });
				break;
			case TutoType_Stage_204.Focus_GoStageBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetStartBtn().transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				break;
			case TutoType_Stage_204.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_204.FREE_Play_1:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_204.DL_122:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(122, () => { Next(); });
				break;
			case TutoType_Stage_204.Select_Char_1012:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				charcard = STAGE.m_Chars[0];
				pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(0.5f);
				Start_ClickDelay();
				break;
			case TutoType_Stage_204.Char_1012_SkillEnd:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_204.Delay_Merge:
				POPUP.ShowTutoUI().RemoveFX();
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_204.Focus_Merge_Medicine:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetMaking.GetFirstMakeCard.transform;
				ui.SetFocus(2, rtf.position, rtf.rect.width, rtf.rect.height, true);
				break;
			case TutoType_Stage_204.Delay_Mdeicine:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_204.DL_126:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(126, () => { Next(); });
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(1f);
				charcard = STAGE.m_Chars[0];
				charcard.SkillColoTimeInit();
				break;
			case TutoType_Stage_204.End:
				SetTutoEnd();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.AccToggleCheck();
				break;
		}
	}

	public bool TouchCheckLock_Stage_204(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_204 type = GetTutoState<TutoType_Stage_204>();
		Item_Stage_Char charcard;
		switch (type) {
			case TutoType_Stage_204.Decksetting_Delay:
			case TutoType_Stage_204.StageStart_Loading:
			case TutoType_Stage_204.FREE_Play_1:
			case TutoType_Stage_204.Delay_Mdeicine:
				return false;
			case TutoType_Stage_204.Focus_GoStageBtn:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return (int)args[0] == 0;
			case TutoType_Stage_204.Select_Char_1012:
				if (checktype != TutoTouchCheckType.StageCard_Char) return true;
				charcard = (Item_Stage_Char)args[0];
				return charcard.m_Info != null && charcard.m_Info.m_Idx != 1012;
			case TutoType_Stage_204.Char_1012_SkillEnd:
				if (checktype != TutoTouchCheckType.StageCard && checktype != TutoTouchCheckType.PopupOnClose) return true;
				if (checktype == TutoTouchCheckType.StageCard) return ((Item_Stage)args[0]).m_Info.m_TEnemyData == null;
				return false;
			case TutoType_Stage_204.Focus_Merge_Medicine:
				if (checktype != TutoTouchCheckType.StageMaking) return true;
				return ((Item_Stage_MakeCard)args[0]).m_MatType != StageMaterialType.MedBottle;
		}
		return true;
	}
}
