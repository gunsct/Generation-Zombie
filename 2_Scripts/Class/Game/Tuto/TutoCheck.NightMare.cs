using UnityEngine;

public enum TutoType_Nightmare
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 1621 대화 </summary>
	DLG_1621,
	/// <summary> 아포칼립스 버튼 클릭 </summary>
	Select_Nightmare,
	/// <summary> 1631 대화 </summary>
	DLG_1631,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Nightmare()
	{
		TutoType_Nightmare type = GetTutoState<TutoType_Nightmare>();
		return false;
	}
	void PlayTuto_Nightmare(int no, object[] args)
	{
		TutoType_Nightmare type = (TutoType_Nightmare)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type)
		{
		case TutoType_Nightmare.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_Nightmare.DLG_1621:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1621, () => { Next(); });
			break;
		case TutoType_Nightmare.Select_Nightmare:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetStageDiffBtn(StageDifficultyType.Nightmare);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, obj.transform, Vector3.zero, 0, 0, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_Nightmare.DLG_1631:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1631, () => { Next(); });
			break;
		case TutoType_Nightmare.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Nightmare(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Nightmare type = (TutoType_Nightmare)m_TutoState[m_NowTuto];
		switch (type)
		{
		case TutoType_Nightmare.Select_Nightmare:
			if (checktype != TutoTouchCheckType.StageDifficulty) return true;
			return (StageDifficultyType)args[0] != StageDifficultyType.Nightmare;
		}
		return true;
	}
}
