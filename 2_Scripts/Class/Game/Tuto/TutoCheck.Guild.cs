using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Guild
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 5070 대화 </summary>
	DLG_5070,
	/// <summary> 길드 버튼 클릭 </summary>
	Select_Guild,
	/// <summary> 길드 연출 시작 연출 </summary>
	View_GuildList,
	/// <summary> 5072 대화 </summary>
	DLG_5072,
	/// <summary> 5075 대화 </summary>
	DLG_5075,
	/// <summary> 5077 대화 </summary>
	DLG_5077,
	/// <summary> 5079 대화 </summary>
	DLG_5079,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Guild()
	{
		TutoType_Guild type = GetTutoState<TutoType_Guild>();
		return false;
	}
	void PlayTuto_Guild(int no, object[] args)
	{
		TutoType_Guild type = (TutoType_Guild)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;

		switch (type)
		{
		case TutoType_Guild.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_Guild.DLG_5070:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(5070, () => { Next(); });
			break;
		case TutoType_Guild.Select_Guild:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetSideIcon(Main_Play.IconName.Union);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_Guild.View_GuildList:
			playui = POPUP.GetPopup();
			((Union_JoinList)playui).ScrollLock(true);
			((Union_JoinList)playui).InputLock(true);
			POPUP.RemoveTutoUI();
			break;
		case TutoType_Guild.DLG_5072:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(5072, () => { Next(); });
			break;
		case TutoType_Guild.DLG_5075:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Union_JoinList)playui).GetTutoObj(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, obj.transform, Vector3.zero, rtf.rect.width, rtf.rect.height, false);

			ui.StartDlg(5075, () => { Next(); });
			break;
		case TutoType_Guild.DLG_5077:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Union_JoinList)playui).GetTutoObj(1);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);

			ui.StartDlg(5077, () => { Next(); });
			break;
		case TutoType_Guild.DLG_5079:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(5079, () => { Next(); });
			break;
		case TutoType_Guild.End:
			playui = POPUP.GetPopup();
			((Union_JoinList)playui).ScrollLock(false);
			((Union_JoinList)playui).InputLock(false);
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Guild(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Guild type = (TutoType_Guild)m_TutoState[m_NowTuto];
		switch (type)
		{
		case TutoType_Guild.Select_Guild:
			if (checktype != TutoTouchCheckType.Play_Btn) return true;
			return (int)args[0] != 9;
		}
		return true;
	}
}
