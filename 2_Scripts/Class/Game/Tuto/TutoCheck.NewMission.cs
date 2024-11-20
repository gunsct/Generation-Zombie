using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_NewMission
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	DL_5004,
	DL_5005,
	Focus_MissionBtn,
	DL_5006,
	Focus_MissionList,
	DL_5007,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_NewMission()
	{
		TutoType_NewMission type = GetTutoState<TutoType_NewMission>();
		return false;
	}
	void PlayTuto_NewMission(int no, object[] args)
	{
		TutoType_NewMission type = (TutoType_NewMission)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type) {
			case TutoType_NewMission.StartTalk:
				POPUP.ShowTutoStartAction(5017, () => {
					Next();
				});
				break;
			case TutoType_NewMission.DL_5004:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5004, () => { Next(); });
				break;
			case TutoType_NewMission.DL_5005:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5005, () => { Next(); });
				break;
			case TutoType_NewMission.Focus_MissionBtn:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				obj = ((Main_Play)playui).GetSideIcon(Main_Play.IconName.Mission);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				ui.StartDlg(-1);
				ui.UIClone();
				break;
			case TutoType_NewMission.DL_5006:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5006, () => { Next(); });
				break;
			case TutoType_NewMission.Focus_MissionList:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetPopup();
				obj = playui.GetComponent<NewNDailyMission>().GetPanel(1).GetComponent<Item_BeginnerQuest>().GetMissionList();
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
				ui.StartDlg(-1);
				ui.UIClone();
				Next();
				break;
			case TutoType_NewMission.DL_5007:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				//ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5007, () => { Next(); });
				break;
			case TutoType_NewMission.End:
				SetTutoEnd();
				break;
		}
	}

	public bool TouchCheckLock_NewMission(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_NewMission type = (TutoType_NewMission)m_TutoState[m_NowTuto];
		switch (type) {
			case TutoType_NewMission.Focus_MissionBtn:
				if (checktype != TutoTouchCheckType.Play_Btn && checktype != TutoTouchCheckType.MissionBtn) return true;
				return checktype == TutoTouchCheckType.Play_Btn ? (int)args[0] != 6 : (int)args[0] != 2;

		}
		return true;
	}
}
