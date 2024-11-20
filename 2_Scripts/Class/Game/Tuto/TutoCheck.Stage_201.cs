using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_201
{
	StageStart_Loading = 1,
	Focus_Reward,
	DL_1161,
	Delay_Reward,
	Focus_MissionGuide,
	DL_5201,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_201()
	{
		TutoType_Stage_201 type = GetTutoState<TutoType_Stage_201>();
		//switch (type) {
		//	case TutoType_Stage_201.Delay_Clear:
		//		return true;
		//}
		return true;
	}
	void PlayTuto_Stage_201(int no, object[] args)
	{
		TutoType_Stage_201 type = (TutoType_Stage_201)no;
		TutoUI ui;
		RectTransform rtf;
		//DeckSetting decksetting;

		switch (type) {
			case TutoType_Stage_201.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_201.Focus_Reward:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				Stage_StartReward mainui = POPUP.GetPopup().GetComponent<Stage_StartReward>(); 
				Vector3 panalpos = Utile_Class.GetCanvasPosition(mainui.GetPanel.transform.position);
				Rect rect = POPUP.GetComponent<RectTransform>().rect;
				ui.SetFocus(2, panalpos, rect.width, rect.height * 0.5f, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_Stage_201.DL_1161:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.StartDlg(1161, () => { Next(); });
				break;
			case TutoType_Stage_201.Delay_Reward:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_201.Focus_MissionGuide:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				ui.SetTutoFrame(false);
				Main_Stage stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetMissionGuideObj.transform;
				ui.SetFocus(2, rtf.position + new Vector3(-rtf.rect.width * 0.5f * rtf.lossyScale.x, -rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_Stage_201.DL_5201:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(5201, () => { Next(); });
				break;
			//case TutoType_Stage_201.Delay_Clear:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			case TutoType_Stage_201.End:
				SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Stage_201(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_201 type = GetTutoState<TutoType_Stage_201>();
		//Item_CharManageCard card;
		switch (type) {
			case TutoType_Stage_201.Delay_Reward:
				return false;
			case TutoType_Stage_201.StageStart_Loading: 
				return false;
			case TutoType_Stage_201.Focus_Reward:
			case TutoType_Stage_201.DL_1161:
				if (checktype != TutoTouchCheckType.StageCard) return false;
				return true;
		}
		return true;
	}
}
