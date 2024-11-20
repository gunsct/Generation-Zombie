using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Replay
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	DL_5091,
	Focus_ReplayBtn,
	DL_5092,
	Focus_ReplayList4,
	DL_5094,
	Focus_RewardList4,
	DL_5095,
	Focus_GoBtnList4,
	Focus_CloseBtn,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Replay()
	{
		TutoType_Replay type = GetTutoState<TutoType_Replay>();
		return false;
	}
	void PlayTuto_Replay(int no, object[] args)
	{
		TutoType_Replay type = (TutoType_Replay)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type) {
			case TutoType_Replay.StartTalk:
				POPUP.ShowTutoStartAction(1901, () => {
					Next();
				});
				break;
			case TutoType_Replay.DL_5091:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5091, () => { Next(); });
				break;
			case TutoType_Replay.Focus_ReplayBtn:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				obj = ((Main_Play)playui).GetReplayBtn();
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				ui.StartDlg(-1);
				ui.UIClone();
				break;
			case TutoType_Replay.DL_5092:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5092, () => { Next(); });
				break;
			case TutoType_Replay.Focus_ReplayList4:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetPopup();
				obj = playui.GetComponent<StgReplay>().GetStgList(3);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
				ui.StartDlg(-1);
				ui.UIClone();
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_Replay.DL_5094:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5094, () => { Next(); });
				break;
			case TutoType_Replay.Focus_RewardList4:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetPopup();
				obj = playui.GetComponent<StgReplay>().GetStgList(3).GetComponent<Item_StgReplay_Stage_Element>().GetRewardCard;
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
				ui.StartDlg(-1);
				ui.UIClone();
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_Replay.DL_5095:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5095, () => { Next(); });
				break;
			case TutoType_Replay.Focus_GoBtnList4:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetPopup();
				obj = playui.GetComponent<StgReplay>().GetStgList(3).GetComponent<Item_StgReplay_Stage_Element>().GetGoBtn;
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
				ui.StartDlg(-1);
				ui.UIClone();
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_Replay.Focus_CloseBtn:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetPopup();
				obj = ((StgReplay)playui).GetCloseBtn;
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				ui.StartDlg(-1);
				ui.UIClone();
				break;
			case TutoType_Replay.End:
				SetTutoEnd();
				break;
		}
	}

	public bool TouchCheckLock_Replay(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Replay type = (TutoType_Replay)m_TutoState[m_NowTuto];
		switch (type) {
			case TutoType_Replay.Focus_ReplayBtn:
				if (checktype != TutoTouchCheckType.Play_Btn) return true;
				return  (int)args[0] != 10;
			case TutoType_Replay.Focus_CloseBtn:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return (PopupName)args[1] != PopupName.StgReplay;
		}
		return true;
	}
}
