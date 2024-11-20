using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_601
{
	StageStart_Loading = 1,
	DL_1191,
	Focus_SurvIcon,
	SelectStageCard_1,
	DL_1201,
	//Delay_Clear,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_601()
	{
		TutoType_Stage_601 type = GetTutoState<TutoType_Stage_601>();
		//switch (type) {
		//	case TutoType_Stage_601.Delay_Clear:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_Stage_601(int no, object[] args)
	{
		TutoType_Stage_601 type = (TutoType_Stage_601)no;
		TutoUI ui;
		RectTransform rtf;
		Item_Stage card;
		Vector3 pos, temp;

		switch (type)
		{
			case TutoType_Stage_601.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_601.DL_1191:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(1191, () => { Next(); });
				break;
			case TutoType_Stage_601.Focus_SurvIcon:
				Main_Stage mainui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				ui = POPUP.ShowTutoUI();
				rtf = (RectTransform)mainui.GetStatObj(StatType.Hyg).transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				mainui.TutoSrvStatOn(mainui.GetStatObj(StatType.Hyg), () => { Next(); }, 2f);
				break;
			case TutoType_Stage_601.SelectStageCard_1:
				ui = POPUP.ShowTutoUI();
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 3, temp.y - pos.y, false);
				Next();
				break;
			case TutoType_Stage_601.DL_1201:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.StartDlg(1201, () => { Next(); });
				break;
			//case TutoType_Stage_601.Delay_Clear:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			case TutoType_Stage_601.End:
				SetTutoEnd();
				Main_Stage stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.AccToggleCheck();
				break;
		}
	}

	public bool TouchCheckLock_Stage_601(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_601 type = GetTutoState<TutoType_Stage_601>();
		switch (type)
		{
			case TutoType_Stage_601.StageStart_Loading:
			//case TutoType_Stage_601.Delay_Clear:
				return false;
		}
		return true;
	}
}
