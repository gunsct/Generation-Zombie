using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_105
{
	StageStart_Loading,
	FREE_Play_1,
	DL_501,
	ZoomOutIn,
	Focus_Line_0,
	Machingun_Action_End,
	DL_504,
	Select_Exit,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_105()
	{
		TutoType_Stage_105 type = GetTutoState<TutoType_Stage_105>();
		switch (type) {
			case TutoType_Stage_105.FREE_Play_1:
			case TutoType_Stage_105.Select_Exit:
				return true;
		}
		return false;
	}
	void PlayTuto_Stage_105(int no, object[] args)
	{
		TutoType_Stage_105 type = (TutoType_Stage_105)no;
		TutoUI ui;
		Item_Stage card;
		Vector3 pos, temp;
		List<GameObject> fxs = new List<GameObject>();

		switch (type)
		{
			case TutoType_Stage_105.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_105.FREE_Play_1:
				break;
			case TutoType_Stage_105.DL_501:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(501, () => { Next(); });
				break;
			case TutoType_Stage_105.ZoomOutIn:
				POPUP.RemoveTutoUI();
				STAGE.CamAction(StageMng.CamActionType.Zoom_Out_Up);
				PlayEffSound(SND_IDX.VOC_0114);
				POPUP.StartTutoTimer(() => {
					STAGE.CamAction(StageMng.CamActionType.Zoom_In_Down);
					POPUP.StartTutoTimer(() => {
						Next();
					}, 1f);
				}, 3f);
				break;
			case TutoType_Stage_105.Focus_Line_0:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 3, temp.y - pos.y, true);
				ui.StartDlg(503);
				break;
			case TutoType_Stage_105.Machingun_Action_End:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_105.DL_504:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(504, () => { Next(); });
				break;
			case TutoType_Stage_105.Select_Exit:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_105.End:
				SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Stage_105(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_105 type = GetTutoState<TutoType_Stage_105>();
		Item_Stage stagecard;
		switch (type) {
			case TutoType_Stage_105.StageStart_Loading:
			case TutoType_Stage_105.FREE_Play_1:
				return false;
			case TutoType_Stage_105.Focus_Line_0:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 0 || stagecard.m_Info.m_NowTData.m_Type != StageCardType.MachineGun;
			case TutoType_Stage_105.Select_Exit:
				return false;
			//	if (checktype == TutoTouchCheckType.StageCard) {
			//		stagecard = (Item_Stage)args[0];
			//		bool block = stagecard.m_Info.m_NowTData.m_Type != StageCardType.Exit;
			//		if (!STAGE.IS_SelectAction()) {
			//			if (!block) POPUP.RemoveTutoUI();
			//			else {
			//				POPUP.ShowTutoUI().SetFocus(0, Vector3.zero, 0, 0);
			//				POPUP.ShowTutoUI().SetTutoFrame(false);
			//				POPUP.ShowTutoUI().StartDlg(UTILE.Get_Random(506, 508), () => { POPUP.RemoveTutoUI(); });
			//			}
			//		}
			//		return block;
			//	}
			//	else {
			//		POPUP.ShowTutoUI().SetFocus(0, Vector3.zero, 0, 0);
			//		POPUP.ShowTutoUI().SetTutoFrame(false);
			//		POPUP.ShowTutoUI().StartDlg(UTILE.Get_Random(506, 508), ()=> { POPUP.RemoveTutoUI(); });
			//		return true;
			//	}
			case TutoType_Stage_105.Machingun_Action_End:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return false;
		}
		return true;
	}
}
