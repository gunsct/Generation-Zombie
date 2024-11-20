using UnityEngine;

public enum TutoType_Hard
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 1601 대화 </summary>
	DLG_1601,
	/// <summary> 나이트메어 버튼 클릭 </summary>
	Select_Hard,
	/// <summary> 1611 대화 </summary>
	DLG_1611,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Hard()
	{
		TutoType_Hard type = GetTutoState<TutoType_Hard>();
		return false;
	}
	void PlayTuto_Hard(int no, object[] args)
	{
		TutoType_Hard type = (TutoType_Hard)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type)
		{
		case TutoType_Hard.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_Hard.DLG_1601:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1601, () => { Next(); });
			break;
		case TutoType_Hard.Select_Hard:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetStageDiffBtn(StageDifficultyType.Hard);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, obj.transform, Vector3.zero, 0, 0, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_Hard.DLG_1611:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1611, () => { Next(); });
			break;
		case TutoType_Hard.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Hard(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Hard type = (TutoType_Hard)m_TutoState[m_NowTuto];
		switch (type)
		{
		case TutoType_Hard.Select_Hard:
			if (checktype != TutoTouchCheckType.StageDifficulty) return true;
			return (StageDifficultyType)args[0] != StageDifficultyType.Hard;
		}
		return true;
	}
}
